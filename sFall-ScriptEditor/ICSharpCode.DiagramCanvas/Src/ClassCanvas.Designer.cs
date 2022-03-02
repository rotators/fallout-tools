/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 23/09/2006
 * Time: 14:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ICSharpCode.ClassDiagram
{
	partial class ClassCanvas : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.CanvasPicture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.CanvasPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // CanvasPicture
            // 
            this.CanvasPicture.Cursor = System.Windows.Forms.Cursors.Default;
            this.CanvasPicture.Location = new System.Drawing.Point(0, 0);
            this.CanvasPicture.Name = "CanvasPicture";
            this.CanvasPicture.Size = new System.Drawing.Size(150, 150);
            this.CanvasPicture.TabIndex = 0;
            this.CanvasPicture.TabStop = false;
            this.CanvasPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBoxPaint);
            this.CanvasPicture.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PictureBoxMouseClick);
            this.CanvasPicture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBoxMouseDown);
            this.CanvasPicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBoxMouseMove);
            this.CanvasPicture.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBoxMouseUp);
            // 
            // ClassCanvas
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.CanvasPicture);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.Name = "ClassCanvas";
            this.Size = new System.Drawing.Size(250, 250);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ClassCanvasDragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.ClassCanvasDragOver);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ClassCanvasKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ClassCanvasKeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.CanvasPicture)).EndInit();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.PictureBox CanvasPicture;
	}
}
