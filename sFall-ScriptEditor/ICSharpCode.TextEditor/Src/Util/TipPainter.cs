// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ICSharpCode.TextEditor.Util
{
	static class TipPainter
	{
		const float HorizontalBorder = 12;
		const float VerticalBorder   = 8;
		const float LogicalDpi       = 96;

		static float ScaleX(Graphics graphics, float value)
		{
			return value * graphics.DpiX / LogicalDpi;
		}

		static float ScaleY(Graphics graphics, float value)
		{
			return value * graphics.DpiY / LogicalDpi;
		}
		
		internal static bool darkScheme = false;
		
        //static Color tipTextColor = Color.Black;
		public static Color TipTextColor
		{
			get { return darkScheme ? Color.FromArgb(242, 242, 242) : Color.FromArgb(32, 32, 32); }
		}

		public static Color TipSecondaryTextColor {
			get { return darkScheme ? Color.FromArgb(205, 205, 210) : Color.FromArgb(65, 65, 65); }
		}

		public static Color TipBackgroundColor {
			get { return darkScheme ? Color.FromArgb(37, 37, 38) : Color.FromArgb(252, 252, 252); }
		}

		public static Color TipBorderColor {
			get { return darkScheme ? Color.FromArgb(86, 86, 91) : Color.FromArgb(190, 190, 190); }
		}

		public static Color TipAccentColor {
			get { return darkScheme ? Color.FromArgb(55, 148, 255) : Color.FromArgb(0, 120, 212); }
		}

		//static StringFormat centerTipFormat = CreateTipStringFormat();
		
		public static Size GetTipSize(Control control, Graphics graphics, Font font, string description)
		{
			return GetTipSize(control, graphics, new TipText (graphics, font, description));
		}
		
		static Rectangle GetWorkingArea(Control control)
		{
			Form ownerForm = control.FindForm();
			if (ownerForm.Owner != null) {
				ownerForm = ownerForm.Owner;
			}
			
			return Screen.GetWorkingArea(ownerForm);
		}
		
		public static Size GetTipSize(Control control, Graphics graphics, TipSection tipData)
		{
			Size tipSize = Size.Empty;
			SizeF tipSizeF = SizeF.Empty;
			
			RectangleF workingArea = GetWorkingArea(control);
			
			PointF screenLocation = control.PointToScreen(Point.Empty);
			
			float horizontalBorder = ScaleX(graphics, HorizontalBorder);
			float verticalBorder = ScaleY(graphics, VerticalBorder);
			SizeF maxLayoutSize = new SizeF(workingArea.Right - screenLocation.X - horizontalBorder * 2,
			                                workingArea.Bottom - screenLocation.Y - verticalBorder * 2);
			
			if (maxLayoutSize.Width > 0 && maxLayoutSize.Height > 0) {
				graphics.TextRenderingHint =
					TextRenderingHint.AntiAliasGridFit;
				
				tipData.SetMaximumSize(maxLayoutSize);
				tipSizeF = tipData.GetRequiredSize();
				tipData.SetAllocatedSize(tipSizeF);
				
				tipSizeF += new SizeF(horizontalBorder * 2, verticalBorder * 2);
				tipSize = Size.Ceiling(tipSizeF);
			}
			
			if (control.ClientSize != tipSize) {
				control.ClientSize = tipSize;
			}
			
			return tipSize;
		}
		
		public static Size GetLeftHandSideTipSize(Control control, Graphics graphics, TipSection tipData, Point p)
		{
			Size tipSize = Size.Empty;
			SizeF tipSizeF = SizeF.Empty;
			
			RectangleF workingArea = GetWorkingArea(control);
			
			PointF screenLocation = p;
			
			float horizontalBorder = ScaleX(graphics, HorizontalBorder);
			float verticalBorder = ScaleY(graphics, VerticalBorder);
			SizeF maxLayoutSize = new SizeF(screenLocation.X - horizontalBorder * 2,
			                                workingArea.Bottom - screenLocation.Y - verticalBorder * 2);
			
			if (maxLayoutSize.Width > 0 && maxLayoutSize.Height > 0) {
				graphics.TextRenderingHint =
					TextRenderingHint.AntiAliasGridFit;
				
				tipData.SetMaximumSize(maxLayoutSize);
				tipSizeF = tipData.GetRequiredSize();
				tipData.SetAllocatedSize(tipSizeF);
				
				tipSizeF += new SizeF(horizontalBorder * 2, verticalBorder * 2);
				tipSize = Size.Ceiling(tipSizeF);
			}
			
			return tipSize;
		}
		
		public static Size DrawTip(Control control, Graphics graphics, Font font, string description)
		{
			return DrawTip(control, graphics, new TipText (graphics, font, description));
		}
		
		public static Size DrawTip(Control control, Graphics graphics, TipSection tipData)
		{
			Size tipSize = Size.Empty;
			SizeF tipSizeF = SizeF.Empty;
			
			PointF screenLocation = control.PointToScreen(Point.Empty);
			
			RectangleF workingArea = GetWorkingArea(control);
			
			float horizontalBorder = ScaleX(graphics, HorizontalBorder);
			float verticalBorder = ScaleY(graphics, VerticalBorder);
			SizeF maxLayoutSize = new SizeF(workingArea.Right - screenLocation.X - horizontalBorder * 2,
			                                workingArea.Bottom - screenLocation.Y - verticalBorder * 2);
			
			if (maxLayoutSize.Width > 0 && maxLayoutSize.Height > 0) {
				graphics.TextRenderingHint =
					TextRenderingHint.ClearTypeGridFit;
				graphics.TextContrast = 0;

				tipData.SetMaximumSize(maxLayoutSize);
				tipSizeF = tipData.GetRequiredSize();
				tipData.SetAllocatedSize(tipSizeF);
				
				tipSizeF += new SizeF(horizontalBorder * 2, verticalBorder * 2);
				tipSize = Size.Ceiling(tipSizeF);
			}
			
			if (control.ClientSize != tipSize) {
				control.ClientSize = tipSize;
			}
			
			if (tipSize != Size.Empty) {
				Rectangle borderRectangle = new Rectangle
					(Point.Empty, tipSize - new Size(1, 1));
				
				RectangleF displayRectangle = new RectangleF
					(horizontalBorder, verticalBorder,
					 tipSizeF.Width - horizontalBorder * 2,
					 tipSizeF.Height - verticalBorder * 2);
				
				using (Pen borderFrame = new Pen(TipBorderColor))
					graphics.DrawRectangle(borderFrame, borderRectangle);
				using (Brush accentBrush = new SolidBrush(TipAccentColor))
					graphics.FillRectangle(accentBrush, ScaleX(graphics, 1), ScaleY(graphics, 1),
						ScaleX(graphics, 3), System.Math.Max(0, tipSize.Height - ScaleY(graphics, 2)));
				tipData.Draw(new PointF(horizontalBorder, verticalBorder));
			}
			return tipSize;
		}
		
		public static Size DrawFixedWidthTip(Control control, Graphics graphics, TipSection tipData)
		{
			Size tipSize = Size.Empty;
			SizeF tipSizeF = SizeF.Empty;
			
			PointF screenLocation = control.PointToScreen(new Point(control.Width, 0));
			
			RectangleF workingArea = GetWorkingArea(control);
			
			float horizontalBorder = ScaleX(graphics, HorizontalBorder);
			float verticalBorder = ScaleY(graphics, VerticalBorder);
			SizeF maxLayoutSize = new SizeF(screenLocation.X - horizontalBorder * 2,
			                                workingArea.Bottom - screenLocation.Y - verticalBorder * 2);
			
			if (maxLayoutSize.Width > 0 && maxLayoutSize.Height > 0) {
				graphics.TextRenderingHint =
					TextRenderingHint.AntiAliasGridFit;
				
				tipData.SetMaximumSize(maxLayoutSize);
				tipSizeF = tipData.GetRequiredSize();
				tipData.SetAllocatedSize(tipSizeF);
				
				tipSizeF += new SizeF(horizontalBorder * 2, verticalBorder * 2);
				tipSize = Size.Ceiling(tipSizeF);
			}
			
			if (control.Height != tipSize.Height) {
				control.Height = tipSize.Height;
			}
			
			if (tipSize != Size.Empty) {
				Rectangle borderRectangle = new Rectangle
					(Point.Empty, control.Size - new Size(1, 1));
				
				RectangleF displayRectangle = new RectangleF
					(horizontalBorder, verticalBorder,
					 tipSizeF.Width - horizontalBorder * 2,
					 tipSizeF.Height - verticalBorder * 2);
				
				// DrawRectangle draws from Left to Left + Width. A bug? :-/
				using (Pen borderFrame = new Pen(TipBorderColor))
					graphics.DrawRectangle(borderFrame, borderRectangle);
				using (Brush accentBrush = new SolidBrush(TipAccentColor))
					graphics.FillRectangle(accentBrush, ScaleX(graphics, 1), ScaleY(graphics, 1),
						ScaleX(graphics, 3), System.Math.Max(0, control.Height - ScaleY(graphics, 2)));
				tipData.Draw(new PointF(horizontalBorder, verticalBorder));
			}
			return tipSize;
		}
	}
}
