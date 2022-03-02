using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI;
using ScriptEditor.TextEditorUtilities;

namespace ScriptEditor
{
    public partial class DialogPreview : Form
    {
        private List<DialogueParser> Arguments = new List<DialogueParser>();

        private List<string> nodesNavigation = new List<string>();
        private int currentNavigation = 0;

        private TabInfo sourceTab;

        private IDocument document
        {
            get { return sourceTab.textEditor.Document; }
        }

        private Procedure[] scrProc
        {
            get { return sourceTab.parseInfo.procs; }
        }

        private int readMsgNum = -1;
        private string[] MessagesData;
        
        private bool allow;
        private bool user;
        private bool needUpdate;

        public bool InitReady 
        {
            get { return (MessagesData != null); }
        }

        public DialogPreview(TabInfo sourceTab)
        {
            InitializeComponent();

            this.Text += sourceTab.filename;
            this.sourceTab = sourceTab;

            NodesComboBox.Items.AddRange(DialogueParser.GetAllNodesName(scrProc).ToArray());

            Procedure curProc = sourceTab.parseInfo.GetProcedureFromPosition(sourceTab.textEditor.ActiveTextAreaControl.Caret.Line);
            if (curProc == null || !NodesComboBox.Items.Contains(curProc.name)) {
                int indx = sourceTab.parseInfo.GetProcedureIndex("talk_p_proc");
                if (indx == -1)
                    return;
                curProc = scrProc[indx];
            }
            
            MessagesData = File.ReadAllLines(sourceTab.msgFilePath, Settings.EncCodePage);
            if (sourceTab.messages.Count == 0) MessageFile.ParseMessages(sourceTab, MessagesData);

            NodesComboBox.Text = curProc.name;
            nodesNavigation.Add(curProc.name);
            GotoNode(curProc);
        }

        private void GotoNode(Procedure curProc)
        {
            Arguments.Clear();
            dgvMessages.Rows.Clear();

            string body = Utilities.GetProcedureCode(document, curProc);
            if (body == null)
                return;
            DialogueParser.ParseNodeCode(body, Arguments, sourceTab.parseInfo);

            BuildMessageDialog();
        }
      
        private void BuildMessageDialog()
        {
            int addFemale = 0;
            if (femaleToolStripMenuItem.Checked)
                int.TryParse(toolStripTextBox.Text, out addFemale);

            string msg, msgPath = null;

            foreach (DialogueParser line in Arguments)
            {
                if (line.opcode == OpcodeType.None) continue;
                int n = addFemale;
                bool error = false;
                if (line.numberMsgLine > 0) {
                    
                    if (readMsgNum != line.numberMsgFile) {
                        if (line.numberMsgFile != -1) {
                            string path;
                            if (!MessageFile.GetPath(sourceTab, line.numberMsgFile, out path)) {
                                msg = String.Format(MessageFile.msgfileError, line.numberMsgFile);
                                msgPath = null;
                                error = true;
                                goto skip;
                            }
                            msgPath = path;
                        } else
                            msgPath = sourceTab.msgFilePath;

                        readMsgNum = line.numberMsgFile;
                        MessagesData = File.ReadAllLines(msgPath, Settings.EncCodePage); // загрузить другой файл сообщений
                    }

                    msg = MessageFile.GetMessages(MessagesData, addFemale + line.numberMsgLine);
                    if (msg == null && addFemale > 0) {
                        msg = MessageFile.GetMessages(MessagesData, line.numberMsgLine);
                        n = 0;
                    }
                    if (msg == null) {
                        msg = MessageFile.messageError;
                        error = true;
                    }
                } else
                    msg = "<" + line.shortcode + ">";
skip:
                dgvMessages.Rows.Add(line.toNode.Trim('"', ' '), msg, line.iq, (line.numberMsgLine > 0) ? n + line.numberMsgLine : line.numberMsgLine);
                dgvMessages.Rows[dgvMessages.Rows.Count - 1].Cells[0].Tag = line.opcode;
                if (line.opcode == OpcodeType.Option || line.opcode == OpcodeType.giq_option || line.opcode == OpcodeType.gsay_option) {
                    dgvMessages.Rows[dgvMessages.Rows.Count - 1].Cells[1].Value = (char)0x25CF + " " + msg;
                    if (!error)
                        dgvMessages.Rows[dgvMessages.Rows.Count - 1].Cells[1].Style.ForeColor = Color.Blue;
                }
                if (line.numberMsgFile != -1)
                    dgvMessages.Rows[dgvMessages.Rows.Count - 1].Cells[3].Tag = msgPath;
                if (error)
                    dgvMessages.Rows[dgvMessages.Rows.Count - 1].Cells[1].Style.ForeColor = Color.Red;
            }
        }

        private void dgvMessages_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string node = dgvMessages.CurrentRow.Cells[0].Value.ToString();
            OpcodeType opcode = (OpcodeType)dgvMessages.CurrentRow.Cells[0].Tag;
            
            if (e.ColumnIndex == 1) {
                if (opcode == OpcodeType.Option || opcode == OpcodeType.giq_option || opcode == OpcodeType.gsay_option || opcode == OpcodeType.call) {
                    OptionsTextLabel.Text = dgvMessages.CurrentRow.Cells[1].Value.ToString();
                    user = false;
                    NodesComboBox.Text = node;
                    AddToNavigation(node);
                    if (JumpToolStripMenuItem.Checked) 
                        JumpProcedure(node);
                }
            } else if (e.ColumnIndex == 0) {
                if (opcode == OpcodeType.Reply || opcode == OpcodeType.Message)
                    node = NodesComboBox.Text;

                JumpProcedure(node);
            }
        }
        
        private void JumpProcedure(string nodeName)
        {                
            int index = sourceTab.parseInfo.GetProcedureIndex(nodeName);
            if (index == -1)
                return;

            TextEditor te = this.Owner as TextEditor; 
            te.SelectLine(scrProc[index].fstart, scrProc[index].d.start, true);
        }

        private void dgvMessages_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3) {
                int nLine = (int)dgvMessages.CurrentRow.Cells[3].Value;
                if (nLine != -1) {
                    string path = (string)dgvMessages.CurrentRow.Cells[3].Tag ?? sourceTab.msgFilePath;
                    MessageEditor.MessageEditorInit(path, nLine).ShowDialog();
                }
            }
        }
        
        private void AddToNavigation(string node)
        {
            int count = nodesNavigation.Count;
            if (++currentNavigation < count) {   
                nodesNavigation.RemoveRange(currentNavigation, count - currentNavigation);
            }
            nodesNavigation.Add(node);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (currentNavigation == 0) return;
            string name = nodesNavigation[--currentNavigation];
            allow = false;
            NodesComboBox.Text = name;
            GotoNode(scrProc[sourceTab.parseInfo.GetProcedureIndex(name)]);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (currentNavigation >= nodesNavigation.Count - 1) return; 
            string name = nodesNavigation[++currentNavigation];
            allow = false;
            NodesComboBox.Text = name;
            GotoNode(scrProc[sourceTab.parseInfo.GetProcedureIndex(name)]);
        }

        private void NodesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (allow) {
                string name = NodesComboBox.Text;
                if (user) {
                    nodesNavigation.Clear();
                    currentNavigation = 0;
                    nodesNavigation.Add(name);
                }
                GotoNode(scrProc[sourceTab.parseInfo.GetProcedureIndex(name)]);
            } else 
                allow = true;
            user = false;
        }

        private void NodesComboBox_DropDownClosed(object sender, EventArgs e)
        {
            user = true;
        }

        private void femaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripTextBox.Enabled = femaleToolStripMenuItem.Checked;
            //update messages
            GotoNode(scrProc[sourceTab.parseInfo.GetProcedureIndex(NodesComboBox.Text)]);
        }

        private void DialogPreview_Activated(object sender, EventArgs e)
        {
            if (!needUpdate)
                return;
            
            List<string> nodes = DialogueParser.GetAllNodesName(scrProc);
            if (NodesComboBox.Items.Count != nodes.Count) {
                var sItem = NodesComboBox.SelectedItem;
                
                allow = false;
                NodesComboBox.Items.Clear();
                NodesComboBox.Items.AddRange(nodes.ToArray());
                
                if (sItem == null)
                    return;
                
                foreach (var item in NodesComboBox.Items)
                {
                    if (item.ToString() == sItem.ToString()) {
                        NodesComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
            needUpdate = false;
        }

        private void DialogPreview_Shown(object sender, EventArgs e)
        {
            TextEditor te = this.Owner as TextEditor;
            te.ParserUpdatedInfo += delegate { needUpdate = true; };
        }
    }
}
