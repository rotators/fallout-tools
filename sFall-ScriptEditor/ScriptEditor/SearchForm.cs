using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

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
            bool designTime = LicenseManager.UsageMode == LicenseUsageMode.Designtime;
            InitializeComponent();

            // The WinForms designer must be able to construct the control tree without
            // reading the user's configuration or creating runtime theme hooks.
            if (designTime)
                return;

            Opacity = 0D;
            bChange.FlatStyle = FlatStyle.Flat;
            bReplace.FlatStyle = FlatStyle.Flat;
            bChange.Paint += DarkDisabledButton.PaintDisabledButton;
            bReplace.Paint += DarkDisabledButton.PaintDisabledButton;

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
            SetSearchPathEnabled(!running && rbFolder.Checked);
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
            SetSearchPathEnabled(folderSearch);
            bReplace.Enabled = !folderSearch;
        }

        private void SetSearchPathEnabled(bool enabled)
        {
            bool redrawWasSuppressed = false;
            if (cbSearchPath.IsHandleCreated) {
                SendMessage(cbSearchPath.Handle, WmSetRedraw, IntPtr.Zero, IntPtr.Zero);
                redrawWasSuppressed = true;
            }

            cbSearchPath.Enabled = enabled;

            if (redrawWasSuppressed) {
                InterfaceTheme.Apply(cbSearchPath);
                SendMessage(cbSearchPath.Handle, WmSetRedraw, new IntPtr(1), IntPtr.Zero);
                cbSearchPath.Invalidate(true);
                cbSearchPath.Update();
            }
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

        private const int WmSetRedraw = 0x000B;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam);
    }
}
