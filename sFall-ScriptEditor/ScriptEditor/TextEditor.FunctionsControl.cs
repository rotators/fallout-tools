using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;

using ScriptEditor.TextEditorUI;
using ScriptEditor.TextEditorUI.Nodes;

using ScriptEditor.TextEditorUtilities;

namespace ScriptEditor
{
    partial class TextEditor
    {
        #region Main functions

        public enum OpenType { None, File, Text }

        public TabInfo Open(string file, OpenType type, bool addToMRU = true, bool alwaysNew = false, bool recent = false, bool seltab = true,
                            bool commandline = false, bool fcdOpen = false, bool alreadyOpen = true, bool outputFolder = false, bool clearBuildLog = true)
        {
            bool decompileSuccess = false;
            string infile = null;

            if (type == OpenType.File) {
                if (!Path.IsPathRooted(file))
                    file = Path.GetFullPath(file);

                if (commandline && Path.GetExtension(file).ToLowerInvariant() == ".msg") {
                    if (currentTab == null)
                        wState = FormWindowState.Minimized;
                    MessageEditor.MessageEditorOpen(file, this).SendMsgLine += AcceptMsgLine;
                    return null;
                }
                // Check file
                bool Exists;
                if (!FileAssociation.CheckFileAllow(file, out Exists))
                    return null;
                //Add this file to the recent files list
                if (addToMRU) {
                    if (!Exists && recent && MessageBox.Show("This recent file not found. Delete recent link to file?", "Open file error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        recent = true;
                    else
                        recent = false; // don't delete file link from recent list
                    Settings.AddRecentFile(file, recent);
                    UpdateRecentList();
                }
                if (!Exists)
                    return null;
                //If this is an int, decompile
                if (string.Compare(Path.GetExtension(file), ".int", true) == 0) {
                    if (!this.Focused)
                        ShowMe();

                    infile = file;
                    if (clearBuildLog) tbOutput.Clear();
                    tabControl2.SelectedIndex = 1;
                    MaximizeLog();

                    string decomp = new Compiler(roundTrip).Decompile(file, this);
                    if (decomp == null) {
                        MessageBox.Show("Decompilation of '" + file + "' was not successful", "Error");
                        return null;
                    } else {
                        file = decomp;
                        decompileSuccess = true;
                        // fix for procedure begin
                        ParserInternal.FixProcedureBegin(file);
                    }
                } else {
                    //Check if the file is already open
                    var tab = CheckTabs(tabs, file);
                    if (tab != null) {
                        if (seltab)
                            tabControl1.SelectTab(tab.index);
                        ShowMe();
                        if (!alreadyOpen || MessageBox.Show("This file is already open!\nDo you want to open another one same file?", "Question",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            return tab;
                    }
                }
            }
            //Create the text editor and set up the tab
            ICSharpCode.TextEditor.TextEditorControl te = new ICSharpCode.TextEditor.TextEditorControl();

            if (caretSoftwareModeToolStripMenuItem.CheckState == CheckState.Indeterminate)
                caretSoftwareModeToolStripMenuItem.Checked = (Caret.GraphicsMode == ImplementationMode.SoftwareMode);

            te.TextEditorProperties.LineViewerStyle = LineViewerStyle.FullRow;
            te.TextEditorProperties.TabIndent = Settings.tabSize;
            te.TextEditorProperties.IndentationSize = Settings.tabSize;
            te.TextEditorProperties.ShowTabs = Settings.showTabsChar;
            te.TextEditorProperties.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            te.TextEditorProperties.NativeDrawText = Settings.winAPITextRender;
            te.TextEditorProperties.DarkScheme = ColorTheme.IsDarkTheme;

            if (type == OpenType.File && String.Compare(Path.GetExtension(file), ".msg", true) == 0) {
                te.Document.TextEditorProperties.Encoding = Settings.EncCodePage;
                te.SetHighlighting(ColorTheme.IsDarkTheme ? "MessageDark": "Message");
                te.TextEditorProperties.EnableFolding = false;
                te.TextEditorProperties.ConvertTabsToSpaces = false;
                te.TextEditorProperties.ShowVerticalRuler = false;
                te.TextEditorProperties.IndentStyle = IndentStyle.None;
                te.TextEditorProperties.ShowLineNumbers = false;
                te.TextEditorProperties.Font = new Font("Verdana", 10 + Settings.sizeFont, FontStyle.Regular, GraphicsUnit.Point);
            } else {
                te.SetHighlighting(ColorTheme.HighlightingScheme); // Activate the highlighting, use the name from the SyntaxDefinition node.
                te.Document.FoldingManager.FoldingStrategy = new CodeFolder();
                te.TextEditorProperties.ConvertTabsToSpaces = Settings.tabsToSpaces;
                te.TextEditorProperties.ShowSpaces = Settings.showTabsChar;
                te.TextEditorProperties.IndentStyle = IndentStyle.Smart;
                te.TextEditorProperties.ShowVerticalRuler = Settings.showVRuler;
                te.TextEditorProperties.VerticalRulerRow = Settings.tabSize;
                te.TextEditorProperties.AllowCaretBeyondEOL = true;
                //te.TextEditorProperties.CaretLine = true;
                Settings.SetTextAreaFont(te);
            }

            if (type == OpenType.File)
                te.LoadFile(file, false, true);
            else if (type == OpenType.Text)
                te.Text = file;

            // set tabinfo
            TabInfo ti = new TabInfo();
            ti.index = tabControl1.TabCount;
            ti.history.linePosition = new List<TextLocation>();
            ti.history.pointerCur = -1;
            ti.textEditor = te;

            bool createNew = false;
            if (type == OpenType.None) { // only for new create script
                sfdScripts.FileName = "NewScript";
                if (sfdScripts.ShowDialog() == DialogResult.OK) {
                    file = sfdScripts.FileName;
                    type = OpenType.File;
                    ti.changed = true;
                    te.Text = Properties.Resources.newScript;
                } else
                    return null;
                createNew = true;
            } //else
              //  ti.changed = false;

            if (type == OpenType.File ) { //&& !alwaysNew
                if (alwaysNew) {
                    string temp = Path.Combine(Settings.scriptTempPath, unsaved);
                    File.Copy(file, temp, true);
                    file = temp;
                }
                ti.filepath = file;
                ti.filename = Path.GetFileName(file);
            } else {
                ti.filepath = null;
                ti.filename = unsaved;
            }

            tabs.Add(ti);
            TabPage tp = new TabPage(ti.filename);
            tp.ImageIndex = (ti.changed) ? 1 : 0;
            tp.Controls.Add(te);
            te.Dock = DockStyle.Fill;
            tabControl1.TabPages.Add(tp);
            if (tabControl1.TabPages.Count == 1)
                EnableFormControls();
            if (type == OpenType.File) {
                if (!alwaysNew)
                    tp.ToolTipText = ti.filepath;
                string ext = Path.GetExtension(file).ToLowerInvariant();
                if (ext == ".ssl" || ext == ".h") {
                    te.Text = Utilities.NormalizeNewLine(te.Text);
                    if (formatCodeToolStripMenuItem.Checked)
                        te.Text = Utilities.FormattingCode(te.Text);
                    ti.shouldParse = true;
                    //ti.needsParse = true; // set 'true' only edit text

                    FirstParseScript(ti); // First Parse

                    if (!createNew && Settings.storeLastPosition) {
                        int pos = Settings.GetLastScriptPosition(ti.filename.ToLowerInvariant());
                        te.ActiveTextAreaControl.Caret.Line = pos;
                        te.ActiveTextAreaControl.CenterViewOn(pos, -1);
                    }
                    if (Settings.autoOpenMsgs && ti.filepath != null)
                        AssociateMsg(ti, false);
                }
                ti.FileTime = File.GetLastWriteTime(ti.filepath);
            }
            te.OptionsChanged();
            // TE events
            te.TextChanged += textChanged;
            SetActiveAreaEvents(te);
            te.ContextMenuStrip = editorMenuStrip;
            //
            if (tabControl1.TabPages.Count > 1) {
                if (seltab)
                    tabControl1.SelectTab(tp);
            } else
                tabControl1_Selected(null, null);

            if (fcdOpen)
                dialogNodesDiagramToolStripMenuItem_Click(null, null);

            if (!roundTrip && decompileSuccess) {
                SaveFileDialog sfDecomp = new SaveFileDialog();
                sfDecomp.Title = "Enter name to save decompile file";
                sfDecomp.Filter = "Script files|*.ssl";
                sfDecomp.RestoreDirectory = true;
                sfDecomp.InitialDirectory = (!outputFolder || Settings.outputDir == null) ? Path.GetDirectoryName(infile) : Settings.outputDir;
                sfDecomp.FileName = Path.GetFileNameWithoutExtension(infile);

                if (sfDecomp.ShowDialog() == DialogResult.OK) {
                    ti.filename = Path.GetFileName(sfDecomp.FileName);
                    ti.filepath = sfDecomp.FileName;

                    File.Copy(file, ti.filepath, true);
                    File.Delete(file);
                    ti.FileTime = File.GetLastWriteTime(ti.filepath);

                    tabControl1.TabPages[ti.index].Text = tabs[ti.index].filename;
                    tabControl1.TabPages[ti.index].ToolTipText = tabs[ti.index].filepath;
                    this.Text = SSE + ti.filepath + ((pDefineStripComboBox.SelectedIndex > 0) ? " [" + pDefineStripComboBox.Text + "]" : "");

                    ForceParseScript();
                }
                sfDecomp.Dispose();
            }
            return ti;
        }

        private void CheckChandedFile()
        {
            if (!currentTab.CheckFileTime()) {
                this.Activated -= TextEditor_Activated;
                DialogResult result = MessageBox.Show(currentTab.filepath +
                                                      "\nThe script file was changed outside the editor." +
                                                      "\nDo you want to update the script file?",
                                                      "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) {
                    currentTab.FileTime = File.GetLastWriteTime(currentTab.filepath);
                    int caretLine = currentActiveTextAreaCtrl.Caret.Line;
                    int scrollValue = currentActiveTextAreaCtrl.VScrollBar.Value;
                    currentTab.textEditor.BeginUpdate();
                    currentTab.textEditor.LoadFile(currentTab.filepath, false, true);
                    currentActiveTextAreaCtrl.VScrollBar.Value = scrollValue;
                    currentActiveTextAreaCtrl.Caret.Line = caretLine;
                    currentTab.textEditor.EndUpdate();

                    currentTab.changed = false;
                    SetTabTextChange(currentTab.index);
                } else
                    currentTab.FileTime = File.GetLastWriteTime(currentTab.filepath);
                this.Activated += TextEditor_Activated;
            }
        }

        private void Save(TabInfo tab, bool close = false)
        {
            if (tab != null) {
                if (tab.filepath == null) {
                    SaveAs(tab, close);
                    return;
                }
                while (bwSyntaxParser.IsBusy) {
                    System.Threading.Thread.Sleep(50); // Avoid stomping on files while the parser is running
                    Application.DoEvents();
                }
                savingRunning = true;
                bool msg = (Path.GetExtension(tab.filename) == ".msg");

                if (Settings.autoTrailingSpaces && !msg) {
                    new ICSharpCode.TextEditor.Actions.RemoveTrailingWS().Execute(currentActiveTextAreaCtrl.TextArea);
                }
                if (close && tab.textEditor.Document.FoldingManager.FoldMarker.Count > 0) {
                    CodeFolder.SetProceduresCollapsed(tab.textEditor.Document, tab.filename);
                }
                string saveText = tab.textEditor.Text;
                if (msg && Settings.EncCodePage.CodePage == 866) {
                    saveText = saveText.Replace('\u0425', '\u0058'); // Replacement russian letter "X", to english letter
                }
                Utilities.ConvertToUnixPlatform(ref saveText);

                tab.SaveInternal(saveText, tab.textEditor.Encoding, msg, close, tab.shouldParse);

                if (tab.changed && Settings.pathHeadersFiles != null && Path.GetExtension(tab.filename).ToLowerInvariant() == ".h" &&
                    String.Equals(Settings.pathHeadersFiles, Path.GetDirectoryName(tab.filepath), StringComparison.OrdinalIgnoreCase)) {
                    GetMacros.GetGlobalMacros(Settings.pathHeadersFiles);
                }

                tab.changed = false;
                SetTabTextChange(tab.index);
                savingRunning = false;
            }
        }

        private void SaveAs(TabInfo tab, bool close = false)
        {
            if (tab == null)
                return;

            switch (Path.GetExtension(tab.filename).ToLowerInvariant()) {
                case ".ssl":
                    sfdScripts.FilterIndex = 1;
                    break;
                case ".h":
                    sfdScripts.FilterIndex = 2;
                    break;
                case ".msg":
                    sfdScripts.FilterIndex = 3;
                    break;
                default:
                    sfdScripts.FilterIndex = 4;
                    break;
            }
            sfdScripts.FileName = tab.filename;

            if (sfdScripts.ShowDialog() == DialogResult.OK) {
                tab.filepath = sfdScripts.FileName;
                tab.filename = Path.GetFileName(tab.filepath);
                tabControl1.TabPages[tab.index].Text = tabs[tab.index].filename;
                tabControl1.TabPages[tab.index].ToolTipText = tabs[tab.index].filepath;
                Save(tab, close);
                Settings.AddRecentFile(tab.filepath);
                string ext = Path.GetExtension(tab.filepath).ToLowerInvariant();
                if (Settings.enableParser && (ext == ".ssl" || ext == ".h")) {
                    tab.shouldParse = true;
                    tab.needsParse = true;
                    tab.parseInfo.reParseData = true;
                    parserLabel.Text = "Parser: Wait for update";
                    ParseScript();
                }
                this.Text = SSE + tab.filepath + ((pDefineStripComboBox.SelectedIndex > 0) ? " [" + pDefineStripComboBox.Text + "]" : "");
            }
        }

        private void Close(TabInfo tab)
        {
            if (tab == null | tab.index == -1)
                return;

            int i = tab.index;
            var tag = tabControl1.TabPages[i].Tag;
            if (tag != null)
                ((NodeDiagram)tag).Close(); //also close diagram editor

            while (tab.nodeFlowchartTE.Count > 0)
                tab.nodeFlowchartTE[0].CloseEditor(true);

            bool skip = tab.changed; // если изменен, то пропустить сохранение состояний Folds в методе KeepScriptSetting
            if (tab.changed) {
                switch (MessageBox.Show("Save changes to " + tab.filename + "?", "Message", MessageBoxButtons.YesNoCancel)) {
                    case DialogResult.Yes:
                        Save(tab, true);
                        if (tab.changed)
                            return;
                        break;
                    case DialogResult.No:
                        break;
                    default:
                        return;
                }
            }
            KeepScriptSetting(tab, skip);

            if (i == tabControl1.SelectedIndex && tabControl1.TabPages.Count >= 3) {
                if (previousTabIndex != -1) {
                    tabControl1.SelectedIndex = previousTabIndex; // переход к предыдущей выбранной вкладке
                }
                else if (tabControl1.SelectedIndex < tabControl1.TabPages.Count - 1) {
                    if (i > 0) tabControl1.SelectedIndex++; // переход к следущей по номеру вкладки
                } else {
                    tabControl1.SelectedIndex--;
                }
            }
            tabControl1.TabPages.RemoveAt(i);
            tabs.RemoveAt(i);

            for (int j = i; j < tabs.Count; j++) tabs[j].index--;

            for (int j = 0; j < tabs.Count; j++)
            {
                if (tabs[j].msgFileTab == tab) {
                    tabs[j].msgFileTab = null;
                    tabs[j].messages.Clear();
                }
            }
            tab.index = -1;
            previousTabIndex = -1; // сбросить после удаления вкладки
        }

        private bool Compile(TabInfo tab, out string msg, bool showMessages = true, bool preprocess = false, bool showIcon = true)
        {
            msg = String.Empty;
            if (string.Compare(Path.GetExtension(tab.filename), ".ssl", true) != 0) {
                if (showMessages) MessageBox.Show("You cannot compile this file.", "Compile Error");
                return false;
            }
            if (!Settings.ignoreCompPath && !preprocess && Settings.outputDir == null) {
                if (showMessages) MessageBox.Show("No output path selected.\nPlease select your scripts directory before compiling", "Compile Error");
                return false;
            }
            if (tab.changed) Save(tab);
            if (tab.changed || tab.filepath == null) return false;

            bool success = new Compiler(roundTrip).Compile(tab.filepath, out msg, tab.buildErrors, preprocess, tab.parseInfo.ShortCircuitEvaluation);

            foreach (ErrorType et in new ErrorType[] { ErrorType.Error, ErrorType.Warning, ErrorType.Message })
            {
                foreach (Error e in tab.buildErrors)
                {
                    if (e.type == et) {
                        dgvErrors.Rows.Add(e.type.ToString(), Path.GetFileName(e.fileName), e.line, e);
                        if (et == ErrorType.Error) dgvErrors.Rows[dgvErrors.Rows.Count - 1].Cells[0].Style.ForeColor = Color.Red;
                    }
                }
            }

            if (dgvErrors.RowCount > 0) dgvErrors.Rows[0].Cells[0].Selected = false;

            if (preprocess) return success;

            if (!success) {
                parserLabel.Text = "Failed to compiled: " + tab.filename;
                parserLabel.ForeColor = Color.Firebrick;
                msg += "\r\n Compilation Failed! (See the output build and errors window log for details).";
                CompileFail.Play();

                if (showMessages) {
                    if (Settings.warnOnFailedCompile) {
                        tabControl2.SelectedIndex = 2 - Convert.ToInt32(Settings.userCmdCompile);
                        MaximizeLog();
                    }// else
                     //   new CompiledStatus(false, this).ShowCompileStatus();
                }
            } else {
                if (showMessages && showIcon)
                    new CompiledStatus(true, this).ShowCompileStatus();
                parserLabel.Text = "Compiled: " + tab.filename + " at " + DateTime.Now.ToString("HH:mm:ss");
                parserLabel.ForeColor = Color.DarkGreen;
                msg += "\r\n Compilation Successfully!\r\n";
            }
            return success;
        }
        #endregion

        #region Tabs control functions

        internal static TabInfo CheckTabs(List<TabInfo> tabs, string filepath)
        {
            foreach (TabInfo tab in tabs)
            {
                if (String.Equals(tab.filepath, filepath, StringComparison.OrdinalIgnoreCase)) return tab;
            }
            return null;
        }

        internal static bool CheckTabs(string filepath, List<TabInfo> tabs)
        {
            return CheckTabs(tabs, filepath) != null;
        }

        private void SetTabTextChange(int i) { tabControl1.TabPages[i].ImageIndex = (tabs[i].changed ? 1 : 0); }

        private void SwitchToTab(int index)
        {
            if (tabControl1.TabPages.Count > 1) tabControl1.SelectTab(index);
        }

        // Called when creating a new document and when switching tabs
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            // останавливаем таймеры парсеров
            intParserTimer.Stop();
            extParserTimer.Stop();

            if (tabControl1.SelectedIndex == -1) {
                currentTab = null;
                parserLabel.Text = (Settings.enableParser) ? "Parser: No file" : parseoff;
                SetFormControlsOff();
            } else {
                if (currentTab != null) {
                    previousTabIndex = currentTab.index;
                }
                currentTab = tabs[tabControl1.SelectedIndex];
                //if (!Settings.enableParser && currentTab.parseInfo != null)
                //    currentTab.parseInfo.parseData = false;

                if (currentTab.msgFileTab != null)
                    MessageFile.ParseMessages(currentTab);

                // Create or Delete Variable treeview
                if (!Settings.enableParser && tabControl3.TabPages.Count > 2) {
                    if (currentTab.parseInfo != null) {
                        if (!currentTab.parseInfo.parseData) {
                            tabControl3.TabPages.RemoveAt(1);
                        }
                    } else {
                        tabControl3.TabPages.RemoveAt(1);
                    }
                } else if (tabControl3.TabPages.Count < 3 && (Settings.enableParser || currentTab.parseInfo != null)) {
                    if (currentTab.parseInfo != null && currentTab.parseInfo.parseData) {
                        CreateTabVarTree();
                    }
                }
                if (currentTab.shouldParse) {
                    if (Settings.enableParser && currentTab.parseInfo.parseError && !currentTab.needsParse) {
                        parserLabel.Text = "Parser: Parsing script error (see parser errors log)";
                    } else
                    if (currentTab.needsParse) {
                        parserLabel.Text = (Settings.enableParser) ? "Parser: Waiting to update..." : parseoff;
                        // Update parse info
                        ParseScript();
                    } else
                        parserLabel.Text = (Settings.enableParser) ? "Parser: Idle" : parseoff;
                } else
                    parserLabel.Text = (Settings.enableParser) ? "Parser: Not an SSL file" : parseoff;

                UpdateLog();
                currentHighlightProc = null;
                UpdateNames(true);
                // text editor set focus
                currentActiveTextAreaCtrl.Select();
                ControlFormStateOn_Off();
                this.Text = SSE + currentTab.filepath + ((pDefineStripComboBox.SelectedIndex > 0) ? " [" + pDefineStripComboBox.Text + "]" : "");

                if (sender != null) CheckChandedFile();
            }
        }
        #endregion

        // Goto script text of selected Variable or Procedure in treeview
        public void SelectLine(string file, int line, bool pselect = false, int column = -1, int sLen = -1)
        {
            if (line <= 0) return;

            bool not_this = false;
            if (currentTab == null || file != currentTab.filepath) {
                if (Open(file, OpenType.File, false, alreadyOpen: false) == null) {
                    MessageBox.Show("Could not open file '" + file + "'", "Error");
                    return;
                }
                not_this = true;
            }
            LineSegment ls;
            if (line > currentDocument.TotalNumberOfLines)
                ls = currentDocument.GetLineSegment(currentDocument.TotalNumberOfLines - 1);
            else
                ls = currentDocument.GetLineSegment(line - 1);

            TextLocation start, end;
            if (column == -1 || column > ls.Length) {
                start = new TextLocation(0, ls.LineNumber);
                if (column == -1)
                    end = new TextLocation(ls.Length, ls.LineNumber);
                else
                    end = new TextLocation(0, ls.LineNumber);
            } else {
                column--;
                if (sLen == -1) {
                    foreach (var w in ls.Words)
                    {
                        if (w.Type != TextWordType.Word)
                            continue;
                        int pos = w.Offset + w.Length;
                        if ((column >= w.Offset) && (column <= pos)) {
                            column = w.Offset;
                            sLen = w.Length;
                            break;
                        }
                    }
                }
                start = new TextLocation(column, ls.LineNumber);
                end = new TextLocation(start.Column + sLen, ls.LineNumber);
            }
            // Expand or Collapse folding
            foreach (FoldMarker fm in currentDocument.FoldingManager.FoldMarker) {
                if (OnlyProcStripButton.Checked) {
                    if (fm.FoldType == FoldType.MemberBody || fm.FoldType == FoldType.Region) {
                        if (fm.StartLine == start.Line)
                            fm.IsFolded = false;
                        else if (fm.FoldType != FoldType.Region)
                            fm.IsFolded = true;
                    }
                } else {
                    if (fm.StartLine == start.Line) {
                        fm.IsFolded = false;
                        break;
                    }
                }
            }
            // Scroll and select
            currentActiveTextAreaCtrl.Caret.Position = start;
            if (not_this || !pselect || !OnlyProcStripButton.Checked)
                currentActiveTextAreaCtrl.SelectionManager.SetSelection(start, end);
            else
                currentActiveTextAreaCtrl.SelectionManager.ClearSelection();

            if (!not_this) {
                if (pselect)
                    currentActiveTextAreaCtrl.TextArea.TextView.FirstVisibleLine = start.Line - 1;
                else
                    currentActiveTextAreaCtrl.CenterViewOn(start.Line + 10, 0);
            } else
                currentActiveTextAreaCtrl.CenterViewOn(start.Line - 15, 0);
            currentTab.textEditor.Refresh();
        }

        #region Tree browser control

        private void CreateTabVarTree() { tabControl3.TabPages.Insert(1, VarTab); }

        private enum TreeStatus { idle, update, freeze }

        internal static Procedure currentHighlightProc = null;
        private  static TreeNode currentHighlightNode = null;
        private bool updateHighlightPocedure = true;

        // подсветить процедуру в дереве
        private void HighlightCurrentPocedure(int curLine)
        {
            Procedure proc;
            if (curLine == -2) {
                proc = currentHighlightProc;
                currentHighlightProc = null;
                currentHighlightNode = null;
            } else {
                proc = currentTab.parseInfo.GetProcedureFromPosition(curLine);
            }
            if (proc != null && proc != currentHighlightProc) {
                if (currentHighlightProc != null && currentHighlightProc.name.Equals(proc.name, StringComparison.OrdinalIgnoreCase)) return;
                TreeNodeCollection nodes;
                if (ProcTree.Nodes.Count > 1)
                    nodes = ProcTree.Nodes[1].Nodes;
                else
                    nodes = ProcTree.Nodes[0].Nodes; // for parser off
                foreach (TreeNode node in nodes)
                {
                    string name = ((Procedure)node.Tag).name;
                    if (name == proc.name) {
                        node.Text = node.Text.Insert(0, "► ");
                        node.ForeColor = ColorTheme.HighlightProcedureTree;
                        if (currentHighlightNode != null) {
                            currentHighlightNode.ForeColor = ProcTree.ForeColor;
                            currentHighlightNode.Text = currentHighlightNode.Text.Substring(2);
                        }
                        currentHighlightProc = proc;
                        currentHighlightNode = node;
                        break;
                    }
                }
            } else if (currentHighlightProc != null && currentHighlightProc != proc) {
                currentHighlightNode.Text = currentHighlightNode.Text.Substring(2);
                currentHighlightNode.ForeColor = ProcTree.ForeColor;
                currentHighlightProc = null;
                currentHighlightNode = null;
            }
        }

        // Create names for procedures and variables in treeview
        private void UpdateNames(bool newCreate = false)
        {
            if (ProcTree.Tag != null && (TreeStatus)ProcTree.Tag == TreeStatus.freeze) {
                ProcTree.Tag = TreeStatus.idle;
                return;
            }

            if (currentTab == null || !currentTab.shouldParse || currentTab.parseInfo == null) return;

            object selectedNode = null;
            if (ProcTree.SelectedNode != null)
                selectedNode = ProcTree.SelectedNode.Tag;

            ProcTree.Tag = TreeStatus.update;

            string scrollNode = null;
            if (!newCreate && ProcTree.Nodes.Count != 0) {
                for (int i = ProcTree.Nodes.Count -1; i >= 0; i--)
                {
                    if (!ProcTree.Nodes[i].IsExpanded) continue;
                    for (int j = ProcTree.Nodes[i].Nodes.Count - 1; j >= 0; j--)
                    {
                        if (ProcTree.Nodes[i].Nodes[j].IsVisible) {
                            scrollNode = ProcTree.Nodes[i].Nodes[j].Name;
                            break;
                        }
                    }
                    if (scrollNode != null) break;
                }
            }
            ProcTree.BeginUpdate();
            ProcTree.Nodes.Clear();

            TreeNode rootNode;
            foreach (var s in TREEPROCEDURES) {
                rootNode = ProcTree.Nodes.Add(s, s);
                rootNode.ForeColor = Color.DodgerBlue;
                rootNode.NodeFont = new Font("Arial", 9F, FontStyle.Bold, GraphicsUnit.Point);
            }
            ProcTree.Nodes[0].ToolTipText = "Procedures declared and located in headers files." + treeTipProcedure;
            ProcTree.Nodes[0].Tag = 0; // global tag
            ProcTree.Nodes[1].ToolTipText = "Procedures declared and located in this script." + treeTipProcedure;
            ProcTree.Nodes[1].Tag = 1; // local tag

            foreach (Procedure p in currentTab.parseInfo.procs) {
                // TODO: Это нужно только для отключенного парсера?
                if (!Settings.enableParser && p.d.end == -1) continue; //skip imported or broken procedures

                TreeNode tn = new TreeNode((!ViewArgsStripButton.Checked)? p.name : p.ToString(false));
                tn.Name = p.name;
                tn.Tag = p;
                foreach (Variable var in p.variables) {
                    TreeNode tn2 = new TreeNode(var.name);
                    tn2.Name = var.name;
                    tn2.Tag = var;
                    tn2.ToolTipText = var.ToString();
                    tn.Nodes.Add(tn2);
                }
                if (p.filename.Equals(currentTab.filename, StringComparison.OrdinalIgnoreCase) == false || p.IsImported) {
                    tn.ToolTipText = p.ToString() + "\ndeclarate file: " + p.filename;
                    ProcTree.Nodes[0].Nodes.Add(tn);
                    ProcTree.Nodes[0].Expand();
                } else {
                    tn.ToolTipText = p.ToString();
                    ProcTree.Nodes[1].Nodes.Add(tn);
                    ProcTree.Nodes[1].Expand();
                }
            }

            if (!Settings.enableParser && !currentTab.parseInfo.parseData) {
                ProcTree.Nodes.RemoveAt(0);
                if (tabControl3.TabPages.Count > 2) // удалить и вкладку если отсутсвует информация
                    tabControl3.TabPages.RemoveAt(1);
            } else {
                VarTree.BeginUpdate();
                VarTree.Nodes.Clear();

                foreach (var s in TREEVARIABLES) {
                    rootNode = VarTree.Nodes.Add(s);
                    rootNode.ForeColor = Color.DodgerBlue;
                    rootNode.NodeFont = new Font("Arial", 9F, FontStyle.Bold, GraphicsUnit.Point);
                }
                VarTree.Nodes[0].ToolTipText = "Variables declared and located in headers files." + treeTipVariable;
                VarTree.Nodes[1].ToolTipText = "Variables declared and located in this script." + treeTipVariable;

                foreach (Variable var in currentTab.parseInfo.vars) {
                    TreeNode tn = new TreeNode(var.name);
                    tn.Tag = var;
                    if (var.filename.Equals(currentTab.filename, StringComparison.OrdinalIgnoreCase) == false) {
                        tn.ToolTipText = var.ToString() + "\ndeclarate file: " + var.filename;
                        VarTree.Nodes[0].Nodes.Add(tn);
                        VarTree.Nodes[0].Expand();
                    } else {
                        tn.ToolTipText = var.ToString();
                        VarTree.Nodes[1].Nodes.Add(tn);
                        VarTree.Nodes[1].Expand();
                    }
                }
                if (VarTree.Nodes[0].Nodes.Count == 0) VarTree.Nodes[0].ForeColor = Color.Gray;
                if (VarTree.Nodes[1].Nodes.Count == 0) VarTree.Nodes[1].ForeColor = Color.Gray;

                foreach (TreeNode node in VarTree.Nodes)
                    SetNodeCollapseStatus(node);

                VarTree.EndUpdate();
            }
            foreach (TreeNode node in ProcTree.Nodes)
                SetNodeCollapseStatus(node);

            if (ProcTree.Nodes[0].Nodes.Count == 0) ProcTree.Nodes[0].ForeColor = Color.Gray;
            if (ProcTree.Nodes.Count > 1) {
                if (ProcTree.Nodes[1].Nodes.Count == 0)
                    ProcTree.Nodes[1].ForeColor = Color.Gray;
                //ProcTree.Nodes[1].EnsureVisible();
            }

            if (selectedNode != null) {
                TreeNode[] nodes = null;
                if (selectedNode is Procedure)
                    nodes = ProcTree.Nodes.Find(((Procedure)selectedNode).name, true);
                else if (selectedNode is Variable)
                    nodes = ProcTree.Nodes.Find(((Variable)selectedNode).name, true);
                if (nodes != null && nodes.Length > 0)
                    ProcTree.SelectedNode = nodes[0];
            }
            ProcTree.EndUpdate();
            HighlightCurrentPocedure((currentHighlightProc == null) ? currentActiveTextAreaCtrl.Caret.Line : -2);

            // scroll to node
            if (scrollNode != null) {
                foreach (TreeNode nodes in ProcTree.Nodes) {
                    foreach (TreeNode node in nodes.Nodes) {
                        if (node.Name == scrollNode) {
                            if (node.PrevNode != null) node.PrevNode.EnsureVisible();
                            scrollNode = null;
                            break;
                        }
                    }
                    if (scrollNode == null) break;
                }
                if (scrollNode != null && currentHighlightNode != null) currentHighlightNode.EnsureVisible();
            }
            ProcTree.Tag = TreeStatus.idle;
        }

        // обновляет процедуры в nodes.Tag
        private void UpdateNodesTags()
        {
            // Avoid stomping on files while the parser is running
            while (parserIsRunning) System.Threading.Thread.Sleep(10);

            TreeNodeCollection nodes;
            if (ProcTree.Nodes.Count > 1)
                nodes = ProcTree.Nodes[1].Nodes;
            else
                nodes = ProcTree.Nodes[0].Nodes; // for parser off

            for (int i = 0; i < nodes.Count; i++)
            {
                Procedure np = nodes[i].Tag as Procedure;
                if (np == null) continue;

                foreach (Procedure p in currentTab.parseInfo.procs)
                {
                    if (p.Name == np.Name) {
                        //if (p.d.declared != np.d.declared)
                        nodes[i].Tag = p;
                        break;
                    }
                }
            }
        }

        private string GetCorrectNodeKeyName(TreeNode node)
        {
            string nodeKey = node.FullPath;
            int n = nodeKey.IndexOf('\\');
            if (n != -1) nodeKey = nodeKey.Remove(n + 1) + node.Name;
            return nodeKey;
        }

        private void SetNodeCollapseStatus(TreeNode node)
        {
            string nodeKey = GetCorrectNodeKeyName(node);
            if (currentTab.treeExpand.ContainsKey(nodeKey)) {
                    if (currentTab.treeExpand[nodeKey])
                        node.Collapse();
                    else
                        node.Expand();
            }
            foreach (TreeNode nd in node.Nodes) SetNodeCollapseStatus(nd);
        }

        bool treeExpandCollapse = false;

        private void TreeExpandCollapse(TreeViewEventArgs e)
        {
            TreeNode tn = e.Node;
            if (tn == null) return;

            bool collapsed = (e.Action == TreeViewAction.Collapse);
            string nodeKey = GetCorrectNodeKeyName(tn);
            if (!currentTab.treeExpand.ContainsKey(nodeKey))
                currentTab.treeExpand.Add(nodeKey, collapsed);
            else
                currentTab.treeExpand[nodeKey] = collapsed;
            if (tn.Parent == null) treeExpandCollapse = true;
        }

        private void TreeView_DClickMouse(object sender, MouseEventArgs e) {
            if (e.X <= 20) return;
            TreeNode node = (!treeExpandCollapse) ? ((TreeView)sender).GetNodeAt(e.Location) : null;
            treeExpandCollapse = false;
            if (node != null) TreeView_ClickBehavior(node);
        }

        // Click on node tree Procedures/Variables
        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!ctrlKeyPress || e.Action == TreeViewAction.Unknown) return;
            TreeView_ClickBehavior(e.Node);
        }

        private void TreeView_ClickBehavior(TreeNode node)
        {
            string file = null, name = null;
            int line = 0;
            bool pSelect = false;
            if (node.Tag is Variable) {
                Variable var = (Variable)node.Tag;
                if (!ctrlKeyPress) {
                    file = var.fdeclared;
                    line = var.d.declared;
                } else {
                    name = var.name;
                }
            } else if (node.Tag is Procedure) {
                Procedure proc = (Procedure)node.Tag;
                if (!ctrlKeyPress) {
                    file = proc.fstart;
                    line = proc.d.start;
                    if (line == -1 || file == null) { // goto declared
                        file = proc.fdeclared;
                        line = proc.d.declared;
                    }
                    pSelect = true;
                } else {
                    name = proc.name;
                }
            }
            if (file != null) {
                SelectLine(file, line, pSelect);
            } else if (name != null) {
                Utilities.InsertText(name, currentActiveTextAreaCtrl);
            }
        }

        void Tree_AfterExpandCollapse(object sender, TreeViewEventArgs e)
        {
            if ((TreeStatus)ProcTree.Tag == TreeStatus.idle)
                TreeExpandCollapse(e);
        }

        private void ProcTree_Leave(object sender, EventArgs e)
        {
            ProcTree.SelectedNode = null;
        }

        private void ProcTree_MouseDown(object sender, MouseEventArgs e)
        {
            dbClick = false;
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left && e.Clicks == 2) {
                TreeNode tn = ProcTree.GetNodeAt(e.X, e.Y);
                if (tn != null) dbClick = true;
                if (e.Button == MouseButtons.Right) {
                    ProcTree.SelectedNode = tn;
                }
            }
        }

        private void ProcTree_BeforeExpandCollapse(object sender, TreeViewCancelEventArgs e) {
            if (e.Action == TreeViewAction.Expand || e.Action == TreeViewAction.Collapse) {
                 if (dbClick || ctrlKeyPress) {
                    if (e.Node.Tag is Procedure) e.Cancel = true;
                    dbClick = false;
                }
            }
        }
        #endregion

        #region Refactoring Control

        private void UpdateEditorToolStripMenu()
        {
            TextLocation tl = currentActiveTextAreaCtrl.Caret.Position;

            // includes
            string line = TextUtilities.GetLineAsString(currentDocument, tl.Line).Trim();
            if (!line.TrimStart().StartsWith(ParserInternal.INCLUDE)) {
                openIncludeToolStripMenuItem.Enabled = false;
            }

            // skip for specific color text
            if (ColorTheme.CheckColorPosition(currentDocument, tl))
                return;

            //Refactor name
            if (!Settings.enableParser) {
                renameToolStripMenuItem.Text += ": Disabled";
                renameToolStripMenuItem.ToolTipText = "It is required to enable the parser in the settings.";
                return;
            }

            if (currentTab.parseInfo != null) {
                NameType itemType = NameType.None;
                IParserInfo item = null;

                string word = TextUtilities.GetWordAt(currentDocument, currentDocument.PositionToOffset(tl));
                item = currentTab.parseInfo.Lookup(word, currentTab.filepath, tl.Line + 1);
                if (item != null) {
                    itemType = item.Type();
                    renameToolStripMenuItem.Tag = item;
                    if (!currentTab.needsParse)
                        renameToolStripMenuItem.Enabled = true;
                }

                switch (itemType)
                {
                    case NameType.LVar: // variable procedure
                    case NameType.GVar: // variable script
                        findReferencesToolStripMenuItem.Enabled = true;
                        findDeclerationToolStripMenuItem.Enabled = true;
                        findDefinitionToolStripMenuItem.Enabled = false;
                        renameToolStripMenuItem.Text += (itemType == NameType.LVar)
                                                        ? (((Variable)item).IsArgument ? ": Argument variable" : ": Local variable")
                                                        : ": Script variable";
                        if (item.IsExported)
                            renameToolStripMenuItem.ToolTipText = "Note: Renaming exported variables will result in an error in the scripts using this variable.";
                        break;

                    case NameType.Proc:
                        findReferencesToolStripMenuItem.Enabled = currentTab.parseInfo.parseData; //true;
                        findDeclerationToolStripMenuItem.Enabled = true;
                        findDefinitionToolStripMenuItem.Enabled = !item.IsImported;
                        renameToolStripMenuItem.Text += ": Procedure";
                        if (item.IsExported)
                            renameToolStripMenuItem.ToolTipText = "Note: Renaming exported procedures will result in an error in the scripts using this procedure.";
                        break;

                    case NameType.Macro:
                        findReferencesToolStripMenuItem.Enabled = false;
                        findDeclerationToolStripMenuItem.Enabled = true;
                        findDefinitionToolStripMenuItem.Enabled = false;
                        Macro macro = (Macro)item;
                        if (!ProgramInfo.macrosGlobal.ContainsKey(macro.token) && macro.fdeclared == currentTab.filepath)
                            renameToolStripMenuItem.Text += ": Local macro";
                        else
                            renameToolStripMenuItem.Text += ": Global macro";
                        break;

                    default:
                        if (!currentTab.parseInfo.parseData) {
                            renameToolStripMenuItem.Text += ": Out of data";
                            renameToolStripMenuItem.ToolTipText = "The parser data is missing.";
                        } else
                            renameToolStripMenuItem.Text += ": None";
                        break;
                }
                if (item != null && item.IsImported) {
                    renameToolStripMenuItem.Enabled = !item.IsImported;
                    renameToolStripMenuItem.ToolTipText = "The feature is disabled, will be available in future versions.";
                }
            } else {
                renameToolStripMenuItem.Text += ": Out of data";
                renameToolStripMenuItem.ToolTipText = "The parser data is missing.";
            }
        }

        private void editorMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (currentTab == null /*&& !treeView1.Focused*/) {
                e.Cancel = true;
                return;
            }
            if (currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected) {
                highlightToolStripMenuItem.Visible = true;
                renameToolStripMenuItem.Visible = false;
            } else {
                highlightToolStripMenuItem.Visible = false;
                renameToolStripMenuItem.Visible = true;
                renameToolStripMenuItem.Text = "Rename";
                renameToolStripMenuItem.Enabled = false;
                renameToolStripMenuItem.ToolTipText = (currentTab.needsParse) ? "Waiting get parsing data..." : "";
            }
            //openIncludeToolStripMenuItem.Enabled = false;
            findReferencesToolStripMenuItem.Enabled = false;
            findDeclerationToolStripMenuItem.Enabled = false;
            findDefinitionToolStripMenuItem.Enabled = false;
            UpdateEditorToolStripMenu();
        }

        private void editorMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            findReferencesToolStripMenuItem.Enabled = true;
            findDeclerationToolStripMenuItem.Enabled = true;
            findDefinitionToolStripMenuItem.Enabled = true;
            openIncludeToolStripMenuItem.Enabled = true;
        }
        #endregion

        private void AssociateMsg(TabInfo tab, bool create)
        {
            if (tab.filepath == null || tab.msgFileTab != null)
                return;

            if (Settings.autoOpenMsgs && msgAutoOpenEditorStripMenuItem.Checked && !create) {
                MessageEditor.MessageEditorInit(tab, this);
                Focus();
            } else {
                string path;
                if (MessageFile.GetAssociatePath(tab, create, out path)) {
                    tab.msgFilePath = path;
                    tab.msgFileTab = Open(tab.msgFilePath, OpenType.File, false);
                }
            }
        }

        public void AcceptMsgLine(string line)
        {
            if (currentTab != null) {
                Utilities.InsertText(line, currentActiveTextAreaCtrl);
                this.Focus();
            }
        }

        private void UpdateRecentList()
        {
            string[] items = Settings.GetRecent();
            int count = Open_toolStripSplitButton.DropDownItems.Count-1;
            for (int i = 3; i <= count; i++) {
                Open_toolStripSplitButton.DropDownItems.RemoveAt(3);
            }
            for (int i = items.Length - 1; i >= 0; i--) {
                Open_toolStripSplitButton.DropDownItems.Add(items[i], null, recentItem_Click);
            }
        }

        private void AddSearchTextComboBox(string world)
        {
            if (world.Length == 0) return;

            bool addSearchText = true;
            foreach (var item in SearchTextComboBox.Items)
            {
                if (world == item.ToString()) {
                    addSearchText = false;
                    break;
                }
            }
            if (addSearchText) {
                SearchTextComboBox.Items.Insert(0, world);
                if (sf != null) sf.cbSearch.Items.Insert(0, world); // add to advanced search form
            }
        }

        private void KeepScriptSetting(TabInfo tab, bool skip)
        {
            if (!skip && tab.filepath != null && tab.textEditor.Document.FoldingManager.FoldMarker.Count > 0) {
                CodeFolder.SetProceduresCollapsed(tab.textEditor.Document, tab.filename);
            }
            // store last script position
            if (Path.GetExtension(tab.filepath).ToLowerInvariant() == ".ssl" && tab.filename != unsaved)
                Settings.SetLastScriptPosition(tab.filename.ToLowerInvariant(), tab.textEditor.ActiveTextAreaControl.Caret.Line);
        }

        #region Dialog System

        private void dialogNodesDiagramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null || currentTab.parseInfo == null) return;

            if (!Path.GetExtension(currentTab.filename).Equals(".ssl", StringComparison.OrdinalIgnoreCase)) {
                MessageBox.Show(MessageFile.WrongTypeFile, currentTab.filename);
                return;
            }

            var tag = tabControl1.TabPages[currentTab.index].Tag;
            if (tag != null) {
                NodeDiagram ndForm = ((NodeDiagram)tag);
                if (ndForm.WindowState == FormWindowState.Minimized)
                    ndForm.WindowState = FormWindowState.Maximized;
                ndForm.Activate();
                return;
            }

            string msgPath;
            if (!MessageFile.GetAssociatePath(currentTab, false, out msgPath)) {
                MessageBox.Show(MessageFile.MissingFile, "Nodes Flowchart Editor");
                return;
            }
            ScriptEditor.TextEditorUI.Function.DialogFunctionsRules.BuildOpcodesDictionary();

            currentTab.msgFilePath = msgPath;

            NodeDiagram NodesView = new NodeDiagram(currentTab);
            NodesView.FormClosed += delegate { tabControl1.TabPages[currentTab.index].Tag = null; };
            NodesView.ChangeNodes += delegate { ForceParseScript(); }; //Force Parse Script;
            NodesView.Show();

            tabControl1.TabPages[currentTab.index].Tag = NodesView;

            this.ParserUpdatedInfo += delegate
            {
                if (NodesView != null)
                    NodesView.NeedUpdate = true;
            };
        }

        private void previewDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null || currentTab.parseInfo == null) return;

            if (!Path.GetExtension(currentTab.filename).Equals(".ssl", StringComparison.OrdinalIgnoreCase)) {
                MessageBox.Show(MessageFile.WrongTypeFile, currentTab.filename) ;
                return;
            }

            string msgPath;
            if (!MessageFile.GetAssociatePath(currentTab, false, out msgPath)) {
                MessageBox.Show(MessageFile.MissingFile, "Dialog Preview");
                return;
            }
            currentTab.msgFilePath = msgPath;

            ScriptEditor.TextEditorUI.Function.DialogFunctionsRules.BuildOpcodesDictionary();

            DialogPreview DialogView = new DialogPreview(currentTab);
            if (!DialogView.InitReady) {
                DialogView.Dispose();
                MessageBox.Show("This script does not contain dialog procedures.", "Dialog Preview");
            }
            else
                DialogView.Show(this);
        }

        private void dialogFunctionConfigToolStripMenuItem_Click(object sender, EventArgs e) {
            ScriptEditor.TextEditorUI.Function.DialogFunctionsRules.BuildOpcodesDictionary();
            new ScriptEditor.TextEditorUI.Function.FunctionsRules().ShowDialog(this);
        }

        private void editNodeCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Procedure proc = (Procedure)ProcTree.SelectedNode.Tag;

            if (currentTab.messages.Count == 0) {
                string msgPath;
                if (!MessageFile.GetAssociatePath(currentTab, false, out msgPath)) {
                    MessageBox.Show(MessageFile.MissingFile, "Node Editor");
                    return;
                }
                currentTab.msgFilePath = msgPath;
                MessageFile.ParseMessages(currentTab, File.ReadAllLines(currentTab.msgFilePath, Settings.EncCodePage));
            }

            foreach (var nodeTE in currentTab.nodeFlowchartTE)
            {
                if (nodeTE.NodeName == proc.name) {
                    nodeTE.Activate();
                    return;
                }
            }

            ScriptEditor.TextEditorUI.Function.DialogFunctionsRules.BuildOpcodesDictionary();

            FlowchartTE nodeEditor = new FlowchartTE(proc, currentTab);
            nodeEditor.Disposed += delegate(object s, EventArgs e1) { currentTab.nodeFlowchartTE.Remove((FlowchartTE)s); };
            nodeEditor.ApplyCode += new EventHandler<FlowchartTE.CodeArgs>(nodeEditor_ApplyCode);
            nodeEditor.ShowEditor(this);

            currentTab.nodeFlowchartTE.Add(nodeEditor);
        }

        private void nodeEditor_ApplyCode(object sender, FlowchartTE.CodeArgs e)
        {
            if (e.Change) {
                if (Utilities.ReplaceProcedureCode(currentDocument, currentTab.parseInfo, e.Name, e.Code)) {
                    MessageBox.Show("In the source script, there is no dialog node with this name.", "Apply code error");
                    return;
                }
                e.Change = false;
                ForceParseScript();
            }
        }
        #endregion

        #region Misc Control

        private void ShowTabsSpaces()
        {
            if (currentTab == null)
                return;

            if (Path.GetExtension(currentTab.filename).ToLowerInvariant() != ".msg")
                currentDocument.TextEditorProperties.ShowSpaces = showTabsAndSpacesToolStripMenuItem.Checked;

            currentDocument.TextEditorProperties.ShowTabs = showTabsAndSpacesToolStripMenuItem.Checked;;
            currentTab.textEditor.Refresh();
        }

        private void SizeFontToString()
        {
            // base 10 (min 5,  max 30)
            float percent = (float)((10 + Settings.sizeFont) / 10.0f) * 100.0f;
            FontSizeStripStatusLabel.Text = percent.ToString() + '%';

            if (currentTab != null) {
                var fontName = currentTab.textEditor.TextEditorProperties.Font.Name;
                var font = new Font(fontName, 10.0f + Settings.sizeFont, FontStyle.Regular);
                currentTab.textEditor.TextEditorProperties.Font = font;
                currentTab.textEditor.Refresh();
                currentActiveTextAreaCtrl.Caret.RecreateCaret();
            }
        }

        public void SetFocusDocument()
        {
            TextArea_SetFocus(null, null);
        }

        private Control FindFocus(Control cnt)
        {
            if (cnt == null)
                return null;

            foreach (Control c in cnt.Controls)
            {
                if (c.CanFocus && c.Focused)
                    return c;

                Control fc = FindFocus(c);

                if (fc != null)
                    return fc;
            }
            return null;
        }

        private void SetProjectFolderText()
        {
            tslProject.Text = "Project: " + Settings.solutionProjectFolder;
            tslProject.Enabled = true;
        }
        #endregion

        #region Log

        private void MaximizeLog()
        {
            if (currentTab == null && splitContainer1.Panel2Collapsed) {
                showLogWindowToolStripMenuItem.Checked = true;
                splitContainer1.Panel2Collapsed = false;
            }
            if (minimizeLogSize == 0) return;

            if (Settings.editorSplitterPosition == -1) {
                Settings.editorSplitterPosition = Size.Height - (Size.Height / 4);
            }
            splitContainer1.SplitterDistance = Settings.editorSplitterPosition;
            minimizeLogSize = 0;
        }

        private void UpdateLog()
        {
            if (autoRefreshToolStripMenuItem.Checked &&
               (currentTab.parserErrors.Count > 0 || currentTab.buildErrors.Count > 0))
            {
                OutputErrorLog(currentTab);
            } else {
                if (Settings.enableParser)
                    tbOutputParse.Text = currentTab.parserLog;

                if (currentTab.buildLog != null)
                    tbOutput.Text = currentTab.buildLog;
            }
        }

        public void PrintBuildLog(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            tbOutput.BeginInvoke((MethodInvoker)(() =>
                tbOutput.AppendText(e.Data + Environment.NewLine))
            );
        }
        #endregion
    }
}
