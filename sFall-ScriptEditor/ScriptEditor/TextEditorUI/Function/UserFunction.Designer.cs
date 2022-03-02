namespace ScriptEditor.TextEditorUI
{
    partial class UserFunction
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
        private void InitializeComponent()
        {
            this.tbCode = new System.Windows.Forms.TextBox();
            this.tbDesc = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bClose = new System.Windows.Forms.Button();
            this.bDone = new System.Windows.Forms.Button();
            this.tbName = new System.Windows.Forms.TextBox();
            this.cbHighlight = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbCode
            // 
            this.tbCode.Location = new System.Drawing.Point(12, 46);
            this.tbCode.Name = "tbCode";
            this.tbCode.Size = new System.Drawing.Size(465, 20);
            this.tbCode.TabIndex = 1;
            // 
            // tbDesc
            // 
            this.tbDesc.Location = new System.Drawing.Point(12, 91);
            this.tbDesc.Multiline = true;
            this.tbDesc.Name = "tbDesc";
            this.tbDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDesc.Size = new System.Drawing.Size(465, 47);
            this.tbDesc.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Paste Code:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Short Descriptions:";
            // 
            // bClose
            // 
            this.bClose.Location = new System.Drawing.Point(402, 144);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(75, 23);
            this.bClose.TabIndex = 4;
            this.bClose.Text = "Close";
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Click += new System.EventHandler(this.bClose_Click);
            // 
            // bDone
            // 
            this.bDone.Location = new System.Drawing.Point(321, 144);
            this.bDone.Name = "bDone";
            this.bDone.Size = new System.Drawing.Size(75, 23);
            this.bDone.TabIndex = 5;
            this.bDone.Text = "Done";
            this.bDone.UseVisualStyleBackColor = true;
            this.bDone.Click += new System.EventHandler(this.bDone_Click);
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(12, 7);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(238, 20);
            this.tbName.TabIndex = 0;
            // 
            // cbHighlight
            // 
            this.cbHighlight.AutoSize = true;
            this.cbHighlight.Location = new System.Drawing.Point(376, 9);
            this.cbHighlight.Name = "cbHighlight";
            this.cbHighlight.Size = new System.Drawing.Size(101, 17);
            this.cbHighlight.TabIndex = 7;
            this.cbHighlight.Text = "Add to Highlight";
            this.cbHighlight.UseVisualStyleBackColor = true;
            this.cbHighlight.Click += new System.EventHandler(this.cbHighlight_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(256, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Name Function";
            // 
            // UserFunction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 174);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbHighlight);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.bDone);
            this.Controls.Add(this.bClose);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbDesc);
            this.Controls.Add(this.tbCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "UserFunction";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add User Function";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbCode;
        private System.Windows.Forms.TextBox tbDesc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bClose;
        private System.Windows.Forms.Button bDone;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.CheckBox cbHighlight;
        private System.Windows.Forms.Label label3;
    }
}