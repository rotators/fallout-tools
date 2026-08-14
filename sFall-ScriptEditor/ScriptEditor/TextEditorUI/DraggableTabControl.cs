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

    [Category("Action")]
    [Description("Fires before tabs are swapped (indexes indicate positions before swap)")]
    public event SwapEventHandler tabsSwapped;

    public DraggableTabControl()
    {
        MouseDown += OnMouseDown;
        MouseMove += OnMouseMove;
        //MouseUp += OnMouseUp;
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
                using (Graphics graphics = Graphics.FromHdc(m.WParam))
                using (Brush brush = new SolidBrush(Color.FromArgb(53, 53, 56)))
                    graphics.FillRectangle(brush, ClientRectangle);
                m.Result = new IntPtr(1);
                return;
            }

            if (m.Msg == WM_PAINT)
            {
                PaintStruct paint;
                IntPtr hdc = BeginPaint(Handle, out paint);
                try
                {
                    using (Graphics graphics = Graphics.FromHdc(hdc))
                        DrawDarkTabControl(graphics);
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

    private void DrawDarkTabControl(Graphics graphics)
    {
        Rectangle page = DisplayRectangle;
        int headerBottom = page.Top;
        for (int i = 0; i < TabCount; i++)
            headerBottom = Math.Max(headerBottom, GetTabRect(i).Bottom);

        using (Brush headerBrush = new SolidBrush(Color.FromArgb(53, 53, 56)))
        using (Brush selectedBrush = new SolidBrush(Color.FromArgb(40, 40, 42)))
        using (Pen pen = new Pen(Color.FromArgb(68, 68, 72)))
        {
            graphics.FillRectangle(headerBrush, ClientRectangle);

            for (int i = 0; i < TabCount; i++)
            {
                Rectangle tab = GetTabRect(i);
                graphics.FillRectangle(i == SelectedIndex ? selectedBrush : headerBrush, tab);
                graphics.DrawRectangle(pen, tab.X, tab.Y, Math.Max(0, tab.Width - 1), Math.Max(0, tab.Height - 1));

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

                TextRenderer.DrawText(graphics, pageTab.Text, Font, textRect, Color.Gainsboro,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            graphics.FillRectangle(headerBrush, 0, Math.Max(0, page.Top - 2), Width, 3);
            graphics.DrawRectangle(pen, 0, 0, Math.Max(0, Width - 1), Math.Max(0, Height - 1));
        }
    }
    private void OnMouseDown(object sender, MouseEventArgs e)
    {
        m_DraggedTab = TabAt(e.Location);
        m_X = e.X;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left || m_DraggedTab == null || e.X == m_X)
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