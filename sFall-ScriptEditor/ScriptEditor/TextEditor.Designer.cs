namespace ScriptEditor {
    partial class TextEditor {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextEditor));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.FunctionTreeLeft = new System.Windows.Forms.TreeView();
            this.cmsFunctions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addUserFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator51 = new System.Windows.Forms.ToolStripSeparator();
            this.addTreeNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameTreeNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator52 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteNodeFuncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TabClose_button = new System.Windows.Forms.Button();
            this.Split_button = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SearchToolStrip = new System.Windows.Forms.ToolStrip();
            this.CaseButton = new System.Windows.Forms.ToolStripButton();
            this.WholeWordButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.SendtoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.SearchTextComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.FindForwardButton = new System.Windows.Forms.ToolStripButton();
            this.FindBackButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.ReplaceButton = new System.Windows.Forms.ToolStripButton();
            this.ReplaceTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.ReplaceAllButton = new System.Windows.Forms.ToolStripButton();
            this.SearchHideStripButton = new System.Windows.Forms.ToolStripButton();
            this.minimizelog_button = new System.Windows.Forms.Button();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPageParse = new System.Windows.Forms.TabPage();
            this.tbOutputParse = new System.Windows.Forms.TextBox();
            this.tabPageBuild = new System.Windows.Forms.TabPage();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.tabPageError = new System.Windows.Forms.TabPage();
            this.cmsError = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmCopyLogText = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator44 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator47 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmShowParserLog = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmShowBuildLog = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator43 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiClearAllLog = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvErrors = new System.Windows.Forms.DataGridView();
            this.cType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.ProcTree = new System.Windows.Forms.TreeView();
            this.ProcMnContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editNodeCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator38 = new System.Windows.Forms.ToolStripSeparator();
            this.createProcedureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameProcedureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.moveProcedureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteProcedureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripProcedures = new System.Windows.Forms.ToolStrip();
            this.GoBeginStripButton = new System.Windows.Forms.ToolStripButton();
            this.OnlyProcStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.NewProcStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator31 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator32 = new System.Windows.Forms.ToolStripSeparator();
            this.ViewArgsStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator35 = new System.Windows.Forms.ToolStripSeparator();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.FunctionsTree = new System.Windows.Forms.TreeView();
            this.tpExplorerFiles = new System.Windows.Forms.TabPage();
            this.treeProjectFiles = new System.Windows.Forms.TreeView();
            this.toolStripSolution = new System.Windows.Forms.ToolStrip();
            this.tsbSetProjectFolder = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator57 = new System.Windows.Forms.ToolStripSeparator();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.EmptyStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.LineStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ColStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.FontSizeStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripMain = new System.Windows.Forms.ToolStrip();
            this.FunctionButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.New_toolStripDropDownButton = new System.Windows.Forms.ToolStripSplitButton();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.Open_toolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.Save_toolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.Save_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAll_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.SaveAs_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator48 = new System.Windows.Forms.ToolStripSeparator();
            this.saveUTF8ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSaveAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator55 = new System.Windows.Forms.ToolStripSeparator();
            this.Outline_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.Undo_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.Redo_ToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.DecIndentStripButton = new System.Windows.Forms.ToolStripButton();
            this.CommentStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.Search_toolStripButton = new System.Windows.Forms.ToolStripSplitButton();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator33 = new System.Windows.Forms.ToolStripSeparator();
            this.quickFindToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.Back_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.Forward_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.GotoProc_StripButton = new System.Windows.Forms.ToolStripSplitButton();
            this.gotoToLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.Edit_toolStripButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.splitDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ToggleBlockCommentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.capitalizeCaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator40 = new System.Windows.Forms.ToolStripSeparator();
            this.leadingTabsSpacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allTabsSpacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator39 = new System.Windows.Forms.ToolStripSeparator();
            this.trailingSpacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator45 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmMessageTextChecker = new System.Windows.Forms.ToolStripMenuItem();
            this.showAutocompleteWordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator41 = new System.Windows.Forms.ToolStripSeparator();
            this.showTabsAndSpacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator42 = new System.Windows.Forms.ToolStripSeparator();
            this.Script_toolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.editRegisteredScriptsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            this.defineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Headers_toolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.includeFileToCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openAllIncludesScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator50 = new System.Windows.Forms.ToolStripSeparator();
            this.openHeaderFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.MSG_toolStripButton = new System.Windows.Forms.ToolStripSplitButton();
            this.msgFileEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dialogNodesDiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewDialogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.dialogFunctionConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator53 = new System.Windows.Forms.ToolStripSeparator();
            this.msgAutoOpenEditorStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.qCompile_toolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.Compile_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CompileAllOpen_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator34 = new System.Windows.Forms.ToolStripSeparator();
            this.MassCompile_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Preprocess_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roundtripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.decompileF1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oldDecompileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator49 = new System.Windows.Forms.ToolStripSeparator();
            this.pDefineStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.Help_toolStripButton = new System.Windows.Forms.ToolStripSplitButton();
            this.About_toolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmSetProjectFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.Settings_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.encodingMessagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsDefaultMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EncodingDOSmenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.win32RenderTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.caretSoftwareModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator30 = new System.Windows.Forms.ToolStripSeparator();
            this.ParsingErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showIndentLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textLineNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator46 = new System.Windows.Forms.ToolStripSeparator();
            this.browserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLogWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.decIndentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.funcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gotoProcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createProcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbUpdateParserData = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator56 = new System.Windows.Forms.ToolStripSeparator();
            this.tslProject = new System.Windows.Forms.ToolStripLabel();
            this.cmsTabControls = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInExternalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator37 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllButThisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ofdScripts = new System.Windows.Forms.OpenFileDialog();
            this.sfdScripts = new System.Windows.Forms.SaveFileDialog();
            this.fbdMassCompile = new System.Windows.Forms.FolderBrowserDialog();
            this.bwSyntaxParser = new System.ComponentModel.BackgroundWorker();
            this.editorMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.highlightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.findDeclerationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findReferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator36 = new System.Windows.Forms.ToolStripSeparator();
            this.openIncludeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.convertHexDecToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator54 = new System.Windows.Forms.ToolStripSeparator();
            this.UpperCaseToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.LowerCaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator29 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
            this.commentTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncommentTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AlignToLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatingCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.fbdProjectFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new DraggableTabControl();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.cmsFunctions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SearchToolStrip.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPageParse.SuspendLayout();
            this.tabPageBuild.SuspendLayout();
            this.tabPageError.SuspendLayout();
            this.cmsError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).BeginInit();
            this.tabControl3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.ProcMnContext.SuspendLayout();
            this.toolStripProcedures.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tpExplorerFiles.SuspendLayout();
            this.toolStripSolution.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.ToolStripMain.SuspendLayout();
            this.cmsTabControls.SuspendLayout();
            this.editorMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer3);
            this.panel1.Controls.Add(this.ToolStripMain);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1290, 707);
            this.panel1.TabIndex = 2;
            // 
            // splitContainer3
            // 
            this.splitContainer3.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 25);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.FunctionTreeLeft);
            this.splitContainer3.Panel1.Padding = new System.Windows.Forms.Padding(3, 2, 1, 4);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer3.Size = new System.Drawing.Size(1290, 682);
            this.splitContainer3.SplitterDistance = 28;
            this.splitContainer3.TabIndex = 6;
            this.splitContainer3.TabStop = false;
            // 
            // FunctionTreeLeft
            // 
            this.FunctionTreeLeft.BackColor = System.Drawing.Color.GhostWhite;
            this.FunctionTreeLeft.ContextMenuStrip = this.cmsFunctions;
            this.FunctionTreeLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FunctionTreeLeft.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FunctionTreeLeft.ForeColor = System.Drawing.SystemColors.WindowText;
            this.FunctionTreeLeft.Indent = 18;
            this.FunctionTreeLeft.LineColor = System.Drawing.Color.Silver;
            this.FunctionTreeLeft.Location = new System.Drawing.Point(3, 2);
            this.FunctionTreeLeft.Name = "FunctionTreeLeft";
            this.FunctionTreeLeft.ShowNodeToolTips = true;
            this.FunctionTreeLeft.Size = new System.Drawing.Size(24, 676);
            this.FunctionTreeLeft.TabIndex = 0;
            this.FunctionTreeLeft.TabStop = false;
            this.FunctionTreeLeft.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.FunctionsTree_NodeMouseClick);
            this.FunctionTreeLeft.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FunctionTree_MouseMove);
            // 
            // cmsFunctions
            // 
            this.cmsFunctions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addUserFunctionToolStripMenuItem,
            this.editFunctionToolStripMenuItem,
            this.toolStripSeparator51,
            this.addTreeNodeToolStripMenuItem,
            this.renameTreeNodeToolStripMenuItem,
            this.toolStripSeparator52,
            this.deleteNodeFuncToolStripMenuItem});
            this.cmsFunctions.Name = "cmsFunctions";
            this.cmsFunctions.ShowImageMargin = false;
            this.cmsFunctions.Size = new System.Drawing.Size(164, 126);
            this.cmsFunctions.Opening += new System.ComponentModel.CancelEventHandler(this.cmsFunctions_Opening);
            // 
            // addUserFunctionToolStripMenuItem
            // 
            this.addUserFunctionToolStripMenuItem.Name = "addUserFunctionToolStripMenuItem";
            this.addUserFunctionToolStripMenuItem.ShowShortcutKeys = false;
            this.addUserFunctionToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.addUserFunctionToolStripMenuItem.Text = "Add User Function";
            this.addUserFunctionToolStripMenuItem.Click += new System.EventHandler(this.addUserFunctionToolStripMenuItem_Click);
            // 
            // editFunctionToolStripMenuItem
            // 
            this.editFunctionToolStripMenuItem.Name = "editFunctionToolStripMenuItem";
            this.editFunctionToolStripMenuItem.ShowShortcutKeys = false;
            this.editFunctionToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.editFunctionToolStripMenuItem.Text = "Edit User Function";
            this.editFunctionToolStripMenuItem.Click += new System.EventHandler(this.editDescriptionToolStripMenuItem_Click);
            // 
            // toolStripSeparator51
            // 
            this.toolStripSeparator51.Name = "toolStripSeparator51";
            this.toolStripSeparator51.Size = new System.Drawing.Size(160, 6);
            // 
            // addTreeNodeToolStripMenuItem
            // 
            this.addTreeNodeToolStripMenuItem.Name = "addTreeNodeToolStripMenuItem";
            this.addTreeNodeToolStripMenuItem.ShowShortcutKeys = false;
            this.addTreeNodeToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.addTreeNodeToolStripMenuItem.Text = "Add Tree Node";
            this.addTreeNodeToolStripMenuItem.Click += new System.EventHandler(this.addTreeNodeToolStripMenuItem_Click);
            // 
            // renameTreeNodeToolStripMenuItem
            // 
            this.renameTreeNodeToolStripMenuItem.Name = "renameTreeNodeToolStripMenuItem";
            this.renameTreeNodeToolStripMenuItem.ShowShortcutKeys = false;
            this.renameTreeNodeToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.renameTreeNodeToolStripMenuItem.Text = "Rename Tree Node";
            this.renameTreeNodeToolStripMenuItem.Click += new System.EventHandler(this.renameTreeNodeToolStripMenuItem_Click);
            // 
            // toolStripSeparator52
            // 
            this.toolStripSeparator52.Name = "toolStripSeparator52";
            this.toolStripSeparator52.Size = new System.Drawing.Size(160, 6);
            // 
            // deleteNodeFuncToolStripMenuItem
            // 
            this.deleteNodeFuncToolStripMenuItem.Name = "deleteNodeFuncToolStripMenuItem";
            this.deleteNodeFuncToolStripMenuItem.ShowShortcutKeys = false;
            this.deleteNodeFuncToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.deleteNodeFuncToolStripMenuItem.Text = "Delete Node/Function";
            this.deleteNodeFuncToolStripMenuItem.Click += new System.EventHandler(this.deleteNodeFuncToolStripMenuItem_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            this.splitContainer2.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl3);
            this.splitContainer2.Panel2.Controls.Add(this.statusStrip);
            this.splitContainer2.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer2.Size = new System.Drawing.Size(1258, 682);
            this.splitContainer2.SplitterDistance = 987;
            this.splitContainer2.SplitterWidth = 3;
            this.splitContainer2.TabIndex = 4;
            this.splitContainer2.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.TabClose_button);
            this.splitContainer1.Panel1.Controls.Add(this.Split_button);
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel1.Controls.Add(this.SearchToolStrip);
            this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Panel1MinSize = 200;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.minimizelog_button);
            this.splitContainer1.Panel2.Controls.Add(this.tabControl2);
            this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Size = new System.Drawing.Size(987, 682);
            this.splitContainer1.SplitterDistance = 626;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 3;
            this.splitContainer1.TabStop = false;
            // 
            // TabClose_button
            // 
            this.TabClose_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TabClose_button.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.TabClose_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TabClose_button.ForeColor = System.Drawing.Color.Black;
            this.TabClose_button.Image = ((System.Drawing.Image)(resources.GetObject("TabClose_button.Image")));
            this.TabClose_button.Location = new System.Drawing.Point(965, 28);
            this.TabClose_button.Name = "TabClose_button";
            this.TabClose_button.Size = new System.Drawing.Size(18, 18);
            this.TabClose_button.TabIndex = 0;
            this.TabClose_button.TabStop = false;
            this.TabClose_button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTips.SetToolTip(this.TabClose_button, "Close this document");
            this.TabClose_button.UseVisualStyleBackColor = true;
            this.TabClose_button.Visible = false;
            this.TabClose_button.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // Split_button
            // 
            this.Split_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Split_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Split_button.ForeColor = System.Drawing.Color.Black;
            this.Split_button.Image = ((System.Drawing.Image)(resources.GetObject("Split_button.Image")));
            this.Split_button.Location = new System.Drawing.Point(966, 606);
            this.Split_button.Name = "Split_button";
            this.Split_button.Size = new System.Drawing.Size(16, 16);
            this.Split_button.TabIndex = 2;
            this.Split_button.TabStop = false;
            this.toolTips.SetToolTip(this.Split_button, "Split document");
            this.Split_button.UseVisualStyleBackColor = true;
            this.Split_button.Visible = false;
            this.Split_button.Click += new System.EventHandler(this.SplitDoc_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "save.png");
            this.imageList1.Images.SetKeyName(1, "nosave.png");
            // 
            // SearchToolStrip
            // 
            this.SearchToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CaseButton,
            this.WholeWordButton,
            this.toolStripSeparator23,
            this.SendtoolStripButton,
            this.SearchTextComboBox,
            this.FindForwardButton,
            this.FindBackButton,
            this.toolStripSeparator22,
            this.ReplaceButton,
            this.ReplaceTextBox,
            this.ReplaceAllButton,
            this.SearchHideStripButton});
            this.SearchToolStrip.Location = new System.Drawing.Point(0, 0);
            this.SearchToolStrip.Name = "SearchToolStrip";
            this.SearchToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.SearchToolStrip.Size = new System.Drawing.Size(987, 25);
            this.SearchToolStrip.TabIndex = 3;
            this.SearchToolStrip.Resize += new System.EventHandler(this.SearchToolStrip_Resize);
            // 
            // CaseButton
            // 
            this.CaseButton.CheckOnClick = true;
            this.CaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CaseButton.Image = ((System.Drawing.Image)(resources.GetObject("CaseButton.Image")));
            this.CaseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CaseButton.Name = "CaseButton";
            this.CaseButton.Size = new System.Drawing.Size(23, 22);
            this.CaseButton.Text = "Match case";
            // 
            // WholeWordButton
            // 
            this.WholeWordButton.CheckOnClick = true;
            this.WholeWordButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.WholeWordButton.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WholeWordButton.ForeColor = System.Drawing.Color.RoyalBlue;
            this.WholeWordButton.Image = ((System.Drawing.Image)(resources.GetObject("WholeWordButton.Image")));
            this.WholeWordButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.WholeWordButton.Name = "WholeWordButton";
            this.WholeWordButton.Size = new System.Drawing.Size(27, 22);
            this.WholeWordButton.Text = "W";
            this.WholeWordButton.ToolTipText = "Whole word";
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            this.toolStripSeparator23.Size = new System.Drawing.Size(6, 25);
            // 
            // SendtoolStripButton
            // 
            this.SendtoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SendtoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("SendtoolStripButton.Image")));
            this.SendtoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SendtoolStripButton.Name = "SendtoolStripButton";
            this.SendtoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.SendtoolStripButton.ToolTipText = "Send word under cursor.";
            this.SendtoolStripButton.Click += new System.EventHandler(this.SendtoolStripButton_Click);
            // 
            // SearchTextComboBox
            // 
            this.SearchTextComboBox.AutoSize = false;
            this.SearchTextComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.SearchTextComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.SearchTextComboBox.MaxDropDownItems = 16;
            this.SearchTextComboBox.Name = "SearchTextComboBox";
            this.SearchTextComboBox.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.SearchTextComboBox.Size = new System.Drawing.Size(150, 22);
            this.SearchTextComboBox.ToolTipText = "Search text";
            // 
            // FindForwardButton
            // 
            this.FindForwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.FindForwardButton.Image = ((System.Drawing.Image)(resources.GetObject("FindForwardButton.Image")));
            this.FindForwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FindForwardButton.Name = "FindForwardButton";
            this.FindForwardButton.Size = new System.Drawing.Size(23, 22);
            this.FindForwardButton.Text = "Find Forward";
            this.FindForwardButton.ToolTipText = "Find Forward [F3]";
            this.FindForwardButton.Click += new System.EventHandler(this.FindForwardButton_Click);
            // 
            // FindBackButton
            // 
            this.FindBackButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.FindBackButton.Image = ((System.Drawing.Image)(resources.GetObject("FindBackButton.Image")));
            this.FindBackButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FindBackButton.Name = "FindBackButton";
            this.FindBackButton.Size = new System.Drawing.Size(23, 22);
            this.FindBackButton.Text = "Find Back [Ctrl+F3]";
            this.FindBackButton.ToolTipText = "Find Back [Shift+F3]";
            this.FindBackButton.Click += new System.EventHandler(this.FindBackButton_Click);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.AutoSize = false;
            this.toolStripSeparator22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            this.toolStripSeparator22.Size = new System.Drawing.Size(6, 25);
            // 
            // ReplaceButton
            // 
            this.ReplaceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ReplaceButton.Image = ((System.Drawing.Image)(resources.GetObject("ReplaceButton.Image")));
            this.ReplaceButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.ReplaceButton.Name = "ReplaceButton";
            this.ReplaceButton.Size = new System.Drawing.Size(23, 22);
            this.ReplaceButton.Text = "Replace";
            this.ReplaceButton.Click += new System.EventHandler(this.ReplaceButton_Click);
            // 
            // ReplaceTextBox
            // 
            this.ReplaceTextBox.AutoSize = false;
            this.ReplaceTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.ReplaceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ReplaceTextBox.HideSelection = false;
            this.ReplaceTextBox.Name = "ReplaceTextBox";
            this.ReplaceTextBox.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.ReplaceTextBox.Size = new System.Drawing.Size(150, 22);
            this.ReplaceTextBox.ToolTipText = "Replace text";
            // 
            // ReplaceAllButton
            // 
            this.ReplaceAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ReplaceAllButton.Image = ((System.Drawing.Image)(resources.GetObject("ReplaceAllButton.Image")));
            this.ReplaceAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ReplaceAllButton.Name = "ReplaceAllButton";
            this.ReplaceAllButton.Size = new System.Drawing.Size(23, 22);
            this.ReplaceAllButton.Text = "Replace All";
            this.ReplaceAllButton.Click += new System.EventHandler(this.ReplaceAllButton_Click);
            // 
            // SearchHideStripButton
            // 
            this.SearchHideStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.SearchHideStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SearchHideStripButton.Image = ((System.Drawing.Image)(resources.GetObject("SearchHideStripButton.Image")));
            this.SearchHideStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SearchHideStripButton.Name = "SearchHideStripButton";
            this.SearchHideStripButton.Size = new System.Drawing.Size(23, 22);
            this.SearchHideStripButton.Text = "Hide panel";
            this.SearchHideStripButton.Click += new System.EventHandler(this.Search_Panel);
            // 
            // minimizelog_button
            // 
            this.minimizelog_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minimizelog_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.minimizelog_button.Image = ((System.Drawing.Image)(resources.GetObject("minimizelog_button.Image")));
            this.minimizelog_button.Location = new System.Drawing.Point(964, -1);
            this.minimizelog_button.Name = "minimizelog_button";
            this.minimizelog_button.Size = new System.Drawing.Size(20, 20);
            this.minimizelog_button.TabIndex = 6;
            this.minimizelog_button.TabStop = false;
            this.minimizelog_button.Tag = "0";
            this.toolTips.SetToolTip(this.minimizelog_button, "Minimize Log");
            this.minimizelog_button.UseVisualStyleBackColor = true;
            this.minimizelog_button.Click += new System.EventHandler(this.minimize_log_button_Click);
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPageParse);
            this.tabControl2.Controls.Add(this.tabPageBuild);
            this.tabControl2.Controls.Add(this.tabPageError);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.ItemSize = new System.Drawing.Size(75, 18);
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.ShowToolTips = true;
            this.tabControl2.Size = new System.Drawing.Size(987, 54);
            this.tabControl2.TabIndex = 1;
            this.tabControl2.TabStop = false;
            this.tabControl2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tabControl2_MouseClick);
            // 
            // tabPageParse
            // 
            this.tabPageParse.Controls.Add(this.tbOutputParse);
            this.tabPageParse.Location = new System.Drawing.Point(4, 22);
            this.tabPageParse.Name = "tabPageParse";
            this.tabPageParse.Size = new System.Drawing.Size(979, 28);
            this.tabPageParse.TabIndex = 2;
            this.tabPageParse.Text = "Parser";
            this.tabPageParse.ToolTipText = "Parser output log";
            this.tabPageParse.UseVisualStyleBackColor = true;
            // 
            // tbOutputParse
            // 
            this.tbOutputParse.AcceptsReturn = true;
            this.tbOutputParse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(35)))));
            this.tbOutputParse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbOutputParse.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbOutputParse.ForeColor = System.Drawing.Color.Cornsilk;
            this.tbOutputParse.Location = new System.Drawing.Point(0, 0);
            this.tbOutputParse.Margin = new System.Windows.Forms.Padding(0);
            this.tbOutputParse.MaxLength = 327670000;
            this.tbOutputParse.Multiline = true;
            this.tbOutputParse.Name = "tbOutputParse";
            this.tbOutputParse.ReadOnly = true;
            this.tbOutputParse.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbOutputParse.Size = new System.Drawing.Size(979, 28);
            this.tbOutputParse.TabIndex = 1;
            this.tbOutputParse.TabStop = false;
            // 
            // tabPageBuild
            // 
            this.tabPageBuild.Controls.Add(this.tbOutput);
            this.tabPageBuild.Location = new System.Drawing.Point(4, 22);
            this.tabPageBuild.Name = "tabPageBuild";
            this.tabPageBuild.Size = new System.Drawing.Size(979, 28);
            this.tabPageBuild.TabIndex = 0;
            this.tabPageBuild.Text = "Build";
            this.tabPageBuild.ToolTipText = "Build output log";
            this.tabPageBuild.UseVisualStyleBackColor = true;
            // 
            // tbOutput
            // 
            this.tbOutput.AcceptsReturn = true;
            this.tbOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(35)))));
            this.tbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbOutput.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbOutput.ForeColor = System.Drawing.Color.Cornsilk;
            this.tbOutput.Location = new System.Drawing.Point(0, 0);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ReadOnly = true;
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbOutput.Size = new System.Drawing.Size(979, 28);
            this.tbOutput.TabIndex = 0;
            this.tbOutput.TabStop = false;
            // 
            // tabPageError
            // 
            this.tabPageError.ContextMenuStrip = this.cmsError;
            this.tabPageError.Controls.Add(this.dgvErrors);
            this.tabPageError.Location = new System.Drawing.Point(4, 22);
            this.tabPageError.Name = "tabPageError";
            this.tabPageError.Size = new System.Drawing.Size(979, 28);
            this.tabPageError.TabIndex = 1;
            this.tabPageError.Text = "Errors";
            this.tabPageError.UseVisualStyleBackColor = true;
            // 
            // cmsError
            // 
            this.cmsError.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmCopyLogText,
            this.toolStripSeparator44,
            this.refreshLogToolStripMenuItem,
            this.autoRefreshToolStripMenuItem,
            this.toolStripSeparator47,
            this.tsmShowParserLog,
            this.tsmShowBuildLog,
            this.toolStripSeparator43,
            this.tsmiClearAllLog});
            this.cmsError.Name = "cmsError";
            this.cmsError.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.cmsError.Size = new System.Drawing.Size(158, 154);
            // 
            // tsmCopyLogText
            // 
            this.tsmCopyLogText.Name = "tsmCopyLogText";
            this.tsmCopyLogText.ShowShortcutKeys = false;
            this.tsmCopyLogText.Size = new System.Drawing.Size(157, 22);
            this.tsmCopyLogText.Text = "Copy select text";
            this.tsmCopyLogText.Click += new System.EventHandler(this.tsmCopyLogText_Click);
            // 
            // toolStripSeparator44
            // 
            this.toolStripSeparator44.Name = "toolStripSeparator44";
            this.toolStripSeparator44.Size = new System.Drawing.Size(154, 6);
            // 
            // refreshLogToolStripMenuItem
            // 
            this.refreshLogToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshLogToolStripMenuItem.Image")));
            this.refreshLogToolStripMenuItem.Name = "refreshLogToolStripMenuItem";
            this.refreshLogToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.refreshLogToolStripMenuItem.Text = "Refresh log";
            this.refreshLogToolStripMenuItem.Click += new System.EventHandler(this.RefreshLog_Click);
            // 
            // autoRefreshToolStripMenuItem
            // 
            this.autoRefreshToolStripMenuItem.CheckOnClick = true;
            this.autoRefreshToolStripMenuItem.Name = "autoRefreshToolStripMenuItem";
            this.autoRefreshToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.autoRefreshToolStripMenuItem.Text = "Auto Refresh";
            // 
            // toolStripSeparator47
            // 
            this.toolStripSeparator47.Name = "toolStripSeparator47";
            this.toolStripSeparator47.Size = new System.Drawing.Size(154, 6);
            // 
            // tsmShowParserLog
            // 
            this.tsmShowParserLog.Checked = true;
            this.tsmShowParserLog.CheckOnClick = true;
            this.tsmShowParserLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmShowParserLog.Name = "tsmShowParserLog";
            this.tsmShowParserLog.ShowShortcutKeys = false;
            this.tsmShowParserLog.Size = new System.Drawing.Size(157, 22);
            this.tsmShowParserLog.Text = "Show parser log";
            this.tsmShowParserLog.Click += new System.EventHandler(this.RefreshLog_Click);
            // 
            // tsmShowBuildLog
            // 
            this.tsmShowBuildLog.Checked = true;
            this.tsmShowBuildLog.CheckOnClick = true;
            this.tsmShowBuildLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmShowBuildLog.Name = "tsmShowBuildLog";
            this.tsmShowBuildLog.ShowShortcutKeys = false;
            this.tsmShowBuildLog.Size = new System.Drawing.Size(157, 22);
            this.tsmShowBuildLog.Text = "Show build log";
            this.tsmShowBuildLog.Click += new System.EventHandler(this.RefreshLog_Click);
            // 
            // toolStripSeparator43
            // 
            this.toolStripSeparator43.Name = "toolStripSeparator43";
            this.toolStripSeparator43.Size = new System.Drawing.Size(154, 6);
            // 
            // tsmiClearAllLog
            // 
            this.tsmiClearAllLog.Name = "tsmiClearAllLog";
            this.tsmiClearAllLog.ShowShortcutKeys = false;
            this.tsmiClearAllLog.Size = new System.Drawing.Size(157, 22);
            this.tsmiClearAllLog.Text = "Clear All";
            this.tsmiClearAllLog.Click += new System.EventHandler(this.tsmiClearAllLog_Click);
            // 
            // dgvErrors
            // 
            this.dgvErrors.AllowUserToAddRows = false;
            this.dgvErrors.AllowUserToDeleteRows = false;
            this.dgvErrors.AllowUserToResizeRows = false;
            this.dgvErrors.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvErrors.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvErrors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvErrors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cType,
            this.cFile,
            this.cLine,
            this.cMessage});
            this.dgvErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvErrors.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvErrors.Location = new System.Drawing.Point(0, 0);
            this.dgvErrors.MultiSelect = false;
            this.dgvErrors.Name = "dgvErrors";
            this.dgvErrors.ReadOnly = true;
            this.dgvErrors.RowHeadersVisible = false;
            this.dgvErrors.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvErrors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvErrors.Size = new System.Drawing.Size(979, 28);
            this.dgvErrors.TabIndex = 0;
            this.dgvErrors.TabStop = false;
            this.dgvErrors.DoubleClick += new System.EventHandler(this.dgvErrors_DoubleClick);
            // 
            // cType
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.cType.DefaultCellStyle = dataGridViewCellStyle3;
            this.cType.HeaderText = "Type";
            this.cType.Name = "cType";
            this.cType.ReadOnly = true;
            this.cType.Width = 55;
            // 
            // cFile
            // 
            this.cFile.HeaderText = "File";
            this.cFile.Name = "cFile";
            this.cFile.ReadOnly = true;
            // 
            // cLine
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.cLine.DefaultCellStyle = dataGridViewCellStyle4;
            this.cLine.HeaderText = "Line";
            this.cLine.Name = "cLine";
            this.cLine.ReadOnly = true;
            this.cLine.Width = 40;
            // 
            // cMessage
            // 
            this.cMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cMessage.HeaderText = "Message";
            this.cMessage.Name = "cMessage";
            this.cMessage.ReadOnly = true;
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tabPage4);
            this.tabControl3.Controls.Add(this.tabPage6);
            this.tabControl3.Controls.Add(this.tpExplorerFiles);
            this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabControl3.ImageList = this.imageList1;
            this.tabControl3.ItemSize = new System.Drawing.Size(60, 18);
            this.tabControl3.Location = new System.Drawing.Point(0, 0);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.Padding = new System.Drawing.Point(0, 0);
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.ShowToolTips = true;
            this.tabControl3.Size = new System.Drawing.Size(265, 654);
            this.tabControl3.TabIndex = 1;
            this.tabControl3.TabStop = false;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage4.Controls.Add(this.ProcTree);
            this.tabPage4.Controls.Add(this.toolStripProcedures);
            this.tabPage4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabPage4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(0, 0, 2, 2);
            this.tabPage4.Size = new System.Drawing.Size(257, 628);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "Procedures";
            this.tabPage4.ToolTipText = "List of script procedures";
            // 
            // ProcTree
            // 
            this.ProcTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.ProcTree.ContextMenuStrip = this.ProcMnContext;
            this.ProcTree.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ProcTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProcTree.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ProcTree.HotTracking = true;
            this.ProcTree.Indent = 18;
            this.ProcTree.ItemHeight = 16;
            this.ProcTree.Location = new System.Drawing.Point(0, 25);
            this.ProcTree.Name = "ProcTree";
            this.ProcTree.ShowNodeToolTips = true;
            this.ProcTree.ShowRootLines = false;
            this.ProcTree.Size = new System.Drawing.Size(255, 601);
            this.ProcTree.TabIndex = 0;
            this.ProcTree.TabStop = false;
            this.ProcTree.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.ProcTree_BeforeExpandCollapse);
            this.ProcTree.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.Tree_AfterExpandCollapse);
            this.ProcTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.ProcTree_BeforeExpandCollapse);
            this.ProcTree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.Tree_AfterExpandCollapse);
            this.ProcTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterSelect);
            this.ProcTree.Leave += new System.EventHandler(this.ProcTree_Leave);
            this.ProcTree.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ProcTree_MouseClick);
            this.ProcTree.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TreeView_DClickMouse);
            this.ProcTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ProcTree_MouseDown);
            this.ProcTree.MouseLeave += new System.EventHandler(this.ProcTree_MouseLeave);
            this.ProcTree.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ProcTree_MouseMove);
            // 
            // ProcMnContext
            // 
            this.ProcMnContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editNodeCodeToolStripMenuItem,
            this.toolStripSeparator38,
            this.createProcedureToolStripMenuItem,
            this.renameProcedureToolStripMenuItem,
            this.toolStripSeparator20,
            this.moveProcedureToolStripMenuItem,
            this.deleteProcedureToolStripMenuItem});
            this.ProcMnContext.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.ProcMnContext.Name = "ProcMnContext";
            this.ProcMnContext.Size = new System.Drawing.Size(220, 126);
            this.ProcMnContext.Opening += new System.ComponentModel.CancelEventHandler(this.ProcMnContext_Opening);
            // 
            // editNodeCodeToolStripMenuItem
            // 
            this.editNodeCodeToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.editNodeCodeToolStripMenuItem.Name = "editNodeCodeToolStripMenuItem";
            this.editNodeCodeToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.editNodeCodeToolStripMenuItem.Text = "Edit Node code";
            this.editNodeCodeToolStripMenuItem.Click += new System.EventHandler(this.editNodeCodeToolStripMenuItem_Click);
            // 
            // toolStripSeparator38
            // 
            this.toolStripSeparator38.Name = "toolStripSeparator38";
            this.toolStripSeparator38.Size = new System.Drawing.Size(216, 6);
            // 
            // createProcedureToolStripMenuItem
            // 
            this.createProcedureToolStripMenuItem.Name = "createProcedureToolStripMenuItem";
            this.createProcedureToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.createProcedureToolStripMenuItem.Text = "Create Procedure    Ctrl+P";
            this.createProcedureToolStripMenuItem.Click += new System.EventHandler(this.createProcedureToolStripMenuItem_Click);
            // 
            // renameProcedureToolStripMenuItem
            // 
            this.renameProcedureToolStripMenuItem.Enabled = false;
            this.renameProcedureToolStripMenuItem.Name = "renameProcedureToolStripMenuItem";
            this.renameProcedureToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.renameProcedureToolStripMenuItem.Text = "Rename Procedure";
            this.renameProcedureToolStripMenuItem.Click += new System.EventHandler(this.renameProcedureToolStripMenuItem_Click);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(216, 6);
            // 
            // moveProcedureToolStripMenuItem
            // 
            this.moveProcedureToolStripMenuItem.Enabled = false;
            this.moveProcedureToolStripMenuItem.Name = "moveProcedureToolStripMenuItem";
            this.moveProcedureToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.moveProcedureToolStripMenuItem.Text = "Move Procedure";
            this.moveProcedureToolStripMenuItem.Click += new System.EventHandler(this.moveProcedureToolStripMenuItem_Click);
            // 
            // deleteProcedureToolStripMenuItem
            // 
            this.deleteProcedureToolStripMenuItem.Enabled = false;
            this.deleteProcedureToolStripMenuItem.Name = "deleteProcedureToolStripMenuItem";
            this.deleteProcedureToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.deleteProcedureToolStripMenuItem.Text = "Delete Procedure";
            this.deleteProcedureToolStripMenuItem.Click += new System.EventHandler(this.deleteProcedureToolStripMenuItem_Click);
            // 
            // toolStripProcedures
            // 
            this.toolStripProcedures.BackColor = System.Drawing.Color.Transparent;
            this.toolStripProcedures.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripProcedures.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GoBeginStripButton,
            this.OnlyProcStripButton,
            this.toolStripSeparator19,
            this.NewProcStripButton,
            this.toolStripSeparator31,
            this.toolStripSeparator32,
            this.ViewArgsStripButton,
            this.toolStripSeparator35});
            this.toolStripProcedures.Location = new System.Drawing.Point(0, 0);
            this.toolStripProcedures.Name = "toolStripProcedures";
            this.toolStripProcedures.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripProcedures.Size = new System.Drawing.Size(258, 25);
            this.toolStripProcedures.TabIndex = 1;
            // 
            // GoBeginStripButton
            // 
            this.GoBeginStripButton.AutoSize = false;
            this.GoBeginStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.GoBeginStripButton.Image = ((System.Drawing.Image)(resources.GetObject("GoBeginStripButton.Image")));
            this.GoBeginStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.GoBeginStripButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.GoBeginStripButton.Name = "GoBeginStripButton";
            this.GoBeginStripButton.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.GoBeginStripButton.Size = new System.Drawing.Size(26, 22);
            this.GoBeginStripButton.Text = "Begin";
            this.GoBeginStripButton.ToolTipText = "Goto definitions script section.";
            this.GoBeginStripButton.Click += new System.EventHandler(this.GoBeginStripButton_Click);
            // 
            // OnlyProcStripButton
            // 
            this.OnlyProcStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.OnlyProcStripButton.CheckOnClick = true;
            this.OnlyProcStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.OnlyProcStripButton.Image = ((System.Drawing.Image)(resources.GetObject("OnlyProcStripButton.Image")));
            this.OnlyProcStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.OnlyProcStripButton.Name = "OnlyProcStripButton";
            this.OnlyProcStripButton.Size = new System.Drawing.Size(23, 22);
            this.OnlyProcStripButton.ToolTipText = "Closing procedure folders when going to selected procedure.";
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripSeparator19.Size = new System.Drawing.Size(6, 25);
            // 
            // NewProcStripButton
            // 
            this.NewProcStripButton.Image = ((System.Drawing.Image)(resources.GetObject("NewProcStripButton.Image")));
            this.NewProcStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NewProcStripButton.Name = "NewProcStripButton";
            this.NewProcStripButton.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.NewProcStripButton.Size = new System.Drawing.Size(63, 22);
            this.NewProcStripButton.Text = "Create";
            this.NewProcStripButton.ToolTipText = "Create new procedure [Ctrl+P]";
            this.NewProcStripButton.Click += new System.EventHandler(this.createProcedureToolStripMenuItem_Click);
            // 
            // toolStripSeparator31
            // 
            this.toolStripSeparator31.Name = "toolStripSeparator31";
            this.toolStripSeparator31.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripSeparator31.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator32
            // 
            this.toolStripSeparator32.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator32.Name = "toolStripSeparator32";
            this.toolStripSeparator32.Size = new System.Drawing.Size(6, 25);
            // 
            // ViewArgsStripButton
            // 
            this.ViewArgsStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.ViewArgsStripButton.CheckOnClick = true;
            this.ViewArgsStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ViewArgsStripButton.Image = ((System.Drawing.Image)(resources.GetObject("ViewArgsStripButton.Image")));
            this.ViewArgsStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ViewArgsStripButton.Name = "ViewArgsStripButton";
            this.ViewArgsStripButton.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.ViewArgsStripButton.Size = new System.Drawing.Size(23, 22);
            this.ViewArgsStripButton.ToolTipText = "Show variables arguments in a procedure name.";
            this.ViewArgsStripButton.CheckedChanged += new System.EventHandler(this.ViewArgsStripButton_CheckedChanged);
            // 
            // toolStripSeparator35
            // 
            this.toolStripSeparator35.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator35.Name = "toolStripSeparator35";
            this.toolStripSeparator35.Size = new System.Drawing.Size(6, 25);
            // 
            // tabPage6
            // 
            this.tabPage6.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage6.Controls.Add(this.FunctionsTree);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.tabPage6.Size = new System.Drawing.Size(260, 628);
            this.tabPage6.TabIndex = 2;
            this.tabPage6.Text = "Functions";
            this.tabPage6.ToolTipText = "All list of macros and opcodes";
            // 
            // FunctionsTree
            // 
            this.FunctionsTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.FunctionsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FunctionsTree.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FunctionsTree.ForeColor = System.Drawing.SystemColors.WindowText;
            this.FunctionsTree.Indent = 18;
            this.FunctionsTree.ItemHeight = 16;
            this.FunctionsTree.LineColor = System.Drawing.Color.Gainsboro;
            this.FunctionsTree.Location = new System.Drawing.Point(0, 2);
            this.FunctionsTree.Name = "FunctionsTree";
            this.FunctionsTree.ShowNodeToolTips = true;
            this.FunctionsTree.Size = new System.Drawing.Size(258, 624);
            this.FunctionsTree.TabIndex = 0;
            this.FunctionsTree.TabStop = false;
            this.FunctionsTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.FunctionsTree_NodeMouseClick);
            this.FunctionsTree.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FunctionTree_MouseMove);
            // 
            // tpExplorerFiles
            // 
            this.tpExplorerFiles.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tpExplorerFiles.Controls.Add(this.treeProjectFiles);
            this.tpExplorerFiles.Controls.Add(this.toolStripSolution);
            this.tpExplorerFiles.Location = new System.Drawing.Point(4, 22);
            this.tpExplorerFiles.Name = "tpExplorerFiles";
            this.tpExplorerFiles.Padding = new System.Windows.Forms.Padding(1);
            this.tpExplorerFiles.Size = new System.Drawing.Size(260, 628);
            this.tpExplorerFiles.TabIndex = 3;
            this.tpExplorerFiles.Text = "Solution";
            this.tpExplorerFiles.ToolTipText = "Folder of project files.";
            // 
            // treeProjectFiles
            // 
            this.treeProjectFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.treeProjectFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeProjectFiles.Location = new System.Drawing.Point(1, 26);
            this.treeProjectFiles.Name = "treeProjectFiles";
            this.treeProjectFiles.Size = new System.Drawing.Size(258, 601);
            this.treeProjectFiles.TabIndex = 1;
            // 
            // toolStripSolution
            // 
            this.toolStripSolution.BackColor = System.Drawing.Color.Transparent;
            this.toolStripSolution.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripSolution.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbSetProjectFolder,
            this.toolStripSeparator57});
            this.toolStripSolution.Location = new System.Drawing.Point(1, 1);
            this.toolStripSolution.Name = "toolStripSolution";
            this.toolStripSolution.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripSolution.Size = new System.Drawing.Size(258, 25);
            this.toolStripSolution.TabIndex = 0;
            // 
            // tsbSetProjectFolder
            // 
            this.tsbSetProjectFolder.Image = ((System.Drawing.Image)(resources.GetObject("tsbSetProjectFolder.Image")));
            this.tsbSetProjectFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSetProjectFolder.Name = "tsbSetProjectFolder";
            this.tsbSetProjectFolder.Size = new System.Drawing.Size(66, 22);
            this.tsbSetProjectFolder.Text = "Project";
            this.tsbSetProjectFolder.ToolTipText = "Set project folder.";
            // 
            // toolStripSeparator57
            // 
            this.toolStripSeparator57.Name = "toolStripSeparator57";
            this.toolStripSeparator57.Size = new System.Drawing.Size(6, 25);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EmptyStripStatusLabel,
            this.LineStripStatusLabel,
            this.ColStripStatusLabel,
            this.FontSizeStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 654);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(268, 28);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 2;
            // 
            // EmptyStripStatusLabel
            // 
            this.EmptyStripStatusLabel.AutoSize = false;
            this.EmptyStripStatusLabel.Name = "EmptyStripStatusLabel";
            this.EmptyStripStatusLabel.Size = new System.Drawing.Size(5, 23);
            // 
            // LineStripStatusLabel
            // 
            this.LineStripStatusLabel.AutoSize = false;
            this.LineStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.LineStripStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.LineStripStatusLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LineStripStatusLabel.Name = "LineStripStatusLabel";
            this.LineStripStatusLabel.Size = new System.Drawing.Size(99, 23);
            this.LineStripStatusLabel.Spring = true;
            this.LineStripStatusLabel.Text = "Line:1";
            this.LineStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ColStripStatusLabel
            // 
            this.ColStripStatusLabel.AutoSize = false;
            this.ColStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.ColStripStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.ColStripStatusLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ColStripStatusLabel.Name = "ColStripStatusLabel";
            this.ColStripStatusLabel.Size = new System.Drawing.Size(99, 23);
            this.ColStripStatusLabel.Spring = true;
            this.ColStripStatusLabel.Text = "Col:1";
            this.ColStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FontSizeStripStatusLabel
            // 
            this.FontSizeStripStatusLabel.AutoSize = false;
            this.FontSizeStripStatusLabel.AutoToolTip = true;
            this.FontSizeStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.FontSizeStripStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.FontSizeStripStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.FontSizeStripStatusLabel.IsLink = true;
            this.FontSizeStripStatusLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.FontSizeStripStatusLabel.Name = "FontSizeStripStatusLabel";
            this.FontSizeStripStatusLabel.Size = new System.Drawing.Size(50, 23);
            this.FontSizeStripStatusLabel.Text = "100%";
            this.FontSizeStripStatusLabel.ToolTipText = "The text font size. Hold key Ctrl + Mouse click decrease size.";
            this.FontSizeStripStatusLabel.VisitedLinkColor = System.Drawing.Color.Blue;
            this.FontSizeStripStatusLabel.Click += new System.EventHandler(this.FontSizeStripStatusLabel_Click);
            // 
            // ToolStripMain
            // 
            this.ToolStripMain.AutoSize = false;
            this.ToolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FunctionButton,
            this.toolStripSeparator27,
            this.New_toolStripDropDownButton,
            this.toolStripSeparator7,
            this.Open_toolStripSplitButton,
            this.toolStripSeparator13,
            this.Save_toolStripSplitButton,
            this.toolStripSeparator8,
            this.tsbSaveAll,
            this.toolStripSeparator55,
            this.Outline_toolStripButton,
            this.toolStripSeparator17,
            this.Undo_toolStripButton,
            this.Redo_ToolStripButton,
            this.toolStripSeparator11,
            this.DecIndentStripButton,
            this.CommentStripButton,
            this.toolStripSeparator10,
            this.Search_toolStripButton,
            this.toolStripSeparator12,
            this.Back_toolStripButton,
            this.Forward_toolStripButton,
            this.GotoProc_StripButton,
            this.toolStripSeparator21,
            this.Edit_toolStripButton,
            this.toolStripSeparator42,
            this.Script_toolStripSplitButton,
            this.Headers_toolStripSplitButton,
            this.toolStripSeparator16,
            this.MSG_toolStripButton,
            this.toolStripSeparator9,
            this.qCompile_toolStripSplitButton,
            this.Help_toolStripButton,
            this.toolStripSeparator4,
            this.toolStripSeparator14,
            this.toolStripDropDownButton2,
            this.toolStripDropDownButton1,
            this.tsbUpdateParserData,
            this.toolStripSeparator56,
            this.tslProject});
            this.ToolStripMain.Location = new System.Drawing.Point(0, 0);
            this.ToolStripMain.Name = "ToolStripMain";
            this.ToolStripMain.Size = new System.Drawing.Size(1290, 25);
            this.ToolStripMain.TabIndex = 2;
            this.ToolStripMain.Text = "toolStrip2";
            // 
            // FunctionButton
            // 
            this.FunctionButton.CheckOnClick = true;
            this.FunctionButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.FunctionButton.Image = ((System.Drawing.Image)(resources.GetObject("FunctionButton.Image")));
            this.FunctionButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FunctionButton.Name = "FunctionButton";
            this.FunctionButton.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.FunctionButton.Size = new System.Drawing.Size(23, 22);
            this.FunctionButton.Text = "Function Tree";
            this.FunctionButton.ToolTipText = "Show/Hide function tree [Alt+E]";
            this.FunctionButton.Click += new System.EventHandler(this.FunctionButton_Click);
            // 
            // toolStripSeparator27
            // 
            this.toolStripSeparator27.Name = "toolStripSeparator27";
            this.toolStripSeparator27.Size = new System.Drawing.Size(6, 25);
            // 
            // New_toolStripDropDownButton
            // 
            this.New_toolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem});
            this.New_toolStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("New_toolStripDropDownButton.Image")));
            this.New_toolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.New_toolStripDropDownButton.Name = "New_toolStripDropDownButton";
            this.New_toolStripDropDownButton.Size = new System.Drawing.Size(64, 22);
            this.New_toolStripDropDownButton.Text = "New";
            this.New_toolStripDropDownButton.ToolTipText = "Create new script [Ctrl+N]";
            this.New_toolStripDropDownButton.ButtonClick += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.newToolStripMenuItem.Text = "New Script";
            this.newToolStripMenuItem.ToolTipText = "Create new empty script file.";
            this.newToolStripMenuItem.Visible = false;
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.AutoSize = false;
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(8, 25);
            // 
            // Open_toolStripSplitButton
            // 
            this.Open_toolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.recentToolStripMenuItem,
            this.toolStripSeparator18});
            this.Open_toolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("Open_toolStripSplitButton.Image")));
            this.Open_toolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Open_toolStripSplitButton.Name = "Open_toolStripSplitButton";
            this.Open_toolStripSplitButton.Size = new System.Drawing.Size(69, 22);
            this.Open_toolStripSplitButton.Text = "Open";
            this.Open_toolStripSplitButton.ToolTipText = "Open script [Ctrl+O]";
            this.Open_toolStripSplitButton.ButtonClick += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Visible = false;
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // recentToolStripMenuItem
            // 
            this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            this.recentToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.recentToolStripMenuItem.Text = "Clear recent files";
            this.recentToolStripMenuItem.Click += new System.EventHandler(this.recentToolStripMenuItem_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(161, 6);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.AutoSize = false;
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(8, 25);
            // 
            // Save_toolStripSplitButton
            // 
            this.Save_toolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Save_ToolStripMenuItem,
            this.SaveAll_ToolStripMenuItem,
            this.toolStripSeparator15,
            this.SaveAs_ToolStripMenuItem,
            this.saveAsTemplateToolStripMenuItem,
            this.toolStripSeparator48,
            this.saveUTF8ToolStripMenuItem});
            this.Save_toolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("Save_toolStripSplitButton.Image")));
            this.Save_toolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Save_toolStripSplitButton.Name = "Save_toolStripSplitButton";
            this.Save_toolStripSplitButton.Size = new System.Drawing.Size(65, 22);
            this.Save_toolStripSplitButton.Text = "Save";
            this.Save_toolStripSplitButton.ToolTipText = "Save current script [Ctrl+S]";
            this.Save_toolStripSplitButton.ButtonClick += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // Save_ToolStripMenuItem
            // 
            this.Save_ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("Save_ToolStripMenuItem.Image")));
            this.Save_ToolStripMenuItem.Name = "Save_ToolStripMenuItem";
            this.Save_ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.Save_ToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Save_ToolStripMenuItem.Text = "Save";
            this.Save_ToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // SaveAll_ToolStripMenuItem
            // 
            this.SaveAll_ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("SaveAll_ToolStripMenuItem.Image")));
            this.SaveAll_ToolStripMenuItem.Name = "SaveAll_ToolStripMenuItem";
            this.SaveAll_ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.SaveAll_ToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.SaveAll_ToolStripMenuItem.Text = "Save All";
            this.SaveAll_ToolStripMenuItem.Click += new System.EventHandler(this.saveAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(186, 6);
            // 
            // SaveAs_ToolStripMenuItem
            // 
            this.SaveAs_ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("SaveAs_ToolStripMenuItem.Image")));
            this.SaveAs_ToolStripMenuItem.Name = "SaveAs_ToolStripMenuItem";
            this.SaveAs_ToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.SaveAs_ToolStripMenuItem.Text = "Save as...";
            this.SaveAs_ToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // saveAsTemplateToolStripMenuItem
            // 
            this.saveAsTemplateToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveAsTemplateToolStripMenuItem.Image")));
            this.saveAsTemplateToolStripMenuItem.Name = "saveAsTemplateToolStripMenuItem";
            this.saveAsTemplateToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.saveAsTemplateToolStripMenuItem.Text = "Save as Template";
            this.saveAsTemplateToolStripMenuItem.Click += new System.EventHandler(this.saveAsTemplateToolStripMenuItem_Click);
            // 
            // toolStripSeparator48
            // 
            this.toolStripSeparator48.Name = "toolStripSeparator48";
            this.toolStripSeparator48.Size = new System.Drawing.Size(186, 6);
            // 
            // saveUTF8ToolStripMenuItem
            // 
            this.saveUTF8ToolStripMenuItem.CheckOnClick = true;
            this.saveUTF8ToolStripMenuItem.Name = "saveUTF8ToolStripMenuItem";
            this.saveUTF8ToolStripMenuItem.ShowShortcutKeys = false;
            this.saveUTF8ToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.saveUTF8ToolStripMenuItem.Text = "Save in UTF-8";
            this.saveUTF8ToolStripMenuItem.ToolTipText = "Always save the script file in UTF-8 encoding, otherwise used the Windows ANSI en" +
    "coding page.";
            this.saveUTF8ToolStripMenuItem.Click += new System.EventHandler(this.saveUTF8ToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.AutoSize = false;
            this.toolStripSeparator8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(8, 25);
            // 
            // tsbSaveAll
            // 
            this.tsbSaveAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSaveAll.Image = ((System.Drawing.Image)(resources.GetObject("tsbSaveAll.Image")));
            this.tsbSaveAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSaveAll.Name = "tsbSaveAll";
            this.tsbSaveAll.Size = new System.Drawing.Size(23, 22);
            this.tsbSaveAll.Text = "Save All";
            this.tsbSaveAll.ToolTipText = "Save all scrips [Ctrl+Shift+S]";
            this.tsbSaveAll.Click += new System.EventHandler(this.saveAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator55
            // 
            this.toolStripSeparator55.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.toolStripSeparator55.Name = "toolStripSeparator55";
            this.toolStripSeparator55.Size = new System.Drawing.Size(6, 25);
            // 
            // Outline_toolStripButton
            // 
            this.Outline_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Outline_toolStripButton.Enabled = false;
            this.Outline_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Outline_toolStripButton.Image")));
            this.Outline_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Outline_toolStripButton.Name = "Outline_toolStripButton";
            this.Outline_toolStripButton.Size = new System.Drawing.Size(23, 22);
            this.Outline_toolStripButton.ToolTipText = "Folding Expand/Collapse";
            this.Outline_toolStripButton.Click += new System.EventHandler(this.outlineToolStripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(6, 25);
            // 
            // Undo_toolStripButton
            // 
            this.Undo_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Undo_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Undo_toolStripButton.Image")));
            this.Undo_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Undo_toolStripButton.Name = "Undo_toolStripButton";
            this.Undo_toolStripButton.Size = new System.Drawing.Size(23, 22);
            this.Undo_toolStripButton.Text = "Undo";
            this.Undo_toolStripButton.ToolTipText = "Undo [Ctrl+Z]";
            this.Undo_toolStripButton.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // Redo_ToolStripButton
            // 
            this.Redo_ToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Redo_ToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Redo_ToolStripButton.Image")));
            this.Redo_ToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Redo_ToolStripButton.Name = "Redo_ToolStripButton";
            this.Redo_ToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.Redo_ToolStripButton.Text = "Redo";
            this.Redo_ToolStripButton.ToolTipText = "Redo [Ctrl+Y]";
            this.Redo_ToolStripButton.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 25);
            // 
            // DecIndentStripButton
            // 
            this.DecIndentStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.DecIndentStripButton.Enabled = false;
            this.DecIndentStripButton.Image = ((System.Drawing.Image)(resources.GetObject("DecIndentStripButton.Image")));
            this.DecIndentStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DecIndentStripButton.Name = "DecIndentStripButton";
            this.DecIndentStripButton.Size = new System.Drawing.Size(23, 22);
            this.DecIndentStripButton.ToolTipText = "Decrease text indent [Alt+Q]\r\nDecrease indent of selected text with left alignmen" +
    "t [Shift-Tab]\r\n(use Tab key for increase text indent)";
            this.DecIndentStripButton.Click += new System.EventHandler(this.DecIndentStripButton_Click);
            // 
            // CommentStripButton
            // 
            this.CommentStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CommentStripButton.Enabled = false;
            this.CommentStripButton.Image = ((System.Drawing.Image)(resources.GetObject("CommentStripButton.Image")));
            this.CommentStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CommentStripButton.Name = "CommentStripButton";
            this.CommentStripButton.Size = new System.Drawing.Size(23, 22);
            this.CommentStripButton.ToolTipText = "Comment/Uncomment selected block text [Ctrl+Devide]";
            this.CommentStripButton.Click += new System.EventHandler(this.CommentStripButton_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 25);
            // 
            // Search_toolStripButton
            // 
            this.Search_toolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchToolStripMenuItem,
            this.findNextToolStripMenuItem,
            this.findPreviousToolStripMenuItem,
            this.toolStripSeparator33,
            this.quickFindToolStripMenuItem});
            this.Search_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Search_toolStripButton.Image")));
            this.Search_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Search_toolStripButton.Name = "Search_toolStripButton";
            this.Search_toolStripButton.Size = new System.Drawing.Size(61, 22);
            this.Search_toolStripButton.Text = "Find";
            this.Search_toolStripButton.ToolTipText = "Search & Replace";
            this.Search_toolStripButton.ButtonClick += new System.EventHandler(this.Search_Panel);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("searchToolStripMenuItem.Image")));
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.searchToolStripMenuItem.Text = "Advanced Search";
            this.searchToolStripMenuItem.ToolTipText = "Advanced Search and Replace.";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // findNextToolStripMenuItem
            // 
            this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
            this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.findNextToolStripMenuItem.Text = "Find Next";
            this.findNextToolStripMenuItem.Click += new System.EventHandler(this.FindForwardButton_Click);
            // 
            // findPreviousToolStripMenuItem
            // 
            this.findPreviousToolStripMenuItem.Name = "findPreviousToolStripMenuItem";
            this.findPreviousToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.findPreviousToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.findPreviousToolStripMenuItem.Text = "Find Previous";
            this.findPreviousToolStripMenuItem.Click += new System.EventHandler(this.FindBackButton_Click);
            // 
            // toolStripSeparator33
            // 
            this.toolStripSeparator33.Name = "toolStripSeparator33";
            this.toolStripSeparator33.Size = new System.Drawing.Size(205, 6);
            // 
            // quickFindToolStripMenuItem
            // 
            this.quickFindToolStripMenuItem.Name = "quickFindToolStripMenuItem";
            this.quickFindToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
            this.quickFindToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.quickFindToolStripMenuItem.Text = "Quick Find";
            this.quickFindToolStripMenuItem.ToolTipText = "Find word under the cursor.";
            this.quickFindToolStripMenuItem.Click += new System.EventHandler(this.quickFindToolStripMenuItem_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 25);
            // 
            // Back_toolStripButton
            // 
            this.Back_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Back_toolStripButton.Enabled = false;
            this.Back_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Back_toolStripButton.Image")));
            this.Back_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Back_toolStripButton.Name = "Back_toolStripButton";
            this.Back_toolStripButton.Size = new System.Drawing.Size(23, 22);
            this.Back_toolStripButton.Text = "Back";
            this.Back_toolStripButton.ToolTipText = "Navigation Back ";
            this.Back_toolStripButton.Click += new System.EventHandler(this.Back_toolStripButton_Click);
            // 
            // Forward_toolStripButton
            // 
            this.Forward_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Forward_toolStripButton.Enabled = false;
            this.Forward_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Forward_toolStripButton.Image")));
            this.Forward_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Forward_toolStripButton.Name = "Forward_toolStripButton";
            this.Forward_toolStripButton.Size = new System.Drawing.Size(23, 22);
            this.Forward_toolStripButton.Text = "Forward";
            this.Forward_toolStripButton.ToolTipText = "Navigation Forward ";
            this.Forward_toolStripButton.Click += new System.EventHandler(this.Forward_toolStripButton_Click);
            // 
            // GotoProc_StripButton
            // 
            this.GotoProc_StripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.GotoProc_StripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gotoToLineToolStripMenuItem});
            this.GotoProc_StripButton.Enabled = false;
            this.GotoProc_StripButton.Image = ((System.Drawing.Image)(resources.GetObject("GotoProc_StripButton.Image")));
            this.GotoProc_StripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.GotoProc_StripButton.Name = "GotoProc_StripButton";
            this.GotoProc_StripButton.Size = new System.Drawing.Size(32, 22);
            this.GotoProc_StripButton.Tag = "Button";
            this.GotoProc_StripButton.Text = "Goto Procedure";
            this.GotoProc_StripButton.ToolTipText = "Goto procedure under cursor[Alt+P]";
            this.GotoProc_StripButton.ButtonClick += new System.EventHandler(this.findDefinitionToolStripMenuItem_Click);
            // 
            // gotoToLineToolStripMenuItem
            // 
            this.gotoToLineToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("gotoToLineToolStripMenuItem.Image")));
            this.gotoToLineToolStripMenuItem.Name = "gotoToLineToolStripMenuItem";
            this.gotoToLineToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.gotoToLineToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.gotoToLineToolStripMenuItem.Text = "Goto Line";
            this.gotoToLineToolStripMenuItem.ToolTipText = "Goto line document";
            this.gotoToLineToolStripMenuItem.Click += new System.EventHandler(this.GoToLineToolStripMenuItemClick);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            this.toolStripSeparator21.Size = new System.Drawing.Size(6, 25);
            // 
            // Edit_toolStripButton
            // 
            this.Edit_toolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.splitDocumentToolStripMenuItem,
            this.toolStripSeparator1,
            this.ToggleBlockCommentToolStripMenuItem,
            this.capitalizeCaseToolStripMenuItem,
            this.toolStripSeparator40,
            this.leadingTabsSpacesToolStripMenuItem,
            this.allTabsSpacesToolStripMenuItem,
            this.toolStripSeparator39,
            this.trailingSpacesToolStripMenuItem,
            this.formatCodeToolStripMenuItem,
            this.toolStripSeparator45,
            this.tsmMessageTextChecker,
            this.showAutocompleteWordToolStripMenuItem,
            this.toolStripSeparator41,
            this.showTabsAndSpacesToolStripMenuItem});
            this.Edit_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Edit_toolStripButton.Image")));
            this.Edit_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Edit_toolStripButton.Name = "Edit_toolStripButton";
            this.Edit_toolStripButton.Size = new System.Drawing.Size(64, 22);
            this.Edit_toolStripButton.Text = "Code";
            this.Edit_toolStripButton.ToolTipText = "Code text tool";
            // 
            // splitDocumentToolStripMenuItem
            // 
            this.splitDocumentToolStripMenuItem.Enabled = false;
            this.splitDocumentToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("splitDocumentToolStripMenuItem.Image")));
            this.splitDocumentToolStripMenuItem.Name = "splitDocumentToolStripMenuItem";
            this.splitDocumentToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.splitDocumentToolStripMenuItem.Size = new System.Drawing.Size(278, 22);
            this.splitDocumentToolStripMenuItem.Text = "Split Document";
            this.splitDocumentToolStripMenuItem.ToolTipText = "Split document viewer";
            this.splitDocumentToolStripMenuItem.Click += new System.EventHandler(this.SplitDoc_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(275, 6);
            // 
            // ToggleBlockCommentToolStripMenuItem
            // 
            this.ToggleBlockCommentToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ToggleBlockCommentToolStripMenuItem.Image")));
            this.ToggleBlockCommentToolStripMenuItem.Name = "ToggleBlockCommentToolStripMenuItem";
            this.ToggleBlockCommentToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Multiply";
            this.ToggleBlockCommentToolStripMenuItem.Size = new System.Drawing.Size(278, 22);
            this.ToggleBlockCommentToolStripMenuItem.Text = "Region Comment";
            this.ToggleBlockCommentToolStripMenuItem.ToolTipText = "Toggle selected text region comment.";
            this.ToggleBlockCommentToolStripMenuItem.Click += new System.EventHandler(this.ToggleBlockCommentToolStripMenuItem_Click);
            // 
            // capitalizeCaseToolStripMenuItem
            // 
            this.capitalizeCaseToolStripMenuItem.Name = "capitalizeCaseToolStripMenuItem";
            this.capitalizeCaseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.W)));
            this.capitalizeCaseToolStripMenuItem.Size = new System.Drawing.Size(278, 22);
            this.capitalizeCaseToolStripMenuItem.Text = "Capitalize Word Case";
            this.capitalizeCaseToolStripMenuItem.Click += new System.EventHandler(this.capitalizeCaseToolStripMenuItem_Click);
            // 
            // toolStripSeparator40
            // 
            this.toolStripSeparator40.Name = "toolStripSeparator40";
            this.toolStripSeparator40.Size = new System.Drawing.Size(275, 6);
            // 
            // leadingTabsSpacesToolStripMenuItem
            // 
            this.leadingTabsSpacesToolStripMenuItem.Name = "leadingTabsSpacesToolStripMenuItem";
            this.leadingTabsSpacesToolStripMenuItem.Size = new System.Drawing.Size(278, 22);
            this.leadingTabsSpacesToolStripMenuItem.Text = "Convert leading Tabs to Spaces";
            this.leadingTabsSpacesToolStripMenuItem.Click += new System.EventHandler(this.leadingTabsSpacesToolStripMenuItem_Click);
            // 
            // allTabsSpacesToolStripMenuItem
            // 
            this.allTabsSpacesToolStripMenuItem.Name = "allTabsSpacesToolStripMenuItem";
            this.allTabsSpacesToolStripMenuItem.Size = new System.Drawing.Size(278, 22);
            this.allTabsSpacesToolStripMenuItem.Text = "Convert all Tabs to Spaces";
            this.allTabsSpacesToolStripMenuItem.Click += new System.EventHandler(this.allTabsSpacesToolStripMenuItem_Click);
            // 
            // toolStripSeparator39
            // 
            this.toolStripSeparator39.Name = "toolStripSeparator39";
            this.toolStripSeparator39.Size = new System.Drawing.Size(275, 6);
            // 
            // trailingSpacesToolStripMenuItem
            // 
            this.trailingSpacesToolStripMenuItem.CheckOnClick = true;
            this.trailingSpacesToolStripMenuItem.Name = "trailingSpacesToolStripMenuItem";
            this.trailingSpacesToolStripMenuItem.Size = new System.Drawing.Size(278, 22);
            this.trailingSpacesToolStripMenuItem.Text = "Auto Trailing Spaces/Tabs";
            this.trailingSpacesToolStripMenuItem.ToolTipText = "Automatically remove the spaces and tabs at the end of each line when you save th" +
    "e document.\r\nNote: This will not work only for message files (.msg).";
            this.trailingSpacesToolStripMenuItem.Click += new System.EventHandler(this.trailingSpacesToolStripMenuItem_Click);
            // 
            // formatCodeToolStripMenuItem
            // 
            this.formatCodeToolStripMenuItem.CheckOnClick = true;
            this.formatCodeToolStripMenuItem.Name = "formatCodeToolStripMenuItem";
            this.formatCodeToolStripMenuItem.Size = new System.Drawing.Size(278, 22);
            this.formatCodeToolStripMenuItem.Text = "Auto Formatting Code";
            this.formatCodeToolStripMenuItem.ToolTipText = resources.GetString("formatCodeToolStripMenuItem.ToolTipText");
            // 
            // toolStripSeparator45
            // 
            this.toolStripSeparator45.Name = "toolStripSeparator45";
            this.toolStripSeparator45.Size = new System.Drawing.Size(275, 6);
            // 
            // tsmMessageTextChecker
            // 
            this.tsmMessageTextChecker.Enabled = false;
            this.tsmMessageTextChecker.Name = "tsmMessageTextChecker";
            this.tsmMessageTextChecker.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.M)));
            this.tsmMessageTextChecker.Size = new System.Drawing.Size(278, 22);
            this.tsmMessageTextChecker.Text = "Check message text structure";
            this.tsmMessageTextChecker.ToolTipText = "Checks the current open message file, for errors in the parentheses structure.";
            this.tsmMessageTextChecker.Click += new System.EventHandler(this.tsmMessageTextChecker_Click);
            // 
            // showAutocompleteWordToolStripMenuItem
            // 
            this.showAutocompleteWordToolStripMenuItem.Name = "showAutocompleteWordToolStripMenuItem";
            this.showAutocompleteWordToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Space)));
            this.showAutocompleteWordToolStripMenuItem.Size = new System.Drawing.Size(278, 22);
            this.showAutocompleteWordToolStripMenuItem.Text = "Autocomplete Word";
            this.showAutocompleteWordToolStripMenuItem.Click += new System.EventHandler(this.showAutocompleteWordToolStripMenuItem_Click);
            // 
            // toolStripSeparator41
            // 
            this.toolStripSeparator41.Name = "toolStripSeparator41";
            this.toolStripSeparator41.Size = new System.Drawing.Size(275, 6);
            // 
            // showTabsAndSpacesToolStripMenuItem
            // 
            this.showTabsAndSpacesToolStripMenuItem.CheckOnClick = true;
            this.showTabsAndSpacesToolStripMenuItem.Name = "showTabsAndSpacesToolStripMenuItem";
            this.showTabsAndSpacesToolStripMenuItem.Size = new System.Drawing.Size(278, 22);
            this.showTabsAndSpacesToolStripMenuItem.Text = "Show Tabs and Spaces";
            this.showTabsAndSpacesToolStripMenuItem.Click += new System.EventHandler(this.showTabsAndSpacesToolStripMenuItem_Click);
            // 
            // toolStripSeparator42
            // 
            this.toolStripSeparator42.Name = "toolStripSeparator42";
            this.toolStripSeparator42.Size = new System.Drawing.Size(6, 25);
            // 
            // Script_toolStripSplitButton
            // 
            this.Script_toolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editRegisteredScriptsToolStripMenuItem,
            this.toolStripSeparator26,
            this.defineToolStripMenuItem});
            this.Script_toolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("Script_toolStripSplitButton.Image")));
            this.Script_toolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Script_toolStripSplitButton.Name = "Script_toolStripSplitButton";
            this.Script_toolStripSplitButton.Size = new System.Drawing.Size(57, 22);
            this.Script_toolStripSplitButton.Text = "List";
            this.Script_toolStripSplitButton.ToolTipText = "Register current script to script.lst.";
            this.Script_toolStripSplitButton.ButtonClick += new System.EventHandler(this.registerScriptToolStripMenuItem_Click);
            // 
            // editRegisteredScriptsToolStripMenuItem
            // 
            this.editRegisteredScriptsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editRegisteredScriptsToolStripMenuItem.Image")));
            this.editRegisteredScriptsToolStripMenuItem.Name = "editRegisteredScriptsToolStripMenuItem";
            this.editRegisteredScriptsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.editRegisteredScriptsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.editRegisteredScriptsToolStripMenuItem.Text = "Scripts List Editor";
            this.editRegisteredScriptsToolStripMenuItem.ToolTipText = "Open script registered editor.";
            this.editRegisteredScriptsToolStripMenuItem.Click += new System.EventHandler(this.editRegisteredScriptsToolStripMenuItem_Click);
            // 
            // toolStripSeparator26
            // 
            this.toolStripSeparator26.Name = "toolStripSeparator26";
            this.toolStripSeparator26.Size = new System.Drawing.Size(185, 6);
            // 
            // defineToolStripMenuItem
            // 
            this.defineToolStripMenuItem.CheckOnClick = true;
            this.defineToolStripMenuItem.Name = "defineToolStripMenuItem";
            this.defineToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.defineToolStripMenuItem.Text = "Define to Scripts.h";
            this.defineToolStripMenuItem.ToolTipText = "Also register definitions for script in the file \"Scripts.h\".";
            this.defineToolStripMenuItem.Click += new System.EventHandler(this.defineToolStripMenuItem_Click);
            // 
            // Headers_toolStripSplitButton
            // 
            this.Headers_toolStripSplitButton.DropDownButtonWidth = 12;
            this.Headers_toolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.includeFileToCodeToolStripMenuItem,
            this.openAllIncludesScriptToolStripMenuItem,
            this.toolStripSeparator50,
            this.openHeaderFileToolStripMenuItem});
            this.Headers_toolStripSplitButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Headers_toolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("Headers_toolStripSplitButton.Image")));
            this.Headers_toolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Headers_toolStripSplitButton.MergeAction = System.Windows.Forms.MergeAction.Remove;
            this.Headers_toolStripSplitButton.Name = "Headers_toolStripSplitButton";
            this.Headers_toolStripSplitButton.Size = new System.Drawing.Size(75, 22);
            this.Headers_toolStripSplitButton.Text = "Include";
            this.Headers_toolStripSplitButton.ToolTipText = "Quick open include header files.";
            this.Headers_toolStripSplitButton.ButtonClick += new System.EventHandler(this.Headers_toolStripSplitButton_ButtonClick);
            // 
            // includeFileToCodeToolStripMenuItem
            // 
            this.includeFileToCodeToolStripMenuItem.Enabled = false;
            this.includeFileToCodeToolStripMenuItem.Name = "includeFileToCodeToolStripMenuItem";
            this.includeFileToCodeToolStripMenuItem.ShowShortcutKeys = false;
            this.includeFileToCodeToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.includeFileToCodeToolStripMenuItem.Text = "Include file to Code";
            this.includeFileToCodeToolStripMenuItem.Click += new System.EventHandler(this.includeFileToCodeToolStripMenuItem_Click);
            // 
            // openAllIncludesScriptToolStripMenuItem
            // 
            this.openAllIncludesScriptToolStripMenuItem.Enabled = false;
            this.openAllIncludesScriptToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openAllIncludesScriptToolStripMenuItem.Image")));
            this.openAllIncludesScriptToolStripMenuItem.Name = "openAllIncludesScriptToolStripMenuItem";
            this.openAllIncludesScriptToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.G)));
            this.openAllIncludesScriptToolStripMenuItem.ShowShortcutKeys = false;
            this.openAllIncludesScriptToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.openAllIncludesScriptToolStripMenuItem.Text = "Open all include files";
            this.openAllIncludesScriptToolStripMenuItem.ToolTipText = "Open all include file in this script. [Alt+Shift+G]";
            this.openAllIncludesScriptToolStripMenuItem.Click += new System.EventHandler(this.openIncludesScriptToolStripMenuItem_Click);
            // 
            // toolStripSeparator50
            // 
            this.toolStripSeparator50.Name = "toolStripSeparator50";
            this.toolStripSeparator50.Size = new System.Drawing.Size(161, 6);
            // 
            // openHeaderFileToolStripMenuItem
            // 
            this.openHeaderFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openHeaderFileToolStripMenuItem.Image")));
            this.openHeaderFileToolStripMenuItem.Name = "openHeaderFileToolStripMenuItem";
            this.openHeaderFileToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.openHeaderFileToolStripMenuItem.Text = "Open Header file";
            this.openHeaderFileToolStripMenuItem.Click += new System.EventHandler(this.openHeaderFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(6, 25);
            // 
            // MSG_toolStripButton
            // 
            this.MSG_toolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.msgFileEditorToolStripMenuItem,
            this.dialogNodesDiagramToolStripMenuItem,
            this.previewDialogToolStripMenuItem,
            this.toolStripSeparator24,
            this.dialogFunctionConfigToolStripMenuItem,
            this.toolStripSeparator53,
            this.msgAutoOpenEditorStripMenuItem});
            this.MSG_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("MSG_toolStripButton.Image")));
            this.MSG_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MSG_toolStripButton.Name = "MSG_toolStripButton";
            this.MSG_toolStripButton.Size = new System.Drawing.Size(71, 22);
            this.MSG_toolStripButton.Text = "Dialog";
            this.MSG_toolStripButton.ToolTipText = "Open associate MSG file";
            this.MSG_toolStripButton.ButtonClick += new System.EventHandler(this.associateMsgToolStripMenuItem_Click);
            // 
            // msgFileEditorToolStripMenuItem
            // 
            this.msgFileEditorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("msgFileEditorToolStripMenuItem.Image")));
            this.msgFileEditorToolStripMenuItem.Name = "msgFileEditorToolStripMenuItem";
            this.msgFileEditorToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.msgFileEditorToolStripMenuItem.Text = "Message file editor";
            this.msgFileEditorToolStripMenuItem.Click += new System.EventHandler(this.msgFileEditorToolStripMenuItem_Click);
            // 
            // dialogNodesDiagramToolStripMenuItem
            // 
            this.dialogNodesDiagramToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("dialogNodesDiagramToolStripMenuItem.Image")));
            this.dialogNodesDiagramToolStripMenuItem.Name = "dialogNodesDiagramToolStripMenuItem";
            this.dialogNodesDiagramToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.dialogNodesDiagramToolStripMenuItem.Text = "Nodes Flowchart ";
            this.dialogNodesDiagramToolStripMenuItem.Click += new System.EventHandler(this.dialogNodesDiagramToolStripMenuItem_Click);
            // 
            // previewDialogToolStripMenuItem
            // 
            this.previewDialogToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("previewDialogToolStripMenuItem.Image")));
            this.previewDialogToolStripMenuItem.Name = "previewDialogToolStripMenuItem";
            this.previewDialogToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.previewDialogToolStripMenuItem.Text = "Preview and testing";
            this.previewDialogToolStripMenuItem.Click += new System.EventHandler(this.previewDialogToolStripMenuItem_Click);
            // 
            // toolStripSeparator24
            // 
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            this.toolStripSeparator24.Size = new System.Drawing.Size(194, 6);
            // 
            // dialogFunctionConfigToolStripMenuItem
            // 
            this.dialogFunctionConfigToolStripMenuItem.Name = "dialogFunctionConfigToolStripMenuItem";
            this.dialogFunctionConfigToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.dialogFunctionConfigToolStripMenuItem.Text = "Functions Config";
            this.dialogFunctionConfigToolStripMenuItem.Click += new System.EventHandler(this.dialogFunctionConfigToolStripMenuItem_Click);
            // 
            // toolStripSeparator53
            // 
            this.toolStripSeparator53.Name = "toolStripSeparator53";
            this.toolStripSeparator53.Size = new System.Drawing.Size(194, 6);
            // 
            // msgAutoOpenEditorStripMenuItem
            // 
            this.msgAutoOpenEditorStripMenuItem.Checked = true;
            this.msgAutoOpenEditorStripMenuItem.CheckOnClick = true;
            this.msgAutoOpenEditorStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.msgAutoOpenEditorStripMenuItem.Name = "msgAutoOpenEditorStripMenuItem";
            this.msgAutoOpenEditorStripMenuItem.ShowShortcutKeys = false;
            this.msgAutoOpenEditorStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.msgAutoOpenEditorStripMenuItem.Text = "Open in message editor";
            this.msgAutoOpenEditorStripMenuItem.ToolTipText = "Open associated MSG files in the message editor, instead of a text document.";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            // 
            // qCompile_toolStripSplitButton
            // 
            this.qCompile_toolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Compile_ToolStripMenuItem,
            this.CompileAllOpen_ToolStripMenuItem,
            this.toolStripSeparator34,
            this.MassCompile_ToolStripMenuItem,
            this.toolStripSeparator3,
            this.Preprocess_ToolStripMenuItem,
            this.roundtripToolStripMenuItem,
            this.toolStripSeparator25,
            this.decompileF1ToolStripMenuItem,
            this.oldDecompileToolStripMenuItem,
            this.toolStripSeparator49,
            this.pDefineStripComboBox});
            this.qCompile_toolStripSplitButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.qCompile_toolStripSplitButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.qCompile_toolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("qCompile_toolStripSplitButton.Image")));
            this.qCompile_toolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.qCompile_toolStripSplitButton.Name = "qCompile_toolStripSplitButton";
            this.qCompile_toolStripSplitButton.Size = new System.Drawing.Size(84, 22);
            this.qCompile_toolStripSplitButton.Text = "Compile";
            this.qCompile_toolStripSplitButton.ToolTipText = "Quick Compile Script [F8]";
            this.qCompile_toolStripSplitButton.ButtonClick += new System.EventHandler(this.compileToolStripMenuItem1_Click);
            // 
            // Compile_ToolStripMenuItem
            // 
            this.Compile_ToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Compile_ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("Compile_ToolStripMenuItem.Image")));
            this.Compile_ToolStripMenuItem.Name = "Compile_ToolStripMenuItem";
            this.Compile_ToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.Compile_ToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.Compile_ToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.Compile_ToolStripMenuItem.Text = "Compile Script";
            this.Compile_ToolStripMenuItem.Click += new System.EventHandler(this.compileToolStripMenuItem1_Click);
            // 
            // CompileAllOpen_ToolStripMenuItem
            // 
            this.CompileAllOpen_ToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CompileAllOpen_ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("CompileAllOpen_ToolStripMenuItem.Image")));
            this.CompileAllOpen_ToolStripMenuItem.Name = "CompileAllOpen_ToolStripMenuItem";
            this.CompileAllOpen_ToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.CompileAllOpen_ToolStripMenuItem.Text = "Compile All Open";
            this.CompileAllOpen_ToolStripMenuItem.Click += new System.EventHandler(this.compileAllOpenToolStripMenuItem_Click);
            // 
            // toolStripSeparator34
            // 
            this.toolStripSeparator34.Name = "toolStripSeparator34";
            this.toolStripSeparator34.Size = new System.Drawing.Size(185, 6);
            // 
            // MassCompile_ToolStripMenuItem
            // 
            this.MassCompile_ToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MassCompile_ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("MassCompile_ToolStripMenuItem.Image")));
            this.MassCompile_ToolStripMenuItem.Name = "MassCompile_ToolStripMenuItem";
            this.MassCompile_ToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.MassCompile_ToolStripMenuItem.Text = "Mass Compile";
            this.MassCompile_ToolStripMenuItem.Click += new System.EventHandler(this.massCompileToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(185, 6);
            // 
            // Preprocess_ToolStripMenuItem
            // 
            this.Preprocess_ToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Preprocess_ToolStripMenuItem.Name = "Preprocess_ToolStripMenuItem";
            this.Preprocess_ToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.Preprocess_ToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.Preprocess_ToolStripMenuItem.Text = "Preprocess";
            this.Preprocess_ToolStripMenuItem.ToolTipText = "Open script file after preprocessor.";
            this.Preprocess_ToolStripMenuItem.Click += new System.EventHandler(this.preprocessToolStripMenuItem_Click);
            // 
            // roundtripToolStripMenuItem
            // 
            this.roundtripToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.roundtripToolStripMenuItem.Name = "roundtripToolStripMenuItem";
            this.roundtripToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.roundtripToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.roundtripToolStripMenuItem.Text = "Roundtrip";
            this.roundtripToolStripMenuItem.ToolTipText = "Compile script into output folder and open decompiled script file.";
            this.roundtripToolStripMenuItem.Click += new System.EventHandler(this.roundtripToolStripMenuItem_Click);
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            this.toolStripSeparator25.Size = new System.Drawing.Size(185, 6);
            // 
            // decompileF1ToolStripMenuItem
            // 
            this.decompileF1ToolStripMenuItem.CheckOnClick = true;
            this.decompileF1ToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.decompileF1ToolStripMenuItem.Name = "decompileF1ToolStripMenuItem";
            this.decompileF1ToolStripMenuItem.ShowShortcutKeys = false;
            this.decompileF1ToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.decompileF1ToolStripMenuItem.Text = "Decompile mode Fallout 1";
            this.decompileF1ToolStripMenuItem.ToolTipText = "Forced to decompile scripts of the format Fallout 1.\r\n(used option -1 for int2ssl" +
    ".exe)";
            this.decompileF1ToolStripMenuItem.Click += new System.EventHandler(this.decompileF1ToolStripMenuItem_Click);
            // 
            // oldDecompileToolStripMenuItem
            // 
            this.oldDecompileToolStripMenuItem.CheckOnClick = true;
            this.oldDecompileToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.oldDecompileToolStripMenuItem.Name = "oldDecompileToolStripMenuItem";
            this.oldDecompileToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.oldDecompileToolStripMenuItem.Text = "Use old decompiler";
            this.oldDecompileToolStripMenuItem.ToolTipText = "When decompiling a script, forced used the earlier version of the decompiler (int" +
    "2ssl v3.5).\r\nThis option allows to solve some problems that are present in the l" +
    "ater versions of the decompiler.";
            this.oldDecompileToolStripMenuItem.Click += new System.EventHandler(this.oldDecompileToolStripMenuItem_Click);
            // 
            // toolStripSeparator49
            // 
            this.toolStripSeparator49.Name = "toolStripSeparator49";
            this.toolStripSeparator49.Size = new System.Drawing.Size(185, 6);
            // 
            // pDefineStripComboBox
            // 
            this.pDefineStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pDefineStripComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.pDefineStripComboBox.Items.AddRange(new object[] {
            "<Not Preset>",
            "DEBUG",
            "RELEASE"});
            this.pDefineStripComboBox.Name = "pDefineStripComboBox";
            this.pDefineStripComboBox.Size = new System.Drawing.Size(121, 22);
            this.pDefineStripComboBox.ToolTipText = "Definition of conditional compilation constant for preprocessor #if directives.\r\n" +
    "You can add custom defines to file PreprocDefine.ini";
            this.pDefineStripComboBox.SelectedIndexChanged += new System.EventHandler(this.pDefineStripComboBox_SelectedIndexChanged);
            // 
            // Help_toolStripButton
            // 
            this.Help_toolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Help_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Help_toolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.About_toolStripButton});
            this.Help_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Help_toolStripButton.Image")));
            this.Help_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Help_toolStripButton.MergeIndex = 1;
            this.Help_toolStripButton.Name = "Help_toolStripButton";
            this.Help_toolStripButton.Size = new System.Drawing.Size(32, 22);
            this.Help_toolStripButton.Text = "Help";
            this.Help_toolStripButton.ToolTipText = "Open the scripting document folder.";
            this.Help_toolStripButton.ButtonClick += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // About_toolStripButton
            // 
            this.About_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("About_toolStripButton.Image")));
            this.About_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.About_toolStripButton.Name = "About_toolStripButton";
            this.About_toolStripButton.Size = new System.Drawing.Size(152, 22);
            this.About_toolStripButton.Text = "About Editor";
            this.About_toolStripButton.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Settings_ToolStripMenuItem,
            this.tsmSetProjectFolder,
            this.encodingMessagesToolStripMenuItem,
            this.toolStripSeparator5,
            this.win32RenderTextToolStripMenuItem,
            this.caretSoftwareModeToolStripMenuItem,
            this.toolStripSeparator30,
            this.ParsingErrorsToolStripMenuItem,
            this.showIndentLineToolStripMenuItem,
            this.textLineNumberToolStripMenuItem,
            this.toolStripSeparator46,
            this.browserToolStripMenuItem,
            this.showLogWindowToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.ShowDropDownArrow = false;
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(69, 22);
            this.toolStripDropDownButton2.Text = "Options";
            // 
            // Settings_ToolStripMenuItem
            // 
            this.Settings_ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("Settings_ToolStripMenuItem.Image")));
            this.Settings_ToolStripMenuItem.Name = "Settings_ToolStripMenuItem";
            this.Settings_ToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.Settings_ToolStripMenuItem.Text = "Settings";
            this.Settings_ToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // tsmSetProjectFolder
            // 
            this.tsmSetProjectFolder.Image = ((System.Drawing.Image)(resources.GetObject("tsmSetProjectFolder.Image")));
            this.tsmSetProjectFolder.Name = "tsmSetProjectFolder";
            this.tsmSetProjectFolder.Size = new System.Drawing.Size(192, 22);
            this.tsmSetProjectFolder.Text = "Set Project folder";
            this.tsmSetProjectFolder.ToolTipText = "Sets the path to your project folder with .ssl and .h files.\r\nУстанавливает путь " +
    "к вашей папке проекта с .ssl и .h файлами.";
            this.tsmSetProjectFolder.Click += new System.EventHandler(this.tsmSetProjectFolder_Click);
            // 
            // encodingMessagesToolStripMenuItem
            // 
            this.encodingMessagesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.windowsDefaultMenuItem,
            this.EncodingDOSmenuItem});
            this.encodingMessagesToolStripMenuItem.Name = "encodingMessagesToolStripMenuItem";
            this.encodingMessagesToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.encodingMessagesToolStripMenuItem.ShowShortcutKeys = false;
            this.encodingMessagesToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.encodingMessagesToolStripMenuItem.Text = "Encoding messages file";
            this.encodingMessagesToolStripMenuItem.ToolTipText = "Saving and reading dialog message files in the selected encoding.";
            // 
            // windowsDefaultMenuItem
            // 
            this.windowsDefaultMenuItem.Checked = true;
            this.windowsDefaultMenuItem.CheckOnClick = true;
            this.windowsDefaultMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.windowsDefaultMenuItem.Name = "windowsDefaultMenuItem";
            this.windowsDefaultMenuItem.Size = new System.Drawing.Size(182, 22);
            this.windowsDefaultMenuItem.Text = "Windows Default";
            this.windowsDefaultMenuItem.ToolTipText = "Use the encoding of the specified Windows operating system.";
            this.windowsDefaultMenuItem.Click += new System.EventHandler(this.EncodingMenuItem_Click);
            // 
            // EncodingDOSmenuItem
            // 
            this.EncodingDOSmenuItem.Name = "EncodingDOSmenuItem";
            this.EncodingDOSmenuItem.Size = new System.Drawing.Size(182, 22);
            this.EncodingDOSmenuItem.Tag = "dos";
            this.EncodingDOSmenuItem.Text = "OEM-866 (Dos/Rus)";
            this.EncodingDOSmenuItem.Click += new System.EventHandler(this.EncodingMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(189, 6);
            // 
            // win32RenderTextToolStripMenuItem
            // 
            this.win32RenderTextToolStripMenuItem.Checked = true;
            this.win32RenderTextToolStripMenuItem.CheckOnClick = true;
            this.win32RenderTextToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.win32RenderTextToolStripMenuItem.Name = "win32RenderTextToolStripMenuItem";
            this.win32RenderTextToolStripMenuItem.ShowShortcutKeys = false;
            this.win32RenderTextToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.win32RenderTextToolStripMenuItem.Text = "WinAPI Render Text";
            this.win32RenderTextToolStripMenuItem.ToolTipText = "Use WinAPI functions to output display text.\r\n(this works faster than the graphic" +
    "s GDI method).";
            this.win32RenderTextToolStripMenuItem.Click += new System.EventHandler(this.win32RenderTextToolStripMenuItem_Click);
            // 
            // caretSoftwareModeToolStripMenuItem
            // 
            this.caretSoftwareModeToolStripMenuItem.Checked = true;
            this.caretSoftwareModeToolStripMenuItem.CheckOnClick = true;
            this.caretSoftwareModeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.caretSoftwareModeToolStripMenuItem.Name = "caretSoftwareModeToolStripMenuItem";
            this.caretSoftwareModeToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.caretSoftwareModeToolStripMenuItem.Text = "Caret software mode";
            this.caretSoftwareModeToolStripMenuItem.Click += new System.EventHandler(this.caretModeToolStripMenuItem_Click);
            // 
            // toolStripSeparator30
            // 
            this.toolStripSeparator30.Name = "toolStripSeparator30";
            this.toolStripSeparator30.Size = new System.Drawing.Size(189, 6);
            // 
            // ParsingErrorsToolStripMenuItem
            // 
            this.ParsingErrorsToolStripMenuItem.Checked = true;
            this.ParsingErrorsToolStripMenuItem.CheckOnClick = true;
            this.ParsingErrorsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ParsingErrorsToolStripMenuItem.Name = "ParsingErrorsToolStripMenuItem";
            this.ParsingErrorsToolStripMenuItem.ShowShortcutKeys = false;
            this.ParsingErrorsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.ParsingErrorsToolStripMenuItem.Text = "Highlight parsing errors";
            this.ParsingErrorsToolStripMenuItem.ToolTipText = "Highlight (underscore) parser errors in the script code.";
            this.ParsingErrorsToolStripMenuItem.Click += new System.EventHandler(this.ParsingErrorsToolStripMenuItem_Click);
            // 
            // showIndentLineToolStripMenuItem
            // 
            this.showIndentLineToolStripMenuItem.Checked = true;
            this.showIndentLineToolStripMenuItem.CheckOnClick = true;
            this.showIndentLineToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showIndentLineToolStripMenuItem.Name = "showIndentLineToolStripMenuItem";
            this.showIndentLineToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.showIndentLineToolStripMenuItem.Text = "Show indent line";
            this.showIndentLineToolStripMenuItem.ToolTipText = "Show the vertical indent line.";
            this.showIndentLineToolStripMenuItem.Click += new System.EventHandler(this.showIndentLineToolStripMenuItem_Click);
            // 
            // textLineNumberToolStripMenuItem
            // 
            this.textLineNumberToolStripMenuItem.Checked = true;
            this.textLineNumberToolStripMenuItem.CheckOnClick = true;
            this.textLineNumberToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.textLineNumberToolStripMenuItem.Name = "textLineNumberToolStripMenuItem";
            this.textLineNumberToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.textLineNumberToolStripMenuItem.Text = "Line Numbers";
            this.textLineNumberToolStripMenuItem.ToolTipText = "Show/Hide line text numbers.";
            this.textLineNumberToolStripMenuItem.Click += new System.EventHandler(this.ShowLineNumbers);
            // 
            // toolStripSeparator46
            // 
            this.toolStripSeparator46.Name = "toolStripSeparator46";
            this.toolStripSeparator46.Size = new System.Drawing.Size(189, 6);
            // 
            // browserToolStripMenuItem
            // 
            this.browserToolStripMenuItem.Checked = true;
            this.browserToolStripMenuItem.CheckOnClick = true;
            this.browserToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.browserToolStripMenuItem.Name = "browserToolStripMenuItem";
            this.browserToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
            this.browserToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.browserToolStripMenuItem.Text = "Right Browser";
            this.browserToolStripMenuItem.Click += new System.EventHandler(this.browserToolStripMenuItem_Click);
            // 
            // showLogWindowToolStripMenuItem
            // 
            this.showLogWindowToolStripMenuItem.Checked = true;
            this.showLogWindowToolStripMenuItem.CheckOnClick = true;
            this.showLogWindowToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showLogWindowToolStripMenuItem.Name = "showLogWindowToolStripMenuItem";
            this.showLogWindowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.L)));
            this.showLogWindowToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.showLogWindowToolStripMenuItem.Text = "Log Window";
            this.showLogWindowToolStripMenuItem.ToolTipText = "Show/Hide log window";
            this.showLogWindowToolStripMenuItem.Click += new System.EventHandler(this.showLogWindowToolStripMenuItem_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.decIndentToolStripMenuItem,
            this.funcToolStripMenuItem,
            this.gotoProcToolStripMenuItem,
            this.createProcToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Overflow = System.Windows.Forms.ToolStripItemOverflow.Always;
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(29, 20);
            this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Visible = false;
            this.toolStripDropDownButton1.Click += new System.EventHandler(this.DecIndentStripButton_Click);
            // 
            // decIndentToolStripMenuItem
            // 
            this.decIndentToolStripMenuItem.Name = "decIndentToolStripMenuItem";
            this.decIndentToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Q)));
            this.decIndentToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.decIndentToolStripMenuItem.Text = "DecIndent";
            this.decIndentToolStripMenuItem.Click += new System.EventHandler(this.DecIndentStripButton_Click);
            // 
            // funcToolStripMenuItem
            // 
            this.funcToolStripMenuItem.Name = "funcToolStripMenuItem";
            this.funcToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
            this.funcToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.funcToolStripMenuItem.Text = "func";
            this.funcToolStripMenuItem.Click += new System.EventHandler(this.funcToolStripMenuItem_Click);
            // 
            // gotoProcToolStripMenuItem
            // 
            this.gotoProcToolStripMenuItem.Name = "gotoProcToolStripMenuItem";
            this.gotoProcToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.P)));
            this.gotoProcToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.gotoProcToolStripMenuItem.Tag = "Button";
            this.gotoProcToolStripMenuItem.Text = "gotoProc";
            this.gotoProcToolStripMenuItem.Click += new System.EventHandler(this.findDefinitionToolStripMenuItem_Click);
            // 
            // createProcToolStripMenuItem
            // 
            this.createProcToolStripMenuItem.Name = "createProcToolStripMenuItem";
            this.createProcToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.createProcToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.createProcToolStripMenuItem.Text = "createProc";
            this.createProcToolStripMenuItem.Click += new System.EventHandler(this.createProcedureToolStripMenuItem_Click);
            // 
            // tsbUpdateParserData
            // 
            this.tsbUpdateParserData.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbUpdateParserData.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tsbUpdateParserData.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tsbUpdateParserData.Image = ((System.Drawing.Image)(resources.GetObject("tsbUpdateParserData.Image")));
            this.tsbUpdateParserData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUpdateParserData.Name = "tsbUpdateParserData";
            this.tsbUpdateParserData.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.tsbUpdateParserData.Size = new System.Drawing.Size(23, 22);
            this.tsbUpdateParserData.ToolTipText = "Update parser data.";
            this.tsbUpdateParserData.Click += new System.EventHandler(this.tsbUpdateParserData_Click);
            // 
            // toolStripSeparator56
            // 
            this.toolStripSeparator56.Name = "toolStripSeparator56";
            this.toolStripSeparator56.Size = new System.Drawing.Size(6, 25);
            // 
            // tslProject
            // 
            this.tslProject.ActiveLinkColor = System.Drawing.Color.RoyalBlue;
            this.tslProject.Enabled = false;
            this.tslProject.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tslProject.IsLink = true;
            this.tslProject.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.tslProject.LinkColor = System.Drawing.Color.MediumBlue;
            this.tslProject.MergeAction = System.Windows.Forms.MergeAction.Remove;
            this.tslProject.MergeIndex = 1;
            this.tslProject.Name = "tslProject";
            this.tslProject.Size = new System.Drawing.Size(94, 22);
            this.tslProject.Text = "Project:  <unset>";
            this.tslProject.ToolTipText = "Click to open the project folder in the Explorer window.\r\nЩелкните, чтобы открыть" +
    " папку проекта в окне Проводника.";
            this.tslProject.Click += new System.EventHandler(this.tslProject_Click);
            // 
            // cmsTabControls
            // 
            this.cmsTabControls.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFolderToolStripMenuItem,
            this.openInExternalToolStripMenuItem,
            this.toolStripSeparator37,
            this.closeToolStripMenuItem,
            this.closeAllToolStripMenuItem,
            this.closeAllButThisToolStripMenuItem});
            this.cmsTabControls.Name = "cmsTabControls";
            this.cmsTabControls.ShowImageMargin = false;
            this.cmsTabControls.Size = new System.Drawing.Size(173, 120);
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.ShowShortcutKeys = false;
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.openFolderToolStripMenuItem.Text = "Open Folder in Explorer";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // openInExternalToolStripMenuItem
            // 
            this.openInExternalToolStripMenuItem.Name = "openInExternalToolStripMenuItem";
            this.openInExternalToolStripMenuItem.ShowShortcutKeys = false;
            this.openInExternalToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.openInExternalToolStripMenuItem.Text = "Open in External Editor";
            this.openInExternalToolStripMenuItem.Click += new System.EventHandler(this.openInExternalToolStripMenuItem_Click);
            // 
            // toolStripSeparator37
            // 
            this.toolStripSeparator37.Name = "toolStripSeparator37";
            this.toolStripSeparator37.Size = new System.Drawing.Size(169, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeyDisplayString = "MMB";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem1_Click);
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.closeAllToolStripMenuItem.Text = "Close All";
            this.closeAllToolStripMenuItem.Click += new System.EventHandler(this.CloseAllToolStripMenuItemClick);
            // 
            // closeAllButThisToolStripMenuItem
            // 
            this.closeAllButThisToolStripMenuItem.Name = "closeAllButThisToolStripMenuItem";
            this.closeAllButThisToolStripMenuItem.ShowShortcutKeys = false;
            this.closeAllButThisToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.closeAllButThisToolStripMenuItem.Text = "Close All But This";
            this.closeAllButThisToolStripMenuItem.Click += new System.EventHandler(this.CloseAllButThisToolStripMenuItemClick);
            // 
            // ofdScripts
            // 
            this.ofdScripts.Filter = "All supported files|*.ssl;*.int;*.h;*.msg|Script files|*.ssl|Header files|*.h|Com" +
    "piled scripts|*.int|Message files|*.msg|All files|*.*";
            this.ofdScripts.Multiselect = true;
            this.ofdScripts.RestoreDirectory = true;
            this.ofdScripts.Title = "Select script or header to open";
            // 
            // sfdScripts
            // 
            this.sfdScripts.DefaultExt = "ssl";
            this.sfdScripts.Filter = "Script file (.ssl)|*.ssl|Header file (.h)|*.h|Message files (.msg)|*.msg|All file" +
    "s|*.*";
            this.sfdScripts.RestoreDirectory = true;
            this.sfdScripts.Title = "Save file as";
            // 
            // fbdMassCompile
            // 
            this.fbdMassCompile.Description = "Select folder to compile";
            this.fbdMassCompile.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.fbdMassCompile.ShowNewFolderButton = false;
            // 
            // bwSyntaxParser
            // 
            this.bwSyntaxParser.WorkerSupportsCancellation = true;
            this.bwSyntaxParser.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwSyntaxParser_DoWork);
            this.bwSyntaxParser.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwSyntaxParser_RunWorkerCompleted);
            // 
            // editorMenuStrip
            // 
            this.editorMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.highlightToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.toolStripSeparator2,
            this.findDeclerationToolStripMenuItem,
            this.findDefinitionToolStripMenuItem,
            this.findReferencesToolStripMenuItem,
            this.toolStripSeparator36,
            this.openIncludeToolStripMenuItem,
            this.toolStripSeparator6,
            this.convertHexDecToolStripMenuItem,
            this.toolStripSeparator54,
            this.UpperCaseToolStripMenuItem1,
            this.LowerCaseToolStripMenuItem,
            this.toolStripSeparator29,
            this.cutToolStripMenuItem1,
            this.copyToolStripMenuItem1,
            this.pasteToolStripMenuItem1,
            this.toolStripSeparator28,
            this.commentTextToolStripMenuItem,
            this.uncommentTextToolStripMenuItem,
            this.AlignToLeftToolStripMenuItem,
            this.formatingCodeToolStripMenuItem});
            this.editorMenuStrip.Name = "editorMenuStrip1";
            this.editorMenuStrip.Size = new System.Drawing.Size(226, 392);
            this.editorMenuStrip.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.editorMenuStrip_Closed);
            this.editorMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.editorMenuStrip_Opening);
            // 
            // highlightToolStripMenuItem
            // 
            this.highlightToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("highlightToolStripMenuItem.Image")));
            this.highlightToolStripMenuItem.Name = "highlightToolStripMenuItem";
            this.highlightToolStripMenuItem.ShortcutKeyDisplayString = "Middle Mouse";
            this.highlightToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.highlightToolStripMenuItem.Text = "Highlight";
            this.highlightToolStripMenuItem.ToolTipText = "Highlight selected text [click middle mouse button]";
            this.highlightToolStripMenuItem.Visible = false;
            this.highlightToolStripMenuItem.Click += new System.EventHandler(this.highlightToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("renameToolStripMenuItem.Image")));
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Visible = false;
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(222, 6);
            // 
            // findDeclerationToolStripMenuItem
            // 
            this.findDeclerationToolStripMenuItem.Name = "findDeclerationToolStripMenuItem";
            this.findDeclerationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F12)));
            this.findDeclerationToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.findDeclerationToolStripMenuItem.Text = "Goto declaration";
            this.findDeclerationToolStripMenuItem.Click += new System.EventHandler(this.findDeclerationToolStripMenuItem_Click);
            // 
            // findDefinitionToolStripMenuItem
            // 
            this.findDefinitionToolStripMenuItem.Name = "findDefinitionToolStripMenuItem";
            this.findDefinitionToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.findDefinitionToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.findDefinitionToolStripMenuItem.Text = "Goto definition";
            this.findDefinitionToolStripMenuItem.Click += new System.EventHandler(this.findDefinitionToolStripMenuItem_Click);
            // 
            // findReferencesToolStripMenuItem
            // 
            this.findReferencesToolStripMenuItem.Name = "findReferencesToolStripMenuItem";
            this.findReferencesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F12)));
            this.findReferencesToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.findReferencesToolStripMenuItem.Text = "Find references";
            this.findReferencesToolStripMenuItem.Click += new System.EventHandler(this.findReferencesToolStripMenuItem_Click);
            // 
            // toolStripSeparator36
            // 
            this.toolStripSeparator36.Name = "toolStripSeparator36";
            this.toolStripSeparator36.Size = new System.Drawing.Size(222, 6);
            // 
            // openIncludeToolStripMenuItem
            // 
            this.openIncludeToolStripMenuItem.Name = "openIncludeToolStripMenuItem";
            this.openIncludeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.G)));
            this.openIncludeToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.openIncludeToolStripMenuItem.Text = "Open include";
            this.openIncludeToolStripMenuItem.Click += new System.EventHandler(this.openIncludeToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(222, 6);
            // 
            // convertHexDecToolStripMenuItem
            // 
            this.convertHexDecToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("convertHexDecToolStripMenuItem.Image")));
            this.convertHexDecToolStripMenuItem.Name = "convertHexDecToolStripMenuItem";
            this.convertHexDecToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.convertHexDecToolStripMenuItem.Text = "Convert value: Hex <> Dec";
            this.convertHexDecToolStripMenuItem.Click += new System.EventHandler(this.convertHexDecToolStripMenuItem_Click);
            // 
            // toolStripSeparator54
            // 
            this.toolStripSeparator54.Name = "toolStripSeparator54";
            this.toolStripSeparator54.Size = new System.Drawing.Size(222, 6);
            // 
            // UpperCaseToolStripMenuItem1
            // 
            this.UpperCaseToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("UpperCaseToolStripMenuItem1.Image")));
            this.UpperCaseToolStripMenuItem1.Name = "UpperCaseToolStripMenuItem1";
            this.UpperCaseToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
            this.UpperCaseToolStripMenuItem1.Size = new System.Drawing.Size(225, 22);
            this.UpperCaseToolStripMenuItem1.Text = "Upper Case";
            this.UpperCaseToolStripMenuItem1.Click += new System.EventHandler(this.UPPERCASEToolStripMenuItemClick);
            // 
            // LowerCaseToolStripMenuItem
            // 
            this.LowerCaseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("LowerCaseToolStripMenuItem.Image")));
            this.LowerCaseToolStripMenuItem.Name = "LowerCaseToolStripMenuItem";
            this.LowerCaseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Z)));
            this.LowerCaseToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.LowerCaseToolStripMenuItem.Text = "Lower Case";
            this.LowerCaseToolStripMenuItem.Click += new System.EventHandler(this.LowecaseToolStripMenuItemClick);
            // 
            // toolStripSeparator29
            // 
            this.toolStripSeparator29.Name = "toolStripSeparator29";
            this.toolStripSeparator29.Size = new System.Drawing.Size(222, 6);
            // 
            // cutToolStripMenuItem1
            // 
            this.cutToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem1.Image")));
            this.cutToolStripMenuItem1.Name = "cutToolStripMenuItem1";
            this.cutToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem1.Size = new System.Drawing.Size(225, 22);
            this.cutToolStripMenuItem1.Text = "Cut";
            this.cutToolStripMenuItem1.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem1
            // 
            this.copyToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem1.Image")));
            this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
            this.copyToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem1.Size = new System.Drawing.Size(225, 22);
            this.copyToolStripMenuItem1.Text = "Copy";
            this.copyToolStripMenuItem1.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem1
            // 
            this.pasteToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem1.Image")));
            this.pasteToolStripMenuItem1.Name = "pasteToolStripMenuItem1";
            this.pasteToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem1.Size = new System.Drawing.Size(225, 22);
            this.pasteToolStripMenuItem1.Text = "Paste";
            this.pasteToolStripMenuItem1.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripSeparator28
            // 
            this.toolStripSeparator28.Name = "toolStripSeparator28";
            this.toolStripSeparator28.Size = new System.Drawing.Size(222, 6);
            // 
            // commentTextToolStripMenuItem
            // 
            this.commentTextToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("commentTextToolStripMenuItem.Image")));
            this.commentTextToolStripMenuItem.Name = "commentTextToolStripMenuItem";
            this.commentTextToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.commentTextToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.commentTextToolStripMenuItem.Text = "Comment text";
            this.commentTextToolStripMenuItem.Click += new System.EventHandler(this.CommentTextStripButton_Click);
            // 
            // uncommentTextToolStripMenuItem
            // 
            this.uncommentTextToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("uncommentTextToolStripMenuItem.Image")));
            this.uncommentTextToolStripMenuItem.Name = "uncommentTextToolStripMenuItem";
            this.uncommentTextToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
            this.uncommentTextToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.uncommentTextToolStripMenuItem.Text = "Uncomment text";
            this.uncommentTextToolStripMenuItem.Click += new System.EventHandler(this.UnCommentTextStripButton_Click);
            // 
            // AlignToLeftToolStripMenuItem
            // 
            this.AlignToLeftToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("AlignToLeftToolStripMenuItem.Image")));
            this.AlignToLeftToolStripMenuItem.Name = "AlignToLeftToolStripMenuItem";
            this.AlignToLeftToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.AlignToLeftToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.AlignToLeftToolStripMenuItem.Text = "Align selected text";
            this.AlignToLeftToolStripMenuItem.ToolTipText = "Align selected block text to left.";
            this.AlignToLeftToolStripMenuItem.Click += new System.EventHandler(this.AlignToLeftToolStripMenuItem_Click);
            // 
            // formatingCodeToolStripMenuItem
            // 
            this.formatingCodeToolStripMenuItem.Name = "formatingCodeToolStripMenuItem";
            this.formatingCodeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.formatingCodeToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.formatingCodeToolStripMenuItem.Text = "Formating code";
            this.formatingCodeToolStripMenuItem.ToolTipText = "Formatting selected text code, inserting missing dividing spaces between arithmet" +
    "ic and logical operations.";
            this.formatingCodeToolStripMenuItem.Click += new System.EventHandler(this.formatingCodeToolStripMenuItem_Click);
            // 
            // toolTips
            // 
            this.toolTips.AutoPopDelay = 50000;
            this.toolTips.InitialDelay = 500;
            this.toolTips.OwnerDraw = true;
            this.toolTips.ReshowDelay = 100;
            // 
            // fbdProjectFolder
            // 
            this.fbdProjectFolder.Description = "Select the folder containing .ssl and .h files of your project.";
            this.fbdProjectFolder.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // tabControl1
            // 
            this.tabControl1.AllowDrop = true;
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.ImageList = this.imageList1;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.ShowToolTips = true;
            this.tabControl1.Size = new System.Drawing.Size(984, 601);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.TabStop = false;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            this.tabControl1.DragDrop += new System.Windows.Forms.DragEventHandler(this.TextEditorDragDrop);
            this.tabControl1.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextEditorDragEnter);
            this.tabControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseClick);
            // 
            // TextEditor
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1290, 707);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "TextEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sfall Script Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextEditor_FormClosing);
            this.Load += new System.EventHandler(this.TextEditor_Load);
            this.Shown += new System.EventHandler(this.TextEditor_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.TextEditorDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextEditorDragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextEditor_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextEditor_KeyUp);
            this.Resize += new System.EventHandler(this.TextEditor_Resize);
            this.panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.cmsFunctions.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.SearchToolStrip.ResumeLayout(false);
            this.SearchToolStrip.PerformLayout();
            this.tabControl2.ResumeLayout(false);
            this.tabPageParse.ResumeLayout(false);
            this.tabPageParse.PerformLayout();
            this.tabPageBuild.ResumeLayout(false);
            this.tabPageBuild.PerformLayout();
            this.tabPageError.ResumeLayout(false);
            this.cmsError.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).EndInit();
            this.tabControl3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ProcMnContext.ResumeLayout(false);
            this.toolStripProcedures.ResumeLayout(false);
            this.toolStripProcedures.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tpExplorerFiles.ResumeLayout(false);
            this.tpExplorerFiles.PerformLayout();
            this.toolStripSolution.ResumeLayout(false);
            this.toolStripSolution.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ToolStripMain.ResumeLayout(false);
            this.ToolStripMain.PerformLayout();
            this.cmsTabControls.ResumeLayout(false);
            this.editorMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.TabPage tabPageParse;
        private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllButThisToolStripMenuItem;

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.OpenFileDialog ofdScripts;
        private System.Windows.Forms.SaveFileDialog sfdScripts;
        private DraggableTabControl tabControl1;
        private System.Windows.Forms.FolderBrowserDialog fbdMassCompile;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPageBuild;
        private System.Windows.Forms.TabPage tabPageError;
        private System.Windows.Forms.DataGridView dgvErrors;
        private System.ComponentModel.BackgroundWorker bwSyntaxParser;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ContextMenuStrip editorMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem findReferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findDeclerationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findDefinitionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openIncludeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip cmsTabControls;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem UpperCaseToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem LowerCaseToolStripMenuItem;
        private System.Windows.Forms.ToolStrip ToolStripMain;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Button minimizelog_button;
        private System.Windows.Forms.ToolStripButton Undo_toolStripButton;
        private System.Windows.Forms.ToolStripButton Redo_ToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSplitButton Open_toolStripSplitButton;
        private System.Windows.Forms.ToolStripSplitButton Save_toolStripSplitButton;
        private System.Windows.Forms.ToolStripMenuItem SaveAs_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveAll_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripButton Back_toolStripButton;
        private System.Windows.Forms.ToolStripButton Forward_toolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripSplitButton New_toolStripDropDownButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripSplitButton qCompile_toolStripSplitButton;
        private System.Windows.Forms.ToolStripMenuItem CompileAllOpen_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MassCompile_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Save_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Settings_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem Preprocess_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem showLogWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Compile_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton Script_toolStripSplitButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripMenuItem editRegisteredScriptsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem saveAsTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem roundtripToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton Outline_toolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.Button TabClose_button;
        private System.Windows.Forms.ToolStripSplitButton Headers_toolStripSplitButton;
        private System.Windows.Forms.ToolStripMenuItem openAllIncludesScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openHeaderFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Button Split_button;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSplitButton GotoProc_StripButton;
        private System.Windows.Forms.ToolStripMenuItem gotoToLineToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel LineStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel ColStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel EmptyStripStatusLabel;
        private System.Windows.Forms.ContextMenuStrip ProcMnContext;
        private System.Windows.Forms.ToolStripMenuItem createProcedureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameProcedureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveProcedureToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ToolStripMenuItem deleteProcedureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textLineNumberToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton FunctionButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
        private System.Windows.Forms.ToolStripSplitButton MSG_toolStripButton;
        private System.Windows.Forms.ToolStripMenuItem previewDialogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem msgFileEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStrip SearchToolStrip;
        private System.Windows.Forms.ToolStripButton CaseButton;
        private System.Windows.Forms.ToolStripButton FindForwardButton;
        private System.Windows.Forms.ToolStripButton FindBackButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator22;
        private System.Windows.Forms.ToolStripButton ReplaceButton;
        private System.Windows.Forms.ToolStripTextBox ReplaceTextBox;
        private System.Windows.Forms.ToolStripButton ReplaceAllButton;
        private System.Windows.Forms.ToolStripSplitButton Search_toolStripButton;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton SearchHideStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator23;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator24;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator26;
        private System.Windows.Forms.ToolStripMenuItem defineToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton SendtoolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator27;
        private System.Windows.Forms.ToolStripButton DecIndentStripButton;
        private System.Windows.Forms.ToolStripButton CommentStripButton;
        private System.Windows.Forms.ToolStripMenuItem AlignToLeftToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator29;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator28;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator30;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem decIndentToolStripMenuItem;
        protected internal System.Windows.Forms.ToolStripMenuItem msgAutoOpenEditorStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator25;
        protected internal System.Windows.Forms.ToolStripComboBox SearchTextComboBox;
        private System.Windows.Forms.ToolStripComboBox pDefineStripComboBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn cType;
        private System.Windows.Forms.DataGridViewTextBoxColumn cFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLine;
        private System.Windows.Forms.DataGridViewTextBoxColumn cMessage;
        private System.Windows.Forms.ToolStripMenuItem browserToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.ToolStripMenuItem funcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commentTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncommentTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gotoProcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatingCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStripProcedures;
        private System.Windows.Forms.ToolStripButton OnlyProcStripButton;
        private System.Windows.Forms.ToolStripButton GoBeginStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
        private System.Windows.Forms.ToolStripButton NewProcStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator31;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator32;
        private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator33;
        private System.Windows.Forms.ToolStripMenuItem quickFindToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator34;
        private System.Windows.Forms.ToolStripButton ViewArgsStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator35;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator36;
        private System.Windows.Forms.ToolStripButton WholeWordButton;
        private System.Windows.Forms.ToolStripMenuItem highlightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ParsingErrorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dialogNodesDiagramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem encodingMessagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsDefaultMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EncodingDOSmenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator37;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator38;
        private System.Windows.Forms.ToolStripMenuItem editNodeCodeToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripDropDownButton Edit_toolStripButton;
        private System.Windows.Forms.ToolStripMenuItem ToggleBlockCommentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator40;
        private System.Windows.Forms.ToolStripMenuItem capitalizeCaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator39;
        private System.Windows.Forms.ToolStripMenuItem trailingSpacesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leadingTabsSpacesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allTabsSpacesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showTabsAndSpacesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator41;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator42;
        private System.Windows.Forms.ContextMenuStrip cmsError;
        private System.Windows.Forms.ToolStripMenuItem tsmShowParserLog;
        private System.Windows.Forms.ToolStripMenuItem tsmShowBuildLog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator43;
        private System.Windows.Forms.ToolStripMenuItem tsmCopyLogText;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearAllLog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator44;
        private System.Windows.Forms.ToolStripMenuItem tsmMessageTextChecker;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator45;
        private System.Windows.Forms.ToolStripSplitButton Help_toolStripButton;
        private System.Windows.Forms.ToolStripMenuItem About_toolStripButton;
        private System.Windows.Forms.ToolStripMenuItem showIndentLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator46;
        private System.Windows.Forms.ToolStripMenuItem refreshLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoRefreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator47;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator48;
        private System.Windows.Forms.ToolStripMenuItem saveUTF8ToolStripMenuItem;
        internal System.Windows.Forms.TextBox tbOutputParse;
        private System.Windows.Forms.ToolStripStatusLabel FontSizeStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem decompileF1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator49;
        private System.Windows.Forms.ToolStripMenuItem win32RenderTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openInExternalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem includeFileToCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator50;
        private System.Windows.Forms.ContextMenuStrip cmsFunctions;
        private System.Windows.Forms.ToolStripMenuItem addUserFunctionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTreeNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator51;
        private System.Windows.Forms.ToolStripMenuItem editFunctionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator52;
        private System.Windows.Forms.ToolStripMenuItem renameTreeNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteNodeFuncToolStripMenuItem;
        internal System.Windows.Forms.TreeView FunctionTreeLeft;
        internal System.Windows.Forms.TreeView FunctionsTree;
        internal System.Windows.Forms.TreeView ProcTree;
        private System.Windows.Forms.ToolStripMenuItem oldDecompileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showAutocompleteWordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dialogFunctionConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator53;
        private System.Windows.Forms.ToolStripMenuItem convertHexDecToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator54;
        private System.Windows.Forms.ToolStripMenuItem splitDocumentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem caretSoftwareModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton tsbSaveAll;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator55;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator56;
        private System.Windows.Forms.ToolStripButton tsbUpdateParserData;
        private System.Windows.Forms.TabPage tpExplorerFiles;
        private System.Windows.Forms.TreeView treeProjectFiles;
        private System.Windows.Forms.ToolStrip toolStripSolution;
        private System.Windows.Forms.ToolStripButton tsbSetProjectFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator57;
        private System.Windows.Forms.FolderBrowserDialog fbdProjectFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmSetProjectFolder;
        private System.Windows.Forms.ToolStripLabel tslProject;
        private System.Windows.Forms.ToolStripMenuItem createProcToolStripMenuItem;
    }
}