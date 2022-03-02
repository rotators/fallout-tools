/*
 * Created by SharpDevelop.
 * User: Killian
 * Date: 21.07.2014
 * Time: 1:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ScriptEditor
{
	partial class GoToLine
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
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
            this.label1 = new System.Windows.Forms.Label();
            this.bGo = new System.Windows.Forms.Button();
            this.tbLine = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.tbLine)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Line :";
            // 
            // bGo
            // 
            this.bGo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bGo.Location = new System.Drawing.Point(135, 4);
            this.bGo.Name = "bGo";
            this.bGo.Size = new System.Drawing.Size(33, 23);
            this.bGo.TabIndex = 2;
            this.bGo.Text = "GO";
            this.bGo.UseVisualStyleBackColor = true;
            this.bGo.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GoToLineKeyUp);
            // 
            // tbLine
            // 
            this.tbLine.Location = new System.Drawing.Point(48, 7);
            this.tbLine.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tbLine.Name = "tbLine";
            this.tbLine.Size = new System.Drawing.Size(81, 20);
            this.tbLine.TabIndex = 1;
            this.tbLine.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbLine.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tbLine.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TbLineKeyDown);
            // 
            // GoToLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(176, 35);
            this.Controls.Add(this.tbLine);
            this.Controls.Add(this.bGo);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GoToLine";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Go to line";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.tbLine)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		internal System.Windows.Forms.Button bGo;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.NumericUpDown tbLine;
	}
}
