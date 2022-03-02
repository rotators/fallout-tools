namespace ScriptEditor {
    partial class MessageEditor {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageEditor));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sendLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.playerMarkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.addDescriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.dgvMessage = new ScriptEditor.TextEditorUI.DataGridViewEx();
            this.cLine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.NewStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.msgOpenButton = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.msgSaveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.SaveAsStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.SendStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.MoveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.IncAddStripButton = new System.Windows.Forms.ToolStripButton();
            this.InsertEmptyStripButton = new System.Windows.Forms.ToolStripButton();
            this.InsertCommentStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.DeleteLineStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.StripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.OpenNotepadtoolStripButton = new System.Windows.Forms.ToolStripSplitButton();
            this.openAsTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.alwaysOnTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.HighlightingCommToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ColorComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.showLIPColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.fontSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FontSizeComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.encodingTextDOSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NextStripButton = new System.Windows.Forms.ToolStripButton();
            this.BackStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessage)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendLineToolStripMenuItem,
            this.toolStripSeparator12,
            this.playerMarkerToolStripMenuItem,
            this.toolStripSeparator11,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.toolStripSeparator14,
            this.addDescriptionToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(188, 132);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // sendLineToolStripMenuItem
            // 
            this.sendLineToolStripMenuItem.Name = "sendLineToolStripMenuItem";
            this.sendLineToolStripMenuItem.ShortcutKeyDisplayString = "MMB / Alt+S";
            this.sendLineToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.sendLineToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.sendLineToolStripMenuItem.Text = "Send Line";
            this.sendLineToolStripMenuItem.ToolTipText = "Send current line number to an open script.";
            this.sendLineToolStripMenuItem.Click += new System.EventHandler(this.SendStripButton_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(184, 6);
            // 
            // playerMarkerToolStripMenuItem
            // 
            this.playerMarkerToolStripMenuItem.Name = "playerMarkerToolStripMenuItem";
            this.playerMarkerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.O)));
            this.playerMarkerToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.playerMarkerToolStripMenuItem.Text = "Set Player Marker";
            this.playerMarkerToolStripMenuItem.ToolTipText = "Set player options marker";
            this.playerMarkerToolStripMenuItem.Click += new System.EventHandler(this.playerMarkerToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(184, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(184, 6);
            // 
            // addDescriptionToolStripMenuItem
            // 
            this.addDescriptionToolStripMenuItem.Name = "addDescriptionToolStripMenuItem";
            this.addDescriptionToolStripMenuItem.ShowShortcutKeys = false;
            this.addDescriptionToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.addDescriptionToolStripMenuItem.Text = "Add/Edit comment for line";
            this.addDescriptionToolStripMenuItem.Click += new System.EventHandler(this.addDescriptionToolStripMenuItem_Click);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.dgvMessage);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Location = new System.Drawing.Point(0, 26);
            this.groupBox.Name = "groupBox";
            this.groupBox.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox.Size = new System.Drawing.Size(941, 562);
            this.groupBox.TabIndex = 1;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Messages";
            // 
            // dgvMessage
            // 
            this.dgvMessage.AllowUserToAddRows = false;
            this.dgvMessage.AllowUserToOrderColumns = true;
            this.dgvMessage.AllowUserToResizeRows = false;
            this.dgvMessage.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.dgvMessage.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvMessage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvMessage.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            this.dgvMessage.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvMessage.ColumnHeadersHeight = 22;
            this.dgvMessage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvMessage.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cLine,
            this.cDescription,
            this.cLip});
            this.dgvMessage.ContextMenuStrip = this.contextMenuStrip1;
            this.dgvMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMessage.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvMessage.EnableHeadersVisualStyles = false;
            this.dgvMessage.GridColor = System.Drawing.Color.Silver;
            this.dgvMessage.Location = new System.Drawing.Point(4, 17);
            this.dgvMessage.MultiSelect = false;
            this.dgvMessage.Name = "dgvMessage";
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dgvMessage.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvMessage.RowHeadersVisible = false;
            this.dgvMessage.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvMessage.RowTemplate.Height = 18;
            this.dgvMessage.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMessage.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvMessage.ShowCellErrors = false;
            this.dgvMessage.ShowEditingIcon = false;
            this.dgvMessage.ShowRowErrors = false;
            this.dgvMessage.Size = new System.Drawing.Size(933, 541);
            this.dgvMessage.TabIndex = 0;
            this.dgvMessage.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvMessage_CellBeginEdit);
            this.dgvMessage.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMessage_CellClick);
            this.dgvMessage.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMessage_CellDoubleClick);
            this.dgvMessage.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMessage_CellEndEdit);
            this.dgvMessage.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvMessage_CellMouseClick);
            this.dgvMessage.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMessage_CellValueChanged);
            this.dgvMessage.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvMessage_CellValueNeeded);
            this.dgvMessage.SelectionChanged += new System.EventHandler(this.dgvMessage_SelectionChanged);
            this.dgvMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvMessage_KeyDown);
            this.dgvMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvMessage_KeyPress);
            // 
            // cLine
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Chocolate;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.SteelBlue;
            this.cLine.DefaultCellStyle = dataGridViewCellStyle2;
            this.cLine.HeaderText = "Line";
            this.cLine.Name = "cLine";
            this.cLine.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cLine.ToolTipText = "Msg line number";
            this.cLine.Width = 50;
            // 
            // cDescription
            // 
            this.cDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.cDescription.DefaultCellStyle = dataGridViewCellStyle3;
            this.cDescription.HeaderText = "Message or comment text";
            this.cDescription.Name = "cDescription";
            this.cDescription.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cDescription.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cLip
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.SteelBlue;
            this.cLip.DefaultCellStyle = dataGridViewCellStyle4;
            this.cLip.HeaderText = "Lip File";
            this.cLip.Name = "cLip";
            this.cLip.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cLip.Width = 40;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewStripButton,
            this.toolStripSeparator7,
            this.msgOpenButton,
            this.toolStripSeparator16,
            this.msgSaveButton,
            this.toolStripSeparator8,
            this.SaveAsStripButton,
            this.toolStripSeparator1,
            this.SendStripButton,
            this.toolStripSeparator9,
            this.MoveToolStripButton,
            this.toolStripSeparator4,
            this.IncAddStripButton,
            this.InsertEmptyStripButton,
            this.InsertCommentStripButton,
            this.toolStripSeparator5,
            this.DeleteLineStripButton,
            this.toolStripSeparator2,
            this.StripComboBox,
            this.toolStripSeparator10,
            this.OpenNotepadtoolStripButton,
            this.toolStripSeparator15,
            this.toolStripDropDownButton1,
            this.SearchStripTextBox,
            this.toolStripDropDownButton2,
            this.NextStripButton,
            this.BackStripButton,
            this.toolStripSeparator3});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(941, 26);
            this.toolStrip.TabIndex = 7;
            // 
            // NewStripButton
            // 
            this.NewStripButton.AutoSize = false;
            this.NewStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NewStripButton.Image = ((System.Drawing.Image)(resources.GetObject("NewStripButton.Image")));
            this.NewStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NewStripButton.Name = "NewStripButton";
            this.NewStripButton.Size = new System.Drawing.Size(25, 23);
            this.NewStripButton.ToolTipText = "Clear & New";
            this.NewStripButton.Click += new System.EventHandler(this.NewStripButton_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.AutoSize = false;
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(10, 26);
            // 
            // msgOpenButton
            // 
            this.msgOpenButton.Image = ((System.Drawing.Image)(resources.GetObject("msgOpenButton.Image")));
            this.msgOpenButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.msgOpenButton.Name = "msgOpenButton";
            this.msgOpenButton.Size = new System.Drawing.Size(69, 23);
            this.msgOpenButton.Text = "Open";
            this.msgOpenButton.ToolTipText = "Open message file";
            this.msgOpenButton.ButtonClick += new System.EventHandler(this.msgOpenButton_ButtonClick);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(6, 26);
            // 
            // msgSaveButton
            // 
            this.msgSaveButton.Enabled = false;
            this.msgSaveButton.Image = ((System.Drawing.Image)(resources.GetObject("msgSaveButton.Image")));
            this.msgSaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.msgSaveButton.Name = "msgSaveButton";
            this.msgSaveButton.Size = new System.Drawing.Size(53, 23);
            this.msgSaveButton.Text = "Save";
            this.msgSaveButton.ToolTipText = "Save messageg file. [Ctrl+S]";
            this.msgSaveButton.Click += new System.EventHandler(this.msgSaveButton_ButtonClick);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.AutoSize = false;
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(10, 26);
            // 
            // SaveAsStripButton
            // 
            this.SaveAsStripButton.AutoSize = false;
            this.SaveAsStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveAsStripButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveAsStripButton.Image")));
            this.SaveAsStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveAsStripButton.Name = "SaveAsStripButton";
            this.SaveAsStripButton.Size = new System.Drawing.Size(25, 23);
            this.SaveAsStripButton.Text = "Save As...";
            this.SaveAsStripButton.Click += new System.EventHandler(this.SaveAsStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AutoSize = false;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(10, 25);
            // 
            // SendStripButton
            // 
            this.SendStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SendStripButton.Image = ((System.Drawing.Image)(resources.GetObject("SendStripButton.Image")));
            this.SendStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SendStripButton.Name = "SendStripButton";
            this.SendStripButton.Size = new System.Drawing.Size(23, 23);
            this.SendStripButton.ToolTipText = "Send current line number to an open script [Alt+S]";
            this.SendStripButton.Click += new System.EventHandler(this.SendStripButton_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.AutoSize = false;
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(8, 26);
            // 
            // MoveToolStripButton
            // 
            this.MoveToolStripButton.CheckOnClick = true;
            this.MoveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MoveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("MoveToolStripButton.Image")));
            this.MoveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MoveToolStripButton.Name = "MoveToolStripButton";
            this.MoveToolStripButton.Size = new System.Drawing.Size(23, 23);
            this.MoveToolStripButton.ToolTipText = "Enable multi-selective mode for moving and deleting rows.";
            this.MoveToolStripButton.Click += new System.EventHandler(this.MoveToolStripButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.AutoSize = false;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(10, 26);
            // 
            // IncAddStripButton
            // 
            this.IncAddStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.IncAddStripButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.IncAddStripButton.Image = ((System.Drawing.Image)(resources.GetObject("IncAddStripButton.Image")));
            this.IncAddStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.IncAddStripButton.Name = "IncAddStripButton";
            this.IncAddStripButton.Size = new System.Drawing.Size(23, 23);
            this.IncAddStripButton.ToolTipText = "Add next number line [Alt+A or Enter]";
            this.IncAddStripButton.Click += new System.EventHandler(this.IncAddStripButton_Click);
            // 
            // InsertEmptyStripButton
            // 
            this.InsertEmptyStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.InsertEmptyStripButton.Image = ((System.Drawing.Image)(resources.GetObject("InsertEmptyStripButton.Image")));
            this.InsertEmptyStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.InsertEmptyStripButton.Name = "InsertEmptyStripButton";
            this.InsertEmptyStripButton.Size = new System.Drawing.Size(23, 23);
            this.InsertEmptyStripButton.ToolTipText = "Insert an empty line below the current line. [Ctrl+Enter] \r\nor [Shift+Enter] - Insert an empty line above the current line.";
            this.InsertEmptyStripButton.Click += new System.EventHandler(this.InsertEmptyStripButton_Click);
            // 
            // InsertCommentStripButton
            // 
            this.InsertCommentStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.InsertCommentStripButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.InsertCommentStripButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.InsertCommentStripButton.Image = ((System.Drawing.Image)(resources.GetObject("InsertCommentStripButton.Image")));
            this.InsertCommentStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.InsertCommentStripButton.Name = "InsertCommentStripButton";
            this.InsertCommentStripButton.Size = new System.Drawing.Size(26, 23);
            this.InsertCommentStripButton.Text = "#";
            this.InsertCommentStripButton.ToolTipText = "Insert comment line. [Alt+C]";
            this.InsertCommentStripButton.Click += new System.EventHandler(this.InsertCommentStripButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.AutoSize = false;
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(10, 26);
            // 
            // DeleteLineStripButton
            // 
            this.DeleteLineStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.DeleteLineStripButton.Image = ((System.Drawing.Image)(resources.GetObject("DeleteLineStripButton.Image")));
            this.DeleteLineStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DeleteLineStripButton.Name = "DeleteLineStripButton";
            this.DeleteLineStripButton.Size = new System.Drawing.Size(23, 23);
            this.DeleteLineStripButton.ToolTipText = "Delete current line [Ctrl+Delete]";
            this.DeleteLineStripButton.Click += new System.EventHandler(this.DeleteLineStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AutoSize = false;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(10, 25);
            // 
            // StripComboBox
            // 
            this.StripComboBox.AutoSize = false;
            this.StripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StripComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.StripComboBox.Items.AddRange(new object[] {
            "1",
            "5",
            "10",
            "20",
            "30",
            "40",
            "50"});
            this.StripComboBox.Name = "StripComboBox";
            this.StripComboBox.Size = new System.Drawing.Size(40, 22);
            this.StripComboBox.ToolTipText = "The line number after the comment is increased by this number.";
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.AutoSize = false;
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(10, 26);
            // 
            // OpenNotepadtoolStripButton
            // 
            this.OpenNotepadtoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.OpenNotepadtoolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openAsTextToolStripMenuItem});
            this.OpenNotepadtoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("OpenNotepadtoolStripButton.Image")));
            this.OpenNotepadtoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.OpenNotepadtoolStripButton.Name = "OpenNotepadtoolStripButton";
            this.OpenNotepadtoolStripButton.Size = new System.Drawing.Size(32, 23);
            this.OpenNotepadtoolStripButton.ToolTipText = "Open this message file in external editor.";
            this.OpenNotepadtoolStripButton.ButtonClick += new System.EventHandler(this.OpenNotepadtoolStripButton_Click);
            // 
            // openAsTextToolStripMenuItem
            // 
            this.openAsTextToolStripMenuItem.Name = "openAsTextToolStripMenuItem";
            this.openAsTextToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.openAsTextToolStripMenuItem.Text = "Open as Text";
            this.openAsTextToolStripMenuItem.ToolTipText = "Closes the message editor window and opens the file for editing in text format.";
            this.openAsTextToolStripMenuItem.Click += new System.EventHandler(this.openAsTextToolStripMenuItem_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.AutoSize = false;
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(8, 26);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alwaysOnTopToolStripMenuItem,
            this.toolStripSeparator13,
            this.HighlightingCommToolStripMenuItem,
            this.showLIPColumnToolStripMenuItem,
            this.toolStripSeparator6,
            this.fontSizeToolStripMenuItem,
            this.encodingTextDOSToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(62, 23);
            this.toolStripDropDownButton1.Text = "Options";
            // 
            // alwaysOnTopToolStripMenuItem
            // 
            this.alwaysOnTopToolStripMenuItem.CheckOnClick = true;
            this.alwaysOnTopToolStripMenuItem.Name = "alwaysOnTopToolStripMenuItem";
            this.alwaysOnTopToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.alwaysOnTopToolStripMenuItem.Text = "Window On Top";
            this.alwaysOnTopToolStripMenuItem.CheckedChanged += new System.EventHandler(this.alwaysOnTopToolStripMenuItem_CheckedChanged);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(179, 6);
            // 
            // HighlightingCommToolStripMenuItem
            // 
            this.HighlightingCommToolStripMenuItem.CheckOnClick = true;
            this.HighlightingCommToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ColorComboBox});
            this.HighlightingCommToolStripMenuItem.Name = "HighlightingCommToolStripMenuItem";
            this.HighlightingCommToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.HighlightingCommToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.HighlightingCommToolStripMenuItem.Text = "Highlighting";
            this.HighlightingCommToolStripMenuItem.ToolTipText = "Highlighting the color of the comment lines.";
            this.HighlightingCommToolStripMenuItem.Click += new System.EventHandler(this.HighlightingCheck);
            // 
            // ColorComboBox
            // 
            this.ColorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ColorComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.ColorComboBox.Items.AddRange(new object[] {
            "LightYellow",
            "PaleGreen",
            "Lavender"});
            this.ColorComboBox.Name = "ColorComboBox";
            this.ColorComboBox.Size = new System.Drawing.Size(75, 22);
            this.ColorComboBox.SelectedIndexChanged += new System.EventHandler(this.ColorComboBox_SelectedIndexChanged);
            // 
            // showLIPColumnToolStripMenuItem
            // 
            this.showLIPColumnToolStripMenuItem.CheckOnClick = true;
            this.showLIPColumnToolStripMenuItem.Name = "showLIPColumnToolStripMenuItem";
            this.showLIPColumnToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.showLIPColumnToolStripMenuItem.Text = "Show LIP Column";
            this.showLIPColumnToolStripMenuItem.Click += new System.EventHandler(this.showLIPColumnToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(179, 6);
            // 
            // fontSizeToolStripMenuItem
            // 
            this.fontSizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FontSizeComboBox});
            this.fontSizeToolStripMenuItem.Name = "fontSizeToolStripMenuItem";
            this.fontSizeToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.fontSizeToolStripMenuItem.Text = "Font Size";
            // 
            // FontSizeComboBox
            // 
            this.FontSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FontSizeComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.FontSizeComboBox.Items.AddRange(new object[] {
            "9",
            "10",
            "12",
            "14",
            "16",
            "18",
            "20",
            "24"});
            this.FontSizeComboBox.Name = "FontSizeComboBox";
            this.FontSizeComboBox.Size = new System.Drawing.Size(75, 22);
            this.FontSizeComboBox.ToolTipText = "Hotkey change size [Ctlr + NumPlus] and [Ctlr + NumMinus]";
            // 
            // encodingTextDOSToolStripMenuItem
            // 
            this.encodingTextDOSToolStripMenuItem.CheckOnClick = true;
            this.encodingTextDOSToolStripMenuItem.Name = "encodingTextDOSToolStripMenuItem";
            this.encodingTextDOSToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.encodingTextDOSToolStripMenuItem.Text = "Encoding: OEM 866";
            this.encodingTextDOSToolStripMenuItem.ToolTipText = "Read and write Msg files in cyrillic encoding OEM 866.";
            this.encodingTextDOSToolStripMenuItem.Click += new System.EventHandler(this.encodingTextDOSToolStripMenuItem_Click);
            // 
            // SearchStripTextBox
            // 
            this.SearchStripTextBox.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.SearchStripTextBox.AutoSize = false;
            this.SearchStripTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.SearchStripTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SearchStripTextBox.Name = "SearchStripTextBox";
            this.SearchStripTextBox.Size = new System.Drawing.Size(300, 22);
            this.SearchStripTextBox.ToolTipText = "Search text";
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.comentToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.findFToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(29, 23);
            this.toolStripDropDownButton2.Text = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Visible = false;
            this.toolStripDropDownButton2.Click += new System.EventHandler(this.SendStripButton_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.addToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.addToolStripMenuItem.Text = "add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.IncAddStripButton_Click);
            // 
            // comentToolStripMenuItem
            // 
            this.comentToolStripMenuItem.Name = "comentToolStripMenuItem";
            this.comentToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.comentToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.comentToolStripMenuItem.Text = "coment";
            this.comentToolStripMenuItem.Click += new System.EventHandler(this.InsertCommentStripButton_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.saveToolStripMenuItem.Text = "save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.msgSaveButton_ButtonClick);
            // 
            // findFToolStripMenuItem
            // 
            this.findFToolStripMenuItem.Name = "findFToolStripMenuItem";
            this.findFToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findFToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.findFToolStripMenuItem.Text = "findF";
            this.findFToolStripMenuItem.Click += new System.EventHandler(this.Downbutton_Click);
            // 
            // NextStripButton
            // 
            this.NextStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.NextStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NextStripButton.Image = ((System.Drawing.Image)(resources.GetObject("NextStripButton.Image")));
            this.NextStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NextStripButton.Name = "NextStripButton";
            this.NextStripButton.Size = new System.Drawing.Size(23, 23);
            this.NextStripButton.ToolTipText = "Next find [F3]";
            this.NextStripButton.Click += new System.EventHandler(this.Downbutton_Click);
            // 
            // BackStripButton
            // 
            this.BackStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.BackStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BackStripButton.Image = ((System.Drawing.Image)(resources.GetObject("BackStripButton.Image")));
            this.BackStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BackStripButton.Name = "BackStripButton";
            this.BackStripButton.Size = new System.Drawing.Size(23, 23);
            this.BackStripButton.ToolTipText = "Back find";
            this.BackStripButton.Click += new System.EventHandler(this.Upbutton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 26);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "msg";
            this.openFileDialog.Filter = "Message files|*.msg";
            this.openFileDialog.InitialDirectory = "D:\\";
            this.openFileDialog.RestoreDirectory = true;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Message files|*.msg";
            this.saveFileDialog.RestoreDirectory = true;
            // 
            // MessageEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(941, 588);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.toolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(250, 250);
            this.Name = "MessageEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = " - Message Editor";
            this.Text = " - Message Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MessageEditor_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MessageEditor_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MessageEditor_KeyUp);
            this.Resize += new System.EventHandler(this.MessageEditor_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessage)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextEditorUI.DataGridViewEx dgvMessage;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton IncAddStripButton;
        private System.Windows.Forms.ToolStripButton InsertCommentStripButton;
        private System.Windows.Forms.ToolStripButton InsertEmptyStripButton;
        private System.Windows.Forms.ToolStripButton SendStripButton;
        private System.Windows.Forms.ToolStripButton DeleteLineStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton NextStripButton;
        private System.Windows.Forms.ToolStripButton BackStripButton;
        private System.Windows.Forms.ToolStripTextBox SearchStripTextBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem showLIPColumnToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton msgSaveButton;
        private System.Windows.Forms.ToolStripButton SaveAsStripButton;
        private System.Windows.Forms.ToolStripButton NewStripButton;
        private System.Windows.Forms.ToolStripSplitButton msgOpenButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripComboBox StripComboBox;
        private System.Windows.Forms.ToolStripMenuItem encodingTextDOSToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton MoveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem playerMarkerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alwaysOnTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HighlightingCommToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox FontSizeComboBox;
        private System.Windows.Forms.ToolStripComboBox ColorComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripMenuItem addDescriptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem comentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem findFToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton OpenNotepadtoolStripButton;
        private System.Windows.Forms.ToolStripMenuItem openAsTextToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLine;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLip;
    }
}