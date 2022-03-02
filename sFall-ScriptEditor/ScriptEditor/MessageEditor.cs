using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ScriptEditor.TextEditorUI;
using ScriptEditor.TextEditorUtilities;

namespace ScriptEditor
{
    public delegate void SendLineHandler(string msgLine);

    partial class MessageEditor : Form
    {
        private const int WM_SETREDRAW = 0x000B;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        public event SendLineHandler SendMsgLine;

        private List<string> linesMsg;
        private string msgPath;
        private bool isCancelEdit;
        private bool allow;

        private Color editColor;
        private bool editAllowed = true;
        private bool isEditMode = false;
        private bool shiftKeyDown = false;

        private const string COMMENT = "#";
        private const string pcMarker = "\u25CF ";

        private Color pcColor = Color.FromArgb(0, 0, 220);
        private Color cmColor;

        private Encoding enc = Settings.EncCodePage;

        private TabInfo associateTab;
        private Form scriptForm;

        internal bool closeOnSend = false;

        private cell SelectLine = new cell();

        private struct cell
        {
            public int row;
            public int col;
        }

        public enum CommentColor { LightYellow, PaleGreen, Lavender }

        //List<Entry> rowsEntry = new List<Entry>();
        //List<List<Entry>> rowsUndo = new List<Entry>();

        private void RowsClear()
        {
            dgvMessage.Rows.Clear();
            //rowsEntry.Clear();
        }

        #region Entry DGV
        private class Entry
        {
            static private bool wrapLine = false;

            public string msgLine = string.Empty;
            public string msglip = string.Empty;
            public string msgText = string.Empty;
            public string description = string.Empty;

            public bool pcMark = false;
            public bool commentLine = false;

            public int tabCount = 1;

            static public bool IsWrap
            {
                get { return wrapLine; }
            }

            public Entry() { }

            public Entry(string line)
            {
                //wrapLine = false;
                if (line.Length == 0) return;
                if (line.TrimStart().StartsWith(COMMENT)) {
                    msgLine = "-";
                    msgText = line;
                    commentLine = true;
                } else {
                    string[] splitLine = line.Split(new char[] {'}'}, 4);
                    if (splitLine.Length == 0) return; // msg bad

                    wrapLine = splitLine.Length == 3;

                    if (splitLine[0].StartsWith("\t")) {
                        tabCount = splitLine[0].Length;
                        splitLine[0] = splitLine[0].TrimStart('\t');
                        tabCount -= splitLine[0].Length;
                        pcMark = true;
                    }
                    int z = splitLine[0].IndexOf('{');
                    msgLine = splitLine[0].Substring(z + 1); // номер строки

                    z = splitLine[1].IndexOf('{');
                    msglip = splitLine[1].Substring(z + 1);

                    z = splitLine[2].IndexOf('{');
                    msgText = splitLine[2].Substring(z + 1);
                    if (splitLine.Length > 3 && splitLine[3].Length > 0)
                        description = splitLine[3].TrimEnd();
                }
            }

            public void Append(string line)
            {
                int end = line.IndexOf('}');
                wrapLine = (end == -1);
                if (wrapLine) {
                    msgText += Environment.NewLine + line;
                } else {
                    msgText += Environment.NewLine + line.Remove(end);
                    description = line.Substring(end + 1).TrimEnd();
                }
            }

            public string ToString(out bool prev)
            {
                prev = false;
                int result;
                if (int.TryParse(msgLine, out result)) {
                    string tab = (pcMark) ? new String('\t', tabCount) : String.Empty;
                    return (tab + "{" + msgLine + "}{" + msglip + "}{" + msgText + "}" + description);
                }
                return (msgText + description);
            }
        }
        #endregion

        private void AddRow(Entry e)
        {
            //if (dgvMessage.VirtualMode) {
            //    rowsEntry.Add(e);
            //    return;
            //}

            string message = (e.pcMark) ? pcMarker + e.msgText : e.msgText;
            dgvMessage.Rows.Add(e.msgLine, message, e.msglip);
            int row = dgvMessage.Rows.Count - 1;
            dgvMessage.Rows[row].Cells[0].Tag = e;
            dgvMessage.Rows[row].Cells[1].ToolTipText = e.description.Trim();

            if (e.commentLine)
                MarkCommentLine(row);
            else if (e.pcMark)
                dgvMessage.Rows[row].Cells[1].Style.ForeColor = pcColor;
        }

        private void InsertRow(int i, Entry e)
        {
            if (i >= dgvMessage.Rows.Count) {
                SelectLine.row = dgvMessage.Rows.Count;
                AddRow(e);
            } else {
                string message = (e.pcMark) ? pcMarker + e.msgText : e.msgText;
                dgvMessage.Rows.Insert(i, e.msgLine, message, e.msglip);
                dgvMessage.Rows[i].Cells[0].Tag = e;
            }
            if (e.commentLine)
                MarkCommentLine(i);
        }

        private void MarkCommentLine(int row)
        {
            if (!Settings.msgHighlightComment)
                return;

            foreach (DataGridViewCell cell in dgvMessage.Rows[row].Cells)
                cell.Style.BackColor = cmColor;
        }

        private void HighlightingCommentUpdate()
        {
            Color clr = (Settings.msgHighlightComment) ? cmColor : dgvMessage.RowsDefaultCellStyle.BackColor;
            for (int row = 0; row < dgvMessage.Rows.Count; row++)
            {
                Entry ent = (Entry)dgvMessage.Rows[row].Cells[0].Tag;
                if (ent.commentLine) {
                    for (int col = 0; col < dgvMessage.Rows[row].Cells.Count; col++)
                        dgvMessage.Rows[row].Cells[col].Style.BackColor = clr;
                }
            }
        }

        private void SetCommentColor()
        {
            Color lineColumnColor;
            switch ((CommentColor)Settings.msgHighlightColor)
            {
                case CommentColor.LightYellow:
                default:
                    cmColor = Color.FromArgb(255, 255, 200);
                    lineColumnColor = Color.LightYellow;
                break;
                case CommentColor.Lavender:
                    cmColor = Color.Lavender;
                    lineColumnColor = Color.FromArgb(240, 240, 250);
                    break;
                case CommentColor.PaleGreen:
                    cmColor = Color.FromArgb(195, 255, 195);
                    lineColumnColor = Color.FromArgb(215, 255, 215);
                    break;
            }
            dgvMessage.Columns[0].DefaultCellStyle.BackColor = lineColumnColor;
        }

        #region Initial form
        // call from testing dialog tools and node code editor
        public static MessageEditor MessageEditorInit(string msgPath, int line, TabInfo tab = null, bool sendState = false)
        {
            MessageEditor msgEdit = new MessageEditor(msgPath, tab);

            for (int i = 0; i < msgEdit.dgvMessage.RowCount; i++)
            {
                int number;
                if (int.TryParse(msgEdit.dgvMessage.Rows[i].Cells[0].Value.ToString(), out number))
                    if (number == line) {
                        msgEdit.dgvMessage.Rows[i].Cells[1].Selected = true;
                        msgEdit.dgvMessage.FirstDisplayedScrollingRowIndex = (i <= 5) ? i : i - 5;
                        break;
                    }
            }
            msgEdit.SendStripButton.Enabled = sendState;

            return msgEdit;
        }

        // call from main script editor
        public static MessageEditor MessageEditorInit(TabInfo tab, Form frm)
        {
            string msgPath = null;
            if (tab != null) {
                if (!MessageFile.GetAssociatePath(tab, true, out msgPath))
                    return null;

                tab.msgFilePath = msgPath;
            }

            // Show form
            MessageEditor msgEdit = new MessageEditor(msgPath, tab);
            msgEdit.scriptForm = frm;
            if (tab != null)
                msgEdit.alwaysOnTopToolStripMenuItem.Checked = true;
            msgEdit.Show();
            //if (Settings.autoOpenMsgs && msgEdit.scrptEditor.msgAutoOpenEditorStripMenuItem.Checked)
            //    msgEdit.WindowState = FormWindowState.Minimized;

            return msgEdit;
        }

        // for open custom message file
        public static MessageEditor MessageEditorOpen(string msgPath, Form frm)
        {
            if (msgPath == null)
                MessageBox.Show("No output path selected.", "Error");

            // Show form
            MessageEditor msgEdit = new MessageEditor(msgPath, null);
            msgEdit.scriptForm = frm;
            msgEdit.WindowState = FormWindowState.Maximized;
            frm.TopMost = false;
            msgEdit.Show();

            return msgEdit;
        }
        #endregion

        #region Constructor
        public MessageEditor(string msgfile) : this (msgfile, null)
        {
            SendStripButton.Enabled = false;
        }

        private MessageEditor(string msg, TabInfo ti)
        {
            InitializeComponent();

            //dgvMessage.DoubleBuffered(true);
            //dgvMessage.VirtualMode = true;

            dgvMessage.Columns[2].Visible = Settings.msgLipColumn;
            showLIPColumnToolStripMenuItem.Checked = Settings.msgLipColumn;

            FontSizeComboBox.SelectedIndex = Settings.msgFontSize;
            if (Settings.msgFontSize != 0)
                FontSizeChanged(null, null);
            FontSizeComboBox.SelectedIndexChanged += FontSizeChanged;

            ColorComboBox.SelectedIndex = Settings.msgHighlightColor;
            HighlightingCommToolStripMenuItem.Checked = Settings.msgHighlightComment;

            if (Settings.encoding == (byte)EncodingType.OEM866)
                encodingTextDOSToolStripMenuItem.Checked = true;

            StripComboBox.SelectedIndex = 2;
            if (!Settings.msgLipColumn) {
                dgvMessage.Columns[2].Visible = false;
                showLIPColumnToolStripMenuItem.Checked = false;
            }

            UpdateRecentList();

            msgPath = msg;
            if (msgPath != null)
                ReadMsgFile();
            else {
                this.Text = "Empty" + this.Tag;
                AddRow(new Entry());
                linesMsg = new List<string>();
            }

            associateTab = ti;
            if (associateTab != null)
                MessageFile.ParseMessages(associateTab, linesMsg.ToArray());
        }
        #endregion

        #region Load/Save Msg File
        private void ReadMsgFile()
        {
            linesMsg = new List<string>(File.ReadAllLines(msgPath, enc));

            ProgressBarForm progress = null;
            if (linesMsg.Count > 250)
                progress = new ProgressBarForm(this, linesMsg.Count);

            dgvMessage.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            SendMessage(dgvMessage.Handle, WM_SETREDRAW, false, 0);

            for (int i = 0; i < linesMsg.Count; i++)
            {
                Entry entry;
                try {
                    entry = new Entry(linesMsg[i]);
                } catch (Exception) {
                    MessageBox.Show("Message file is bad!\nLine Error: " + i.ToString());
                    break;
                }
                while (Entry.IsWrap && ++i < linesMsg.Count) entry.Append(linesMsg[i]);

                AddRow(entry);
                if (progress != null) progress.SetProgress = i;
            }
            //if (dgvMessage.VirtualMode) dgvMessage.RowCount = rowsEntry.Count;

            SendMessage(dgvMessage.Handle, WM_SETREDRAW, true, 0);
            dgvMessage.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;

            if (progress != null)
                progress.Dispose();

            this.Text = Path.GetFileName(msgPath) + this.Tag;
            groupBox.Text = msgPath;
            msgSaveButton.Enabled = false;
        }

        private void SaveFileMsg()
        {
            bool prevLine;
            bool replaceX = (enc.CodePage == 866);
            dgvMessage.EndEdit();
            linesMsg.Clear();
            for (int i = 0; i < dgvMessage.Rows.Count; i++)
            {
                Entry entries = (Entry)dgvMessage.Rows[i].Cells[0].Tag;
                string line = entries.ToString(out prevLine);

                if (replaceX)
                    line = line.Replace('\u0425', '\u0058'); //Replacement of Russian letter "X", to English letter

                linesMsg.Add(line);

                if (prevLine)
                    linesMsg[i - 1] = linesMsg[i - 1].TrimEnd('}');

                foreach (DataGridViewCell cells in dgvMessage.Rows[i].Cells)
                {
                    switch (cells.ColumnIndex) {
                        case 0:
                            cells.Style.ForeColor = Color.Chocolate;
                            break;
                        case 2:
                            cells.Style.ForeColor = Color.Gray;
                            break;
                        default:
                            if (entries.pcMark)
                                cells.Style.ForeColor = pcColor;
                            else
                                cells.Style.ForeColor = dgvMessage.RowsDefaultCellStyle.ForeColor;
                            break;
                    }
                }
            }
            File.WriteAllLines(msgPath, linesMsg.ToArray(), enc);
            msgSaveButton.Enabled = false;

            if (associateTab != null)
                MessageFile.ParseMessages(associateTab, linesMsg.ToArray());
        }
        #endregion

        private void dgvMessage_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1 || isCancelEdit) return;

            DataGridViewCell cell = dgvMessage.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Entry entry = (Entry)dgvMessage.Rows[e.RowIndex].Cells[0].Tag;

            string valCell = (string)cell.Value;
            if (valCell == null) valCell = string.Empty;

            switch (e.ColumnIndex) {
                case 0: // line
                    int result;
                    if (!int.TryParse(valCell, out result)) {
                        MessageBox.Show("Line must contain only numbers.", "Line Error");
                        isCancelEdit = true;
                        cell.Value = entry.msgLine;
                    } else
                        entry.msgLine = valCell;
                    break;
                case 1: // message
                    if (valCell.IndexOfAny(new char[] { '{', '}' }) != -1) {
                        isCancelEdit = true;
                        valCell = entry.msgText; // cancel
                    } else {
                        if (valCell.Length > 0 && valCell.TrimStart().StartsWith(pcMarker[0].ToString())) {
                            entry.msgText = valCell.Remove(0, 1).TrimStart(); // удалить лишний маркер
                        } else {
                            entry.msgText = valCell;
                        }
                    }
                    // добавить/удалить маркер для отображения в таблице
                    if (valCell.Length > 0) {
                        if (entry.pcMark) {
                            if (!valCell.TrimStart().StartsWith(pcMarker)) {
                                valCell = pcMarker + valCell; // добавить
                            }
                        } else if (valCell.TrimStart().StartsWith(pcMarker)) {
                                valCell = entry.msgText;      // удалить
                        }
                    }
                    cell.Value = valCell;
                    break;
                case 2: // lipfile
                    if (valCell.IndexOfAny(new char[] { '{', '}' }) != -1) {
                        isCancelEdit = true;
                        cell.Value = entry.msglip;
                    } else
                        entry.msglip = valCell;
                    break;
            }
            if (!isCancelEdit) {
                dgvMessage.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;
                msgSaveButton.Enabled = true;
            }
            isCancelEdit = false;
        }

        private void dgvMessage_SelectionChanged(object sender, EventArgs e)
        {
            if (allow) {
                SelectLine.row = (dgvMessage.CurrentRow == null) ? 0 : dgvMessage.CurrentRow.Index;
                SelectLine.col = (dgvMessage.CurrentRow == null) ? 0 : dgvMessage.CurrentCell.ColumnIndex;
            } else
                allow = true;

            editAllowed = false;
        }

        private void SendStripButton_Click(object sender, EventArgs e)
        {
            if (SendMsgLine == null) return;

            string line = (string)dgvMessage.Rows[SelectLine.row].Cells[0].Value;
            int result;
            if (int.TryParse(line, out result)) {
                SendMsgLine(line);
                if (closeOnSend)
                    this.Close();
            }
        }

        private void NewStripButton_Click(object sender, EventArgs e)
        {
            RowsClear();

            AddRow(new Entry("# Look Name"));
            AddRow(new Entry("{100}{}{}"));
            AddRow(new Entry("# Description"));
            AddRow(new Entry("{101}{}{}"));

            this.Text = "unsaved.msg" + this.Tag;
            this.groupBox.Text = "Messages";

            linesMsg = new List<string>();
            msgPath = null;
            msgSaveButton.Enabled = false;
        }

        private void msgOpenButton_ButtonClick(object sender, EventArgs e)
        {
            string path = msgPath;
            if (path == null && Settings.outputDir != null)
                path = Path.GetFullPath(Path.Combine(Settings.outputDir, MessageFile.MessageTextSubPath));

            openFileDialog.InitialDirectory = Path.GetDirectoryName(path);
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                msgPath = openFileDialog.FileName;
                RowsClear();
                ReadMsgFile();
                if (Path.IsPathRooted(msgPath)) {
                    Settings.AddMsgRecentFile(msgPath);
                    UpdateRecentList();
                }
            }
        }

        private void msgSaveButton_ButtonClick(object sender, EventArgs e)
        {
            if (msgPath == null)
                SaveAsStripButton_Click(null, null);
            else
                SaveFileMsg();
        }

        private void SaveAsStripButton_Click(object sender, EventArgs e)
        {
            string path = msgPath;

            if (path == null && Settings.outputDir != null)
                path = Path.GetFullPath(Path.Combine(Settings.outputDir, MessageFile.MessageTextSubPath));

            saveFileDialog.InitialDirectory = Path.GetDirectoryName(path);
            saveFileDialog.FileName = Path.GetFileName(msgPath);
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                msgPath = saveFileDialog.FileName;
                SaveFileMsg();
                this.Text = Path.GetFileName(msgPath) + this.Tag;
                groupBox.Text = msgPath;
                Settings.AddMsgRecentFile(msgPath);
                UpdateRecentList();
            }
        }

        private bool AddNewLine()
        {
            int Line = 0, nLine;
            bool _comm = false;
            bool isEdit = dgvMessage.IsCurrentCellInEditMode;

            for (int n = SelectLine.row; n >= 0; n--)
            {
                if (int.TryParse((string)dgvMessage.Rows[n].Cells[0].Value, out Line)) break;
                string val = (string)dgvMessage.Rows[n].Cells[1].Value;
                if (/*val != null &&*/ val.StartsWith("#")) _comm = true;
            }
            if (_comm) {
                Line = (int)Math.Round((decimal)Line / 10) * 10;
                Line += Convert.ToInt32(StripComboBox.Text);
            } else Line++;
            for (int n = 0; n < dgvMessage.Rows.Count; n++)
            {
                if (int.TryParse((string)dgvMessage.Rows[n].Cells[0].Value, out nLine)) {
                    if (Line == nLine) {
                        return false;
                    }
                }
            }

            dgvMessage.EndEdit();

            if ((string)dgvMessage.Rows[SelectLine.row].Cells[0].Value != string.Empty) SelectLine.row++;
            InsertRow(SelectLine.row, new Entry("{" + Line + "}{}{}"));
            msgSaveButton.Enabled = true;
            allow = false;
            try { dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].Selected = true; }
            catch { };

            if (isEdit)
                dgvMessage.BeginEdit(false);

            return true;
        }

        private void IncAddStripButton_Click(object sender, EventArgs e)
        {
            if (!AddNewLine()) System.Media.SystemSounds.Question.Play();
        }

        private void InsertEmptyStripButton_Click(object sender, EventArgs e)
        {
            if (sender != null && !shiftKeyDown)
                SelectLine.row++;

            InsertRow(SelectLine.row, new Entry(""));
            allow = false;

            if (sender == null || shiftKeyDown)
                SelectLine.row++;
            try { dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].Selected = true; }
            catch { };

            msgSaveButton.Enabled = true;
        }

        private void DeleteLineStripButton_Click(object sender, EventArgs e)
        {
            if (dgvMessage.Rows.Count <= 1) return;

            DataGridViewSelectedRowCollection selRows = dgvMessage.SelectedRows;
            if (selRows.Count > 0) {
                foreach (DataGridViewRow row in selRows)
                    dgvMessage.Rows.Remove(row);
                if (dgvMessage.RowCount == 0) AddRow(new Entry());
            } else {
                dgvMessage.Rows.RemoveAt(SelectLine.row);
                if (SelectLine.row >= dgvMessage.Rows.Count) SelectLine.row--;
            }
            msgSaveButton.Enabled = true;
        }

        private void InsertCommentStripButton_Click(object sender, EventArgs e)
        {
            if (dgvMessage.IsCurrentCellInEditMode || dgvMessage.MultiSelect)
                return;

            string comment = COMMENT;
            if ((string)dgvMessage.Rows[SelectLine.row].Cells[0].Value == String.Empty) {
                comment += dgvMessage.Rows[SelectLine.row].Cells[1].Value;
                dgvMessage.Rows.RemoveAt(SelectLine.row);
            } else
                SelectLine.row++;

            InsertRow(SelectLine.row, new Entry(comment));
            allow = false;
            try { dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].Selected = true; }
            catch { };
            msgSaveButton.Enabled = true;
        }

        #region Search function
        private void Finds(int rowStart, int colStart, int rev = 1)
        {
            string find_str = SearchStripTextBox.Text.Trim();
            if (find_str.Length == 0) {
                MessageBox.Show("Nothing found.");
                return;
            }

            cell findPosition = new cell();
            findPosition.col = -1;

            if (rev == -1 && rowStart == 0) rowStart = dgvMessage.RowCount - 1;

            for (int row = rowStart; row < dgvMessage.RowCount; row += rev)
            {
                if (row < 0) break;
                for (int col = colStart; col < dgvMessage.ColumnCount; col++)
                {
                    if (dgvMessage.Rows[row].Cells[col].Value == null)
                        continue;
                    string value = dgvMessage.Rows[row].Cells[col].Value.ToString();
                    if (value.IndexOf(find_str, 0, StringComparison.OrdinalIgnoreCase) != -1) {
                        findPosition.row = row;
                        findPosition.col = col;
                        break;
                    }
                }
                if (findPosition.col != 0) break;
                colStart = 0;
            }
            if (findPosition.col != 0) {
                dgvMessage.FirstDisplayedScrollingRowIndex = (findPosition.row <= 5) ? findPosition.row : findPosition.row - 5;
                dgvMessage.Rows[findPosition.row].Cells[findPosition.col].Selected = true;
            }

            if (findPosition.col == -1)
                System.Media.SystemSounds.Exclamation.Play();
            else {
                SelectLine = findPosition;
            }
        }

        private void Downbutton_Click(object sender, EventArgs e)
        {
            Finds(SelectLine.row, SelectLine.col + 1);
        }

        private void Upbutton_Click(object sender, EventArgs e)
        {
            Finds(SelectLine.row, SelectLine.col + 1, -1);
        }
        #endregion

        private void showLIPColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvMessage.Columns[2].Visible = showLIPColumnToolStripMenuItem.Checked;
            Settings.msgLipColumn = showLIPColumnToolStripMenuItem.Checked;
        }

        private void UpdateRecentList()
        {
            string[] items = Settings.GetMsgRecent();
            msgOpenButton.DropDownItems.Clear();
            for (int i = items.Length - 1; i >= 0; i--) {
                msgOpenButton.DropDownItems.Add(items[i], null, MsgRecentClick);
            }
        }

        private void MsgRecentClick(object sender, EventArgs e)
        {
            string rFile = ((ToolStripMenuItem)sender).Text;
            // Check recent file
            bool delete = false;
            if (File.Exists(rFile)) {
                msgPath = rFile;
                RowsClear();
                ReadMsgFile();
            } else if (MessageBox.Show("Message file not found.\n Delete this recent link?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                delete = true; // delete from recent list
            Settings.AddMsgRecentFile(rFile, delete);
            UpdateRecentList();
        }

        private void encodingTextDOSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enc = (encodingTextDOSToolStripMenuItem.Checked) ? Encoding.GetEncoding("cp866") : Encoding.Default;
            if (msgPath != null) {
                dgvMessage.SelectionChanged -= dgvMessage_SelectionChanged;

                RowsClear();
                ReadMsgFile();
                dgvMessage.FirstDisplayedScrollingRowIndex = (SelectLine.row <= 5) ? SelectLine.row : SelectLine.row - 5;
                if (SelectLine.row > 0 || SelectLine.col > 0)
                    dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].Selected = true;

                dgvMessage.SelectionChanged += dgvMessage_SelectionChanged;
            }
        }

        private void MessageEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey) {
                shiftKeyDown = true;
            }
            else if (e.KeyCode == Keys.Escape) {
                e.Handled = true;
                Close();
            }
            else if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Control &&
                     !dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].IsInEditMode)
                DeleteLineStripButton_Click(sender, e);
            else if (e.Control && e.KeyCode == Keys.Subtract) {
                if (FontSizeComboBox.SelectedIndex == 0)
                    return;
                FontSizeComboBox.SelectedIndex--;
            }
            else if (e.Control && e.KeyCode == Keys.Add) {
                if (FontSizeComboBox.SelectedIndex == FontSizeComboBox.Items.Count - 1)
                    return;
                FontSizeComboBox.SelectedIndex++;
            }
        }

        private void MessageEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey) shiftKeyDown = false;
        }

        private void MessageEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (msgSaveButton.Enabled) {
                var result = MessageBox.Show("Do you want to save changes to message file?", "Warning", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes) {
                    msgSaveButton.PerformClick();
                } else if (result == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        string restoreText;
        bool textNeedRestore = false;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool cancelPress = false;
            if (textNeedRestore && keyData == Keys.Escape) {
                isCancelEdit = true;
                dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].Value = restoreText;
                isCancelEdit = false;
            } else if (isEditMode) {
                int keyPress = (int)msg.WParam;
                int caretPossiton = ((TextBox)dgvMessage.EditingControl).SelectionStart;
                if ((keyPress == 40 || keyPress == 38)) { // Up/Down
                    var lines = ((TextBox)dgvMessage.EditingControl).Lines;
                    if (lines.Length == 1) {
                        if (keyPress == 40) // down
                            msg.WParam = (IntPtr)35;
                        else
                            msg.WParam = (IntPtr)36;
                    } else {
                        int len = lines[0].Length;
                        if (keyPress == 40) { // down
                            for (int i = 1; i < lines.Length - 1; i++) len += lines[i].Length + 2;
                            if (caretPossiton > len)
                                cancelPress = true;
                        } else {
                            if (caretPossiton <= len)
                                cancelPress = true;
                        }
                    }
                } else if (keyPress == 37) { // Left
                    if (caretPossiton == 0)
                        cancelPress = true;
                } else if (keyPress == 39) { // Right
                    if (caretPossiton == ((TextBox)dgvMessage.EditingControl).TextLength)
                        cancelPress = true;
                } else if ((keyPress == 35 || keyPress == 36)) { // Home/End
                    if (keyPress == 36)
                        ((TextBox)dgvMessage.EditingControl).SelectionStart = 0;
                    else
                        ((TextBox)dgvMessage.EditingControl).SelectionStart = ((TextBox)dgvMessage.EditingControl).TextLength;
                    cancelPress = true;
                }
            }
            if (cancelPress) msg.WParam = (IntPtr)0;
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void dgvMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar)) {
                if (SelectLine.col == 1)
                    dgvMessage.BeginEdit(true);
                else {
                    textNeedRestore = true;
                    restoreText = (string)dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].Value;
                    isCancelEdit = true;
                    dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].Value = restoreText + e.KeyChar;
                    isCancelEdit = false;
                    dgvMessage.BeginEdit(false);
                }
            }
        }

        private void dgvMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Control && !dgvMessage.MultiSelect) {
                InsertEmptyStripButton_Click(sender, EventArgs.Empty);
                e.SuppressKeyPress = true;
            } else if (e.KeyCode == Keys.Enter && e.Shift && !dgvMessage.MultiSelect) {
                InsertEmptyStripButton_Click(null, EventArgs.Empty);
                e.SuppressKeyPress = true;
            } else if (e.KeyCode == Keys.Enter && !dgvMessage.MultiSelect) {
                string _cell = (string)dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].Value;
                if (_cell != null && _cell.Length > 0 || !AddNewLine()) {
                    dgvMessage.BeginEdit(false);
                }
                 e.SuppressKeyPress = true;
            } else if ((e.KeyCode == Keys.Delete && !e.Control) || e.KeyCode== Keys.Back && !dgvMessage.MultiSelect) {
                BackspaceEdit();
            }
        }

        private void BackspaceEdit()
        {
            var _cell = dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col];
            var data = _cell.Value;
            isCancelEdit = true;
            _cell.Value = String.Empty;
            dgvMessage.BeginEdit(false);
            _cell.Value = data;
            isCancelEdit = false;
        }

        private void playerMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Entry entry = (Entry)dgvMessage.Rows[SelectLine.row].Cells[0].Tag;
            if (entry.msgLine == "-")
                return;

            isCancelEdit = true;

            bool changed = (dgvMessage.Rows[SelectLine.row].Cells[1].Style.ForeColor == Color.Red);

            string text;
            if (entry.pcMark) {
                text = entry.msgText;
                entry.pcMark = false;
                if (!changed)
                    dgvMessage.Rows[SelectLine.row].Cells[1].Style.ForeColor = dgvMessage.RowsDefaultCellStyle.ForeColor;
            } else {
                text = pcMarker + entry.msgText;
                entry.pcMark = true;
                if (!changed)
                    dgvMessage.Rows[SelectLine.row].Cells[1].Style.ForeColor = pcColor;
            }

            dgvMessage.Rows[SelectLine.row].Cells[1].Value = text;
            isCancelEdit = false;
            msgSaveButton.Enabled = true;
        }

        private void MoveToolStripButton_Click(object sender, EventArgs e)
        {
            if (MoveToolStripButton.Checked) {
                EnabledControls(false);
                dgvMessage.EditMode = DataGridViewEditMode.EditProgrammatically;
                dgvMessage.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvMessage.Cursor = Cursors.Hand;
            } else {
                EnabledControls(true);
                dgvMessage.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
                dgvMessage.SelectionMode = DataGridViewSelectionMode.CellSelect;
                dgvMessage.Cursor = DefaultCursor;
            }
        }

        private void EnabledControls(bool status)
        {
            dgvMessage.MultiSelect = !status;
            SendStripButton.Enabled = status;
            IncAddStripButton.Enabled = status;
            InsertEmptyStripButton.Enabled = status;
            InsertCommentStripButton.Enabled = status;
            addToolStripMenuItem.Enabled = status;
            BackStripButton.Enabled = status;
            NextStripButton.Enabled = status;
            playerMarkerToolStripMenuItem.Enabled = status;
            sendLineToolStripMenuItem.Enabled = status;
            addDescriptionToolStripMenuItem.Enabled = status;
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvMessage.MultiSelect) {
                DataGridViewSelectedRowCollection rows = dgvMessage.SelectedRows;
                int index_min = Math.Min(rows[0].Index, rows[rows.Count - 1].Index) - 1;
                int index_max = Math.Max(rows[0].Index, rows[rows.Count - 1].Index);

                if (index_min < 0) return;

                DataGridViewRow row = dgvMessage.Rows[index_min];
                dgvMessage.Rows.RemoveAt(index_min);
                dgvMessage.Rows.Insert(index_max, row);

                msgSaveButton.Enabled = true;

            } else if (SelectLine.row > 0) {
                DataGridViewRow row = dgvMessage.Rows[--SelectLine.row];
                dgvMessage.Rows.RemoveAt(SelectLine.row);
                dgvMessage.Rows.Insert(SelectLine.row + 1, row);

                msgSaveButton.Enabled = true;
            }
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvMessage.MultiSelect) {
                DataGridViewSelectedRowCollection rows = dgvMessage.SelectedRows;
                int index_min = Math.Min(rows[0].Index, rows[rows.Count - 1].Index);
                int index_max = Math.Max(rows[0].Index, rows[rows.Count - 1].Index) + 1;

                if (index_max > dgvMessage.Rows.Count - 1) return;

                DataGridViewRow row = dgvMessage.Rows[index_max];
                dgvMessage.Rows.RemoveAt(index_max);
                dgvMessage.Rows.Insert(index_min, row);

                msgSaveButton.Enabled = true;

            } else if (SelectLine.row < dgvMessage.Rows.Count - 1) {
                DataGridViewRow row = dgvMessage.Rows[++SelectLine.row];
                dgvMessage.Rows.RemoveAt(SelectLine.row);
                dgvMessage.Rows.Insert(SelectLine.row - 1, row);

                msgSaveButton.Enabled = true;
            }
        }

        private void dgvMessage_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var _cell = dgvMessage.Rows[e.RowIndex].Cells[e.ColumnIndex];
            editColor = _cell.Style.BackColor;
            _cell.Style.BackColor = Color.Beige;
            dgvMessage.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            isEditMode = true;
        }

        private void dgvMessage_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dgvMessage.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = editColor;
            dgvMessage.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            textNeedRestore = false;
            isEditMode = false;
        }

        private void alwaysOnTopToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (alwaysOnTopToolStripMenuItem.Checked)
                this.Owner = scriptForm;
            else
                this.Owner = null;
        }

        private void FontSizeChanged(object sender, EventArgs e)
        {
            int size = int.Parse(FontSizeComboBox.Text);
            dgvMessage.Columns[1].DefaultCellStyle.Font = new Font(dgvMessage.Columns[1].DefaultCellStyle.Font.Name, size, FontStyle.Regular);

            // set for line column
            switch (size) {
                case 24:
                    size -= 10; // 14
                    break;
                case 20:
                    size -= 7; // 13
                    break;
                case 18:
                    size -= 6; // 12
                    break;
                case 16:
                    size -= 5; // 11
                    break;
                case 14:
                    size -= 4; // 10
                    break;
                case 12:
                    size -= 3; // 9
                    break;
                default:
                    size -= 1; // 9/8
                    break;
            }
            dgvMessage.Columns[0].DefaultCellStyle.Font = new Font(dgvMessage.Columns[0].DefaultCellStyle.Font.Name, size, FontStyle.Bold);

            if (size >= 13) {
                dgvMessage.Columns[0].Width = 75;
            } else if (size >= 11) {
                dgvMessage.Columns[0].Width = 65;
            } else {
                dgvMessage.Columns[0].Width = 50;
            }

            if (sender != null) Settings.msgFontSize = (byte)FontSizeComboBox.SelectedIndex;
        }

        private void HighlightingCheck(object sender, EventArgs e)
        {
            Settings.msgHighlightComment = HighlightingCommToolStripMenuItem.Checked;

            if (dgvMessage.RowCount > 0)
                HighlightingCommentUpdate();
        }

        private void ColorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvMessage.RowCount > 0)
                Settings.msgHighlightColor = (byte)ColorComboBox.SelectedIndex;

            SetCommentColor();

            if (dgvMessage.RowCount > 0)
                HighlightingCommentUpdate();
        }

        private void dgvMessage_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex > -1 && e.Button == MouseButtons.Middle) {
                dgvMessage.Rows[e.RowIndex].Selected = true;
                SelectLine.row = e.RowIndex;
                sendLineToolStripMenuItem.PerformClick();
            }
        }

        private void dgvMessage_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
                dgvMessage.BeginEdit(true);
        }

        private void dgvMessage_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            if (editAllowed && dgvMessage.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected)
                dgvMessage.BeginEdit(false);

            editAllowed = true;
        }

        private void addDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Entry entry = (Entry)dgvMessage.Rows[SelectLine.row].Cells[0].Tag;
            string desc = entry.description.TrimStart();
            if (InputBox.ShowDialog("Add/Edit Description line", ref desc, 125) == DialogResult.OK) {
                desc = desc.Trim();
                if (desc.Length > 0 && !desc.StartsWith("#")) {
                    desc = desc.Insert(0, " # ");
                }
                entry.description = desc;
                dgvMessage.Rows[SelectLine.row].Cells[1].ToolTipText = entry.description;
                msgSaveButton.Enabled = true;
            }
        }

        private void OpenNotepadtoolStripButton_Click(object sender, EventArgs e)
        {
            if (msgPath != null)
                Settings.OpenInExternalEditor(msgPath);
        }

        private void MessageEditor_Resize(object sender, EventArgs e)
        {
            int width = this.Width - toolStripDropDownButton1.Bounds.Right - 120;
            SearchStripTextBox.Width = (width >= 100) ? width : 100;

            toolStrip.PerformLayout();
            if (SearchStripTextBox.IsOnOverflow) SearchStripTextBox.Width = 240;
        }

        private void openAsTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (msgPath == null) return;

            this.Close();
            if (!this.IsDisposed) return;

            TabInfo ti = ((TextEditor)scriptForm).Open(msgPath, TextEditor.OpenType.File, false);
            if (associateTab != null) associateTab.msgFileTab = ti;
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isEditMode) e.Cancel = true;
        }

        #region Virtual mode events

        private void dgvMessage_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            //Entry entry = rowsEntry[e.RowIndex];

            //switch (e.ColumnIndex) {
            //case 0:
            //    e.Value = entry.msgLine;
            //    if (entry.commentLine && Settings.msgHighlightComment)
            //        dgvMessage.Rows[e.RowIndex].DefaultCellStyle.BackColor = cmColor;
            //    break;
            //case 1:
            //    e.Value = entry.msgText;
            //    if (entry.pcMark)
            //        dgvMessage.Rows[e.RowIndex].Cells[1].Style.ForeColor = pcColor;
            //    break;
            //case 2:
            //    e.Value = entry.msglip;
            //    break;
            //}
        }

        #endregion
    }
}
