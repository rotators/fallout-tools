namespace ScriptEditor.TextEditorUI.Nodes
{
    partial class FlowchartTE : System.Windows.Forms.UserControl
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlowchartTE));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.ApplytoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.UndotoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.RedotoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbPasteNode = new System.Windows.Forms.ToolStripButton();
            this.cmbNodesName = new System.Windows.Forms.ToolStripComboBox();
            this.tsbGotoNode = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.onToptoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.insertMsgLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.pasteOpcodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.replyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.messagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.nOptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lowOptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.mstrToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gmstrToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toCustomCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.dgvMessages = new System.Windows.Forms.DataGridView();
            this.LineColumn = new System.Windows.Forms.DataGridViewLinkColumn();
            this.MsgColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.callToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessages)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ApplytoolStripButton,
            this.toolStripSeparator1,
            this.UndotoolStripButton,
            this.RedotoolStripButton,
            this.toolStripSeparator2,
            this.tsbPasteNode,
            this.cmbNodesName,
            this.tsbGotoNode,
            this.toolStripSeparator9,
            this.onToptoolStripButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(831, 25);
            this.toolStrip.TabIndex = 6;
            this.toolStrip.Text = "toolStrip";
            // 
            // ApplytoolStripButton
            // 
            this.ApplytoolStripButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ApplytoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("ApplytoolStripButton.Image")));
            this.ApplytoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ApplytoolStripButton.Name = "ApplytoolStripButton";
            this.ApplytoolStripButton.Size = new System.Drawing.Size(56, 22);
            this.ApplytoolStripButton.Text = "Done";
            this.ApplytoolStripButton.ToolTipText = "Apply code changes and close the window.";
            this.ApplytoolStripButton.Click += new System.EventHandler(this.ApplytoolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AutoSize = false;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(8, 25);
            // 
            // UndotoolStripButton
            // 
            this.UndotoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("UndotoolStripButton.Image")));
            this.UndotoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.UndotoolStripButton.Name = "UndotoolStripButton";
            this.UndotoolStripButton.Size = new System.Drawing.Size(52, 22);
            this.UndotoolStripButton.Text = "Undo";
            this.UndotoolStripButton.ToolTipText = "Undo [Ctrl+Z]";
            this.UndotoolStripButton.Click += new System.EventHandler(this.UndotoolStripButton_Click);
            // 
            // RedotoolStripButton
            // 
            this.RedotoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("RedotoolStripButton.Image")));
            this.RedotoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RedotoolStripButton.Name = "RedotoolStripButton";
            this.RedotoolStripButton.Size = new System.Drawing.Size(52, 22);
            this.RedotoolStripButton.Text = "Redo";
            this.RedotoolStripButton.ToolTipText = "Redo [Ctrl+Y]";
            this.RedotoolStripButton.Click += new System.EventHandler(this.RedotoolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AutoSize = false;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(8, 25);
            // 
            // tsbPasteNode
            // 
            this.tsbPasteNode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPasteNode.Image = ((System.Drawing.Image)(resources.GetObject("tsbPasteNode.Image")));
            this.tsbPasteNode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPasteNode.Name = "tsbPasteNode";
            this.tsbPasteNode.Size = new System.Drawing.Size(23, 22);
            this.tsbPasteNode.ToolTipText = "Paste select name Node to code";
            this.tsbPasteNode.Click += new System.EventHandler(this.tsbPasteNode_Click);
            // 
            // cmbNodesName
            // 
            this.cmbNodesName.AutoSize = false;
            this.cmbNodesName.BackColor = System.Drawing.SystemColors.Info;
            this.cmbNodesName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNodesName.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.cmbNodesName.Items.AddRange(new object[] {
            "<Unselect Node>"});
            this.cmbNodesName.MaxDropDownItems = 40;
            this.cmbNodesName.Name = "cmbNodesName";
            this.cmbNodesName.Size = new System.Drawing.Size(150, 21);
            this.cmbNodesName.Sorted = true;
            this.cmbNodesName.DropDownClosed += new System.EventHandler(this.cmbNodesName_DropDownClosed);
            // 
            // tsbGotoNode
            // 
            this.tsbGotoNode.Image = ((System.Drawing.Image)(resources.GetObject("tsbGotoNode.Image")));
            this.tsbGotoNode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGotoNode.Name = "tsbGotoNode";
            this.tsbGotoNode.Size = new System.Drawing.Size(40, 22);
            this.tsbGotoNode.Text = "Go";
            this.tsbGotoNode.ToolTipText = "Goto to select Node";
            this.tsbGotoNode.Click += new System.EventHandler(this.tsbGotoNode_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            // 
            // onToptoolStripButton
            // 
            this.onToptoolStripButton.CheckOnClick = true;
            this.onToptoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.onToptoolStripButton.Image = global::ScriptEditor.Properties.Resources.KeepWindowOff;
            this.onToptoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.onToptoolStripButton.Name = "onToptoolStripButton";
            this.onToptoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.onToptoolStripButton.Text = "Window On Top";
            this.onToptoolStripButton.Click += new System.EventHandler(this.onToptoolStripButton_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertMsgLineToolStripMenuItem,
            this.toolStripSeparator7,
            this.pasteOpcodeToolStripMenuItem,
            this.toCustomCodeToolStripMenuItem,
            this.toolStripSeparator4,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator3,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(162, 198);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // insertMsgLineToolStripMenuItem
            // 
            this.insertMsgLineToolStripMenuItem.Name = "insertMsgLineToolStripMenuItem";
            this.insertMsgLineToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.insertMsgLineToolStripMenuItem.Text = "Insert message";
            this.insertMsgLineToolStripMenuItem.Click += new System.EventHandler(this.insertMsgLineToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(158, 6);
            // 
            // pasteOpcodeToolStripMenuItem
            // 
            this.pasteOpcodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customToolStripMenuItem,
            this.toolStripSeparator5,
            this.replyToolStripMenuItem,
            this.messagesToolStripMenuItem,
            this.toolStripSeparator6,
            this.nOptionToolStripMenuItem,
            this.lowOptionToolStripMenuItem,
            this.toolStripSeparator8,
            this.mstrToolStripMenuItem,
            this.gmstrToolStripMenuItem,
            this.toolStripSeparator10,
            this.callToolStripMenuItem});
            this.pasteOpcodeToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pasteOpcodeToolStripMenuItem.Name = "pasteOpcodeToolStripMenuItem";
            this.pasteOpcodeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.pasteOpcodeToolStripMenuItem.Text = "Quick Macros";
            // 
            // customToolStripMenuItem
            // 
            this.customToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.customToolStripMenuItem.Name = "customToolStripMenuItem";
            this.customToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.customToolStripMenuItem.Text = "Custom";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(149, 6);
            // 
            // replyToolStripMenuItem
            // 
            this.replyToolStripMenuItem.Name = "replyToolStripMenuItem";
            this.replyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.replyToolStripMenuItem.Tag = "Reply({msg_num});";
            this.replyToolStripMenuItem.Text = "Reply";
            this.replyToolStripMenuItem.Click += new System.EventHandler(this.pasteOpcodeToolStripMenuItem_Click);
            // 
            // messagesToolStripMenuItem
            // 
            this.messagesToolStripMenuItem.Name = "messagesToolStripMenuItem";
            this.messagesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.messagesToolStripMenuItem.Tag = "NMessage({msg_num});";
            this.messagesToolStripMenuItem.Text = "Messages";
            this.messagesToolStripMenuItem.Click += new System.EventHandler(this.pasteOpcodeToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(149, 6);
            // 
            // nOptionToolStripMenuItem
            // 
            this.nOptionToolStripMenuItem.Name = "nOptionToolStripMenuItem";
            this.nOptionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.nOptionToolStripMenuItem.Tag = "NOption({msg_num}, {node}, {iq});";
            this.nOptionToolStripMenuItem.Text = "Option";
            this.nOptionToolStripMenuItem.Click += new System.EventHandler(this.pasteOpcodeToolStripMenuItem_Click);
            // 
            // lowOptionToolStripMenuItem
            // 
            this.lowOptionToolStripMenuItem.Name = "lowOptionToolStripMenuItem";
            this.lowOptionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.lowOptionToolStripMenuItem.Tag = "NLowOption({msg_num},  {node});";
            this.lowOptionToolStripMenuItem.Text = "LowOption";
            this.lowOptionToolStripMenuItem.Click += new System.EventHandler(this.pasteOpcodeToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(149, 6);
            // 
            // mstrToolStripMenuItem
            // 
            this.mstrToolStripMenuItem.Name = "mstrToolStripMenuItem";
            this.mstrToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mstrToolStripMenuItem.Tag = "mstr({msg_num})";
            this.mstrToolStripMenuItem.Text = "mstr";
            this.mstrToolStripMenuItem.Click += new System.EventHandler(this.pasteOpcodeToolStripMenuItem_Click);
            // 
            // gmstrToolStripMenuItem
            // 
            this.gmstrToolStripMenuItem.Name = "gmstrToolStripMenuItem";
            this.gmstrToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.gmstrToolStripMenuItem.Tag = "g_mstr({msg_num})";
            this.gmstrToolStripMenuItem.Text = "g_mstr";
            this.gmstrToolStripMenuItem.Click += new System.EventHandler(this.pasteOpcodeToolStripMenuItem_Click);
            // 
            // toCustomCodeToolStripMenuItem
            // 
            this.toCustomCodeToolStripMenuItem.Name = "toCustomCodeToolStripMenuItem";
            this.toCustomCodeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.toCustomCodeToolStripMenuItem.Text = "Store to custom";
            this.toCustomCodeToolStripMenuItem.Click += new System.EventHandler(this.toCustomCodeToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(158, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+X";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+V";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(158, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 25);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.ContextMenuStrip = this.contextMenuStrip;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.dgvMessages);
            this.splitContainer.Size = new System.Drawing.Size(831, 584);
            this.splitContainer.SplitterDistance = 434;
            this.splitContainer.TabIndex = 7;
            // 
            // dgvMessages
            // 
            this.dgvMessages.AllowUserToAddRows = false;
            this.dgvMessages.AllowUserToDeleteRows = false;
            this.dgvMessages.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.dgvMessages.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvMessages.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvMessages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMessages.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LineColumn,
            this.MsgColumn,
            this.FileColumn});
            this.dgvMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMessages.Location = new System.Drawing.Point(0, 0);
            this.dgvMessages.MultiSelect = false;
            this.dgvMessages.Name = "dgvMessages";
            this.dgvMessages.RowHeadersWidth = 25;
            this.dgvMessages.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvMessages.RowTemplate.Height = 20;
            this.dgvMessages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvMessages.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMessages.Size = new System.Drawing.Size(831, 146);
            this.dgvMessages.TabIndex = 0;
            this.dgvMessages.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMessages_CellContentClick);
            this.dgvMessages.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvMessages_CellMouseClick);
            this.dgvMessages.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMessages_CellValueChanged);
            // 
            // LineColumn
            // 
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LemonChiffon;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.DodgerBlue;
            this.LineColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.LineColumn.HeaderText = "Line";
            this.LineColumn.LinkColor = System.Drawing.Color.DodgerBlue;
            this.LineColumn.Name = "LineColumn";
            this.LineColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.LineColumn.TrackVisitedState = false;
            this.LineColumn.Width = 50;
            // 
            // MsgColumn
            // 
            this.MsgColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LemonChiffon;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.MsgColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.MsgColumn.HeaderText = "Message Text";
            this.MsgColumn.Name = "MsgColumn";
            this.MsgColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.MsgColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FileColumn
            // 
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LemonChiffon;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Maroon;
            this.FileColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.FileColumn.HeaderText = "Msg File";
            this.FileColumn.Name = "FileColumn";
            this.FileColumn.ReadOnly = true;
            this.FileColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.FileColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.FileColumn.Width = 80;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(149, 6);
            // 
            // callToolStripMenuItem
            // 
            this.callToolStripMenuItem.Name = "callToolStripMenuItem";
            this.callToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.callToolStripMenuItem.Tag = "call {node};";
            this.callToolStripMenuItem.Text = "Call node";
            this.callToolStripMenuItem.Click += new System.EventHandler(this.pasteOpcodeToolStripMenuItem_Click);
            // 
            // FlowchartTE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.toolStrip);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "FlowchartTE";
            this.Size = new System.Drawing.Size(831, 609);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessages)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton ApplytoolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton UndotoolStripButton;
        private System.Windows.Forms.ToolStripButton RedotoolStripButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton onToptoolStripButton;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.DataGridView dgvMessages;
        private System.Windows.Forms.ToolStripMenuItem pasteOpcodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem replyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem messagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem nOptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lowOptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toCustomCodeToolStripMenuItem;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripMenuItem insertMsgLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem mstrToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gmstrToolStripMenuItem;
        private System.Windows.Forms.DataGridViewLinkColumn LineColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MsgColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileColumn;
        private System.Windows.Forms.ToolStripButton tsbPasteNode;
        private System.Windows.Forms.ToolStripComboBox cmbNodesName;
        private System.Windows.Forms.ToolStripButton tsbGotoNode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem callToolStripMenuItem;

    }
}
