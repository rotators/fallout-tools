using System.Drawing;
using System.Windows.Forms;

namespace ScriptEditor {
    partial class SettingsDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label4;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
            this.cbWarnings = new System.Windows.Forms.CheckBox();
            this.cbUseBackward = new System.Windows.Forms.CheckBox();
            this.cbIncludePath = new System.Windows.Forms.CheckBox();
            this.bChange = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.cbWarnFailedCompile = new System.Windows.Forms.CheckBox();
            this.cbMultiThread = new System.Windows.Forms.CheckBox();
            this.cbAutoOpenMessages = new System.Windows.Forms.CheckBox();
            this.bScriptsH = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tbLanguage = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbOptimize = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbTabsToSpaces = new System.Windows.Forms.CheckBox();
            this.cbEnableParser = new System.Windows.Forms.CheckBox();
            this.cbShortCircuit = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.cbAutocomplete = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.cbParserWarn = new System.Windows.Forms.CheckBox();
            this.cbCompilePath = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.msgPathlistView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MsgcontextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bAssociate = new System.Windows.Forms.Button();
            this.cbUserCompile = new System.Windows.Forms.CheckBox();
            this.bHeaders = new System.Windows.Forms.Button();
            this.HintLang_comboBox = new System.Windows.Forms.ComboBox();
            this.Highlight_comboBox = new System.Windows.Forms.ComboBox();
            this.InterfaceTheme_comboBox = new System.Windows.Forms.ComboBox();
            this.cbAssociateID = new System.Windows.Forms.CheckBox();
            this.cbNonColor = new System.Windows.Forms.CheckBox();
            this.cbAutoPaired = new System.Windows.Forms.CheckBox();
            this.tbTabSize = new System.Windows.Forms.NumericUpDown();
            this.cbDebug = new System.Windows.Forms.CheckBox();
            this.cbShowTips = new System.Windows.Forms.CheckBox();
            this.cbShortDesc = new System.Windows.Forms.CheckBox();
            this.cbFonts = new System.Windows.Forms.ComboBox();
            this.cbReopenLastTabs = new System.Windows.Forms.CheckBox();
            this.cbRestoreUnsavedChanges = new System.Windows.Forms.CheckBox();
            this.cbStorePosition = new System.Windows.Forms.CheckBox();
            this.cmbPreprocessor = new System.Windows.Forms.ComboBox();
            this.labelScriptStyle = new System.Windows.Forms.Label();
            this.labelInterfaceStyle = new System.Windows.Forms.Label();
            this.labelCodeFont = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbScriptsHPath = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupEditorBehavior = new System.Windows.Forms.GroupBox();
            this.groupMessageOptions = new System.Windows.Forms.GroupBox();
            this.groupFileAssociation = new System.Windows.Forms.GroupBox();
            label1 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.MsgcontextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbTabSize)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupEditorBehavior.SuspendLayout();
            this.groupMessageOptions.SuspendLayout();
            this.groupFileAssociation.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 16);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(174, 15);
            label1.TabIndex = 5;
            label1.Text = "Compiled/output scripts folder:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(6, 62);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(133, 15);
            label4.TabIndex = 11;
            label4.Text = "Location of header files:";
            // 
            // cbWarnings
            // 
            this.cbWarnings.AutoSize = true;
            this.cbWarnings.Location = new System.Drawing.Point(6, 67);
            this.cbWarnings.Name = "cbWarnings";
            this.cbWarnings.Size = new System.Drawing.Size(76, 19);
            this.cbWarnings.TabIndex = 4;
            this.cbWarnings.Text = "Warnings";
            this.toolTip.SetToolTip(this.cbWarnings, "Show compiler and preprocessor warning messages.");
            this.cbWarnings.UseVisualStyleBackColor = true;
            // 
            // cbUseBackward
            // 
            this.cbUseBackward.AutoSize = true;
            this.cbUseBackward.Location = new System.Drawing.Point(6, 44);
            this.cbUseBackward.Name = "cbUseBackward";
            this.cbUseBackward.Size = new System.Drawing.Size(133, 19);
            this.cbUseBackward.TabIndex = 2;
            this.cbUseBackward.Text = "Use backward mode";
            this.toolTip.SetToolTip(this.cbUseBackward, "Use compatibility mode for scripts written for the BIS compiler.\r\nAllows legacy s" +
        "cripts without sfall functions to compile without editing.");
            this.cbUseBackward.UseVisualStyleBackColor = true;
            // 
            // cbIncludePath
            // 
            this.cbIncludePath.AutoSize = true;
            this.cbIncludePath.Location = new System.Drawing.Point(363, 58);
            this.cbIncludePath.Name = "cbIncludePath";
            this.cbIncludePath.Size = new System.Drawing.Size(186, 19);
            this.cbIncludePath.TabIndex = 3;
            this.cbIncludePath.Text = "Search includes from this path";
            this.toolTip.SetToolTip(this.cbIncludePath, "Also search this folder for required header files.");
            this.cbIncludePath.UseVisualStyleBackColor = true;
            // 
            // bChange
            // 
            this.bChange.Image = ((System.Drawing.Image)(resources.GetObject("bChange.Image")));
            this.bChange.Location = new System.Drawing.Point(555, 30);
            this.bChange.Name = "bChange";
            this.bChange.Size = new System.Drawing.Size(30, 25);
            this.bChange.TabIndex = 2;
            this.toolTip.SetToolTip(this.bChange, "Choose a folder.");
            this.bChange.UseVisualStyleBackColor = true;
            this.bChange.Click += new System.EventHandler(this.bChange_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // cbWarnFailedCompile
            // 
            this.cbWarnFailedCompile.AutoSize = true;
            this.cbWarnFailedCompile.Location = new System.Drawing.Point(6, 90);
            this.cbWarnFailedCompile.Name = "cbWarnFailedCompile";
            this.cbWarnFailedCompile.Size = new System.Drawing.Size(156, 19);
            this.cbWarnFailedCompile.TabIndex = 7;
            this.cbWarnFailedCompile.Text = "Show error log on failure";
            this.toolTip.SetToolTip(this.cbWarnFailedCompile, "Show the error log if script compilation fails.");
            this.cbWarnFailedCompile.UseVisualStyleBackColor = true;
            // 
            // cbMultiThread
            // 
            this.cbMultiThread.AutoSize = true;
            this.cbMultiThread.Location = new System.Drawing.Point(204, 90);
            this.cbMultiThread.Name = "cbMultiThread";
            this.cbMultiThread.Size = new System.Drawing.Size(177, 19);
            this.cbMultiThread.TabIndex = 8;
            this.cbMultiThread.Text = "Multithreaded mass compile";
            this.cbMultiThread.UseVisualStyleBackColor = true;
            // 
            // cbAutoOpenMessages
            // 
            this.cbAutoOpenMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbAutoOpenMessages.AutoSize = true;
            this.cbAutoOpenMessages.Location = new System.Drawing.Point(9, 23);
            this.cbAutoOpenMessages.Name = "cbAutoOpenMessages";
            this.cbAutoOpenMessages.Size = new System.Drawing.Size(123, 19);
            this.cbAutoOpenMessages.TabIndex = 0;
            this.cbAutoOpenMessages.Text = "Open message file";
            this.toolTip.SetToolTip(this.cbAutoOpenMessages, "Automatically open an associated message file if it exists when the script is ope" +
        "ned.");
            this.cbAutoOpenMessages.UseVisualStyleBackColor = true;
            // 
            // bScriptsH
            // 
            this.bScriptsH.Image = ((System.Drawing.Image)(resources.GetObject("bScriptsH.Image")));
            this.bScriptsH.Location = new System.Drawing.Point(555, 122);
            this.bScriptsH.Name = "bScriptsH";
            this.bScriptsH.Size = new System.Drawing.Size(30, 25);
            this.bScriptsH.TabIndex = 7;
            this.toolTip.SetToolTip(this.bScriptsH, "Choose the Scripts.H file.");
            this.bScriptsH.UseVisualStyleBackColor = true;
            this.bScriptsH.Click += new System.EventHandler(this.bScriptsH_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "h";
            this.openFileDialog1.FileName = "scripts.h";
            this.openFileDialog1.Filter = "Header files|*.h";
            this.openFileDialog1.RestoreDirectory = true;
            this.openFileDialog1.Title = "Select Scripts.h file";
            // 
            // tbLanguage
            // 
            this.tbLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbLanguage.Location = new System.Drawing.Point(447, 19);
            this.tbLanguage.MaxLength = 8;
            this.tbLanguage.Name = "tbLanguage";
            this.tbLanguage.Size = new System.Drawing.Size(138, 23);
            this.tbLanguage.TabIndex = 2;
            this.toolTip.SetToolTip(this.tbLanguage, "Message-file language folder. Default: english.");
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(328, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "Message language:";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // cbOptimize
            // 
            this.cbOptimize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOptimize.FormattingEnabled = true;
            this.cbOptimize.Items.AddRange(new object[] {
            "None",
            "Basic",
            "Full",
            "Experimental"});
            this.cbOptimize.Location = new System.Drawing.Point(204, 41);
            this.cbOptimize.Name = "cbOptimize";
            this.cbOptimize.Size = new System.Drawing.Size(95, 23);
            this.cbOptimize.TabIndex = 3;
            this.toolTip.SetToolTip(this.cbOptimize, resources.GetString("cbOptimize.ToolTip"));
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(305, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 15);
            this.label6.TabIndex = 16;
            this.label6.Text = "Optimization";
            this.toolTip.SetToolTip(this.label6, "Compile with the selected optimization level.");
            // 
            // cbTabsToSpaces
            // 
            this.cbTabsToSpaces.AutoSize = true;
            this.cbTabsToSpaces.Location = new System.Drawing.Point(269, 77);
            this.cbTabsToSpaces.Name = "cbTabsToSpaces";
            this.cbTabsToSpaces.Size = new System.Drawing.Size(145, 19);
            this.cbTabsToSpaces.TabIndex = 7;
            this.cbTabsToSpaces.Text = "Convert tabs to spaces";
            this.toolTip.SetToolTip(this.cbTabsToSpaces, "Convert tabs to spaces when Tab is pressed.\r\nThe tab size is set to the right.");
            this.cbTabsToSpaces.UseVisualStyleBackColor = true;
            // 
            // cbEnableParser
            // 
            this.cbEnableParser.AutoSize = true;
            this.cbEnableParser.Location = new System.Drawing.Point(6, 17);
            this.cbEnableParser.Name = "cbEnableParser";
            this.cbEnableParser.Size = new System.Drawing.Size(68, 19);
            this.cbEnableParser.TabIndex = 0;
            this.cbEnableParser.Text = "Enabled";
            this.toolTip.SetToolTip(this.cbEnableParser, "Enable parsing for open scripts.\r\nEnables Find Declaration, Find References, and " +
        "the globals panel.");
            this.cbEnableParser.UseVisualStyleBackColor = true;
            this.cbEnableParser.CheckedChanged += new System.EventHandler(this.cbEnableParser_CheckedChanged);
            // 
            // cbShortCircuit
            // 
            this.cbShortCircuit.AutoSize = true;
            this.cbShortCircuit.Location = new System.Drawing.Point(204, 67);
            this.cbShortCircuit.Name = "cbShortCircuit";
            this.cbShortCircuit.Size = new System.Drawing.Size(150, 19);
            this.cbShortCircuit.TabIndex = 6;
            this.cbShortCircuit.Text = "Short-circuit evaluation";
            this.toolTip.SetToolTip(this.cbShortCircuit, "With short-circuit evaluation, the right side of AND or OR is skipped once the re" +
        "sult is known.\nExample: in X AND Y, Y is skipped when X is false. In X OR Y, Y i" +
        "s skipped when X is true.");
            this.cbShortCircuit.UseVisualStyleBackColor = true;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 30000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // cbAutocomplete
            // 
            this.cbAutocomplete.AutoSize = true;
            this.cbAutocomplete.Location = new System.Drawing.Point(6, 19);
            this.cbAutocomplete.Name = "cbAutocomplete";
            this.cbAutocomplete.Size = new System.Drawing.Size(140, 19);
            this.cbAutocomplete.TabIndex = 0;
            this.cbAutocomplete.Text = "Enable Autocomplete";
            this.toolTip.SetToolTip(this.cbAutocomplete, "Show procedure, variable, constant, and macro suggestions as you type.\nPress Ctrl" +
        "+Space to open the suggestion list.");
            this.cbAutocomplete.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(495, 664);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 27);
            this.button1.TabIndex = 9;
            this.button1.Text = "OK";
            this.toolTip.SetToolTip(this.button1, "Save settings and close.");
            this.button1.UseVisualStyleBackColor = true;
            // 
            // cbParserWarn
            // 
            this.cbParserWarn.AutoSize = true;
            this.cbParserWarn.BackColor = System.Drawing.Color.Transparent;
            this.cbParserWarn.Location = new System.Drawing.Point(96, 17);
            this.cbParserWarn.Name = "cbParserWarn";
            this.cbParserWarn.Size = new System.Drawing.Size(76, 19);
            this.cbParserWarn.TabIndex = 1;
            this.cbParserWarn.Text = "Warnings";
            this.toolTip.SetToolTip(this.cbParserWarn, "Show parser warnings.");
            this.cbParserWarn.UseVisualStyleBackColor = false;
            // 
            // cbCompilePath
            // 
            this.cbCompilePath.AutoSize = true;
            this.cbCompilePath.Location = new System.Drawing.Point(363, 12);
            this.cbCompilePath.Name = "cbCompilePath";
            this.cbCompilePath.Size = new System.Drawing.Size(170, 19);
            this.cbCompilePath.TabIndex = 0;
            this.cbCompilePath.Text = "Don\'t use compilation path";
            this.toolTip.SetToolTip(this.cbCompilePath, "Compile scripts in the same folder as their source .ssl file.");
            this.cbCompilePath.UseVisualStyleBackColor = true;
            this.cbCompilePath.CheckedChanged += new System.EventHandler(this.cbCompilePath_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.msgPathlistView);
            this.groupBox3.Location = new System.Drawing.Point(8, 540);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(434, 118);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Paths to message files";
            this.toolTip.SetToolTip(this.groupBox3, "Additional folders in which the editor will search for associated message files.\r" +
        "\nRight-click the list to add, remove, or reorder paths.");
            // 
            // msgPathlistView
            // 
            this.msgPathlistView.BackColor = System.Drawing.SystemColors.Window;
            this.msgPathlistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.msgPathlistView.ContextMenuStrip = this.MsgcontextMenu;
            this.msgPathlistView.FullRowSelect = true;
            this.msgPathlistView.GridLines = true;
            this.msgPathlistView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.msgPathlistView.HideSelection = false;
            this.msgPathlistView.Location = new System.Drawing.Point(8, 20);
            this.msgPathlistView.MultiSelect = false;
            this.msgPathlistView.Name = "msgPathlistView";
            this.msgPathlistView.ShowGroups = false;
            this.msgPathlistView.Size = new System.Drawing.Size(420, 90);
            this.msgPathlistView.TabIndex = 0;
            this.toolTip.SetToolTip(this.msgPathlistView, "Search paths for message files.\r\nThe top path has priority over the paths below i" +
        "t.\r\nRight-click this list to add, remove, or reorder paths.");
            this.msgPathlistView.UseCompatibleStateImageBehavior = false;
            this.msgPathlistView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 380;
            // 
            // MsgcontextMenu
            // 
            this.MsgcontextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPathToolStripMenuItem,
            this.deletePathToolStripMenuItem,
            this.toolStripSeparator1,
            this.moveUpToolStripMenuItem,
            this.modeDownToolStripMenuItem});
            this.MsgcontextMenu.Name = "MsgcontextMenu";
            this.MsgcontextMenu.ShowImageMargin = false;
            this.MsgcontextMenu.Size = new System.Drawing.Size(181, 98);
            // 
            // addPathToolStripMenuItem
            // 
            this.addPathToolStripMenuItem.Name = "addPathToolStripMenuItem";
            this.addPathToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addPathToolStripMenuItem.Text = "Add path";
            this.addPathToolStripMenuItem.Click += new System.EventHandler(this.addPathToolStripMenuItem_Click);
            // 
            // deletePathToolStripMenuItem
            // 
            this.deletePathToolStripMenuItem.Name = "deletePathToolStripMenuItem";
            this.deletePathToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deletePathToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deletePathToolStripMenuItem.Text = "Delete path";
            this.deletePathToolStripMenuItem.Click += new System.EventHandler(this.deletePathToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // modeDownToolStripMenuItem
            // 
            this.modeDownToolStripMenuItem.Name = "modeDownToolStripMenuItem";
            this.modeDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.modeDownToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.modeDownToolStripMenuItem.Text = "Move Down";
            this.modeDownToolStripMenuItem.Click += new System.EventHandler(this.modeDownToolStripMenuItem_Click);
            // 
            // bAssociate
            // 
            this.bAssociate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bAssociate.Location = new System.Drawing.Point(8, 78);
            this.bAssociate.Name = "bAssociate";
            this.bAssociate.Size = new System.Drawing.Size(139, 28);
            this.bAssociate.TabIndex = 0;
            this.bAssociate.Text = "Set file associations...";
            this.toolTip.SetToolTip(this.bAssociate, "Associate .ssl, .int, and .msg files with sFall Script Editor.");
            this.bAssociate.UseVisualStyleBackColor = true;
            this.bAssociate.Click += new System.EventHandler(this.bAssociate_Click);
            // 
            // cbUserCompile
            // 
            this.cbUserCompile.AutoSize = true;
            this.cbUserCompile.Location = new System.Drawing.Point(6, 20);
            this.cbUserCompile.Name = "cbUserCompile";
            this.cbUserCompile.Size = new System.Drawing.Size(160, 19);
            this.cbUserCompile.TabIndex = 0;
            this.cbUserCompile.Text = "Use batch file to compile ";
            this.toolTip.SetToolTip(this.cbUserCompile, "Compile scripts using UserComp.bat.");
            this.cbUserCompile.UseVisualStyleBackColor = true;
            this.cbUserCompile.CheckedChanged += new System.EventHandler(this.cbUserCompile_CheckedChanged);
            // 
            // bHeaders
            // 
            this.bHeaders.Image = ((System.Drawing.Image)(resources.GetObject("bHeaders.Image")));
            this.bHeaders.Location = new System.Drawing.Point(555, 76);
            this.bHeaders.Name = "bHeaders";
            this.bHeaders.Size = new System.Drawing.Size(30, 25);
            this.bHeaders.TabIndex = 5;
            this.toolTip.SetToolTip(this.bHeaders, "Choose a folder.");
            this.bHeaders.UseVisualStyleBackColor = true;
            this.bHeaders.Click += new System.EventHandler(this.bHeaders_Click);
            // 
            // HintLang_comboBox
            // 
            this.HintLang_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.HintLang_comboBox.FormattingEnabled = true;
            this.HintLang_comboBox.Items.AddRange(new object[] {
            "English",
            "Russian"});
            this.HintLang_comboBox.Location = new System.Drawing.Point(136, 75);
            this.HintLang_comboBox.Name = "HintLang_comboBox";
            this.HintLang_comboBox.Size = new System.Drawing.Size(112, 23);
            this.HintLang_comboBox.TabIndex = 6;
            this.toolTip.SetToolTip(this.HintLang_comboBox, "Language for function descriptions and help tips.");
            // 
            // Highlight_comboBox
            // 
            this.Highlight_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Highlight_comboBox.FormattingEnabled = true;
            this.Highlight_comboBox.Items.AddRange(new object[] {
            "Original",
            "F-Geck",
            "Dark"});
            this.Highlight_comboBox.Location = new System.Drawing.Point(75, 21);
            this.Highlight_comboBox.Name = "Highlight_comboBox";
            this.Highlight_comboBox.Size = new System.Drawing.Size(79, 23);
            this.Highlight_comboBox.TabIndex = 0;
            this.toolTip.SetToolTip(this.Highlight_comboBox, "Syntax-highlighting scheme for script code.");
            // 
            // InterfaceTheme_comboBox
            // 
            this.InterfaceTheme_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.InterfaceTheme_comboBox.FormattingEnabled = true;
            this.InterfaceTheme_comboBox.Items.AddRange(new object[] {
            "System",
            "Light",
            "Dark"});
            this.InterfaceTheme_comboBox.Location = new System.Drawing.Point(264, 21);
            this.InterfaceTheme_comboBox.Name = "InterfaceTheme_comboBox";
            this.InterfaceTheme_comboBox.Size = new System.Drawing.Size(76, 23);
            this.InterfaceTheme_comboBox.TabIndex = 1;
            this.toolTip.SetToolTip(this.InterfaceTheme_comboBox, "Application color scheme.");
            // 
            // cbAssociateID
            // 
            this.cbAssociateID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbAssociateID.AutoSize = true;
            this.cbAssociateID.Location = new System.Drawing.Point(168, 23);
            this.cbAssociateID.Name = "cbAssociateID";
            this.cbAssociateID.Size = new System.Drawing.Size(106, 19);
            this.cbAssociateID.TabIndex = 1;
            this.cbAssociateID.Text = "Associate by ID";
            this.toolTip.SetToolTip(this.cbAssociateID, "Associate message files by script number rather than name. Requires Scripts.lst.");
            this.cbAssociateID.UseVisualStyleBackColor = true;
            // 
            // cbNonColor
            // 
            this.cbNonColor.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbNonColor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cbNonColor.BackgroundImage")));
            this.cbNonColor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.cbNonColor.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbNonColor.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbNonColor.Location = new System.Drawing.Point(163, 19);
            this.cbNonColor.Name = "cbNonColor";
            this.cbNonColor.Size = new System.Drawing.Size(22, 22);
            this.cbNonColor.TabIndex = 1;
            this.cbNonColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.cbNonColor, "Use colored entries in AutoComplete.");
            this.cbNonColor.UseVisualStyleBackColor = true;
            // 
            // cbAutoPaired
            // 
            this.cbAutoPaired.AutoSize = true;
            this.cbAutoPaired.Location = new System.Drawing.Point(6, 42);
            this.cbAutoPaired.Name = "cbAutoPaired";
            this.cbAutoPaired.Size = new System.Drawing.Size(156, 19);
            this.cbAutoPaired.TabIndex = 2;
            this.cbAutoPaired.Text = "Input paired parentheses";
            this.toolTip.SetToolTip(this.cbAutoPaired, "Automatically insert matching parentheses and quotation marks.");
            this.cbAutoPaired.UseVisualStyleBackColor = true;
            // 
            // tbTabSize
            // 
            this.tbTabSize.Location = new System.Drawing.Point(532, 75);
            this.tbTabSize.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.tbTabSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tbTabSize.Name = "tbTabSize";
            this.tbTabSize.Size = new System.Drawing.Size(52, 23);
            this.tbTabSize.TabIndex = 8;
            this.tbTabSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip.SetToolTip(this.tbTabSize, "Default tab size: 3.");
            this.tbTabSize.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // cbDebug
            // 
            this.cbDebug.AutoSize = true;
            this.cbDebug.BackColor = System.Drawing.SystemColors.Control;
            this.cbDebug.Location = new System.Drawing.Point(112, 67);
            this.cbDebug.Name = "cbDebug";
            this.cbDebug.Size = new System.Drawing.Size(61, 19);
            this.cbDebug.TabIndex = 5;
            this.cbDebug.Text = "Debug";
            this.toolTip.SetToolTip(this.cbDebug, "Show additional optimization diagnostics during compilation.");
            this.cbDebug.UseVisualStyleBackColor = true;
            // 
            // cbShowTips
            // 
            this.cbShowTips.AutoSize = true;
            this.cbShowTips.Location = new System.Drawing.Point(8, 22);
            this.cbShowTips.Name = "cbShowTips";
            this.cbShowTips.Size = new System.Drawing.Size(77, 19);
            this.cbShowTips.TabIndex = 0;
            this.cbShowTips.Text = "Show tips";
            this.toolTip.SetToolTip(this.cbShowTips, "Show code and function tips after typing an opening parenthesis.");
            this.cbShowTips.UseVisualStyleBackColor = true;
            // 
            // cbShortDesc
            // 
            this.cbShortDesc.AutoSize = true;
            this.cbShortDesc.Location = new System.Drawing.Point(91, 22);
            this.cbShortDesc.Name = "cbShortDesc";
            this.cbShortDesc.Size = new System.Drawing.Size(121, 19);
            this.cbShortDesc.TabIndex = 1;
            this.cbShortDesc.Text = "Short descriptions";
            this.toolTip.SetToolTip(this.cbShortDesc, "Show brief descriptions for opcodes.");
            this.cbShortDesc.UseVisualStyleBackColor = true;
            // 
            // cbFonts
            // 
            this.cbFonts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFonts.DropDownWidth = 80;
            this.cbFonts.FormattingEnabled = true;
            this.cbFonts.IntegralHeight = false;
            this.cbFonts.ItemHeight = 15;
            this.cbFonts.Items.AddRange(new object[] {
            "Courier New"});
            this.cbFonts.Location = new System.Drawing.Point(439, 21);
            this.cbFonts.MaxDropDownItems = 10;
            this.cbFonts.Name = "cbFonts";
            this.cbFonts.Size = new System.Drawing.Size(145, 23);
            this.cbFonts.TabIndex = 2;
            this.toolTip.SetToolTip(this.cbFonts, "Font for the text editor code. The font size is set in the main editor window in " +
        "the lower right corner.\r\n(Size is set from +3 ...- 3 of the default size for the" +
        " selected font)");
            // 
            // cbReopenLastTabs
            // 
            this.cbReopenLastTabs.AutoSize = true;
            this.cbReopenLastTabs.Location = new System.Drawing.Point(8, 47);
            this.cbReopenLastTabs.Name = "cbReopenLastTabs";
            this.cbReopenLastTabs.Size = new System.Drawing.Size(209, 19);
            this.cbReopenLastTabs.TabIndex = 3;
            this.cbReopenLastTabs.Text = "Reopen tabs from previous session";
            this.toolTip.SetToolTip(this.cbReopenLastTabs, "Automatically reopen tabs from the previous session.");
            this.cbReopenLastTabs.UseVisualStyleBackColor = true;
            // 
            // cbRestoreUnsavedChanges
            // 
            this.cbRestoreUnsavedChanges.AutoSize = true;
            this.cbRestoreUnsavedChanges.Location = new System.Drawing.Point(269, 47);
            this.cbRestoreUnsavedChanges.Name = "cbRestoreUnsavedChanges";
            this.cbRestoreUnsavedChanges.Size = new System.Drawing.Size(212, 19);
            this.cbRestoreUnsavedChanges.TabIndex = 4;
            this.cbRestoreUnsavedChanges.Text = "Restore unsaved changes on restart";
            this.toolTip.SetToolTip(this.cbRestoreUnsavedChanges, "Keep recovery copies of modified files and open untitled documents for restoratio" +
        "n after a restart or unexpected closure. Saved files are not overwritten automat" +
        "ically.");
            this.cbRestoreUnsavedChanges.UseVisualStyleBackColor = true;
            // 
            // cbStorePosition
            // 
            this.cbStorePosition.AutoSize = true;
            this.cbStorePosition.Location = new System.Drawing.Point(269, 22);
            this.cbStorePosition.Name = "cbStorePosition";
            this.cbStorePosition.Size = new System.Drawing.Size(120, 19);
            this.cbStorePosition.TabIndex = 2;
            this.cbStorePosition.Text = "Store last position";
            this.toolTip.SetToolTip(this.cbStorePosition, "Remember the cursor position in each file and restore it when the file is reopene" +
        "d.");
            this.cbStorePosition.UseVisualStyleBackColor = true;
            // 
            // cmbPreprocessor
            // 
            this.cmbPreprocessor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPreprocessor.DropDownWidth = 180;
            this.cmbPreprocessor.FormattingEnabled = true;
            this.cmbPreprocessor.Items.AddRange(new object[] {
            "Internal MCPP (built-in sfall compiler)",
            "External MCPP",
            "OpenWatcom C32 preprocessor"});
            this.cmbPreprocessor.Location = new System.Drawing.Point(204, 13);
            this.cmbPreprocessor.Name = "cmbPreprocessor";
            this.cmbPreprocessor.Size = new System.Drawing.Size(165, 23);
            this.cmbPreprocessor.TabIndex = 1;
            this.toolTip.SetToolTip(this.cmbPreprocessor, "Uses another preprocessor instead of the built-in sfall compiler.\r\nNote: Try diff" +
        "erent preprocessors if you have problems with pre-processing the script during c" +
        "ompilation.");
            // 
            // labelScriptStyle
            // 
            this.labelScriptStyle.AutoSize = true;
            this.labelScriptStyle.Location = new System.Drawing.Point(9, 25);
            this.labelScriptStyle.Name = "labelScriptStyle";
            this.labelScriptStyle.Size = new System.Drawing.Size(67, 15);
            this.labelScriptStyle.TabIndex = 45;
            this.labelScriptStyle.Text = "Script style:";
            // 
            // labelInterfaceStyle
            // 
            this.labelInterfaceStyle.AutoSize = true;
            this.labelInterfaceStyle.Location = new System.Drawing.Point(182, 25);
            this.labelInterfaceStyle.Name = "labelInterfaceStyle";
            this.labelInterfaceStyle.Size = new System.Drawing.Size(83, 15);
            this.labelInterfaceStyle.TabIndex = 46;
            this.labelInterfaceStyle.Text = "Interface style:";
            // 
            // labelCodeFont
            // 
            this.labelCodeFont.AutoSize = true;
            this.labelCodeFont.Location = new System.Drawing.Point(377, 25);
            this.labelCodeFont.Name = "labelCodeFont";
            this.labelCodeFont.Size = new System.Drawing.Size(63, 15);
            this.labelCodeFont.TabIndex = 47;
            this.labelCodeFont.Text = "Code font:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbPreprocessor);
            this.groupBox1.Controls.Add(this.cbDebug);
            this.groupBox1.Controls.Add(this.cbUserCompile);
            this.groupBox1.Controls.Add(this.cbShortCircuit);
            this.groupBox1.Controls.Add(this.cbWarnings);
            this.groupBox1.Controls.Add(this.cbUseBackward);
            this.groupBox1.Controls.Add(this.cbMultiThread);
            this.groupBox1.Controls.Add(this.cbWarnFailedCompile);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cbOptimize);
            this.groupBox1.Location = new System.Drawing.Point(8, 68);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(389, 125);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Compilation";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.bHeaders);
            this.groupBox2.Controls.Add(this.tbScriptsHPath);
            this.groupBox2.Controls.Add(this.cbCompilePath);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.cbIncludePath);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(label1);
            this.groupBox2.Controls.Add(this.bChange);
            this.groupBox2.Controls.Add(this.bScriptsH);
            this.groupBox2.Controls.Add(label4);
            this.groupBox2.Location = new System.Drawing.Point(8, 313);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(593, 158);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Paths";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 108);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(139, 15);
            this.label8.TabIndex = 17;
            this.label8.Text = "Location of Scripts.H file:";
            // 
            // tbScriptsHPath
            // 
            this.tbScriptsHPath.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tbScriptsHPath.Location = new System.Drawing.Point(6, 124);
            this.tbScriptsHPath.Name = "tbScriptsHPath";
            this.tbScriptsHPath.ReadOnly = true;
            this.tbScriptsHPath.Size = new System.Drawing.Size(543, 23);
            this.tbScriptsHPath.TabIndex = 6;
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.textBox2.Location = new System.Drawing.Point(6, 32);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(543, 23);
            this.textBox2.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.textBox1.Location = new System.Drawing.Point(6, 78);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(543, 23);
            this.textBox1.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 15);
            this.label2.TabIndex = 26;
            this.label2.Text = "Description language:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(8, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 55);
            this.label3.TabIndex = 29;
            this.label3.Text = "Choose which supported files Windows opens with this editor.";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbParserWarn);
            this.groupBox4.Controls.Add(this.cbEnableParser);
            this.groupBox4.Location = new System.Drawing.Point(408, 68);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(193, 50);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Parser";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbNonColor);
            this.groupBox5.Controls.Add(this.cbAutoPaired);
            this.groupBox5.Controls.Add(this.cbAutocomplete);
            this.groupBox5.Location = new System.Drawing.Point(408, 124);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(193, 69);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Code editor";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(436, 79);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 15);
            this.label7.TabIndex = 42;
            this.label7.Text = "Tab/indent size:";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.Highlight_comboBox);
            this.groupBox6.Controls.Add(this.InterfaceTheme_comboBox);
            this.groupBox6.Controls.Add(this.cbFonts);
            this.groupBox6.Controls.Add(this.labelScriptStyle);
            this.groupBox6.Controls.Add(this.labelInterfaceStyle);
            this.groupBox6.Controls.Add(this.labelCodeFont);
            this.groupBox6.Location = new System.Drawing.Point(8, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(593, 56);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Appearance";
            // 
            // groupEditorBehavior
            // 
            this.groupEditorBehavior.Controls.Add(this.cbShowTips);
            this.groupEditorBehavior.Controls.Add(this.cbShortDesc);
            this.groupEditorBehavior.Controls.Add(this.cbStorePosition);
            this.groupEditorBehavior.Controls.Add(this.cbReopenLastTabs);
            this.groupEditorBehavior.Controls.Add(this.cbRestoreUnsavedChanges);
            this.groupEditorBehavior.Controls.Add(this.label2);
            this.groupEditorBehavior.Controls.Add(this.HintLang_comboBox);
            this.groupEditorBehavior.Controls.Add(this.cbTabsToSpaces);
            this.groupEditorBehavior.Controls.Add(this.label7);
            this.groupEditorBehavior.Controls.Add(this.tbTabSize);
            this.groupEditorBehavior.Location = new System.Drawing.Point(8, 199);
            this.groupEditorBehavior.Name = "groupEditorBehavior";
            this.groupEditorBehavior.Size = new System.Drawing.Size(593, 108);
            this.groupEditorBehavior.TabIndex = 4;
            this.groupEditorBehavior.TabStop = false;
            this.groupEditorBehavior.Text = "Editing";
            // 
            // groupMessageOptions
            // 
            this.groupMessageOptions.Controls.Add(this.cbAutoOpenMessages);
            this.groupMessageOptions.Controls.Add(this.cbAssociateID);
            this.groupMessageOptions.Controls.Add(this.label5);
            this.groupMessageOptions.Controls.Add(this.tbLanguage);
            this.groupMessageOptions.Location = new System.Drawing.Point(8, 477);
            this.groupMessageOptions.Name = "groupMessageOptions";
            this.groupMessageOptions.Size = new System.Drawing.Size(593, 51);
            this.groupMessageOptions.TabIndex = 6;
            this.groupMessageOptions.TabStop = false;
            this.groupMessageOptions.Text = "Message files";
            // 
            // groupFileAssociation
            // 
            this.groupFileAssociation.Controls.Add(this.label3);
            this.groupFileAssociation.Controls.Add(this.bAssociate);
            this.groupFileAssociation.Location = new System.Drawing.Point(448, 540);
            this.groupFileAssociation.Name = "groupFileAssociation";
            this.groupFileAssociation.Size = new System.Drawing.Size(153, 118);
            this.groupFileAssociation.TabIndex = 8;
            this.groupFileAssociation.TabStop = false;
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(611, 699);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupEditorBehavior);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupMessageOptions);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupFileAssociation);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsDialog_FormClosing);
            this.Load += new System.EventHandler(this.SettingsDialog_Load);
            this.groupBox3.ResumeLayout(false);
            this.MsgcontextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbTabSize)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupEditorBehavior.ResumeLayout(false);
            this.groupEditorBehavior.PerformLayout();
            this.groupMessageOptions.ResumeLayout(false);
            this.groupMessageOptions.PerformLayout();
            this.groupFileAssociation.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.CheckBox cbAutocomplete;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox cbShortCircuit;
        private System.Windows.Forms.CheckBox cbEnableParser;
        private System.Windows.Forms.CheckBox cbTabsToSpaces;

        #endregion

        private System.Windows.Forms.CheckBox cbWarnings;
        private System.Windows.Forms.CheckBox cbUseBackward;
        private System.Windows.Forms.CheckBox cbIncludePath;
        private System.Windows.Forms.Button bChange;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox cbWarnFailedCompile;
        private System.Windows.Forms.CheckBox cbMultiThread;
        private System.Windows.Forms.CheckBox cbAutoOpenMessages;
        private System.Windows.Forms.Button bScriptsH;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox tbLanguage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbOptimize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox HintLang_comboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView msgPathlistView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ComboBox Highlight_comboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbParserWarn;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbCompilePath;
        private System.Windows.Forms.ContextMenuStrip MsgcontextMenu;
        private System.Windows.Forms.ToolStripMenuItem addPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deletePathToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modeDownToolStripMenuItem;
        private System.Windows.Forms.Button bAssociate;
        private System.Windows.Forms.CheckBox cbUserCompile;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button bHeaders;
        private System.Windows.Forms.TextBox tbScriptsHPath;
        private System.Windows.Forms.CheckBox cbAssociateID;
        private System.Windows.Forms.NumericUpDown tbTabSize;
        private System.Windows.Forms.CheckBox cbNonColor;
        private System.Windows.Forms.CheckBox cbAutoPaired;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox cbDebug;
        private System.Windows.Forms.CheckBox cbShowTips;
        private System.Windows.Forms.CheckBox cbShortDesc;
        private System.Windows.Forms.ComboBox cbFonts;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox cbStorePosition;
        private System.Windows.Forms.CheckBox cbReopenLastTabs;
        private System.Windows.Forms.CheckBox cbRestoreUnsavedChanges;
        private System.Windows.Forms.ComboBox cmbPreprocessor;
        private System.Windows.Forms.ComboBox InterfaceTheme_comboBox;
        private System.Windows.Forms.Label labelScriptStyle;
        private System.Windows.Forms.Label labelInterfaceStyle;
        private System.Windows.Forms.Label labelCodeFont;
    }
}

