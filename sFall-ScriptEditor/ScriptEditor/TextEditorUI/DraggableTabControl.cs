using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

// Declare a delegate
public delegate void SwapEventHandler(object sender, TabsSwappedEventArgs e);

public class DraggableTabControl : TabControl
{
    private TabPage m_DraggedTab;
    private int m_X;
    private int m_HotTabIndex = -1;
    private int m_HotCloseIndex = -1;
    private bool m_OverflowHot;
    private int m_ClosePressedIndex = -1;
    private ContextMenuStrip m_OverflowMenu;
    private readonly HashSet<TabPage> m_ModifiedTabs = new HashSet<TabPage>();
    private readonly HashSet<TabPage> m_UntitledTabs = new HashSet<TabPage>();

    [Category("Appearance")]
    [DefaultValue(false)]
    [Description("Shows a close button on each tab.")]
    public bool ShowCloseButtons { get; set; }

    [Category("Appearance")]
    [DefaultValue(false)]
    [Description("Shows a saved or modified document icon before each tab label.")]
    public bool ShowDocumentStatusIcons { get; set; }

    [Category("Action")]
    [Description("Fires when the close button on a tab is clicked.")]
    public event EventHandler<TabCloseRequestedEventArgs> TabCloseRequested;

    [Category("Action")]
    [Description("Fires before tabs are swapped (indexes indicate positions before swap)")]
    public event SwapEventHandler tabsSwapped;

    public DraggableTabControl()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        MouseDown += OnMouseDown;
        MouseMove += OnMouseMove;
        MouseUp += OnMouseUp;
        MouseLeave += OnMouseLeave;
        SelectedIndexChanged += delegate { Invalidate(); };
    }

    public void SetDocumentModified(TabPage page, bool modified)
    {
        if (page == null)
            return;

        if (modified)
            m_ModifiedTabs.Add(page);
        else
            m_ModifiedTabs.Remove(page);

        InvalidateDocumentTab(page);
    }

    public void SetDocumentUntitled(TabPage page, bool untitled)
    {
        if (page == null)
            return;

        if (untitled)
            m_UntitledTabs.Add(page);
        else
            m_UntitledTabs.Remove(page);

        InvalidateDocumentTab(page);
    }

    private void InvalidateDocumentTab(TabPage page)
    {
        int index = TabPages.IndexOf(page);
        if (index >= 0)
            Invalidate(GetTabRect(index));
    }

    protected override void OnControlRemoved(ControlEventArgs e)
    {
        TabPage page = e.Control as TabPage;
        if (page != null) {
            m_ModifiedTabs.Remove(page);
            m_UntitledTabs.Remove(page);
        }
        base.OnControlRemoved(e);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PaintStruct
    {
        public IntPtr Hdc;
        public bool Erase;
        public NativeRect PaintRect;
        public bool Restore;
        public bool IncUpdate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Reserved;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr BeginPaint(IntPtr hwnd, out PaintStruct paint);

    [DllImport("user32.dll")]
    private static extern bool EndPaint(IntPtr hwnd, ref PaintStruct paint);

    protected override void WndProc(ref Message m)
    {
        const int WM_ERASEBKGND = 0x0014;
        const int WM_PAINT = 0x000F;
        const int TCM_ADJUSTRECT = 0x1328;

        if (m.Msg == TCM_ADJUSTRECT && m.WParam == IntPtr.Zero
            && ScriptEditor.InterfaceTheme.IsDark)
        {
            base.WndProc(ref m);
            NativeRect rect = (NativeRect)Marshal.PtrToStructure(m.LParam, typeof(NativeRect));
            rect.Left -= 4;
            rect.Top -= 3;
            rect.Right += 4;
            rect.Bottom += 4;
            Marshal.StructureToPtr(rect, m.LParam, false);
            return;
        }

        if (IsHandleCreated)
        {
            if (m.Msg == WM_ERASEBKGND)
            {
                // WM_PAINT covers the invalidated header area. Erasing it first produces
                // a visible intermediate frame while the mouse moves between tabs.
                m.Result = new IntPtr(1);
                return;
            }

            if (m.Msg == WM_PAINT)
            {
                PaintStruct paint;
                IntPtr hdc = BeginPaint(Handle, out paint);
                try
                {
                    Rectangle updateBounds = Rectangle.FromLTRB(
                        paint.PaintRect.Left, paint.PaintRect.Top,
                        paint.PaintRect.Right, paint.PaintRect.Bottom);
                    if (updateBounds.Width > 0 && updateBounds.Height > 0) {
                        using (Graphics target = Graphics.FromHdc(hdc))
                        // TextRenderer uses GDI and ignores Graphics transforms. Keep the
                        // control's original coordinates in the buffer, then copy only the
                        // invalidated rectangle to the window.
                        using (Bitmap buffer = new Bitmap(
                            Math.Max(1, updateBounds.Right), Math.Max(1, updateBounds.Bottom)))
                        using (Graphics graphics = Graphics.FromImage(buffer)) {
                            DrawTabControl(graphics, ScriptEditor.InterfaceTheme.IsDark);
                            target.DrawImage(buffer, updateBounds, updateBounds, GraphicsUnit.Pixel);
                        }
                    }
                }
                finally
                {
                    EndPaint(Handle, ref paint);
                }
                m.Result = IntPtr.Zero;
                return;
            }
        }

        base.WndProc(ref m);
    }

    private void DrawTabControl(Graphics graphics, bool dark)
    {
        Rectangle page = DisplayRectangle;
        int headerBottom = page.Top;
        for (int i = 0; i < TabCount; i++)
            headerBottom = Math.Max(headerBottom, GetTabRect(i).Bottom);

        Color headerColor = dark ? Color.FromArgb(53, 53, 56) : Color.FromArgb(243, 243, 245);
        Color selectedColor = dark ? Color.FromArgb(40, 40, 42) : Color.White;
        Color hoverColor = dark ? Color.FromArgb(60, 60, 64) : Color.FromArgb(231, 234, 238);
        Color borderColor = dark ? Color.FromArgb(68, 68, 72) : Color.FromArgb(207, 210, 214);
        Color accentColor = Color.FromArgb(0, 120, 212);
        using (Brush headerBrush = new SolidBrush(headerColor))
        using (Pen pen = new Pen(borderColor))
        {
            graphics.FillRectangle(headerBrush, ClientRectangle);
            graphics.DrawLine(pen, 0, Math.Max(0, headerBottom - 1),
                Math.Max(0, Width - 1), Math.Max(0, headerBottom - 1));

            for (int i = 0; i < TabCount; i++)
            {
                Rectangle tab = GetTabRect(i);
                Color tabColor = i == SelectedIndex ? selectedColor : (i == m_HotTabIndex ? hoverColor : headerColor);
                using (Brush tabBrush = new SolidBrush(tabColor))
                    graphics.FillRectangle(tabBrush, tab);
                graphics.DrawLine(pen, tab.Right - 1, tab.Top + 3, tab.Right - 1, tab.Bottom - 3);

                if (i == SelectedIndex)
                {
                    int accentHeight = ScriptEditor.DpiHelper.Scale(this, 2);
                    using (Brush accentBrush = new SolidBrush(accentColor))
                        graphics.FillRectangle(accentBrush, tab.Left + 2, tab.Bottom - accentHeight,
                            Math.Max(0, tab.Width - 4), accentHeight);
                }

                Rectangle textRect = tab;
                TabPage pageTab = TabPages[i];
                if (ShowDocumentStatusIcons)
                {
                    Rectangle statusBounds = GetDocumentStatusRectangle(tab);
                    DrawDocumentStatusIcon(graphics, statusBounds, m_ModifiedTabs.Contains(pageTab), m_UntitledTabs.Contains(pageTab), dark);
                    textRect.X = statusBounds.Right + ScriptEditor.DpiHelper.Scale(this, 4);
                    textRect.Width = Math.Max(0, tab.Right - textRect.X);
                }
                if (ImageList != null && pageTab.ImageIndex >= 0 && pageTab.ImageIndex < ImageList.Images.Count)
                {
                    Image image = ImageList.Images[pageTab.ImageIndex];
                    int imageY = tab.Y + Math.Max(0, (tab.Height - image.Height) / 2);
                    graphics.DrawImage(image, tab.X + 5, imageY, image.Width, image.Height);
                    textRect.X += image.Width + 7;
                    textRect.Width = Math.Max(0, textRect.Width - image.Width - 7);
                }

                if (ShowCloseButtons)
                    textRect.Width = Math.Max(0, GetCloseButtonRectangle(i).Left - textRect.Left - 2);

                int textPadding = ScriptEditor.DpiHelper.Scale(this, ShowDocumentStatusIcons ? 1 : 5);
                textRect.X += textPadding;
                textRect.Width = Math.Max(0, textRect.Width - textPadding);
                Color textColor = dark
                    ? (i == SelectedIndex ? Color.White : Color.Gainsboro)
                    : (i == SelectedIndex ? Color.FromArgb(28, 28, 30) : Color.FromArgb(68, 68, 72));
                using (Brush textBrush = new SolidBrush(textColor))
                using (StringFormat textFormat = new StringFormat(StringFormat.GenericTypographic))
                {
                    textFormat.Alignment = StringAlignment.Near;
                    textFormat.FormatFlags |= StringFormatFlags.NoWrap;
                    SizeF textSize = graphics.MeasureString(pageTab.Text, Font, Int32.MaxValue, textFormat);
                    float textY = textRect.Top + Math.Max(0f, (textRect.Height - textSize.Height) / 2f);

                    if (textSize.Width <= textRect.Width)
                    {
                        graphics.DrawString(pageTab.Text, Font, textBrush,
                            new PointF(textRect.Left, textY), textFormat);
                    }
                    else
                    {
                        float fitScale = textSize.Width <= 0f ? 1f : textRect.Width / textSize.Width;
                        if (fitScale >= 0.9f)
                        {
                            GraphicsState state = graphics.Save();
                            graphics.TranslateTransform(textRect.Left, textY);
                            graphics.ScaleTransform(fitScale, 1f);
                            graphics.DrawString(pageTab.Text, Font, textBrush, PointF.Empty, textFormat);
                            graphics.Restore(state);
                        }
                        else
                        {
                            textFormat.LineAlignment = StringAlignment.Center;
                            textFormat.Trimming = StringTrimming.EllipsisCharacter;
                            graphics.DrawString(pageTab.Text, Font, textBrush, textRect, textFormat);
                        }
                    }
                }

                if (ShouldShowCloseButton(i))
                    DrawCloseButton(graphics, i, dark);
            }

            DrawOverflowButton(graphics, dark);
        }
    }

    private Rectangle GetDocumentStatusRectangle(Rectangle tab)
    {
        int width = ScriptEditor.DpiHelper.Scale(this, 10);
        int height = ScriptEditor.DpiHelper.Scale(this, 12);
        return new Rectangle(
            tab.Left + ScriptEditor.DpiHelper.Scale(this, 5),
            tab.Top + Math.Max(0, (tab.Height - height) / 2),
            width,
            height);
    }

    private static void DrawDocumentStatusIcon(Graphics graphics, Rectangle bounds, bool modified, bool untitled, bool dark)
    {
        bool highlighted = modified || untitled;
        Color outline = highlighted
            ? Color.FromArgb(230, 159, 50)
            : (dark ? Color.FromArgb(158, 162, 168) : Color.FromArgb(105, 110, 116));
        Color fill = highlighted
            ? (dark ? Color.FromArgb(88, 67, 35) : Color.FromArgb(255, 241, 211))
            : (dark ? Color.FromArgb(64, 65, 69) : Color.FromArgb(250, 250, 250));
        int fold = Math.Max(2, bounds.Width / 3);

        Point[] document = {
            new Point(bounds.Left, bounds.Top),
            new Point(bounds.Right - fold - 1, bounds.Top),
            new Point(bounds.Right - 1, bounds.Top + fold),
            new Point(bounds.Right - 1, bounds.Bottom - 1),
            new Point(bounds.Left, bounds.Bottom - 1)
        };

        SmoothingMode previous = graphics.SmoothingMode;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using (Brush brush = new SolidBrush(fill))
        using (Pen pen = new Pen(outline, Math.Max(1f, bounds.Width / 10f)))
        {
            graphics.FillPolygon(brush, document);
            graphics.DrawPolygon(pen, document);
            graphics.DrawLine(pen, bounds.Right - fold - 1, bounds.Top,
                bounds.Right - fold - 1, bounds.Top + fold);
            graphics.DrawLine(pen, bounds.Right - fold - 1, bounds.Top + fold,
                bounds.Right - 1, bounds.Top + fold);
        }
        graphics.SmoothingMode = previous;
    }

    private Rectangle GetCloseButtonRectangle(int index)
    {
        if (!ShowCloseButtons || index < 0 || index >= TabCount)
            return Rectangle.Empty;
        Rectangle tab = GetTabRect(index);
        int size = Math.Min(ScriptEditor.DpiHelper.Scale(this, 14), Math.Max(0, tab.Height - 5));
        return new Rectangle(tab.Right - size - ScriptEditor.DpiHelper.Scale(this, 3),
            tab.Top + Math.Max(0, (tab.Height - size) / 2), size, size);
    }

    private bool ShouldShowCloseButton(int index)
    {
        return ShowCloseButtons && (index == SelectedIndex || index == m_HotTabIndex);
    }
    private void DrawCloseButton(Graphics graphics, int index, bool dark)
    {
        Rectangle bounds = GetCloseButtonRectangle(index);
        if (bounds.IsEmpty || !ShouldShowCloseButton(index))
            return;
        bool hovered = m_HotCloseIndex == index;
        bool pressed = hovered && m_ClosePressedIndex == index;
        if (hovered)
        {
            Color hoverBack = dark
                ? (pressed ? Color.FromArgb(90, 55, 55) : Color.FromArgb(78, 65, 67))
                : (pressed ? Color.FromArgb(225, 190, 190) : Color.FromArgb(235, 215, 215));
            using (Brush hoverBrush = new SolidBrush(hoverBack))
                graphics.FillRectangle(hoverBrush, bounds);
        }
        int inset = Math.Max(3, ScriptEditor.DpiHelper.Scale(this, 4));
        Color closeColor = dark ? Color.Gainsboro : Color.FromArgb(92, 92, 96);
        using (Pen pen = new Pen(closeColor,
            Math.Max(1F, ScriptEditor.DpiHelper.Scale(1.25F, ScriptEditor.DpiHelper.GetDpi(graphics)))))
        {
            graphics.DrawLine(pen, bounds.Left + inset, bounds.Top + inset,
                bounds.Right - inset - 1, bounds.Bottom - inset - 1);
            graphics.DrawLine(pen, bounds.Right - inset - 1, bounds.Top + inset,
                bounds.Left + inset, bounds.Bottom - inset - 1);
        }
    }

    private Rectangle GetOverflowButtonRectangle()
    {
        if (!HasOverflow())
            return Rectangle.Empty;
        int height = TabCount > 0 ? GetTabRect(0).Height : ScriptEditor.DpiHelper.Scale(this, 24);
        int width = ScriptEditor.DpiHelper.Scale(this, 24);
        return new Rectangle(Math.Max(0, ClientSize.Width - width - 2), 1, width, Math.Max(1, height - 1));
    }

    private bool HasOverflow()
    {
        if (!IsHandleCreated || TabCount < 2)
            return false;
        int reserved = ScriptEditor.DpiHelper.Scale(this, 28);
        Rectangle first = GetTabRect(0);
        Rectangle last = GetTabRect(TabCount - 1);
        return first.Left < 0 || last.Left < first.Left || last.Right > ClientSize.Width - reserved;
    }

    private void DrawOverflowButton(Graphics graphics, bool dark)
    {
        Rectangle bounds = GetOverflowButtonRectangle();
        if (bounds.IsEmpty)
            return;
        bool hovered = m_OverflowHot;
        Color back = dark
            ? (hovered ? Color.FromArgb(70, 70, 74) : Color.FromArgb(53, 53, 56))
            : (hovered ? SystemColors.ControlLight : SystemColors.Control);
        Color fore = dark ? Color.Gainsboro : SystemColors.ControlText;
        Color border = dark ? Color.FromArgb(68, 68, 72) : SystemColors.ControlDark;
        using (Brush brush = new SolidBrush(back))
            graphics.FillRectangle(brush, bounds);
        using (Pen pen = new Pen(border))
            graphics.DrawRectangle(pen, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1);
        TextRenderer.DrawText(graphics, "▼", Font, bounds, fore,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }

    private void ShowOverflowMenu()
    {
        if (m_OverflowMenu != null)
            m_OverflowMenu.Dispose();
        m_OverflowMenu = new ContextMenuStrip();
        for (int i = 0; i < TabCount; i++)
        {
            int index = i;
            ToolStripMenuItem item = new ToolStripMenuItem(TabPages[i].Text);
            item.Checked = i == SelectedIndex;
            item.ToolTipText = TabPages[i].ToolTipText;
            item.Click += delegate { SelectedIndex = index; };
            m_OverflowMenu.Items.Add(item);
        }
        ScriptEditor.InterfaceTheme.Apply(m_OverflowMenu);
        Rectangle bounds = GetOverflowButtonRectangle();
        m_OverflowMenu.Show(this, new Point(bounds.Left, bounds.Bottom));
    }
    private void OnMouseDown(object sender, MouseEventArgs e)
    {
        Rectangle overflow = GetOverflowButtonRectangle();
        if (e.Button == MouseButtons.Left && !overflow.IsEmpty && overflow.Contains(e.Location))
        {
            m_DraggedTab = null;
            m_ClosePressedIndex = -1;
            return;
        }
        int closeIndex = TabAtIndex(e.Location);
        if (e.Button == MouseButtons.Left && closeIndex >= 0 &&
            ShouldShowCloseButton(closeIndex) && GetCloseButtonRectangle(closeIndex).Contains(e.Location))
        {
            m_ClosePressedIndex = closeIndex;
            m_DraggedTab = null;
            Invalidate(GetCloseButtonRectangle(closeIndex));
            return;
        }
        m_DraggedTab = TabAt(e.Location);
        m_X = e.X;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        TabPage hoveredTab = TabAt(e.Location);
        int hotIndex = hoveredTab == null ? -1 : TabPages.IndexOf(hoveredTab);
        if (hotIndex != m_HotTabIndex)
        {
            int previousHotIndex = m_HotTabIndex;
            m_HotTabIndex = hotIndex;
            InvalidateTabHeader(previousHotIndex);
            InvalidateTabHeader(m_HotTabIndex);
        }

        int hotCloseIndex = hotIndex >= 0 && ShouldShowCloseButton(hotIndex) && GetCloseButtonRectangle(hotIndex).Contains(e.Location)
            ? hotIndex : -1;
        if (hotCloseIndex != m_HotCloseIndex) {
            InvalidateCloseButton(m_HotCloseIndex);
            m_HotCloseIndex = hotCloseIndex;
            InvalidateCloseButton(m_HotCloseIndex);
        }

        Rectangle overflow = GetOverflowButtonRectangle();
        bool overflowHot = !overflow.IsEmpty && overflow.Contains(e.Location);
        if (overflowHot != m_OverflowHot) {
            m_OverflowHot = overflowHot;
            if (!overflow.IsEmpty)
                Invalidate(overflow, false);
        }

        if (e.Button != MouseButtons.Left || m_ClosePressedIndex >= 0 || m_DraggedTab == null || e.X == m_X)
        {
            return;
        }
        m_X = e.X;

        TabPage tab = TabAt(e.Location);

        if (tab == null || tab == m_DraggedTab)
        {
            return;
        }

        Swap(m_DraggedTab, tab);
    }

    private void OnMouseUp(object sender, MouseEventArgs e)
    {
        m_DraggedTab = null;
        Rectangle overflow = GetOverflowButtonRectangle();
        if (e.Button == MouseButtons.Left && !overflow.IsEmpty && overflow.Contains(e.Location))
        {
            m_ClosePressedIndex = -1;
            ShowOverflowMenu();
            return;
        }

        int closeIndex = m_ClosePressedIndex;
        m_ClosePressedIndex = -1;
        if (e.Button == MouseButtons.Left && closeIndex >= 0 && closeIndex < TabCount &&
            ShouldShowCloseButton(closeIndex) && GetCloseButtonRectangle(closeIndex).Contains(e.Location))
        {
            EventHandler<TabCloseRequestedEventArgs> handler = TabCloseRequested;
            if (handler != null)
                handler(this, new TabCloseRequestedEventArgs(closeIndex));
        }
        InvalidateCloseButton(closeIndex);
    }

    private void OnMouseLeave(object sender, EventArgs e)
    {
        int previousHotIndex = m_HotTabIndex;
        int previousCloseIndex = m_HotCloseIndex;
        Rectangle overflow = GetOverflowButtonRectangle();
        m_HotTabIndex = -1;
        m_HotCloseIndex = -1;
        m_OverflowHot = false;
        InvalidateTabHeader(previousHotIndex);
        InvalidateCloseButton(previousCloseIndex);
        if (!overflow.IsEmpty)
            Invalidate(overflow, false);
    }

    private void InvalidateTabHeader(int index)
    {
        if (index < 0 || index >= TabCount)
            return;
        Rectangle bounds = GetTabRect(index);
        bounds.Inflate(1, 1);
        Invalidate(bounds, false);
    }

    private void InvalidateCloseButton(int index)
    {
        Rectangle bounds = GetCloseButtonRectangle(index);
        if (!bounds.IsEmpty)
            Invalidate(bounds, false);
    }

   /* private void OnMouseUp(object sender, MouseEventArgs e)
    {
        m_DraggedTab = TabAt(e.Location);
    }*/

    private TabPage TabAt(Point position)
    {
        int count = TabCount;

        for (int i = 0; i < count; i++)
        {
            if (GetTabRect(i).Contains(position))
            {
                return TabPages[i];
            }
        }

        return null;
    }

    private int TabAtIndex(Point position)
    {
        for (int i = 0; i < TabCount; i++)
            if (GetTabRect(i).Contains(position))
                return i;
        return -1;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && m_OverflowMenu != null)
            m_OverflowMenu.Dispose();
        base.Dispose(disposing);
    }

    private void Swap(TabPage a, TabPage b)
    {
        int iA = TabPages.IndexOf(a);
        int iB = TabPages.IndexOf(b);

        int d = GetTabRect(iA).Width - GetTabRect(iB).Width;

        if (tabsSwapped != null) {
        	tabsSwapped(this, new TabsSwappedEventArgs(iA, iB));
        }
        TabPages.RemoveAt(iB);
        TabPages.Insert(iA, b);

        if (d < -1) Cursor.Position = new Point((iA > iB) ? Cursor.Position.X + d : Cursor.Position.X - d, Cursor.Position.Y);
    }
}

public sealed class TabCloseRequestedEventArgs : EventArgs
{
    public int TabIndex { get; private set; }

    public TabCloseRequestedEventArgs(int tabIndex)
    {
        TabIndex = tabIndex;
    }
}

public class TabsSwappedEventArgs : EventArgs
{
    public int aIndex { get; set; }
    public int bIndex { get; set; }

    public TabsSwappedEventArgs(int _a, int _b)
    {
    	aIndex = _a;
    	bIndex = _b;
    }
}
