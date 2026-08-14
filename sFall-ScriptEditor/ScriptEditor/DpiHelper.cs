using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScriptEditor
{
    internal static class DpiHelper
    {
        internal const int LogicalDpi = 96;

        internal static int Scale(int logicalPixels, int dpi)
        {
            if (dpi <= 0) dpi = LogicalDpi;
            return (int)Math.Round(logicalPixels * dpi / (double)LogicalDpi,
                MidpointRounding.AwayFromZero);
        }

        internal static float Scale(float logicalPixels, float dpi)
        {
            if (dpi <= 0) dpi = LogicalDpi;
            return logicalPixels * dpi / LogicalDpi;
        }

        internal static int Scale(Control control, int logicalPixels)
        {
            return Scale(logicalPixels, control == null ? LogicalDpi : control.DeviceDpi);
        }

        internal static Size Scale(Control control, Size logicalSize)
        {
            return new Size(Scale(control, logicalSize.Width), Scale(control, logicalSize.Height));
        }

        internal static Point Scale(Control control, Point logicalPoint)
        {
            return new Point(Scale(control, logicalPoint.X), Scale(control, logicalPoint.Y));
        }

        internal static Padding Scale(Control control, Padding logicalPadding)
        {
            return new Padding(Scale(control, logicalPadding.Left), Scale(control, logicalPadding.Top),
                Scale(control, logicalPadding.Right), Scale(control, logicalPadding.Bottom));
        }
    }
}
