using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScriptEditor
{
    internal static class ThemedMessageBox
    {
        internal static DialogResult Show(string text)
        {
            return Show(null, text, null, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        internal static DialogResult Show(string text, string caption)
        {
            return Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        internal static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return Show(null, text, caption, buttons, MessageBoxIcon.None);
        }

        internal static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return Show(null, text, caption, buttons, icon);
        }

        internal static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            using (Form dialog = new Form()) {
                dialog.Text = String.IsNullOrEmpty(caption) ? Application.ProductName : caption;
                dialog.AutoScaleMode = AutoScaleMode.Dpi;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.ShowIcon = false;
                dialog.ShowInTaskbar = false;
                dialog.StartPosition = owner == null ? FormStartPosition.CenterScreen : FormStartPosition.CenterParent;
                dialog.Padding = new Padding(14);
                Size messageSize = TextRenderer.MeasureText(text ?? String.Empty, SystemFonts.MessageBoxFont,
                    new Size(380, Int32.MaxValue), TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);
                dialog.ClientSize = new Size(450, Math.Max(118, messageSize.Height + 72));

                TableLayoutPanel content = new TableLayoutPanel();
                content.Dock = DockStyle.Fill;
                content.ColumnCount = icon == MessageBoxIcon.None ? 1 : 2;
                content.RowCount = 2;
                content.ColumnStyles.Add(new ColumnStyle(icon == MessageBoxIcon.None ? SizeType.Percent : SizeType.AutoSize,
                    icon == MessageBoxIcon.None ? 100F : 0F));
                if (icon != MessageBoxIcon.None)
                    content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                content.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                content.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                if (icon != MessageBoxIcon.None) {
                    PictureBox picture = new PictureBox();
                    picture.Image = GetIcon(icon).ToBitmap();
                    picture.Size = new Size(32, 32);
                    picture.Margin = new Padding(0, 3, 12, 0);
                    content.Controls.Add(picture, 0, 0);
                }

                Label message = new Label();
                message.AutoSize = true;
                message.Dock = DockStyle.Fill;
                message.MaximumSize = new Size(380, 0);
                message.Text = text ?? String.Empty;
                message.Margin = new Padding(0, 3, 0, 12);
                content.Controls.Add(message, icon == MessageBoxIcon.None ? 0 : 1, 0);

                FlowLayoutPanel buttonsPanel = new FlowLayoutPanel();
                buttonsPanel.AutoSize = true;
                buttonsPanel.Dock = DockStyle.Right;
                buttonsPanel.FlowDirection = FlowDirection.RightToLeft;
                buttonsPanel.WrapContents = false;
                buttonsPanel.Margin = Padding.Empty;
                AddButtons(dialog, buttonsPanel, buttons);
                content.SetColumnSpan(buttonsPanel, content.ColumnCount);
                content.Controls.Add(buttonsPanel, 0, 1);

                dialog.Controls.Add(content);
                InterfaceTheme.ApplyOnLoad(dialog);
                return owner == null ? dialog.ShowDialog() : dialog.ShowDialog(owner);
            }
        }

        private static void AddButtons(Form dialog, FlowLayoutPanel panel, MessageBoxButtons buttons)
        {
            switch (buttons) {
                case MessageBoxButtons.OKCancel:
                    AddButton(dialog, panel, "Cancel", DialogResult.Cancel, true);
                    AddButton(dialog, panel, "OK", DialogResult.OK, false);
                    break;
                case MessageBoxButtons.YesNo:
                    AddButton(dialog, panel, "No", DialogResult.No, true);
                    AddButton(dialog, panel, "Yes", DialogResult.Yes, false);
                    break;
                case MessageBoxButtons.YesNoCancel:
                    AddButton(dialog, panel, "Cancel", DialogResult.Cancel, true);
                    AddButton(dialog, panel, "No", DialogResult.No, false);
                    AddButton(dialog, panel, "Yes", DialogResult.Yes, false);
                    break;
                case MessageBoxButtons.RetryCancel:
                    AddButton(dialog, panel, "Cancel", DialogResult.Cancel, true);
                    AddButton(dialog, panel, "Retry", DialogResult.Retry, false);
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    AddButton(dialog, panel, "Ignore", DialogResult.Ignore, false);
                    AddButton(dialog, panel, "Retry", DialogResult.Retry, false);
                    AddButton(dialog, panel, "Abort", DialogResult.Abort, true);
                    break;
                default:
                    AddButton(dialog, panel, "OK", DialogResult.OK, true);
                    break;
            }
        }

        private static void AddButton(Form dialog, FlowLayoutPanel panel, string text, DialogResult result, bool cancel)
        {
            Button button = new Button();
            button.Text = text;
            button.DialogResult = result;
            button.Size = new Size(82, 26);
            panel.Controls.Add(button);
            if (cancel)
                dialog.CancelButton = button;
            else if (dialog.AcceptButton == null)
                dialog.AcceptButton = button;
        }

        private static Icon GetIcon(MessageBoxIcon icon)
        {
            switch (icon) {
                case MessageBoxIcon.Error:
                    return SystemIcons.Error;
                case MessageBoxIcon.Warning:
                    return SystemIcons.Warning;
                case MessageBoxIcon.Information:
                    return SystemIcons.Information;
                case MessageBoxIcon.Question:
                    return SystemIcons.Question;
                default:
                    return SystemIcons.Application;
            }
        }
    }
}
