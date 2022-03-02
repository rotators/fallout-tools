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
            bar = new ProgressBar() { Width = 305, Height = 15, Top = 14, Maximum = max};
            var lb = new Label() { Text = str, Top = 0, Left = 10, AutoSize = true};

            progressForm = new Form()
            {
                MinimumSize = new Size(200, 20), Width = 312, Height = 20,
                ControlBox = false, ShowIcon = false, ShowInTaskbar = false,
                StartPosition = (owner.Location.IsEmpty) ? FormStartPosition.CenterScreen : FormStartPosition.Manual,
                FormBorderStyle = FormBorderStyle.FixedSingle
            };
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
