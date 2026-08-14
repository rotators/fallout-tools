using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI;

using ScriptEditor.SyntaxRules;

namespace ScriptEditor
{
    partial class TextEditor
    {
        private const string parseoff = "Parser: Disabled";

        public event EventHandler ParserUpdatedInfo; // Event for update nodes diagram
        internal TabInfo LastParserUpdatedTab { get; private set; }

        private bool firstParse;

        internal static volatile bool parserIsRunning;
        internal static bool parsingErrors = true;

        private DateTime extParser_TimeNext, intParser_TimeNext;
        private Timer extParserTimer, intParserTimer;
        private WorkerArgs activeParserArgs;

        #region Parser Control
        private void textChanged(object sender, EventArgs e)
        {
            TabInfo changedTab = currentTab;
            ICSharpCode.TextEditor.TextEditorControl editor = sender as ICSharpCode.TextEditor.TextEditorControl;
            if (editor != null)
                changedTab = tabs.FirstOrDefault(t => t.textEditor == editor);
            if (changedTab == null || savingRunning || changedTab.DisableParseAndStatusChange)
                return;

            changedTab.MarkTextChanged();
            Error.ClearBuildErrorMarkers(changedTab);

            if (!changedTab.changed) {
                changedTab.changed = true;
                if (changedTab.index >= 0)
                    SetTabTextChange(changedTab.index);
            }
            if (sender != null && changedTab.shouldParse) {
                if (!changedTab.needsParse) {
                    changedTab.needsParse = true;
                    if (currentTab == changedTab)
                        parserLabel.Text = "Parser: Update changes";
                }
                if (currentTab == changedTab)
                    ParseScript(3);
            }
        }

        // Parse first open script
        private void FirstParseScript(TabInfo cTab)
        {
            cTab.textEditor.Document.ExtraWordList = new HighlightExtraWord();

            tbOutputParse.Text = string.Empty;

            firstParse = true;
            try {
                GetMacros.GetGlobalMacros(Settings.pathHeadersFiles);

                DEBUGINFO("First Parse...");
                new ParserInternal(cTab, this);

                var ExtParser = new ParserExternal(firstParse);
                cTab.parseInfo = ExtParser.Parse(cTab.textEditor.Text, cTab.filepath, cTab.parseInfo);
                DEBUGINFO("External first parse status: " + ExtParser.LastStatus);

                HighlightProcedures.AddAllToList(cTab.textEditor.Document, cTab.parseInfo.procs);
                CodeFolder.UpdateFolding(cTab.textEditor.Document, cTab.filename, cTab.parseInfo.procs);
                CodeFolder.GetProceduresCollapse(cTab.textEditor.Document, cTab.filename);

                GetParserErrorLog(cTab);

                if (cTab.parseInfo.parseError) {
                    tabControl2.SelectedIndex = 2;
                    if (WindowState != FormWindowState.Minimized) MaximizeLog();
                }
            } catch (Exception ex) {
                cTab.needsParse = true;
                parserLabel.Text = "Parser: Error while processing incomplete code";
                parserLabel.ForeColor = Color.Crimson;
                DEBUGINFO("Initial parser error: " + ex);
            } finally {
                parserIsRunning = false;
                firstParse = false;
            }
        }

        // Parse script
        private void ParseScript(int delay = 2)
        {
            if (!Settings.enableParser) { // Parse Off
                int iDelay = 0;
                if (delay > 1) iDelay = delay / 2;
                intParser_TimeNext = DateTime.Now + TimeSpan.FromSeconds(iDelay);
                if (!intParserTimer.Enabled) intParserTimer.Start();
            } else {
                intParser_TimeNext = DateTime.Now + TimeSpan.FromMilliseconds(100);
                if (!intParserTimer.Enabled) intParserTimer.Start();
            }
            // Запустить так-же и внешний парсер (для полученния макросов)
            extParser_TimeNext = DateTime.Now + TimeSpan.FromSeconds(delay);
            if (!extParserTimer.Enabled) extParserTimer.Start(); // External Parser begin
        }

        //Force update parser data
        private void ForceParseScript()
        {
            ForceParseScript(currentTab);
        }

        private void ForceParseScript(TabInfo tab)
        {
            if (tab == null || tab.index < 0 || tab.parseInfo == null)
                return;

            // останавливаем ранее сработавшие таймеры
            intParserTimer.Stop();
            extParserTimer.Stop();

            if (bwSyntaxParser.IsBusy || parserIsRunning) {
                tab.needsParse = true;
                if (currentTab == tab)
                    ParseScript(0);
                return;
            }

            if (Settings.enableParser && tab.parseInfo.parseData) {
                parserIsRunning = true; // parse work
                CodeFolder.UpdateFolding(tab.textEditor.Document, tab.filepath);
                bwSyntaxParser.RunWorkerAsync(new WorkerArgs(tab.textEditor.Document.TextContent, tab));
            } else {
                try {
                    new ParserInternal(tab, this);
                    CodeFolder.UpdateFolding(tab.textEditor.Document, tab.filename, tab.parseInfo.procs);
                    ParserCompleted(tab, false);
                } catch (Exception ex) {
                    tab.needsParse = true;
                    if (currentTab == tab) {
                        parserLabel.Text = "Parser: Error while processing incomplete code";
                        parserLabel.ForeColor = Color.Crimson;
                    }
                    DEBUGINFO("Forced parser error: " + ex);
                } finally {
                    parserIsRunning = false;
                }
            }
        }

        // Delay timer for internal parsing
        void InternalParser_Tick(object sender, EventArgs e)
        {
            if (currentTab == null || !currentTab.shouldParse) {
                intParserTimer.Stop();
                DEBUGINFO("Stop: Internal Parser");
                return;
            }

            if (DateTime.Now >= intParser_TimeNext && !parserIsRunning) {
                intParserTimer.Stop();

                DEBUGINFO("Run: Internal Parser");

                try {
                    if (!Settings.enableParser) { // Parser off
                        tbOutputParse.Text = string.Empty;
                        parserLabel.Text = "Parser: Get only macros";
                        parserLabel.ForeColor = Color.Crimson;

                        new ParserInternal(currentTab, this);
                        CodeFolder.UpdateFolding(currentDocument, currentTab.filename, currentTab.parseInfo.procs);
                        ParserCompleted(currentTab, false);
                    } else {
                        CodeFolder.UpdateFolding(currentDocument, currentTab.filepath);
                        //Quick update procedure data
                        ParserInternal.UpdateProcInfo(ref currentTab.parseInfo, currentDocument.TextContent, currentTab.filepath);
                    }
                } catch (Exception ex) {
                    parserIsRunning = false;
                    currentTab.needsParse = true;
                    parserLabel.Text = "Parser: Error while processing incomplete code";
                    parserLabel.ForeColor = Color.Crimson;
                    DEBUGINFO("Internal parser error: " + ex);
                }
            }
        }

        // Timer for external parsing
        void ExternalParser_Tick(object sender, EventArgs e)
        {
            if (currentTab == null || !currentTab.shouldParse) {
                extParserTimer.Stop();
                DEBUGINFO("Stop: External Parser");
                return;
            }

            if (DateTime.Now >= extParser_TimeNext && !bwSyntaxParser.IsBusy && !parserIsRunning) {
                if (autoComplete.IsVisible) return;
                parserIsRunning = true;
                extParserTimer.Stop();

                DEBUGINFO("Run: External Parser");

                if (Settings.enableParser) {
                    parserLabel.Text = "Parser: Working";
                    parserLabel.ForeColor = Color.Crimson;
                }
                bwSyntaxParser.RunWorkerAsync(new WorkerArgs(currentDocument.TextContent, currentTab));
            }
        }

        // External parse start
        private void bwSyntaxParser_DoWork(object sender, DoWorkEventArgs eventArgs)
        {
            WorkerArgs args = (WorkerArgs)eventArgs.Argument;
            activeParserArgs = args;
            if (bwSyntaxParser.CancellationPending) {
                eventArgs.Cancel = true;
                return;
            }
            try {
                var ExtParser = new ParserExternal(false);
                args.parseInfo = ExtParser.Parse(args.text, args.tab.filepath, args.previousParseInfo);
                args.status = ExtParser.LastStatus;
                //args.parseIsFail = prevStatus & (args.status > 0);
                if (bwSyntaxParser.CancellationPending)
                    eventArgs.Cancel = true;
                else
                    eventArgs.Result = args;
            } finally {
                parserIsRunning = false;
            }
        }

        // External parse finish
        private void bwSyntaxParser_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            parserIsRunning = false;
            WorkerArgs args = e.Error == null && !e.Cancelled ? e.Result as WorkerArgs : activeParserArgs;
            activeParserArgs = null;

            if (isClosing || IsDisposed || Disposing || e.Cancelled) return;
            if (e.Error != null) {
                if (args != null && args.tab != null) args.tab.needsParse = true;
                if (args == null || currentTab == args.tab) {
                    parserLabel.Text = "Parser: Error while processing incomplete code";
                    parserLabel.ForeColor = Color.Crimson;
                }
                DEBUGINFO("Parser error: " + e.Error);
                return;
            }
            if (!Settings.enableParser) return; // выход для предотвращения второго прохода когда внешний парсер выключен

            if (args == null || args.tab == null) {
                DEBUGINFO("Parser error: Background parser returned no result.");
                return;
            }
            if (!args.IsCurrent) {
                args.tab.needsParse = true;
                DEBUGINFO("Discarded stale parser result for document revision " + args.textRevision + ".");
                if (currentTab == args.tab)
                    ParseScript(0);
                return;
            }
            if (args.parseInfo != null)
                args.tab.parseInfo = args.parseInfo;
            DEBUGINFO(">>> Ext parse status: " + args);
            ParserCompleted(args.tab, args.parseIsFail);
        }

        private void ParserCompleted(TabInfo tab, bool parseIsFail)
        {
            if (currentTab == tab) {
                //Procedure[] procs = null;
                //if (parseIsFail) { // предыдущая попытка парсинга была неудачной
                //    procs = ParserInternal.GetProcsData(tab.textEditor.Text, tab.filepath); // обновить данные об имеющихся процедур (для чего?)
                //}
                HighlightProcedures.UpdateList(tab.textEditor.Document, tab.parseInfo.procs); //(!parseIsFail) ? tab.parseInfo.procs : procs
                UpdateNames(); // Update Tree Variables/Procedures

                if (tab.filepath != null) {
                    if (tab.parseInfo.parseData) { //.parsed
                        if (tab.textEditor.Document.FoldingManager.FoldMarker.Count > 0) //tab.parseInfo.procs.Length
                            Outline_toolStripButton.Enabled = true;

                        if (Settings.enableParser)
                            parserLabel.Text = (!tab.parseInfo.parseError) ? "Parser: Complete" : "Parser: Script syntax error (see parser errors log)";
                        else
                            parserLabel.Text = parseoff + " [Get only macros]";
                    } else {
                        parserLabel.Text = (Settings.enableParser) ? "Parser: Failed script parsing (see parser errors log)" : parseoff + " [Get only macros]";
                        //currentTab.needsParse = true; // требуется обновление
                    }
                    tab.needsParse = false;
                } else {
                    parserLabel.Text = (Settings.enableParser) ? "Parser: Get only local macros" : parseoff;
                }
            }
            GetParserErrorLog(tab);
            // Event for update
            LastParserUpdatedTab = tab;
            try {
                if (ParserUpdatedInfo != null) ParserUpdatedInfo(this, EventArgs.Empty);
            } finally {
                LastParserUpdatedTab = null;
            }
        }
        #endregion

        #region Parser Log
        private void GetParserErrorLog(TabInfo tab)
        {
            string log = String.Empty;
            if (File.Exists("errors.txt")) {
                try {
                    log = File.ReadAllText("errors.txt", System.Text.Encoding.Default);
                    File.Delete("errors.txt");
                } catch (IOException) {
                    //в случаях ошибки в parser.dll, не освобождается созданный им файл, что приводит к ошибке доступа
                    File.Copy("errors.txt", "parser.log");
                    log = File.ReadAllText("parser.log", System.Text.Encoding.Default);
                    File.Delete("parser.log");
                }
            }
            tab.parserLog = Error.ParserLog(log, tab);

            // Когда установлена опция и происходит первый парсинг - не обновлять лог ошибок
            if (!firstParse || !autoRefreshToolStripMenuItem.Checked) OutputErrorLog(tab);
        }

        private void OutputErrorLog(TabInfo tab)
        {
            dgvErrors.Rows.Clear();
            if (Settings.enableParser) {
                tbOutputParse.Text = tab.parserLog;
                if (tsmShowParserLog.Checked) {
                    foreach (Error err in tab.parserErrors)
                        dgvErrors.Rows.Add(err.type.ToString(), Path.GetFileName(err.fileName), err.line, err);
                }
            }
            if (tab.buildLog != null) {
                tbOutput.Text = tab.buildLog;
                if (tsmShowBuildLog.Checked && tab.buildErrors.Count > 0) {
                    dgvErrors.Rows.Add("Build Log");
                    dgvErrors.Rows[dgvErrors.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Gainsboro;
                    foreach (Error err in tab.buildErrors)
                        dgvErrors.Rows.Add(err.type.ToString(), Path.GetFileName(err.fileName), err.line, err);
                }
            }
        }

        public void intParserPrint(string info)
        {
            if (!Settings.enableParser) {
                tbOutputParse.BeginInvoke((MethodInvoker)(() => tbOutputParse.Text = info + tbOutputParse.Text));
            }
        }
        #endregion
    }
}
