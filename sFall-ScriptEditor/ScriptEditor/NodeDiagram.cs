using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using System.Xml;
using System.Xml.XPath;

using ICSharpCode.ClassDiagram;
using ICSharpCode.Diagrams.Drawables;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI;
using ScriptEditor.TextEditorUI.ToolTips;
using ScriptEditor.TextEditorUI.Nodes;
using ScriptEditor.TextEditorUtilities;

namespace ScriptEditor
{
    public partial class NodeDiagram : Form
    {
        public event EventHandler ChangeNodes; // event for parse script editor

        private ClassCanvas nodesCanvas = new ClassCanvas();

        // List of names of all used nodes in script
        private List<string> nodesProcedureName = new List<string>();
        // List of hidden nodes
        private Dictionary<string, NodeCanvasItem> HideNodes = new Dictionary<string, NodeCanvasItem>();
        // List of all used nodes with parser data
        private Dictionary<string, List<DialogueParser>> NodesData = new Dictionary<string, List<DialogueParser>>();

        private List<ContentBody> NodeBody;
        private List<string> linkFrom;
        private List<LinkTo> linkToList;

        private string[] MessagesData;

        private TabInfo sourceTab;
        private IDocument scriptText;
        private Procedure[] scriptProc;

        private string fcdFilePath = null;   // путь и файл для сохранения

        private string scriptName;
        private bool shiftDown;
        private bool needUpdate;
        private bool autoUpdate;        // save setting
        private bool woExitNode;        // save setting
        private bool fсClosed;          // закрытие основной формы диаграмм

        private bool shouldUpdate = true; // нужно ли обновление холста после изменения в коде скрипта, используетя для предотвращения не нужного обновления.

        public bool NeedUpdate
        {
            private get { return needUpdate; }
            set {
                    if (shouldUpdate) needUpdate = value;
                    shouldUpdate = true;
                    UpdateProceduresInfo();
                }
        }

        #region Form Initialize

        private const string help = "Управление:\n" +
                                    "Shift + Левый Щелчек на строке текта сообщения - Быстрое редактирования сообщения.\n" +
                                    "Ctrl + Левый Щелчек - Добавляет или снимает выделение ноды.\n" +
                                    "Средняя кнопка Мыши - Премещение хоста.\n" +
                                    "Колесеко Мышки - Увеличить/Уменьшить маштаб.\n" +
                                    "Правая кнопка Мышки - Вызвать контекстное меню.";

        public NodeDiagram(TabInfo tabInfo)
        {
            InitializeComponent();

            if (Settings.hintsLang != 0) HelptoolStripButton.ToolTipText = help;

            // set settings
            autoUpdate = autoUpdateNodesToolStripMenuItem.Checked = Settings.autoUpdate;
            woExitNode =  ShowExitNodeToolStripMenuItem.Checked = Settings.woExitNode;
            autoArrangeToolStripMenuItem.Checked = Settings.autoArrange;
            autoSaveOnExitToolStripMenuItem.Checked = Settings.autoSaveChart;
            autoHideNodesToolStripMenuItem.Checked = Settings.autoHideNodes;

            this.Text += tabInfo.filename;

            this.sourceTab = tabInfo;
            this.scriptText = tabInfo.textEditor.Document;
            this.scriptProc = tabInfo.parseInfo.procs;

            nodesCanvas.KeyDown += NodeDiagram_KeyDown;
            nodesCanvas.KeyUp += NodeDiagram_KeyUp;

            this.Controls.Add(nodesCanvas);
            toolStrip.SendToBack();
            nodesCanvas.Dock = DockStyle.Fill;
            nodesCanvas.ContextMenuStrip = contextMenuStrip;

            // Canvas Events
            nodesCanvas.CanvasItemHover += CanvasItemHover;
            nodesCanvas.CanvasItemSelected += CanvasItemSelected;
            nodesCanvas.CanvasItemUnSelected += CanvasItemUnSelected;
            nodesCanvas.LayoutChanged += LayoutChanged;
            nodesCanvas.Scroll += ScrollChanged;
            nodesCanvas.CanvasMouseWheel += CanvasMouseWheel;
            nodesCanvas.ZoomChanged += new EventHandler(CanvasZoomChanged);
            //nodesCanvas.ContextMenuStrip.Closed

            scriptName = scriptProc[tabInfo.parseInfo.GetProcedureIndex("talk_p_proc")].filename;

            InitData();
        }

        private void NodeDiagram_Shown(object sender, EventArgs e)
        {
            nodesCanvas.Visible = false;

            string ext = "." + saveFileDialog.DefaultExt;
            string fcd = Path.ChangeExtension(sourceTab.filepath, ext);
            if (File.Exists(fcd)) {
                fcdFilePath = fcd;
                LoadFlowchartDiagram();
            } else { // fcd sub folder
                fcd = Path.Combine(Path.GetDirectoryName(sourceTab.filepath), saveFileDialog.DefaultExt,
                                   Path.GetFileNameWithoutExtension(sourceTab.filepath) + ext);
                if (File.Exists(fcd)) {
                    fcdFilePath = fcd;
                    LoadFlowchartDiagram();
                } else {
                    #if DEBUG
                        CreateCanvasNodes();
                    #endif
                }
            }
        }
        #endregion

        #region Create Nodes
        /// <summary>
        /// Main Init
        /// </summary>
        private void InitData()
        {
            // получить имена всех Node процедур из скрипта
            nodesProcedureName = DialogueParser.GetAllNodesName(scriptProc);
            if (nodesProcedureName.Count == 0)
                return;

            ReadMessageData(sourceTab.msgFilePath);

            // распарсить полученный блок Node процедуры, проанализировав связи, получить тексты сообщений.
            foreach (var name in nodesProcedureName)
            {
                if (woExitNode && name.Equals("node999", StringComparison.OrdinalIgnoreCase))
                    continue;

                int index = sourceTab.parseInfo.GetProcedureIndex(name, scriptProc);
                string nodeCode = Utilities.GetProcedureCode(scriptText, scriptProc[index]);
                if (nodeCode == null)
                    continue;

                ParseNodeCode(name, nodeCode);
            }
        }

        private List<DialogueParser> ParseNodeCode(string nodeName, string nodeCode)
        {
            List<DialogueParser> DialogNode = new List<DialogueParser>();
            DialogueParser.PrepareNodeCode(nodeCode, DialogNode, sourceTab.parseInfo, !tsbShowCommentCode.Checked);

            // Имя ноды и данные
            NodesData.Add(nodeName, DialogNode);

            return DialogNode;
        }

        /// <summary>
        /// Добавление для отображения нод на холсте, из подготовленных данных NodesData
        /// </summary>
        private void CreateCanvasNodes()
        {
            int shiftY = 0;
            int shiftX = 0;

            foreach (var node in NodesData)
                AddNodeToCanvas(node, ref shiftY, ref shiftX);

            nodesCanvas.Visible = true;
            nodesCanvas.Refresh();
            MessageFile.ShowMissingFiles();
        }

        private void AddNodeToCanvas(KeyValuePair<string, List<DialogueParser>> node, ref int shiftY, ref int shiftX)
        {
            INode nodeData; // this not used
            NodeCanvasItem nodeItem = CreateNodeItem(node, out nodeData);

            if (!nodeData.ShowCodeNodeButton) nodeItem.RemoveItemContex(); // удаляет строки с кодом

            if (Settings.autoHideNodes && nodeItem.GetNodeData.NodeType == NodesType.Unused) {
                nodeItem.Hidden = true;
                HideNodes.Add(node.Key, nodeItem);
            } else {
                nodeItem.X = 25 + shiftX;
                nodeItem.Y = 25 + shiftY;
                nodesCanvas.AddCanvasItem(nodeItem);

                int height = (int)nodeItem.Height;
                if (height > 500) {
                    shiftX += 50 + (int)nodeItem.Width;
                } else {
                    shiftY += 25 + height;
                    shiftX += 25 + ((int)nodeItem.Width / 2);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeData"></param>
        /// <returns></returns>
        private NodeCanvasItem CreateNodeItem(KeyValuePair<string, List<DialogueParser>> node, out INode nodeData)
        {
            nodeData = GetNodeData(node);

            nodeData.ShowCodeNodeButton = tsbShowAllCode.Checked;

            // создать ноду для отображения из сформированных данных DataNode
            NodeCanvasItem nodeItem = ClassCanvas.CreateItemFromType(nodeData);

            nodeItem.ContentClick += ClickContentText;
            nodeItem.ShowCodeButtonClick += nodeItem_ShowCodeButtonClick;

            return nodeItem;
        }

        void nodeItem_ShowCodeButtonClick(NodeCanvasItem item)
        {
            UpdateAllNodes();
        }

        /// <summary>
        /// Подготовить и получить данные для дальнейшего отображения ноды на холсте
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private INode GetNodeData(KeyValuePair<string, List<DialogueParser>> node)
        {
            List<DialogueParser> value = node.Value;
            NodeBody = new List<ContentBody>();

            // node link to node, and message text
            linkToList = new List<LinkTo>();

            for (int n = 0; n < value.Count; n++)
            {
                if (value[n].opcode == OpcodeType.Option || value[n].opcode == OpcodeType.giq_option ||
                    value[n].opcode == OpcodeType.gsay_option || value[n].opcode == OpcodeType.call)
                {
                    linkToList.Add(new LinkTo(value[n].toNode, n + 1));
                }

                string msgText = (value[n].opcode != OpcodeType.None && value[n].opcode != OpcodeType.call)
                               ? GetMessageText(value[n].numberMsgFile, value[n].numberMsgLine)
                               : null;

                //поместить распарсенные данные из Node процедуры в List<ContentBody>
                NodeBody.Add(new ContentBody((value[n].code.StartsWith(new string(' ', Settings.tabSize))) ? value[n].code.Remove(0, Settings.tabSize): value[n].code,
                                             msgText, value[n].opcode, value[n].numberMsgLine, value[n].numberMsgFile, n));
            }

            // получить список имен: link from nodes
            List<string> linklist = new List<string>();

            foreach (var data in NodesData)
            {
                foreach (var nd in data.Value)
                {
                    if (nd.toNode == null || linklist.Contains(data.Key)) continue;

                    if (nd.toNode.Equals(node.Key, StringComparison.OrdinalIgnoreCase)) linklist.Add(data.Key);
                }
            }

            int i = 0;
            linkFrom = new List<string>() { String.Empty, String.Empty };
            foreach (var link in linklist)
            {
                linkFrom[i] += ((linkFrom[i].Length > 0) ? " : " : String.Empty) + link;
                i = 1 - i;
            }

            NodesType type = NodesType.Default;
            if (node.Key.Equals("talk_p_proc", StringComparison.OrdinalIgnoreCase))
                type = NodesType.DialogStart;
            else if (linkToList.Count == 0 && linklist.Count > 0)
                type = NodesType.DialogEnd;
            else if (linkToList.Count > 0 && linklist.Count == 0)
                type = NodesType.NoFromLink;
            else if (linkToList.Count == 0 && linklist.Count == 0)
                type = NodesType.Unused;

            return new DataNode(node.Key, linkToList, linkFrom, NodeBody, type);
        }

        /// <summary>
        /// Получить и подготовить данные для указанной ноды
        /// (метод используется при загрузке сохраненных данных из XML файла)
        /// </summary>
        /// <param name="nodeName">Имя ноды</param>
        private INode GetNodeData(string nodeName)
        {
            if (NodesData.ContainsKey(nodeName))
                return GetNodeData(new KeyValuePair<string, List<DialogueParser>>(nodeName, NodesData[nodeName]));
            else
                return null;
        }

        // UNDONE:
        /*private void UpdateNode(string name)
        {
            if (!NodesData.ContainsKey(name))
                return;

            //UpdateProceduresInfo();

            ///
            int index = sourceTab.parseInfo.GetProcedureIndex(name, scriptProc);
            string nodeProcedureText = Utilities.GetProcedureCode(scriptText, scriptProc[index]);
            if (nodeProcedureText == null)
                return;

            List<DialogueParser> DialogNode = new List<DialogueParser>();
            DialogueParser.PrepareNodeCode(nodeProcedureText, DialogNode, sourceTab.parseInfo, !tsbShowCommentCode.Checked);
            //

            NodesData.Remove(name); // удалить данные существующей ноды

            // Имя ноды и данные
            NodesData.Add(name, DialogNode);

            int dummy = 0;
            var node = new KeyValuePair<string, List<DialogueParser>>(name, NodesData[name]);
            ReCreateNodeCanvas(node, ref dummy);
        }*/

        /// <summary>
        /// Обновить данные для всех созданных нод на холсте
        /// </summary>
        private void UpdateAllNodes()
        {
            NodesData.Clear();
            nodesProcedureName.Clear();

            UpdateProceduresInfo();

            InitData();

            bool focused = (nodesCanvas.GetLastFocusItem() != null);
            int shift = 0;

            List<INode> Nodes = new List<INode>();
            foreach (var node in NodesData)
                Nodes.Add(ReCreateNodeCanvas(node, ref shift));

            //
            nodesCanvas.RemoveUnusedNodes(Nodes);

            if (focused)
                nodesCanvas.SetJustLastFocus();
        }

        /// <summary>
        /// Пересоздание уже существующей ноды на холсте
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private INode ReCreateNodeCanvas(KeyValuePair<string, List<DialogueParser>> node, ref int shift)
        {
            // проверить не скрыта ли нода стаким именем
            bool IsHide = HideNodes.ContainsKey(node.Key);

            INode nodeData;
            NodeCanvasItem newNodeItem = CreateNodeItem(node, out nodeData);

            CanvasItem existNodeItem;
            //получить данные из уже созданной ноды
            if (IsHide)
                existNodeItem = HideNodes[node.Key];
            else
                existNodeItem = nodesCanvas.GetNodeCanvasItem(nodeData.Name);

            if (existNodeItem != null) {
                // установить расположение и размеры ноды на холсте
                newNodeItem.X = existNodeItem.X;
                newNodeItem.Y = existNodeItem.Y;
                newNodeItem.Width = existNodeItem.Width;

                // установить cостояния
                newNodeItem.Collapsed = ((NodeCanvasItem)existNodeItem).Collapsed;

                INode _nodeData = ((NodeCanvasItem)existNodeItem).GetNodeData;
                if (_nodeData.GetStateShowNodeCodeButton()) {
                    nodeData.SetStateShowNodeCodeButton();
                    nodeData.ShowCodeNodeButton = _nodeData.ShowCodeNodeButton;
                    newNodeItem.ViewAllNodeCode = _nodeData.ShowCodeNodeButton;
                }

                for (int l = 0; l < newNodeItem.NodeContentsCount; l++)
                    newNodeItem.SetContentCollapsed(l.ToString(), ((NodeCanvasItem)existNodeItem).ContentIsCollapsed(l.ToString()));

            } else if (!IsHide) {
                //уст. позицию вслучае если ноды на холсте несуществует
                newNodeItem.X = 100 + shift;
                newNodeItem.Y = 100;

                shift += 350;
            }

            if (nodeData.GetStateShowNodeCodeButton()) {
                if (!nodeData.ShowCodeNodeButton)
                    newNodeItem.RemoveItemContex();
            } else if (!tsbShowAllCode.Checked) {
                newNodeItem.RemoveItemContex();
            }

            //добавляем обновленную и удаляем существующую ноду
            if (IsHide) {
                HideNodes.Remove(node.Key);
                newNodeItem.Hidden = true;
                HideNodes.Add(node.Key, newNodeItem);
            } else {
                nodesCanvas.AddCanvasItem(newNodeItem, existNodeItem);
                nodesCanvas.RemoveCanvasItem(existNodeItem);
            }
            return nodeData;
        }

        /// <summary>
        ///  Создать новую ноду на холсте
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="nodeCode"></param>
        private void CreateNewNode(string nodeName, string nodeCode)
        {
            nodesProcedureName.Add(nodeName);

            INode nodeData;
            List<DialogueParser> DialogNode = ParseNodeCode(nodeName, nodeCode);
            NodeCanvasItem nodeItem = CreateNodeItem(new KeyValuePair<string, List<DialogueParser>>(nodeName, DialogNode), out nodeData);

            // Установить позицию для новой ноды
            int x = nodesCanvas.HorizontalScroll.Value + (nodesCanvas.Width / 2);
            int y = nodesCanvas.VerticalScroll.Value + (nodesCanvas.Height / 2);
            nodeItem.X = (x - (nodeItem.Width / 2)) / nodesCanvas.Zoom;
            nodeItem.Y = (y - (nodeItem.Height / 2)) / nodesCanvas.Zoom;

            nodeItem.Collapsed = false;

            if (!nodeData.ShowCodeNodeButton) nodeItem.RemoveItemContex(); // удаляет строки с кодом

            nodesCanvas.AddCanvasItem(nodeItem);
            nodesCanvas.SetFocusedCanvasItem(nodeItem);

            #region Simple example add new node
            /*
            * List<LinkTo> linkList;
            * List<ContentBody> ProcedureBody;
            * List<string> linkfrom = new List<string>();
            * linkList = new List<LinkTo>();
            * 
            * linkfrom.Add("Start");
            * linkfrom.Add("End");
            * linkList.Add(new LinkTo("Node001",4));
            * linkList.Add(new LinkTo("Node002",6));
            * 
            * ProcedureBody = new List<ContentBody>();
            * ProcedureBody.Add(new ContentBody("If <condition>", null, OpcodeType.None));
            * ProcedureBody.Add(new ContentBody("reply", "Я только прошу не трогать ее и, э-э, не фотографировать со вспышкой.", OpcodeType.Reply));
            * ProcedureBody.Add(new ContentBody("else", null, OpcodeType.None));
            * ProcedureBody.Add(new ContentBody("options", "Как твое имя?", OpcodeType.Option));
            * ProcedureBody.Add(new ContentBody("end", null, OpcodeType.None));
            * ProcedureBody.Add(new ContentBody("options", "Проваливай!", OpcodeType.Option));
            * NodeCanvasItem item = ClassCanvas.CreateItemFromType(new DataNode(nodeName, linkList, linkfrom, ProcedureBody, NodesType.Unused));
            */
            #endregion
        }
        #endregion

        #region Hide/Unhide nodes
        private void HideNode(NodeCanvasItem node)
        {
            string key = node.GetNodeData.Name;
            node.Hidden = true;
            HideNodes.Add(key, node);
            nodesCanvas.RemoveCanvasItem(node);
        }

        private void UnHideNode(string key)
        {
            NodeCanvasItem item = HideNodes[key];
            if (item.X == 0 && item.Y == 0)
                    item.Y = item.X = 25;
            item.Hidden = false;
            nodesCanvas.ReAddedCanvasItem(item);
            HideNodes.Remove(key);

            //nodesCanvas.ClearAllItemsFocus();
            nodesCanvas.Refresh();
        }

        private void UnHideAll()
        {
            int shift = 20;
            foreach (NodeCanvasItem item in HideNodes.Values) {
                if (item.X == 0 && item.Y == 0) {
                    item.Y = 20;
                    item.X = shift;
                    shift += (int)item.Width + 20;
                }
                item.Hidden = false;
                nodesCanvas.ReAddedCanvasItem(item);
            }
            HideNodes.Clear();

            nodesCanvas.ClearAllItemsFocus();
            nodesCanvas.Refresh();
        }
        #endregion

        // Update data procedures
        private void UpdateProceduresInfo()
        {
            scriptProc = ParserInternal.GetProcsData(scriptText.TextContent, sourceTab.filepath);
        }

        private void ReadMessageData(string msgfilePath)
        {
            MessagesData = File.ReadAllLines(msgfilePath, Settings.EncCodePage);
            if (msgfilePath == sourceTab.msgFilePath)
                MessageFile.ParseMessages(sourceTab, MessagesData);
        }

        private string GetMessageText(int msgNum, int msgLineNum)
        {
            string msg = null;
            if (msgLineNum > 0) {
                if (msgNum > 0) {
                    string path;
                    if (MessageFile.GetPath(sourceTab, msgNum, out path, true)) {
                        string[] MsgData = File.ReadAllLines(path, Settings.EncCodePage);
                        msg = MessageFile.GetMessages(MsgData, msgLineNum);
                    } else
                        msg = String.Format(MessageFile.msgfileError, msgNum);
                }
                else if (sourceTab.messages.ContainsKey(msgLineNum)) {
                        msg = sourceTab.messages[msgLineNum];
                }
                if (msg == null) msg = MessageFile.messageError;
            } else
                msg = "ParseCode: <Could not get line number of this code>";

            return msg;
        }

        private bool NodeIsEditing(NodeCanvasItem node)
        {
            if (node.IsEditing) return true;

            foreach (var fcTE in sourceTab.nodeFlowchartTE)
            {
                if (fcTE.NodeName == node.GetNodeData.Name) return true;
            }
            return false;
        }

        private void CanvasItemHover(object sender, CanvasItemEventArgs e)
        {
            if (e.CanvasItem is NodeCanvasItem) {
                editNodeToolStripMenuItem.Enabled = true;
                editNodeToolStripMenuItem.Tag = e.CanvasItem;
                editNodeToolStripMenuItem.Text = "Edit Node: " + e.CanvasItem.ToString();

                if (((NodeCanvasItem)e.CanvasItem).GetNodeData.Name.Equals("talk_p_proc", StringComparison.OrdinalIgnoreCase) == false) {
                    renameNodeToolStripMenuItem.Enabled = !NodeIsEditing(((NodeCanvasItem)e.CanvasItem));
                    renameNodeToolStripMenuItem.Tag = e.CanvasItem;
                } else
                    renameNodeToolStripMenuItem.Enabled = false;
                renameNodeToolStripMenuItem.Text = "Rename Node: " + e.CanvasItem.ToString();
            } else {
                editNodeToolStripMenuItem.Enabled = false;
                editNodeToolStripMenuItem.Tag = null;
                editNodeToolStripMenuItem.Text = "Edit Node";

                renameNodeToolStripMenuItem.Enabled = false;
                renameNodeToolStripMenuItem.Tag = null;
                renameNodeToolStripMenuItem.Text = "Rename Node";
            }
        }

        private void CanvasItemSelected(object sender, CanvasItemEventArgs e)
        {
            if (e.CanvasItem is NodeCanvasItem) {
                NodeCanvasItem NodeItem = (NodeCanvasItem)e.CanvasItem;
                contextMenuStrip.Tag = e.CanvasItem;

                hideNodeToolStripMenuItem.Enabled = true;
                deleteNoteToolStripMenuItem.Enabled = !NodeIsEditing(NodeItem); //!NodeItem.IsEditing;

                JumpNodeToolStripMenuItem.Enabled = true;
                JumpNodeToolStripMenuItem.Text = "Jump from: " + NodeItem.ToString() + " to";

                if (JumpNodeToolStripMenuItem.Tag != e.CanvasItem) {
                    JumpNodeToolStripMenuItem.DropDownItems.Clear();
                    if (NodeItem.GetNodeData.LinkedToNodes.Count > 0) {
                        JumpNodeToolStripMenuItem.DropDownItems.Add("-- Link To --");
                        JumpNodeToolStripMenuItem.DropDownItems[0].Enabled = false;
                    }
                }
                else if (JumpNodeToolStripMenuItem.Tag != null)
                         return;

                JumpNodeToolStripMenuItem.Tag = e.CanvasItem;

                foreach (var linkTo in NodeItem.GetNodeData.LinkedToNodes)
                {
                    JumpNodeToolStripMenuItem.DropDownItems.Add(linkTo.NameTo, null, new EventHandler(JumpClick));
                    int count = JumpNodeToolStripMenuItem.DropDownItems.Count - 1;

                    JumpNodeToolStripMenuItem.DropDownItems[count].ForeColor = (HideNodes.ContainsKey(linkTo.NameTo))
                                                                             ? Color.Gray : Color.OrangeRed;
                }

                string linkLine = String.Join(":", NodeItem.GetNodeData.LinkedFromNodes);
                string[] linkFrom = linkLine.Split(new char[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                if (linkFrom.Length > 0) {
                    JumpNodeToolStripMenuItem.DropDownItems.Add("-- Link From --");
                    JumpNodeToolStripMenuItem.DropDownItems[
                        JumpNodeToolStripMenuItem.DropDownItems.Count - 1].Enabled = false;

                    foreach (var link in linkFrom)
                    {
                        JumpNodeToolStripMenuItem.DropDownItems.Add(link.Trim(), null, new EventHandler(JumpClick));
                        int count = JumpNodeToolStripMenuItem.DropDownItems.Count - 1;
                        JumpNodeToolStripMenuItem.DropDownItems[count].ForeColor = (HideNodes.ContainsKey(link.Trim()))
                                                                                    ? Color.Gray : Color.Blue;
                    }
                }
            } else {
                CanvasItemUnSelected(null, e);
                if (e.CanvasItem is NoteCanvasItem)
                    deleteNoteToolStripMenuItem.Enabled = true;
            }
        }

        private void CanvasItemUnSelected(object sender, EventArgs e)
        {
            JumpNodeToolStripMenuItem.Enabled = false;
            JumpNodeToolStripMenuItem.Text = "Jump from: --- to";
            JumpNodeToolStripMenuItem.DropDownItems.Clear();
            JumpNodeToolStripMenuItem.DropDownItems.Add("-- Link To --");
            JumpNodeToolStripMenuItem.DropDownItems[0].Enabled = false;
            JumpNodeToolStripMenuItem.Tag = contextMenuStrip.Tag = null;

            hideNodeToolStripMenuItem.Enabled = false;
            deleteNoteToolStripMenuItem.Enabled = false;
        }

        private void JumpClick(object sender, EventArgs e)
        {
            string name = ((ToolStripItem)sender).Text;

            if (HideNodes.ContainsKey(name))
                UnHideNode(name);

            NodeCanvasItem NodeItem = (NodeCanvasItem)nodesCanvas.GetNodeCanvasItem(name);
            if (NodeItem == null)
                return;

            JumpToNode(NodeItem);
            nodesCanvas.SetFocusedCanvasItem(NodeItem);
        }

        private void JumpToNode(NodeCanvasItem NodeItem)
        {
            float zoom = nodesCanvas.Zoom;
            Point position = new Point((int)(NodeItem.X * zoom), (int)(NodeItem.Y * zoom));

            // центрируем положение
            int w = ((int)(NodeItem.Width * zoom)) / 2;
            int h = ((int)(NodeItem.Height * zoom)) / 2;
            Size cs = nodesCanvas.ClientSize;
            position.Offset(-(cs.Width / 2) + w, -(cs.Height / 2) + h);

            nodesCanvas.SetCanvasScrollPosition = position;
        }

        private void ClickContentText(object sender, TextSegment ts)
        {
            if (ts.IndexContent == -1) return;

            if (shiftDown) {
                ContentBody cbData = ((NodeCanvasItem)sender).GetNodeData.NodeContent[ts.IndexContent];
                if (cbData.msgNum == -1)
                    return;
                EditContenText(cbData.msgNum, cbData.msgFileNum, ts);
            } else
                ShowContentText(ts);
        }

        private void EditContenText(int msgNum, int msgFileNum, TextSegment ts)
        {
            string msgfile = null;
            string message = null;
            shiftDown = false;

            if (msgFileNum != -1) {
                if (!MessageFile.GetPath(sourceTab, msgFileNum, out msgfile)) {
                    MessageBox.Show("The requested message file: " + msgfile + "\ncould not be found.", "Missing messages file");
                    return;
                }
            } else
                msgfile = sourceTab.msgFilePath;

            ReadMessageData(msgfile);

            if (msgFileNum != -1)
                message = MessageFile.GetMessages(MessagesData, msgNum);
            else if (sourceTab.messages.ContainsKey(msgNum))
                message = sourceTab.messages[msgNum];

            if (message == null) {
                MessageBox.Show("The requested message line number in: " + msgfile + "\nfile, could not be found.", "Message missing");
                return;
            }

            if (InputBox.ShowDialog("Message file: " + msgfile, ref message) == DialogResult.OK) {
                if (MessageFile.SaveToMessageFile(ref MessagesData, msgfile, message, msgNum)) {
                    if (msgFileNum == -1)
                        sourceTab.messages[msgNum] = message;
                    ts.Text = "\"" + message + "\"";
                }
            }
        }

        private void ShowContentText(TextSegment ts)
        {
            int x = (int)(ts.AbsoluteX * nodesCanvas.Zoom) - nodesCanvas.HorizontalScroll.Value;
            int y = (int)(ts.AbsoluteY * nodesCanvas.Zoom) - nodesCanvas.VerticalScroll.Value;

            int lines = ts.GetTextLines();
            switch (lines) {
                case 3:
                    y += 2;
                    break;
                case 2:
                    y += (int)(ts.TextHeight * nodesCanvas.Zoom) - 5;
                    break;
                case 1:
                    y += (int)(ts.TextHeight * nodesCanvas.Zoom);
                    break;
                default:
                    y -= 2;
                    break;
            }

            string message = ts.Text;
            int len = message.Length;
            if ((lines > 0 && ts.TextWidth > ts.ActualWidth) ||
                (lines == 0 && len > 100 && (ts.TextWidth > ts.ActualWidth))) {
                int loop = 1;         // количество частей: 1 - строка поделена на две части
                int divPos = len / 2; // делим строку пополам

                if (divPos > 75) {
                    loop++;
                    divPos = len / 3;
                    if (divPos > 75) {
                        loop++;
                        divPos = len / 4;
                    }
                }

                int incPos = divPos;
                int i, j;

                for (int l = loop; l > 0; l--)
                {
                    int insertPos = i = j = divPos;
                    while (j > 0 && i < len)
                    {
                        if (Char.IsWhiteSpace(message[--j])) { // назад
                            insertPos = j;
                            divPos -= (divPos - j);
                            break;
                        }
                        else if (Char.IsWhiteSpace(message[i])) { // вперед
                            insertPos = i;
                            divPos += (i - divPos);
                            break;
                        }
                        i++;
                    }
                    message = message.Insert(insertPos, Environment.NewLine);
                    divPos += incPos; // следующая позиция вставки
                }
            }
            // popup show
            msgPopup.RemoveAll();
            msgPopup.Tag = message;
            msgPopup.Show(message, nodesCanvas, x, y, 1000 + (len * 50));
        }

        private void CreatetoolStripButton_Click(object sender, EventArgs e)
        {
            if (nodesCanvas.NodesTotalCount > 0)
                if (MessageBox.Show("Do you want to clean up the existing flowchart and create a new one?", "Create New",
                    MessageBoxButtons.YesNo) == DialogResult.No)
                    return;

            nodesCanvas.ClearCanvas();
            HideNodes.Clear();
            CreateCanvasNodes();

            if (autoArrangeToolStripMenuItem.Checked)
                nodesCanvas.AutoArrange();
            else
                nodesCanvas.Refresh();

            HelpTip.Show(@"NOTE: Save the flowchart file to the folder (or subfolder \fcd) of the source script, so that the editor next time automatically opens the flowchart file.", nodesCanvas, 10, 10, 10000);

            nodesCanvas.Select();
        }

        private void renameNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string nName = ((NodeCanvasItem)(renameNodeToolStripMenuItem.Tag)).GetNodeData.Name;
            nName = Refactor.RenameProcedure(nName, scriptText, sourceTab);
            if (nName != null) {
                if (nName.IndexOf("node", StringComparison.OrdinalIgnoreCase) == -1) {
                    MessageBox.Show("The name of the procedure must contain the word 'Node'");
                    scriptText.UndoStack.Undo();
                    scriptText.UndoStack.ClearRedoStack();
                    return;
                }
                ((NodeCanvasItem)(renameNodeToolStripMenuItem.Tag)).GetNodeData.Name = nName;
                UpdateAllNodes();
            }
        }

        private void deleteNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nodesCanvas.ActiveControl is TextBox) {
                // удаление текста в TextBox
                TextBox editBox = nodesCanvas.ActiveControl as TextBox;
                if (editBox.Text.Length > 0) {
                    int posCaret = editBox.SelectionStart;
                    if (posCaret >= editBox.Text.Length) return;

                    editBox.Text = editBox.Text.Remove(posCaret, 1);
                    editBox.SelectionStart = posCaret;
                }
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete the all selected nodes/notes?\n(When delete a node, the corresponding procedure in the script will be deleted).", "Deleting",
                MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            List<string> removeProcName = new List<string>();

            do {
                CanvasItem item = nodesCanvas.GetLastFocusItem();
                if (item == null)
                    break;

                if (item is NoteCanvasItem) {
                    nodesCanvas.RemoveCanvasItem(item);
                    continue;
                }

                string name = ((NodeCanvasItem)item).GetNodeData.Name;
                if (name.Equals("talk_p_proc", StringComparison.OrdinalIgnoreCase)) {
                    MessageBox.Show("You cannot delete the procedural handler 'talk_p_proc' from the flowchart editor.");
                    break; //don't delete talk_p_proc
                }

                if (NodeIsEditing((NodeCanvasItem)item)) {
                    MessageBox.Show("You cannot delete a node procedure: " + name + " that is edited at the current time.");
                    break;
                }

                nodesCanvas.RemoveCanvasItem(item);
                NodesData.Remove(name);
                nodesProcedureName.Remove(name);
                removeProcName.Add(name);

            } while (true);

            if (removeProcName.Count > 0) {
                Utilities.DeleteProcedure(removeProcName, scriptText);

                if (ChangeNodes != null)
                    ChangeNodes(this, EventArgs.Empty); //event
            }

            nodesCanvas.ClearDragItems();

            CanvasItemUnSelected(null, EventArgs.Empty);
            UpdateAllNodes();

            nodesCanvas.Refresh();
        }

        private void addNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TemplateNode tNode = new TemplateNode();
            tNode.CreateClick += new TemplateNode.CreateClickHandler(CreateClick);
            tNode.ShowForm();
        }

        private bool CreateClick(object sender, string nodeName, string nodeCode)
        {
            string defName = "Node";
            if (nodeName.IndexOf(defName, StringComparison.OrdinalIgnoreCase) < 0)
                nodeName = defName + nodeName;

            if (sourceTab.parseInfo.CheckExistsName(nodeName, NameType.Proc)) {
                MessageBox.Show("The procedure/variable or declared macro with this name already exists.", "Unable to rename");
                return false;
            }

            CreateNewNode(nodeName, nodeCode);

            shouldUpdate = false; // не обновлять другие ноды на холсте

            //Создать процедуру в скрипте
            int dummy = 0;
            ParserInternal.UpdateParseBuffer(scriptText.TextContent);
            int declrLine = ParserInternal.GetEndLineProcDeclaration();
            int procLine = scriptText.TotalNumberOfLines - 1;
            string procblock = "\r\nprocedure " + nodeName + " begin\r\n" + nodeCode + "\r\nend\r\n";
            Utilities.InsertProcedure(sourceTab.textEditor.ActiveTextAreaControl,
                                      nodeName, procblock, declrLine, procLine, ref dummy);

            nodesCanvas.Refresh();

            if (ChangeNodes != null)
                ChangeNodes(this, EventArgs.Empty); //event

            return true; //close dialog
        }

        private void addNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NoteCanvasItem note = new NoteCanvasItem();
            note.X = (40 + nodesCanvas.HorizontalScroll.Value) / nodesCanvas.Zoom;
            note.Y = (40 + nodesCanvas.VerticalScroll.Value) / nodesCanvas.Zoom;
            note.Width = 200;
            note.Height = 50;
            nodesCanvas.AddCanvasItem(note);

            nodesCanvas.Refresh();
        }

        private void CanvasZoomChanged(object sender, EventArgs e)
        {
            int zoom = (int)Math.Round(nodesCanvas.Zoom * 100f);
            if (e == null) {
                Zoom.Value = zoom; // не изменять маштаб
                return;
            }
            PercentLabel.Text = zoom + "%";
            if (Zoom.Value != zoom)
                Zoom.Value = zoom;
        }

        private void Zoom_ValueChanged(object sender, EventArgs e)
        {
            nodesCanvas.Zoom = Zoom.Value / 100.0f;
        }

        private void CanvasMouseWheel(object sender, MouseEventArgs e)
        {
            int value = 2; //inc/dec zoom value

            int zm = Zoom.Value;

            if (e.Delta > 0)
                zm += value;
            else
                zm -= value;

            if (zm < 5)
                zm = 5;
            else if (zm > 200)
                zm = 200;

            Zoom.Value = zm;

            #if DEBUG
                //ZoomLabel.Text = string.Format("X:{0}% : Y:{1}%", nodesCanvas.Percent.X, nodesCanvas.Percent.Y);
            #endif
        }

        private void editNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NodeCanvasItem node = (CanvasItem)editNodeToolStripMenuItem.Tag as NodeCanvasItem;
            if (node == null)
                return;

            foreach (var fcTE in sourceTab.nodeFlowchartTE)
            {
                if (fcTE.NodeName.Equals(node.GetNodeData.Name, StringComparison.OrdinalIgnoreCase)) {
                    fcTE.Activate();
                    return;
                }
            }

            ((NodeCanvasItem)editNodeToolStripMenuItem.Tag).StartEditingNode(); //set node edit status

            int index = sourceTab.parseInfo.GetProcedureIndex(node.GetNodeData.Name, scriptProc);
            if (index == -1)
                return;
            FlowchartTE nodeEditor = new FlowchartTE(scriptProc[index], sourceTab, nodesProcedureName, true);
            sourceTab.nodeFlowchartTE.Add(nodeEditor);

            nodeEditor.ApplyCode += nodeEditor_ApplyCode;
            nodeEditor.Disposed += new EventHandler(nodeEditor_Disposed);
            nodeEditor.ShowEditor(this, nodeEditLink: (NodeCanvasItem)editNodeToolStripMenuItem.Tag);

            renameNodeToolStripMenuItem.Enabled = deleteNoteToolStripMenuItem.Enabled = false;
        }

        void nodeEditor_Disposed(object sender, EventArgs e)
        {
            if (fсClosed)
                return;

            foreach (var fcTE in sourceTab.nodeFlowchartTE)
            {
                if (fcTE.NodeName.Equals(((FlowchartTE)sender).NodeName, StringComparison.OrdinalIgnoreCase)) {
                    sourceTab.nodeFlowchartTE.Remove(fcTE);
                    break;
                }
            }
        }

        void nodeEditor_ApplyCode(object sender, FlowchartTE.CodeArgs e)
        {
            if (e.Change) {
                if (Utilities.ReplaceProcedureCode(scriptText, sourceTab.parseInfo, e.Name, e.Code)) {
                    MessageBox.Show("In the source script, there is no dialog node with this name.", "Apply error");
                    return;
                }

                if (ChangeNodes != null)
                    ChangeNodes(this, EventArgs.Empty); //event

                if (!fсClosed)
                    UpdateAllNodes();
                //else
                //    UpdateProceduresInfo();

                e.Change = false;
            }
            if (!fсClosed)
                e.Close = true;
        }

        private void CollapsetoolStripButton_Click(object sender, EventArgs e)
        {
            nodesCanvas.CollapseAll(shiftDown);
        }

        private void ExpandtoolStripButton_Click(object sender, EventArgs e)
        {
            nodesCanvas.ExpandAll(shiftDown);
        }

        private void ShrinkAlltoolStripButton_Click(object sender, EventArgs e)
        {
            nodesCanvas.ShrinkAllWidths();
        }

        private void MatchAlltoolStripButton_Click(object sender, EventArgs e)
        {
            nodesCanvas.MatchAllWidths();
        }

        private void HidePopUp()
        {
            msgPopup.Hide(nodesCanvas);
            HelpTip.Hide(nodesCanvas);
        }

        void ScrollChanged(object sender, EventArgs e)
        {
            HidePopUp();
        }

        void LayoutChanged(object sender, EventArgs e)
        {
            HidePopUp();
        }

        void NodeDiagram_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
                shiftDown = false;
        }

        void NodeDiagram_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
                shiftDown = true;
        }

        private void highQualityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nodesCanvas.HighQuality = highQualityToolStripMenuItem.Checked;
            nodesCanvas.Refresh();
        }

        private void ArrangetoolStripButton_Click(object sender, EventArgs e)
        {
           nodesCanvas.AutoArrange();
        }

        private void UpdatetoolStripButton_Click(object sender, EventArgs e)
        {
            UpdateNodes();
        }

        private void UpdateNodes()
        {
            NeedUpdate = false;
            UpdateAllNodes();
            nodesCanvas.Refresh();
        }

        private void NodeDiagram_FormClosing(object sender, FormClosingEventArgs e)
        {
            fсClosed = true;
            foreach (FlowchartTE fcTE in sourceTab.nodeFlowchartTE)
            {
                if (fcTE.OpenFromDiagram)
                    fcTE.CloseEditor(true);
            }

            if (autoSaveOnExitToolStripMenuItem.Checked && fcdFilePath != null)
                WriteToXml().Save(fcdFilePath);
        }

        private void NodeDiagram_Activated(object sender, EventArgs e)
        {
            if (autoUpdate && NeedUpdate) {
                UpdateAllNodes();
                NeedUpdate = false;
            } else if (NeedUpdate) {
                HelpTip.Show("You need to update the nodes. The script source code has been changed.", nodesCanvas, 10, 10, 5000);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            nodesCanvas.WireOnlySelect = toolStripButton1.Checked;
            nodesCanvas.Refresh();
        }

        private void autoUpdateNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoUpdate = Settings.autoUpdate = autoUpdateNodesToolStripMenuItem.Checked;
        }

        private void ShowExitNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            woExitNode = Settings.woExitNode = ShowExitNodeToolStripMenuItem.Checked;
        }

        private void Zoom_MouseLeave(object sender, EventArgs e)
        {
            if (Zoom.Focused)
                nodesCanvas.Focus();
        }

        private void Zoom_MouseEnter(object sender, EventArgs e)
        {
            if (nodesCanvas.Focused && !Zoom.Focused)
                Zoom.Focus();
        }

        private void autoArrangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.autoArrange = autoArrangeToolStripMenuItem.Checked;
        }

        private void autoSaveOnExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.autoSaveChart = autoSaveOnExitToolStripMenuItem.Checked;
        }

        private void lowDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassCanvas.NodeLowDetails = lowDetailsToolStripMenuItem.Checked;
            UpdateNodes();
        }

        private void tsbShowCommentCode_Click(object sender, EventArgs e)
        {
            if (tsbShowCommentCode.Checked)
                tsbShowCommentCode.ForeColor = Color.Red;
            else
                tsbShowCommentCode.ForeColor = SystemColors.ControlText;
            UpdateAllNodes();
            nodesCanvas.Refresh();
        }

        private void HelptoolStripButton_Click(object sender, EventArgs e)
        {
            HelpTip.Show(HelptoolStripButton.ToolTipText, nodesCanvas, nodesCanvas.Width - 300, 10, 10000);
        }

        private void contextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            if (!Program.KeyHook(Keys.ControlKey))
                nodesCanvas.CtrlDown = false;
        }

        private void autoHideNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.autoHideNodes = autoHideNodesToolStripMenuItem.Checked;
        }

        private void contextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (unhideNodeToolStripMenuItem.HasDropDownItems)
                return;

            foreach (var key in HideNodes.Keys)
                unhideNodeToolStripMenuItem.DropDownItems.Add(key, null, new EventHandler(UnHideNode_Click));
        }

        private void UnHideNode_Click(object sender, EventArgs e)
        {
            UnHideNode(((ToolStripItem)sender).Text);
            unhideNodeToolStripMenuItem.DropDownItems.Clear();
        }

        private void unhideAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnHideAll();
            unhideNodeToolStripMenuItem.DropDownItems.Clear();
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NodeCanvasItem item = (NodeCanvasItem)contextMenuStrip.Tag;
            do {
                HideNode(item);
                item = (NodeCanvasItem)nodesCanvas.GetLastFocusItem();
            } while (item != null);

            CanvasItemUnSelected(null, EventArgs.Empty);
            nodesCanvas.Refresh();
            unhideNodeToolStripMenuItem.DropDownItems.Clear();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void addEditRulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ScriptEditor.TextEditorUI.Function.FunctionsRules().ShowDialog(this);
        }

        private void showAllCode_Click(object sender, EventArgs e)
        {
            tsbShowAllCode.Checked = !tsbShowAllCode.Checked;
            UpdateNodes();
        }

        #region Save/Load Diagram
        private void openDiagramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = fcdFilePath ?? sourceTab.filepath;
            openFileDialog.InitialDirectory = Path.GetDirectoryName(path);

            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                fcdFilePath = openFileDialog.FileName;
                LoadFlowchartDiagram();
            }
        }

        private void saveDiagramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fcdFilePath == null) {
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(sourceTab.filepath);
                saveFileDialog.FileName = Path.ChangeExtension(scriptName, saveFileDialog.DefaultExt);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    fcdFilePath = saveFileDialog.FileName;
                else
                    return;
            }
            WriteToXml().Save(fcdFilePath);
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "png";
            sfd.AddExtension = true;
            sfd.Filter = "PNG Image (.png)|*.png";
            if (sfd.ShowDialog() == DialogResult.OK)
                nodesCanvas.SaveToImage(sfd.FileName);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        private XmlDocument WriteToXml ()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<NodesDiagram/>");

            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
            doc.InsertBefore(decl, doc.FirstChild);

            XmlAttribute script = doc.CreateAttribute("ScriptName");
            script.Value = scriptName;
            doc.DocumentElement.Attributes.Append(script);

            XmlAttribute zoom = doc.CreateAttribute("Zoom");
            zoom.Value = nodesCanvas.Zoom.ToString(System.Globalization.CultureInfo.InvariantCulture);
            doc.DocumentElement.Attributes.Append(zoom);

            XmlAttribute positionX = doc.CreateAttribute("PositionX");
            positionX.Value = nodesCanvas.HorizontalScroll.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            doc.DocumentElement.Attributes.Append(positionX);

            XmlAttribute positionY = doc.CreateAttribute("PositionY");
            positionY.Value = nodesCanvas.VerticalScroll.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            doc.DocumentElement.Attributes.Append(positionY);

            //Save hidden nodes
            foreach (var item in HideNodes)
                item.Value.WriteToXml(doc);

            return nodesCanvas.WriteToXml(doc);
        }

        private void LoadFlowchartDiagram()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fcdFilePath);
            LoadFromXml(doc);

            // Check and create new node
            int shiftX = 0;
            foreach (var item in NodesData)
            {
                int shiftY = 0;
                if (!HideNodes.ContainsKey(item.Key) && !nodesCanvas.Contains(item.Key)) {
                    MessageBox.Show("Added new node from the script: " + item.Key, "Loading...");
                    AddNodeToCanvas(item, ref shiftY, ref shiftX);
                }
            }

            nodesCanvas.Visible = true;
            nodesCanvas.Refresh();
            MessageFile.ShowMissingFiles();
            nodesCanvas.Select();
        }

        private void LoadFromXml (IXPathNavigable doc)
        {
            XPathNavigator nav = doc.CreateNavigator();

            XPathNodeIterator ni = nav.Select(@"/NodesDiagram");
            ni.MoveNext();
            string nameScript = ni.Current.GetAttribute("ScriptName", "");
            if (!scriptName.Equals(nameScript, StringComparison.OrdinalIgnoreCase)) {
                MessageBox.Show("This flowchart file was saved for another script file.", "Wrong Flowchart: " + nameScript);
                return;
            }

            float zoom = float.Parse(ni.Current.GetAttribute("Zoom", ""), System.Globalization.CultureInfo.InvariantCulture);
            int positionX = int.Parse(ni.Current.GetAttribute("PositionX", ""), System.Globalization.CultureInfo.InvariantCulture);
            int positionY = int.Parse(ni.Current.GetAttribute("PositionY", ""), System.Globalization.CultureInfo.InvariantCulture);

            nodesCanvas.ClearCanvas();
            HideNodes.Clear();

            ni = nav.Select(@"/NodesDiagram/Node");
            while (ni.MoveNext())
            {
                string nodeName = ni.Current.GetAttribute("Name", "");
                INode nd = GetNodeData(nodeName);

                NodeCanvasItem canvasitem = ClassCanvas.CreateItemFromType(nd);
                if (canvasitem != null) {
                    canvasitem.LoadFromXml(ni.Current);
                    canvasitem.ContentClick += ClickContentText;
                    canvasitem.ShowCodeButtonClick += nodeItem_ShowCodeButtonClick;

                    // установить сохраненное значение кнопки ShowCode для ноды
                    if (NodeCanvasItem.showCode != -1) {
                        nd.SetStateShowNodeCodeButton();
                        nd.ShowCodeNodeButton = (NodeCanvasItem.showCode != 0);
                    } else {
                        nd.ShowCodeNodeButton = tsbShowAllCode.Checked;
                    }
                    canvasitem.ViewAllNodeCode = nd.ShowCodeNodeButton;
                    if (!nd.ShowCodeNodeButton) {
                        canvasitem.RemoveItemContex();
                    }

                    if (!canvasitem.Hidden)
                        nodesCanvas.AddCanvasItem(canvasitem);
                    else
                        HideNodes.Add(nodeName, canvasitem);
                } else
                    MessageBox.Show("Deleted existing dialog node: " + nodeName + ", that does not exist in the script code.", "Loading...");
            }

            ni = nav.Select(@"/NodesDiagram/Note");
            while (ni.MoveNext())
            {
                NoteCanvasItem note = new NoteCanvasItem();
                note.LoadFromXml(ni.Current);
                nodesCanvas.AddCanvasItem(note);
            }

            nodesCanvas.Zoom = zoom;
            nodesCanvas.SetCanvasScrollPosition = new Point(positionX, positionY);
        }
        #endregion

        #region Handles drawing for ToolTip
        private void toolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            if (((ToolTip)sender).Tag != null) {
                TipPainter.DrawSizeMessage(e);
            } else {
                TipPainter.DrawMessage(e);
            }
        }

        private void msgPopup_Popup(object sender, PopupEventArgs e) {
            Size sz = TextRenderer.MeasureText((string)msgPopup.Tag, new Font("Arial", 12.0f, FontStyle.Regular, GraphicsUnit.Point));;
            sz.Height += 6;
            e.ToolTipSize = sz;
        }
        #endregion
    }
}
