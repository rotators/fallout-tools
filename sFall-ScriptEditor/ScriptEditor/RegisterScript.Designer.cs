
namespace ScriptEditor {
    partial class RegisterScript {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterScript));
            this.dgvScripts = new ScriptEditor.TextEditorUI.DataGridViewEx();
            this.EntryCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLine = new System.Windows.Forms.DataGridViewButtonColumn();
            this.cScript = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cVars = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.DefinetextBox = new System.Windows.Forms.TextBox();
            this.AllowCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Save_button = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Addbutton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.Delbutton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Upbutton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Downbutton = new System.Windows.Forms.ToolStripButton();
            this.FindtextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScripts)).BeginInit();
            this.groupBox.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvScripts
            // 
            this.dgvScripts.AllowUserToAddRows = false;
            this.dgvScripts.AllowUserToDeleteRows = false;
            this.dgvScripts.AllowUserToResizeRows = false;
            this.dgvScripts.BackgroundColor = System.Drawing.SystemColors.MenuBar;
            this.dgvScripts.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvScripts.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvScripts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvScripts.ColumnHeadersHeight = 24;
            this.dgvScripts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvScripts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.EntryCol,
            this.cLine,
            this.cScript,
            this.cDescription,
            this.cVars,
            this.cName});
            this.dgvScripts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvScripts.Location = new System.Drawing.Point(6, 19);
            this.dgvScripts.MultiSelect = false;
            this.dgvScripts.Name = "dgvScripts";
            this.dgvScripts.RowHeadersVisible = false;
            this.dgvScripts.RowHeadersWidth = 30;
            this.dgvScripts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvScripts.Size = new System.Drawing.Size(724, 453);
            this.dgvScripts.TabIndex = 0;
            this.dgvScripts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvScripts_CellClick);
            this.dgvScripts.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvScripts_CellContentClick);
            // 
            // EntryCol
            // 
            this.EntryCol.HeaderText = "Entry";
            this.EntryCol.Name = "EntryCol";
            this.EntryCol.ReadOnly = true;
            this.EntryCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EntryCol.Visible = false;
            this.EntryCol.Width = 10;
            // 
            // cLine
            // 
            this.cLine.HeaderText = "Script #";
            this.cLine.Name = "cLine";
            this.cLine.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cLine.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.cLine.ToolTipText = "Script ID";
            this.cLine.Width = 50;
            // 
            // cScript
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.cScript.DefaultCellStyle = dataGridViewCellStyle2;
            this.cScript.HeaderText = "Script File";
            this.cScript.Name = "cScript";
            this.cScript.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cScript.Width = 90;
            // 
            // cDescription
            // 
            this.cDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cDescription.FillWeight = 70F;
            this.cDescription.HeaderText = "Descriptions";
            this.cDescription.Name = "cDescription";
            this.cDescription.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // cVars
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.cVars.DefaultCellStyle = dataGridViewCellStyle3;
            this.cVars.HeaderText = "LVars:";
            this.cVars.Name = "cVars";
            this.cVars.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cVars.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cVars.ToolTipText = "Local Variables";
            this.cVars.Width = 40;
            // 
            // cName
            // 
            this.cName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cName.FillWeight = 30F;
            this.cName.HeaderText = "Script Name";
            this.cName.Name = "cName";
            this.cName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cName.ToolTipText = "Script game name in scrname.msg";
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.dgvScripts);
            this.groupBox.Location = new System.Drawing.Point(3, 54);
            this.groupBox.Name = "groupBox";
            this.groupBox.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox.Size = new System.Drawing.Size(736, 478);
            this.groupBox.TabIndex = 1;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Scripts List";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "RegisterReady.png");
            this.imageList1.Images.SetKeyName(1, "RegisterNeed.png");
            // 
            // DefinetextBox
            // 
            this.DefinetextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DefinetextBox.Enabled = false;
            this.DefinetextBox.Location = new System.Drawing.Point(185, 29);
            this.DefinetextBox.Name = "DefinetextBox";
            this.DefinetextBox.Size = new System.Drawing.Size(497, 20);
            this.DefinetextBox.TabIndex = 7;
            // 
            // AllowCheckBox
            // 
            this.AllowCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AllowCheckBox.AutoSize = true;
            this.AllowCheckBox.Enabled = false;
            this.AllowCheckBox.ForeColor = System.Drawing.Color.Firebrick;
            this.AllowCheckBox.Location = new System.Drawing.Point(688, 30);
            this.AllowCheckBox.Name = "AllowCheckBox";
            this.AllowCheckBox.Size = new System.Drawing.Size(51, 17);
            this.AllowCheckBox.TabIndex = 8;
            this.AllowCheckBox.Text = "Allow";
            this.AllowCheckBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label2.Location = new System.Drawing.Point(6, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(173, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Add definition for script to Scripts.h:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Save_button,
            this.toolStripSeparator1,
            this.Addbutton,
            this.toolStripSeparator4,
            this.Delbutton,
            this.toolStripSeparator2,
            this.Upbutton,
            this.toolStripSeparator3,
            this.Downbutton,
            this.FindtextBox,
            this.toolStripLabel1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(742, 25);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // Save_button
            // 
            this.Save_button.Image = ((System.Drawing.Image)(resources.GetObject("Save_button.Image")));
            this.Save_button.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Save_button.Name = "Save_button";
            this.Save_button.Size = new System.Drawing.Size(79, 22);
            this.Save_button.Text = "Registered";
            this.Save_button.ToolTipText = "Save all changes to files.";
            this.Save_button.Click += new System.EventHandler(this.Save_button_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // Addbutton
            // 
            this.Addbutton.AutoSize = false;
            this.Addbutton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Addbutton.Image = ((System.Drawing.Image)(resources.GetObject("Addbutton.Image")));
            this.Addbutton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Addbutton.Name = "Addbutton";
            this.Addbutton.Size = new System.Drawing.Size(25, 22);
            this.Addbutton.ToolTipText = "Add script line";
            this.Addbutton.Click += new System.EventHandler(this.Addbutton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // Delbutton
            // 
            this.Delbutton.AutoSize = false;
            this.Delbutton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Delbutton.Image = ((System.Drawing.Image)(resources.GetObject("Delbutton.Image")));
            this.Delbutton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Delbutton.Name = "Delbutton";
            this.Delbutton.Size = new System.Drawing.Size(25, 22);
            this.Delbutton.ToolTipText = "Delete last script line";
            this.Delbutton.Click += new System.EventHandler(this.Delbutton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // Upbutton
            // 
            this.Upbutton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Upbutton.AutoSize = false;
            this.Upbutton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Upbutton.Image = ((System.Drawing.Image)(resources.GetObject("Upbutton.Image")));
            this.Upbutton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Upbutton.Name = "Upbutton";
            this.Upbutton.Size = new System.Drawing.Size(25, 22);
            this.Upbutton.ToolTipText = "Find up";
            this.Upbutton.Click += new System.EventHandler(this.Upbutton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator3.AutoSize = false;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(8, 25);
            // 
            // Downbutton
            // 
            this.Downbutton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Downbutton.AutoSize = false;
            this.Downbutton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Downbutton.Image = ((System.Drawing.Image)(resources.GetObject("Downbutton.Image")));
            this.Downbutton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Downbutton.Name = "Downbutton";
            this.Downbutton.Size = new System.Drawing.Size(25, 22);
            this.Downbutton.ToolTipText = "Find down";
            this.Downbutton.Click += new System.EventHandler(this.Downbutton_Click);
            // 
            // FindtextBox
            // 
            this.FindtextBox.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.FindtextBox.AutoSize = false;
            this.FindtextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.FindtextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FindtextBox.Name = "FindtextBox";
            this.FindtextBox.Size = new System.Drawing.Size(300, 21);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel1.AutoSize = false;
            this.toolStripLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLabel1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripLabel1.Image")));
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(20, 16);
            // 
            // RegisterScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 535);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.AllowCheckBox);
            this.Controls.Add(this.DefinetextBox);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(250, 250);
            this.Name = "RegisterScript";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Scripts list editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RegisterScript_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RegisterScript_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvScripts)).EndInit();
            this.groupBox.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ScriptEditor.TextEditorUI.DataGridViewEx dgvScripts;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox DefinetextBox;
        private System.Windows.Forms.CheckBox AllowCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton Save_button;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton Addbutton;
        private System.Windows.Forms.ToolStripButton Delbutton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton Upbutton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton Downbutton;
        private System.Windows.Forms.ToolStripTextBox FindtextBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn EntryCol;
        private System.Windows.Forms.DataGridViewButtonColumn cLine;
        private System.Windows.Forms.DataGridViewTextBoxColumn cScript;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn cVars;
        private System.Windows.Forms.DataGridViewTextBoxColumn cName;
    }
}