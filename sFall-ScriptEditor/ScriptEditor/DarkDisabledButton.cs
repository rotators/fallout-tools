using System.Drawing;
using System.Windows.Forms;

namespace ScriptEditor
{
    [System.ComponentModel.ToolboxItem(false)]
    public sealed class DarkDisabledButton : Button
    {
        internal static void PaintDisabledButton(object sender, PaintEventArgs e)
        {
            Button button = sender as Button;
            if (button == null || !InterfaceTheme.IsDark)
                return;

            bool mouseOver = button.Enabled && button.ClientRectangle.Contains(button.PointToClient(Cursor.Position));
            bool pressed = mouseOver && button.Capture && MouseButtons == MouseButtons.Left;
            Color backColor = !button.Enabled ? Color.FromArgb(53, 53, 56)
                : pressed ? Color.FromArgb(40, 40, 42)
                : mouseOver ? Color.FromArgb(85, 85, 90)
                : Color.FromArgb(53, 53, 56);
            Color textColor = button.Enabled ? Color.Gainsboro : Color.FromArgb(170, 170, 175);
            Color borderColor = Color.FromArgb(68, 68, 72);
            e.Graphics.Clear(backColor);
            ControlPaint.DrawBorder(e.Graphics, button.ClientRectangle, borderColor, ButtonBorderStyle.Solid);

            Size textSize = TextRenderer.MeasureText(button.Text, button.Font, Size.Empty, TextFormatFlags.NoPadding);
            int imageWidth = button.Image == null ? 0 : button.Image.Width;
            int spacing = button.Image == null || button.Text.Length == 0 ? 0 : DpiHelper.Scale(button, 4);
            int contentWidth = imageWidth + spacing + textSize.Width;
            int edge = DpiHelper.Scale(button, 2);
            int x = System.Math.Max(edge, (button.ClientSize.Width - contentWidth) / 2);
            if (button.Image != null) {
                int imageY = (button.ClientSize.Height - button.Image.Height) / 2;
                if (button.Enabled)
                    e.Graphics.DrawImage(button.Image, x, imageY, button.Image.Width, button.Image.Height);
                else
                    ControlPaint.DrawImageDisabled(e.Graphics, button.Image, x, imageY, backColor);
                x += imageWidth + spacing;
            }

            Rectangle textBounds = new Rectangle(x, 0,
                System.Math.Max(0, button.ClientSize.Width - x - edge), button.ClientSize.Height);
            TextRenderer.DrawText(e.Graphics, button.Text, button.Font, textBounds, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!InterfaceTheme.IsDark) {
                base.OnPaint(e);
                return;
            }

            bool mouseOver = Enabled && ClientRectangle.Contains(PointToClient(Cursor.Position));
            bool pressed = mouseOver && Capture && MouseButtons == MouseButtons.Left;
            Color backColor = pressed ? Color.FromArgb(40, 40, 42)
                : mouseOver ? Color.FromArgb(85, 85, 90)
                : Color.FromArgb(53, 53, 56);
            Color textColor = Enabled ? Color.Gainsboro : Color.FromArgb(170, 170, 175);
            Color borderColor = Color.FromArgb(68, 68, 72);
            e.Graphics.Clear(backColor);
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, borderColor, ButtonBorderStyle.Solid);

            Size textSize = TextRenderer.MeasureText(Text, Font, Size.Empty, TextFormatFlags.NoPadding);
            int imageWidth = Image == null ? 0 : Image.Width;
            int spacing = Image == null || Text.Length == 0 ? 0 : DpiHelper.Scale(this, 4);
            int contentWidth = imageWidth + spacing + textSize.Width;
            int edge = DpiHelper.Scale(this, 2);
            int x = System.Math.Max(edge, (ClientSize.Width - contentWidth) / 2);
            if (Image != null) {
                int imageY = (ClientSize.Height - Image.Height) / 2;
                if (Enabled)
                    e.Graphics.DrawImage(Image, x, imageY, Image.Width, Image.Height);
                else
                    ControlPaint.DrawImageDisabled(e.Graphics, Image, x, imageY, backColor);
                x += imageWidth + spacing;
            }
            Rectangle textBounds = new Rectangle(x, 0,
                System.Math.Max(0, ClientSize.Width - x - edge), ClientSize.Height);
            TextRenderer.DrawText(e.Graphics, Text, Font, textBounds, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);

            if (Enabled && Focused && ShowFocusCues) {
                Rectangle focusBounds = ClientRectangle;
                int focusInset = DpiHelper.Scale(this, 3);
                focusBounds.Inflate(-focusInset, -focusInset);
                ControlPaint.DrawFocusRectangle(e.Graphics, focusBounds, textColor, backColor);
            }
        }
    }
}
