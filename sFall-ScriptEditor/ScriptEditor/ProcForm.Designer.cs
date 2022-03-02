namespace ScriptEditor
{
    partial class ProcForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBoxProcedure = new System.Windows.Forms.GroupBox();
            this.rbAfterSelProcedure = new System.Windows.Forms.RadioButton();
            this.rbPasteAtEnd = new System.Windows.Forms.RadioButton();
            this.cbCopyBodyProc = new System.Windows.Forms.CheckBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.Cancel = new System.Windows.Forms.Button();
            this.Create = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxProcedure.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxProcedure
            // 
            this.groupBoxProcedure.Controls.Add(this.rbAfterSelProcedure);
            this.groupBoxProcedure.Controls.Add(this.rbPasteAtEnd);
            this.groupBoxProcedure.Controls.Add(this.cbCopyBodyProc);
            this.groupBoxProcedure.Location = new System.Drawing.Point(12, 38);
            this.groupBoxProcedure.Name = "groupBoxProcedure";
            this.groupBoxProcedure.Size = new System.Drawing.Size(265, 65);
            this.groupBoxProcedure.TabIndex = 0;
            this.groupBoxProcedure.TabStop = false;
            this.groupBoxProcedure.Text = "Procedure options";
            // 
            // rbAfterSelProcedure
            // 
            this.rbAfterSelProcedure.AutoSize = true;
            this.rbAfterSelProcedure.Location = new System.Drawing.Point(141, 19);
            this.rbAfterSelProcedure.Name = "rbAfterSelProcedure";
            this.rbAfterSelProcedure.Size = new System.Drawing.Size(129, 17);
            this.rbAfterSelProcedure.TabIndex = 2;
            this.rbAfterSelProcedure.Text = "Insert at caret position";
            this.toolTip.SetToolTip(this.rbAfterSelProcedure, "Inserts the created procedure to the current position of the carriage cursor or t" +
        "o the position after the selected procedure.");
            this.rbAfterSelProcedure.UseVisualStyleBackColor = true;
            // 
            // rbPasteAtEnd
            // 
            this.rbPasteAtEnd.AutoSize = true;
            this.rbPasteAtEnd.Checked = true;
            this.rbPasteAtEnd.Location = new System.Drawing.Point(16, 19);
            this.rbPasteAtEnd.Name = "rbPasteAtEnd";
            this.rbPasteAtEnd.Size = new System.Drawing.Size(113, 17);
            this.rbPasteAtEnd.TabIndex = 1;
            this.rbPasteAtEnd.TabStop = true;
            this.rbPasteAtEnd.Text = "Paste at end script";
            this.rbPasteAtEnd.UseVisualStyleBackColor = true;
            // 
            // cbCopyBodyProc
            // 
            this.cbCopyBodyProc.AutoSize = true;
            this.cbCopyBodyProc.Location = new System.Drawing.Point(16, 42);
            this.cbCopyBodyProc.Name = "cbCopyBodyProc";
            this.cbCopyBodyProc.Size = new System.Drawing.Size(203, 17);
            this.cbCopyBodyProc.TabIndex = 0;
            this.cbCopyBodyProc.Text = "Copy from current selected procedure";
            this.toolTip.SetToolTip(this.cbCopyBodyProc, "Copies the code of the currently selected procedure to the newly created procedur" +
        "e.");
            this.cbCopyBodyProc.UseVisualStyleBackColor = true;
            // 
            // tbName
            // 
            this.tbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbName.Location = new System.Drawing.Point(12, 8);
            this.tbName.MaxLength = 200;
            this.tbName.Multiline = true;
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(346, 25);
            this.tbName.TabIndex = 1;
            this.tbName.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbName_MouseClick);
            this.tbName.Leave += new System.EventHandler(this.tbName_Leave);
            this.tbName.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.tbName_PreviewKeyDown);
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(283, 80);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 3;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // Create
            // 
            this.Create.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Create.Location = new System.Drawing.Point(283, 38);
            this.Create.Name = "Create";
            this.Create.Size = new System.Drawing.Size(75, 23);
            this.Create.TabIndex = 2;
            this.Create.Text = "Create";
            this.Create.UseVisualStyleBackColor = true;
            // 
            // toolTip
            // 
            this.toolTip.ShowAlways = true;
            // 
            // ProcForm
            // 
            this.AcceptButton = this.Create;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(370, 114);
            this.Controls.Add(this.Create);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.groupBoxProcedure);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProcForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Procedure";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProcForm_FormClosing);
            this.Shown += new System.EventHandler(this.ProcForm_Shown);
            this.groupBoxProcedure.ResumeLayout(false);
            this.groupBoxProcedure.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancel;
        protected internal System.Windows.Forms.Button Create;
        private System.Windows.Forms.RadioButton rbPasteAtEnd;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.RadioButton rbAfterSelProcedure;
        private System.Windows.Forms.CheckBox cbCopyBodyProc;
        internal System.Windows.Forms.GroupBox groupBoxProcedure;
        private System.Windows.Forms.ToolTip toolTip;
    }
}