using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScriptEditor
{
    public partial class SettingsDialog : Form
    {
        private string outPath;
        private string scriptsHPath;
        private string headersFilesPath;
        private bool cancelRequested;

        public SettingsDialog()
        {
            // Keep the native light controls out of the first visible frame. Their
            // handles are created and themed in OnLoad before the dialog is revealed.
            Opacity = 0D;
            outPath = Settings.outputDir;
            scriptsHPath = Settings.pathScriptsHFile;
            headersFilesPath = Settings.pathHeadersFiles;
            InitializeComponent();

            PerformLayout();

            if (Settings.useMcpp) 
                cmbPreprocessor.SelectedIndex = 1;
            else if (Settings.useWatcom)
                cmbPreprocessor.SelectedIndex = 2;
            else
                cmbPreprocessor.SelectedIndex = 0;

            cbUseBackward.Checked = (Settings.compileBackwardMode > 0);
            cbIncludePath.Checked = Settings.searchIncludePath;
            cbOptimize.SelectedIndex = (Settings.optimize == 255 ? 1 : Settings.optimize);
            cbWarnings.Checked = Settings.showWarnings;
            cbDebug.Checked = Settings.showDebug;
            cbWarnFailedCompile.Checked = Settings.warnOnFailedCompile;
            cbMultiThread.Checked = Settings.multiThreaded;
            cbAutoOpenMessages.Checked = Settings.autoOpenMsgs;
            tbLanguage.Text = Settings.language;
            cbTabsToSpaces.Checked = Settings.tabsToSpaces;
            tbTabSize.Value = Settings.tabSize;
            cbEnableParser.Checked = Settings.enableParser;
            cbShortCircuit.Checked = Settings.shortCircuit;
            cbAutocomplete.Checked = Settings.autocomplete;
            cbNonColor.Checked = Settings.autocompleteColor;
            cbAutoPaired.Checked = Settings.autoInputPaired;
            Highlight_comboBox.SelectedIndex = Settings.highlight;
            InterfaceTheme_comboBox.SelectedIndex = (int)Settings.interfaceTheme;
            HintLang_comboBox.SelectedIndex = Settings.hintsLang;
            if (!Settings.enableParser) cbParserWarn.Enabled = false;
            cbParserWarn.Checked = Settings.parserWarn;
            cbCompilePath.Checked = Settings.ignoreCompPath;
            cbUserCompile.Checked = Settings.userCmdCompile;
            cbAssociateID.Checked = Settings.associateID;
            cbShowTips.Checked = Settings.showTips;
            cbShortDesc.Checked = Settings.shortDesc;
            cbStorePosition.Checked = Settings.storeLastPosition;
            cbReopenLastTabs.Checked = Settings.reopenLastTabs;
            foreach (var item in Settings.msgListPath)
                msgPathlistView.Items.Add(item.ToString());
            SetLabelText();

            int dsize = 80;
            for (int i=0; i < Settings.Fonts.Families.Length; i++)
            {
                string fontName = Settings.Fonts.Families[i].Name;
                dsize = Math.Max((int)(fontName.Length * 6.5f), dsize);
                cbFonts.Items.Add(fontName);
            }
            if (cbFonts.Items.Count > 1) {
                cbFonts.DropDownWidth = dsize;
                cbFonts.SelectedIndex = Settings.selectFont;

            } else
                cbFonts.SelectedIndex = 0;
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

        protected override void OnShown(EventArgs e)
        {
            // OnLoad has now created and themed every native child window.
            InterfaceTheme.Apply(this);
            Refresh();
            Opacity = 1D;
            base.OnShown(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) {
                cancelRequested = true;
                DialogResult = DialogResult.Cancel;
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static void EnsureControlHandles(Control parent)
        {
            IntPtr handle = parent.Handle;
            foreach (Control child in parent.Controls)
                EnsureControlHandles(child);
        }

        private static void ConfigureSettingsComboBox(ComboBox comboBox)
        {
            comboBox.AutoSize = false;
            comboBox.IntegralHeight = false;
            comboBox.ItemHeight = DpiHelper.Scale(comboBox, 17);
        }
        private System.Windows.Forms.GroupBox groupEditorBehavior;
        private System.Windows.Forms.GroupBox groupMessageOptions;
        private System.Windows.Forms.GroupBox groupFileAssociation;

        private void SetLabelText()
        {
            textBox2.Text = outPath == null ? "<unset>" : outPath;
            tbScriptsHPath.Text = scriptsHPath == null ? "<unset>" : scriptsHPath;
            textBox1.Text = headersFilesPath == null ? "<unset>" : headersFilesPath;
        }

        private void SettingsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cancelRequested || DialogResult != DialogResult.OK)
                return;

            byte previousHintsLanguage = Settings.hintsLang;
            bool previousShowTips = Settings.showTips;
            bool previousShortDescription = Settings.shortDesc;

            Settings.useMcpp = false;
            Settings.useWatcom = false;
            switch (cmbPreprocessor.SelectedIndex) {
            case 1 :
                Settings.useMcpp = true;
                break;
            case 2 :
                Settings.useWatcom = true;
                break;
            }
            Settings.searchIncludePath = cbIncludePath.Checked;
            Settings.optimize = (byte)cbOptimize.SelectedIndex;
            Settings.showDebug = cbDebug.Checked;
            Settings.showWarnings = cbWarnings.Checked;
            Settings.warnOnFailedCompile = cbWarnFailedCompile.Checked;
            Settings.multiThreaded = cbMultiThread.Checked;
            Settings.outputDir = outPath;
            Settings.autoOpenMsgs = cbAutoOpenMessages.Checked;
            Settings.pathScriptsHFile = scriptsHPath;
            Settings.pathHeadersFiles = headersFilesPath;
            Settings.language = tbLanguage.Text.Length == 0 ? "english" : tbLanguage.Text;
            Settings.tabsToSpaces = cbTabsToSpaces.Checked;
            Settings.tabSize = (int)tbTabSize.Value;
            if (Settings.tabSize < 1 || Settings.tabSize > 30) Settings.tabSize = 3;

            Settings.enableParser = cbEnableParser.Checked;
            Settings.shortCircuit = cbShortCircuit.Checked;
            Settings.autocomplete = cbAutocomplete.Checked;
            Settings.autocompleteColor = cbNonColor.Checked;
            Settings.autoInputPaired = cbAutoPaired.Checked;
            Settings.highlight = (byte)Highlight_comboBox.SelectedIndex;
            Settings.interfaceTheme = (InterfaceThemeMode)InterfaceTheme_comboBox.SelectedIndex;
            Settings.hintsLang = (byte)HintLang_comboBox.SelectedIndex;
            Settings.parserWarn = cbParserWarn.Checked;
            Settings.ignoreCompPath = cbCompilePath.Checked;
            Settings.userCmdCompile = cbUserCompile.Checked;
            Settings.associateID = cbAssociateID.Checked;
            Settings.showTips = cbShowTips.Checked;
            Settings.shortDesc = cbShortDesc.Checked;
            Settings.msgListPath.Clear();
            Settings.selectFont= (byte)cbFonts.SelectedIndex;
            Settings.storeLastPosition = cbStorePosition.Checked;
            Settings.reopenLastTabs = cbReopenLastTabs.Checked;
            Settings.compileBackwardMode = cbUseBackward.Checked ? 1 : 0;

            foreach (ListViewItem item in msgPathlistView.Items)
                Settings.msgListPath.Add(item.Text);
            Settings.Save();

            if (Settings.hintsLang != previousHintsLanguage) {
                foreach (Form form in Application.OpenForms) {
                    TextEditor editor = form as TextEditor;
                    if (editor != null)
                        editor.RefreshDescriptionLanguage();
                }
            }
            if (Settings.showTips != previousShowTips || Settings.shortDesc != previousShortDescription) {
                foreach (Form form in Application.OpenForms) {
                    TextEditor editor = form as TextEditor;
                    if (editor != null)
                        editor.RefreshDescriptionOptions();
                }
            }
        }

        private void bChange_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Select compiled scripts folder";
            folderBrowserDialog1.SelectedPath = outPath;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                outPath = folderBrowserDialog1.SelectedPath;
                SetLabelText();
            }
        }

        private void bScriptsH_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = scriptsHPath ?? headersFilesPath;
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                scriptsHPath = openFileDialog1.FileName;
                SetLabelText();
            }
        }

        private void cbEnableParser_CheckedChanged(object sender, EventArgs e)
        {
            cbParserWarn.Enabled = cbEnableParser.Checked;
        }

        private void addPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                string msgPath = folderBrowserDialog1.SelectedPath;
                if (msgPathlistView.Items.Count > 0) {
                    msgPathlistView.Items.Insert(0, msgPath);
                } else msgPathlistView.Items.Add(msgPath);
                //msgPathlistView.Items[msgPathlistView.Items.Count - 1].ToolTipText = msgPath;
            }
        }

        private void deletePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (msgPathlistView.Items == null)
                return;
            msgPathlistView.Items.RemoveAt(msgPathlistView.FocusedItem.Index);
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (msgPathlistView.Items == null)
                return;
            int sInd = msgPathlistView.FocusedItem.Index;
            if (sInd == 0)
                return;
            string iPath = msgPathlistView.Items[--sInd].Text;
            PathItemSub(sInd, iPath);
        }

        private void modeDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (msgPathlistView.Items == null)
                return;
            int sInd = msgPathlistView.FocusedItem.Index;
            if (sInd == msgPathlistView.Items.Count - 1)
                return;
            string iPath = msgPathlistView.Items[++sInd].Text;
            PathItemSub(sInd, iPath);
        }

        private void PathItemSub(int sInd, string iPath)
        {
            msgPathlistView.Items[sInd].Text = msgPathlistView.FocusedItem.Text;
            msgPathlistView.FocusedItem.Text = iPath;
            msgPathlistView.Items[sInd].Selected = true;
            msgPathlistView.Items[sInd].Focused = true;
        }

        private void bAssociate_Click(object sender, EventArgs e)
        {
            FileAssociation.Associate(true);
        }

        private void cbCompilePath_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = !cbCompilePath.Checked;
        }

        private void cbUserCompile_CheckedChanged(object sender, EventArgs e)
        {
            //cbCompilePath.Enabled = !cbUserCompile.Checked;
            //textBox2.Enabled = !cbUserCompile.Checked & !cbCompilePath.Checked;
            cmbPreprocessor.Enabled = !cbUserCompile.Checked;
        }

        private void bHeaders_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Select headers files folder";
            folderBrowserDialog1.SelectedPath = headersFilesPath;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                headersFilesPath = folderBrowserDialog1.SelectedPath;
                if (scriptsHPath == null)
                    scriptsHPath = headersFilesPath + @"\SCRIPTS.H";
                SetLabelText();
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void SettingsDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
