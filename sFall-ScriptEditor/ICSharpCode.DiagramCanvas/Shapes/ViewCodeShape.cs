using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ICSharpCode.ClassDiagram
{
	public class ViewCodeShape : SmallButtonShape
	{
		static Pen strokeOn  = Pens.Black;
		static Pen strokeOff = Pens.DimGray;

		static Point[] lines = new Point[]
		{
			new Point(1, 2), new Point(7, 2),
			new Point(1, 4), new Point(5, 4),
			new Point(1, 6), new Point(8, 6),
			new Point(1, 8), new Point(6, 8)
		};

		static Point[] cross = new Point[]
		{
			new Point(2, 2), new Point(8, 8),
			new Point(2, 8), new Point(8, 2),
		};

		public bool IsPressed { set; get; }

		public ViewCodeShape(bool pressed)
		{
			IsPressed = pressed;
		}

		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			base.Draw(graphics);

			Pen pen = (IsPressed) ? strokeOn : strokeOff;
			for (int i = 0; i < lines.Length; i += 2)
			{
				graphics.DrawLine(pen, lines[i], lines[i + 1]);
			}
			if (!IsPressed) {
				graphics.DrawLine(Pens.Red, cross[0], cross[1]);
				graphics.DrawLine(Pens.Red, cross[2], cross[3]);
			}
		}

		public override float ShapeWidth
		{
			get { return 20.0f; }
		}

		public override float ShapeHeight
		{
			get { return 20.0f; }
		}
	}
}
