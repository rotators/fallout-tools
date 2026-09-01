using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

// Compatibility delegate retained for the editor integration.
public delegate void SwapEventHandler(object sender, TabsSwappedEventArgs e);

/// <summary>
/// A fully managed document-tab host. The tab strip owns layout, painting,
/// scrolling, hit-testing, closing, and reordering. TabPage instances are used
/// only as stable document/content identities; no native Win32 tab strip or
/// up-down scroller participates in input handling.
/// </summary>
public class DraggableTabControl : UserControl
{
    private readonly List<TabPage> m_Pages = new List<TabPage>();
    private readonly ManagedTabPageCollection m_TabPages;
    private readonly Panel m_ContentPanel;
    private readonly TabControl m_PageHost;
    private readonly ToolTip m_ToolTip;
    private bool m_PageHostChromeMeasured;
    private int m_PageHostChromeLeft;
    private int m_PageHostChromeTop;
    private int m_PageHostChromeRight;
    private int m_PageHostChromeBottom;
    private readonly HashSet<TabPage> m_ModifiedTabs = new HashSet<TabPage>();
    private readonly HashSet<TabPage> m_UntitledTabs = new HashSet<TabPage>();
    private readonly List<Rectangle> m_TabBounds = new List<Rectangle>();
    private readonly List<int> m_TabWidths = new List<int>();

    private int m_SelectedIndex = -1;
    private int m_FirstVisibleIndex;
    private int m_LastVisibleIndex = -1;
    private TabPage m_HotTab;
    private TabPage m_HotCloseTab;
    private TabPage m_ClosePressedTab;
    private Rectangle m_ClosePressedBounds;
    private TabPage m_DraggedTab;
    private TabPage m_SwapLockTarget;
    private Point m_DragStart;
    private int m_NavigationPressed;
    private Rectangle m_LeftNavigationBounds;
    private Rectangle m_RightNavigationBounds;
    private bool m_HasOverflow;
    private bool m_IsReordering;
    private bool m_ShowCloseButtons;
    private bool m_ShowDocumentStatusIcons;
    private bool m_ShowToolTips;
    private Size m_ItemSize = new Size(0, 26);
    private Point m_TabPadding = new Point(16, 3);
    private ImageList m_ImageList;

    public DraggableTabControl()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BackColor = SystemColors.Control;
        m_TabPages = new ManagedTabPageCollection(this);
        m_ContentPanel = new Panel {
            Margin = System.Windows.Forms.Padding.Empty,
            AutoScroll = false
        };
        m_PageHost = new TabControl {
            Appearance = TabAppearance.FlatButtons,
            SizeMode = TabSizeMode.Fixed,
            ItemSize = new Size(0, 1),
            Multiline = true,
            TabStop = false,
            Margin = System.Windows.Forms.Padding.Empty,
            Visible = false
        };
        m_ContentPanel.Controls.Add(m_PageHost);
        Controls.Add(m_ContentPanel);
        m_ContentPanel.SendToBack();
        m_ToolTip = new ToolTip();
    }

    protected override Control.ControlCollection CreateControlsInstance()
    {
        return new DraggableTabControlCollection(this);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ManagedTabPageCollection TabPages { get { return m_TabPages; } }

    [Browsable(false)]
    public int TabCount { get { return m_Pages.Count; } }

    [Category("Behavior")]
    [DefaultValue(-1)]
    public int SelectedIndex
    {
        get { return m_SelectedIndex; }
        set { SelectIndex(value, true); }
    }

    [Browsable(false)]
    public TabPage SelectedTab
    {
        get { return m_SelectedIndex >= 0 && m_SelectedIndex < m_Pages.Count ? m_Pages[m_SelectedIndex] : null; }
        set { SelectIndex(value == null ? -1 : m_Pages.IndexOf(value), true); }
    }

    [Category("Appearance")]
    [DefaultValue(false)]
    public bool ShowCloseButtons
    {
        get { return m_ShowCloseButtons; }
        set { if (m_ShowCloseButtons != value) { m_ShowCloseButtons = value; Invalidate(); } }
    }

    [Category("Appearance")]
    [DefaultValue(false)]
    public bool ShowDocumentStatusIcons
    {
        get { return m_ShowDocumentStatusIcons; }
        set { if (m_ShowDocumentStatusIcons != value) { m_ShowDocumentStatusIcons = value; Invalidate(); } }
    }

    [Category("Behavior")]
    [DefaultValue(false)]
    public bool ShowToolTips
    {
        get { return m_ShowToolTips; }
        set { m_ShowToolTips = value; if (!value) m_ToolTip.SetToolTip(this, null); }
    }

    [Category("Appearance")]
    public Size ItemSize
    {
        get { return m_ItemSize; }
        set { m_ItemSize = value; PerformLayout(); EnsureSelectedVisible(); Invalidate(); }
    }

    [Category("Appearance")]
    public new Point Padding
    {
        get { return m_TabPadding; }
        set { m_TabPadding = value; EnsureSelectedVisible(); Invalidate(); }
    }

    [Category("Appearance")]
    public TabSizeMode SizeMode { get; set; }

    [Category("Appearance")]
    public ImageList ImageList
    {
        get { return m_ImageList; }
        set { m_ImageList = value; Invalidate(); }
    }

    [Browsable(false)]
    public bool IsReordering { get { return m_IsReordering; } }

    public override Rectangle DisplayRectangle
    {
        get { return new Rectangle(0, HeaderHeight, ClientSize.Width, Math.Max(0, ClientSize.Height - HeaderHeight)); }
    }

    [Category("Action")]
    public event EventHandler<TabCloseRequestedEventArgs> TabCloseRequested;

    [Category("Action")]
    public event SwapEventHandler tabsSwapped;

    [Category("Action")]
    public event TabControlEventHandler Selected;

    private int HeaderHeight
    {
        get { return Math.Max(ScriptEditor.DpiHelper.Scale(this, 24), m_ItemSize.Height); }
    }

    private int NavigationButtonWidth
    {
        get { return ScriptEditor.DpiHelper.Scale(this, 20); }
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        Rectangle content = DisplayRectangle;
        m_ContentPanel.Bounds = content;
        LayoutPageHost();
    }

    private void LayoutPageHost()
    {
        // TabPage can only be parented by TabControl. Keep a native TabControl as
        // an implementation-only page host, then clip its complete native strip.
        // All visible tab layout and input remains owned by this control.
        Rectangle panelBounds = m_ContentPanel.ClientRectangle;
        if (panelBounds.Width <= 0 || panelBounds.Height <= 0)
            return;

        if (m_PageHost.TabCount == 0) {
            m_PageHost.Visible = false;
            return;
        }

        // Probe the native TabControl chrome once while it is hidden. Reapplying
        // the panel bounds before every final clipped layout resized the selected
        // TabPage twice and briefly exposed the native light-themed host.
        if (!m_PageHostChromeMeasured) {
            m_PageHost.SetBounds(0, 0,
                Math.Max(100, panelBounds.Width), Math.Max(100, panelBounds.Height));
            Rectangle pageBounds = m_PageHost.DisplayRectangle;
            m_PageHostChromeLeft = Math.Max(0, pageBounds.Left);
            m_PageHostChromeTop = Math.Max(0, pageBounds.Top);
            m_PageHostChromeRight = Math.Max(0, m_PageHost.ClientSize.Width - pageBounds.Right);
            m_PageHostChromeBottom = Math.Max(0, m_PageHost.ClientSize.Height - pageBounds.Bottom);
            m_PageHostChromeMeasured = true;
        }

        Rectangle finalBounds = new Rectangle(
            -m_PageHostChromeLeft,
            -m_PageHostChromeTop,
            panelBounds.Width + m_PageHostChromeLeft + m_PageHostChromeRight,
            panelBounds.Height + m_PageHostChromeTop + m_PageHostChromeBottom);
        if (m_PageHost.Bounds != finalBounds)
            m_PageHost.Bounds = finalBounds;
        m_PageHost.Visible = true;
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        EnsureSelectedVisible();
        Invalidate();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        EnsureSelectedVisible();
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        BuildTabLayout();
        DrawTabStrip(e.Graphics);
    }

    private void DrawTabStrip(Graphics graphics)
    {
        bool dark = ScriptEditor.InterfaceTheme.IsDark;
        Color headerColor = dark ? Color.FromArgb(53, 53, 56) : Color.FromArgb(243, 243, 245);
        Color selectedColor = dark ? Color.FromArgb(40, 40, 42) : Color.White;
        Color hoverColor = dark ? Color.FromArgb(60, 60, 64) : Color.FromArgb(231, 234, 238);
        Color borderColor = dark ? Color.FromArgb(68, 68, 72) : Color.FromArgb(207, 210, 214);
        Color accentColor = Color.FromArgb(0, 120, 212);
        Rectangle header = new Rectangle(0, 0, ClientSize.Width, HeaderHeight);

        using (Brush headerBrush = new SolidBrush(headerColor))
        using (Pen borderPen = new Pen(borderColor))
        {
            graphics.FillRectangle(headerBrush, header);
            graphics.DrawLine(borderPen, 0, HeaderHeight - 1, Math.Max(0, ClientSize.Width - 1), HeaderHeight - 1);

            for (int i = 0; i < m_Pages.Count; i++)
            {
                Rectangle tab = m_TabBounds[i];
                if (tab.IsEmpty)
                    continue;
                TabPage page = m_Pages[i];
                Color fill = page == SelectedTab ? selectedColor : (page == m_HotTab ? hoverColor : headerColor);
                using (Brush tabBrush = new SolidBrush(fill))
                    graphics.FillRectangle(tabBrush, tab);
                graphics.DrawLine(borderPen, tab.Right - 1, tab.Top + 3, tab.Right - 1, tab.Bottom - 3);

                if (page == SelectedTab)
                {
                    int accentHeight = ScriptEditor.DpiHelper.Scale(this, 2);
                    using (Brush accentBrush = new SolidBrush(accentColor))
                        graphics.FillRectangle(accentBrush, tab.Left + 2, tab.Bottom - accentHeight,
                            Math.Max(0, tab.Width - 4), accentHeight);
                }

                Rectangle textBounds = tab;
                if (ShowDocumentStatusIcons)
                {
                    Rectangle statusBounds = GetDocumentStatusRectangle(tab);
                    DrawDocumentStatusIcon(graphics, statusBounds, m_ModifiedTabs.Contains(page),
                        m_UntitledTabs.Contains(page), dark);
                    textBounds.X = statusBounds.Right + ScriptEditor.DpiHelper.Scale(this, 4);
                    textBounds.Width = Math.Max(0, tab.Right - textBounds.X);
                }
                if (ImageList != null && page.ImageIndex >= 0 && page.ImageIndex < ImageList.Images.Count)
                {
                    Image image = ImageList.Images[page.ImageIndex];
                    int imageY = tab.Y + Math.Max(0, (tab.Height - image.Height) / 2);
                    graphics.DrawImage(image, tab.X + 5, imageY, image.Width, image.Height);
                    textBounds.X += image.Width + 7;
                    textBounds.Width = Math.Max(0, textBounds.Width - image.Width - 7);
                }
                if (ShowCloseButtons)
                    textBounds.Width = Math.Max(0, GetCloseButtonRectangle(page).Left - textBounds.Left - 2);

                int textPadding = ScriptEditor.DpiHelper.Scale(this, ShowDocumentStatusIcons ? 1 : 5);
                textBounds.X += textPadding;
                textBounds.Width = Math.Max(0, textBounds.Width - textPadding);
                Color textColor = dark
                    ? (page == SelectedTab ? Color.White : Color.Gainsboro)
                    : (page == SelectedTab ? Color.FromArgb(28, 28, 30) : Color.FromArgb(68, 68, 72));
                TextRenderer.DrawText(graphics, page.Text, Font, textBounds, textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine);

                if (ShouldShowCloseButton(page))
                    DrawCloseButton(graphics, page, dark);
            }

            if (m_HasOverflow)
            {
                DrawNavigationButton(graphics, m_LeftNavigationBounds, true,
                    m_FirstVisibleIndex > 0, dark, borderColor);
                DrawNavigationButton(graphics, m_RightNavigationBounds, false,
                    m_LastVisibleIndex >= 0 && m_LastVisibleIndex < m_Pages.Count - 1, dark, borderColor);
            }
        }
    }

    private static void DrawNavigationButton(Graphics graphics, Rectangle bounds, bool left,
        bool enabled, bool dark, Color borderColor)
    {
        Color back = dark ? Color.FromArgb(53, 53, 56) : Color.FromArgb(243, 243, 245);
        Color fore = enabled
            ? (dark ? Color.Gainsboro : Color.FromArgb(68, 68, 72))
            : (dark ? Color.FromArgb(96, 96, 100) : Color.FromArgb(170, 170, 174));
        using (Brush brush = new SolidBrush(back))
            graphics.FillRectangle(brush, bounds);
        using (Pen border = new Pen(borderColor))
            graphics.DrawRectangle(border, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1);

        int cx = bounds.Left + bounds.Width / 2;
        int cy = bounds.Top + bounds.Height / 2;
        int size = Math.Max(3, bounds.Width / 5);
        Point[] arrow = left
            ? new[] { new Point(cx + size / 2, cy - size), new Point(cx - size / 2, cy), new Point(cx + size / 2, cy + size) }
            : new[] { new Point(cx - size / 2, cy - size), new Point(cx + size / 2, cy), new Point(cx - size / 2, cy + size) };
        using (Pen pen = new Pen(fore, 1.5F))
        {
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            graphics.DrawLines(pen, arrow);
        }
    }

    private void BuildTabLayout()
    {
        m_TabWidths.Clear();
        m_TabBounds.Clear();
        int totalWidth = 0;
        foreach (TabPage page in m_Pages)
        {
            int width = MeasureTabWidth(page);
            m_TabWidths.Add(width);
            m_TabBounds.Add(Rectangle.Empty);
            totalWidth += width;
        }

        int navigationWidth = NavigationButtonWidth;
        m_HasOverflow = totalWidth > ClientSize.Width;
        int availableRight = Math.Max(0, ClientSize.Width - (m_HasOverflow ? navigationWidth * 2 : 0));
        if (!m_HasOverflow)
            m_FirstVisibleIndex = 0;
        else if (m_Pages.Count > 0)
            m_FirstVisibleIndex = Math.Max(0, Math.Min(m_FirstVisibleIndex, m_Pages.Count - 1));

        m_LeftNavigationBounds = m_HasOverflow
            ? new Rectangle(availableRight, 1, navigationWidth, Math.Max(1, HeaderHeight - 2))
            : Rectangle.Empty;
        m_RightNavigationBounds = m_HasOverflow
            ? new Rectangle(availableRight + navigationWidth, 1, navigationWidth, Math.Max(1, HeaderHeight - 2))
            : Rectangle.Empty;

        int x = 0;
        m_LastVisibleIndex = -1;
        for (int i = m_FirstVisibleIndex; i < m_Pages.Count; i++)
        {
            int width = m_TabWidths[i];
            if (x > 0 && x + width > availableRight)
                break;
            width = Math.Min(width, Math.Max(0, availableRight - x));
            if (width <= 0)
                break;
            m_TabBounds[i] = new Rectangle(x, 1, width, Math.Max(1, HeaderHeight - 1));
            x += width;
            m_LastVisibleIndex = i;
        }
    }

    private int MeasureTabWidth(TabPage page)
    {
        int width = TextRenderer.MeasureText(page == null ? String.Empty : page.Text, Font,
            new Size(Int32.MaxValue, HeaderHeight), TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine).Width;
        width += ScriptEditor.DpiHelper.Scale(this, 14);
        if (ShowDocumentStatusIcons)
            width += ScriptEditor.DpiHelper.Scale(this, 16);
        if (ShowCloseButtons)
            width += ScriptEditor.DpiHelper.Scale(this, 20);
        if (ImageList != null && page != null && page.ImageIndex >= 0)
            width += ImageList.ImageSize.Width + ScriptEditor.DpiHelper.Scale(this, 5);
        int minimum = ScriptEditor.DpiHelper.Scale(this, 74);
        int maximum = ScriptEditor.DpiHelper.Scale(this, 190);
        return Math.Max(minimum, Math.Min(maximum, width));
    }

    private void EnsureSelectedVisible()
    {
        if (m_SelectedIndex < 0 || m_SelectedIndex >= m_Pages.Count || ClientSize.Width <= 0)
            return;
        BuildTabLayout();
        if (!m_HasOverflow || !m_TabBounds[m_SelectedIndex].IsEmpty)
            return;

        int available = Math.Max(1, ClientSize.Width - NavigationButtonWidth * 2);
        int first = m_SelectedIndex;
        int used = m_TabWidths[m_SelectedIndex];
        while (first > 0 && used + m_TabWidths[first - 1] <= available)
        {
            first--;
            used += m_TabWidths[first];
        }
        m_FirstVisibleIndex = first;
        BuildTabLayout();
    }

    private void NormalizeFirstVisibleIndex()
    {
        if (m_Pages.Count == 0) {
            m_FirstVisibleIndex = 0;
            return;
        }
        m_FirstVisibleIndex = Math.Max(0, Math.Min(m_FirstVisibleIndex, m_Pages.Count - 1));
        BuildTabLayout();
        if (!m_HasOverflow) {
            m_FirstVisibleIndex = 0;
            return;
        }
        int available = Math.Max(1, ClientSize.Width - NavigationButtonWidth * 2);
        while (m_FirstVisibleIndex > 0)
        {
            int used = 0;
            for (int i = m_FirstVisibleIndex; i < m_Pages.Count && used + m_TabWidths[i] <= available; i++)
                used += m_TabWidths[i];
            if (used + m_TabWidths[m_FirstVisibleIndex - 1] > available)
                break;
            m_FirstVisibleIndex--;
        }
    }

    public Rectangle GetTabRect(int index)
    {
        BuildTabLayout();
        return index >= 0 && index < m_TabBounds.Count ? m_TabBounds[index] : Rectangle.Empty;
    }

    internal bool IsTabNavigationArea(Point location)
    {
        BuildTabLayout();
        return m_HasOverflow && (m_LeftNavigationBounds.Contains(location) || m_RightNavigationBounds.Contains(location));
    }

    public void SelectTab(int index) { SelectedIndex = index; }
    public void SelectTab(TabPage page) { SelectedTab = page; }

    private void SelectIndex(int index, bool raiseEvent)
    {
        if (m_Pages.Count == 0)
            index = -1;
        else
            index = Math.Max(0, Math.Min(index, m_Pages.Count - 1));
        if (m_SelectedIndex == index)
            return;

        m_SelectedIndex = index;
        EnsureSelectedVisible();
        UpdatePageVisibility();
        Invalidate();
        if (raiseEvent && Selected != null)
            Selected(this, new TabControlEventArgs(SelectedTab, m_SelectedIndex, TabControlAction.Selected));
    }

    private void UpdatePageVisibility()
    {
        TabPage selected = SelectedTab;
        if (selected != null && m_PageHost.TabPages.Contains(selected))
            m_PageHost.SelectedTab = selected;
    }

    internal void AddPage(TabPage page, int index)
    {
        if (page == null || m_Pages.Contains(page))
            return;
        TabPage selectedBefore = SelectedTab;
        index = Math.Max(0, Math.Min(index, m_Pages.Count));
        m_Pages.Insert(index, page);
        m_PageHost.TabPages.Insert(index, page);
        if (selectedBefore == null)
            SelectIndex(0, true);
        else {
            m_SelectedIndex = m_Pages.IndexOf(selectedBefore);
            UpdatePageVisibility();
        }
        LayoutPageHost();
        EnsureSelectedVisible();
        Invalidate();
    }

    internal void RemovePage(TabPage page)
    {
        int index = m_Pages.IndexOf(page);
        if (index < 0)
            return;
        TabPage selectedBefore = SelectedTab;
        bool wasSelected = object.ReferenceEquals(page, selectedBefore);
        m_Pages.RemoveAt(index);
        m_PageHost.TabPages.Remove(page);
        m_ModifiedTabs.Remove(page);
        m_UntitledTabs.Remove(page);
        if (object.ReferenceEquals(page, m_ClosePressedTab))
            ClearClosePress();
        if (object.ReferenceEquals(page, m_DraggedTab))
            m_DraggedTab = null;

        if (m_Pages.Count == 0)
            SelectIndex(-1, true);
        else if (wasSelected) {
            m_SelectedIndex = -1;
            SelectIndex(Math.Min(index, m_Pages.Count - 1), true);
        } else {
            m_SelectedIndex = m_Pages.IndexOf(selectedBefore);
            UpdatePageVisibility();
        }
        LayoutPageHost();
        NormalizeFirstVisibleIndex();
        Invalidate();
    }

    public void MoveTab(TabPage page, int targetIndex)
    {
        int sourceIndex = m_Pages.IndexOf(page);
        if (sourceIndex < 0 || m_Pages.Count < 2)
            return;
        targetIndex = Math.Max(0, Math.Min(targetIndex, m_Pages.Count - 1));
        if (sourceIndex == targetIndex)
            return;

        TabPage selected = SelectedTab;
        m_IsReordering = true;
        try {
            if (tabsSwapped != null)
                tabsSwapped(this, new TabsSwappedEventArgs(sourceIndex, targetIndex));
            m_Pages.RemoveAt(sourceIndex);
            m_Pages.Insert(targetIndex, page);
            // The page host is implementation-only and its order is irrelevant.
            // Keeping the TabPage attached prevents the editor surface from being
            // recreated (and briefly flashing its default background) while dragging.
            m_SelectedIndex = m_Pages.IndexOf(selected);
        }
        finally { m_IsReordering = false; }
        EnsureSelectedVisible();
        Invalidate();
    }

    public void SetDocumentModified(TabPage page, bool modified)
    {
        if (page == null) return;
        if (modified) m_ModifiedTabs.Add(page); else m_ModifiedTabs.Remove(page);
        InvalidateDocumentTab(page);
    }

    public void SetDocumentUntitled(TabPage page, bool untitled)
    {
        if (page == null) return;
        if (untitled) m_UntitledTabs.Add(page); else m_UntitledTabs.Remove(page);
        InvalidateDocumentTab(page);
    }

    private void InvalidateDocumentTab(TabPage page)
    {
        Rectangle bounds = GetTabRect(m_Pages.IndexOf(page));
        if (!bounds.IsEmpty) Invalidate(bounds);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        BuildTabLayout();
        if (e.Button != MouseButtons.Left)
            return;

        if (m_HasOverflow && m_LeftNavigationBounds.Contains(e.Location)) {
            m_NavigationPressed = -1;
            Capture = true;
            return;
        }
        if (m_HasOverflow && m_RightNavigationBounds.Contains(e.Location)) {
            m_NavigationPressed = 1;
            Capture = true;
            return;
        }

        TabPage page = TabAt(e.Location);
        if (page == null)
            return;
        Rectangle close = GetCloseButtonRectangle(page);
        if (ShouldShowCloseButton(page) && close.Contains(e.Location)) {
            m_ClosePressedTab = page;
            m_ClosePressedBounds = close;
            Capture = true;
            Invalidate(close);
            return;
        }

        SelectedTab = page;
        m_DraggedTab = page;
        m_SwapLockTarget = null;
        m_DragStart = e.Location;
        Capture = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        BuildTabLayout();
        TabPage hot = TabAt(e.Location);
        TabPage hotClose = hot != null && GetCloseButtonRectangle(hot).Contains(e.Location) ? hot : null;
        if (!object.ReferenceEquals(hot, m_HotTab) || !object.ReferenceEquals(hotClose, m_HotCloseTab)) {
            m_HotTab = hot;
            m_HotCloseTab = hotClose;
            if (ShowToolTips)
                m_ToolTip.SetToolTip(this, hot == null ? null : hot.ToolTipText);
            Invalidate(new Rectangle(0, 0, ClientSize.Width, HeaderHeight));
        }

        if (e.Button != MouseButtons.Left || m_DraggedTab == null || m_ClosePressedTab != null || m_NavigationPressed != 0)
            return;
        Size drag = SystemInformation.DragSize;
        if (Math.Abs(e.X - m_DragStart.X) < drag.Width / 2 && Math.Abs(e.Y - m_DragStart.Y) < drag.Height / 2)
            return;
        TabPage target = TabAt(e.Location);
        if (target == null || target == m_DraggedTab)
            return;
        if (target == m_SwapLockTarget)
            return;
        MoveTab(m_DraggedTab, m_Pages.IndexOf(target));
        m_SwapLockTarget = target;
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        TabPage closeTab = m_ClosePressedTab;
        Rectangle closeBounds = m_ClosePressedBounds;
        int navigation = m_NavigationPressed;
        ClearClosePress();
        m_NavigationPressed = 0;
        m_DraggedTab = null;
        m_SwapLockTarget = null;
        Capture = false;

        if (e.Button == MouseButtons.Left && navigation != 0) {
            Rectangle bounds = navigation < 0 ? m_LeftNavigationBounds : m_RightNavigationBounds;
            if (bounds.Contains(e.Location))
                ScrollTabs(navigation);
        } else if (e.Button == MouseButtons.Left && closeTab != null && m_Pages.Contains(closeTab)
            && closeBounds.Contains(e.Location)) {
            EventHandler<TabCloseRequestedEventArgs> handler = TabCloseRequested;
            if (handler != null)
                handler(this, new TabCloseRequestedEventArgs(m_Pages.IndexOf(closeTab), closeTab));
        }
        Invalidate(new Rectangle(0, 0, ClientSize.Width, HeaderHeight));
        base.OnMouseUp(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        m_HotTab = null;
        m_HotCloseTab = null;
        if (ShowToolTips) m_ToolTip.SetToolTip(this, null);
        Invalidate(new Rectangle(0, 0, ClientSize.Width, HeaderHeight));
    }

    private void ScrollTabs(int direction)
    {
        BuildTabLayout();
        if (!m_HasOverflow)
            return;
        if (direction < 0 && m_FirstVisibleIndex > 0)
            m_FirstVisibleIndex--;
        else if (direction > 0 && m_LastVisibleIndex < m_Pages.Count - 1)
            m_FirstVisibleIndex++;
        BuildTabLayout();
        Invalidate(new Rectangle(0, 0, ClientSize.Width, HeaderHeight));
    }

    private TabPage TabAt(Point location)
    {
        BuildTabLayout();
        for (int i = 0; i < m_TabBounds.Count; i++)
            if (!m_TabBounds[i].IsEmpty && m_TabBounds[i].Contains(location))
                return m_Pages[i];
        return null;
    }

    private bool ShouldShowCloseButton(TabPage page)
    {
        return ShowCloseButtons && (object.ReferenceEquals(page, SelectedTab) || object.ReferenceEquals(page, m_HotTab));
    }

    private Rectangle GetCloseButtonRectangle(TabPage page)
    {
        int index = m_Pages.IndexOf(page);
        if (!ShowCloseButtons || index < 0)
            return Rectangle.Empty;
        BuildTabLayout();
        Rectangle tab = m_TabBounds[index];
        if (tab.IsEmpty)
            return Rectangle.Empty;
        int size = Math.Min(ScriptEditor.DpiHelper.Scale(this, 14), Math.Max(0, tab.Height - 5));
        return new Rectangle(tab.Right - size - ScriptEditor.DpiHelper.Scale(this, 3),
            tab.Top + Math.Max(0, (tab.Height - size) / 2), size, size);
    }

    private void DrawCloseButton(Graphics graphics, TabPage page, bool dark)
    {
        Rectangle bounds = GetCloseButtonRectangle(page);
        if (bounds.IsEmpty)
            return;
        bool hovered = object.ReferenceEquals(m_HotCloseTab, page);
        bool pressed = object.ReferenceEquals(m_ClosePressedTab, page);
        if (hovered) {
            Color hoverBack = dark
                ? (pressed ? Color.FromArgb(90, 55, 55) : Color.FromArgb(78, 65, 67))
                : (pressed ? Color.FromArgb(225, 190, 190) : Color.FromArgb(235, 215, 215));
            using (Brush brush = new SolidBrush(hoverBack)) graphics.FillRectangle(brush, bounds);
        }
        int inset = Math.Max(3, ScriptEditor.DpiHelper.Scale(this, 4));
        Color color = dark ? Color.Gainsboro : Color.FromArgb(92, 92, 96);
        using (Pen pen = new Pen(color, 1.25F)) {
            graphics.DrawLine(pen, bounds.Left + inset, bounds.Top + inset,
                bounds.Right - inset - 1, bounds.Bottom - inset - 1);
            graphics.DrawLine(pen, bounds.Right - inset - 1, bounds.Top + inset,
                bounds.Left + inset, bounds.Bottom - inset - 1);
        }
    }

    private void ClearClosePress()
    {
        m_ClosePressedTab = null;
        m_ClosePressedBounds = Rectangle.Empty;
    }

    private Rectangle GetDocumentStatusRectangle(Rectangle tab)
    {
        int width = ScriptEditor.DpiHelper.Scale(this, 10);
        int height = ScriptEditor.DpiHelper.Scale(this, 12);
        return new Rectangle(tab.Left + ScriptEditor.DpiHelper.Scale(this, 5),
            tab.Top + Math.Max(0, (tab.Height - height) / 2), width, height);
    }

    private static void DrawDocumentStatusIcon(Graphics graphics, Rectangle bounds,
        bool modified, bool untitled, bool dark)
    {
        bool highlighted = modified || untitled;
        Color outline = highlighted ? Color.FromArgb(230, 159, 50)
            : (dark ? Color.FromArgb(158, 162, 168) : Color.FromArgb(105, 110, 116));
        Color fill = highlighted
            ? (dark ? Color.FromArgb(88, 67, 35) : Color.FromArgb(255, 241, 211))
            : (dark ? Color.FromArgb(64, 65, 69) : Color.FromArgb(250, 250, 250));
        int fold = Math.Max(2, bounds.Width / 3);
        Point[] document = {
            new Point(bounds.Left, bounds.Top), new Point(bounds.Right - fold - 1, bounds.Top),
            new Point(bounds.Right - 1, bounds.Top + fold), new Point(bounds.Right - 1, bounds.Bottom - 1),
            new Point(bounds.Left, bounds.Bottom - 1)
        };
        SmoothingMode previous = graphics.SmoothingMode;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using (Brush brush = new SolidBrush(fill))
        using (Pen pen = new Pen(outline, Math.Max(1F, bounds.Width / 10F))) {
            graphics.FillPolygon(brush, document);
            graphics.DrawPolygon(pen, document);
            graphics.DrawLine(pen, bounds.Right - fold - 1, bounds.Top,
                bounds.Right - fold - 1, bounds.Top + fold);
            graphics.DrawLine(pen, bounds.Right - fold - 1, bounds.Top + fold,
                bounds.Right - 1, bounds.Top + fold);
        }
        graphics.SmoothingMode = previous;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) m_ToolTip.Dispose();
        base.Dispose(disposing);
    }

    public sealed class ManagedTabPageCollection : IEnumerable<TabPage>
    {
        private readonly DraggableTabControl m_Owner;
        internal ManagedTabPageCollection(DraggableTabControl owner) { m_Owner = owner; }
        public int Count { get { return m_Owner.m_Pages.Count; } }
        public TabPage this[int index] { get { return m_Owner.m_Pages[index]; } }
        public void Add(TabPage page) { m_Owner.AddPage(page, Count); }
        public void AddRange(TabPage[] pages) { if (pages != null) foreach (TabPage page in pages) Add(page); }
        public void Insert(int index, TabPage page) { m_Owner.AddPage(page, index); }
        public void Remove(TabPage page) { m_Owner.RemovePage(page); }
        public void RemoveAt(int index) { if (index >= 0 && index < Count) Remove(this[index]); }
        public int IndexOf(TabPage page) { return m_Owner.m_Pages.IndexOf(page); }
        public bool Contains(TabPage page) { return m_Owner.m_Pages.Contains(page); }
        public IEnumerator<TabPage> GetEnumerator() { return m_Owner.m_Pages.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }

    private sealed class DraggableTabControlCollection : Control.ControlCollection
    {
        private readonly DraggableTabControl m_Owner;

        internal DraggableTabControlCollection(DraggableTabControl owner) : base(owner)
        {
            m_Owner = owner;
        }

        public override void Add(Control value)
        {
            TabPage page = value as TabPage;
            if (page != null) {
                m_Owner.TabPages.Add(page);
                return;
            }
            base.Add(value);
        }

        public override void Remove(Control value)
        {
            TabPage page = value as TabPage;
            if (page != null && m_Owner.TabPages.Contains(page)) {
                m_Owner.TabPages.Remove(page);
                return;
            }
            base.Remove(value);
        }
    }
}

public sealed class TabCloseRequestedEventArgs : EventArgs
{
    public int TabIndex { get; private set; }
    public TabPage TabPage { get; private set; }
    public TabCloseRequestedEventArgs(int tabIndex, TabPage tabPage) { TabIndex = tabIndex; TabPage = tabPage; }
}

public class TabsSwappedEventArgs : EventArgs
{
    public int aIndex { get; set; }
    public int bIndex { get; set; }
    public TabsSwappedEventArgs(int a, int b) { aIndex = a; bIndex = b; }
}
