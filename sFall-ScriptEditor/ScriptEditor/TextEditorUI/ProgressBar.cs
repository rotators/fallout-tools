using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ScriptEditor.TextEditorUI
{
    internal class ProgressBarForm
    {
        Form progressForm;
        ProgressBar bar;

        public ProgressBarForm(Form owner, int max, string str = "Loading message file...")
        {
            progressForm = new Form()
            {
                AutoScaleDimensions = new SizeF(96F, 96F), AutoScaleMode = AutoScaleMode.Dpi,
                ControlBox = false, ShowIcon = false, ShowInTaskbar = false,
                StartPosition = (owner.Location.IsEmpty) ? FormStartPosition.CenterScreen : FormStartPosition.Manual,
                FormBorderStyle = FormBorderStyle.FixedSingle
            };
            int dpi = owner.DeviceDpi;
            bar = new ProgressBar() {
                Width = DpiHelper.Scale(305, dpi), Height = DpiHelper.Scale(15, dpi),
                Top = DpiHelper.Scale(14, dpi), Maximum = max
            };
            var lb = new Label() {
                Text = str, Top = 0, Left = DpiHelper.Scale(10, dpi), AutoSize = true
            };
            progressForm.MinimumSize = DpiHelper.Scale(owner, new Size(200, 20));
            progressForm.Width = DpiHelper.Scale(312, dpi);
            progressForm.Height = DpiHelper.Scale(20, dpi);
            progressForm.Controls.Add(lb);
            progressForm.Controls.Add(bar);

            if (!owner.Location.IsEmpty)
                progressForm.Location = new Point(owner.Location.X + (owner.Width - progressForm.Width) / 2,
                                                  owner.Location.Y + (owner.Height - progressForm.Height) / 2);
            progressForm.Show(owner);
            Application.DoEvents();
        }

        public int SetProgress
        {
            set {
                bar.Value = value;
                Application.DoEvents();
            }
        }

        public void IncProgress()
        {
            if (bar.Value < bar.Maximum) bar.Value++;
            Application.DoEvents();
        }

        public void Dispose()
        {
            progressForm.Close();
            progressForm.Dispose();
        }
    }
}
