using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI;
using ScriptEditor.TextEditorUtilities;

using ScriptEditor.SyntaxRules;

namespace ScriptEditor
{
    partial class TextEditor
    {
        #region Search Function
        private bool SubSearchInternal(List<int> offsets, List<int> lengths)
        {
            AddSearchTextComboBox(sf.cbSearch.Text);

            RegexOptions option = RegexOptions.None;
            Regex regex = null;

            if (!sf.cbCase.Checked) option = RegexOptions.IgnoreCase;

            if (sf.cbRegular.Checked)
                regex = new Regex(sf.cbSearch.Text, option);
            else if (Settings.searchWholeWord)
                regex = new Regex(@"\b" + sf.cbSearch.Text + @"\b", option);

            if (sf.rbFolder.Checked && (Settings.lastSearchPath == null || !Directory.Exists(Settings.lastSearchPath))) {
                MessageBox.Show("No search path set.", "Error");
                return false;
            }
            if (!sf.cbFindAll.Checked) {
                if (sf.rbCurrent.Checked || (sf.rbAll.Checked && tabs.Count < 2)) {
                    if (currentTab == null)
                        return false;
                    if (Utilities.SearchAndScroll(currentActiveTextAreaCtrl, regex, sf.cbSearch.Text, sf.cbCase.Checked, ref PosChangeType))
                        return true;
                } else if (sf.rbAll.Checked) {
                    int starttab = currentTab == null ? 0 : currentTab.index;
                    int endtab = starttab == 0 ? tabs.Count - 1 : starttab - 1;
                    int tab = starttab - 1;
                    int caretOffset = currentActiveTextAreaCtrl.Caret.Offset;
                    do {
                        if (++tab == tabs.Count)
                            tab = 0; //restart tab
                        int start, len;
                        if (Utilities.Search(tabs[tab].textEditor.Text, sf.cbSearch.Text, regex, caretOffset + 1, false, sf.cbCase.Checked, out start, out len)) {
                            Utilities.FindSelected(tabs[tab].textEditor.ActiveTextAreaControl, start, len, ref PosChangeType);
                            if (currentTab == null || currentTab.index != tab)
                                tabControl1.SelectTab(tab);
                            return true;
                        }
                        caretOffset = 0; // search from begin
                    } while (tab != endtab);
                } else {
                    sf.lbFindFiles.Items.Clear();
                    sf.lbFindFiles.Tag = regex;
                    List<string> files = sf.GetFolderFiles();
                    ProgressBarForm progress = new ProgressBarForm(this, files.Count, "Search matches...");
                    for (int i = 0; i < files.Count; i++)
                    {
                        if (Utilities.Search(File.ReadAllText(files[i]), sf.cbSearch.Text, regex, sf.cbCase.Checked))
                            sf.lbFindFiles.Items.Add(files[i]);
                        progress.SetProgress = i;
                    }
                    progress.Dispose();
                    sf.labelCount.Text = sf.lbFindFiles.Items.Count.ToString();
                    if (sf.lbFindFiles.Items.Count > 0) {
                        if (sf.Height < 500) sf.Height = 500;
                        return true;
                    }
                }
            } else {
                DataGridView dgv = CommonDGV.DataGridCreate();
                dgv.DoubleClick += dgvErrors_DoubleClick;

                if (sf.rbCurrent.Checked || (sf.rbAll.Checked && tabs.Count < 2)) {
                    if (currentTab == null)
                        return false;
                    Utilities.SearchForAll(currentTab, sf.cbSearch.Text, regex, sf.cbCase.Checked, dgv, offsets, lengths);
                } else if (sf.rbAll.Checked) {
                    for (int i = 0; i < tabs.Count; i++)
                        Utilities.SearchForAll(tabs[i], sf.cbSearch.Text, regex, sf.cbCase.Checked, dgv, offsets, lengths);
                } else {
                    List<string> files = sf.GetFolderFiles();
                    ProgressBarForm progress = new ProgressBarForm(this, files.Count, "Search matches...");
                    for (int i = 0; i < files.Count; i++) {
                        Utilities.SearchForAll(File.ReadAllLines(files[i]), Path.GetFullPath(files[i]), sf.cbSearch.Text, regex, sf.cbCase.Checked, dgv);
                        progress.SetProgress = i;
                    }
                    progress.Dispose();
                }
                if (dgv.RowCount > 0) {
                    TabPage tp = new TabPage("Search results");
                    tp.ToolTipText = "Find text: " + sf.cbSearch.Text;
                    tp.Controls.Add(dgv);
                    dgv.Dock = DockStyle.Fill;
                    tabControl2.TabPages.Add(tp);
                    tabControl2.SelectTab(tp);
                    MaximizeLog();
                    return true;
                }
            }
            MessageBox.Show("Search string not found", "Search");
            return false;
        }
        #endregion

        #region Search & Replace function form
        private string lastSearchText = string.Empty;

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string searchText = lastSearchText;
            if (sf == null) {
                sf = new SearchForm();
                sf.Owner = this;

                sf.FormClosed += delegate(object a1, FormClosedEventArgs a2) {
                    lastSearchText = sf.cbSearch.Text;
                    sf = null;
                };

                sf.lbFindFiles.MouseDoubleClick += delegate (object a1, MouseEventArgs a2) {
                    if (sf.lbFindFiles.Items.Count == 0) return;

                    string file = sf.lbFindFiles.SelectedItem.ToString();

                    TabInfo tab = CheckTabs(tabs, file); // проверить открыт ли уже этот файл
                    bool isOpen = (tab != null);
                    if (!isOpen) tab = Open(file, OpenType.File, false);

                    Utilities.SearchAndScroll(tab.textEditor.ActiveTextAreaControl, (Regex)sf.lbFindFiles.Tag,
                                              sf.cbSearch.Text, sf.cbCase.Checked, ref PosChangeType, false);

                    if (isOpen) SwitchToTab(tab.index);
                };

                sf.bSearch.Click += new EventHandler(bSearch_Click);
                sf.bReplace.Click += new EventHandler(bReplace_Click);

                sf.cbSearch.Items.AddRange(SearchTextComboBox.Items.Cast<String>().ToArray());
            } else {
                sf.WindowState = FormWindowState.Normal;
                sf.Focus();

                searchText = sf.cbSearch.Text;
            }

            if (currentTab != null && currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected) {
                searchText = currentActiveTextAreaCtrl.SelectionManager.SelectedText;
            }
            if (searchText.Length == 0) {
                searchText = Clipboard.GetText();
            }
            if (searchText.Length > 0 && searchText.Length < 255) {
                sf.cbSearch.Text = searchText;
            }
            sf.Show();
            sf.cbSearch.Focus();
            sf.cbSearch.SelectAll();
        }

        private void bSearch_Click(object sender, EventArgs e)
        {
            sf.cbSearch.Text = sf.cbSearch.Text.Trim();
            if (sf.cbSearch.Text.Length == 0)
                return;
            SubSearchInternal(null, null);
        }

        void bReplace_Click(object sender, EventArgs e)
        {
            sf.cbSearch.Text = sf.cbSearch.Text.Trim();
            if (sf.rbFolder.Checked || sf.cbSearch.Text.Length == 0)
                return;
            if (sf.cbFindAll.Checked) {
                List<int> lengths = new List<int>(), offsets = new List<int>();
                if (!SubSearchInternal(offsets, lengths))
                    return;
                for (int i = offsets.Count - 1; i >= 0; i--)
                {
                    currentDocument.Replace(offsets[i], lengths[i], sf.tbReplace.Text);
                }
            } else {
                currentActiveTextAreaCtrl.Caret.Column--;
                if (!SubSearchInternal(null, null))
                    return;
                ISelection selected = currentActiveTextAreaCtrl.SelectionManager.SelectionCollection[0];
                currentDocument.Replace(selected.Offset, selected.Length, sf.tbReplace.Text);
                selected.EndPosition = new TextLocation(selected.StartPosition.Column + sf.tbReplace.Text.Length, selected.EndPosition.Line);
                currentActiveTextAreaCtrl.SelectionManager.SetSelection(selected);
            }
        }

        // Search for quick panel
        private void FindForwardButton_Click(object sender, EventArgs e)
        {
            string find = SearchTextComboBox.Text.Trim();
            if (find.Length == 0 || currentTab == null)
                return;
            int z = Utilities.SearchPanel(currentTab.textEditor.Text, find, currentActiveTextAreaCtrl.Caret.Offset + 1,
                                            CaseButton.Checked, WholeWordButton.Checked);
            if (z != -1)
                Utilities.FindSelected(currentActiveTextAreaCtrl, z, find.Length, ref PosChangeType);
            else
                DontFind.Play();
            AddSearchTextComboBox(find);
        }

        private void FindBackButton_Click(object sender, EventArgs e)
        {
            string find = SearchTextComboBox.Text.Trim();
            if (find.Length == 0 || currentTab == null)
                return;
            int offset = currentActiveTextAreaCtrl.Caret.Offset;
            string text = currentTab.textEditor.Text.Remove(offset);
            int z = Utilities.SearchPanel(text, find, offset - 1, CaseButton.Checked, WholeWordButton.Checked, true);
            if (z != -1)
                Utilities.FindSelected(currentActiveTextAreaCtrl, z, find.Length, ref PosChangeType);
            else
                DontFind.Play();
            AddSearchTextComboBox(find);
        }

        private void ReplaceButton_Click(object sender, EventArgs e)
        {
            string find = SearchTextComboBox.Text.Trim();
            if (find.Length == 0)
                return;
            string replace = ReplaceTextBox.Text.Trim();
            int z = Utilities.SearchPanel(currentTab.textEditor.Text, find, currentActiveTextAreaCtrl.Caret.Offset,
                                            CaseButton.Checked, WholeWordButton.Checked);
            if (z != -1)
                Utilities.FindSelected(currentActiveTextAreaCtrl, z, find.Length, ref PosChangeType, replace);
            else
                DontFind.Play();
            AddSearchTextComboBox(find);
        }

        private void ReplaceAllButton_Click(object sender, EventArgs e)
        {
            string find = SearchTextComboBox.Text.Trim();
            if (find.Length == 0)
                return;

            string replace = ReplaceTextBox.Text.Trim();
            int z, offset = 0;
            do {
                z = Utilities.SearchPanel(currentTab.textEditor.Text, find, offset,
                                            CaseButton.Checked, WholeWordButton.Checked);
                if (z != -1)
                    currentActiveTextAreaCtrl.Document.Replace(z, find.Length, replace);
                offset = z + 1;
            } while (z != -1);
            AddSearchTextComboBox(find);
        }

        private void SendtoolStripButton_Click(object sender, EventArgs e)
        {
            string word = currentActiveTextAreaCtrl.SelectionManager.SelectedText;
            if (word == string.Empty)
                word = TextUtilities.GetWordAt(currentDocument, currentActiveTextAreaCtrl.Caret.Offset);
            if (word != string.Empty)
                SearchTextComboBox.Text = word;
        }

        private void quickFindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;

            SendtoolStripButton.PerformClick();
            FindForwardButton.PerformClick();
            if (!SearchToolStrip.Visible) {
                SearchToolStrip.Visible = true;
                TabClose_button.Top += (SearchToolStrip.Visible) ? 25 : -25;
            }
        }

        private void Search_Panel(object sender, EventArgs e)
        {
            if (currentTab == null && !SearchToolStrip.Visible) {
                findToolStripMenuItem_Click(null, null);
                return;
            }
            SearchToolStrip.Visible = !SearchToolStrip.Visible;
            TabClose_button.Top += (SearchToolStrip.Visible) ? 25 : -25;
        }
        #endregion

        #region References/Decleration/Definition & Include function
        private void findReferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextLocation tl = currentActiveTextAreaCtrl.Caret.Position; //(TextLocation)editorMenuStrip.Tag;
            string word = TextUtilities.GetWordAt(currentDocument, currentDocument.PositionToOffset(tl));

            Reference[] refs = currentTab.parseInfo.LookupReferences(word, currentTab.filepath, tl.Line);
            if (refs == null)
                return;
            if (refs.Length == 0) {
                MessageBox.Show("No references found", "Reference", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataGridView dgv = CommonDGV.DataGridCreate();
            dgv.DoubleClick += dgvErrors_DoubleClick;

            int lastLine = 0, nextColumn = 0;
            foreach (var r in refs)
            {
                if (lastLine != r.line) nextColumn = 0; 
                Error error = new Error(ErrorType.Search) {
                    fileName = r.file,
                    line = r.line,
                    column = TextUtilities.GetLineAsString(currentDocument, r.line - 1).IndexOf(word, nextColumn, StringComparison.OrdinalIgnoreCase),
                    len = word.Length,
                    message = (String.Compare(Path.GetFileName(r.file), currentTab.filename, true) == 0)
                               ? TextUtilities.GetLineAsString(currentDocument, r.line - 1).TrimStart()
                               : "< Preview is not possible: for viewing goto this the reference link >"
                };
                lastLine = r.line;
                nextColumn = error.column + word.Length;
                if (error.column > 0)
                    error.column++;
                dgv.Rows.Add(r.file, r.line, error);
            }

            TabPage tp = new TabPage("'" + word + "' references");
            tp.Controls.Add(dgv);
            dgv.Dock = DockStyle.Fill;
            tabControl2.TabPages.Add(tp);
            tabControl2.SelectTab(tp);
            MaximizeLog();
            TextArea_SetFocus(null, null);
        }

        private void findDeclerationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextLocation tl = currentActiveTextAreaCtrl.Caret.Position; //(TextLocation)editorMenuStrip.Tag;
            string word = TextUtilities.GetWordAt(currentDocument, currentDocument.PositionToOffset(tl));
            string file;
            int line;
            currentTab.parseInfo.LookupDecleration(word, currentTab.filepath, tl.Line, out file, out line);
            SelectLine(file, line);
        }

        private void findDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null) return;

            string word, file = currentTab.filepath;
            int line;
            TextLocation tl = currentActiveTextAreaCtrl.Caret.Position;
            if (((ToolStripDropDownItem)sender).Tag != null) { // for "Button"
                if (!currentTab.shouldParse)
                    return;

                ParserInternal.UpdateParseBuffer(currentTab.textEditor.Text);

                word = TextUtilities.GetWordAt(currentDocument, currentDocument.PositionToOffset(tl));
                line = ParserInternal.GetProcedureBlock(word, 0, true).begin;
                if (line != -1)
                    line++;
                else
                    return;
            } else {
                //TextLocation tl = (TextLocation)editorMenuStrip.Tag;
                word = TextUtilities.GetWordAt(currentDocument, currentDocument.PositionToOffset(tl));
                currentTab.parseInfo.LookupDefinition(word, out file, out line);
            }
            SelectLine(file, line);
        }

        private void openIncludeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextLocation tl = currentActiveTextAreaCtrl.Caret.Position; //(TextLocation)editorMenuStrip.Tag;
            string[] line = TextUtilities.GetLineAsString(currentDocument, tl.Line).Split('"');
            if (line.Length < 2)
                return;

            if (!Settings.searchIncludePath && currentTab.filepath == null && !Path.IsPathRooted(line[1])) {
                MessageBox.Show("Cannot open includes given via a relative path for an unsaved script", "Error");
                return;
            }
            if (line[1].IndexOfAny(Path.GetInvalidPathChars()) != -1) return;
            ParserInternal.GetIncludePath(ref line[1], Path.GetDirectoryName(currentTab.filepath));
            if (Open(line[1], OpenType.File, false) == null)
                MessageBox.Show("Header file not found!\n" + line[1], null, MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Refactor.Rename((IParserInfo)renameToolStripMenuItem.Tag, currentDocument, currentTab, tabs);
        }
        #endregion

        #region Autocomplete and tips function control
        private void TextArea_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //PosChangeType = PositionType.OverridePos; // Save position change for navigation, if key was pressed

            if (e.KeyCode == Keys.Enter) updateHighlightPocedure = false;

            if (autoComplete.IsVisible) {
                autoComplete.TA_PreviewKeyDown(e);
                if (Settings.autocomplete && e.KeyCode == Keys.Back) {
                    autoComplete.GenerateList(String.Empty, currentTab,
                        currentActiveTextAreaCtrl.Caret.Offset - 1, toolTips.Tag, true);
                }
            }
            if (toolTips.Active) {
                if (e.KeyCode == Keys.Up || (e.KeyCode == Keys.Down && !autoComplete.IsVisible)
                    || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape
                    || (toolTips.Tag != null && !(bool)toolTips.Tag)) {
                        ToolTipsHide();
                }
                else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) {
                    int caret = currentActiveTextAreaCtrl.Caret.Offset;
                    int offset = caret;
                    if (e.KeyCode == Keys.Left)
                        caret--;
                    else {
                        caret++;
                        offset = TextUtilities.SearchBracketForward(currentDocument, showTipsColumn + 1, '(', ')');
                    }
                    if (showTipsColumn >= caret || caret > offset) ToolTipsHide();
                }
            }
            if (e.KeyCode == Keys.Tab) { // Закрытие списка, если нажата клавиша таб после ключевого слова
                if (Utilities.AutoCompleteKeyWord(currentActiveTextAreaCtrl)) {
                    e.IsInputKey = true;
                    autoComplete.ShiftCaret = false;
                    if (autoComplete.IsVisible)
                        autoComplete.Close();
                }
            }
        }

        private void VScrollBar_ValueChanged(object sender, EventArgs e)
        {
            autoComplete.TA_MouseScroll(currentTab.textEditor.ActiveTextAreaControl);
            if (toolTips.Active) ToolTipsHide();
        }

        private void ToolTipsHide()
        {
            if (autoComplete.IsVisible && (bool)toolTips.Tag)
                autoComplete.Close();

            toolTips.Hide(panel1);
            toolTips.Tag = toolTips.Active = false;
        }

        private void TextEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) {
                autoComplete.Hide();
                ctrlKeyPress = true;
            }
        }

        private void TextEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) {
                autoComplete.UnHide();
                ctrlKeyPress = false;
            }
        }

        private void showAutocompleteWordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab != null && Settings.autocomplete) {
                Caret caret = currentActiveTextAreaCtrl.Caret;
                if (!ColorTheme.CheckColorPosition(currentDocument, caret.Position))
                    autoComplete.GenerateList(String.Empty, currentTab, caret.Offset, null);
            }
        }

        //
        private void ShowCodeTips(string tipText, Caret caret, int duration, bool tag = false)
        {
            int offset = TextUtilities.FindWordStart(currentDocument, caret.Offset - 1);
            offset = caret.Offset - offset;
            Point pos = caret.GetScreenPosition(caret.Line, caret.Column - offset);
            pos.Offset(currentActiveTextAreaCtrl.FindForm().PointToClient(
                       currentActiveTextAreaCtrl.Parent.PointToScreen(currentActiveTextAreaCtrl.Location)));
            offset = (autoComplete.IsVisible) ? -25 : 20;
            pos.Offset(0, offset);

            if (tag) showTipsColumn = caret.Offset;

            toolTips.Active = true;
            toolTips.Tag = tag;
            toolTips.Show(tipText, panel1, pos, duration);
        }

        private int  inputPairedBrackets = 0;
        private char keyPressChar;

        private void TextArea_KeyPressed(object sender, KeyPressEventArgs e)
        {
            keyPressChar = e.KeyChar;
            var caret = currentActiveTextAreaCtrl.Caret;

            if (Settings.autoInputPaired && e.KeyChar == '"') {
                List<Utilities.Quote> quotes = new  List<Utilities.Quote>();
                Utilities.GetQuotesPosition(TextUtilities.GetLineAsString(currentDocument, caret.Line), quotes);
                // skiping quotes "..." region
                bool inQuotes = false;
                foreach (Utilities.Quote q in quotes)
                {
                    if (caret.Column > q.Open && caret.Column < q.Close) {
                        inQuotes = true;
                        break;
                    }
                }
                if (!inQuotes) {
                    char chR = currentDocument.GetCharAt(caret.Offset);
                    char chL = currentDocument.GetCharAt(caret.Offset - 1);
                    if ((chL == '(' && chR == ')') || (chL != '"' && (chR == ' ' || chR == '\r') && !Char.IsLetterOrDigit(chL)))
                        currentDocument.Insert(caret.Offset, "\"");
                    else if (chL == '"' && chR == '"')
                        currentDocument.Remove(caret.Offset, 1);
                }
            }
            else if (e.KeyChar == '(' || e.KeyChar == '[' || e.KeyChar == '{') {
                if (autoComplete.IsVisible) autoComplete.Close();
                if (e.KeyChar == '{') return;

                if (Settings.showTips && currentTab.parseInfo != null && e.KeyChar == '(') {
                    string word = TextUtilities.GetWordAt(currentDocument, caret.Offset - 1);
                    if (word != String.Empty) {
                        string item = ProgramInfo.LookupOpcodesToken(word);
                        if (item != null) {
                            int z = item.IndexOf('\n');
                            if (z > 0)
                                item = item.Remove(z);
                        }
                        if (item == null)
                            item = currentTab.parseInfo.LookupToken(word, null, 0, true);
                        if (item != null)
                            ShowCodeTips(item, caret, 50000, true);
                    }
                }

                if (Settings.autoInputPaired && Char.IsWhiteSpace(currentDocument.GetCharAt(caret.Offset))) {
                    inputPairedBrackets = 2;
                    string bracket = (e.KeyChar == '[') ? "]" : ")";
                    currentDocument.Insert(caret.Offset, bracket);
                }
            } else if (e.KeyChar == ')' || e.KeyChar == ']' || e.KeyChar == '}') {
                if (toolTips.Active) ToolTipsHide();
                if (e.KeyChar == '}') return;

                if (Settings.autoInputPaired && inputPairedBrackets > 0) {
                    char bracket = (e.KeyChar == ']') ? '[' : '(';
                    if (currentDocument.GetCharAt(caret.Offset -1) == bracket && currentDocument.GetCharAt(caret.Offset) == e.KeyChar) {
                        currentDocument.Remove(caret.Offset, 1);
                        // TODO BUG: В контроле баг при использовании TextBuffer - стирается строка символов.
                        //currentDocument.TextBufferStrategy.Remove(caret.Offset, 1);
                        //currentActiveTextAreaCtrl.TextArea.Refresh();
                    }
                }
            } else {
                if (Settings.autocomplete) {
                    if (!ColorTheme.CheckColorPosition(currentDocument, caret.Position))
                        autoComplete.GenerateList(e.KeyChar.ToString(), currentTab, caret.Offset - 1, toolTips.Tag);
                }
            }
            if (inputPairedBrackets > 0) inputPairedBrackets--;
        }

        void TextArea_KeyUp(object sender, KeyEventArgs e)
        {
            if (!currentTab.shouldParse) return;
            updateHighlightPocedure = true;

            if (e.KeyCode == Keys.OemSemicolon && keyPressChar == ';') Utilities.FormattingCodeSmart(currentActiveTextAreaCtrl);

            if (!Settings.showTips || toolTips.Active || !Char.IsLetter(Convert.ToChar(e.KeyValue))) return;

            var caret = currentActiveTextAreaCtrl.Caret;
            string word = TextUtilities.GetWordAt(currentDocument, caret.Offset - 1);
            if (word != String.Empty) {
                switch (word) {
                    case "for":
                    case "foreach":
                    case "while":
                    case "switch":
                    case "if":
                    case "ifel":
                    case "elif":
                        ShowCodeTips("Press the TAB key to insert the autocode.", caret, 5000);
                        break;
                }
            }
        }
        #endregion

        #region Navigation Back/Forward
        /*
         * AddPos       - Добавлять в историю новую позицию перемещения.
         * NoStore      - Не сохранять следующее перемещение в историю.
         * OverridePos  - Перезаписать позицию перемещения в текущей позиции истории.
         * Disabled     - Не сохранять все последуюшие перемещения в историю (до явного включения функции).
         */
        internal enum PositionType { AddPos, NoStore, OverridePos, Disabled }

        private void SetBackForwardButtonState()
        {
            if (currentTab.history.pointerCur > 0)
                Back_toolStripButton.Enabled = true;
            else
                Back_toolStripButton.Enabled = false;

            if (currentTab.history.pointerCur == currentTab.history.pointerEnd || currentTab.history.pointerCur < 0)
                Forward_toolStripButton.Enabled = false;
            else if (currentTab.history.pointerCur > 0 || currentTab.history.pointerCur < currentTab.history.pointerEnd)
                Forward_toolStripButton.Enabled = true;
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            string ext = Path.GetExtension(currentTab.filename).ToLowerInvariant();
            if (ext != ".ssl" && ext != ".h") return;

            TextLocation _position = currentActiveTextAreaCtrl.Caret.Position;
            int curLine = _position.Line + 1;
            LineStripStatusLabel.Text = "Line: " + curLine;
            ColStripStatusLabel.Text = "Col: " + (_position.Column + 1);

            Utilities.SelectedTextColorRegion(_position, currentActiveTextAreaCtrl);

            if (updateHighlightPocedure) HighlightCurrentPocedure(_position.Line);

            if (PosChangeType == PositionType.Disabled) return;
        PosChange:
            if (PosChangeType >= PositionType.NoStore) { // also OverridePos
                if (PosChangeType == PositionType.OverridePos && currentTab.history.pointerCur != -1)
                    currentTab.history.linePosition[currentTab.history.pointerCur] = _position;

                PosChangeType = PositionType.AddPos; // set default
                return;
            }

            int diff = Math.Abs(curLine - currentTab.history.prevPosition);
            currentTab.history.prevPosition = curLine;
            if (diff > 1) {
                currentTab.history.pointerCur++;
                if (currentTab.history.pointerCur >= currentTab.history.linePosition.Count)
                    currentTab.history.linePosition.Add(_position);
                else
                    currentTab.history.linePosition[currentTab.history.pointerCur] = _position;
                currentTab.history.pointerEnd = currentTab.history.pointerCur;
            } else {
                PosChangeType = PositionType.OverridePos;
                goto PosChange;
            }

            SetBackForwardButtonState();
        }

        private void Back_toolStripButton_Click(object sender, EventArgs e)
        {
            if (currentTab == null || currentTab.history.pointerCur == 0)
                return;

            currentTab.history.pointerCur--;
            GotoViewLine();
        }

        private void Forward_toolStripButton_Click(object sender, EventArgs e)
        {
            if (currentTab == null || currentTab.history.pointerCur >= currentTab.history.pointerEnd)
                return;

            currentTab.history.pointerCur++;
            GotoViewLine();
        }

        private void GotoViewLine()
        {
            PosChangeType = PositionType.NoStore;
            TextLocation _position = currentTab.history.linePosition[currentTab.history.pointerCur];
            currentActiveTextAreaCtrl.Caret.Position = _position;
            currentTab.history.prevPosition = _position.Line + 1;

            int firstLine = currentActiveTextAreaCtrl.TextArea.TextView.FirstVisibleLine;
            int lastLine = firstLine + currentActiveTextAreaCtrl.TextArea.TextView.VisibleLineCount - 1;
            if (_position.Line <= firstLine || _position.Line + 1 >= lastLine)
                currentActiveTextAreaCtrl.CenterViewOn(currentActiveTextAreaCtrl.Caret.Line, 0);

            SetBackForwardButtonState();
        }
        #endregion

        #region Procedure function Create/Rename/Delete/Move
        // Create Handlers Procedures
        public void CreateProcBlock(string name)
        {
            if (currentTab.parseInfo.CheckExistsName(name, false)) {
                MessageBox.Show("A procedure with this name has already been declared.", "Info");
                return;
            }
            byte line = (name == "look_at_p_proc" || name == "description_p_proc") ? (byte)1 : (byte)0;

            ProcForm CreateProcFrm = new ProcForm(name, true);

            if (ProcTree.SelectedNode != null && ProcTree.SelectedNode.Tag is Procedure)
                CreateProcFrm.CopyProcedure = false;
            else
                CreateProcFrm.groupBoxProcedure.Enabled = false;

            ProcTree.HideSelection = false;

            if (CreateProcFrm.ShowDialog() == DialogResult.Cancel) {
                ProcTree.HideSelection = true;
                return;
            }

            InsertAt placeAt = CreateProcFrm.PlaceAt;

            ProcedureBlock block = new ProcedureBlock();
            if (placeAt == InsertAt.After) {
                var proc = currentHighlightProc ?? (Procedure)ProcTree.SelectedNode.Tag;
                if (proc != null) {
                    block.begin  = proc.d.start;
                    block.end    = proc.d.end;
                    block.declar = proc.d.declared;
                } else {
                    placeAt = InsertAt.Caret;
                }
            }
            PrepareInsertProcedure(CreateProcFrm.ProcedureName, block, placeAt, line);

            CreateProcFrm.Dispose();
            ProcTree.HideSelection = true;
        }

        // Create Procedures
        private void createProcedureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null || !currentTab.shouldParse) return;

            string word = null;

            bool IsSelectProcedure = ProcTree.SelectedNode != null && ProcTree.SelectedNode.Tag is Procedure;
            if (IsSelectProcedure)
                word = ProcTree.SelectedNode.Name;
            else if (currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected)
                word = currentActiveTextAreaCtrl.SelectionManager.SelectedText;

            ProcForm CreateProcFrm = new ProcForm(word, false, true);

            CreateProcFrm.SetInsertAtArter = true;
            if (!IsSelectProcedure && currentHighlightProc == null) {
                CreateProcFrm.groupBoxProcedure.Enabled = false;
            }

            ProcTree.HideSelection = false;
            if (CreateProcFrm.ShowDialog() == DialogResult.Cancel) {
                ProcTree.HideSelection = true;
                return;
            }

            string name = CreateProcFrm.CheckName;
            if (name == null) return;
            if (currentTab.parseInfo.CheckExistsName(name, NameType.Proc)) {
                MessageBox.Show("A procedure with this name has already been declared.", "Info");
                return;
            }

            InsertAt placeAt = CreateProcFrm.PlaceAt;

            ProcedureBlock block = new ProcedureBlock();
            if (CreateProcFrm.CopyProcedure || placeAt == InsertAt.After) {
                Procedure proc = currentHighlightProc;
                if (ProcTree.SelectedNode != null) {
                    proc = ProcTree.SelectedNode.Tag  as Procedure;
                    if (proc == null) proc = currentHighlightProc;
                }
                if (proc != null) {
                    block.begin  = proc.d.start;
                    block.end    = proc.d.end;
                    block.declar = proc.d.declared;
                    block.copy   = CreateProcFrm.CopyProcedure;
                } else {
                    placeAt = InsertAt.Caret;
                }
            }

            name = CreateProcFrm.ProcedureName;
            PrepareInsertProcedure(name, block, placeAt);

            CreateProcFrm.Dispose();
            ProcTree.HideSelection = true;
        }

        // Create procedure block
        private void PrepareInsertProcedure(string name, ProcedureBlock block, InsertAt placeAt = InsertAt.Caret, byte overrides = 0)
        {
            int declrLine, procLine = 0, caretline = 3;
            string procbody;

            //Copy from procedure
            if (block.copy) {
                procbody = Utilities.GetRegionText(currentDocument, block.begin, block.end - 2) + Environment.NewLine;
                overrides = 1;
            } else
                procbody = new string(' ', Settings.tabSize) + ("script_overrides;\r\n\r\n");

            string procblock = (overrides > 0)
                       ? "\r\nprocedure " + name + " begin\r\n" + procbody + "end"
                       : "\r\nprocedure " + name + " begin\r\n\r\nend";

            int total = currentDocument.TotalNumberOfLines - 1;
            Procedure pTop = currentTab.parseInfo.GetTopProcedure();
            int declrEndLine = ParserInternal.GetRegionDeclaration(currentTab.textEditor.Document.TextContent, (pTop != null) ? pTop.d.start - 1 : total).end;
            int caretLine = currentActiveTextAreaCtrl.Caret.Line;

            if (total == 0 || caretLine <= declrEndLine) placeAt = InsertAt.End;

            // declaration line
            if (placeAt == InsertAt.Caret) {
                procLine = caretLine;
                Procedure p = currentTab.parseInfo.GetNearProcedure(procLine); // найти процедуру которая расположена рядом
                if (p != null) {
                    declrLine = p.d.Declaration;
                    if (procLine < p.d.start) {
                        declrLine--; // размесить над
                    }
                    if ((procLine + 2) == p.d.start) {
                        procblock += Environment.NewLine;
                    }
                } else {
                    placeAt = InsertAt.End;
                    ParserInternal.UpdateParseBuffer(currentTab.textEditor.Text);
                    declrLine = ParserInternal.GetEndLineProcDeclaration();
                }
            }
            else if (placeAt == InsertAt.After)
                declrLine = block.declar;
            else {
                ParserInternal.UpdateParseBuffer(currentTab.textEditor.Text);
                declrLine = ParserInternal.GetEndLineProcDeclaration();
            }
            // procedure line
            if (placeAt == InsertAt.After) {
                procLine = block.end; // after current procedure
                if (procLine > total)
                    procLine = block.end = total;
                else if (block.end < total)
                    block.end++;

                if (block.end == total || TextUtilities.GetLineAsString(currentDocument, block.end).Trim().Length > 0)
                    procblock += Environment.NewLine;
            }
            else if (placeAt == InsertAt.End) {
                procLine = total; // paste to end script
            }
            if (declrLine <= -1) declrLine = declrEndLine  + 1;

            Utilities.InsertProcedure(currentActiveTextAreaCtrl, name, procblock, declrLine, procLine, ref caretline);

            caretline += procLine + overrides;
            currentActiveTextAreaCtrl.Caret.Column = 0;
            currentActiveTextAreaCtrl.Caret.Line = caretline;
            currentActiveTextAreaCtrl.CenterViewOn(caretline, 0);

            currentHighlightProc = null;
            HighlightProcedures.AddToList(currentDocument, name);
            ForceParseScript();
            SetFocusDocument();
        }

        // Rename Procedures
        private void renameProcedureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Procedure proc = ProcTree.SelectedNode.Tag as Procedure;
            if (proc == null) return;

            ProcTree.HideSelection = false;
            string newName = Refactor.RenameProcedure(proc, currentDocument, currentTab, tabs);
            ProcTree.HideSelection = true;

            if (newName != null) {
                ProcTree.SelectedNode.Text = newName; // обновить имя в обозревателе

                // выполнить обновление
                ProcTree.Tag = TreeStatus.freeze; // предотвращает следующее обновление обозревателя
                ForceParseScript();
                SetFocusDocument();
            }
        }

        // Delete Procedures
        private void deleteProcedureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Procedure proc = ProcTree.SelectedNode.Tag as Procedure;
            if (proc == null) return;

            //if (proc.IsImported) {
            //    MessageBox.Show("You can't delete the imported procedure.");
            //    return;
            //}

            if (MessageBox.Show("Are you sure you want to delete \"" + proc.name + "\" procedure?",
                                "Warning", MessageBoxButtons.YesNo) == DialogResult.No) {
                return;
            }
            Utilities.PrepareDeleteProcedure(proc, currentDocument);
            ProcTree.Nodes.Remove(ProcTree.SelectedNode);

            currentActiveTextAreaCtrl.SelectionManager.ClearSelection();

            HighlightProcedures.DeleteFromList(currentDocument, proc.name);

            ProcTree.Tag = TreeStatus.freeze; // предотвращает следующее обновление обозревателя
            ForceParseScript();
            UpdateNodesTags();
            SetFocusDocument();
            HighlightCurrentPocedure(currentActiveTextAreaCtrl.Caret.Line);
        }

        private void MoveProcedure(int sIndex)
        {
            bool moveToEnd = false;
            int root = ProcTree.Nodes.Count - 1;

            if (sIndex > moveActive) {
                if (sIndex >= (ProcTree.Nodes[root].Nodes.Count - 1))
                    moveToEnd = true;
                else
                    sIndex++;
            } else if (sIndex == moveActive)
                return; //exit move

            Procedure moveProc = (Procedure)ProcTree.Nodes[root].Nodes[moveActive].Tag;
            // copy body
            ParserInternal.UpdateParseBuffer(currentDocument.TextContent);
            ProcedureBlock block = ParserInternal.GetProcedureBlock(moveProc.name, 0, true);
            block.declar = moveProc.d.declared;

            string copy_defproc;
            string copy_procbody = Environment.NewLine + Utilities.GetRegionText(currentDocument, block.begin, block.end);

            currentDocument.UndoStack.StartUndoGroup();
            currentActiveTextAreaCtrl.SelectionManager.ClearSelection();

            Utilities.DeleteProcedure(currentDocument, block, out copy_defproc);

            string name = ((Procedure)ProcTree.Nodes[root].Nodes[sIndex].Tag).Name;

            ParserInternal.UpdateParseBuffer(currentDocument.TextContent);
            // insert declration
            int offset = 0;
            if (copy_defproc != null) {
                int p_def = ParserInternal.GetDeclarationProcedureLine(name);
                if (moveToEnd) p_def++;
                if (p_def != -1) offset = currentDocument.PositionToOffset(new TextLocation(0, p_def));
                currentDocument.Insert(offset, copy_defproc + Environment.NewLine);
            }
            //paste proc block
            block = ParserInternal.GetProcedureBlock(name, 0, true);
            int p_begin;
            if (moveToEnd) {
                p_begin = block.end + 1;
                copy_procbody = Environment.NewLine + copy_procbody;
            } else {
                p_begin = block.begin;
                copy_procbody += Environment.NewLine;
            }
            offset = currentDocument.PositionToOffset(new TextLocation(0, p_begin));
            offset += TextUtilities.GetLineAsString(currentDocument, p_begin).Length;

            currentDocument.Insert(offset, copy_procbody);
            currentDocument.UndoStack.EndUndoGroup();

            // Перемещение процедуры в дереве
            if (sIndex > moveActive && !moveToEnd)
                sIndex--;

            TreeNode nd = ProcTree.Nodes[root].Nodes[moveActive];
            ProcTree.Nodes[root].Nodes.RemoveAt(moveActive);
            ProcTree.Nodes[root].Nodes.Insert(sIndex, nd);
            ProcTree.SelectedNode = ProcTree.Nodes[root].Nodes[sIndex];
            ProcTree.Focus();
            ProcTree.Select();

            currentHighlightProc = null;
            ParserInternal.UpdateProcInfo(ref currentTab.parseInfo, currentDocument.TextContent, currentTab.filepath);
            CodeFolder.UpdateFolding(currentDocument, currentTab.filename, currentTab.parseInfo.procs);
        }

        private void moveProcedureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProcTree.SelectedNode == null)
                return;
            if (moveActive == -1) {
                moveActive = ProcTree.SelectedNode.Index;
                ProcTree.SelectedNode.ForeColor = Color.Red;
                ProcTree.AfterSelect -= TreeView_AfterSelect;
                ProcTree.SelectedNode = ProcTree.Nodes[0];
                ProcTree.AfterSelect += ProcTree_AfterSelect;
                //ProcTree.ShowNodeToolTips = false;
            }
        }

        private void ProcTree_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveActive < 0)
                return;

            TreeNode node = ProcTree.GetNodeAt(e.Location);
            if (node != null && Functions.NodeHitCheck(e.Location, node.Bounds)) {
                if (node.Index > moveActive)
                    ProcTree.Cursor = Cursors.PanSouth;
                else
                    ProcTree.Cursor = Cursors.PanNorth;
            } else
                ProcTree.Cursor = Cursors.No;
        }

        private void ProcTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent == null || e.Node.Parent.Text != TREEPROCEDURES[1])
                return;
            ProcTree.AfterSelect -= ProcTree_AfterSelect;
            currentTab.textEditor.TextChanged -= textChanged;
            MoveProcedure(e.Node.Index);
            currentTab.textEditor.TextChanged += textChanged;
            ProcTree.AfterSelect += TreeView_AfterSelect;
            ProcTree.SelectedNode.ForeColor = ProcTree.ForeColor;
            ProcTree.Cursor = Cursors.Hand;
            moveActive = -1;
            //ProcTree.ShowNodeToolTips = true;
            // set changed document
            textChanged(null, EventArgs.Empty);
        }

        private void ProcTree_MouseLeave(object sender, EventArgs e)
        {
            if (moveActive != -1) {
                ProcTree.AfterSelect -= ProcTree_AfterSelect;
                ProcTree.AfterSelect += TreeView_AfterSelect;
                ProcTree.Nodes[ProcTree.Nodes.Count - 1].Nodes[moveActive].ForeColor = ProcTree.ForeColor;
                ProcTree.Cursor = Cursors.Hand;
                moveActive = -1;
            }
        }

        private void ProcTree_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                ProcTree_MouseLeave(null, null);
            }
        }

        private void ProcMnContext_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ProcTree.SelectedNode != null && ProcTree.SelectedNode.Tag is Procedure &&
                ProcTree.SelectedNode.Parent != null && (int)ProcTree.SelectedNode.Parent.Tag == 1)
            {
                Procedure proc = ProcTree.SelectedNode.Tag as Procedure;
                string pName = proc.Name;
                if (pName.IndexOf("node") > -1 || pName == "talk_p_proc")
                    editNodeCodeToolStripMenuItem.Enabled = true;
                else
                    editNodeCodeToolStripMenuItem.Enabled = false;
                renameProcedureToolStripMenuItem.Enabled = true;
                moveProcedureToolStripMenuItem.Enabled = true;
                deleteProcedureToolStripMenuItem.Enabled = true;
                deleteProcedureToolStripMenuItem.Text = "Delete: " + proc.name;
            } else {
                editNodeCodeToolStripMenuItem.Enabled = false;
                renameProcedureToolStripMenuItem.Enabled = false;
                moveProcedureToolStripMenuItem.Enabled = false;
                deleteProcedureToolStripMenuItem.Enabled = false;
                deleteProcedureToolStripMenuItem.Text = "Delete procedure";
            }
        }
        #endregion
    }
}
