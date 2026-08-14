using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ScriptEditor
{
    internal static class InterfaceTheme
    {
        private static readonly Color DarkBack = Color.FromArgb(40, 40, 42);
        private static readonly Color DarkControl = Color.FromArgb(53, 53, 56);
        private static readonly Color DarkText = Color.Gainsboro;
        private static readonly Color DarkSelection = Color.FromArgb(85, 85, 90);
        private static readonly Color DarkBorder = Color.FromArgb(68, 68, 72);
        private static readonly object GridSectionRowTag = new object();
        private static readonly Dictionary<Form, bool> AppliedForms = new Dictionary<Form, bool>();
        private static readonly HashSet<TabControl> ThemedTabs = new HashSet<TabControl>();
        private static readonly Dictionary<TabControl, TabAppearance> TabAppearances = new Dictionary<TabControl, TabAppearance>();
        private static readonly Dictionary<TabControl, bool> TabMultiline = new Dictionary<TabControl, bool>();
        private static readonly Dictionary<ToolStripStatusLabel, ToolStripStatusLabelBorderSides> StatusBorders = new Dictionary<ToolStripStatusLabel, ToolStripStatusLabelBorderSides>();
        private static readonly Dictionary<ScrollBar, ScrollBarBorderWindow> ScrollBarBorders = new Dictionary<ScrollBar, ScrollBarBorderWindow>();
        private static readonly Dictionary<Control, ControlBorderWindow> ControlBorders = new Dictionary<Control, ControlBorderWindow>();
        private static readonly Dictionary<ButtonBase, FlatStyle> ButtonStyles = new Dictionary<ButtonBase, FlatStyle>();
        private static readonly Dictionary<TextBoxBase, BorderStyle> TextBoxBorders = new Dictionary<TextBoxBase, BorderStyle>();
        private static readonly Dictionary<ComboBox, FlatStyle> ComboStyles = new Dictionary<ComboBox, FlatStyle>();
        private static readonly Dictionary<ComboBox, DrawMode> ComboDrawModes = new Dictionary<ComboBox, DrawMode>();
        private static readonly HashSet<ComboBox> DrawnCombos = new HashSet<ComboBox>();
        private static readonly Dictionary<ComboBox, ComboBoxWindow> ComboWindows = new Dictionary<ComboBox, ComboBoxWindow>();
        private static readonly Dictionary<GroupBox, FlatStyle> GroupStyles = new Dictionary<GroupBox, FlatStyle>();
        private static readonly Dictionary<ListView, bool> ListGridLines = new Dictionary<ListView, bool>();
        private static readonly Dictionary<ListView, ListViewGridWindow> ListGridWindows = new Dictionary<ListView, ListViewGridWindow>();
        private static readonly HashSet<Control> DynamicControls = new HashSet<Control>();
        private static readonly HashSet<ContextMenuStrip> ThemedContextMenus = new HashSet<ContextMenuStrip>();
        private static readonly ToolStripProfessionalRenderer DarkToolStripRenderer = new ToolStripProfessionalRenderer(new DarkColorTable());

        internal static void Start()
        {
                        SetPreferredTheme(IsDark);
Application.Idle += delegate { ApplyToOpenForms(); };
        }

        internal static bool IsDark {
            get {
                if (Settings.interfaceTheme == InterfaceThemeMode.Dark) return true;
                if (Settings.interfaceTheme == InterfaceThemeMode.System) return IsSystemDark();
                return false;
            }
        }

        internal static void Apply(Control control)
        {
            ApplyControl(control, IsDark);
            control.Invalidate(true);
        }
        internal static void Apply(Form form)
        {
            bool dark = IsDark;
                        SetPreferredTheme(dark);
ApplyControl(form, dark);
            SetTitleBarTheme(form, dark);
            AppliedForms[form] = dark;
            form.Invalidate(true);
        }

        internal static void ApplyToOpenForms()
        {
            bool dark = IsDark;
            foreach (Form form in Application.OpenForms) {
                bool appliedDark;
                if (!AppliedForms.TryGetValue(form, out appliedDark) || appliedDark != dark)
                    Apply(form);
            }
        }

        internal static void ApplyGridSectionRow(DataGridViewRow row)
        {
            if (row == null) return;
            row.Tag = GridSectionRowTag;
            ApplyGridSectionRow(row, IsDark);
        }

        private static void ApplyGridSectionRow(DataGridViewRow row, bool dark)
        {
            row.DefaultCellStyle.BackColor = dark ? DarkControl : Color.Gainsboro;
            row.DefaultCellStyle.ForeColor = dark ? DarkText : SystemColors.ControlText;
            row.DefaultCellStyle.SelectionBackColor = dark ? DarkSelection : SystemColors.Highlight;
            row.DefaultCellStyle.SelectionForeColor = dark ? Color.White : SystemColors.HighlightText;
        }

        private static bool IsSystemDark()
        {
            try {
                object value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1);
                return value is int && (int)value == 0;
            } catch { return false; }
        }

        private static void ApplyControl(Control control, bool dark)
        {
            RegisterDynamicTheming(control);
            ApplyNativeTheme(control, dark);
            if (control is ICSharpCode.TextEditor.TextEditorControl) {
                ApplyNativeThemeToChildren(control, dark);
                ApplyScrollBarBorders(control);
                return;
            }

            control.BackColor = dark ? DarkControl : SystemColors.Control;
            control.ForeColor = dark ? DarkText : SystemColors.ControlText;

            TextBoxBase textBox = control as TextBoxBase;
            if (textBox != null) {
                textBox.BackColor = dark ? DarkBack : (textBox.ReadOnly ? SystemColors.ControlLight : SystemColors.Window);
                textBox.ForeColor = dark ? DarkText : SystemColors.WindowText;
                BorderStyle original;
                if (!TextBoxBorders.TryGetValue(textBox, out original)) { original = textBox.BorderStyle; TextBoxBorders.Add(textBox, original); }
                if (dark)
                    textBox.BorderStyle = textBox.Multiline && (textBox.Dock == DockStyle.Fill || textBox.ReadOnly) ? BorderStyle.None : BorderStyle.FixedSingle;
                else
                    textBox.BorderStyle = original;
            }

            ComboBox comboBox = control as ComboBox;
            if (comboBox != null) {
                comboBox.BackColor = dark ? DarkBack : SystemColors.Window;
                comboBox.ForeColor = dark ? DarkText : SystemColors.WindowText;
                DrawMode originalDrawMode;
                if (!ComboDrawModes.TryGetValue(comboBox, out originalDrawMode)) {
                    originalDrawMode = comboBox.DrawMode;
                    ComboDrawModes.Add(comboBox, originalDrawMode);
                }
                comboBox.DrawMode = dark ? DrawMode.OwnerDrawFixed : originalDrawMode;
                if (DrawnCombos.Add(comboBox)) comboBox.DrawItem += DrawComboBoxItem;
                ComboBoxWindow comboWindow;
                if (!ComboWindows.TryGetValue(comboBox, out comboWindow)) {
                    comboWindow = new ComboBoxWindow(comboBox);
                    ComboWindows.Add(comboBox, comboWindow);
                }
                comboWindow.RequestPaint();
            }

            DataGridView grid = control as DataGridView;
            if (grid != null) {
                grid.BackgroundColor = dark ? DarkBack : SystemColors.ControlLight;
                grid.GridColor = dark ? DarkBorder : SystemColors.ControlDark;
                grid.EnableHeadersVisualStyles = !dark;
                grid.DefaultCellStyle.BackColor = dark ? DarkBack : SystemColors.Window;
                grid.DefaultCellStyle.ForeColor = dark ? DarkText : SystemColors.WindowText;
                grid.DefaultCellStyle.SelectionBackColor = dark ? DarkSelection : SystemColors.Highlight;
                grid.DefaultCellStyle.SelectionForeColor = dark ? Color.White : SystemColors.HighlightText;
                grid.ColumnHeadersDefaultCellStyle.BackColor = dark ? DarkControl : SystemColors.Control;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = dark ? DarkText : SystemColors.ControlText;
                grid.RowHeadersDefaultCellStyle.BackColor = dark ? DarkControl : SystemColors.Control;
                grid.RowHeadersDefaultCellStyle.ForeColor = dark ? DarkText : SystemColors.ControlText;
                foreach (DataGridViewRow row in grid.Rows) {
                    if (object.ReferenceEquals(row.Tag, GridSectionRowTag))
                        ApplyGridSectionRow(row, dark);
                }
            }

            ButtonBase button = control as ButtonBase;
            if (button != null) {
                FlatStyle original;
                if (!ButtonStyles.TryGetValue(button, out original)) { original = button.FlatStyle; ButtonStyles.Add(button, original); }
                button.FlatStyle = dark ? FlatStyle.Flat : original;
                if (dark) {
                    button.FlatAppearance.BorderColor = DarkBorder;
                    button.FlatAppearance.MouseOverBackColor = DarkSelection;
                    button.FlatAppearance.MouseDownBackColor = DarkBack;
                }
            }

            ComboBox themedCombo = control as ComboBox;
            if (themedCombo != null) {
                FlatStyle original;
                if (!ComboStyles.TryGetValue(themedCombo, out original)) { original = themedCombo.FlatStyle; ComboStyles.Add(themedCombo, original); }
                themedCombo.FlatStyle = dark ? FlatStyle.Flat : original;
            }

            GroupBox group = control as GroupBox;
            if (group != null) {
                FlatStyle original;
                if (!GroupStyles.TryGetValue(group, out original)) { original = group.FlatStyle; GroupStyles.Add(group, original); }
                group.FlatStyle = dark ? FlatStyle.Flat : original;
            }

            TreeView tree = control as TreeView;
            if (tree != null) {
                tree.BackColor = dark ? DarkBack : SystemColors.Window;
                tree.ForeColor = dark ? DarkText : SystemColors.WindowText;
                tree.LineColor = dark ? DarkBorder : SystemColors.ControlDark;
            }

            ListView list = control as ListView;
            if (list != null) {
                bool originalGridLines;
                if (!ListGridLines.TryGetValue(list, out originalGridLines)) {
                    originalGridLines = list.GridLines;
                    ListGridLines.Add(list, originalGridLines);
                }
                list.BackColor = dark ? DarkBack : SystemColors.Window;
                list.ForeColor = dark ? DarkText : SystemColors.WindowText;
                list.GridLines = dark ? false : originalGridLines;
                ListViewGridWindow gridWindow;
                if (!ListGridWindows.TryGetValue(list, out gridWindow)) {
                    gridWindow = new ListViewGridWindow(list);
                    ListGridWindows.Add(list, gridWindow);
                }
                gridWindow.Enabled = dark && originalGridLines;
            }

            ListBox listBox = control as ListBox;
            if (listBox != null) {
                listBox.BackColor = dark ? DarkBack : SystemColors.Window;
                listBox.ForeColor = dark ? DarkText : SystemColors.WindowText;
            }

            SplitContainer split = control as SplitContainer;
            if (split != null) split.BackColor = dark ? DarkBorder : SystemColors.Control;

            LinkLabel link = control as LinkLabel;
            if (link != null && dark) {
                link.LinkColor = Color.FromArgb(86, 156, 214);
                link.ActiveLinkColor = Color.FromArgb(120, 180, 230);
                link.VisitedLinkColor = Color.FromArgb(170, 130, 210);
            }

            if (dark && ShouldDrawDarkBorder(control)) EnsureControlBorder(control);

            TabControl tabControl = control as TabControl;
            if (tabControl != null) ApplyTabControl(tabControl, dark);

            ToolStrip toolStrip = control as ToolStrip;
            if (toolStrip != null) ApplyToolStrip(toolStrip, dark);

            ContextMenuStrip contextMenu = control.ContextMenuStrip;
            if (contextMenu != null) {
                ApplyToolStrip(contextMenu, dark);
                if (ThemedContextMenus.Add(contextMenu))
                    contextMenu.Opening += delegate { ApplyToolStrip(contextMenu, IsDark); };
            }

            foreach (Control child in control.Controls) ApplyControl(child, dark);
        }

        private static void ApplyTabControl(TabControl tabControl, bool dark)
        {
            TabAppearance appearance;
            if (!TabAppearances.TryGetValue(tabControl, out appearance)) { appearance = tabControl.Appearance; TabAppearances.Add(tabControl, appearance); }
            bool multiline;
            if (!TabMultiline.TryGetValue(tabControl, out multiline)) { multiline = tabControl.Multiline; TabMultiline.Add(tabControl, multiline); }
            tabControl.Appearance = dark ? TabAppearance.FlatButtons : appearance;
            tabControl.Multiline = dark ? true : multiline;
            tabControl.DrawMode = dark ? TabDrawMode.OwnerDrawFixed : TabDrawMode.Normal;
            if (ThemedTabs.Add(tabControl)) tabControl.DrawItem += DrawTab;
            foreach (TabPage page in tabControl.TabPages) {
                page.BackColor = dark ? DarkBack : SystemColors.Control;
                page.ForeColor = dark ? DarkText : SystemColors.ControlText;
            }
        }

        private static void DrawTab(object sender, DrawItemEventArgs e)
        {
            TabControl tabControl = (TabControl)sender;
            if (!IsDark || e.Index < 0 || e.Index >= tabControl.TabPages.Count) return;

            bool selected = (e.State & DrawItemState.Selected) != 0;
            Color back = selected ? DarkBack : DarkControl;
            using (Brush brush = new SolidBrush(back)) e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder(e.Graphics, e.Bounds, DarkBorder, ButtonBorderStyle.Solid);
            TextRenderer.DrawText(e.Graphics, tabControl.TabPages[e.Index].Text, tabControl.Font, e.Bounds,
                DarkText, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static void DrawComboBoxItem(object sender, DrawItemEventArgs e)
        {
            if (!IsDark) return;
            ComboBox comboBox = (ComboBox)sender;
            bool selected = (e.State & DrawItemState.Selected) != 0;
            Color back = selected ? DarkSelection : DarkBack;
            using (Brush brush = new SolidBrush(back)) e.Graphics.FillRectangle(brush, e.Bounds);
            if (e.Index >= 0) {
                TextRenderer.DrawText(e.Graphics, comboBox.GetItemText(comboBox.Items[e.Index]), comboBox.Font,
                    new Rectangle(e.Bounds.X + 2, e.Bounds.Y, System.Math.Max(0, e.Bounds.Width - 4), e.Bounds.Height),
                    DarkText, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }
        private static void ApplyToolStrip(ToolStrip toolStrip, bool dark)
        {
            toolStrip.BackColor = dark ? DarkControl : SystemColors.Control;
            toolStrip.ForeColor = dark ? DarkText : SystemColors.ControlText;
            toolStrip.RenderMode = dark ? ToolStripRenderMode.Professional : ToolStripRenderMode.System;
            if (dark) toolStrip.Renderer = DarkToolStripRenderer;
            ApplyToolStripItems(toolStrip.Items, dark);
        }

        private static void ApplyToolStripItems(ToolStripItemCollection items, bool dark)
        {
            foreach (ToolStripItem item in items) {
                item.BackColor = dark ? DarkControl : SystemColors.Control;
                item.ForeColor = dark ? DarkText : SystemColors.ControlText;
                ToolStripLabel linkLabel = item as ToolStripLabel;
                if (linkLabel != null && linkLabel.IsLink) {
                    linkLabel.LinkColor = dark ? Color.FromArgb(86, 156, 214) : Color.MediumBlue;
                    linkLabel.ActiveLinkColor = dark ? Color.FromArgb(120, 180, 230) : Color.RoyalBlue;
                    linkLabel.VisitedLinkColor = dark ? Color.FromArgb(170, 130, 210) : Color.Purple;
                }
                ToolStripStatusLabel statusLabel = item as ToolStripStatusLabel;
                if (statusLabel != null) {
                    ToolStripStatusLabelBorderSides original;
                    if (!StatusBorders.TryGetValue(statusLabel, out original)) {
                        original = statusLabel.BorderSides;
                        StatusBorders.Add(statusLabel, original);
                    }
                    statusLabel.BorderSides = dark ? ToolStripStatusLabelBorderSides.None : original;
                }
                ToolStripControlHost host = item as ToolStripControlHost;
                if (host != null && host.Control != null) ApplyControl(host.Control, dark);
                ToolStripDropDownItem dropDown = item as ToolStripDropDownItem;
                if (dropDown != null) ApplyToolStrip(dropDown.DropDown, dark);
            }
        }

        private static void RegisterDynamicTheming(Control control)
        {
            if (!DynamicControls.Add(control)) return;
            control.HandleCreated += DynamicHandleCreated;
            control.ControlAdded += DynamicControlAdded;
            control.VisibleChanged += DynamicVisibleChanged;
        }

        private static void DynamicHandleCreated(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;
            bool dark = IsDark;
            ApplyNativeTheme(control, dark);
            ScrollBar scrollBar = control as ScrollBar;
            if (dark && scrollBar != null && !ScrollBarBorders.ContainsKey(scrollBar))
                ScrollBarBorders.Add(scrollBar, new ScrollBarBorderWindow(scrollBar));
        }

        private static void DynamicControlAdded(object sender, ControlEventArgs e)
        {
            ApplyControl(e.Control, IsDark);
        }

        private static void DynamicVisibleChanged(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;
            if (!control.Visible) return;
            bool dark = IsDark;
            ApplyNativeTheme(control, dark);
            ApplyNativeThemeToChildren(control, dark);
            control.Invalidate(true);
        }
        private static bool ShouldDrawDarkBorder(Control control)
        {
            if (control is TextBoxBase || control is ComboBox || control is ListView || control is TreeView ||
                control is DataGridView || control is NumericUpDown) return true;
            Panel panel = control as Panel;
            if (panel != null && panel.BorderStyle != BorderStyle.None) return true;
            PictureBox picture = control as PictureBox;
            return picture != null && picture.BorderStyle != BorderStyle.None;
        }

        private static void EnsureControlBorder(Control control)
        {
            if (!ControlBorders.ContainsKey(control))
                ControlBorders.Add(control, new ControlBorderWindow(control));
        }

        private sealed class ControlBorderWindow : NativeWindow
        {
            private readonly Control control;

            internal ControlBorderWindow(Control control)
            {
                this.control = control;
                AssignHandle(control.Handle);
                control.HandleCreated += delegate { AssignHandle(control.Handle); };
                control.HandleDestroyed += delegate { ReleaseHandle(); };
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if ((m.Msg != 0x000F && m.Msg != 0x0085) || !IsDark || !control.IsHandleCreated) return;
                using (Graphics graphics = Graphics.FromHwnd(control.Handle))
                using (Pen pen = new Pen(DarkBorder))
                {
                    int width = System.Math.Max(0, control.Width - 1);
                    int height = System.Math.Max(0, control.Height - 1);
                    graphics.DrawRectangle(pen, 0, 0, width, height);
                    if (width > 2 && height > 2) graphics.DrawRectangle(pen, 1, 1, width - 2, height - 2);

                }
            }
        }
        private sealed class ComboBoxWindow : NativeWindow
        {
            private const int PaintComboMessage = 0x8001;
            private readonly ComboBox comboBox;
            private bool paintPending;

            internal ComboBoxWindow(ComboBox comboBox)
            {
                this.comboBox = comboBox;
                AssignHandle(comboBox.Handle);
                comboBox.HandleCreated += delegate { AssignHandle(comboBox.Handle); RequestPaint(); };
                comboBox.HandleDestroyed += delegate { paintPending = false; ReleaseHandle(); };
            }

            internal void RequestPaint()
            {
                if (!comboBox.IsHandleCreated || paintPending) return;
                paintPending = true;
                PostMessage(comboBox.Handle, PaintComboMessage, System.IntPtr.Zero, System.IntPtr.Zero);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == PaintComboMessage) {
                    paintPending = false;
                    if (IsDark && comboBox.IsHandleCreated) DrawArrowButton();
                    return;
                }

                base.WndProc(ref m);
                if (m.Msg == 0x000F || m.Msg == 0x0085 || m.Msg == 0x0200 ||
                    m.Msg == 0x0201 || m.Msg == 0x0202 || m.Msg == 0x0215)
                    RequestPaint();
            }

            private void DrawArrowButton()
            {
                ComboBoxInfo info = new ComboBoxInfo();
                info.cbSize = Marshal.SizeOf(typeof(ComboBoxInfo));
                Rectangle buttonBounds;
                if (GetComboBoxInfo(comboBox.Handle, ref info)) {
                    buttonBounds = Rectangle.FromLTRB(System.Math.Max(0, info.rcButton.Left - 2), info.rcButton.Top,
                        info.rcButton.Right, info.rcButton.Bottom);
                } else {
                    int width = SystemInformation.VerticalScrollBarWidth;
                    buttonBounds = new Rectangle(comboBox.ClientSize.Width - width, 0,
                        width, comboBox.ClientSize.Height);
                }

                if (buttonBounds.Width < 1 || buttonBounds.Height < 1) return;
                using (Graphics graphics = Graphics.FromHwnd(comboBox.Handle))
                using (Brush buttonBrush = new SolidBrush(DarkControl))
                using (Brush arrowBrush = new SolidBrush(DarkText))
                using (Pen borderPen = new Pen(DarkBorder)) {
                    graphics.FillRectangle(buttonBrush, buttonBounds);
                    graphics.DrawRectangle(borderPen, buttonBounds.Left, buttonBounds.Top,
                        buttonBounds.Width - 1, buttonBounds.Height - 1);
                    int centerX = buttonBounds.Left + buttonBounds.Width / 2;
                    int centerY = buttonBounds.Top + buttonBounds.Height / 2;
                    Point[] arrow = {
                        new Point(centerX - 4, centerY - 2),
                        new Point(centerX + 4, centerY - 2),
                        new Point(centerX, centerY + 2)
                    };
                    graphics.FillPolygon(arrowBrush, arrow);
                }
            }
        }
        private sealed class ListViewGridWindow : NativeWindow
        {
            private readonly ListView list;

            internal bool Enabled { get; set; }

            internal ListViewGridWindow(ListView list)
            {
                this.list = list;
                AssignHandle(list.Handle);
                list.HandleCreated += delegate { AssignHandle(list.Handle); };
                list.HandleDestroyed += delegate { ReleaseHandle(); };
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if (m.Msg != 0x000F || !Enabled || !IsDark || !list.IsHandleCreated) return;

                int rowHeight = list.Items.Count > 0 ? list.Items[0].Bounds.Height : list.Font.Height + 5;
                if (rowHeight < 1) return;
                using (Graphics graphics = Graphics.FromHwnd(list.Handle))
                using (Pen pen = new Pen(DarkBorder)) {
                    for (int y = rowHeight - 1; y < list.ClientSize.Height; y += rowHeight)
                        graphics.DrawLine(pen, 0, y, list.ClientSize.Width - 1, y);
                }
            }
        }
        private static void ApplyScrollBarBorders(Control control)
        {
            ScrollBar scrollBar = control as ScrollBar;
            if (scrollBar != null && !ScrollBarBorders.ContainsKey(scrollBar))
                ScrollBarBorders.Add(scrollBar, new ScrollBarBorderWindow(scrollBar));
            foreach (Control child in control.Controls) ApplyScrollBarBorders(child);
        }

        private sealed class ScrollBarBorderWindow : NativeWindow
        {
            private readonly ScrollBar scrollBar;

            internal ScrollBarBorderWindow(ScrollBar scrollBar)
            {
                this.scrollBar = scrollBar;
                AssignHandle(scrollBar.Handle);
                scrollBar.HandleCreated += delegate { AssignHandle(scrollBar.Handle); };
                scrollBar.HandleDestroyed += delegate { ReleaseHandle(); };
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if (m.Msg != 0x000F || !IsDark || !scrollBar.IsHandleCreated) return;
                using (Graphics graphics = Graphics.FromHwnd(scrollBar.Handle))
                    DrawDarkScrollBar(graphics);
            }

            private void DrawDarkScrollBar(Graphics graphics)
            {
                bool vertical = scrollBar is VScrollBar;
                int length = vertical ? scrollBar.Height : scrollBar.Width;
                int breadth = vertical ? scrollBar.Width : scrollBar.Height;
                int button = System.Math.Min(breadth, length / 2);
                int trackLength = System.Math.Max(0, length - button * 2);

                using (Brush controlBrush = new SolidBrush(DarkControl))
                using (Brush trackBrush = new SolidBrush(DarkBack))
                using (Brush thumbBrush = new SolidBrush(DarkSelection))
                using (Brush arrowBrush = new SolidBrush(DarkText))
                using (Pen borderPen = new Pen(DarkBorder))
                {
                    graphics.FillRectangle(controlBrush, scrollBar.ClientRectangle);
                    Rectangle track = vertical
                        ? new Rectangle(1, button, System.Math.Max(0, breadth - 2), trackLength)
                        : new Rectangle(button, 1, trackLength, System.Math.Max(0, breadth - 2));
                    graphics.FillRectangle(trackBrush, track);

                    int range = System.Math.Max(1, scrollBar.Maximum - scrollBar.Minimum + 1);
                    int page = System.Math.Max(1, scrollBar.LargeChange);
                    int minimumThumb = System.Math.Min(trackLength, 18);
                    int thumbLength = System.Math.Max(minimumThumb, (int)((long)trackLength * page / range));
                    thumbLength = System.Math.Min(trackLength, thumbLength);
                    int maximumPosition = System.Math.Max(scrollBar.Minimum, scrollBar.Maximum - page + 1);
                    int positionRange = System.Math.Max(1, maximumPosition - scrollBar.Minimum);
                    int movable = System.Math.Max(0, trackLength - thumbLength);
                    int offset = (int)((long)movable * (scrollBar.Value - scrollBar.Minimum) / positionRange);
                    Rectangle thumb = vertical
                        ? new Rectangle(3, button + offset, System.Math.Max(0, breadth - 6), thumbLength)
                        : new Rectangle(button + offset, 3, thumbLength, System.Math.Max(0, breadth - 6));
                    graphics.FillRectangle(thumbBrush, thumb);

                    int center = breadth / 2;
                    int inset = System.Math.Max(3, breadth / 4);
                    if (vertical) {
                        graphics.FillPolygon(arrowBrush, new Point[] { new Point(center, inset), new Point(inset, button - inset), new Point(breadth - inset, button - inset) });
                        graphics.FillPolygon(arrowBrush, new Point[] { new Point(center, length - inset), new Point(inset, length - button + inset), new Point(breadth - inset, length - button + inset) });
                    } else {
                        graphics.FillPolygon(arrowBrush, new Point[] { new Point(inset, center), new Point(button - inset, inset), new Point(button - inset, breadth - inset) });
                        graphics.FillPolygon(arrowBrush, new Point[] { new Point(length - inset, center), new Point(length - button + inset, inset), new Point(length - button + inset, breadth - inset) });
                    }
                    graphics.DrawRectangle(borderPen, 0, 0, System.Math.Max(0, scrollBar.Width - 1), System.Math.Max(0, scrollBar.Height - 1));
                }
            }
        }
        private static void SetTitleBarTheme(Form form, bool dark)
        {
            if (!form.IsHandleCreated) return;
            try {
                int value = dark ? 1 : 0;
                DwmSetWindowAttribute(form.Handle, 20, ref value, sizeof(int));
                DwmSetWindowAttribute(form.Handle, 19, ref value, sizeof(int));
            } catch { }
        }

        private static void ApplyNativeTheme(Control control, bool dark)
        {
            if (!control.IsHandleCreated) return;
            try {
                AllowDarkModeForWindow(control.Handle, dark);
                SetWindowTheme(control.Handle, dark ? "DarkMode_Explorer" : "Explorer", null);
                EnumChildWindows(control.Handle, delegate(System.IntPtr hwnd, System.IntPtr param) {
                    AllowDarkModeForWindow(hwnd, dark);
                    SetWindowTheme(hwnd, dark ? "DarkMode_Explorer" : "Explorer", null);
                    return true;
                }, System.IntPtr.Zero);
            } catch { }
        }

        private static void ApplyNativeThemeToChildren(Control control, bool dark)
        {
            foreach (Control child in control.Controls) {
                ApplyNativeTheme(child, dark);
                ApplyNativeThemeToChildren(child, dark);
            }
        }

        private static void SetPreferredTheme(bool dark)
        {
            try {
                SetPreferredAppMode(dark ? PreferredAppMode.ForceDark : PreferredAppMode.ForceLight);
                ToolStripManager.Renderer = dark ? (ToolStripRenderer)DarkToolStripRenderer : new ToolStripSystemRenderer();
                FlushMenuThemes();
            } catch { }
        }
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(System.IntPtr hwnd, int attribute, ref int attributeValue, int attributeSize);
        private delegate bool EnumChildProc(System.IntPtr hwnd, System.IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeRect
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ComboBoxInfo
        {
            internal int cbSize;
            internal NativeRect rcItem;
            internal NativeRect rcButton;
            internal int stateButton;
            internal System.IntPtr hwndCombo;
            internal System.IntPtr hwndItem;
            internal System.IntPtr hwndList;
        }

        [DllImport("user32.dll")]
        private static extern bool GetComboBoxInfo(System.IntPtr hwndCombo, ref ComboBoxInfo info);
        [DllImport("user32.dll")]
        private static extern bool PostMessage(System.IntPtr hwnd, int message, System.IntPtr wParam, System.IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(System.IntPtr parent, EnumChildProc callback, System.IntPtr lParam);
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(System.IntPtr hwnd, string subAppName, string subIdList);

        [DllImport("uxtheme.dll", EntryPoint = "#133")]
        private static extern bool AllowDarkModeForWindow(System.IntPtr hwnd, bool allow);

        [DllImport("uxtheme.dll", EntryPoint = "#135")]
        private static extern PreferredAppMode SetPreferredAppMode(PreferredAppMode appMode);

        [DllImport("uxtheme.dll", EntryPoint = "#136")]
        private static extern void FlushMenuThemes();

        private enum PreferredAppMode
        {
            Default,
            AllowDark,
            ForceDark,
            ForceLight
        }

        private sealed class DarkColorTable : ProfessionalColorTable
        {
            public override Color ToolStripDropDownBackground { get { return DarkControl; } }
            public override Color ToolStripBorder { get { return DarkBorder; } }
            public override Color MenuBorder { get { return DarkBorder; } }
            public override Color SeparatorDark { get { return DarkBorder; } }
            public override Color SeparatorLight { get { return DarkBorder; } }
            public override Color GripDark { get { return DarkBorder; } }
            public override Color GripLight { get { return DarkBorder; } }
            public override Color MenuItemBorder { get { return DarkBorder; } }
            public override Color MenuItemPressedGradientBegin { get { return DarkBack; } }
            public override Color MenuItemPressedGradientMiddle { get { return DarkBack; } }
            public override Color MenuItemPressedGradientEnd { get { return DarkBack; } }
            public override Color ButtonSelectedBorder { get { return DarkBorder; } }
            public override Color ButtonSelectedGradientMiddle { get { return DarkSelection; } }
            public override Color ButtonSelectedHighlight { get { return DarkSelection; } }
            public override Color ButtonSelectedHighlightBorder { get { return DarkBorder; } }
            public override Color ButtonPressedBorder { get { return DarkBorder; } }
            public override Color ButtonPressedGradientMiddle { get { return DarkBack; } }
            public override Color ButtonPressedHighlight { get { return DarkBack; } }
            public override Color ButtonPressedHighlightBorder { get { return DarkBorder; } }
            public override Color ButtonCheckedGradientBegin { get { return DarkSelection; } }
            public override Color ButtonCheckedGradientMiddle { get { return DarkSelection; } }
            public override Color ButtonCheckedGradientEnd { get { return DarkSelection; } }
            public override Color ButtonCheckedHighlight { get { return DarkSelection; } }
            public override Color ButtonCheckedHighlightBorder { get { return DarkBorder; } }
            public override Color CheckBackground { get { return DarkSelection; } }
            public override Color CheckSelectedBackground { get { return DarkSelection; } }
            public override Color CheckPressedBackground { get { return DarkBack; } }
            public override Color ImageMarginGradientBegin { get { return DarkControl; } }
            public override Color ImageMarginGradientMiddle { get { return DarkControl; } }
            public override Color ImageMarginGradientEnd { get { return DarkControl; } }
            public override Color MenuItemSelected { get { return DarkSelection; } }
            public override Color MenuItemSelectedGradientBegin { get { return DarkSelection; } }
            public override Color MenuItemSelectedGradientEnd { get { return DarkSelection; } }
            public override Color ButtonSelectedGradientBegin { get { return DarkSelection; } }
            public override Color ButtonSelectedGradientEnd { get { return DarkSelection; } }
            public override Color ButtonPressedGradientBegin { get { return DarkBack; } }
            public override Color ButtonPressedGradientEnd { get { return DarkBack; } }
            public override Color OverflowButtonGradientBegin { get { return DarkControl; } }
            public override Color OverflowButtonGradientMiddle { get { return DarkControl; } }
            public override Color OverflowButtonGradientEnd { get { return DarkControl; } }
            public override Color ToolStripGradientBegin { get { return DarkControl; } }
            public override Color ToolStripGradientMiddle { get { return DarkControl; } }
            public override Color ToolStripGradientEnd { get { return DarkControl; } }
            public override Color StatusStripGradientBegin { get { return DarkControl; } }
            public override Color StatusStripGradientEnd { get { return DarkControl; } }
        }
    }
}
