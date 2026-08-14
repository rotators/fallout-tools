using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ScriptEditor
{
    internal enum NotificationKind
    {
        Information,
        Success,
        Warning,
        Error
    }

    internal static class EditorNotifications
    {
        private sealed class BannerState
        {
            internal Panel Panel;
            internal Label Label;
            internal Timer Timer;
        }

        private static readonly Dictionary<Form, BannerState> Banners = new Dictionary<Form, BannerState>();

        internal static void Show(Control source, string message,
            NotificationKind kind = NotificationKind.Information, int duration = 5000)
        {
            if (String.IsNullOrWhiteSpace(message))
                return;

            Form form = source as Form;
            if (form == null && source != null)
                form = source.FindForm();
            if (form == null)
                form = Form.ActiveForm;

            TextEditor editor = form as TextEditor;
            if (editor != null) {
                editor.ShowStatusMessage(message, kind, duration);
                return;
            }

            if (form != null && !form.IsDisposed) {
                ShowBanner(form, message, kind, duration);
                return;
            }

            foreach (Form openForm in Application.OpenForms) {
                editor = openForm as TextEditor;
                if (editor != null) {
                    editor.ShowStatusMessage(message, kind, duration);
                    return;
                }
            }

            Program.printLog("   " + message);
        }

        private static void ShowBanner(Form form, string message, NotificationKind kind, int duration)
        {
            if (form.InvokeRequired) {
                form.BeginInvoke(new Action<Form, string, NotificationKind, int>(ShowBanner),
                    form, message, kind, duration);
                return;
            }

            BannerState state;
            if (!Banners.TryGetValue(form, out state)) {
                state = CreateBanner(form);
                Banners.Add(form, state);
                form.FormClosed += delegate {
                    BannerState removed;
                    if (Banners.TryGetValue(form, out removed)) {
                        removed.Timer.Dispose();
                        Banners.Remove(form);
                    }
                };
            }

            Color back;
            Color fore;
            GetColors(kind, InterfaceTheme.IsDark, out back, out fore);
            state.Panel.BackColor = back;
            state.Label.BackColor = back;
            state.Label.ForeColor = fore;
            state.Label.Text = GetPrefix(kind) + message.Replace('\r', ' ').Replace('\n', ' ');
            state.Label.AccessibleDescription = message;
            state.Panel.Visible = true;
            state.Panel.BringToFront();
            state.Timer.Stop();
            state.Timer.Interval = Math.Max(1000, duration);
            state.Timer.Start();
        }

        private static BannerState CreateBanner(Form form)
        {
            int margin = DpiHelper.Scale(form, 8);
            int height = DpiHelper.Scale(form, 30);
            Panel panel = new Panel {
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(margin, Math.Max(0, form.ClientSize.Height - height - margin)),
                Size = new Size(Math.Max(40, form.ClientSize.Width - margin * 2), height),
                Visible = false
            };
            Label label = new Label {
                AutoEllipsis = true,
                Dock = DockStyle.Fill,
                Font = form.Font,
                Padding = new Padding(DpiHelper.Scale(form, 8), 0, DpiHelper.Scale(form, 8), 0),
                TextAlign = ContentAlignment.MiddleLeft,
                AccessibleName = "Status message"
            };
            panel.Controls.Add(label);
            form.Controls.Add(panel);

            Timer timer = new Timer { Interval = 5000 };
            timer.Tick += delegate {
                timer.Stop();
                if (!panel.IsDisposed)
                    panel.Visible = false;
            };
            panel.Click += delegate { panel.Visible = false; timer.Stop(); };
            label.Click += delegate { panel.Visible = false; timer.Stop(); };
            return new BannerState { Panel = panel, Label = label, Timer = timer };
        }

        internal static string GetPrefix(NotificationKind kind)
        {
            switch (kind) {
                case NotificationKind.Success: return "Success: ";
                case NotificationKind.Warning: return "Notice: ";
                case NotificationKind.Error: return "Error: ";
                default: return String.Empty;
            }
        }

        internal static void GetColors(NotificationKind kind, bool dark, out Color back, out Color fore)
        {
            fore = dark ? Color.White : Color.FromArgb(25, 25, 25);
            if (dark) {
                switch (kind) {
                    case NotificationKind.Success: back = Color.FromArgb(42, 78, 52); break;
                    case NotificationKind.Warning: back = Color.FromArgb(94, 72, 25); break;
                    case NotificationKind.Error: back = Color.FromArgb(100, 45, 48); break;
                    default: back = Color.FromArgb(42, 72, 98); break;
                }
            } else {
                switch (kind) {
                    case NotificationKind.Success: back = Color.FromArgb(214, 238, 220); break;
                    case NotificationKind.Warning: back = Color.FromArgb(255, 238, 190); break;
                    case NotificationKind.Error: back = Color.FromArgb(250, 218, 220); break;
                    default: back = Color.FromArgb(215, 232, 248); break;
                }
            }
        }
    }
}
