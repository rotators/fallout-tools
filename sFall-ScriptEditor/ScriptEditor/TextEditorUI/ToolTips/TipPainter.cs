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
            using (LinearGradientBrush gradientDefault = new LinearGradientBrush(e.Bounds,
                Color.White, Color.Lavender, LinearGradientMode.Vertical))
            using (Pen lightBorder = new Pen(Color.Gray))
            using (Pen darkBorder = new Pen(Color.DarkGray)) {
                e.Graphics.FillRectangle(gradientDefault, e.Bounds);
                e.Graphics.DrawLines(lightBorder, new Point[] {
                    new Point(0, e.Bounds.Height - 1),
                    new Point(e.Bounds.Width - 1, e.Bounds.Height - 1),
                    new Point(e.Bounds.Width - 1, 0)
                });
                e.Graphics.DrawLines(darkBorder, new Point[] {
                    new Point(0, e.Bounds.Height - 1),
                    new Point(0, 0),
                    new Point(e.Bounds.Width - 1, 0)
                });
                e.DrawText(sff);
            }
        }

        public static void DrawSizeMessage(DrawToolTipEventArgs e)
        {
            using (Font font = new Font("Arial", 12.0F, FontStyle.Regular, GraphicsUnit.Point)) {
                DrawToolTipEventArgs args = new DrawToolTipEventArgs(e.Graphics, e.AssociatedWindow, e.AssociatedControl,
                    e.Bounds, e.ToolTipText, Color.LightYellow, Color.Black, font);
                DrawMessage(args);
            }
        }

        public static void DrawInfo(DrawToolTipEventArgs e)
        {
            using (LinearGradientBrush gradientInfo = new LinearGradientBrush(e.Bounds,
                ColorTheme.TipGradient.Color, ColorTheme.TipGradient.BackgroundColor, LinearGradientMode.Vertical)) {
                e.Graphics.FillRectangle(gradientInfo, e.Bounds);

                if (ColorTheme.IsDarkTheme) {
                    Rectangle border = new Rectangle(e.Bounds.Location, new Size(e.Bounds.Width - 1, e.Bounds.Height - 1));
                    using (Pen borderPen = new Pen(ColorTheme.TipBorderFrame))
                        e.Graphics.DrawRectangle(borderPen, border);

                    Point locationText = e.Bounds.Location;
                    locationText.Offset(DpiHelper.Scale(3, (int)e.Graphics.DpiX),
                        DpiHelper.Scale(1, (int)e.Graphics.DpiY));
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    if (e.Font.Size > 11.5f) {
                        using (Font font = new Font(e.Font.FontFamily, 11.5f, FontStyle.Regular, GraphicsUnit.Pixel))
                            e.Graphics.DrawString(e.ToolTipText, font, ColorTheme.TipText, locationText, sf);
                    } else {
                        e.Graphics.DrawString(e.ToolTipText, e.Font, ColorTheme.TipText, locationText, sf);
                    }
                } else {
                    e.DrawBorder();
                    e.DrawText(sff);
                }
            }
        }

    }
}
