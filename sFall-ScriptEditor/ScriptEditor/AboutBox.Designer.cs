namespace ScriptEditor {
    partial class AboutBox {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.labelProductName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.linkRepository = new System.Windows.Forms.LinkLabel();
            this.linkSfallDocumentation = new System.Windows.Forms.LinkLabel();
            this.linkLocalDocumentation = new System.Windows.Forms.LinkLabel();
            this.labelComponents = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.RichTextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            //
            // tableLayoutPanel
            //
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.labelProductName, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelVersion, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.linkRepository, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.linkSfallDocumentation, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.linkLocalDocumentation, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.labelComponents, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.textBoxDescription, 0, 6);
            this.tableLayoutPanel.Controls.Add(this.okButton, 0, 7);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(8, 8);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 8;
            this.tableLayoutPanel.RowStyles.Clear();
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(504, 404);
            this.tableLayoutPanel.TabIndex = 0;
            //
            // logoPictureBox
            //
            this.logoPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.logoPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.logoPictureBox.TabIndex = 12;
            this.logoPictureBox.TabStop = false;
            this.logoPictureBox.Visible = false;
            //
            // labelProductName
            //
            this.labelProductName.AutoSize = false;
            this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelProductName.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProductName.Location = new System.Drawing.Point(0, 0);
            this.labelProductName.Margin = new System.Windows.Forms.Padding(0);
            this.labelProductName.MaximumSize = new System.Drawing.Size(0, 32);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(504, 32);
            this.labelProductName.TabIndex = 19;
            this.labelProductName.Text = "Sfall Script Editor";
            this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelVersion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersion.Location = new System.Drawing.Point(0, 32);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(0);
            this.labelVersion.MaximumSize = new System.Drawing.Size(0, 24);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(504, 24);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "Version 5.0";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkRepository
            // 
            this.linkRepository.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkRepository.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkRepository.Location = new System.Drawing.Point(0, 56);
            this.linkRepository.Margin = new System.Windows.Forms.Padding(0);
            this.linkRepository.MaximumSize = new System.Drawing.Size(0, 24);
            this.linkRepository.Name = "linkRepository";
            this.linkRepository.Size = new System.Drawing.Size(504, 24);
            this.linkRepository.TabIndex = 21;
            this.linkRepository.Text = "Repository: github.com/rotators/fallout-tools";
            this.linkRepository.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkRepository.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Repository_LinkClicked);
            // 
            // linkSfallDocumentation
            // 
            this.linkSfallDocumentation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkSfallDocumentation.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkSfallDocumentation.Margin = new System.Windows.Forms.Padding(0);
            this.linkSfallDocumentation.Name = "linkSfallDocumentation";
            this.linkSfallDocumentation.Size = new System.Drawing.Size(504, 24);
            this.linkSfallDocumentation.TabIndex = 22;
            this.linkSfallDocumentation.Text = "Sfall scripting documentation: sfall-team.github.io/sfall";
            this.linkSfallDocumentation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkSfallDocumentation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SfallDocumentation_LinkClicked);
            // 
            // linkLocalDocumentation
            // 
            this.linkLocalDocumentation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLocalDocumentation.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLocalDocumentation.Margin = new System.Windows.Forms.Padding(0);
            this.linkLocalDocumentation.Name = "linkLocalDocumentation";
            this.linkLocalDocumentation.Size = new System.Drawing.Size(504, 24);
            this.linkLocalDocumentation.TabIndex = 23;
            this.linkLocalDocumentation.Text = "Open local documentation folder";
            this.linkLocalDocumentation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLocalDocumentation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LocalDocumentation_LinkClicked);
            // 
            // labelComponents
            // 
            this.labelComponents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelComponents.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelComponents.Margin = new System.Windows.Forms.Padding(0);
            this.labelComponents.Name = "labelComponents";
            this.labelComponents.Size = new System.Drawing.Size(504, 28);
            this.labelComponents.TabIndex = 24;
            this.labelComponents.Text = "Components and licenses";
            this.labelComponents.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxDescription.Location = new System.Drawing.Point(0, 156);
            this.textBoxDescription.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textBoxDescription.Size = new System.Drawing.Size(504, 212);
            this.textBoxDescription.TabIndex = 25;
            this.textBoxDescription.TabStop = false;
            this.textBoxDescription.Text = "Description";
            this.textBoxDescription.WordWrap = true;
            this.textBoxDescription.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.Description_LinkClicked);
            this.textBoxDescription.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Description_MouseClick);
            this.textBoxDescription.MouseLeave += new System.EventHandler(this.Description_MouseLeave);
            this.textBoxDescription.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Description_MouseMove);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(414, 376);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(90, 28);
            this.okButton.TabIndex = 26;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // AboutBox
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(520, 420);
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About Sfall Script Editor - Rotators Build";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.LinkLabel linkRepository;
        private System.Windows.Forms.LinkLabel linkSfallDocumentation;
        private System.Windows.Forms.LinkLabel linkLocalDocumentation;
        private System.Windows.Forms.Label labelComponents;
        private System.Windows.Forms.RichTextBox textBoxDescription;
        private System.Windows.Forms.Button okButton;
    }
}
