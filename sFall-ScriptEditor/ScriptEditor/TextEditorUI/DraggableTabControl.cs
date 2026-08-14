using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;

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

    [Category("Appearance")]
    [DefaultValue(false)]
    [Description("Shows a close button on each tab.")]
    public bool ShowCloseButtons { get; set; }

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
        DrawItem += DrawLightTab;
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

        if (m.Msg == TCM_ADJUSTRECT && m.WParam == IntPtr.Zero && ScriptEditor.InterfaceTheme.IsDark)
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

        if (ScriptEditor.InterfaceTheme.IsDark && IsHandleCreated)
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
                            DrawDarkTabControl(graphics);
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
        if (m.Msg == WM_PAINT && IsHandleCreated)
        {
            using (Graphics graphics = Graphics.FromHwnd(Handle))
                DrawOverflowButton(graphics, false);
        }
    }

    private void DrawDarkTabControl(Graphics graphics)
    {
        Rectangle page = DisplayRectangle;
        int headerBottom = page.Top;
        for (int i = 0; i < TabCount; i++)
            headerBottom = Math.Max(headerBottom, GetTabRect(i).Bottom);

        Color headerColor = Color.FromArgb(53, 53, 56);
        Color selectedColor = Color.FromArgb(40, 40, 42);
        Color hoverColor = Color.FromArgb(60, 60, 64);
        Color borderColor = Color.FromArgb(68, 68, 72);
        Color accentColor = Color.FromArgb(0, 120, 212);
        using (Brush headerBrush = new SolidBrush(headerColor))
        using (Pen pen = new Pen(borderColor))
        {
            graphics.FillRectangle(headerBrush, ClientRectangle);

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
                if (ImageList != null && pageTab.ImageIndex >= 0 && pageTab.ImageIndex < ImageList.Images.Count)
                {
                    Image image = ImageList.Images[pageTab.ImageIndex];
                    int imageY = tab.Y + Math.Max(0, (tab.Height - image.Height) / 2);
                    graphics.DrawImage(image, tab.X + 5, imageY, image.Width, image.Height);
                    textRect.X += image.Width + 7;
                    textRect.Width = Math.Max(0, textRect.Width - image.Width - 7);
                }

                if (ShowCloseButtons)
                    textRect.Width = Math.Max(0, GetCloseButtonRectangle(i).Left - textRect.Left - 3);

                TextRenderer.DrawText(graphics, pageTab.Text, Font, textRect,
                    i == SelectedIndex ? Color.White : Color.Gainsboro,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                if (ShowCloseButtons)
                    DrawCloseButton(graphics, i, true);
            }

            graphics.FillRectangle(headerBrush, 0, Math.Max(0, page.Top - 2), Width, 3);
            graphics.DrawRectangle(pen, 0, 0, Math.Max(0, Width - 1), Math.Max(0, Height - 1));
            DrawOverflowButton(graphics, true);
        }
    }

    private void DrawLightTab(object sender, DrawItemEventArgs e)
    {
        if (ScriptEditor.InterfaceTheme.IsDark || e.Index < 0 || e.Index >= TabCount)
            return;

        bool selected = e.Index == SelectedIndex;
        bool hovered = e.Index == m_HotTabIndex;
        Color background = selected ? SystemColors.Window : (hovered ? SystemColors.ControlLight : SystemColors.Control);
        using (Brush brush = new SolidBrush(background))
            e.Graphics.FillRectangle(brush, e.Bounds);
        using (Pen separator = new Pen(SystemColors.ControlDark))
            e.Graphics.DrawLine(separator, e.Bounds.Right - 1, e.Bounds.Top + 3,
                e.Bounds.Right - 1, e.Bounds.Bottom - 3);
        if (selected)
        {
            int accentHeight = ScriptEditor.DpiHelper.Scale(this, 2);
            using (Brush accent = new SolidBrush(SystemColors.Highlight))
                e.Graphics.FillRectangle(accent, e.Bounds.Left + 2, e.Bounds.Bottom - accentHeight,
                    Math.Max(0, e.Bounds.Width - 4), accentHeight);
        }

        Rectangle textRect = e.Bounds;
        TabPage page = TabPages[e.Index];
        if (ImageList != null && page.ImageIndex >= 0 && page.ImageIndex < ImageList.Images.Count)
        {
            Image image = ImageList.Images[page.ImageIndex];
            int imageY = e.Bounds.Y + Math.Max(0, (e.Bounds.Height - image.Height) / 2);
            e.Graphics.DrawImage(image, e.Bounds.X + 5, imageY, image.Width, image.Height);
            textRect.X += image.Width + 7;
            textRect.Width = Math.Max(0, textRect.Width - image.Width - 7);
        }
        if (ShowCloseButtons)
            textRect.Width = Math.Max(0, GetCloseButtonRectangle(e.Index).Left - textRect.Left - 3);
        TextRenderer.DrawText(e.Graphics, page.Text, Font, textRect, SystemColors.ControlText,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        if (ShowCloseButtons)
            DrawCloseButton(e.Graphics, e.Index, false);
    }

    private Rectangle GetCloseButtonRectangle(int index)
    {
        if (!ShowCloseButtons || index < 0 || index >= TabCount)
            return Rectangle.Empty;
        Rectangle tab = GetTabRect(index);
        int size = Math.Min(ScriptEditor.DpiHelper.Scale(this, 16), Math.Max(0, tab.Height - 4));
        return new Rectangle(tab.Right - size - ScriptEditor.DpiHelper.Scale(this, 4),
            tab.Top + Math.Max(0, (tab.Height - size) / 2), size, size);
    }

    private void DrawCloseButton(Graphics graphics, int index, bool dark)
    {
        Rectangle bounds = GetCloseButtonRectangle(index);
        if (bounds.IsEmpty)
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
        using (Pen pen = new Pen(dark ? Color.Gainsboro : SystemColors.ControlText,
            Math.Max(1F, ScriptEditor.DpiHelper.Scale(1.25F, DeviceDpi))))
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
            GetCloseButtonRectangle(closeIndex).Contains(e.Location))
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

        int hotCloseIndex = hotIndex >= 0 && GetCloseButtonRectangle(hotIndex).Contains(e.Location)
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
            GetCloseButtonRectangle(closeIndex).Contains(e.Location))
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
