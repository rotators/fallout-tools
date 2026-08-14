using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScriptEditor
{
    public partial class SearchForm : Form
    {
        private bool isHide = false;
        private bool initialThemeReady;
        private bool prewarming;
        private bool revealing;
        private readonly ToolTip searchToolTip = new ToolTip();

        public SearchForm()
        {
            // Keep native light controls out of the first visible frame. Handles are
            // created and themed in OnLoad before the search window is revealed.
            Opacity = 0D;
            InitializeComponent();

            cbSearchPath.Items.AddRange((Settings.searchListPath.Count > 0)
                                        ? Settings.searchListPath.Cast<String>().ToArray()
                                        : File.ReadAllLines(Settings.SearchFoldersPath));
            SelectSearchPath(Settings.lastSearchPath);
            cbFileMask.SelectedIndex = 0;

            cbCase.Checked = !Settings.searchIgnoreCase;
            cbWord.Checked = Settings.searchWholeWord;
            cbRegular.Checked = Settings.searchRegularExpression;
            cbFindAll.Checked = Settings.searchFindAllMatches;
            cbFindAll.CheckedChanged += cbFindAll_CheckedChanged;
            lbFindFiles.Visible = false;

            searchToolTip.SetToolTip(cbSearch, "Previously used search terms.");
            searchToolTip.SetToolTip(cbSearchPath,
                "Folders previously selected with Browse. Available for folder searches only.");
            searchToolTip.SetToolTip(bChange, "Browse for the folder to search.");
            searchToolTip.SetToolTip(cbFileMask,
                "File type filter. Available for folder searches only.");

            this.KeyUp += delegate(object a1, KeyEventArgs a2)
            {
                if (a2.KeyCode == Keys.Escape) this.bHide.PerformClick();
            };

            rbCurrent.CheckedChanged += SearchScope_CheckedChanged;
            rbAll.CheckedChanged += SearchScope_CheckedChanged;
            rbFolder.CheckedChanged += SearchScope_CheckedChanged;

            switch (Settings.searchScope) {
                case SearchScope.CurrentScript:
                    rbCurrent.Checked = true;
                    break;
                case SearchScope.FilesFolder:
                    rbFolder.Checked = true;
                    break;
                default:
                    rbAll.Checked = true;
                    break;
            }
            UpdateFolderSearchControls();

            this.bChange.Click += delegate(object a1, EventArgs a2)
            {
                if (Directory.Exists(Settings.lastSearchPath))
                    this.fbdSearchFolder.SelectedPath = Settings.lastSearchPath;
                if (this.fbdSearchFolder.ShowDialog() != DialogResult.OK) return;

                Settings.lastSearchPath = this.fbdSearchFolder.SelectedPath;

                SelectSearchPath(Settings.lastSearchPath);
            };

            this.cbSearch.KeyPress += delegate(object a1, KeyPressEventArgs a2)
            {
                if (a2.KeyChar == '\r') {
                    a2.Handled = true;
                    this.bSearch.PerformClick();
                }
            };

            InterfaceTheme.Apply((Control)this);
            PerformLayout();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            EnsureControlHandles(this);
            InterfaceTheme.Apply(this);
            PerformLayout();
            Update();
        }

        protected override void SetVisibleCore(bool value)
        {
            if (value && !initialThemeReady) {
                // Create and theme the complete native control hierarchy before WinForms
                // is allowed to make the window visible. This avoids exposing the initial
                // system-colour layout pass.
                EnsureControlHandles(this);
                InterfaceTheme.Apply(this);
                PerformLayout();
                Refresh();
                Update();
                initialThemeReady = true;
            }

            if (value && !prewarming) {
                revealing = true;
                Opacity = 0D;
            }
            base.SetVisibleCore(value);

            if (value && !prewarming) {
                // Showing an editable ComboBox recreates and repaints its native edit
                // child. Keep the composed window transparent through that pass, then
                // reveal the already-themed pixels as one complete frame.
                EnsureControlHandles(this);
                InterfaceTheme.Apply(this);
                PerformLayout();
                Refresh();
                Update();
                Opacity = 1D;
                revealing = false;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            InterfaceTheme.Apply(this);
            Refresh();
            Update();
            base.OnShown(e);
        }

        protected override bool ShowWithoutActivation
        {
            get { return prewarming; }
        }

        public void Prewarm()
        {
            if (IsDisposed || initialThemeReady || Visible)
                return;

            prewarming = true;
            Opacity = 0D;
            try {
                // Some native WinForms controls only complete their first paint after
                // the top-level window is shown. Perform that paint invisibly so Ctrl+F
                // never exposes their temporary system colours.
                Show();
                EnsureControlHandles(this);
                InterfaceTheme.Apply(this);
                PerformLayout();
                Refresh();
                Update();
                Hide();
            }
            finally {
                prewarming = false;
                Opacity = 0D;
                initialThemeReady = true;
            }
        }

        private static void EnsureControlHandles(Control parent)
        {
            IntPtr handle = parent.Handle;
            foreach (Control child in parent.Controls)
                EnsureControlHandles(child);
        }

        public List<string> GetFolderFiles()
        {
            List<string> files = new List<string>();
            SearchOption so = cbSearchSubfolders.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            if (cbFileMask.SelectedIndex == 0) {
                for (int i = 1; i < cbFileMask.Items.Count; i++)
                   files.AddRange(Directory.GetFiles(Settings.lastSearchPath, cbFileMask.Items[i].ToString(), so));
            } else
                files.AddRange(Directory.GetFiles(Settings.lastSearchPath, cbFileMask.Text, so));

            return files;
        }

        internal string[] GetFolderSearchPatterns()
        {
            if (cbFileMask.SelectedIndex == 0)
                return cbFileMask.Items.Cast<object>().Skip(1).Select(item => item.ToString()).ToArray();
            return new[] { cbFileMask.Text };
        }

        internal bool SearchSubfolders
        {
            get { return cbSearchSubfolders.Checked; }
        }

        internal void SetFolderSearchRunning(bool running)
        {
            cbSearch.Enabled = !running;
            tbReplace.Enabled = !running;
            cbCase.Enabled = !running;
            cbWord.Enabled = !running && !cbRegular.Checked;
            cbRegular.Enabled = !running;
            cbFindAll.Enabled = !running;
            rbCurrent.Enabled = !running;
            rbAll.Enabled = !running;
            rbFolder.Enabled = !running;
            cbSearchPath.Enabled = !running && rbFolder.Checked;
            cbSearchSubfolders.Enabled = !running && rbFolder.Checked;
            cbFileMask.Enabled = !running && rbFolder.Checked;
            bChange.Enabled = !running && rbFolder.Checked;
            bReplace.Enabled = !running && !rbFolder.Checked;
            bSearch.Text = running ? "Cancel" : "Search";
        }

        private void SelectSearchPath(string path)
        {
            const string unsetPath = "<unset>";
            while (cbSearchPath.Items.Contains(unsetPath))
                cbSearchPath.Items.Remove(unsetPath);

            if (String.IsNullOrWhiteSpace(path)) {
                cbSearchPath.Items.Insert(0, unsetPath);
                cbSearchPath.SelectedIndex = 0;
                return;
            }

            int selectedIndex = -1;
            for (int i = 0; i < cbSearchPath.Items.Count; i++) {
                if (String.Equals(cbSearchPath.Items[i].ToString(), path, StringComparison.OrdinalIgnoreCase)) {
                    selectedIndex = i;
                    break;
                }
            }
            if (selectedIndex < 0) {
                cbSearchPath.Items.Add(path);
                selectedIndex = cbSearchPath.Items.Count - 1;
            }
            cbSearchPath.SelectedIndex = selectedIndex;
        }

        private void SearchForm_Deactivate(object sender, EventArgs e)
        {
            if (!isHide && WindowState == FormWindowState.Minimized) Opacity = 0.6;
        }

        private void SearchForm_Activated(object sender, EventArgs e)
        {
            if (prewarming || revealing)
                return;
            Opacity = 1;
            isHide = false;
        }

        private void bHide_Click(object sender, EventArgs e)
        {
            isHide = true;
            this.Hide();
        }

        private void rbFolder_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFolderSearchControls();
        }


        private void UpdateFolderSearchControls()
        {
            bool folderSearch = rbFolder.Checked;
            bChange.Enabled = folderSearch;
            cbSearchSubfolders.Enabled = folderSearch;
            cbFileMask.Enabled = folderSearch;
            cbSearchPath.Enabled = folderSearch;
            bReplace.Enabled = !folderSearch;
        }
        private void cbRegular_CheckedChanged(object sender, EventArgs e)
        {
            cbWord.Enabled = !cbRegular.Checked;
            Settings.searchRegularExpression = cbRegular.Checked;
        }

        private void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.searchListPath.Clear();
            Settings.searchListPath.AddRange(cbSearchPath.Items.Cast<String>()
                .Where(path => !String.Equals(path, "<unset>", StringComparison.OrdinalIgnoreCase)).ToList());
        }

        private void cbFindAll_CheckedChanged(object sender, EventArgs e)
        {
            Settings.searchFindAllMatches = cbFindAll.Checked;
        }

        private void SearchScope_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCurrent.Checked)
                Settings.searchScope = SearchScope.CurrentScript;
            else if (rbFolder.Checked)
                Settings.searchScope = SearchScope.FilesFolder;
            else if (rbAll.Checked)
                Settings.searchScope = SearchScope.AllOpenScripts;

            lbFindFiles.Visible = rbFolder.Checked && lbFindFiles.Items.Count > 0;
        }

        private void cbCase_Click(object sender, EventArgs e)
        {
            Settings.searchIgnoreCase = !cbCase.Checked;
        }

        private void cbWord_Click(object sender, EventArgs e)
        {
            Settings.searchWholeWord = cbWord.Checked;
        }

        private void cbSearchPath_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.lastSearchPath = String.Equals(cbSearchPath.Text, "<unset>", StringComparison.OrdinalIgnoreCase)
                ? null : cbSearchPath.Text;
        }
    }
    internal sealed class DarkDisabledCheckBox : CheckBox
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!InterfaceTheme.IsDark) {
                base.OnPaint(e);
                return;
            }

            Color backColor = Color.FromArgb(53, 53, 56);
            Color textColor = Enabled ? Color.Gainsboro : Color.FromArgb(170, 170, 175);
            Color borderColor = Enabled ? Color.FromArgb(110, 110, 115) : Color.FromArgb(92, 92, 96);
            e.Graphics.Clear(backColor);

            int boxSize = DpiHelper.Scale(this, 13);
            int boxY = (ClientSize.Height - boxSize) / 2;
            Rectangle box = new Rectangle(0, boxY, boxSize, boxSize);
            using (Brush boxBrush = new SolidBrush(Color.FromArgb(40, 40, 42)))
            using (Pen borderPen = new Pen(borderColor)) {
                e.Graphics.FillRectangle(boxBrush, box);
                e.Graphics.DrawRectangle(borderPen, box);
            }
            if (Checked || CheckState == CheckState.Indeterminate) {
                using (Pen checkPen = new Pen(textColor, DpiHelper.Scale(2.0f, DeviceDpi))) {
                    e.Graphics.DrawLines(checkPen, new Point[] {
                        new Point(DpiHelper.Scale(this, 3), boxY + DpiHelper.Scale(this, 7)),
                        new Point(DpiHelper.Scale(this, 6), boxY + DpiHelper.Scale(this, 10)),
                        new Point(DpiHelper.Scale(this, 11), boxY + DpiHelper.Scale(this, 3))
                    });
                }
            }

            int textGap = DpiHelper.Scale(this, 4);
            Rectangle textBounds = new Rectangle(box.Right + textGap, 0,
                System.Math.Max(0, ClientSize.Width - box.Right - textGap), ClientSize.Height);
            TextRenderer.DrawText(e.Graphics, Text, Font, textBounds, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }
    }

    internal sealed class DarkDisabledButton : Button
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!InterfaceTheme.IsDark) {
                base.OnPaint(e);
                return;
            }

            bool mouseOver = Enabled && ClientRectangle.Contains(PointToClient(Cursor.Position));
            bool pressed = mouseOver && Capture && MouseButtons == MouseButtons.Left;
            Color backColor = pressed ? Color.FromArgb(40, 40, 42)
                : mouseOver ? Color.FromArgb(85, 85, 90)
                : Color.FromArgb(53, 53, 56);
            Color textColor = Enabled ? Color.Gainsboro : Color.FromArgb(170, 170, 175);
            Color borderColor = Color.FromArgb(68, 68, 72);
            e.Graphics.Clear(backColor);
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, borderColor, ButtonBorderStyle.Solid);

            Size textSize = TextRenderer.MeasureText(Text, Font, Size.Empty, TextFormatFlags.NoPadding);
            int imageWidth = Image == null ? 0 : Image.Width;
            int spacing = Image == null || Text.Length == 0 ? 0 : DpiHelper.Scale(this, 4);
            int contentWidth = imageWidth + spacing + textSize.Width;
            int edge = DpiHelper.Scale(this, 2);
            int x = System.Math.Max(edge, (ClientSize.Width - contentWidth) / 2);
            if (Image != null) {
                int imageY = (ClientSize.Height - Image.Height) / 2;
                if (Enabled)
                    e.Graphics.DrawImage(Image, x, imageY, Image.Width, Image.Height);
                else
                    ControlPaint.DrawImageDisabled(e.Graphics, Image, x, imageY, backColor);
                x += imageWidth + spacing;
            }
            Rectangle textBounds = new Rectangle(x, 0,
                System.Math.Max(0, ClientSize.Width - x - edge), ClientSize.Height);
            TextRenderer.DrawText(e.Graphics, Text, Font, textBounds, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);

            if (Enabled && Focused && ShowFocusCues) {
                Rectangle focusBounds = ClientRectangle;
                int focusInset = DpiHelper.Scale(this, 3);
                focusBounds.Inflate(-focusInset, -focusInset);
                ControlPaint.DrawFocusRectangle(e.Graphics, focusBounds, textColor, backColor);
            }
        }
    }

}
