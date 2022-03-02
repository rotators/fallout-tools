using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

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