namespace ScriptEditor {
    partial class SearchForm {
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
            System.Windows.Forms.Label label1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this.cbSearch = new System.Windows.Forms.ComboBox();
            this.cbRegular = new System.Windows.Forms.CheckBox();
            this.rbCurrent = new System.Windows.Forms.RadioButton();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.rbFolder = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.bChange = new System.Windows.Forms.Button();
            this.cbSearchSubfolders = new System.Windows.Forms.CheckBox();
            this.bSearch = new System.Windows.Forms.Button();
            this.fbdSearchFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.cbFindAll = new System.Windows.Forms.CheckBox();
            this.tbReplace = new System.Windows.Forms.TextBox();
            this.bReplace = new System.Windows.Forms.Button();
            this.cbSearchPath = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lbFindFiles = new System.Windows.Forms.ListBox();
            this.cbCase = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbWord = new System.Windows.Forms.CheckBox();
            this.cbFileMask = new System.Windows.Forms.ComboBox();
            this.labelCount = new System.Windows.Forms.Label();
            this.bHide = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(13, 167);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(86, 13);
            label1.TabIndex = 5;
            label1.Text = "Folder to search:";
            // 
            // cbSearch
            // 
            this.cbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSearch.FormattingEnabled = true;
            this.cbSearch.Location = new System.Drawing.Point(13, 22);
            this.cbSearch.Name = "cbSearch";
            this.cbSearch.Size = new System.Drawing.Size(418, 21);
            this.cbSearch.TabIndex = 28;
            // 
            // cbRegular
            // 
            this.cbRegular.AutoSize = true;
            this.cbRegular.Location = new System.Drawing.Point(13, 91);
            this.cbRegular.Name = "cbRegular";
            this.cbRegular.Size = new System.Drawing.Size(116, 17);
            this.cbRegular.TabIndex = 1;
            this.cbRegular.Text = "Regular expression";
            this.cbRegular.UseVisualStyleBackColor = true;
            this.cbRegular.CheckedChanged += new System.EventHandler(this.cbRegular_CheckedChanged);
            // 
            // rbCurrent
            // 
            this.rbCurrent.AutoSize = true;
            this.rbCurrent.Location = new System.Drawing.Point(153, 91);
            this.rbCurrent.Name = "rbCurrent";
            this.rbCurrent.Size = new System.Drawing.Size(125, 17);
            this.rbCurrent.TabIndex = 2;
            this.rbCurrent.Text = "Find in current scripts";
            this.rbCurrent.UseVisualStyleBackColor = true;
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Checked = true;
            this.rbAll.Location = new System.Drawing.Point(153, 120);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(129, 17);
            this.rbAll.TabIndex = 3;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "Find in all open scripts";
            this.rbAll.UseVisualStyleBackColor = true;
            // 
            // rbFolder
            // 
            this.rbFolder.AutoSize = true;
            this.rbFolder.Location = new System.Drawing.Point(153, 149);
            this.rbFolder.Name = "rbFolder";
            this.rbFolder.Size = new System.Drawing.Size(106, 17);
            this.rbFolder.TabIndex = 4;
            this.rbFolder.Text = "Find in files folder";
            this.rbFolder.UseVisualStyleBackColor = true;
            this.rbFolder.CheckedChanged += new System.EventHandler(this.rbFolder_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Replace Text:";
            // 
            // bChange
            // 
            this.bChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bChange.Enabled = false;
            this.bChange.Image = ((System.Drawing.Image)(resources.GetObject("bChange.Image")));
            this.bChange.Location = new System.Drawing.Point(326, 209);
            this.bChange.Name = "bChange";
            this.bChange.Size = new System.Drawing.Size(104, 23);
            this.bChange.TabIndex = 7;
            this.bChange.Text = "Change";
            this.bChange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bChange.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bChange.UseVisualStyleBackColor = true;
            // 
            // cbSearchSubfolders
            // 
            this.cbSearchSubfolders.AutoSize = true;
            this.cbSearchSubfolders.Checked = true;
            this.cbSearchSubfolders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSearchSubfolders.Enabled = false;
            this.cbSearchSubfolders.Location = new System.Drawing.Point(13, 213);
            this.cbSearchSubfolders.Name = "cbSearchSubfolders";
            this.cbSearchSubfolders.Size = new System.Drawing.Size(122, 17);
            this.cbSearchSubfolders.TabIndex = 8;
            this.cbSearchSubfolders.Text = "Search in subfolders";
            this.cbSearchSubfolders.UseVisualStyleBackColor = true;
            // 
            // bSearch
            // 
            this.bSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bSearch.Image = ((System.Drawing.Image)(resources.GetObject("bSearch.Image")));
            this.bSearch.Location = new System.Drawing.Point(327, 88);
            this.bSearch.Name = "bSearch";
            this.bSearch.Size = new System.Drawing.Size(103, 23);
            this.bSearch.TabIndex = 10;
            this.bSearch.Text = "Search";
            this.bSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bSearch.UseVisualStyleBackColor = true;
            // 
            // fbdSearchFolder
            // 
            this.fbdSearchFolder.Description = "Pick folder to search";
            this.fbdSearchFolder.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // cbFindAll
            // 
            this.cbFindAll.AutoSize = true;
            this.cbFindAll.Location = new System.Drawing.Point(13, 120);
            this.cbFindAll.Name = "cbFindAll";
            this.cbFindAll.Size = new System.Drawing.Size(102, 17);
            this.cbFindAll.TabIndex = 11;
            this.cbFindAll.Text = "Find all matches";
            this.cbFindAll.UseVisualStyleBackColor = true;
            // 
            // tbReplace
            // 
            this.tbReplace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbReplace.Location = new System.Drawing.Point(13, 62);
            this.tbReplace.Name = "tbReplace";
            this.tbReplace.Size = new System.Drawing.Size(418, 20);
            this.tbReplace.TabIndex = 12;
            // 
            // bReplace
            // 
            this.bReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bReplace.Location = new System.Drawing.Point(326, 117);
            this.bReplace.Name = "bReplace";
            this.bReplace.Size = new System.Drawing.Size(104, 23);
            this.bReplace.TabIndex = 13;
            this.bReplace.Text = "Find && Replace";
            this.bReplace.UseVisualStyleBackColor = true;
            // 
            // cbSearchPath
            // 
            this.cbSearchPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSearchPath.Enabled = false;
            this.cbSearchPath.FormattingEnabled = true;
            this.cbSearchPath.Location = new System.Drawing.Point(12, 184);
            this.cbSearchPath.Name = "cbSearchPath";
            this.cbSearchPath.Size = new System.Drawing.Size(418, 21);
            this.cbSearchPath.Sorted = true;
            this.cbSearchPath.TabIndex = 29;
            this.cbSearchPath.SelectedIndexChanged += new System.EventHandler(this.cbSearchPath_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Find Text:";
            // 
            // lbFindFiles
            // 
            this.lbFindFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbFindFiles.FormattingEnabled = true;
            this.lbFindFiles.HorizontalScrollbar = true;
            this.lbFindFiles.Location = new System.Drawing.Point(13, 254);
            this.lbFindFiles.Name = "lbFindFiles";
            this.lbFindFiles.Size = new System.Drawing.Size(418, 4);
            this.lbFindFiles.TabIndex = 21;
            // 
            // cbCase
            // 
            this.cbCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbCase.AutoSize = true;
            this.cbCase.Checked = true;
            this.cbCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCase.Location = new System.Drawing.Point(238, 3);
            this.cbCase.Name = "cbCase";
            this.cbCase.Size = new System.Drawing.Size(82, 17);
            this.cbCase.TabIndex = 22;
            this.cbCase.Text = "Match case";
            this.cbCase.UseVisualStyleBackColor = true;
            this.cbCase.Click += new System.EventHandler(this.cbCase_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 238);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Found files with matches:";
            // 
            // cbWord
            // 
            this.cbWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbWord.AutoSize = true;
            this.cbWord.Location = new System.Drawing.Point(326, 3);
            this.cbWord.Name = "cbWord";
            this.cbWord.Size = new System.Drawing.Size(105, 17);
            this.cbWord.TabIndex = 24;
            this.cbWord.Text = "Whole word only";
            this.cbWord.UseVisualStyleBackColor = true;
            this.cbWord.Click += new System.EventHandler(this.cbWord_Click);
            // 
            // cbFileMask
            // 
            this.cbFileMask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFileMask.Enabled = false;
            this.cbFileMask.FormattingEnabled = true;
            this.cbFileMask.Items.AddRange(new object[] {
            "All",
            "*.h",
            "*.ssl",
            "*.msg"});
            this.cbFileMask.Location = new System.Drawing.Point(205, 211);
            this.cbFileMask.Name = "cbFileMask";
            this.cbFileMask.Size = new System.Drawing.Size(53, 21);
            this.cbFileMask.TabIndex = 25;
            // 
            // labelCount
            // 
            this.labelCount.AutoSize = true;
            this.labelCount.Location = new System.Drawing.Point(145, 238);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(13, 13);
            this.labelCount.TabIndex = 26;
            this.labelCount.Text = "0";
            // 
            // bHide
            // 
            this.bHide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bHide.Location = new System.Drawing.Point(326, 146);
            this.bHide.Name = "bHide";
            this.bHide.Size = new System.Drawing.Size(104, 23);
            this.bHide.TabIndex = 7;
            this.bHide.Text = "Hide";
            this.bHide.UseVisualStyleBackColor = true;
            this.bHide.Click += new System.EventHandler(this.bHide_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(150, 214);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "File type:";
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 272);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelCount);
            this.Controls.Add(this.cbFileMask);
            this.Controls.Add(this.cbWord);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbCase);
            this.Controls.Add(this.lbFindFiles);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bReplace);
            this.Controls.Add(this.tbReplace);
            this.Controls.Add(this.cbFindAll);
            this.Controls.Add(this.bSearch);
            this.Controls.Add(this.cbSearchSubfolders);
            this.Controls.Add(this.bHide);
            this.Controls.Add(this.bChange);
            this.Controls.Add(this.label2);
            this.Controls.Add(label1);
            this.Controls.Add(this.rbFolder);
            this.Controls.Add(this.rbAll);
            this.Controls.Add(this.rbCurrent);
            this.Controls.Add(this.cbRegular);
            this.Controls.Add(this.cbSearchPath);
            this.Controls.Add(this.cbSearch);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1280, 1024);
            this.MinimumSize = new System.Drawing.Size(435, 303);
            this.Name = "SearchForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search & Replace";
            this.Activated += new System.EventHandler(this.SearchForm_Activated);
            this.Deactivate += new System.EventHandler(this.SearchForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SearchForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ComboBox cbSearch;
        internal System.Windows.Forms.TextBox tbReplace;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bHide;
        private System.Windows.Forms.Label label6;
        internal System.Windows.Forms.ListBox lbFindFiles;
        internal System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.ComboBox cbFileMask;
        internal System.Windows.Forms.Button bSearch;
        internal System.Windows.Forms.Button bReplace;
        internal System.Windows.Forms.RadioButton rbFolder;
        internal System.Windows.Forms.CheckBox cbCase;
        internal System.Windows.Forms.RadioButton rbCurrent;
        internal System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.CheckBox cbSearchSubfolders;
        internal System.Windows.Forms.CheckBox cbRegular;
        internal System.Windows.Forms.CheckBox cbFindAll;
        private System.Windows.Forms.Button bChange;
        private System.Windows.Forms.ComboBox cbSearchPath;
        private System.Windows.Forms.FolderBrowserDialog fbdSearchFolder;
        private System.Windows.Forms.CheckBox cbWord;

    }
}