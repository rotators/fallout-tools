/* Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:03
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ICSharpCode.Diagrams.Drawables
{
	public class TextSegment : BaseRectangle, IDrawableRectangle, IDisposable
	{
		float tw;
		float th;

		Font font;
		Brush brush = Brushes.Black;
		Graphics g;
		string text;

		int index;  // индекс строки кода в ContentBody
		bool isWrap = false;

		StringFormat sf = new StringFormat();

		public TextSegment (Graphics graphics, string text)
			: this (graphics, text, new Font(System.Drawing.FontFamily.GenericSansSerif, 10.0f), false)
		{
		}

		public TextSegment (Graphics graphics, string text, int index, Font font, bool resizable, bool isWrap)
			: this (graphics, text, index, font, resizable, StringAlignment.Near)
		{
			if (isWrap) sf.FormatFlags &= ~StringFormatFlags.NoWrap;
			this.isWrap = isWrap;
		}

		public TextSegment (Graphics graphics, string text, Font font, bool resizable, StringAlignment alignment = StringAlignment.Near)
			: this (graphics, text, -1, font, resizable, alignment)
		{
		}

		public TextSegment (Graphics graphics, string text, int index, Font font, bool resizable, StringAlignment alignment = StringAlignment.Near)
		{
			//if (graphics == null) throw new ArgumentNullException("graphics");
			this.g = graphics;
			this.text = text;
			this.index = index;
			this.font = font;
			sf.Trimming = StringTrimming.EllipsisCharacter;
			sf.Alignment = alignment;
			sf.LineAlignment = StringAlignment.Center;
			sf.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.FitBlackBox;
			MeasureString();
			if (resizable)
				Width = -1;
			else
				Width = float.NaN;
		}

		private void MeasureString ()
		{
			if (text != null && font != null && g != null) {
				SizeF s = g.MeasureString(text, font);
				th = s.Height;
				tw = s.Width;
			} else {
				tw = th = float.NaN;
			}
		}

		public override float Width
		{
			get
			{
				if (float.IsNaN(base.Width)) return tw;
				return base.Width;
			}
			set { base.Width = value; }
		}

		public int IndexContent
		{
			get { return index; }
		}

		public float TextWidth
		{
			get { return tw; }
		}

		public float TextHeight
		{
			get { return th; }
		}

		public override float Height
		{
			get { return (this.isWrap) ? (th * 3.0f) + (font.Size / 2) : font.Size * 1.3f; }
		}

		public override float ActualHeight
		{
			get { return Height; }
			set { base.ActualHeight = value; }
		}

		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				MeasureString();
			}
		}

		public Font Font
		{
			get { return font; }
			set
			{
				font = value;
				MeasureString();
			}
		}

		public Brush Brush
		{
			get { return brush; }
			set { brush = value; }
		}

		public void DrawToGraphics (Graphics graphics)
		{
			if (graphics == null) return;
			RectangleF rect = new RectangleF(AbsoluteX, AbsoluteY, ActualWidth, ActualHeight);
			graphics.DrawString(Text, Font, Brush, rect, sf);
		}

		public void Dispose()
		{
			brush.Dispose();
			sf.Dispose();
		}

		public override float GetAbsoluteContentWidth()
		{
			return TextWidth + 20;
		}

		public override float GetAbsoluteContentHeight()
		{
			return Height;
		}

		public int GetTextLines()
		{
			if (!isWrap) return 0;

			int chars, lines;
			g.MeasureString(text, font, new SizeF(ActualWidth, ActualHeight), sf, out chars, out lines);
			return lines;
		}
	}
}
