using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.ClassDiagram;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;

using ScriptEditor.TextEditorUI.CompleteList;
using ScriptEditor.TextEditorUI.ToolTips;

using ScriptEditor.TextEditorUtilities;

namespace ScriptEditor.TextEditorUI.Nodes
{
    public partial class FlowchartTE
    {
        public event EventHandler<CodeArgs> ApplyCode;
        public class CodeArgs : EventArgs
        {
            public string Name { get; private set; }
            public string Code { get; private set; }
            public bool Change { get; set; }
            public bool Close  { get; set; }

            public CodeArgs (string name, string code, bool change)
            {
                Name = name;
                Code = code;
                Change = change;
            }
        }

        private readonly string customFile = Settings.SettingsFolder + @"\CustomCode.ini";

        private bool changeCode;
        private bool forceClose;

        private IWin32Window owner;
        private Form cForm;
        private TextEditorControl textEditor;

        private NodeCanvasItem nodeEditLink;

        public bool OpenFromDiagram { get; private set; }

        private TabInfo sourceTab;
        private ProgramInfo PI
        {
            get { return sourceTab.parseInfo; }
        }

        private string nodeName;
        public string NodeName
        {
            get { return nodeName; }
            private set {
                nodeName = value;
                cForm.Text = "Сode: " + value;
            }
        }

        List<DialogueParser> nodeParseData = new List<DialogueParser>();

        public FlowchartTE(Procedure cProc, TabInfo ti, List<string> allNodes = null, bool fromDiagram = false)
        {
            InitializeComponent();

            this.sourceTab = ti;

            OpenFromDiagram = fromDiagram;

            //Create the text editor
            TextEditorControl te = new TextEditorControl();

            //te.TextEditorProperties.NativeDrawText = Settings.winAPITextRender;
            te.TextEditorProperties.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            Settings.SetTextAreaFont(te);

            //te.AllowCaretBeyondEOL = true;
            te.EnableFolding = false;
            te.BorderStyle = BorderStyle.FixedSingle;
            te.LineViewerStyle = LineViewerStyle.FullRow;
            te.ShowVRuler = false;
            te.TextEditorProperties.IndentStyle = IndentStyle.Smart;
            te.TextEditorProperties.ConvertTabsToSpaces = Settings.tabsToSpaces;
            te.TextEditorProperties.TabIndent = Settings.tabSize;
            te.TextEditorProperties.IndentationSize = Settings.tabSize;

            // Activate the highlighting, use the name from the SyntaxDefinition node.
            te.TextEditorProperties.DarkScheme = ColorTheme.IsDarkTheme;
            te.SetHighlighting(ColorTheme.HighlightingScheme);
            te.OptionsChanged();

            te.Text = GetProcedureCode(ti.textEditor.Document, cProc);
            textEditor = te;

            // events
            te.TextChanged += CodeChangedTimer;
            te.ActiveTextAreaControl.TextArea.ToolTipRequest += TextArea_ToolTipRequest;

            te.ActiveTextAreaControl.TextArea.PreviewKeyDown += delegate(object sender, PreviewKeyDownEventArgs e) {
                if (e.KeyCode == Keys.Tab) {
                    if (Utilities.AutoCompleteKeyWord(textEditor.ActiveTextAreaControl))
                        e.IsInputKey = true;
                }
            };

            te.ActiveTextAreaControl.Caret.PositionChanged += delegate(object sender, EventArgs e) {
                Utilities.SelectedTextColorRegion(new TextLocation(), textEditor.ActiveTextAreaControl);
            };

            //te.ActiveTextAreaControl.TextArea.MouseDown += delegate(object sender, MouseEventArgs e) {
            //    if (e.Button == MouseButtons.Left)
            //        Utilities.SelectedTextColorRegion(textEditor.ActiveTextAreaControl);
            //};

            splitContainer.Panel1.Controls.Add(te);

            cForm = new Form();
            cForm.ClientSize = this.Size;
            cForm.Controls.Add(this);
            cForm.FormBorderStyle = FormBorderStyle.Sizable;
            cForm.StartPosition = FormStartPosition.CenterScreen;
            cForm.Icon = Properties.Resources.CodeText;

            NodeName = cProc.name;

            cForm.FormClosing += cForm_FormClosing;

            te.Dock = DockStyle.Fill;
            this.Dock = DockStyle.Fill;

            timer.Tick += CodeChanged;

            if (allNodes == null)
                allNodes = DialogueParser.GetAllNodesName(PI.procs);

            cmbNodesName.Items.AddRange(allNodes.ToArray());
            cmbNodesName.SelectedIndex = 0;

            LoadCustomCode();

            ParseCode();

            dgvMessages.ClearSelection();
        }

        private string GetProcedureCode(IDocument document, Procedure p)
        {
            string nodeProcedureText = Utilities.GetProcedureCode(document, p);
            return nodeProcedureText;
        }

        private void cForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (changeCode) {
                if (!cForm.Focused)
                    Activate();

                e.Cancel = SaveClosing(EventArgs.Empty);
            }
            if (!e.Cancel)
                StopEditingNode();
        }

        private void StopEditingNode()
        {
            if (nodeEditLink != null)
                nodeEditLink.StopEditingNode(); //node edit off status
        }

        private bool SaveClosing(EventArgs e)
        {
            MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel;
            if (forceClose)
                buttons = MessageBoxButtons.YesNo;

            DialogResult result = MessageBox.Show("Do you want to apply the changes to the code for this node?", "Apply change", buttons);
            switch (result)
            {
                case DialogResult.Yes:
                    ApplytoolStripButton_Click(null, e);
                    break;
                case DialogResult.Cancel:
                    return true;
            }
            return false;
        }

        private void CodeChangedTimer(object sender, EventArgs e)
        {
            changeCode = true;

            if (((DocumentEventArgs)e).Text == "\r\n")
                return;

            timer.Stop();
            timer.Start();
        }

        private void CodeChanged(object sender, EventArgs e)
        {
            timer.Stop();

            if (dgvMessages.CurrentRow == null) {
                ParseCode();
                if (dgvMessages.Rows.Count != 0)
                    dgvMessages.Rows[0].Selected = false;
                return;
            }

            int selRow = dgvMessages.CurrentRow.Index;
            bool isSelected = dgvMessages.Rows[selRow].Selected;
            DataGridViewRow cRow = dgvMessages.CurrentRow;

            DataGridViewRow[] rows = new DataGridViewRow[dgvMessages.Rows.Count];
            dgvMessages.Rows.CopyTo(rows, 0);

             try {
                ParseCode();
            } catch {
                dgvMessages.Rows.AddRange(rows);
                dgvMessages.CurrentCell = dgvMessages.Rows[selRow].Cells[0];
                return;
            }

            if (dgvMessages.Rows.Count == 0)
                return;

            if (!isSelected)
                dgvMessages.Rows[selRow].Selected = isSelected;
            else {
                foreach (DataGridViewRow row in dgvMessages.Rows)
                    if (row.Cells[0].Value.Equals(cRow.Cells[0].Value)) {
                        dgvMessages.CurrentCell = dgvMessages.Rows[row.Index].Cells[0];
                        selRow = -1; // flag
                        break;
                    }
                if (selRow != -1 && dgvMessages.Rows.Count > 0)
                    dgvMessages.Rows[0].Selected = false;
            }
        }

        private void ParseCode()
        {
            nodeParseData.Clear();
            dgvMessages.Rows.Clear();

            //распарсить код
            DialogueParser.ParseNodeCode(textEditor.Text, nodeParseData, PI, splitOption: StringSplitOptions.None);

            foreach (DialogueParser data in nodeParseData)
            {
                bool error = false;
                string msg = null, path = null;

                if (data.numberMsgLine > 0) {
                    if (data.numberMsgFile > 0) {
                        if (MessageFile.GetPath(sourceTab, data.numberMsgFile, out path)) {
                            string[] MsgData = File.ReadAllLines(path, Settings.EncCodePage);
                            msg = MessageFile.GetMessages(MsgData, data.numberMsgLine);
                        } else {
                            msg = String.Format(MessageFile.msgfileError, data.numberMsgFile); //<Not found message file>
                            error = true;
                        }
                    } else if (sourceTab.messages.ContainsKey(data.numberMsgLine))
                        msg = sourceTab.messages[data.numberMsgLine];
                    if (msg == null) {
                        msg = MessageFile.messageError;
                        error = true;
                    }
                } else
                    continue;

                string msgFile = (data.numberMsgFile == -1) ? sourceTab.msgFilePath : path; //MessageFile.GetMessageFilePath(data.numberMsgFile, curTab);

                dgvMessages.Rows.Add(data.numberMsgLine, msg, Path.GetFileName(msgFile));
                dgvMessages.Rows[dgvMessages.Rows.Count - 1].Cells[0].Tag = data;
                dgvMessages.Rows[dgvMessages.Rows.Count - 1].Cells[2].Tag = msgFile;

                if (error) {
                    dgvMessages.Rows[dgvMessages.Rows.Count - 1].Cells[1].ReadOnly = true;
                    dgvMessages.Rows[dgvMessages.Rows.Count - 1].Cells[1].Style.ForeColor = Color.Red;
                } else if (data.opcode == OpcodeType.Option || data.opcode == OpcodeType.giq_option || data.opcode == OpcodeType.gsay_option)
                    dgvMessages.Rows[dgvMessages.Rows.Count - 1].Cells[1].Style.ForeColor = Color.Blue;
            }
        }

        // Tooltip for opcodes and macros
        private void TextArea_ToolTipRequest(object sender, ToolTipRequestEventArgs e)
        {
            if (!e.InDocument)
                return;

            ToolTipRequest.Show(sourceTab, textEditor.Document, e);
        }

        public void ShowEditor(IWin32Window owner, NodeCanvasItem nodeEditLink = null)
        {
            this.owner = owner;
            this.nodeEditLink = nodeEditLink;

            cForm.Show();
        }

        public void CloseEditor(bool forceClose = false)
        {
            this.forceClose = forceClose;
            cForm.Close();
        }

        public void Activate()
        {
            if (cForm.WindowState == FormWindowState.Minimized)
                cForm.WindowState = FormWindowState.Normal;
            cForm.Activate();

        }
        private void onToptoolStripButton_Click(object sender, EventArgs e)
        {
            if (onToptoolStripButton.Checked) {
                onToptoolStripButton.Image = Properties.Resources.KeepWindowOn;
                cForm.Owner = (Form)owner;
            } else {
                onToptoolStripButton.Image = Properties.Resources.KeepWindowOff;
                cForm.Owner = null;
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null, null);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(null, null);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(null, null);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(null, null);
        }

        private void UndotoolStripButton_Click(object sender, EventArgs e)
        {
            textEditor.Undo();
            if (!textEditor.Document.UndoStack.CanUndo)
                changeCode = false;
        }

        private void RedotoolStripButton_Click(object sender, EventArgs e)
        {
            textEditor.Redo();
        }

        private void ApplytoolStripButton_Click(object sender, EventArgs e)
        {
            var codeArgs = new CodeArgs(NodeName, textEditor.Document.TextContent, changeCode);

            ApplyCode(this, codeArgs);

            changeCode = codeArgs.Change;
            if (codeArgs.Close && e != null)
                cForm.Close();
        }

        private void dgvMessages_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            DialogueParser data = (DialogueParser)dgvMessages.Rows[e.RowIndex].Cells[0].Tag;

            List<TextMarker> marker = textEditor.Document.MarkerStrategy.GetMarkers(0, textEditor.Document.TextLength);
            if (marker.Count > 0)
                textEditor.Document.MarkerStrategy.RemoveMarker(marker[0]);

            LineSegment ls = textEditor.Document.GetLineSegment(data.codeNumLine);
            string codeline = TextUtilities.GetLineAsString(textEditor.Document, data.codeNumLine);
            int offset = codeline.IndexOf(data.shortcode);
            int len = data.shortcode.Length;

            if (offset > 0) {
                while (Char.IsLetter(textEditor.Document.GetCharAt(ls.Offset + (offset - 1))))
                {
                    offset--;
                    len++;
                }
            }
            TextMarker tm = new TextMarker(ls.Offset + offset, len, TextMarkerType.Underlined, Color.DeepPink);
            textEditor.Document.MarkerStrategy.AddMarker(tm);
            textEditor.Refresh();
        }

        private void dgvMessages_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0) {
                string path = (string)dgvMessages.Rows[e.RowIndex].Cells[2].Tag;
                if (File.Exists(path))
                    OpenMessageFile(path, (int)dgvMessages.Rows[e.RowIndex].Cells[0].Value, sourceTab);
                else
                    MessageBox.Show("The requested message file: " + path + "\ncould not be found.", "Missing messages file");
            }
        }

        private void msgEdit_SendMsgLine(string msgLine)
        {
            int iRow = dgvMessages.CurrentRow.Index;

            DialogueParser data = (DialogueParser)dgvMessages.Rows[iRow].Cells[0].Tag;

            LineSegment ls = textEditor.Document.GetLineSegment(data.codeNumLine);
            string codeline = TextUtilities.GetLineAsString(textEditor.Document, data.codeNumLine);
            int offset = codeline.IndexOf(data.shortcode);
            string msgNum = Convert.ToString(data.numberMsgLine);
            offset = codeline.IndexOf(msgNum, offset);
            textEditor.Document.Replace(ls.Offset + offset, msgNum.Length, msgLine);

            // update
            ParseCode();

            dgvMessages.Rows[iRow].Cells[0].Selected = true;
            cForm.TopMost = onToptoolStripButton.Checked;
        }

        private void dgvMessages_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            DataGridViewCell cell = dgvMessages.Rows[e.RowIndex].Cells[1];
            DialogueParser data = (DialogueParser)dgvMessages.Rows[e.RowIndex].Cells[0].Tag;

            string text = (string)cell.Value;
            string msgfilePath = (string)dgvMessages.Rows[e.RowIndex].Cells[2].Tag;
            if (MessageFile.SaveToMessageFile(msgfilePath, text, data.numberMsgLine) && data.numberMsgFile == -1)
                sourceTab.messages[data.numberMsgLine] = text;
        }

        private void pasteOpcodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            object tag = ((ToolStripMenuItem)sender).Tag;
            if (tag == null)
                return;

            Utilities.InsertText((string)tag, textEditor.ActiveTextAreaControl);
        }

        private void insertMsgLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenMessageFile(sourceTab.msgFilePath, 0, sourceTab, true);
        }

        // Open msg file
        private void OpenMessageFile(string msgFile, int msgNum, TabInfo tab, bool context = false)
        {
            MessageEditor msgEdit = MessageEditor.MessageEditorInit(msgFile, msgNum, tab, true);
            if (context) {
                msgEdit.SendMsgLine += delegate(string msgLine) {Utilities.InsertText(msgLine, textEditor.ActiveTextAreaControl); };
            } else
                msgEdit.SendMsgLine += msgEdit_SendMsgLine;

            msgEdit.closeOnSend = true;
            cForm.TopMost = false;
            msgEdit.ShowDialog(); // modal

            // update
            ParseCode();
        }

        private void LoadCustomCode()
        {
            List<string> customCode = new List<string>(File.ReadAllLines(customFile));
            foreach (string code in customCode)
            {
                try {
                    string[] mCode = code.Split('|');
                    AddCustomItem(mCode[0], mCode[1]);
                } catch {
                    continue;
                }
            }
        }

        private void toCustomCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = String.Empty;
            if (InputBox.ShowDialog("Enter name for custom code:", ref name, 20) == DialogResult.OK) {
                string code = textEditor.ActiveTextAreaControl.SelectionManager.SelectedText;
                List<string> customCode = new List<string>(File.ReadAllLines(customFile));
                customCode.Add(name + "|" + code);
                File.WriteAllLines(customFile, customCode);

                AddCustomItem(name, code);
            }
        }

        private void AddCustomItem(string name, string code)
        {
            ToolStripMenuItem mi = new ToolStripMenuItem(name);
            mi.Tag = code;
            mi.Click += pasteOpcodeToolStripMenuItem_Click;
            customToolStripMenuItem.DropDownItems.Add(mi);
        }

        private void contextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            toCustomCodeToolStripMenuItem.Enabled = textEditor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected;
        }

        private void cmbNodesName_DropDownClosed(object sender, EventArgs e)
        {
            textEditor.ActiveTextAreaControl.TextArea.Focus();
            if (cmbNodesName.SelectedIndex > 0
                && textEditor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected)
                Utilities.InsertText(cmbNodesName.Text, textEditor.ActiveTextAreaControl);
        }

        private void tsbPasteNode_Click(object sender, EventArgs e)
        {
            if (cmbNodesName.SelectedIndex > 0)
                Utilities.InsertText(cmbNodesName.Text, textEditor.ActiveTextAreaControl);
        }

        private void tsbGotoNode_Click(object sender, EventArgs e)
        {
            if (cmbNodesName.SelectedIndex > 0) {
                bool _goto = true;
                if (changeCode)
                    _goto = !SaveClosing(null);

                if (_goto) {
                    StopEditingNode();
                    nodeEditLink = null;

                    NodeName = cmbNodesName.Text;

                    int index = PI.GetProcedureIndex(NodeName);
                    textEditor.Text = GetProcedureCode(sourceTab.textEditor.Document, PI.procs[index]);
                    timer.Stop();
                    changeCode = false;

                    ParseCode();

                    textEditor.Refresh();
                }
            }
        }
    }
}
