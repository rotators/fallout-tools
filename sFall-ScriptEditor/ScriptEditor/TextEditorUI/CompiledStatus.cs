using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScriptEditor.TextEditorUI
{
    internal class CompiledStatus : Form
    {
        Timer timer = new Timer();

        float opacity = 1.0f;
        int delay = 50;

        public CompiledStatus(bool status, Form frm)
        {
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            Cursor          = Cursors.IBeam;
            StartPosition   = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.None;
            Owner           = frm;
            ShowInTaskbar   = false;
            MinimumSize     = new Size(96, 96);
            Size            = MinimumSize;
            Location        = new Point(frm.Location.X + (frm.Size.Width / 2) - 48,
                                        frm.Location.Y + (frm.Size.Height / 2) - 48);

            BackgroundImageLayout = ImageLayout.Stretch;
            BackgroundImage = ColorTheme.IsDarkTheme ? Properties.Resources.compiled_dark 
                                                     : Properties.Resources.compiled;
            TransparencyKey = ColorTheme.IsDarkTheme ? Color.FromArgb(50, 50, 50)
                                                     : Color.FromArgb(250, 250, 250);

            Label lbl0 = new Label() {
                Size = new Size(96, 15),
                Top = 2,
                Left = 1,
                TextAlign = ContentAlignment.MiddleCenter,

                Text = status ? "Successfully" : "Failed"
            };

            Label lbl1 =  new Label() {
                AutoSize = true,
                Top = 75,
                Left = 12,

                Text = "Compiled"
            };

            lbl0.Font = lbl1.Font = new Font(lbl0.Font.FontFamily, 9.5f, FontStyle.Bold);
            lbl0.BackColor = lbl1.BackColor = Color.Transparent;
            lbl0.ForeColor = lbl1.ForeColor = status ? Color.GhostWhite : Color.LemonChiffon;

            Controls.Add(lbl0);
            Controls.Add(lbl1);

            timer.Interval = 20;
            timer.Tick += new EventHandler(timer_Tick);

            base.CreateHandle();
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override void OnClick(EventArgs e)
        {
            Hide();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (--delay > 0)
                return;

            opacity -= 0.075f;
            if (opacity <= 0) {
                timer.Stop();
                Hide();
                Dispose(true);
            } else   
                Opacity = opacity;
        }

        public void ShowCompileStatus()
        {
            Show();
            timer.Start();
        }
    }
}
