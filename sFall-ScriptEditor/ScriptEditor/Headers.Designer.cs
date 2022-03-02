namespace ScriptEditor
{
    partial class Headers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Headers));
            this.headersFilelistView = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // headersFilelistView
            // 
            this.headersFilelistView.BackColor = System.Drawing.SystemColors.Control;
            this.headersFilelistView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.headersFilelistView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headersFilelistView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.headersFilelistView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.headersFilelistView.LabelWrap = false;
            this.headersFilelistView.Location = new System.Drawing.Point(0, 0);
            this.headersFilelistView.MultiSelect = false;
            this.headersFilelistView.Name = "headersFilelistView";
            this.headersFilelistView.ShowGroups = false;
            this.headersFilelistView.ShowItemToolTips = true;
            this.headersFilelistView.Size = new System.Drawing.Size(452, 269);
            this.headersFilelistView.SmallImageList = this.imageList1;
            this.headersFilelistView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.headersFilelistView.TabIndex = 0;
            this.headersFilelistView.UseCompatibleStateImageBehavior = false;
            this.headersFilelistView.View = System.Windows.Forms.View.List;
            this.headersFilelistView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
            this.headersFilelistView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            this.headersFilelistView.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.headersFilelistView_PreviewKeyDown);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "h.ico");
            this.imageList1.Images.SetKeyName(1, "h_b.ico");
            this.imageList1.Images.SetKeyName(2, "Include.png");
            // 
            // Headers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(452, 269);
            this.ControlBox = false;
            this.Controls.Add(this.headersFilelistView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "Headers";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Deactivate += new System.EventHandler(this.Headers_Deactivate);
            this.Load += new System.EventHandler(this.Headers_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Headers_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Headers_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView headersFilelistView;
        private System.Windows.Forms.ImageList imageList1;
    }
}