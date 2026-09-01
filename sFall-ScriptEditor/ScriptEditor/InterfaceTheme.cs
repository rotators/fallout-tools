using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private static readonly Color DarkAccent = Color.FromArgb(0, 120, 212);
        private static readonly Font UiFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        private static readonly Font UiFontBold = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        private static readonly Font UiFontItalic = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point);
        private static readonly Font UiFontBoldItalic = new Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
        private static readonly object GridSectionRowTag = new object();
        private static readonly Dictionary<Form, bool> AppliedForms = new Dictionary<Form, bool>();
        private static readonly HashSet<Form> TitleBarHookedForms = new HashSet<Form>();
        private static readonly HashSet<TabControl> ThemedTabs = new HashSet<TabControl>();
        private static readonly Dictionary<TabControl, TabAppearance> TabAppearances = new Dictionary<TabControl, TabAppearance>();
        private static readonly Dictionary<TabControl, bool> TabMultiline = new Dictionary<TabControl, bool>();
        private static readonly Dictionary<ToolStripStatusLabel, ToolStripStatusLabelBorderSides> StatusBorders = new Dictionary<ToolStripStatusLabel, ToolStripStatusLabelBorderSides>();
        private static readonly Dictionary<ToolStripItem, Image> LightToolStripImages = new Dictionary<ToolStripItem, Image>();
        private static readonly Dictionary<string, string> DarkToolbarIconKeys = new Dictionary<string, string> {
            { "FunctionButton", "DarkToolbar_Outline" }, { "New_toolStripDropDownButton", "DarkToolbar_New" },
            { "Open_toolStripSplitButton", "DarkToolbar_Open" }, { "Save_toolStripSplitButton", "DarkToolbar_Save" },
            { "tsbSaveAll", "DarkToolbar_SaveAll" }, { "Outline_toolStripButton", "DarkToolbar_FoldUnfold" },
            { "Undo_toolStripButton", "DarkToolbar_Undo" }, { "Redo_ToolStripButton", "DarkToolbar_Redo" },
            { "DecIndentStripButton", "DarkToolbar_Indent" }, { "CommentStripButton", "DarkToolbar_Comment" },
            { "Search_toolStripButton", "DarkToolbar_Find" }, { "Back_toolStripButton", "DarkToolbar_Back" },
            { "Forward_toolStripButton", "DarkToolbar_Forward" }, { "GotoProc_StripButton", "DarkToolbar_Goto" },
            { "Edit_toolStripButton", "DarkToolbar_Code" }, { "Script_toolStripSplitButton", "DarkToolbar_List" },
            { "Headers_toolStripSplitButton", "DarkToolbar_Include" }, { "MSG_toolStripButton", "DarkToolbar_Dialog" },
            { "qCompile_toolStripSplitButton", "DarkToolbar_Compile" }, { "toolStripDropDownButton2", "DarkToolbar_Options" },
            { "GoBeginStripButton", "DarkToolbar_GoDefinitions" }, { "OnlyProcStripButton", "DarkToolbar_CollapseFolders" },
            { "NewProcStripButton", "DarkToolbar_CreateProcedure" }, { "tsbUpdateParserData", "DarkToolbar_RefreshParser" },
            { "Help_toolStripButton", "DarkToolbar_Help" }, { "ViewArgsStripButton", "DarkToolbar_QuickTips" },
            { "Save_button", "DarkToolbar_ScriptListSaved" }, { "Addbutton", "DarkToolbar_ScriptListAdd" },
            { "Delbutton", "DarkToolbar_ScriptListRemove" }, { "Upbutton", "DarkToolbar_ScriptListFindPrevious" },
            { "Downbutton", "DarkToolbar_ScriptListFindNext" }, { "toolStripLabel1", "DarkToolbar_ScriptListFind" }
        };
        private static readonly Dictionary<ButtonBase, FlatStyle> ButtonStyles = new Dictionary<ButtonBase, FlatStyle>();
        private static readonly Dictionary<Button, Image> LightButtonImages = new Dictionary<Button, Image>();
        private static readonly Dictionary<string, string> DarkButtonIconKeys = new Dictionary<string, string> {
            { "minimizelog_button", "DarkToolbar_OutputPane" },
            { "Split_button", "DarkToolbar_SplitDocument" }
        };
        private static readonly Dictionary<ComboBox, FlatStyle> ComboStyles = new Dictionary<ComboBox, FlatStyle>();
        private static readonly HashSet<ComboBox> PopupThemedCombos = new HashSet<ComboBox>();
        private static readonly Dictionary<GroupBox, FlatStyle> GroupStyles = new Dictionary<GroupBox, FlatStyle>();
        private static readonly Dictionary<DataGridView, DataGridViewHeaderBorderStyle> GridHeaderBorders = new Dictionary<DataGridView, DataGridViewHeaderBorderStyle>();
        private static readonly Dictionary<DataGridView, DataGridViewCellBorderStyle> GridCellBorders = new Dictionary<DataGridView, DataGridViewCellBorderStyle>();
        private static readonly HashSet<DataGridView> ThemedGrids = new HashSet<DataGridView>();
        private static readonly Dictionary<DataGridViewColumn, GridColumnSelectionStyle> GridColumnSelectionStyles = new Dictionary<DataGridViewColumn, GridColumnSelectionStyle>();
        private static readonly Dictionary<DataGridViewComboBoxColumn, GridComboBoxColumnStyle> GridComboBoxColumnStyles = new Dictionary<DataGridViewComboBoxColumn, GridComboBoxColumnStyle>();
        private static readonly HashSet<Control> DynamicControls = new HashSet<Control>();
        private static readonly HashSet<ContextMenuStrip> ThemedContextMenus = new HashSet<ContextMenuStrip>();
        private static readonly ToolStripProfessionalRenderer DarkToolStripRenderer = new DarkRenderer();
        private static readonly Image DarkHelpIcon = CreateHelpIcon(true);
        private static readonly Image LightHelpIcon = CreateHelpIcon(false);

        private static readonly bool SupportsDarkMode =
            Environment.OSVersion.Version.Major > 6 ||
            (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1);

        private sealed class GridColumnSelectionStyle
        {
            internal Color BackColor;
            internal Color ForeColor;
        }

        private sealed class GridComboBoxColumnStyle
        {
            internal Color BackColor;
            internal Color ForeColor;
            internal FlatStyle FlatStyle;
        }

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
            TextEditor editor = form as TextEditor;
            if (editor != null)
                TextEditorUI.ColorTheme.ApplyRightPanelTheme();
            if (TitleBarHookedForms.Add(form))
                form.HandleCreated += delegate { SetTitleBarTheme(form, IsDark); };
            SetTitleBarTheme(form, dark);
            AppliedForms[form] = dark;
            form.Invalidate(true);
        }

        internal static void ApplyOnLoad(Form form)
        {
            bool deferFirstPaint = IsDark && form.Opacity > 0D;
            double originalOpacity = form.Opacity;
            if (deferFirstPaint)
                form.Opacity = 0D;

            Apply(form);
            form.HandleCreated += delegate { Apply(form); };
            form.Load += delegate { Apply(form); };
            form.Shown += delegate {
                Apply(form);
                form.PerformLayout();
                form.Update();
                if (!deferFirstPaint)
                    return;
                form.BeginInvoke((MethodInvoker)delegate {
                    if (form.IsDisposed || !form.IsHandleCreated)
                        return;
                    Apply(form);
                    form.PerformLayout();
                    form.Update();
                    form.Opacity = originalOpacity;
                });
            };
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
            ApplyTypography(control);
            ApplyNativeTheme(control, dark);
            if (control is ICSharpCode.TextEditor.TextEditorControl) {
                ApplyTextEditorHostColors(control, dark);
                RegisterDynamicThemingToChildren(control);
                ApplyNativeThemeToChildren(control, dark);
                return;
            }

            control.BackColor = dark ? DarkControl : SystemColors.Control;
            control.ForeColor = dark ? DarkText : SystemColors.ControlText;

            TextBoxBase textBox = control as TextBoxBase;
            if (textBox != null) {
                textBox.BackColor = dark ? DarkBack : (textBox.ReadOnly ? SystemColors.ControlLight : SystemColors.Window);
                textBox.ForeColor = dark ? DarkText : SystemColors.WindowText;
            }

            ComboBox comboBox = control as ComboBox;
            if (comboBox != null) {
                comboBox.BackColor = dark ? DarkBack : SystemColors.Window;
                comboBox.ForeColor = dark ? DarkText : SystemColors.WindowText;
                if (PopupThemedCombos.Add(comboBox)) comboBox.DropDown += ApplyComboBoxPopupTheme;
            }

            ProgressBar progressBar = control as ProgressBar;
            if (progressBar != null)
                ApplyNativeProgressBar(progressBar, dark);

            DataGridView grid = control as DataGridView;
            if (grid != null) {
                grid.BackgroundColor = dark ? DarkBack : SystemColors.ControlLight;
                grid.GridColor = dark ? Color.FromArgb(56, 56, 60) : SystemColors.ControlDark;
                grid.EnableHeadersVisualStyles = !dark;
                grid.DefaultCellStyle.BackColor = dark ? DarkBack : SystemColors.Window;
                grid.DefaultCellStyle.ForeColor = dark ? DarkText : SystemColors.WindowText;
                grid.DefaultCellStyle.SelectionBackColor = dark ? DarkSelection : SystemColors.Highlight;
                grid.DefaultCellStyle.SelectionForeColor = dark ? Color.White : SystemColors.HighlightText;
                grid.RowsDefaultCellStyle.BackColor = dark ? DarkBack : SystemColors.Window;
                grid.RowsDefaultCellStyle.ForeColor = dark ? DarkText : SystemColors.WindowText;
                grid.RowsDefaultCellStyle.SelectionBackColor = dark ? DarkSelection : SystemColors.Highlight;
                grid.RowsDefaultCellStyle.SelectionForeColor = dark ? Color.White : SystemColors.HighlightText;
                grid.AlternatingRowsDefaultCellStyle.BackColor = dark ? DarkBack : SystemColors.Window;
                grid.AlternatingRowsDefaultCellStyle.ForeColor = dark ? DarkText : SystemColors.WindowText;
                grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = dark ? DarkSelection : SystemColors.Highlight;
                grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = dark ? Color.White : SystemColors.HighlightText;
                grid.ColumnHeadersDefaultCellStyle.BackColor = dark ? Color.FromArgb(62, 62, 66) : SystemColors.Control;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = dark ? DarkText : SystemColors.ControlText;
                grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = dark ? Color.FromArgb(62, 62, 66) : SystemColors.Highlight;
                grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = dark ? DarkText : SystemColors.HighlightText;
                grid.RowHeadersDefaultCellStyle.BackColor = dark ? DarkControl : SystemColors.Control;
                grid.RowHeadersDefaultCellStyle.ForeColor = dark ? DarkText : SystemColors.ControlText;
                DataGridViewHeaderBorderStyle originalHeaderBorder;
                if (!GridHeaderBorders.TryGetValue(grid, out originalHeaderBorder)) {
                    originalHeaderBorder = grid.ColumnHeadersBorderStyle;
                    GridHeaderBorders.Add(grid, originalHeaderBorder);
                }
                grid.ColumnHeadersBorderStyle = dark ? DataGridViewHeaderBorderStyle.None : originalHeaderBorder;
                DataGridViewCellBorderStyle originalCellBorder;
                if (!GridCellBorders.TryGetValue(grid, out originalCellBorder)) {
                    originalCellBorder = grid.CellBorderStyle;
                    GridCellBorders.Add(grid, originalCellBorder);
                }
                grid.CellBorderStyle = dark && originalCellBorder == DataGridViewCellBorderStyle.Raised
                    ? DataGridViewCellBorderStyle.Single : originalCellBorder;
                if (ThemedGrids.Add(grid)) {
                    grid.EditingControlShowing += GridEditingControlShowing;
                    grid.CellPainting += GridCellPainting;
                }
                foreach (DataGridViewColumn column in grid.Columns)
                    ApplyGridColumnSelectionStyle(column, dark);
                foreach (DataGridViewRow row in grid.Rows) {
                    if (object.ReferenceEquals(row.Tag, GridSectionRowTag))
                        ApplyGridSectionRow(row, dark);
                }
            }

            Button button = control as Button;
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

            Button imageButton = control as Button;
            if (imageButton != null) ApplyButtonImage(imageButton, dark);

            ComboBox themedCombo = control as ComboBox;
            if (themedCombo != null) {
                FlatStyle original;
                if (!ComboStyles.TryGetValue(themedCombo, out original)) { original = themedCombo.FlatStyle; ComboStyles.Add(themedCombo, original); }
                FlatStyle targetFlatStyle = dark ? FlatStyle.Standard : original;
                if (themedCombo.FlatStyle != targetFlatStyle)
                    themedCombo.FlatStyle = targetFlatStyle;
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
                list.BackColor = dark ? DarkBack : SystemColors.Window;
                list.ForeColor = dark ? DarkText : SystemColors.WindowText;
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

        private static void ApplyTextEditorHostColors(Control editor, bool dark)
        {
            Color back = dark ? DarkBack : SystemColors.Window;
            Color fore = dark ? DarkText : SystemColors.WindowText;
            editor.BackColor = back;
            editor.ForeColor = fore;
            ApplyTextEditorPanelColors(editor, back, fore);
        }

        private static void ApplyTextEditorPanelColors(Control parent, Color back, Color fore)
        {
            foreach (Control child in parent.Controls) {
                Panel panel = child as Panel;
                if (panel != null) {
                    panel.BackColor = back;
                    panel.ForeColor = fore;
                }
                ApplyTextEditorPanelColors(child, back, fore);
            }
        }

        private static void GridEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            ApplyControl(e.Control, IsDark);
        }

        private static void GridCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (!IsDark || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridView grid = (DataGridView)sender;
            DataGridViewButtonColumn buttonColumn = grid.Columns[e.ColumnIndex] as DataGridViewButtonColumn;
            if (buttonColumn != null) {
                bool buttonSelected = (e.State & DataGridViewElementStates.Selected) != 0;
                Rectangle buttonCellBounds = e.CellBounds;
                Rectangle buttonBounds = Rectangle.Inflate(buttonCellBounds, -2, -2);
                using (SolidBrush backgroundBrush = new SolidBrush(buttonSelected ? DarkSelection : DarkBack))
                using (SolidBrush buttonBrush = new SolidBrush(buttonSelected ? DarkSelection : DarkControl))
                using (Pen borderPen = new Pen(DarkBorder)) {
                    e.Graphics.FillRectangle(backgroundBrush, buttonCellBounds);
                    e.Graphics.FillRectangle(buttonBrush, buttonBounds);
                    e.Graphics.DrawRectangle(borderPen, buttonBounds.X, buttonBounds.Y,
                        buttonBounds.Width - 1, buttonBounds.Height - 1);
                    TextRenderer.DrawText(e.Graphics, e.FormattedValue == null ? string.Empty : e.FormattedValue.ToString(),
                        e.CellStyle.Font, buttonBounds, DarkText,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
                e.Handled = true;
                return;
            }

            if (!(grid.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn) ||
                (grid.IsCurrentCellInEditMode && grid.CurrentCell != null &&
                 grid.CurrentCell.RowIndex == e.RowIndex && grid.CurrentCell.ColumnIndex == e.ColumnIndex))
                return;

            bool selected = (e.State & DataGridViewElementStates.Selected) != 0;
            Color background = selected ? DarkSelection : DarkBack;
            Rectangle bounds = e.CellBounds;
            using (SolidBrush backgroundBrush = new SolidBrush(background))
            using (Pen borderPen = new Pen(DarkBorder))
            using (Pen arrowPen = new Pen(DarkText)) {
                e.Graphics.FillRectangle(backgroundBrush, bounds);
                e.Graphics.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);

                Rectangle arrowBounds = new Rectangle(bounds.Right - 18, bounds.Top + 1, 17, bounds.Height - 2);
                e.Graphics.DrawLine(borderPen, arrowBounds.Left, arrowBounds.Top, arrowBounds.Left, arrowBounds.Bottom);
                int centerX = arrowBounds.Left + arrowBounds.Width / 2;
                int centerY = arrowBounds.Top + arrowBounds.Height / 2;
                e.Graphics.DrawLine(arrowPen, centerX - 3, centerY - 1, centerX, centerY + 2);
                e.Graphics.DrawLine(arrowPen, centerX, centerY + 2, centerX + 3, centerY - 1);

                Rectangle textBounds = new Rectangle(bounds.Left + 4, bounds.Top + 1, arrowBounds.Left - bounds.Left - 6, bounds.Height - 2);
                TextRenderer.DrawText(e.Graphics, e.FormattedValue == null ? string.Empty : e.FormattedValue.ToString(), e.CellStyle.Font,
                    textBounds, DarkText, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
            e.Handled = true;
        }

        private static void ApplyGridColumnSelectionStyle(DataGridViewColumn column, bool dark)
        {
            GridColumnSelectionStyle original;
            if (!GridColumnSelectionStyles.TryGetValue(column, out original)) {
                original = new GridColumnSelectionStyle {
                    BackColor = column.DefaultCellStyle.SelectionBackColor,
                    ForeColor = column.DefaultCellStyle.SelectionForeColor
                };
                GridColumnSelectionStyles.Add(column, original);
            }

            column.DefaultCellStyle.SelectionBackColor = dark ? DarkSelection : original.BackColor;
            column.DefaultCellStyle.SelectionForeColor = dark ? Color.White : original.ForeColor;

            DataGridViewComboBoxColumn comboBoxColumn = column as DataGridViewComboBoxColumn;
            if (comboBoxColumn == null)
                return;

            GridComboBoxColumnStyle comboBoxOriginal;
            if (!GridComboBoxColumnStyles.TryGetValue(comboBoxColumn, out comboBoxOriginal)) {
                comboBoxOriginal = new GridComboBoxColumnStyle {
                    BackColor = comboBoxColumn.DefaultCellStyle.BackColor,
                    ForeColor = comboBoxColumn.DefaultCellStyle.ForeColor,
                    FlatStyle = comboBoxColumn.FlatStyle
                };
                GridComboBoxColumnStyles.Add(comboBoxColumn, comboBoxOriginal);
            }

            comboBoxColumn.DefaultCellStyle.BackColor = dark ? DarkBack : comboBoxOriginal.BackColor;
            comboBoxColumn.DefaultCellStyle.ForeColor = dark ? DarkText : comboBoxOriginal.ForeColor;
            comboBoxColumn.FlatStyle = dark ? FlatStyle.Flat : comboBoxOriginal.FlatStyle;
        }

        private static void ApplyTypography(Control control)
        {
            if (control is ICSharpCode.TextEditor.TextEditorControl)
                return;

            if (UsesLegacyUiFont(control.Font))
                control.Font = GetUiFont(control.Font.Style);

            ToolStrip toolStrip = control as ToolStrip;
            if (toolStrip != null) {
                toolStrip.Font = UiFont;
                ApplyToolStripTypography(toolStrip.Items);
            }
        }

        internal static Color DialogOptionTextColor {
            get { return IsDark ? Color.FromArgb(86, 156, 214) : Color.Blue; }
        }

        internal static Color DialogErrorTextColor {
            get { return IsDark ? Color.FromArgb(244, 113, 116) : Color.Red; }
        }

        internal static void ApplyDialogGridRow(DataGridViewRow row)
        {
            if (row == null || !IsDark)
                return;
            foreach (DataGridViewCell cell in row.Cells) {
                cell.Style.SelectionBackColor = DarkSelection;
                cell.Style.SelectionForeColor = Color.White;
            }
        }

        private static void ApplyToolStripTypography(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items) {
                if (UsesLegacyUiFont(item.Font))
                    item.Font = GetUiFont(item.Font.Style);
                ToolStripDropDownItem dropDown = item as ToolStripDropDownItem;
                if (dropDown != null)
                    ApplyToolStripTypography(dropDown.DropDownItems);
            }
        }

        private static bool UsesLegacyUiFont(Font font)
        {
            if (font == null) return false;
            string name = font.FontFamily.Name;
            return name == "Microsoft Sans Serif" || name == "Arial" || name == "Tahoma";
        }

        private static Font GetUiFont(FontStyle style)
        {
            bool bold = (style & FontStyle.Bold) != 0;
            bool italic = (style & FontStyle.Italic) != 0;
            if (bold && italic) return UiFontBoldItalic;
            if (bold) return UiFontBold;
            if (italic) return UiFontItalic;
            return UiFont;
        }

        private static void ApplyTabControl(TabControl tabControl, bool dark)
        {
            TabAppearance appearance;
            if (!TabAppearances.TryGetValue(tabControl, out appearance)) { appearance = tabControl.Appearance; TabAppearances.Add(tabControl, appearance); }
            bool multiline;
            if (!TabMultiline.TryGetValue(tabControl, out multiline)) { multiline = tabControl.Multiline; TabMultiline.Add(tabControl, multiline); }
            tabControl.Appearance = appearance;
            tabControl.Multiline = multiline;
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
            bool hovered = (e.State & DrawItemState.HotLight) != 0;
            Color back = selected ? DarkBack : (hovered ? Color.FromArgb(60, 60, 64) : DarkControl);
            using (Brush brush = new SolidBrush(back)) e.Graphics.FillRectangle(brush, e.Bounds);
            using (Pen separator = new Pen(DarkBorder))
                e.Graphics.DrawLine(separator, e.Bounds.Right - 1, e.Bounds.Top + 3,
                    e.Bounds.Right - 1, e.Bounds.Bottom - 3);
            if (selected) {
                int accentHeight = DpiHelper.Scale(tabControl, 2);
                using (Brush accent = new SolidBrush(DarkAccent))
                    e.Graphics.FillRectangle(accent, e.Bounds.Left + 2,
                        e.Bounds.Bottom - accentHeight, System.Math.Max(0, e.Bounds.Width - 4), accentHeight);
            }
            TextRenderer.DrawText(e.Graphics, tabControl.TabPages[e.Index].Text, tabControl.Font, e.Bounds,
                selected ? Color.White : DarkText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
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
                ApplyToolStripImage(item, dark);
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

        private static void ApplyButtonImage(Button button, bool dark)
        {
            Image lightImage;
            if (!LightButtonImages.TryGetValue(button, out lightImage)) {
                lightImage = button.Image;
                LightButtonImages.Add(button, lightImage);
            }

            string resourceKey;
            if (dark && DarkButtonIconKeys.TryGetValue(button.Name, out resourceKey)) {
                Image darkImage = Properties.Resources.ResourceManager.GetObject(resourceKey) as Image;
                if (darkImage != null) {
                    button.Image = darkImage;
                    return;
                }
            }
            button.Image = lightImage;
        }
        private static void ApplyToolStripImage(ToolStripItem item, bool dark)
        {
            Image lightImage;
            if (!LightToolStripImages.TryGetValue(item, out lightImage)) {
                lightImage = item.Image;
                LightToolStripImages.Add(item, lightImage);
            }

            string resourceKey;
            if (dark && item.Name == "Save_button" && item.Tag is bool && (bool)item.Tag) {
                resourceKey = "DarkToolbar_ScriptListUnsaved";
                Image unsavedImage = Properties.Resources.ResourceManager.GetObject(resourceKey) as Image;
                if (unsavedImage != null) {
                    item.Image = unsavedImage;
                    return;
                }
            }
            if (dark && DarkToolbarIconKeys.TryGetValue(item.Name, out resourceKey)) {
                Image darkImage = Properties.Resources.ResourceManager.GetObject(resourceKey) as Image;
                if (darkImage != null) {
                    item.Image = darkImage;
                    return;
                }
            }
            item.Image = lightImage;
        }
        private static void RegisterDynamicTheming(Control control)
        {
            if (!DynamicControls.Add(control)) return;
            control.HandleCreated += DynamicHandleCreated;
            control.ControlAdded += DynamicControlAdded;
            control.VisibleChanged += DynamicVisibleChanged;
            control.Disposed += DynamicControlDisposed;
        }

        private static void DynamicControlDisposed(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;
            DynamicControls.Remove(control);

            Form form = control as Form;
            if (form != null) {
                AppliedForms.Remove(form);
                TitleBarHookedForms.Remove(form);
            }
            TabControl tabControl = control as TabControl;
            if (tabControl != null) {
                ThemedTabs.Remove(tabControl);
                TabAppearances.Remove(tabControl);
                TabMultiline.Remove(tabControl);
            }
            ToolStrip toolStrip = control as ToolStrip;
            if (toolStrip != null) {
                ContextMenuStrip contextMenu = toolStrip as ContextMenuStrip;
                if (contextMenu != null)
                    ThemedContextMenus.Remove(contextMenu);
                RemoveToolStripItemThemeState(toolStrip.Items);
            }
            Button button = control as Button;
            if (button != null)
                ButtonStyles.Remove(button);
            Button imageButton = control as Button;
            if (imageButton != null)
                LightButtonImages.Remove(imageButton);
            ComboBox comboBox = control as ComboBox;
            if (comboBox != null) {
                ComboStyles.Remove(comboBox);
                PopupThemedCombos.Remove(comboBox);
            }
            GroupBox groupBox = control as GroupBox;
            if (groupBox != null)
                GroupStyles.Remove(groupBox);
            DataGridView grid = control as DataGridView;
            if (grid != null) {
                GridHeaderBorders.Remove(grid);
                GridCellBorders.Remove(grid);
                ThemedGrids.Remove(grid);
                foreach (DataGridViewColumn column in grid.Columns) {
                    GridColumnSelectionStyles.Remove(column);
                    DataGridViewComboBoxColumn comboBoxColumn = column as DataGridViewComboBoxColumn;
                    if (comboBoxColumn != null)
                        GridComboBoxColumnStyles.Remove(comboBoxColumn);
                }
            }
        }

        private static void RemoveToolStripItemThemeState(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items) {
                LightToolStripImages.Remove(item);
                ToolStripStatusLabel statusLabel = item as ToolStripStatusLabel;
                if (statusLabel != null)
                    StatusBorders.Remove(statusLabel);
                ToolStripDropDownItem dropDown = item as ToolStripDropDownItem;
                if (dropDown != null)
                    RemoveToolStripItemThemeState(dropDown.DropDownItems);
            }
        }

        private static void DynamicHandleCreated(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;
            ApplyNativeTheme(control, IsDark);
            control.Invalidate();
        }

        private static void RegisterDynamicThemingToChildren(Control control)
        {
            foreach (Control child in control.Controls) {
                RegisterDynamicTheming(child);
                RegisterDynamicThemingToChildren(child);
            }
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
        private static void ApplyComboBoxPopupTheme(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null || !comboBox.IsHandleCreated) return;

            ComboBoxInfo info = new ComboBoxInfo();
            info.cbSize = Marshal.SizeOf(typeof(ComboBoxInfo));
            if (!GetComboBoxInfo(comboBox.Handle, ref info) || info.hwndList == System.IntPtr.Zero)
                return;

            bool dark = IsDark;
            string theme = dark ? "DarkMode_Explorer" : "Explorer";
            if (SupportsDarkMode) {
                try {
                    AllowDarkModeForWindow(info.hwndList, dark);
                    EnumChildWindows(info.hwndList, delegate(System.IntPtr hwnd, System.IntPtr param) {
                        AllowDarkModeForWindow(hwnd, dark);
                        SetWindowTheme(hwnd, theme, null);
                        return true;
                    }, System.IntPtr.Zero);
                } catch { }
            }
            SetWindowTheme(info.hwndList, theme, null);
            InvalidateRect(info.hwndList, System.IntPtr.Zero, true);
        }
        private static void ApplyNativeProgressBar(ProgressBar progressBar, bool dark)
        {
            if (!progressBar.IsHandleCreated)
                return;

            if (dark) {
                SetWindowTheme(progressBar.Handle, string.Empty, string.Empty);
                SendMessage(progressBar.Handle, PbmSetBkColor, System.IntPtr.Zero, ToColorRef(DarkBack));
                SendMessage(progressBar.Handle, PbmSetBarColor, System.IntPtr.Zero, ToColorRef(DarkAccent));
            } else {
                SetWindowTheme(progressBar.Handle, "Explorer", null);
                SendMessage(progressBar.Handle, PbmSetBkColor, System.IntPtr.Zero, new System.IntPtr(-1));
                SendMessage(progressBar.Handle, PbmSetBarColor, System.IntPtr.Zero, new System.IntPtr(-1));
            }
            InvalidateRect(progressBar.Handle, System.IntPtr.Zero, true);
        }

        private static System.IntPtr ToColorRef(Color color)
        {
            return new System.IntPtr(color.R | (color.G << 8) | (color.B << 16));
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
                if (SupportsDarkMode)
                    AllowDarkModeForWindow(control.Handle, dark);
                // Let Windows paint standard control chrome and pressed/focus states.
                // Text inputs only need their managed colours changed; replacing their
                // native theme also recalculates the Designer-assigned control height.
                TextBox standardTextBox = control as TextBox;
                bool hasNativeScrollbar = control is RichTextBox ||
                    (standardTextBox != null && standardTextBox.Multiline && standardTextBox.ScrollBars != ScrollBars.None);
                bool darkComboBox = dark && control is ComboBox;
                bool darkInput = dark && control is TextBoxBase && !hasNativeScrollbar;
                if (darkInput) return;
                string theme = darkComboBox ? "DarkMode_CFD" : (dark ? "DarkMode_Explorer" : "Explorer");
                SetWindowTheme(control.Handle, theme, null);
                EnumChildWindows(control.Handle, delegate(System.IntPtr hwnd, System.IntPtr param) {
                    if (SupportsDarkMode)
                        AllowDarkModeForWindow(hwnd, dark);
                    SetWindowTheme(hwnd, theme, null);
                    return true;
                }, System.IntPtr.Zero);
                // Native TreeView scrolling can leave stale one-pixel row fragments,
                // especially with a custom background colour. Its extended double-
                // buffer style must be applied after the handle exists and reapplied
                // whenever Windows recreates that handle.
                if (control is TreeView)
                    SendMessage(control.Handle, TvmSetExtendedStyle,
                        new System.IntPtr(TvsExDoubleBuffer), new System.IntPtr(TvsExDoubleBuffer));
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
                if (SupportsDarkMode)
                    SetPreferredAppMode(dark ? PreferredAppMode.ForceDark : PreferredAppMode.ForceLight);
                ToolStripManager.Renderer = dark ? (ToolStripRenderer)DarkToolStripRenderer : new ToolStripSystemRenderer();
                if (SupportsDarkMode)
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
        private static extern System.IntPtr SendMessage(System.IntPtr hwnd, int message, System.IntPtr wParam, System.IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(System.IntPtr parent, EnumChildProc callback, System.IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern bool InvalidateRect(System.IntPtr hWnd, System.IntPtr rect, bool erase);
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(System.IntPtr hwnd, string subAppName, string subIdList);

        [DllImport("uxtheme.dll", EntryPoint = "#133")]
        private static extern bool AllowDarkModeForWindow(System.IntPtr hwnd, bool allow);

        [DllImport("uxtheme.dll", EntryPoint = "#135")]
        private static extern PreferredAppMode SetPreferredAppMode(PreferredAppMode appMode);

        [DllImport("uxtheme.dll", EntryPoint = "#136")]
        private static extern void FlushMenuThemes();

        private const int PbmSetBarColor = 0x0409;
        private const int PbmSetBkColor = 0x2001;
        private const int TvmSetExtendedStyle = 0x112C;
        private const int TvsExDoubleBuffer = 0x0004;

        private enum PreferredAppMode
        {
            Default,
            AllowDark,
            ForceDark,
            ForceLight
        }

        private sealed class DarkRenderer : ToolStripProfessionalRenderer
        {
            internal DarkRenderer() : base(new DarkColorTable()) { }
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                if (IsDark && e.ToolStrip is StatusStrip) {
                    using (Pen pen = new Pen(DarkControl))
                        e.Graphics.DrawLine(pen, 0, 0, System.Math.Max(0, e.ToolStrip.Width - 1), 0);
                    return;
                }

                base.OnRenderToolStripBorder(e);
            }

            protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
            {
                base.OnRenderItemBackground(e);
                if (!IsDark) return;

                ToolStripStatusLabel statusLabel = e.Item as ToolStripStatusLabel;
                ToolStripStatusLabelBorderSides sides;
                if (statusLabel == null || !StatusBorders.TryGetValue(statusLabel, out sides) ||
                    sides == ToolStripStatusLabelBorderSides.None) return;

                Rectangle bounds = e.Item.Bounds;
                if (bounds.Width < 1 || bounds.Height < 1) return;
                using (Pen pen = new Pen(DarkBorder)) {
                    if ((sides & ToolStripStatusLabelBorderSides.Left) != 0)
                        e.Graphics.DrawLine(pen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom - 1);
                    if ((sides & ToolStripStatusLabelBorderSides.Top) != 0)
                        e.Graphics.DrawLine(pen, bounds.Left, bounds.Top, bounds.Right - 1, bounds.Top);
                    if ((sides & ToolStripStatusLabelBorderSides.Right) != 0)
                        e.Graphics.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);
                    if ((sides & ToolStripStatusLabelBorderSides.Bottom) != 0)
                        e.Graphics.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
                }
            }
            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                if (IsDark)
                    e.TextColor = e.Item.Enabled ? DarkText : Color.FromArgb(155, 155, 160);
                base.OnRenderItemText(e);
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                if (IsDark) {
                    e.ArrowColor = e.Item != null && e.Item.Enabled
                        ? Color.FromArgb(235, 240, 245)
                        : Color.FromArgb(135, 135, 140);
                }
                base.OnRenderArrow(e);
            }

            protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
            {
                if (!IsDark) {
                    base.OnRenderItemCheck(e);
                    return;
                }

                ToolStripMenuItem menuItem = e.Item as ToolStripMenuItem;
                if (menuItem == null) {
                    base.OnRenderItemCheck(e);
                    return;
                }

                Rectangle imageArea = e.ImageRectangle;
                int glyphSize = System.Math.Min(DpiHelper.Scale(e.ToolStrip, 14),
                    System.Math.Min(imageArea.Width, imageArea.Height));
                Rectangle glyph = new Rectangle(
                    imageArea.Left + System.Math.Max(0, (imageArea.Width - glyphSize) / 2),
                    imageArea.Top + System.Math.Max(0, (imageArea.Height - glyphSize) / 2),
                    glyphSize, glyphSize);

                bool enabled = menuItem.Enabled;
                bool selected = menuItem.Selected;
                Color glyphBack = enabled
                    ? (selected ? Color.FromArgb(18, 132, 224) : DarkAccent)
                    : Color.FromArgb(72, 72, 77);
                Color glyphBorder = enabled
                    ? (selected ? Color.FromArgb(170, 220, 255) : Color.FromArgb(95, 175, 235))
                    : Color.FromArgb(105, 105, 110);
                Color markColor = enabled ? Color.White : Color.FromArgb(185, 185, 190);

                using (Brush background = new SolidBrush(glyphBack))
                    e.Graphics.FillRectangle(background, glyph);
                using (Pen border = new Pen(glyphBorder))
                    e.Graphics.DrawRectangle(border, glyph.Left, glyph.Top,
                        System.Math.Max(0, glyph.Width - 1), System.Math.Max(0, glyph.Height - 1));

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen mark = new Pen(markColor,
                    System.Math.Max(1.5F, DpiHelper.Scale(1.8F, DpiHelper.GetDpi(e.Graphics))))) {
                    mark.StartCap = LineCap.Square;
                    mark.EndCap = LineCap.Square;
                    if (menuItem.CheckState == CheckState.Indeterminate) {
                        int y = glyph.Top + glyph.Height / 2;
                        e.Graphics.DrawLine(mark, glyph.Left + DpiHelper.Scale(e.ToolStrip, 3), y,
                            glyph.Right - DpiHelper.Scale(e.ToolStrip, 4), y);
                    } else {
                        e.Graphics.DrawLines(mark, new Point[] {
                            new Point(glyph.Left + DpiHelper.Scale(e.ToolStrip, 3),
                                glyph.Top + DpiHelper.Scale(e.ToolStrip, 7)),
                            new Point(glyph.Left + DpiHelper.Scale(e.ToolStrip, 6),
                                glyph.Top + DpiHelper.Scale(e.ToolStrip, 10)),
                            new Point(glyph.Left + DpiHelper.Scale(e.ToolStrip, 11),
                                glyph.Top + DpiHelper.Scale(e.ToolStrip, 4))
                        });
                    }
                }
                e.Graphics.SmoothingMode = SmoothingMode.None;
            }
        }

        private static Image CreateHelpIcon(bool dark)
        {
            Bitmap icon = new Bitmap(16, 16);
            using (Graphics graphics = Graphics.FromImage(icon)) {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Color fill = dark ? Color.FromArgb(20, 135, 225) : Color.FromArgb(0, 105, 185);
                Color border = dark ? Color.FromArgb(155, 210, 250) : Color.FromArgb(0, 70, 135);
                using (Brush fillBrush = new SolidBrush(fill))
                    graphics.FillEllipse(fillBrush, 1, 1, 14, 14);
                using (Pen borderPen = new Pen(border))
                    graphics.DrawEllipse(borderPen, 1, 1, 13, 13);
                using (Font font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Pixel))
                    TextRenderer.DrawText(graphics, "?", font, new Rectangle(1, 1, 14, 14), Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
            return icon;
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
