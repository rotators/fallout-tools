namespace DATExplorer
{
    partial class FindFilesListForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindFilesListForm));
            this.lstViewFiles = new System.Windows.Forms.ListView();
            this.chFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // lstViewFiles
            // 
            this.lstViewFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(65)))));
            this.lstViewFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFile,
            this.chPath});
            this.lstViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstViewFiles.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lstViewFiles.Location = new System.Drawing.Point(0, 0);
            this.lstViewFiles.Name = "lstViewFiles";
            this.lstViewFiles.Size = new System.Drawing.Size(390, 473);
            this.lstViewFiles.TabIndex = 0;
            this.lstViewFiles.UseCompatibleStateImageBehavior = false;
            this.lstViewFiles.View = System.Windows.Forms.View.Details;
            this.lstViewFiles.DoubleClick += new System.EventHandler(this.lstViewFiles_DoubleClick);
            // 
            // chFile
            // 
            this.chFile.Text = "Name";
            this.chFile.Width = 160;
            // 
            // chPath
            // 
            this.chPath.Text = "File Path";
            this.chPath.Width = 210;
            // 
            // FindFilesListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(390, 473);
            this.Controls.Add(this.lstViewFiles);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(100, 200);
            this.Name = "FindFilesListForm";
            this.Text = "List of found files";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lstViewFiles;
        private System.Windows.Forms.ColumnHeader chFile;
        private System.Windows.Forms.ColumnHeader chPath;
    }
}