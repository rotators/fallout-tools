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
        private GroupBox groupFileAssociation;
        private GroupBox groupEditorBehavior;
        private GroupBox groupMessageOptions;
        private CheckBox cbReopenLastTabs;

        public SettingsDialog()
        {
            // Keep the native light controls out of the first visible frame. Their
            // handles are created and themed in OnLoad before the dialog is revealed.
            Opacity = 0D;
            outPath = Settings.outputDir;
            scriptsHPath = Settings.pathScriptsHFile;
            headersFilesPath = Settings.pathHeadersFiles;
            InitializeComponent();
            ConfigureSettingsLayout();
            ConfigureFileAssociationSection();

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
                // The dialog currently commits its values whenever it closes, including
                // through the title-bar close button. Escape follows that same behavior.
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

        private void ConfigureSettingsLayout()
        {
            SuspendLayout();

            ClientSize = DpiHelper.Scale(this, new Size(640, 656));
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ConfigureSettingsComboBox(Highlight_comboBox);
            ConfigureSettingsComboBox(InterfaceTheme_comboBox);
            ConfigureSettingsComboBox(cbFonts);
            ConfigureSettingsComboBox(cmbPreprocessor);
            ConfigureSettingsComboBox(cbOptimize);
            ConfigureSettingsComboBox(HintLang_comboBox);

            SetLogicalBounds(groupBox6, 8, 6, 624, 56);
            groupBox6.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox6.Text = "Appearance";
            SetLogicalBounds(labelScriptStyle, 8, 23, 68, 17);
            SetLogicalBounds(Highlight_comboBox, 78, 18, 108, 24);
            SetLogicalBounds(labelInterfaceStyle, 202, 23, 86, 17);
            SetLogicalBounds(InterfaceTheme_comboBox, 290, 18, 108, 24);
            SetLogicalBounds(labelCodeFont, 414, 23, 62, 17);
            SetLogicalBounds(cbFonts, 478, 18, 136, 24);

            SetLogicalBounds(groupBox1, 8, 68, 400, 125);
            groupBox1.Text = "Compilation";
            SetLogicalBounds(cbUserCompile, 8, 22, 164, 20);
            SetLogicalBounds(cmbPreprocessor, 180, 19, 206, 23);
            SetLogicalBounds(cbUseBackward, 8, 49, 164, 20);
            SetLogicalBounds(cbOptimize, 180, 46, 110, 23);
            SetLogicalBounds(label6, 298, 50, 88, 17);
            SetLogicalBounds(cbWarnings, 8, 76, 100, 20);
            SetLogicalBounds(cbDebug, 116, 76, 78, 20);
            SetLogicalBounds(cbShortCircuit, 202, 76, 184, 20);
            SetLogicalBounds(cbWarnFailedCompile, 8, 101, 164, 20);
            SetLogicalBounds(cbMultiThread, 180, 101, 206, 20);

            SetLogicalBounds(groupBox4, 416, 68, 216, 50);
            SetLogicalBounds(cbEnableParser, 8, 20, 92, 20);
            SetLogicalBounds(cbParserWarn, 112, 20, 94, 20);

            SetLogicalBounds(groupBox5, 416, 124, 216, 69);
            SetLogicalBounds(cbAutocomplete, 8, 20, 166, 20);
            SetLogicalBounds(cbNonColor, 182, 17, 24, 24);
            SetLogicalBounds(cbAutoPaired, 8, 44, 198, 20);

            groupEditorBehavior = new GroupBox();
            groupEditorBehavior.Name = "groupEditorBehavior";
            groupEditorBehavior.Text = "Editing";
            groupEditorBehavior.TabStop = false;
            SetLogicalBounds(groupEditorBehavior, 8, 199, 624, 83);
            groupEditorBehavior.Controls.Add(cbShowTips);
            groupEditorBehavior.Controls.Add(cbShortDesc);
            groupEditorBehavior.Controls.Add(HintLang_comboBox);
            groupEditorBehavior.Controls.Add(label2);
            groupEditorBehavior.Controls.Add(cbStorePosition);
            groupEditorBehavior.Controls.Add(cbTabsToSpaces);
            groupEditorBehavior.Controls.Add(label7);
            groupEditorBehavior.Controls.Add(tbTabSize);
            cbReopenLastTabs = new CheckBox();
            cbReopenLastTabs.AutoSize = true;
            cbReopenLastTabs.Name = "cbReopenLastTabs";
            cbReopenLastTabs.Text = "Reopen tabs from previous session";
            cbReopenLastTabs.UseVisualStyleBackColor = true;
            groupEditorBehavior.Controls.Add(cbReopenLastTabs);
            SetLogicalBounds(cbShowTips, 8, 22, 94, 20);
            SetLogicalBounds(cbShortDesc, 112, 22, 138, 20);
            SetLogicalBounds(cbStorePosition, 260, 22, 132, 20);
            SetLogicalBounds(cbReopenLastTabs, 400, 22, 210, 20);
            SetLogicalBounds(label2, 8, 55, 126, 17);
            label2.Text = "Description language:";
            SetLogicalBounds(HintLang_comboBox, 136, 50, 112, 23);
            SetLogicalBounds(cbTabsToSpaces, 270, 52, 174, 20);
            SetLogicalBounds(label7, 452, 55, 104, 17);
            label7.Text = "Tab/indent size:";
            SetLogicalBounds(tbTabSize, 558, 50, 52, 23);
            toolTip.SetToolTip(cbReopenLastTabs,
                "Automatically reopen the script tabs that were open when the editor last closed.");
            Controls.Add(groupEditorBehavior);

            SetLogicalBounds(groupBox2, 8, 288, 624, 146);
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Text = "Paths";
            SetLogicalBounds(groupBox2.Controls["label1"], 8, 18, 190, 17);
            SetLogicalBounds(groupBox2.Controls["label4"], 8, 59, 190, 17);
            SetLogicalBounds(cbCompilePath, 387, 14, 155, 20);
            SetLogicalBounds(textBox2, 8, 34, 568, 20);
            SetLogicalBounds(bChange, 580, 34, 34, 23);
            SetLogicalBounds(cbIncludePath, 387, 55, 190, 20);
            SetLogicalBounds(textBox1, 8, 75, 568, 20);
            SetLogicalBounds(bHeaders, 580, 75, 34, 23);
            SetLogicalBounds(label8, 8, 101, 190, 17);
            SetLogicalBounds(tbScriptsHPath, 8, 117, 568, 20);
            SetLogicalBounds(bScriptsH, 580, 117, 34, 23);

            groupMessageOptions = new GroupBox();
            groupMessageOptions.Name = "groupMessageOptions";
            groupMessageOptions.Text = "Message files";
            groupMessageOptions.TabStop = false;
            SetLogicalBounds(groupMessageOptions, 8, 440, 624, 51);
            groupMessageOptions.Controls.Add(cbAutoOpenMessages);
            groupMessageOptions.Controls.Add(cbAssociateID);
            groupMessageOptions.Controls.Add(label5);
            groupMessageOptions.Controls.Add(tbLanguage);
            SetLogicalBounds(cbAutoOpenMessages, 8, 21, 148, 20);
            SetLogicalBounds(cbAssociateID, 170, 21, 136, 20);
            SetLogicalBounds(label5, 330, 24, 134, 17);
            label5.Text = "Message language:";
            SetLogicalBounds(tbLanguage, 468, 20, 138, 23);
            Controls.Add(groupMessageOptions);

            SetLogicalBounds(button1, 532, 621, 100, 27);

            ResumeLayout(false);
            PerformLayout();
        }

        private static void ConfigureSettingsComboBox(ComboBox comboBox)
        {
            comboBox.AutoSize = false;
            comboBox.IntegralHeight = false;
            comboBox.ItemHeight = DpiHelper.Scale(comboBox, 17);
        }
        private void SetLogicalBounds(Control control, int x, int y, int width, int height)
        {
            control.SetBounds(DpiHelper.Scale(this, x), DpiHelper.Scale(this, y),
                DpiHelper.Scale(this, width), DpiHelper.Scale(this, height));
        }

        private void ConfigureFileAssociationSection()
        {
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            SetLogicalBounds(groupBox3, 8, 497, 466, 118);
            SetLogicalBounds(msgPathlistView, 8, 20, 450, 90);
            columnHeader1.Width = DpiHelper.Scale(this, 442);

            groupFileAssociation = new GroupBox();
            groupFileAssociation.Location = DpiHelper.Scale(this, new Point(482, 497));
            groupFileAssociation.Name = "groupFileAssociation";
            groupFileAssociation.Size = DpiHelper.Scale(this, new Size(150, 118));
            groupFileAssociation.TabIndex = 44;
            groupFileAssociation.TabStop = false;
            groupFileAssociation.Text = "File association";

            label3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label3.AutoSize = false;
            label3.Font = Font;
            label3.Location = DpiHelper.Scale(this, new Point(8, 20));
            label3.Size = DpiHelper.Scale(this, new Size(134, 48));
            label3.Text = "Choose which supported files Windows opens with this editor.";
            label3.TextAlign = ContentAlignment.TopLeft;

            bAssociate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            bAssociate.Font = Font;
            bAssociate.Location = DpiHelper.Scale(this, new Point(8, 78));
            bAssociate.Size = DpiHelper.Scale(this, new Size(134, 28));
            bAssociate.Text = "Set file associations...";

            groupFileAssociation.Controls.Add(label3);
            groupFileAssociation.Controls.Add(bAssociate);
            Controls.Add(groupFileAssociation);

            toolTip.SetToolTip(groupFileAssociation,
                "Configure Windows to open supported script files with Sfall Script Editor.");
            toolTip.SetToolTip(bAssociate,
                "Choose the file types that Windows opens with Sfall Script Editor.");
        }

        private void SetLabelText()
        {
            textBox2.Text = outPath == null ? "<unset>" : outPath;
            tbScriptsHPath.Text = scriptsHPath == null ? "<unset>" : scriptsHPath;
            textBox1.Text = headersFilesPath == null ? "<unset>" : headersFilesPath;
        }

        private void SettingsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
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
    }
}
