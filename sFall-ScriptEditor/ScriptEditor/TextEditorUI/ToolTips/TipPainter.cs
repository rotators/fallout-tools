using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScriptEditor.TextEditorUI.ToolTips
{
    static class TipPainter
    {
        // Specify custom text formatting flags
        static TextFormatFlags sff = TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
        static StringFormat sf = StringFormat.GenericTypographic;

        public static void DrawMessage(DrawToolTipEventArgs e)
        {
            LinearGradientBrush gradientDefault = new LinearGradientBrush(e.Bounds,
                                                        Color.White, Color.Lavender,
                                                        LinearGradientMode.Vertical);

            // Draw the custom background
            e.Graphics.FillRectangle(gradientDefault, e.Bounds);

            // Draw the custom border to appear 3-dimensional
            e.Graphics.DrawLines(new Pen(Color.Gray), new Point[] {
                new Point (0, e.Bounds.Height - 1),
                new Point (e.Bounds.Width - 1, e.Bounds.Height - 1),
                new Point (e.Bounds.Width - 1, 0)
            });
            e.Graphics.DrawLines(new Pen(Color.DarkGray), new Point[] {
                new Point (0, e.Bounds.Height - 1),
                new Point (0, 0),
                new Point (e.Bounds.Width - 1, 0)
            });

            // Draw the standard text with customized formatting options
            e.DrawText(sff);
        }

        public static void DrawSizeMessage(DrawToolTipEventArgs e)
        {
            DrawToolTipEventArgs args = new DrawToolTipEventArgs(e.Graphics, e.AssociatedWindow, e.AssociatedControl, e.Bounds, e.ToolTipText, Color.LightYellow, Color.Black,
                new Font("Arial", 12.0F, FontStyle.Regular, GraphicsUnit.Point));
            DrawMessage(args);
        }

        public static void DrawInfo(DrawToolTipEventArgs e)
        {
            LinearGradientBrush gradientInfo = new LinearGradientBrush(e.Bounds,
                                                    ColorTheme.TipGradient.Color,
                                                    ColorTheme.TipGradient.BackgroundColor,
                                                    LinearGradientMode.Vertical);
            // Draw the custom background
            e.Graphics.FillRectangle(gradientInfo, e.Bounds);

            if (ColorTheme.IsDarkTheme) {
                Rectangle border = new Rectangle(e.Bounds.Location, new Size(e.Bounds.Width - 1, e.Bounds.Height - 1));
                e.Graphics.DrawRectangle(new Pen(ColorTheme.TipBorderFrame), border);

                Point locationText = e.Bounds.Location;
                locationText.Offset(3, 1);
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                Font font;
                if (e.Font.Size > 11.5f) {
                    font = new Font(e.Font.FontFamily, 11.5f, FontStyle.Regular, GraphicsUnit.Pixel);
                } else {
                    font = e.Font;
                }
                e.Graphics.DrawString(e.ToolTipText, font, ColorTheme.TipText, locationText, sf);
            } else {
               e.DrawBorder();
               // Draw the standard text with customized formatting options
               e.DrawText(sff);
            }
        }

    }
}
