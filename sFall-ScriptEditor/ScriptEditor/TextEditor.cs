using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;
using ScriptEditor.SyntaxRules;

using ScriptEditor.TextEditorUI;
using ScriptEditor.TextEditorUI.CompleteList;
using ScriptEditor.TextEditorUI.ToolTips;

using ScriptEditor.TextEditorUtilities;

namespace ScriptEditor
{
    partial class TextEditor : Form
    {
        private const string SSE = AboutBox.appName + " - ";

        private const string unsaved = "unsaved.ssl";
        private const string treeTipProcedure = "\n\n - Click and hold Ctrl key to paste the procedure name into the script.\n - Double click to goto the procedure.";
        private const string treeTipVariable = "\n\n - Click and hold Ctrl key to paste the variable name into the script.\n - Double click to goto the variable.";

        private static readonly string[] TREEPROCEDURES = new string[] { "Global Procedures", "Local Procedures" };
        private static readonly string[] TREEVARIABLES = new string[] { "Global Variables", "Script Variables" };
        private static readonly System.Media.SoundPlayer DontFind = new System.Media.SoundPlayer(Properties.Resources.DontFind);
        private static readonly System.Media.SoundPlayer CompileFail = new System.Media.SoundPlayer(Properties.Resources.CompileError);

        private readonly List<TabInfo> tabs = new List<TabInfo>();
        private TabInfo currentTab;
        private ToolStripLabel parserLabel;

        private SearchForm sf;
        private GoToLine goToLine;

        private int previousTabIndex = -1;
        private int minimizeLogSize;
        private PositionType PosChangeType;
        private int moveActive = -1;
        private int fuctionPanel = -1;
        private FormWindowState wState;
        private readonly string[] commandsArgs;
        private bool SplitEvent;

        private bool ctrlKeyPress;
        private bool dbClick;

        private int showTipsColumn;
        private bool roundTrip = false;
        private bool savingRunning = false;

        internal TreeView VarTree = new TreeView();
        private TabPage VarTab = new TabPage("Variables");

        private AutoComplete autoComplete;

        /// <summary>
        /// Сокращенное свойство.
        /// Return: currentTab.textEditor.Document
        /// </summary>
        private IDocument currentDocument { get { return currentTab.textEditor.Document; } }

        /// <summary>
        /// Сокращенное свойство.
        /// Return: currentTab.textEditor.ActiveTextAreaControl
        /// </summary>
        private TextAreaControl currentActiveTextAreaCtrl { get { return currentTab.textEditor.ActiveTextAreaControl; } }

        internal bool RegistredScriptDialogShow { get; set; }

        private void EnableDoubleBuffering()
        {
           // Set the value of the double-buffering style bits to true.
           //this.SetStyle(ControlStyles.DoubleBuffer |
           //              ControlStyles.UserPaint |
           //              ControlStyles.AllPaintingInWmPaint,
           //              true);
           //this.UpdateStyles();

           Program.SetDoubleBuffered(panel1);
           Program.SetDoubleBuffered(dgvErrors);
        }

        #region Main form control
        public TextEditor(string[] args)
        {
            InitializeComponent();

            tabControl3.TabPages.RemoveAt(2); // скрываем от пользователя еще нереализованный функционал

            EnableDoubleBuffering();
            InitControlEvent();

            commandsArgs = args;
            Settings.SetupWindowPosition(SavedWindows.Main, this);

            if (!Settings.firstRun)
                WindowState = FormWindowState.Maximized;

            pDefineStripComboBox.Items.AddRange(File.ReadAllLines(Settings.PreprocDefPath));
            if (Settings.preprocDef != null)
                pDefineStripComboBox.Text = Settings.preprocDef;
            else
                pDefineStripComboBox.SelectedIndex = 0;
            SearchTextComboBox.Items.AddRange(File.ReadAllLines(Settings.SearchHistoryPath));
            SearchToolStrip.Visible = false;
            defineToolStripMenuItem.Checked = Settings.allowDefine;
            msgAutoOpenEditorStripMenuItem.Checked = Settings.openMsgEditor;
            showTabsAndSpacesToolStripMenuItem.Checked = Settings.showTabsChar;
            trailingSpacesToolStripMenuItem.Checked = Settings.autoTrailingSpaces;
            showIndentLineToolStripMenuItem.Checked = Settings.showVRuler;
            decompileF1ToolStripMenuItem.Checked = Settings.decompileF1;
            saveUTF8ToolStripMenuItem.Checked = Settings.saveScriptUTF8;
            win32RenderTextToolStripMenuItem.Checked = Settings.winAPITextRender;
            oldDecompileToolStripMenuItem.Checked = Settings.oldDecompile;
            SizeFontToString();

            ofdScripts.InitialDirectory = Settings.solutionProjectFolder;

            toolTips.Active = false;
            toolTips.Draw += delegate(object sender, DrawToolTipEventArgs e) { TipPainter.DrawInfo(e); };

            if (Settings.encoding == (byte)EncodingType.OEM866) {
                EncodingDOSmenuItem.Checked = true;
                windowsDefaultMenuItem.Checked = false;
            }

            // Highlighting
            FileSyntaxModeProvider fsmProvider = new FileSyntaxModeProvider(SyntaxFile.SyntaxFolder); // Create new provider with the highlighting directory.
            HighlightingManager.Manager.AddSyntaxModeFileProvider(fsmProvider); // Attach to the text editor.
            ColorTheme.InitTheme(Settings.highlight == 2, this);

            autoComplete = new AutoComplete(panel1, Settings.autocompleteColor);

            // Recent files
            UpdateRecentList();

            // Templates
            foreach (string file in Directory.GetFiles(Path.Combine(Settings.ResourcesFolder, "templates"), "*.ssl"))
            {
                ToolStripMenuItem mi = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(file));
                mi.Tag = file;
                mi.Click += new EventHandler(Template_Click); // Open Templates file
                New_toolStripDropDownButton.DropDownItems.Add(mi);
            }

            if (Settings.pathHeadersFiles == null)
                Headers_toolStripSplitButton.Enabled = false;

            HandlerProcedure.CreateProcHandlers(ProcMnContext, this);
            Functions.CreateTree(FunctionsTree);
            ProgramInfo.LoadOpcodes();

            DontFind.LoadAsync();
            CompileFail.LoadAsync();

            this.Text += " v." + AboutBox.appVersion;
            tbOutput.Text = "***** " +  AboutBox.appName + " v." + AboutBox.appVersion + AboutBox.appDescription + " *****";
        }

#if !DEBUG
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == SingleInstanceManager.WM_SFALL_SCRIPT_EDITOR_OPEN) {
                TabInfo result = null;
                var commandLineArgs = SingleInstanceManager.LoadCommandLine();
                foreach (var fArg in commandLineArgs)
                {
                    string file = fArg;
                    bool fcd = FileAssociation.CheckFCDFile(ref file);
                    if (file != null)
                        result = Open(file, OpenType.File, commandline: true, fcdOpen: fcd);

                }
                if (result != null && !this.Focused)
                    ShowMe();
            }
            base.WndProc(ref m);
        }

        // activate form only for open ssl file
        private void ShowMe()
        {
            if (WindowState == FormWindowState.Minimized)
                WindowState = wState;
            Activate();
            // get our current "TopMost" value (ours will always be false though)
            //bool top = TopMost;
            // make our form jump to the top of everything
            //TopMost = true;
            // set it back to whatever it was
            //TopMost = top;
        }
#else
        private void ShowMe() {}
#endif

#if TRACE
    void DEBUGINFO(string line) { tbOutput.Text = line + "\r\n" + tbOutput.Text; }
#else
    void DEBUGINFO(string line) { }
#endif

        private void TextEditor_Load(object sender, EventArgs e)
        {
            splitContainer3.Panel1Collapsed = true;
            splitContainer2.Panel2Collapsed = true;
            splitContainer1.Panel2Collapsed = true;
            splitContainer2.Panel1MinSize = 300;
            splitContainer2.Panel2MinSize = 150;
            splitContainer1.SplitterDistance = Size.Height;

            if (Settings.editorSplitterPosition == -1)
                minimizeLogSize = Size.Height - (Size.Height / 5);
            else
                minimizeLogSize = Settings.editorSplitterPosition;

            if (Settings.editorSplitterPosition2 != -1)
                splitContainer2.SplitterDistance = Settings.editorSplitterPosition2;
            else
                splitContainer2.SplitterDistance = Size.Width - 200;

            showLogWindowToolStripMenuItem.Checked = Settings.showLog;
            if (Settings.enableParser)
                CreateTabVarTree();
        }

        private void TextEditor_Shown(object sender, EventArgs e)
        {
            if (!Settings.firstRun)
                Settings_ToolStripMenuItem.PerformClick();

            // open documents passed from command line
            foreach (string fArg in commandsArgs)
            {
                string file = fArg;
                bool fcd = FileAssociation.CheckFCDFile(ref file);
                if (file != null)
                    Open(file, TextEditor.OpenType.File, commandline: true, fcdOpen: fcd);
            }

            this.Activated += TextEditor_Activated;
            this.Deactivate += TextEditor_Deactivate;
            SingleInstanceManager.SendEditorOpenMessage();
        }

        private void TextEditor_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
                wState = WindowState;

            if (autoComplete != null)
                autoComplete.Close();
        }

        private void TextEditor_Deactivate(object sender, EventArgs e)
        {
            if (currentTab == null) return;
            currentActiveTextAreaCtrl.TextArea.MouseEnter -= TextArea_SetFocus;
            ctrlKeyPress = false;
        }

        private void TextEditor_Activated(object sender, EventArgs e)
        {
            if (currentTab == null) return;
            currentActiveTextAreaCtrl.TextArea.MouseEnter += TextArea_SetFocus;

            if (WindowState != FormWindowState.Minimized)
                CheckChandedFile();
            else {
                Timer timer = new Timer();
                timer.Interval = 500; // interval time - 0.5 sec
                timer.Tick += delegate(object obj, EventArgs eArg) {
                    timer.Stop();
                    timer.Dispose();
                    CheckChandedFile();
                };
                timer.Start();
            }
            if ((Control.ModifierKeys & Keys.Control) != 0) ctrlKeyPress = true;
        }

        private void TextEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < tabs.Count; i++) {
                bool skip = tabs[i].changed;
                if (tabs[i].changed) {
                    switch (MessageBox.Show("Save changes to " + tabs[i].filename + "?", "Message", MessageBoxButtons.YesNoCancel)) {
                        case DialogResult.Yes:
                            Save(tabs[i], true);
                            if (tabs[i].changed) {
                                e.Cancel = true;
                                return;
                            }
                            break;
                        case DialogResult.No:
                            break;
                        default:
                            e.Cancel = true;
                            return;
                    }
                }
                KeepScriptSetting(tabs[i], skip);
            }

            while (bwSyntaxParser.IsBusy) {
                System.Threading.Thread.Sleep(100); // Avoid stomping on files while the parser is running
                Application.DoEvents();
            }

            splitContainer3.Panel1Collapsed = true;
            int dist = this.Height - (this.Height / 4) + 100;
            Settings.editorSplitterPosition = (splitContainer1.SplitterDistance < dist) ? splitContainer1.SplitterDistance : -1;
            Settings.editorSplitterPosition2 = splitContainer2.SplitterDistance;
            Settings.SaveSettingData(this);
            SyntaxFile.DeleteSyntaxFile();
        }
        #endregion

        #region Control set states
        private void InitControlEvent()
        {
            if (Settings.solutionProjectFolder != null) SetProjectFolderText();

            // Parser
            parserLabel = new ToolStripLabel((Settings.enableParser) ? "Parser: No file" : parseoff);
            parserLabel.Alignment = ToolStripItemAlignment.Right;
            parserLabel.Overflow = ToolStripItemOverflow.Never;
            parserLabel.Click += delegate(object sender, EventArgs e) { ParseScript(0); };
            parserLabel.ToolTipText = "Click - Update parser data.";
            parserLabel.TextChanged += delegate(object sender, EventArgs e) { parserLabel.ForeColor = Color.Black; };
            ToolStripMain.Items.Add(parserLabel);

            // Parser timer
            extParserTimer = new Timer();
            extParserTimer.Interval = 100;
            extParserTimer.Tick += new EventHandler(ExternalParser_Tick);
            intParserTimer = new Timer();
            intParserTimer.Interval = 10;
            intParserTimer.Tick += new EventHandler(InternalParser_Tick);

            // Tabs Swapped
            tabControl1.tabsSwapped += delegate(object sender, TabsSwappedEventArgs e) {
                TabInfo tmp = tabs[e.aIndex];
                tabs[e.aIndex] = tabs[e.bIndex];
                tabs[e.aIndex].index = e.aIndex;
                tabs[e.bIndex] = tmp;
                tabs[e.bIndex].index = e.bIndex;
            };

            // Create Variable Tab
            VarTree.HotTracking = true;
            VarTree.ShowNodeToolTips = true;
            VarTree.ShowRootLines = false;
            VarTree.Indent = 16;
            VarTree.ItemHeight = 14;
            VarTree.MouseDoubleClick += TreeView_DClickMouse;
            VarTree.AfterSelect += TreeView_AfterSelect;
            VarTree.AfterCollapse += Tree_AfterExpandCollapse;
            VarTree.AfterExpand += Tree_AfterExpandCollapse;
            VarTree.Dock = DockStyle.Fill;
            VarTree.BackColor = Color.FromArgb(250, 250, 255);
            VarTree.Cursor = Cursors.Hand;
            VarTab.Padding = new Padding(0, 2, 2, 2);
            VarTab.BackColor = SystemColors.ControlLightLight;
            VarTab.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            VarTab.Controls.Add(VarTree);
        }

        private void SetActiveAreaEvents(TextEditorControl te)
        {
            te.ActiveTextAreaControl.TextArea.MouseDown += delegate(object a1, MouseEventArgs a2) {
                //if (a2.Button == MouseButtons.Left)
                //    Utilities.SelectedTextColorRegion(currentActiveTextAreaCtrl);
                autoComplete.Close();
            };
            te.ActiveTextAreaControl.TextArea.KeyUp += TextArea_KeyUp;
            te.ActiveTextAreaControl.TextArea.KeyPress += TextArea_KeyPressed;
            te.ActiveTextAreaControl.TextArea.MouseEnter += TextArea_SetFocus;
            te.ActiveTextAreaControl.TextArea.PreviewKeyDown += TextArea_PreviewKeyDown;
            te.ActiveTextAreaControl.TextArea.DragDrop += TextEditorDragDrop;

            te.ActiveTextAreaControl.VScrollBar.ValueChanged += VScrollBar_ValueChanged;
            //te.ActiveTextAreaControl.TextArea.MouseWheel += TextArea_MouseWheel;
            //te.ActiveTextAreaControl.VScrollBar.Scroll += delegate(object sender, ScrollEventArgs e) {
            //    var e1 = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, e.OldValue - e.NewValue);
            //    TextArea_MouseWheel(sender, e1);
            //};

            te.ActiveTextAreaControl.TextArea.MouseClick += delegate(object sender, MouseEventArgs e) {
                if (e.Button == MouseButtons.Middle) {
                    Utilities.HighlightingSelectedText(currentActiveTextAreaCtrl);
                    currentTab.textEditor.Refresh();
                } else if (toolTips.Active && e.Button == MouseButtons.Left) {
                     ToolTipsHide();
                }
            };
            te.ActiveTextAreaControl.TextArea.ToolTipRequest += new ToolTipRequestEventHandler(TextArea_ToolTipRequest);
            te.ActiveTextAreaControl.Caret.PositionChanged += new EventHandler(Caret_PositionChanged);
            te.ActiveTextAreaControl.TextArea.MouseDoubleClick += new MouseEventHandler(TextArea_MouseDoubleClick);
        }

        void TextArea_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            Utilities.SelectedTextColorRegion(currentActiveTextAreaCtrl.Caret.Position, currentActiveTextAreaCtrl);
        }

        bool setOnlyOnce = false;

        private void EnableFormControls()
        {
            TabClose_button.Visible = true;
            Split_button.Visible = true;
            splitDocumentToolStripMenuItem.Enabled = true;
            openAllIncludesScriptToolStripMenuItem.Enabled = true;
            GotoProc_StripButton.Enabled = true;
            //Search_toolStripButton.Enabled = true;
            CommentStripButton.Enabled = true;
            if (Settings.showLog)
                splitContainer1.Panel2Collapsed = false;
            includeFileToCodeToolStripMenuItem.Enabled = true;

            // set buttons position
            if (setOnlyOnce) return;
            setOnlyOnce = true;

            int xLocation = tabControl1.DisplayRectangle.Right;
            TabClose_button.Left = xLocation - TabClose_button.Width + 1;
            TabClose_button.Top = tabControl1.DisplayRectangle.Top - 1;

            Split_button.Left = xLocation - Split_button.Width;
            Split_button.Top = tabControl1.DisplayRectangle.Bottom - Split_button.Height;

            minimizelog_button.Left = tabControl2.DisplayRectangle.Right - minimizelog_button.Width + 2;
            minimizelog_button.Top = tabControl2.Top - 1;
        }

        private void ControlFormStateOn_Off()
        {
            autoComplete.Close();

            ShowTabsSpaces();
            ShowLineNumbers(null, null);

            if (currentTab.parseInfo != null && currentDocument.FoldingManager.FoldMarker.Count > 0) //currentTab.parseInfo.procs.Length
                Outline_toolStripButton.Enabled = true;
            else
                Outline_toolStripButton.Enabled = false;

            SetBackForwardButtonState();

            if (currentTab.shouldParse) {
                DecIndentStripButton.Enabled = true;
                //CommentStripButton.Enabled = true;
                AlignToLeftToolStripMenuItem.Enabled = true;
                ToggleBlockCommentToolStripMenuItem.Enabled = true;
                formatingCodeToolStripMenuItem.Enabled = true;
            } else {
                DecIndentStripButton.Enabled = false;
                //CommentStripButton.Enabled = false;
                AlignToLeftToolStripMenuItem.Enabled = false;
                ToggleBlockCommentToolStripMenuItem.Enabled = false;
                formatingCodeToolStripMenuItem.Enabled = false;
            }
        }

        // No selected text tabs
        private void SetFormControlsOff() {
            Outline_toolStripButton.Enabled = false;
            splitContainer2.Panel2Collapsed = true;
            TabClose_button.Visible = false;
            openAllIncludesScriptToolStripMenuItem.Enabled = false;
            Split_button.Visible = false;
            splitDocumentToolStripMenuItem.Enabled = false;
            Back_toolStripButton.Enabled = false;
            Forward_toolStripButton.Enabled = false;
            GotoProc_StripButton.Enabled = false;
            //Search_toolStripButton.Enabled = false;
            if (SearchToolStrip.Visible)
                Search_Panel(null, null);
            DecIndentStripButton.Enabled = false;
            CommentStripButton.Enabled = false;
            Text = SSE.Remove(SSE.Length - 2);
            autoComplete.Close();
            includeFileToCodeToolStripMenuItem.Enabled = false;
        }

        private void ApplySettingsTabs(bool alsoFont = false)
        {
            ColorTheme.SetTheme();

            // Apply settings to all open documents
            foreach (TabInfo ct in tabs) {
                ct.textEditor.TextEditorProperties.TabIndent = Settings.tabSize;
                ct.textEditor.TextEditorProperties.IndentationSize = Settings.tabSize;
                if (!String.Equals(Path.GetExtension(ct.filename), ".msg", StringComparison.OrdinalIgnoreCase)) {
                    ct.textEditor.TextEditorProperties.ConvertTabsToSpaces = Settings.tabsToSpaces;
                    ct.textEditor.TextEditorProperties.ShowVerticalRuler = Settings.showVRuler;
                    ct.textEditor.TextEditorProperties.VerticalRulerRow = Settings.tabSize;
                    ct.textEditor.SetHighlighting(ColorTheme.HighlightingScheme);

                    if (alsoFont)
                        Settings.SetTextAreaFont(ct.textEditor);
                    //ct.textEditor.Refresh();
                    ct.textEditor.Document.ExtraWordList.UpdateColor(ct.textEditor.Document);
                } else {
                    ct.textEditor.Encoding = Settings.EncCodePage;
                    ct.textEditor.SetHighlighting(ColorTheme.IsDarkTheme ? "MessageDark" : "Message");
                }
                ct.textEditor.DarkScheme = ColorTheme.IsDarkTheme; //Установка с обновлением параметров.
            }
        }
        #endregion

        // Tooltip for opcodes and macros
        void TextArea_ToolTipRequest(object sender, ToolTipRequestEventArgs e)
        {
            if (currentTab == null || !e.InDocument) return;

            ToolTipRequest.Show(currentTab, currentDocument, e);
        }

        #region Menu control events

        private void recentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = Open_toolStripSplitButton.DropDownItems.Count;
            if (count < 4 || MessageBox.Show("Do you want to clear the list of recent files ?",
                                             "Recent files", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            for (int i = 3; i < count; i++)
                Open_toolStripSplitButton.DropDownItems.RemoveAt(3);

            Settings.ClearRecent();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool p = Settings.enableParser; //save prev.state
            int f = Settings.selectFont;
            (new SettingsDialog()).ShowDialog();

            ApplySettingsTabs(f != Settings.selectFont);
            if (currentTab != null) tabControl1_Selected(null, null);

            if (Settings.enableParser != p && !Settings.enableParser) {
                parserLabel.Text = parseoff;
                foreach (TabInfo t in tabs)
                {
                    t.treeExpand.Clear();
                }
                if (currentTab != null ) {
                    if (ProcTree.Nodes.Count > 0)
                        ProcTree.Nodes[0].Expand();
                    if (tabControl3.TabPages.Count > 2 && !currentTab.parseInfo.parseData) {
                        tabControl3.TabPages.RemoveAt(1); // удалить вкладку Variables если нет данных
                    }
                }
            } else if (Settings.enableParser != p) {
                //parserLabel.Text = "Parser: Get updated parsing data...";
                //parserLabel.ForeColor = Color.Green;
                foreach (TabInfo t in tabs)
                {
                    t.treeExpand.Clear();
                    if (t.shouldParse && t.parseInfo == null || !t.parseInfo.parseData)
                        t.needsParse = true; //for next parsing
                }
                if (currentTab != null) {
                    if (ProcTree.Nodes.Count > 0) {
                        ProcTree.Nodes[0].Expand();
                        ProcTree.Nodes[1].Expand();
                    }
                    if (VarTree.Nodes.Count > 0) {
                        VarTree.Nodes[0].Expand();
                        VarTree.Nodes[1].Expand();
                    }
                    if (tabControl3.TabPages.Count < 3) {
                        CreateTabVarTree();
                    }
                }
            }
            if (Settings.pathHeadersFiles != null) Headers_toolStripSplitButton.Enabled = true;

            autoComplete.Colored = Settings.autocompleteColor;
            autoComplete.UpdateColor();

            MessageFile.UpdateMessageTextLangPath();

            if (Settings.enableParser) ParseScript(1);
        }

        private void compileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (currentTab != null) {
                dgvErrors.Rows.Clear();

                extParserTimer.Stop(); // предотвратить запуск парсера после компиляции
                currentTab.needsParse = false;

                string msg;
                if (Compile(currentTab, out msg)) {
                    Error.ClearParserErrors(currentTab);
                }
                tbOutput.Text = currentTab.buildLog = msg;
            }
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) {
                for (int i = 0; i < tabs.Count; i++)
                {
                    if (tabControl1.GetTabRect(i).Contains(e.Location)) {
                        if (e.Button == MouseButtons.Middle)
                            Close(tabs[i]);
                        else if (e.Button == MouseButtons.Right) {
                            cmsTabControls.Tag = i;

                            foreach (ToolStripItem item in cmsTabControls.Items)
                                item.Visible = true;

                            cmsTabControls.Show(tabControl1, e.Location);
                        }
                        return;
                    }
                }
            }
        }

        private void tabControl2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) {
                for (int i = 3; i < tabControl2.TabPages.Count; i++)
                {
                    if (tabControl2.GetTabRect(i).Contains(e.Location)) {
                        if (e.Button == MouseButtons.Middle) {
                            int stbi = tabControl2.SelectedIndex;
                            if (stbi == i)
                                tabControl2.Hide();
                            tabControl2.TabPages.RemoveAt(i--);
                            if (stbi == i + 1) {
                                tabControl2.SelectedIndex = (stbi == tabControl2.TabCount) ? stbi - 1 : stbi;
                                tabControl2.Show();
                            }
                        } else if (e.Button == MouseButtons.Right) {
                            cmsTabControls.Tag = i ^ 0x10000000;

                            foreach (ToolStripItem item in cmsTabControls.Items)
                                item.Visible = (item.Text == "Close");

                            cmsTabControls.Show(tabControl2, e.Location);
                        }
                        return;
                    }
                }
            }
            else if (e.Button == MouseButtons.Left && minimizeLogSize != 0 )
                minimizelog_button.PerformClick();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(currentTab);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open(null, OpenType.None);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdScripts.ShowDialog() == DialogResult.OK) {
                foreach (string s in ofdScripts.FileNames)
                {
                    Open(s, OpenType.File);
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs(currentTab);
        }

        private void saveAsTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null || Path.GetExtension(currentTab.filepath).ToLowerInvariant() != ".ssl")
                return;

            SaveFileDialog sfdTemplate = new SaveFileDialog();
            sfdTemplate.Title = "Enter file name for script template";
            sfdTemplate.Filter = "Template file|*.ssl";
            string path = Path.Combine(Settings.ResourcesFolder, "templates");
            sfdTemplate.InitialDirectory = path;

            if (sfdTemplate.ShowDialog() == DialogResult.OK) {
                string fname = Path.GetFileName(sfdTemplate.FileName);
                File.WriteAllText(path + "\\" + fname, currentTab.textEditor.Text, System.Text.Encoding.ASCII);
            }
            sfdTemplate.Dispose();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close(tabs[tabControl1.SelectedIndex]);
        }

        private void recentItem_Click(object sender, EventArgs e)
        {
            Open(((ToolStripMenuItem)sender).Text, OpenType.File, recent: true);
        }

        private void Template_Click(object sender, EventArgs e)
        {
            Open(((ToolStripMenuItem)sender).Tag.ToString(), OpenType.File, false, true);
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                if (tabs[i].changed) Save(tabs[i]);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new AboutBox()).ShowDialog();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(".\\docs\\");
        }

        private void massCompileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.outputDir == null) {
                MessageBox.Show("No output path selected.\nPlease select your scripts directory before compiling", "Error");
                return;
            }
            bool option = Settings.ignoreCompPath;
            Settings.ignoreCompPath = false;

            string compileFolder = Settings.solutionProjectFolder;
            if (compileFolder == null) {
                if (Settings.lastMassCompile != null)
                    fbdMassCompile.SelectedPath = Settings.lastMassCompile;

                if (fbdMassCompile.ShowDialog() != DialogResult.OK) return;

                Settings.lastMassCompile = compileFolder = fbdMassCompile.SelectedPath;
            }
            BatchCompiler.CompileFolder(compileFolder);
            Settings.ignoreCompPath = option;
        }

        private void compileAllOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder FullMsg = new StringBuilder();
            dgvErrors.Rows.Clear();
            string msg;
            for (int i = 0; i < tabs.Count; i++) {
                //FullMsg.AppendLine("*** " + tabs[i].filename);
                Compile(tabs[i], out msg, false);
                tabs[i].buildLog = msg;
                FullMsg.AppendLine(msg);
                FullMsg.AppendLine();
            }
            tbOutput.Text = FullMsg.ToString();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab != null)
                currentActiveTextAreaCtrl.TextArea.ClipboardHandler.Cut(null, null);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab != null)
                currentActiveTextAreaCtrl.TextArea.ClipboardHandler.Copy(null, null);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab != null)
                currentActiveTextAreaCtrl.TextArea.ClipboardHandler.Paste(null, null);
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab != null) {
                currentTab.textEditor.Undo();
                if (!currentDocument.UndoStack.CanUndo) {
                    currentTab.changed = false;
                    SetTabTextChange(currentTab.index);
                }
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab != null) {
                if (currentDocument.UndoStack.CanRedo) {
                    currentTab.changed = true;
                    SetTabTextChange(currentTab.index);
                }
                currentTab.textEditor.Redo();
            }
        }

        private void outlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;

            int cline = currentActiveTextAreaCtrl.Caret.Line;
            foreach (FoldMarker fm in currentDocument.FoldingManager.FoldMarker)
            {
                if (cline >= fm.StartLine && cline <= fm.EndLine)
                    continue;
                if (fm.FoldType == FoldType.MemberBody)
                    fm.IsFolded = !fm.IsFolded;
            }
            currentDocument.FoldingManager.NotifyFoldingsChanged(null);
            currentActiveTextAreaCtrl.CenterViewOn(cline, 0);
        }

        private void registerScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null || RegistredScriptDialogShow)
                return;

            if (currentTab.filepath == null) {
                MessageBox.Show("You cannot register an unsaved script.", "Error");
                return;
            }
            string fName = Path.GetExtension(currentTab.filename).ToLowerInvariant();
            if (fName != ".ssl" && fName != ".int") {
                MessageBox.Show("You cannot register this file.", "Error");
                return;
            }
            fName = Path.ChangeExtension(currentTab.filename, "int");
            if (fName.Length > 12) {
                MessageBox.Show("Script file names must be 8 characters or under to be registered.", "Error");
                return;
            }
            if (currentTab.filename.Length >= 2 && string.Compare(currentTab.filename.Substring(0, 2), "gl", true) == 0) {
                if (MessageBox.Show("This script starts with 'gl', and will be treated by sfall as a global script and loaded automatically.\n" +
                                    "If it's being used as a global script, it does not need to be registered.\n" +
                                    "If it isn't, the script should be renamed before registering it.\n" +
                                    "Are you sure you wish to continue?", "Error") != DialogResult.Yes)
                    return;
            }
            if (fName.IndexOf(' ') != -1) {
                MessageBox.Show("Cannot register a script name that contains a space.", "Error");
                return;
            }
            RegisterScript.Registration(fName);
        }

        private void dgvErrors_DoubleClick(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.SelectedCells.Count != 1)
                return;

            Error error = dgv.Rows[dgv.SelectedCells[0].RowIndex].Cells[dgv == dgvErrors ? 3 : 2].Value as Error;
            if (error != null && error.line != -1)
                SelectLine(error.fileName, error.line, false, error.column, error.len);
        }

        private void preprocessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;

            dgvErrors.Rows.Clear();

            string msg;
            bool result = Compile(currentTab, out msg, true, true);
            tbOutput.Text = currentTab.buildLog = msg;
            if (!result) {
                MessageBox.Show("Pre-processed failed! See build tab log.");
                return;
            }

            string file = Compiler.GetPreprocessedFile(currentTab.filename);
            if (file != null)
                Open(file, OpenType.File, false);
            else
                MessageBox.Show("Failed to fetch preprocessed file");
        }

        private void roundtripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null) return;

            if (Settings.userCmdCompile) {
                MessageBox.Show("It is required to turn off the compilation option via a user cmd file.");
                return;
            }
            dgvErrors.Rows.Clear();

            extParserTimer.Stop(); // предотвратить запуск парсера после компиляции
            currentTab.needsParse = false;

            string msg;
            roundTrip = true;
            bool result = Compile(currentTab, out msg, showIcon: false);
            tbOutput.Text = currentTab.buildLog = msg;
            if (result) {
                Open(new Compiler(true).GetOutputPath(currentTab.filepath), OpenType.File, false, clearBuildLog: false);
            }
            roundTrip = false;
        }

        private void editRegisteredScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!RegistredScriptDialogShow) RegisterScript.Registration(null);
        }

        private void associateMsgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null) return;

            if (!Path.GetExtension(currentTab.filename).Equals(".ssl", StringComparison.OrdinalIgnoreCase)) {
                MessageBox.Show(MessageFile.WrongTypeFile, currentTab.filename) ;
                return;
            }

            if (msgAutoOpenEditorStripMenuItem.Checked) {
                MessageEditor msgForm = MessageEditor.MessageEditorInit(currentTab, this);
                if (msgForm != null)
                    msgForm.SendMsgLine += AcceptMsgLine;
            } else
                AssociateMsg(currentTab, true);
        }

        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int i = (int)cmsTabControls.Tag;
            if ((i & 0x10000000) != 0)
                tabControl2.TabPages.RemoveAt(i ^ 0x10000000);
            else
                Close(tabs[i]);
        }

        void GoToLineToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (currentTab == null || goToLine != null) return;
            goToLine = new GoToLine();
            AddOwnedForm(goToLine);
            goToLine.tbLine.Maximum = currentDocument.TotalNumberOfLines;
            goToLine.tbLine.Select(0, 1);
            goToLine.bGo.Click += delegate(object a1, EventArgs a2) {
                TextAreaControl tac = currentActiveTextAreaCtrl;
                tac.Caret.Column = 0;
                tac.Caret.Line = Convert.ToInt32(goToLine.tbLine.Value - 1);
                tac.CenterViewOn(tac.Caret.Line, 0);
                goToLine.tbLine.Select();
            };
            goToLine.FormClosed += delegate(object a1, FormClosedEventArgs a2) { goToLine = null; };
            goToLine.Show();
        }

        void UPPERCASEToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected)
                new ICSharpCode.TextEditor.Actions.ToUpperCase().Execute(currentActiveTextAreaCtrl.TextArea);
        }

        void LowecaseToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected)
                new ICSharpCode.TextEditor.Actions.ToLowerCase().Execute(currentActiveTextAreaCtrl.TextArea);
        }

        private void ToggleBlockCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;
            if (currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected) {
                new ICSharpCode.TextEditor.Actions.ToggleBlockComment().Execute(
                    currentActiveTextAreaCtrl.TextArea);
            }
        }

        private void capitalizeCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;
            if (currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected)
                new ICSharpCode.TextEditor.Actions.CapitalizeAction().Execute(
                    currentActiveTextAreaCtrl.TextArea);
        }

        private void allTabsSpacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;
            new ICSharpCode.TextEditor.Actions.ConvertTabsToSpaces().Execute(currentActiveTextAreaCtrl.TextArea);
        }

        private void leadingTabsSpacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;
            new ICSharpCode.TextEditor.Actions.ConvertLeadingTabsToSpaces().Execute(currentActiveTextAreaCtrl.TextArea);
        }

        private void showTabsAndSpacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.showTabsChar = showTabsAndSpacesToolStripMenuItem.Checked;
            ShowTabsSpaces();
        }

        private void trailingSpacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.autoTrailingSpaces = trailingSpacesToolStripMenuItem.Checked;
        }

        void CloseAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            for (int i = tabs.Count - 1; i >= 0; i--)
                Close(tabs[i]);
        }

        void CloseAllButThisToolStripMenuItemClick(object sender, EventArgs e)
        {
            int thisIndex = (int)cmsTabControls.Tag;
            for (int i = tabs.Count - 1; i >= 0; i--)
            {
                if (i != thisIndex)
                    Close(tabs[i]);
            }
        }

        void TextEditorDragDrop(object sender, DragEventArgs e)
        {
            if (e.Effect != DragDropEffects.Link) return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                Open(file, OpenType.File);
            }
            Activate();
        }

        void TextEditorDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
        }

        private void minimize_log_button_Click(object sender, EventArgs e)
        {
            if (minimizeLogSize == 0) {
                minimizeLogSize = splitContainer1.SplitterDistance;
                splitContainer1.SplitterDistance = Size.Height;
                Settings.editorSplitterPosition = minimizeLogSize;
            } else {
                int hs = Size.Height - (Size.Height / 4);
                if (Settings.editorSplitterPosition == -1)
                    Settings.editorSplitterPosition = hs;
                if (minimizeLogSize > (hs + 100))
                    splitContainer1.SplitterDistance = hs;
                else
                    splitContainer1.SplitterDistance = Settings.editorSplitterPosition;
                minimizeLogSize = 0;
            }
        }

        private void showLogWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !(Settings.showLog = showLogWindowToolStripMenuItem.Checked);
        }

        private void Headers_toolStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            if (Settings.pathHeadersFiles == null || !Directory.Exists(Settings.pathHeadersFiles)) {
                MessageBox.Show("The headers directory does not exist. Check the correctness of the path setting.");
                return;
            }

            Headers Headfrm = new Headers(Headers_toolStripSplitButton.Bounds.Location);
            if (currentTab != null)
                Headfrm.Tag = currentActiveTextAreaCtrl;
            Headfrm.SelectHeaderFile += delegate(string sHeaderfile) {
                if (sHeaderfile != null)
                    Open(sHeaderfile, OpenType.File, false);
            };
            Headfrm.Show();
        }

        private void openHeaderFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofdHeaders = new OpenFileDialog();
            ofdHeaders.Title = "Select header files to open";
            ofdHeaders.Filter = "Header files|*.h";
            ofdHeaders.Multiselect = true;
            ofdHeaders.RestoreDirectory = true;
            ofdHeaders.InitialDirectory = Settings.pathHeadersFiles;
            if (ofdHeaders.ShowDialog() == DialogResult.OK) {
                foreach (string s in ofdHeaders.FileNames)
                {
                    Open(s, OpenType.File, false);
                }
            }
            ofdHeaders.Dispose();
        }

        private void openIncludesScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab.filepath != null) {
                foreach (string s in ParserInternal.GetAllIncludes(currentTab))
                {
                    Open(s, OpenType.File, addToMRU: false, seltab: false);
                }
            }
        }

        private void SplitDoc_Click(object sender, EventArgs e)
        {
            if (currentTab != null) {
                currentTab.textEditor.Split();
                if (!SplitEvent) {
                    SplitEvent = true;
                    SetActiveAreaEvents(currentTab.textEditor);
                }
                TextArea_SetFocus(null, null);
            }
        }

        private void ShowLineNumbers(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;

            PosChangeType = PositionType.AddPos;

            if (!currentTab.shouldParse) { // for not ssl files
                PosChangeType = PositionType.Disabled;
                splitContainer2.Panel2Collapsed = true;
            } else if (browserToolStripMenuItem.Checked)
                    splitContainer2.Panel2Collapsed = false;

            if (Path.GetExtension(currentTab.filename).ToLowerInvariant() != ".msg") {
                currentDocument.TextEditorProperties.ShowLineNumbers = textLineNumberToolStripMenuItem.Checked;
                currentTab.textEditor.Refresh();
                tsmMessageTextChecker.Enabled = false;
            } else
                tsmMessageTextChecker.Enabled = true;
        }

        private void EncodingMenuItem_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Tag != null /*&& ((ToolStripMenuItem)sender).Tag.ToString() == "dos"*/) {
                EncodingDOSmenuItem.Checked = true;
                windowsDefaultMenuItem.Checked = false;
                Settings.encoding = (byte)EncodingType.OEM866;
            } else {
                EncodingDOSmenuItem.Checked = false;
                windowsDefaultMenuItem.Checked = true;
                Settings.encoding = (byte)EncodingType.Default;
            }
            Settings.EncCodePage = (Settings.encoding == (byte)EncodingType.OEM866) ? Encoding.GetEncoding("cp866") : Encoding.Default;
            ApplySettingsTabs();
        }

        private void defineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.allowDefine = defineToolStripMenuItem.Checked;
        }

        private void DecIndentStripButton_Click(object sender, EventArgs e)
        {
            Utilities.DecIndent(currentActiveTextAreaCtrl);
        }

        private void CommentStripButton_Click(object sender, EventArgs e)
        {
            new ICSharpCode.TextEditor.Actions.ToggleComment().Execute(currentActiveTextAreaCtrl.TextArea);
        }

        private void CommentTextStripButton_Click(object sender, EventArgs e)
        {
            Utilities.CommentText(currentActiveTextAreaCtrl);
        }

        private void UnCommentTextStripButton_Click(object sender, EventArgs e)
        {
            Utilities.UnCommentText(currentActiveTextAreaCtrl);
        }

        private void AlignToLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utilities.AlignToLeft(currentActiveTextAreaCtrl);
        }

        private void highlightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utilities.HighlightingSelectedText(currentActiveTextAreaCtrl);
            currentTab.textEditor.Refresh();
        }

        private void msgFileEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!msgAutoOpenEditorStripMenuItem.Checked && currentTab != null) {
                MessageEditor msgForm = MessageEditor.MessageEditorInit(currentTab, this);
                if (msgForm != null) {
                    msgForm.SendMsgLine += AcceptMsgLine;
                    return;
                }
            }
            MessageEditor.MessageEditorInit(null, this);
        }

        private void pDefineStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
             Settings.preprocDef = (pDefineStripComboBox.SelectedIndex > 0)
                                     ? pDefineStripComboBox.SelectedItem.ToString()
                                     : null;
            if (currentTab != null)
                this.Text = SSE + currentTab.filepath + ((Settings.preprocDef != null)
                                                        ? " [" + Settings.preprocDef + "]" : "");
        }

        private void FunctionsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (e.Node.Tag != null && currentTab != null) {
                if (!Functions.NodeHitCheck(e.Location, e.Node.Bounds))
                    return;

                string code = e.Node.Tag.ToString();
                int pos = code.IndexOf("<cr>");
                if (pos != -1) {
                    string space = new string(' ', currentActiveTextAreaCtrl.Caret.Column);
                    code = code.Replace("<cr>", Environment.NewLine + space);
                }
                if (!currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected) {
                    char c = currentDocument.GetCharAt(currentActiveTextAreaCtrl.Caret.Offset - 1);
                    if (char.IsLetterOrDigit(c)) code = " " + code;

                    if (pos == -1) {
                        c = currentDocument.GetCharAt(currentActiveTextAreaCtrl.Caret.Offset);
                        if (char.IsLetterOrDigit(c)) code += " ";
                    }
                }
                var line = currentActiveTextAreaCtrl.Caret.Position;
                currentActiveTextAreaCtrl.TextArea.InsertString(code);
                // вернуть позицию строки
                currentActiveTextAreaCtrl.Caret.Line = line.Line;
                // установить курсор на начало списка аргументов
                pos = code.IndexOf('{');
                if (pos != -1) {
                    currentActiveTextAreaCtrl.Caret.Column = line.Column + pos;
                }
            } else if (Functions.NodeHitCheck(e.Location, e.Node.Bounds))
                        e.Node.Toggle();
        }

        private void FunctionTree_MouseMove(object sender, MouseEventArgs e)
        {
            var treeView = (TreeView)sender;
            TreeNode node = treeView.GetNodeAt(e.Location);
            if (node != null && node.Tag != null && Functions.NodeHitCheck(e.Location, node.Bounds))
                node.TreeView.Cursor = Cursors.Hand;
            else if (treeView.Cursor != Cursors.Default)
                treeView.Cursor = Cursors.Default;
        }

        private void addUserFunctionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Functions.AddFunction(FunctionTreeLeft.SelectedNode);
        }

        private void editDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Functions.EditFunction(FunctionTreeLeft.SelectedNode);
        }

        private void cmsFunctions_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            addUserFunctionToolStripMenuItem.Enabled = false;
            editFunctionToolStripMenuItem.Enabled = false;
            addTreeNodeToolStripMenuItem.Enabled = false;
            renameTreeNodeToolStripMenuItem.Enabled = false;
            deleteNodeFuncToolStripMenuItem.Enabled = false;

            var node = FunctionTreeLeft.SelectedNode;
            if (node != null) {
                if (node.Tag != null)
                    editFunctionToolStripMenuItem.Enabled = true;

                if (Functions.IsUserFunction(node)) {
                    addUserFunctionToolStripMenuItem.Enabled = true;
                    if (node.Tag == null) {
                        if (node.Level < 2)
                            addTreeNodeToolStripMenuItem.Enabled = true;
                        renameTreeNodeToolStripMenuItem.Enabled = true;
                    }
                    if (node.Level > 0 && (node.Nodes.Count == 0 || node.Tag != null))
                        deleteNodeFuncToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void addTreeNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Functions.AddNode(FunctionTreeLeft.SelectedNode);
        }

        private void renameTreeNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Functions.RenameNode(FunctionTreeLeft.SelectedNode);
        }

        private void deleteNodeFuncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Functions.DeleteNode(FunctionTreeLeft.SelectedNode);
        }

        private void FunctionButton_Click(object sender, EventArgs e)
        {
            Control activeFocus = FindFocus(this.ActiveControl);

            splitContainer3.Hide();

            if (fuctionPanel > 0) {
                splitContainer3.Panel1Collapsed = true;
                fuctionPanel = 0;
            } else {
                if (fuctionPanel == -1) {
                    Functions.CreateTree(FunctionTreeLeft);
                    splitContainer3.Panel2MinSize = 900;
                    splitContainer3.SplitterDistance = 220;
                    fuctionPanel = 220;
                }
                splitContainer3.Panel1Collapsed = false;
                fuctionPanel = splitContainer3.SplitterDistance;
            }

            splitContainer3.Show();
            if (activeFocus != null)
                activeFocus.Select();
        }

        private void funcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FunctionButton.PerformClick();
        }

        private void browserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null ||!currentTab.shouldParse)
                return;
            splitContainer2.Panel2Collapsed = !browserToolStripMenuItem.Checked;
        }

        private void formatingCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utilities.FormattingCode(currentActiveTextAreaCtrl);
        }

        private void GoBeginStripButton_Click(object sender, EventArgs e)
        {
            currentTab.textEditor.BeginUpdate();
            int beginLine = 1;
            foreach (FoldMarker fm in currentDocument.FoldingManager.FoldMarker) {
                if (fm.FoldType == FoldType.Region) {
                    beginLine = fm.StartLine + 1;
                    break;
                }
            }
            SelectLine(currentTab.filepath, beginLine);
            currentActiveTextAreaCtrl.SelectionManager.ClearSelection();
            currentTab.textEditor.EndUpdate();
        }

        void TextArea_SetFocus(object sender, EventArgs e)
        {
            if (!this.ContainsFocus || SearchTextComboBox.Focused || ReplaceTextBox.Focused)
                return;

            if (autoComplete.ShiftCaret) {
                autoComplete.ShiftCaret = false;
                currentActiveTextAreaCtrl.Caret.Position = currentDocument.OffsetToPosition(autoComplete.WordPosition.Key);
                currentActiveTextAreaCtrl.Caret.UpdateCaretPosition();
            }
            currentActiveTextAreaCtrl.TextArea.Focus();
            currentActiveTextAreaCtrl.TextArea.Select();
        }

        private void ViewArgsStripButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNames();
        }

        private void SearchToolStrip_Resize(object sender, EventArgs e)
        {
            int w = ((ToolStrip)sender).Width;
            int size = (w / 2) - 150;
            SearchTextComboBox.Width = size + 50;
            ReplaceTextBox.Width = size;
        }

        private void ParsingErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parsingErrors = ParsingErrorsToolStripMenuItem.Checked;
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", "/n, /select, " + tabs[(int)cmsTabControls.Tag].filepath);
        }

        private void tsmiClearAllLog_Click(object sender, EventArgs e)
        {
            dgvErrors.Rows.Clear();
            if (currentTab != null) {
                currentTab.buildErrors.Clear();
                currentTab.parserErrors.Clear();
            }
        }

        private void tsmCopyLogText_Click(object sender, EventArgs e)
        {
            if (dgvErrors.Rows.Count > 0 && dgvErrors.CurrentCell != null)
                Clipboard.SetText(dgvErrors.CurrentCell.Value.ToString(), TextDataFormat.Text);
        }

        private void RefreshLog_Click(object sender, EventArgs e)
        {
            if (currentTab != null) OutputErrorLog(currentTab);
        }

        private void showIndentLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.showVRuler = showIndentLineToolStripMenuItem.Checked;
            ApplySettingsTabs();
        }

        private void saveUTF8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.saveScriptUTF8 = saveUTF8ToolStripMenuItem.Checked;
        }

        private void tsmMessageTextChecker_Click(object sender, EventArgs e)
        {
            List<Error> report = MessageStructure.CheckStructure(currentActiveTextAreaCtrl, currentTab.filepath);
            if (currentTab.parserErrors.Count > 0 || report.Count > 0)
                dgvErrors.Rows.Clear();

            foreach (Error err in report)
                dgvErrors.Rows.Add(err.type.ToString(), Path.GetFileName(err.fileName), err.line, err);

            if (report.Count > 0) {
                currentTab.parserErrors = report;
                tabControl2.SelectedIndex = 2;
                MaximizeLog();
            } else
                MessageBox.Show("No mistakes!", "Checker");
        }

        private void FontSizeStripStatusLabel_Click(object sender, EventArgs e)
        {
            if (ctrlKeyPress) {
                if (--Settings.sizeFont < -5) Settings.sizeFont = 20;
            } else {
                if (++Settings.sizeFont > 20) Settings.sizeFont = -5;
            }
            SizeFontToString();
        }

        private void decompileF1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.decompileF1 = decompileF1ToolStripMenuItem.Checked;
        }

        private void win32RenderTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.winAPITextRender = win32RenderTextToolStripMenuItem.Checked;
        }

        private void caretModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Caret.GraphicsMode = (caretSoftwareModeToolStripMenuItem.Checked) ? ImplementationMode.SoftwareMode : ImplementationMode.Win32Mode;
            foreach (var tb in tabs) {
                tb.textEditor.ActiveTextAreaControl.Caret.RecreateGraphicsMode();
            }
        }

        private void openInExternalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.OpenInExternalEditor(tabs[(int)cmsTabControls.Tag].filepath);
        }

        private void includeFileToCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.pathHeadersFiles == null || !Directory.Exists(Settings.pathHeadersFiles)) {
                MessageBox.Show("The headers directory does not exist. Check the correctness of the path setting.");
                return;
            }

            Headers Headfrm = new Headers(Headers_toolStripSplitButton.Bounds.Location);
            Headfrm.SelectHeaderFile += delegate(string sHeaderfile)
            {
                Utilities.PasteIncludeFile(sHeaderfile, currentActiveTextAreaCtrl);
            };
            Headfrm.Show();
        }

        private void oldDecompileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.oldDecompile = oldDecompileToolStripMenuItem.Checked;
        }

        private void convertHexDecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected) return;
            bool isConvert = false;

            string text = currentActiveTextAreaCtrl.SelectionManager.SelectedText;
            if (text.IndexOf("0x", StringComparison.CurrentCultureIgnoreCase) != -1) {
                try {
                    text = Convert.ToInt32(text, 16).ToString(); // hex -> dec
                    isConvert = true;
                } catch (Exception) {}
            } else {
                int value;
                if (int.TryParse(text, out value)) {
                    if (value > 0) {
                        int offs = currentActiveTextAreaCtrl.SelectionManager.SelectionCollection[0].Offset;
                        if (offs >= 0 && currentDocument.GetCharAt(offs - 1) == '-') {
                            value = -value;
                            ISelection sp = currentActiveTextAreaCtrl.SelectionManager.SelectionCollection[0];
                            sp.StartPosition = new TextLocation(sp.StartPosition.Column - 1, sp.StartPosition.Line);
                            currentActiveTextAreaCtrl.SelectionManager.SetSelection(sp);
                        }
                    }
                    text = "0x" + Convert.ToString(value, 16).ToUpper(); // dec -> hex
                    isConvert = true;
                }
            }
            if (isConvert) {
                ISelection sel = currentActiveTextAreaCtrl.SelectionManager.SelectionCollection[0];
                currentDocument.Replace(sel.Offset, sel.Length, text);
                currentActiveTextAreaCtrl.TextArea.Caret.Column = sel.StartPosition.Column;
                currentActiveTextAreaCtrl.SelectionManager.ClearSelection();
            }
        }

        private void tsmSetProjectFolder_Click(object sender, EventArgs e)
        {
            if (fbdProjectFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Settings.solutionProjectFolder = fbdProjectFolder.SelectedPath;
                SetProjectFolderText();
            }
        }

        private void tslProject_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Settings.solutionProjectFolder)) {
                System.Diagnostics.Process.Start("explorer", Settings.solutionProjectFolder);
            } else {
                MessageBox.Show("The project folder or path does not exist.", "Error",  MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbUpdateParserData_Click(object sender, EventArgs e)
        {
            if (currentTab == null) return;
            ParseScript(0);
        }
        #endregion
    }
}