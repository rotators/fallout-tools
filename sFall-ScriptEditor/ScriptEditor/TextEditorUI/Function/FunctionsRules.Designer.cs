namespace ScriptEditor.TextEditorUI.Function {
    partial class FunctionsRules {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvTemplates = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label = new System.Windows.Forms.Label();
            this.bHelp = new System.Windows.Forms.Button();
            this.typeColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.functionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.argsColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MsgColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gotoColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iqColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTemplates)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvTemplates
            // 
            this.dgvTemplates.AllowUserToDeleteRows = false;
            this.dgvTemplates.AllowUserToResizeRows = false;
            this.dgvTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTemplates.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvTemplates.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvTemplates.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgvTemplates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTemplates.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.typeColumn,
            this.functionColumn,
            this.argsColumn,
            this.MsgColumn,
            this.gotoColumn,
            this.iqColumn,
            this.FileColumn});
            this.dgvTemplates.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTemplates.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvTemplates.Location = new System.Drawing.Point(12, 12);
            this.dgvTemplates.MultiSelect = false;
            this.dgvTemplates.Name = "dgvTemplates";
            this.dgvTemplates.RowHeadersVisible = false;
            this.dgvTemplates.RowHeadersWidth = 30;
            this.dgvTemplates.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvTemplates.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTemplates.Size = new System.Drawing.Size(438, 198);
            this.dgvTemplates.TabIndex = 2;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(86, 26);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(85, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // label
            // 
            this.label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label.AutoSize = true;
            this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label.Location = new System.Drawing.Point(9, 213);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(399, 15);
            this.label.TabIndex = 3;
            this.label.Tag = "Указание неверных данных для функции выведет программу из строя.";
            this.label.Text = "Note: Specifying incorrect data for the function will crashing the program.";
            // 
            // bHelp
            // 
            this.bHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bHelp.ForeColor = System.Drawing.Color.RoyalBlue;
            this.bHelp.Location = new System.Drawing.Point(430, 212);
            this.bHelp.Name = "bHelp";
            this.bHelp.Size = new System.Drawing.Size(20, 20);
            this.bHelp.TabIndex = 4;
            this.bHelp.Text = "?";
            this.bHelp.UseVisualStyleBackColor = true;
            this.bHelp.Click += new System.EventHandler(this.bHelp_Click);
            // 
            // typeColumn
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.typeColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.typeColumn.DropDownWidth = 80;
            this.typeColumn.HeaderText = "Type";
            this.typeColumn.Items.AddRange(new object[] {
            "Option",
            "Reply",
            "Message",
            "Call"});
            this.typeColumn.Name = "typeColumn";
            this.typeColumn.ToolTipText = "Type of function.";
            this.typeColumn.Width = 80;
            // 
            // functionColumn
            // 
            this.functionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.functionColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.functionColumn.FillWeight = 50F;
            this.functionColumn.HeaderText = "Function Name";
            this.functionColumn.MinimumWidth = 120;
            this.functionColumn.Name = "functionColumn";
            this.functionColumn.ToolTipText = "The name of the custom macro.";
            // 
            // argsColumn
            // 
            dataGridViewCellStyle3.NullValue = "0";
            this.argsColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.argsColumn.HeaderText = "Args";
            this.argsColumn.Name = "argsColumn";
            this.argsColumn.ToolTipText = "The total number of arguments in function.";
            this.argsColumn.Width = 35;
            // 
            // MsgColumn
            // 
            dataGridViewCellStyle4.NullValue = "0";
            this.MsgColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.MsgColumn.HeaderText = "Message";
            this.MsgColumn.Name = "MsgColumn";
            this.MsgColumn.ToolTipText = "A position indicating an argument in a function with a message number string.";
            this.MsgColumn.Width = 55;
            // 
            // gotoColumn
            // 
            dataGridViewCellStyle5.NullValue = "0";
            this.gotoColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.gotoColumn.HeaderText = "Node";
            this.gotoColumn.Name = "gotoColumn";
            this.gotoColumn.ToolTipText = "A position indicating an argument in a function with a node procedure. Set to 0 - if not used.";
            this.gotoColumn.Width = 35;
            // 
            // iqColumn
            // 
            dataGridViewCellStyle6.NullValue = "0";
            this.iqColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.iqColumn.HeaderText = "IQ";
            this.iqColumn.Name = "iqColumn";
            this.iqColumn.ToolTipText = "A position indicating an argument in a function with IQ. Set to 0 - if not used.";
            this.iqColumn.Width = 35;
            // 
            // FileColumn
            // 
            dataGridViewCellStyle7.NullValue = "0";
            this.FileColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.FileColumn.HeaderText = "File";
            this.FileColumn.Name = "FileColumn";
            this.FileColumn.ToolTipText = "A position indicating an argument in a function with a message file number. Set to 0 - if not used.";
            this.FileColumn.Width = 35;
            // 
            // FunctionsRules
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(462, 234);
            this.Controls.Add(this.bHelp);
            this.Controls.Add(this.label);
            this.Controls.Add(this.dgvTemplates);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 100);
            this.Name = "FunctionsRules";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Dialog functions configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RulesDialog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTemplates)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvTemplates;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Button bHelp;
        private System.Windows.Forms.DataGridViewComboBoxColumn typeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn functionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn argsColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MsgColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn gotoColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iqColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileColumn;
    }
}