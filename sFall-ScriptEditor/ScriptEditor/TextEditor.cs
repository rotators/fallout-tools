using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;
using ScriptEditor.SyntaxRules;

using ScriptEditor.TextEditorUI;
using ScriptEditor.TextEditorUI.Function;
using ScriptEditor.TextEditorUI.CompleteList;
using ScriptEditor.TextEditorUI.ToolTips;

using ScriptEditor.TextEditorUtilities;

namespace ScriptEditor
{
    partial class TextEditor : Form
    {
        private static readonly string AppTitle = AboutBox.appName + AboutBox.appDescription + " v." + AboutBox.appVersion;
        private static readonly string SSE = AppTitle + " - ";

        private const string unsaved = "unsaved.ssl";
        private const string treeTipProcedure = "\n\n - Click and hold Ctrl key to paste the procedure name into the script.\n - Double click to goto the procedure.";
        private const string treeTipVariable = "\n\n - Click and hold Ctrl key to paste the variable name into the script.\n - Double click to goto the variable.";

        private static readonly string[] TREEPROCEDURES = new string[] { "Global Procedures", "Local Procedures" };
        private static readonly string[] TREEVARIABLES = new string[] { "Global Variables", "Script Variables" };
        private static readonly System.Media.SoundPlayer DontFind = new System.Media.SoundPlayer(Properties.Resources.DontFind);
        private static readonly System.Media.SoundPlayer CompileFail = new System.Media.SoundPlayer(Properties.Resources.CompileError);

        private readonly List<TabInfo> tabs = new List<TabInfo>();
        private readonly Dictionary<TabPage, TabInfo> documentTabs = new Dictionary<TabPage, TabInfo>();
        private TabInfo currentTab;
        private ToolStripLabel parserLabel;
        private ToolStripMenuItem collapseAllProceduresMenuItem;
        private ToolStripMenuItem expandAllProceduresMenuItem;
        private ToolStripMenuItem collapseOtherProceduresMenuItem;
        private ToolStripMenuItem toggleActiveProcedureMenuItem;
        private ToolStripMenuItem toolbarToggleActiveProcedureMenuItem;
        private ToolStripMenuItem goToMessageMenuItem;
        private ToolStripMenuItem previewDialogContextMenuItem;
        private ContextMenuStrip fontSizeStatusMenu;
        private ToolStripMenuItem zoomOutMenuItem;
        private ToolStripMenuItem zoomInMenuItem;
        private ToolStripMenuItem resetZoomMenuItem;
        private readonly Dictionary<int, ToolStripMenuItem> zoomMenuItems = new Dictionary<int, ToolStripMenuItem>();
        private Procedure previewDialogContextProcedure;
        private int editorContextLine = -1;
        private TextLocation editorContextPosition = TextLocation.Empty;

        private SearchForm sf;
        private GoToLine goToLine;

        private sealed class EmptyTabStripMessageFilter : IMessageFilter
        {
            private readonly Func<Point, bool> isEmptyTabStrip;
            private readonly Action createEmptyTab;
            private int lastClickTick = -1;
            private Point lastClickPosition;

            public EmptyTabStripMessageFilter(Func<Point, bool> isEmptyTabStrip, Action createEmptyTab)
            {
                this.isEmptyTabStrip = isEmptyTabStrip;
                this.createEmptyTab = createEmptyTab;
            }

            public bool PreFilterMessage(ref Message message)
            {
                if (message.Msg != 0x0201 && message.Msg != 0x0203)
                    return false;

                Point screenPoint = Cursor.Position;
                if (!isEmptyTabStrip(screenPoint))
                {
                    lastClickTick = -1;
                    return false;
                }

                int tick = Environment.TickCount;
                bool isDoubleClick = message.Msg == 0x0203 || (lastClickTick >= 0
                    && unchecked(tick - lastClickTick) <= SystemInformation.DoubleClickTime
                    && Math.Abs(screenPoint.X - lastClickPosition.X) <= SystemInformation.DoubleClickSize.Width
                    && Math.Abs(screenPoint.Y - lastClickPosition.Y) <= SystemInformation.DoubleClickSize.Height);
                if (isDoubleClick)
                {
                    lastClickTick = -1;
                    createEmptyTab();
                }
                else
                {
                    lastClickTick = tick;
                    lastClickPosition = screenPoint;
                }
                return false;
            }
        }
        private sealed class NavigationLocation
        {
            public string FilePath;
            public TextLocation Position;
        }

        private sealed class ClosedDocumentLocation
        {
            public string FilePath;
            public TextLocation Position;
        }

        private const int NavigationHistoryLimit = 100;
        private const int RecentlyClosedDocumentLimit = 20;
        private readonly List<NavigationLocation> navigationHistory = new List<NavigationLocation>();
        private readonly List<ClosedDocumentLocation> recentlyClosedDocuments = new List<ClosedDocumentLocation>();
        private int navigationHistoryIndex = -1;

        private TabInfo previousTab;
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
        private bool isClosing = false;
        private bool startupRestorationInProgress = true;
        private Timer statusMessageTimer;
        private Timer unsavedRecoveryTimer;
        private int unsavedRecoveryLastChangeTick = -1;
        private int unsavedRecoveryPendingSinceTick = -1;
        private EmptyTabStripMessageFilter emptyTabStripMessageFilter;

        private const int UnsavedRecoveryIdleDelayMilliseconds = 3000;
        private const int UnsavedRecoveryMaximumDelayMilliseconds = 30000;

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
           dgvErrors.Scroll += dgvErrors_Scroll;
        }

        #region Main form control
        public TextEditor(string[] args)
        {
            InitializeComponent();
            emptyTabStripMessageFilter = new EmptyTabStripMessageFilter(IsEmptyDocumentTabStrip, CreateEmptyDocumentFromTabStrip);
            Application.AddMessageFilter(emptyTabStripMessageFilter);
            FormClosed += delegate { Application.RemoveMessageFilter(emptyTabStripMessageFilter); };
            // The form is created by Application.Run immediately after this constructor.
            // Keep its partially initialized controls out of the first visible frame.
            Opacity = 0D;
            ConfigureHelpMenu();
            ConfigureEditorFoldingMenu();
            ConfigureToolbarFoldingMenu();
            ConfigureMessageNavigationMenu();
            ConfigureDialogPreviewContextMenu();
            ConfigureMainToolbar();
            ConfigureStatusNotifications();
            ConfigureUnsavedDocumentRecovery();
            ConfigureStatusZoomMenu();

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
            ViewArgsStripButton.Checked = Settings.showProcedureArguments;
            SizeFontToString();

            if (Directory.Exists(Settings.lastOpenScriptsFolder))
                ofdScripts.InitialDirectory = Settings.lastOpenScriptsFolder;
            else if (Directory.Exists(Settings.solutionProjectFolder))
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

            this.Text = AppTitle;
            tbOutput.Text = "***** " +  AboutBox.appName + " v." + AboutBox.appVersion + AboutBox.appDescription + " *****";
            InterfaceTheme.Apply(this);
            ColorTheme.ApplyRightPanelTheme();
        }

        internal void RefreshDescriptionLanguage()
        {
            const int builtInProcedureMenuItemCount = 7;
            while (ProcMnContext.Items.Count > builtInProcedureMenuItemCount)
                ProcMnContext.Items.RemoveAt(ProcMnContext.Items.Count - 1);
            HandlerProcedure.CreateProcHandlers(ProcMnContext, this);

            FunctionsTree.Nodes.Clear();
            Functions.CreateTree(FunctionsTree);
            if (fuctionPanel != -1) {
                FunctionTreeLeft.Nodes.Clear();
                Functions.CreateTree(FunctionTreeLeft);
            }

            ProgramInfo.LoadOpcodes();
        }

        internal void RefreshDescriptionOptions()
        {
            if (!Settings.showTips)
                ToolTipsHide();
            ProgramInfo.LoadOpcodes();
        }

        private void ConfigureHelpMenu()
        {
            ToolStripMenuItem repositoryItem = new ToolStripMenuItem("Rotators Tools Repository");
            repositoryItem.Click += delegate { AboutBox.OpenRepository(this); };

            ToolStripMenuItem scriptingDocumentationItem = new ToolStripMenuItem("Sfall scripting documentation");
            scriptingDocumentationItem.Click += delegate { AboutBox.OpenSfallDocumentation(this); };

            Help_toolStripButton.DropDownItems.Add(new ToolStripSeparator());
            Help_toolStripButton.DropDownItems.Add(repositoryItem);
            Help_toolStripButton.DropDownItems.Add(scriptingDocumentationItem);
        }

        private void ConfigureEditorFoldingMenu()
        {
            ToolStripMenuItem foldingMenu = new ToolStripMenuItem("Code folding");
            collapseAllProceduresMenuItem = new ToolStripMenuItem("Collapse all procedures");
            expandAllProceduresMenuItem = new ToolStripMenuItem("Expand all procedures");
            collapseOtherProceduresMenuItem = new ToolStripMenuItem("Collapse all except active procedure");
            toggleActiveProcedureMenuItem = new ToolStripMenuItem("Collapse active procedure");

            collapseAllProceduresMenuItem.Click += CollapseAllProcedures_Click;
            expandAllProceduresMenuItem.Click += ExpandAllProcedures_Click;
            collapseOtherProceduresMenuItem.Click += CollapseOtherProcedures_Click;
            toggleActiveProcedureMenuItem.Click += ToggleActiveProcedure_Click;

            foldingMenu.DropDownItems.Add(collapseAllProceduresMenuItem);
            foldingMenu.DropDownItems.Add(expandAllProceduresMenuItem);
            foldingMenu.DropDownItems.Add(new ToolStripSeparator());
            foldingMenu.DropDownItems.Add(collapseOtherProceduresMenuItem);
            foldingMenu.DropDownItems.Add(toggleActiveProcedureMenuItem);
            editorMenuStrip.Items.Add(new ToolStripSeparator());
            editorMenuStrip.Items.Add(foldingMenu);
        }

        private void ToggleActiveProcedure_Click(object sender, EventArgs e)
        {
            if (currentTab == null) return;
            bool folded;
            int line = object.ReferenceEquals(sender, toolbarToggleActiveProcedureMenuItem)
                ? currentActiveTextAreaCtrl.Caret.Line : EditorContextLine;
            CodeFolder.TryToggleProcedureAtLine(currentDocument, line, out folded);
        }

        private void UpdateActiveProcedureFoldingMenu(int line)
        {
            bool folded = false;
            bool hasProcedure = currentTab != null && CodeFolder.TryGetProcedureFoldedAtLine(currentDocument, line, out folded);
            string text = hasProcedure && folded ? "Expand active procedure" : "Collapse active procedure";
            if (toggleActiveProcedureMenuItem != null) {
                toggleActiveProcedureMenuItem.Enabled = hasProcedure;
                toggleActiveProcedureMenuItem.Text = text;
            }
            if (toolbarToggleActiveProcedureMenuItem != null) {
                toolbarToggleActiveProcedureMenuItem.Enabled = hasProcedure;
                toolbarToggleActiveProcedureMenuItem.Text = text;
            }
        }
        private void ConfigureToolbarFoldingMenu()
        {
            ToolStripMenuItem collapseAll = new ToolStripMenuItem("Collapse all procedures");
            ToolStripMenuItem expandAll = new ToolStripMenuItem("Expand all procedures");
            ToolStripMenuItem collapseOthers = new ToolStripMenuItem("Collapse all except active procedure");
            toolbarToggleActiveProcedureMenuItem = new ToolStripMenuItem("Collapse active procedure");
            collapseAll.Click += CollapseAllProcedures_Click;
            expandAll.Click += ExpandAllProcedures_Click;
            collapseOthers.Click += CollapseOtherProcedures_Click;
            toolbarToggleActiveProcedureMenuItem.Click += ToggleActiveProcedure_Click;
            Outline_toolStripButton.DropDownItems.Add(collapseAll);
            Outline_toolStripButton.DropDownItems.Add(expandAll);
            Outline_toolStripButton.DropDownItems.Add(new ToolStripSeparator());
            Outline_toolStripButton.DropDownItems.Add(collapseOthers);
            Outline_toolStripButton.DropDownItems.Add(toolbarToggleActiveProcedureMenuItem);
            Outline_toolStripButton.DropDownOpening += delegate { UpdateActiveProcedureFoldingMenu(currentActiveTextAreaCtrl.Caret.Line); };
        }
        private void ConfigureMessageNavigationMenu()
        {
            goToMessageMenuItem = new ToolStripMenuItem("Go to message");
            goToMessageMenuItem.Enabled = false;
            goToMessageMenuItem.Click += GoToMessage_Click;
            editorMenuStrip.Items.Insert(editorMenuStrip.Items.IndexOf(toolStripSeparator6), goToMessageMenuItem);
        }

        private void ConfigureDialogPreviewContextMenu()
        {
            previewDialogContextMenuItem = new ToolStripMenuItem("Preview dialog");
            previewDialogContextMenuItem.Enabled = false;
            previewDialogContextMenuItem.Click += PreviewDialogContextMenuItem_Click;
            editorMenuStrip.Items.Insert(editorMenuStrip.Items.IndexOf(toolStripSeparator6), previewDialogContextMenuItem);
        }

        private void UpdateDialogPreviewContextMenu()
        {
            previewDialogContextMenuItem.Enabled = false;
            previewDialogContextProcedure = null;
            if (currentTab == null || currentTab.parseInfo == null || currentTab.filepath == null
                || !Path.GetExtension(currentTab.filepath).Equals(".ssl", StringComparison.OrdinalIgnoreCase))
                return;

            int line = EditorContextLine + 1;
            foreach (Procedure procedure in currentTab.parseInfo.procs) {
                if (procedure == null || line < procedure.d.start || line > procedure.d.end
                    || !String.Equals(procedure.fstart, currentTab.filepath, StringComparison.OrdinalIgnoreCase))
                    continue;

                DialogFunctionsRules.BuildOpcodesDictionary();
                if (DialogueParser.ProcedureContainsPreviewableDialog(currentDocument,
                        currentTab.parseInfo, procedure)) {
                    previewDialogContextProcedure = procedure;
                    previewDialogContextMenuItem.Enabled = true;
                }
                break;
            }
        }

        private void PreviewDialogContextMenuItem_Click(object sender, EventArgs e)
        {
            if (previewDialogContextProcedure != null)
                ShowDialogPreview(previewDialogContextProcedure.name);
        }

        private void UpdateOutlineButtonState()
        {
            if (currentTab == null || currentTab.parseInfo == null) {
                Outline_toolStripButton.Enabled = false;
                Outline_toolStripButton.ToolTipText = "No procedures available";
                return;
            }

            int line = currentActiveTextAreaCtrl.Caret.Line;
            bool hasOtherProcedures = CodeFolder.HasProcedureOutsideLine(currentDocument, line);
            Outline_toolStripButton.Enabled = hasOtherProcedures;
            if (!hasOtherProcedures) {
                Outline_toolStripButton.ToolTipText = "No other procedures available";
                return;
            }

            bool collapse = CodeFolder.HasUnfoldedProcedureOutsideLine(currentDocument, line);
            Outline_toolStripButton.ToolTipText = collapse
                ? "Collapse other procedures"
                : "Expand other procedures";
        }

        private void UpdateGoToMessageMenu()
        {
            goToMessageMenuItem.Enabled = false;
            goToMessageMenuItem.Tag = null;
            KeyValuePair<string, int> target;
            if (!TryGetMessageTarget(EditorContextPosition, out target))
                return;

            goToMessageMenuItem.Tag = target;
            goToMessageMenuItem.Enabled = true;
        }

        private bool TryGetMessageTarget(TextLocation position, out KeyValuePair<string, int> target)
        {
            target = default(KeyValuePair<string, int>);
            if (currentTab == null || currentTab.filepath == null || position == TextLocation.Empty
                || !Path.GetExtension(currentTab.filepath).Equals(".ssl", StringComparison.OrdinalIgnoreCase)
                || position.Line < 0 || position.Line >= currentDocument.TotalNumberOfLines)
                return false;

            LineSegment segment = currentDocument.GetLineSegment(position.Line);
            string lineText = currentDocument.GetText(segment.Offset, segment.Length);
            int preferredOffset = currentDocument.PositionToOffset(position);
            var candidates = new List<KeyValuePair<int, int>>();
            foreach (Match match in Regex.Matches(lineText, @"(?<![A-Za-z_])[0-9]+(?![A-Za-z_])")) {
                int messageNumber;
                if (!int.TryParse(match.Value, out messageNumber))
                    continue;
                int offset = segment.Offset + match.Index;
                var candidate = new KeyValuePair<int, int>(offset, messageNumber);
                if (preferredOffset >= offset && preferredOffset <= offset + match.Length)
                    candidates.Insert(0, candidate);
                else
                    candidates.Add(candidate);
            }

            foreach (var candidate in candidates) {
                string scriptToken;
                if (!ToolTipRequest.TryGetMessageScriptToken(currentTab, currentDocument.TextContent,
                        candidate.Key, candidate.Value, out scriptToken))
                    scriptToken = null;

                string path;
                int physicalLine;
                if (!MessageFile.TryGetMessageLocation(currentTab, scriptToken, candidate.Value,
                        out path, out physicalLine))
                    continue;
                if (String.IsNullOrEmpty(scriptToken))
                    currentTab.msgFilePath = path;
                target = new KeyValuePair<string, int>(path, candidate.Value);
                return true;
            }
            return false;
        }

        private void GoToMessage_Click(object sender, EventArgs e)
        {
            if (!(goToMessageMenuItem.Tag is KeyValuePair<string, int>))
                return;

            NavigateToMessage((KeyValuePair<string, int>)goToMessageMenuItem.Tag);
        }

        private void NavigateToMessage(KeyValuePair<string, int> target)
        {
            TabInfo messageTab = Open(target.Key, OpenType.File, false, alreadyOpen: false);
            if (messageTab == null)
                return;

            int line;
            if (!MessageFile.TryFindMessageLine(messageTab.textEditor.Document.TextContent, target.Value, out line))
                return;
            SelectLine(target.Key, line);
        }

        private void ConfigureMainToolbar()
        {
            ToolStripMain.GripStyle = ToolStripGripStyle.Hidden;
            ToolStripMain.ShowItemToolTips = true;

            ApplyDpiMetrics();
            HandleCreated += delegate { ApplyDpiMetrics(); };
            DpiChanged += delegate { ApplyDpiMetrics(); };

            // File commands form one group; these dividers only added visual noise.
            toolStripSeparator7.Visible = false;
            toolStripSeparator13.Visible = false;
            toolStripSeparator8.Visible = false;
            toolStripSeparator42.Visible = false;

            // Help is right-aligned, so separators beside its old position were redundant.
            toolStripSeparator4.Visible = false;
            toolStripSeparator14.Visible = false;

            tsbSaveAll.ToolTipText = "Save all scripts (Ctrl+Shift+S).";
            GotoProc_StripButton.ToolTipText = "Go to the procedure under the cursor (Alt+P).";
        }

        private void ApplyDpiMetrics()
        {
            ToolStripMain.ImageScalingSize = DpiHelper.Scale(this, new Size(18, 18));
            ToolStripMain.Height = DpiHelper.Scale(this, 32);
            ToolStripMain.Padding = DpiHelper.Scale(this, new Padding(5, 3, 5, 3));
            tabControl1.ItemSize = new Size(0, DpiHelper.Scale(this, 26));
            // Native tab sizing accounts for the label but not our custom status and close glyphs.
            tabControl1.Padding = new Point(DpiHelper.Scale(this, 19), DpiHelper.Scale(this, 4));
            tabControl1.SizeMode = TabSizeMode.Normal;
            tabControl1.ShowDocumentStatusIcons = true;

            foreach (ToolStripItem item in ToolStripMain.Items) {
                if (item is ToolStripSeparator)
                    item.Margin = DpiHelper.Scale(this, new Padding(2, 1, 2, 1));
                else {
                    item.Margin = DpiHelper.Scale(this, new Padding(1, 0, 1, 0));
                    item.Padding = DpiHelper.Scale(this, new Padding(2, 0, 2, 0));
                }
            }

            VarTree.Indent = DpiHelper.Scale(this, 16);
            VarTree.ItemHeight = DpiHelper.Scale(this, 14);
            VarTab.Padding = DpiHelper.Scale(this, new Padding(0, 2, 2, 2));
        }

        private int EditorContextLine
        {
            get {
                if (editorContextLine >= 0)
                    return editorContextLine;
                return currentTab == null ? -1 : currentActiveTextAreaCtrl.Caret.Line;
            }
        }

        private TextLocation EditorContextPosition
        {
            get {
                if (editorContextPosition != TextLocation.Empty)
                    return editorContextPosition;
                return currentTab == null ? TextLocation.Empty : currentActiveTextAreaCtrl.Caret.Position;
            }
        }

        private void CollapseAllProcedures_Click(object sender, EventArgs e)
        {
            if (currentTab == null) return;
            CodeFolder.SetAllProceduresFolded(currentDocument, true);
        }

        private void ExpandAllProcedures_Click(object sender, EventArgs e)
        {
            if (currentTab == null) return;
            CodeFolder.SetAllProceduresFolded(currentDocument, false);
        }

        private void RecordNavigationLocation()
        {
            if (currentTab == null || String.IsNullOrEmpty(currentTab.filepath))
                return;

            NavigationLocation location = new NavigationLocation {
                FilePath = currentTab.filepath,
                Position = currentActiveTextAreaCtrl.Caret.Position
            };
            if (navigationHistoryIndex >= 0) {
                NavigationLocation current = navigationHistory[navigationHistoryIndex];
                if (String.Equals(current.FilePath, location.FilePath, StringComparison.OrdinalIgnoreCase) &&
                    current.Position.Equals(location.Position))
                    return;
            }
            if (navigationHistoryIndex < navigationHistory.Count - 1)
                navigationHistory.RemoveRange(navigationHistoryIndex + 1, navigationHistory.Count - navigationHistoryIndex - 1);
            navigationHistory.Add(location);
            if (navigationHistory.Count > NavigationHistoryLimit)
                navigationHistory.RemoveAt(0);
            navigationHistoryIndex = navigationHistory.Count - 1;
            SetBackForwardButtonState();
        }

        private bool NavigateHistory(int direction)
        {
            int targetIndex = navigationHistoryIndex + direction;
            if (targetIndex < 0 || targetIndex >= navigationHistory.Count)
                return false;

            NavigationLocation target = navigationHistory[targetIndex];
            TabInfo tab = Open(target.FilePath, OpenType.File, false, alreadyOpen: false);
            if (tab == null)
                return false;

            currentActiveTextAreaCtrl.Caret.Position = target.Position;
            currentActiveTextAreaCtrl.CenterViewOn(currentActiveTextAreaCtrl.Caret.Line, 0);
            navigationHistoryIndex = targetIndex;
            SetBackForwardButtonState();
            return true;
        }

        private void RememberClosedDocument(TabInfo tab)
        {
            if (isClosing || tab == null || String.IsNullOrEmpty(tab.filepath) || !File.Exists(tab.filepath))
                return;

            ClosedDocumentLocation location = new ClosedDocumentLocation {
                FilePath = tab.filepath,
                Position = tab.textEditor.ActiveTextAreaControl.Caret.Position
            };
            recentlyClosedDocuments.RemoveAll(item => String.Equals(item.FilePath, location.FilePath, StringComparison.OrdinalIgnoreCase));
            recentlyClosedDocuments.Insert(0, location);
            if (recentlyClosedDocuments.Count > RecentlyClosedDocumentLimit)
                recentlyClosedDocuments.RemoveRange(RecentlyClosedDocumentLimit, recentlyClosedDocuments.Count - RecentlyClosedDocumentLimit);
        }

        private bool ReopenLastClosedDocument()
        {
            while (recentlyClosedDocuments.Count > 0) {
                ClosedDocumentLocation location = recentlyClosedDocuments[0];
                recentlyClosedDocuments.RemoveAt(0);
                if (!File.Exists(location.FilePath))
                    continue;

                TabInfo tab = Open(location.FilePath, OpenType.File, false, alreadyOpen: false);
                if (tab == null)
                    continue;

                currentActiveTextAreaCtrl.Caret.Position = location.Position;
                currentActiveTextAreaCtrl.CenterViewOn(currentActiveTextAreaCtrl.Caret.Line, 0);
                return true;
            }
            return false;
        }


        private void CollapseOtherProcedures_Click(object sender, EventArgs e)
        {
            if (currentTab == null) return;
            int line = EditorContextLine;
            if (CodeFolder.CollapseAllExceptProcedure(currentDocument, line))
                currentActiveTextAreaCtrl.CenterViewOn(line, 0);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Tab) && previousTab != null &&
                previousTab.index >= 0 && previousTab.index < tabControl1.TabPages.Count &&
                previousTab.index != tabControl1.SelectedIndex) {
                tabControl1.SelectTab(previousTab.index);
                return true;
            }
            if (keyData == (Keys.Alt | Keys.Left))
                return NavigateHistory(-1);
            if (keyData == (Keys.Alt | Keys.Right))
                return NavigateHistory(1);
            if (keyData == (Keys.Control | Keys.W) && currentTab != null) {
                Close(currentTab);
                return true;
            }
            if (keyData == (Keys.Control | Keys.Shift | Keys.T))
                return ReopenLastClosedDocument();

            return base.ProcessCmdKey(ref msg, keyData);
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
            tabControl1.Visible = false;
            splitContainer2.Panel1MinSize = 300;
            splitContainer2.Panel2MinSize = 260;
            splitContainer1.SplitterDistance = Size.Height;

            minimizeLogSize = Settings.logPanelCollapsed ? Settings.editorSplitterPosition : 0;

            if (Settings.editorSplitterPosition2 != -1)
                splitContainer2.SplitterDistance = Settings.editorSplitterPosition2;
            else
                splitContainer2.SplitterDistance = Size.Width - 260;

            showLogWindowToolStripMenuItem.Checked = Settings.showLog;
            if (Settings.enableParser)
                CreateTabVarTree();
        }

        private void TextEditor_Shown(object sender, EventArgs e)
        {
            if (!Settings.firstRun)
                Settings_ToolStripMenuItem.PerformClick();

            this.Activated += TextEditor_Activated;
            this.Deactivate += TextEditor_Deactivate;
            SingleInstanceManager.SendEditorOpenMessage();
            InterfaceTheme.Apply(this);
            ColorTheme.ApplyRightPanelTheme();
            Refresh();
            Opacity = 1D;
            if (Settings.IsWindowMaximized(SavedWindows.Main)) {
                WindowState = FormWindowState.Maximized;
                if (Settings.editorSplitterPosition2 != -1)
                    splitContainer2.SplitterDistance = Settings.editorSplitterPosition2;
            }

            // Let native Search & Replace controls complete their first paint while
            // invisible. The reusable dialog can then appear without a light-theme
            // frame when the user presses Ctrl+F for the first time.
            BeginInvoke(new MethodInvoker(delegate {
                if (!IsDisposed && !isClosing) {
                    EnsureSearchForm();
                    sf.Prewarm();
                }
            }));

            // Give Windows one complete painted frame before restoring documents.
            // Session tabs are useful startup state, but they must not delay the shell.
            var startupDocumentsTimer = new Timer { Interval = 50 };
            startupDocumentsTimer.Tick += delegate {
                startupDocumentsTimer.Stop();
                startupDocumentsTimer.Dispose();
                if (!IsDisposed && !isClosing)
                    RestoreStartupDocuments();
            };
            startupDocumentsTimer.Start();
        }

        private void RestoreStartupDocuments()
        {
            RestorePreviousSession(delegate(bool restoredPreviousSession) {
                bool restoredUnsavedSession = Settings.restoreUnsavedChangesOnExit && RestoreUnsavedSession();
                foreach (string fArg in commandsArgs)
                {
                    string file = fArg;
                    bool fcd = FileAssociation.CheckFCDFile(ref file);
                    if (file != null)
                        Open(file, TextEditor.OpenType.File, commandline: true, fcdOpen: fcd);
                }
                startupRestorationInProgress = false;
                tabControl1.Visible = tabControl1.TabPages.Count > 0;
                BeginInvoke(new MethodInvoker(delegate {
                    if (IsDisposed || tabControl1.TabPages.Count == 0)
                        return;
                    Split_button.Visible = !startupRestorationInProgress;
                    PositionEditorCornerButtons();
                }));
            });
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

            if (WindowState != FormWindowState.Minimized) {
                DetectExternalChanges();
                CheckChandedFile();
            }
            else {
                Timer timer = new Timer();
                timer.Interval = 500; // interval time - 0.5 sec
                timer.Tick += delegate(object obj, EventArgs eArg) {
                    timer.Stop();
                    timer.Dispose();
                    DetectExternalChanges();
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
                if (tabs[i].changed && !Settings.restoreUnsavedChangesOnExit) {
                    switch (ScriptEditor.ThemedMessageBox.Show("Save changes to " + tabs[i].filename + "?", "Message", MessageBoxButtons.YesNoCancel)) {
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

            SaveOpenTabSession();
            isClosing = true;
            unsavedRecoveryTimer.Stop();
            if (bwSyntaxParser.IsBusy)
                bwSyntaxParser.CancelAsync();
            splitContainer3.Panel1Collapsed = true;
            if (!splitContainer1.Panel2Collapsed && !Settings.logPanelCollapsed)
                Settings.editorSplitterPosition = splitContainer1.SplitterDistance;
            Settings.editorSplitterPosition2 = splitContainer2.SplitterDistance;
            Settings.SaveSettingData(this);
            SyntaxFile.DeleteSyntaxFile();
        }

        private void ConfigureStatusNotifications()
        {
            statusMessageTimer = new Timer(components);
            statusMessageTimer.Interval = 5000;
            statusMessageTimer.Tick += delegate {
                statusMessageTimer.Stop();
                EmptyStripStatusLabel.Text = String.Empty;
                EmptyStripStatusLabel.ToolTipText = String.Empty;
                EmptyStripStatusLabel.ForeColor = InterfaceTheme.IsDark ? Color.Gainsboro : SystemColors.ControlText;
                EmptyStripStatusLabel.BackColor = InterfaceTheme.IsDark ? Color.FromArgb(53, 53, 56) : SystemColors.Control;
            };
        }

        private void ConfigureUnsavedDocumentRecovery()
        {
            unsavedRecoveryTimer = new Timer(components) { Interval = 500 };
            unsavedRecoveryTimer.Tick += delegate {
                if (isClosing || !Settings.restoreUnsavedChangesOnExit) {
                    unsavedRecoveryTimer.Stop();
                    return;
                }

                int now = Environment.TickCount;
                if (unsavedRecoveryLastChangeTick < 0 || unsavedRecoveryPendingSinceTick < 0) {
                    unsavedRecoveryTimer.Stop();
                    return;
                }

                if (ElapsedMilliseconds(now, unsavedRecoveryLastChangeTick) < UnsavedRecoveryIdleDelayMilliseconds &&
                    ElapsedMilliseconds(now, unsavedRecoveryPendingSinceTick) < UnsavedRecoveryMaximumDelayMilliseconds)
                    return;

                SaveUnsavedDocumentRecovery();
                unsavedRecoveryLastChangeTick = -1;
                unsavedRecoveryPendingSinceTick = -1;
                unsavedRecoveryTimer.Stop();
            };
        }

        private static uint ElapsedMilliseconds(int now, int then)
        {
            return unchecked((uint)(now - then));
        }

        private void RequestUnsavedDocumentRecovery()
        {
            if (isClosing || !Settings.restoreUnsavedChangesOnExit)
                return;

            int now = Environment.TickCount;
            if (unsavedRecoveryPendingSinceTick < 0)
                unsavedRecoveryPendingSinceTick = now;
            unsavedRecoveryLastChangeTick = now;
            unsavedRecoveryTimer.Start();
        }

        private void SaveUnsavedDocumentRecovery()
        {
            if (!Settings.restoreUnsavedChangesOnExit)
                return;

            List<Settings.UnsavedSessionDocument> documents = new List<Settings.UnsavedSessionDocument>();
            int selectedIndex = -1;
            foreach (TabInfo tab in tabs) {
                // Untitled tabs have no file to reopen, so retain them even before their
                // first edit. Saved files need recovery only while they are modified.
                bool untitledDocument = String.IsNullOrWhiteSpace(tab.filepath);
                if (!tab.changed && !untitledDocument)
                    continue;

                if (tab == currentTab)
                    selectedIndex = documents.Count;
                bool savedDocument = !String.IsNullOrWhiteSpace(tab.filepath) && File.Exists(tab.filepath);
                documents.Add(new Settings.UnsavedSessionDocument {
                    Name = tab.filename,
                    FilePath = savedDocument ? tab.filepath : null,
                    Text = tab.textEditor.Text,
                    CaretLine = tab.textEditor.ActiveTextAreaControl.Caret.Line,
                    TabIndex = tab.index
                });
            }

            if (documents.Count == 0)
                Settings.ClearUnsavedSession();
            else
                Settings.SaveUnsavedSession(documents, selectedIndex);
        }
        internal void ShowStatusMessage(string message, NotificationKind kind, int duration)
        {
            if (InvokeRequired) {
                BeginInvoke(new Action<string, NotificationKind, int>(ShowStatusMessage), message, kind, duration);
                return;
            }

            Color back;
            Color fore;
            EditorNotifications.GetColors(kind, InterfaceTheme.IsDark, out back, out fore);
            EmptyStripStatusLabel.Text = EditorNotifications.GetPrefix(kind) + message.Replace('\r', ' ').Replace('\n', ' ');
            EmptyStripStatusLabel.ToolTipText = message;
            EmptyStripStatusLabel.ForeColor = kind == NotificationKind.Information
                ? (InterfaceTheme.IsDark ? Color.FromArgb(130, 190, 235) : Color.FromArgb(0, 90, 160))
                : fore;
            EmptyStripStatusLabel.BackColor = kind == NotificationKind.Information
                ? (InterfaceTheme.IsDark ? Color.FromArgb(53, 53, 56) : SystemColors.Control)
                : back;
            statusMessageTimer.Stop();
            statusMessageTimer.Interval = Math.Max(1000, duration);
            statusMessageTimer.Start();
        }

        private void RestorePreviousSession(Action<bool> completed)
        {
            if (!Settings.reopenLastTabs) {
                completed(false);
                return;
            }

            int selectedIndex;
            string[] paths = Settings.LoadLastSession(out selectedIndex);
            TabInfo selectedTab = null;
            bool restoredAny = false;
            int pathIndex = 0;

            if (paths.Length == 0) {
                completed(false);
                return;
            }

            // Restore one document per UI turn. Individual files still receive their normal
            // initialization, but the shell can repaint and accept input between files.
            var restoreTimer = new Timer { Interval = 1 };
            restoreTimer.Tick += delegate {
                restoreTimer.Stop();
                if (IsDisposed || isClosing) {
                    restoreTimer.Dispose();
                    return;
                }

                while (pathIndex < paths.Length && !File.Exists(paths[pathIndex]))
                    pathIndex++;

                if (pathIndex < paths.Length) {
                    int restoringIndex = pathIndex;
                    TabInfo restored = Open(paths[pathIndex++], OpenType.File, addToMRU: false, seltab: false);
                    restoredAny |= restored != null;
                    if (restoringIndex == selectedIndex)
                        selectedTab = restored;
                }

                if (pathIndex < paths.Length) {
                    restoreTimer.Start();
                    return;
                }

                restoreTimer.Dispose();
                if (selectedTab != null && selectedTab.index >= 0 && selectedTab.index < tabControl1.TabCount)
                    tabControl1.SelectTab(selectedTab.index);
                completed(restoredAny);
            };
            restoreTimer.Start();
        }

        private void MoveTabToIndex(TabInfo tab, int targetIndex)
        {
            if (tab == null || targetIndex < 0)
                return;

            TabPage page = FindDocumentTabPage(tab);
            if (page == null)
                return;

            int currentIndex = tabControl1.TabPages.IndexOf(page);
            if (currentIndex < 0)
                return;

            targetIndex = Math.Min(targetIndex, tabControl1.TabPages.Count - 1);
            if (currentIndex == targetIndex)
                return;

            tabControl1.TabPages.RemoveAt(currentIndex);
            tabControl1.TabPages.Insert(targetIndex, page);
            SynchronizeDocumentTabOrder();
            UpdateDocumentTab(tab.index);
        }

        private bool RestoreUnsavedSession()
        {
            int selectedIndex;
            Settings.UnsavedSessionDocument[] documents = Settings.LoadUnsavedSession(out selectedIndex);
            TabInfo selectedTab = null;
            bool restoredAny = false;
            for (int i = 0; i < documents.Length; i++) {
                Settings.UnsavedSessionDocument document = documents[i];
                TabInfo restored = null;
                if (!String.IsNullOrWhiteSpace(document.FilePath) && File.Exists(document.FilePath)) {
                    restored = CheckTabs(tabs, document.FilePath);
                    if (restored == null)
                        restored = Open(document.FilePath, OpenType.File, addToMRU: false, seltab: false, alreadyOpen: false);
                    if (restored != null) {
                        restored.textEditor.Text = document.Text;
                        restored.filename = Path.GetFileName(document.FilePath);
                        restored.filepath = document.FilePath;
                    }
                } else {
                    restored = Open(document.Text, OpenType.Text, addToMRU: false, seltab: false);
                    if (restored != null)
                        restored.filename = String.IsNullOrWhiteSpace(document.Name) ? GetUnsavedDocumentName() : document.Name;
                }
                if (restored == null)
                    continue;

                // An untouched untitled tab is retained for session continuity, but does
                // not represent work that needs a save confirmation when it is closed.
                restored.changed = !String.IsNullOrWhiteSpace(document.FilePath) || !String.IsNullOrEmpty(document.Text);
                restored.textEditor.ActiveTextAreaControl.Caret.Line = Math.Min(Math.Max(0, document.CaretLine), Math.Max(0, restored.textEditor.Document.TotalNumberOfLines - 1));
                MoveTabToIndex(restored, document.TabIndex);
                UpdateDocumentTab(restored.index);
                restoredAny = true;
                if (i == selectedIndex)
                    selectedTab = restored;
            }
            if (selectedTab != null)
                tabControl1.SelectTab(selectedTab.index);
            return restoredAny;
        }

        private void SaveOpenTabSession()
        {
            List<string> paths = new List<string>();
            int selectedIndex = -1;
            foreach (TabInfo tab in tabs) {
                bool savedDocument = !String.IsNullOrWhiteSpace(tab.filepath) && File.Exists(tab.filepath);
                if (Settings.reopenLastTabs && savedDocument) {
                    if (tab == currentTab)
                        selectedIndex = paths.Count;
                    paths.Add(tab.filepath);
                }
            }

            if (Settings.reopenLastTabs)
                Settings.SaveLastSession(paths, selectedIndex);
            else
                Settings.ClearPreviousTabSession();

            if (Settings.restoreUnsavedChangesOnExit)
                SaveUnsavedDocumentRecovery();
            else
                Settings.ClearUnsavedSession();
        }
        #endregion
        #region Control set states
        private void InitControlEvent()
        {
            SetProjectFolderText();

            // Parser
            parserLabel = new ToolStripLabel((Settings.enableParser) ? "Parser: No file" : parseoff);
            parserLabel.Alignment = ToolStripItemAlignment.Right;
            parserLabel.Overflow = ToolStripItemOverflow.Never;
            parserLabel.Click += delegate(object sender, EventArgs e) { ParseScript(0); };
            parserLabel.ToolTipText = "Click to update parser data.";
            parserLabel.TextChanged += delegate(object sender, EventArgs e) {
                parserLabel.ForeColor = InterfaceTheme.IsDark ? Color.Gainsboro : SystemColors.ControlText;
            };
            ToolStripMain.Items.Add(parserLabel);

            // Parser timer
            extParserTimer = new Timer();
            extParserTimer.Tick += new EventHandler(ExternalParser_Tick);
            intParserTimer = new Timer();
            intParserTimer.Tick += new EventHandler(InternalParser_Tick);

            // Tabs Swapped
            tabControl1.ShowCloseButtons = true;
            tabControl1.TabCloseRequested += delegate(object sender, TabCloseRequestedEventArgs e) {
                TabPage page = e.TabPage;
                if (page == null && e.TabIndex >= 0 && e.TabIndex < tabControl1.TabPages.Count)
                    page = tabControl1.TabPages[e.TabIndex];
                if (page == null)
                    return;

                TabInfo tab;
                if (documentTabs.TryGetValue(page, out tab))
                    Close(tab, page);
            };
            tabControl1.tabsSwapped += delegate(object sender, TabsSwappedEventArgs e) {
                if (e.aIndex < 0 || e.aIndex >= tabs.Count || e.bIndex < 0 || e.bIndex >= tabs.Count)
                    return;

                TabInfo movedTab = tabs[e.aIndex];
                tabs.RemoveAt(e.aIndex);
                int insertIndex = e.bIndex;
                insertIndex = Math.Max(0, Math.Min(insertIndex, tabs.Count));
                tabs.Insert(insertIndex, movedTab);
                for (int index = 0; index < tabs.Count; index++)
                    tabs[index].index = index;
            };

            // Create Variable Tab
            VarTree.HotTracking = true;
            VarTree.ShowNodeToolTips = true;
            VarTree.ShowRootLines = false;
            VarTree.Indent = DpiHelper.Scale(this, 16);
            VarTree.ItemHeight = DpiHelper.Scale(this, 14);
            VarTree.MouseDoubleClick += TreeView_DClickMouse;
            VarTree.AfterSelect += TreeView_AfterSelect;
            VarTree.AfterCollapse += Tree_AfterExpandCollapse;
            VarTree.AfterExpand += Tree_AfterExpandCollapse;
            VarTree.Dock = DockStyle.Fill;
            VarTree.BackColor = Color.FromArgb(250, 250, 255);
            VarTree.Cursor = Cursors.Hand;
            VarTab.Padding = DpiHelper.Scale(this, new Padding(0, 2, 2, 2));
            VarTab.BackColor = SystemColors.ControlLightLight;
            VarTab.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            VarTab.Controls.Add(VarTree);
        }

        private void SetActiveAreaEvents(TextEditorControl te)
        {
            te.ActiveTextAreaControl.TextArea.MouseDown -= TextArea_MouseDown;
            te.ActiveTextAreaControl.TextArea.MouseDown += TextArea_MouseDown;
            te.ActiveTextAreaControl.TextArea.MouseWheel -= TextArea_MouseWheel;
            te.ActiveTextAreaControl.TextArea.MouseWheel += TextArea_MouseWheel;
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

        private void TextArea_MouseDown(object sender, MouseEventArgs e)
        {
            autoComplete.Close();
            if (e.Button != MouseButtons.Right)
                return;

            TextArea textArea = sender as TextArea;
            if (textArea == null || textArea.Document.TotalNumberOfLines == 0) {
                editorContextLine = -1;
                editorContextPosition = TextLocation.Empty;
                return;
            }

            TextLocation location = textArea.TextView.GetLogicalPosition(
                Math.Max(0, e.X - textArea.TextView.DrawingPosition.X),
                e.Y - textArea.TextView.DrawingPosition.Y);
            editorContextLine = Math.Max(0, Math.Min(textArea.Document.TotalNumberOfLines - 1, location.Y));
            LineSegment line = textArea.Document.GetLineSegment(editorContextLine);
            editorContextPosition = new TextLocation(Math.Max(0, Math.Min(line.Length, location.X)), editorContextLine);
        }

        private void TextArea_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == 0 || IsDisposed)
                return;

            BeginInvoke((MethodInvoker)delegate {
                if (currentTab == null || currentTab.textEditor.ActiveTextAreaControl.TextArea != sender)
                    return;

                bool messageFile = Path.GetExtension(currentTab.filename ?? String.Empty)
                    .Equals(".msg", StringComparison.OrdinalIgnoreCase);
                float baseSize = messageFile ? 10.0f : Settings.GetTextAreaFontBaseSize();
                int sizeOffset = (int)Math.Round(currentTab.textEditor.Font.Size - baseSize);
                if (e.Delta < 0 && sizeOffset <= -4)
                    sizeOffset = -5;
                SetStatusZoom(sizeOffset);
            });
        }
        void TextArea_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            Utilities.SelectedTextColorRegion(currentActiveTextAreaCtrl.Caret.Position, currentActiveTextAreaCtrl);
        }

        bool setOnlyOnce = false;

        private void EnableFormControls()
        {
            Split_button.Visible = !startupRestorationInProgress;
            splitDocumentToolStripMenuItem.Enabled = true;
            openAllIncludesScriptToolStripMenuItem.Enabled = true;
            GotoProc_StripButton.Enabled = true;
            //Search_toolStripButton.Enabled = true;
            CommentStripButton.Enabled = true;
            if (Settings.showLog)
                splitContainer1.Panel2Collapsed = false;
            RestoreLogSplitterPosition();
            includeFileToCodeToolStripMenuItem.Enabled = true;

            // set buttons position
            if (setOnlyOnce) return;
            setOnlyOnce = true;

            PositionEditorCornerButtons();
        }

        private void PositionEditorCornerButtons()
        {
            int xLocation = tabControl1.DisplayRectangle.Right;
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

            UpdateOutlineButtonState();

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
            Text = AppTitle;
            autoComplete.Close();
            includeFileToCodeToolStripMenuItem.Enabled = false;
        }

        private void ApplySettingsTabs(bool alsoFont = false)
        {
            ColorTheme.SetTheme();
            ColorTheme.ApplyRightPanelTheme();

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
            if (count < 4 || ScriptEditor.ThemedMessageBox.Show("Do you want to clear the list of recent files ?",
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
            using (SettingsDialog dialog = new SettingsDialog())
                dialog.ShowDialog(this);

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
                    if (tabControl3.TabPages.Count > 2 && currentTab.parseInfo != null && !currentTab.parseInfo.parseData) {
                        tabControl3.TabPages.RemoveAt(1); // удалить вкладку Variables если нет данных
                    }
                }
            } else if (Settings.enableParser != p) {
                //parserLabel.Text = "Parser: Get updated parsing data...";
                //parserLabel.ForeColor = Color.Green;
                foreach (TabInfo t in tabs)
                {
                    t.treeExpand.Clear();
                    if (t.shouldParse && (t.parseInfo == null || !t.parseInfo.parseData))
                        t.needsParse = true; //for next parsing
                }
                if (currentTab != null) {
                    if (ProcTree.Nodes.Count > 0) {
                        ProcTree.Nodes[0].Expand();
                    }
                    if (ProcTree.Nodes.Count > 1) {
                        ProcTree.Nodes[1].Expand();
                    }
                    if (VarTree.Nodes.Count > 0) {
                        VarTree.Nodes[0].Expand();
                    }
                    if (VarTree.Nodes.Count > 1) {
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

                extParserTimer.Stop(); // prevent parsing while compiler processes its temporary input

                string msg;
                if (Compile(currentTab, out msg)) {
                    Error.ClearParserErrors(currentTab);
                }
                tbOutput.Text = currentTab.buildLog = msg;
                QueueCurrentDocumentParse();
            }
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) {
                for (int i = 0; i < tabs.Count; i++)
                {
                    if (tabControl1.GetTabRect(i).Contains(e.Location)) {
                        if (e.Button == MouseButtons.Middle) {
                            TabInfo tab = GetDocumentTabAt(i);
                            if (tab != null)
                                Close(tab);
                        }
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

        private bool IsEmptyDocumentTabStrip(Point screenPoint)
        {
            if (IsDisposed || !tabControl1.Visible)
                return false;

            Point location = tabControl1.PointToClient(screenPoint);
            if (!tabControl1.ClientRectangle.Contains(location))
                return false;

            int headerBottom = tabControl1.DisplayRectangle.Top;
            int lastTabRight = 0;
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                Rectangle tab = tabControl1.GetTabRect(i);
                headerBottom = Math.Max(headerBottom, tab.Bottom);
                lastTabRight = Math.Max(lastTabRight, tab.Right);
                if (tab.Contains(location))
                    return false;
            }

            return location.Y >= 0 && location.Y < headerBottom && location.X >= lastTabRight;
        }

        private void CreateEmptyDocumentFromTabStrip()
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            BeginInvoke(new MethodInvoker(delegate {
                if (!IsDisposed)
                    Open(String.Empty, OpenType.Text);
            }));
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
            Open(String.Empty, OpenType.Text);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Settings.lastOpenScriptsFolder))
                ofdScripts.InitialDirectory = Settings.lastOpenScriptsFolder;
            else if (Directory.Exists(Settings.solutionProjectFolder))
                ofdScripts.InitialDirectory = Settings.solutionProjectFolder;

            ofdScripts.FileName = String.Empty;
            if (ofdScripts.ShowDialog() == DialogResult.OK) {
                string selectedFolder = Path.GetDirectoryName(ofdScripts.FileNames[0]);
                if (Directory.Exists(selectedFolder)) {
                    Settings.lastOpenScriptsFolder = selectedFolder;
                    ofdScripts.InitialDirectory = selectedFolder;
                    Settings.Save();
                }

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
                TabInfo.WriteAllTextAtomic(Path.Combine(path, fname), currentTab.textEditor.Text, currentTab.textEditor.Encoding);
            }
            sfdTemplate.Dispose();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close(currentTab);
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
            using (AboutBox aboutBox = new AboutBox())
                aboutBox.ShowDialog(this);
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox.OpenDocumentationFolder(this);
        }

        private void massCompileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.outputDir == null) {
                ScriptEditor.ThemedMessageBox.Show("No output path selected.\nPlease select your scripts directory before compiling", "Error");
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
            int succeeded = 0;
            int failed = 0;
            for (int i = 0; i < tabs.Count; i++) {
                //FullMsg.AppendLine("*** " + tabs[i].filename);
                if (Compile(tabs[i], out msg, false)) succeeded++;
                else failed++;
                tabs[i].buildLog = msg;
                FullMsg.AppendLine(msg);
                FullMsg.AppendLine();
            }
            tbOutput.Text = FullMsg.ToString();
            EditorNotifications.Show(this,
                String.Format("Compiled {0} open script(s); {1} failed. See the Build log for details.", succeeded, failed),
                failed == 0 ? NotificationKind.Success : NotificationKind.Warning, 7000);
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
            bool collapse = CodeFolder.HasUnfoldedProcedureOutsideLine(currentDocument, cline);
            CodeFolder.SetProceduresOutsideLineFolded(currentDocument, cline, collapse);
            UpdateOutlineButtonState();
            currentActiveTextAreaCtrl.CenterViewOn(cline, 0);
        }

        private void registerScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null || RegistredScriptDialogShow)
                return;

            if (currentTab.filepath == null) {
                ScriptEditor.ThemedMessageBox.Show("You cannot register an unsaved script.", "Error");
                return;
            }
            string fName = Path.GetExtension(currentTab.filename).ToLowerInvariant();
            if (fName != ".ssl" && fName != ".int") {
                ScriptEditor.ThemedMessageBox.Show("You cannot register this file.", "Error");
                return;
            }
            fName = Path.ChangeExtension(currentTab.filename, "int");
            if (fName.Length > 12) {
                ScriptEditor.ThemedMessageBox.Show("Script file names must be 8 characters or under to be registered.", "Error");
                return;
            }
            if (currentTab.filename.Length >= 2 && string.Compare(currentTab.filename.Substring(0, 2), "gl", true) == 0) {
                if (ScriptEditor.ThemedMessageBox.Show("This script starts with 'gl', and will be treated by sfall as a global script and loaded automatically.\n" +
                                    "If it's being used as a global script, it does not need to be registered.\n" +
                                    "If it isn't, the script should be renamed before registering it.\n" +
                                    "Are you sure you wish to continue?", "Error") != DialogResult.Yes)
                    return;
            }
            if (fName.IndexOf(' ') != -1) {
                ScriptEditor.ThemedMessageBox.Show("Cannot register a script name that contains a space.", "Error");
                return;
            }
            RegisterScript.Registration(fName);
        }

        private void dgvErrors_Scroll(object sender, ScrollEventArgs e)
        {
            dgvErrors.Invalidate(true);
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
                ScriptEditor.ThemedMessageBox.Show("Preprocessing failed. See the Build log.");
                return;
            }

            string file = Compiler.GetPreprocessedFile(currentTab.filename);
            if (file != null)
                Open(file, OpenType.File, false);
            else
                ScriptEditor.ThemedMessageBox.Show("Failed to fetch preprocessed file");
        }

        private void roundtripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null) return;

            if (Settings.userCmdCompile) {
                ScriptEditor.ThemedMessageBox.Show("It is required to turn off the compilation option via a user cmd file.");
                return;
            }
            dgvErrors.Rows.Clear();

            extParserTimer.Stop(); // prevent parsing while compiler processes its temporary input

            string msg;
            roundTrip = true;
            bool result = Compile(currentTab, out msg, showIcon: false);
            tbOutput.Text = currentTab.buildLog = msg;
            if (result) {
                Open(new Compiler(true).GetOutputPath(currentTab.filepath), OpenType.File, false, clearBuildLog: false);
            }
            roundTrip = false;
            QueueCurrentDocumentParse();
        }

        private void QueueCurrentDocumentParse()
        {
            if (currentTab == null || !currentTab.shouldParse || isClosing)
                return;

            currentTab.needsParse = true;
            ParseScript(0);
        }

        private void editRegisteredScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!RegistredScriptDialogShow) RegisterScript.Registration(null);
        }

        private void associateMsgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null) return;

            if (!Path.GetExtension(currentTab.filename).Equals(".ssl", StringComparison.OrdinalIgnoreCase)) {
                ScriptEditor.ThemedMessageBox.Show(MessageFile.WrongTypeFile, currentTab.filename) ;
                return;
            }

            KeyValuePair<string, int> target;
            if (TryGetMessageTarget(currentActiveTextAreaCtrl.Caret.Position, out target))
                NavigateToMessage(target);
            else
                AssociateMsg(currentTab, true);
        }

        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int i = (int)cmsTabControls.Tag;
            if ((i & 0x10000000) != 0)
                tabControl2.TabPages.RemoveAt(i ^ 0x10000000);
            else
                Close(GetDocumentTabAt(i));
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
            TabInfo tabToKeep = GetDocumentTabAt((int)cmsTabControls.Tag);
            for (int i = tabs.Count - 1; i >= 0; i--)
            {
                if (!object.ReferenceEquals(tabs[i], tabToKeep))
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

        private bool IsLogPanelCollapsed()
        {
            int maximumDistance = splitContainer1.Height - splitContainer1.Panel2MinSize - splitContainer1.SplitterWidth;
            return splitContainer1.Height > 0 && splitContainer1.SplitterDistance >= maximumDistance;
        }

        private void minimize_log_button_Click(object sender, EventArgs e)
        {
            if (!IsLogPanelCollapsed()) {
                Settings.editorSplitterPosition = splitContainer1.SplitterDistance;
                Settings.logPanelCollapsed = true;
                minimizeLogSize = Settings.editorSplitterPosition;
                splitContainer1.SplitterDistance = splitContainer1.Height - splitContainer1.SplitterWidth;
            } else {
                Settings.logPanelCollapsed = false;
                if (Settings.editorSplitterPosition < splitContainer1.Panel1MinSize)
                    Settings.editorSplitterPosition = splitContainer1.Height - (splitContainer1.Height / 4);
                RestoreLogSplitterPosition();
                minimizeLogSize = 0;
            }
        }

        private void RestoreLogSplitterPosition()
        {
            if (splitContainer1.Height <= 0)
                return;
            if (Settings.logPanelCollapsed) {
                splitContainer1.SplitterDistance = splitContainer1.Height - splitContainer1.SplitterWidth;
                return;
            }
            if (Settings.editorSplitterPosition < 0)
                return;

            int maximumDistance = splitContainer1.Height - splitContainer1.Panel2MinSize - splitContainer1.SplitterWidth;
            int distance = Math.Max(splitContainer1.Panel1MinSize,
                Math.Min(Settings.editorSplitterPosition, maximumDistance));
            if (distance <= maximumDistance)
                splitContainer1.SplitterDistance = distance;
        }

        private void showLogWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !(Settings.showLog = showLogWindowToolStripMenuItem.Checked);
            if (!splitContainer1.Panel2Collapsed) {
                RestoreLogSplitterPosition();
                InterfaceTheme.Apply(splitContainer1.Panel2);
            }
        }

        private void Headers_toolStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            if (Settings.pathHeadersFiles == null || !Directory.Exists(Settings.pathHeadersFiles)) {
                ScriptEditor.ThemedMessageBox.Show("The headers directory does not exist. Check the correctness of the path setting.");
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
                    int caretOffset = currentActiveTextAreaCtrl.Caret.Offset;
                    if (IsIdentifierCharacterAt(currentDocument, caretOffset - 1)) code = " " + code;

                    if (pos == -1 && IsIdentifierCharacterAt(currentDocument, caretOffset)) code += " ";
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

        private static bool IsIdentifierCharacterAt(IDocument document, int offset)
        {
            return offset >= 0 && offset < document.TextLength && char.IsLetterOrDigit(document.GetCharAt(offset));
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
            if (!this.ContainsFocus || SearchTextComboBox.Focused || ReplaceTextBox.Focused || currentTab == null)
                return;

            TextArea senderTextArea = sender as TextArea;
            if (sender != null && (senderTextArea == null || currentActiveTextAreaCtrl == null ||
                currentActiveTextAreaCtrl.TextArea != senderTextArea))
                return;

            if (autoComplete != null && autoComplete.ShiftCaret) {
                autoComplete.ShiftCaret = false;
                currentActiveTextAreaCtrl.Caret.Position = currentDocument.OffsetToPosition(autoComplete.WordPosition.Key);
                currentActiveTextAreaCtrl.Caret.UpdateCaretPosition();
            }
            currentActiveTextAreaCtrl.TextArea.Focus();
            currentActiveTextAreaCtrl.TextArea.Select();
        }

        private void ViewArgsStripButton_CheckedChanged(object sender, EventArgs e)
        {
            Settings.showProcedureArguments = ViewArgsStripButton.Checked;
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
            TabInfo tab = GetDocumentTabAt((int)cmsTabControls.Tag);
            if (tab != null && !String.IsNullOrEmpty(tab.filepath))
                System.Diagnostics.Process.Start("explorer", "/n, /select, " + tab.filepath);
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
                AddDiagnosticRow(err);

            if (report.Count > 0) {
                currentTab.parserErrors = report;
                tabControl2.SelectedIndex = 2;
                MaximizeLog();
            } else
                EditorNotifications.Show(this, "No message-structure problems found.", NotificationKind.Success);
        }

        private void ConfigureStatusZoomMenu()
        {
            fontSizeStatusMenu = new ContextMenuStrip(components);
            zoomOutMenuItem = new ToolStripMenuItem("Zoom out");
            zoomInMenuItem = new ToolStripMenuItem("Zoom in");
            resetZoomMenuItem = new ToolStripMenuItem("Reset zoom (100%)");
            zoomOutMenuItem.Click += delegate { SetStatusZoom(Settings.sizeFont - 1); };
            zoomInMenuItem.Click += delegate { SetStatusZoom(Settings.sizeFont + 1); };
            resetZoomMenuItem.Click += delegate { SetStatusZoom(0); };

            fontSizeStatusMenu.Items.Add(zoomOutMenuItem);
            fontSizeStatusMenu.Items.Add(zoomInMenuItem);
            fontSizeStatusMenu.Items.Add(new ToolStripSeparator());
            fontSizeStatusMenu.Items.Add(resetZoomMenuItem);
            fontSizeStatusMenu.Items.Add(new ToolStripSeparator());
            AddStatusZoomMenuItem(50);
            AddStatusZoomMenuItem(100);
            AddStatusZoomMenuItem(150);
            AddStatusZoomMenuItem(200);
            AddStatusZoomMenuItem(250);
            AddStatusZoomMenuItem(300);
            fontSizeStatusMenu.Opening += delegate {
                UpdateStatusZoomMenu();
                InterfaceTheme.Apply(fontSizeStatusMenu);
            };
        }

        private void AddStatusZoomMenuItem(int percent)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(percent.ToString() + "%");
            item.Tag = percent;
            item.Click += delegate { SetStatusZoom((int)item.Tag / 10 - 10); };
            zoomMenuItems.Add(percent, item);
            fontSizeStatusMenu.Items.Add(item);
        }

        private void FontSizeStripStatusLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                fontSizeStatusMenu.Show(Cursor.Position);
        }

        private void UpdateStatusZoomMenu()
        {
            zoomOutMenuItem.Enabled = Settings.sizeFont > -5;
            zoomInMenuItem.Enabled = Settings.sizeFont < 20;
            resetZoomMenuItem.Enabled = Settings.sizeFont != 0;
            int percent = (10 + Settings.sizeFont) * 10;
            foreach (KeyValuePair<int, ToolStripMenuItem> pair in zoomMenuItems)
                pair.Value.Checked = pair.Key == percent;
        }

        private void SetStatusZoom(int sizeOffset)
        {
            Settings.sizeFont = (sbyte)Math.Max(-5, Math.Min(20, sizeOffset));
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
            TabInfo tab = GetDocumentTabAt((int)cmsTabControls.Tag);
            if (tab != null && !String.IsNullOrEmpty(tab.filepath))
                Settings.OpenInExternalEditor(tab.filepath);
        }

        private void includeFileToCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.pathHeadersFiles == null || !Directory.Exists(Settings.pathHeadersFiles)) {
                ScriptEditor.ThemedMessageBox.Show("The headers directory does not exist. Check the correctness of the path setting.");
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
                        if (offs > 0 && currentDocument.GetCharAt(offs - 1) == '-') {
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

        private void tsmUnsetProjectFolder_Click(object sender, EventArgs e)
        {
            Settings.solutionProjectFolder = String.Empty;
            SetProjectFolderText();
            Settings.Save();
        }

        private void tslProject_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Settings.solutionProjectFolder)) {
                System.Diagnostics.Process.Start("explorer", Settings.solutionProjectFolder);
            } else {
                tsmSetProjectFolder_Click(sender, e);
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
