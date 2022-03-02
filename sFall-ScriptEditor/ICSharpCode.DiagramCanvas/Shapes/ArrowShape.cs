/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ICSharpCode.ClassDiagram
{
	public class ArrowShape : VectorShape
	{
		static GraphicsPath path = InitializePath();
		
		static GraphicsPath InitializePath ()
		{
			GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddPolygon(new PointF[]{
                //          x     y
                new PointF(1.0f, -0.25f),
                new PointF(1.5f, -0.25f),
                new PointF(1.5f, 1.5f),

                new PointF(2.5f, 1.5f),

                new PointF(2.5f, 0.75f),
                new PointF(4.0f, 1.75f),
                new PointF(2.5f, 2.75f),

                new PointF(2.5f, 2.0f),
                new PointF(1.0f, 2.0f)
            });
            path.CloseFigure();

            path.StartFigure();
            path.AddPolygon(new PointF[]{
                new PointF(2.75f, 1.25f),
                new PointF(3.5f, 1.75f),
                new PointF(2.75f, 2.25f)
            });
            path.FillMode = FillMode.Alternate;
            path.CloseFigure();
			return path;
		}
		
		public override float ShapeWidth
		{
			get { return 5.0f; }
		}
		
		public override float ShapeHeight
		{
			get { return 4.0f; }
		}
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			graphics.TranslateTransform(0.5f, 0.5f);
			graphics.FillPath(Brushes.Black, path);
		}
	}
}
