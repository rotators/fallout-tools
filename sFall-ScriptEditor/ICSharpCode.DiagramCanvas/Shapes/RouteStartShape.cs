/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 11/9/2006
 * Time: 4:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using ICSharpCode.Diagrams;

namespace ICSharpCode.ClassDiagram
{
	/// <summary>
	/// Description of RouteStartShape.
	/// </summary>
	public class RouteStartShape : RouteShape
	{
        Pen stroke = new Pen(Color.FromArgb(128, 0, 0, 0), 2);
        Brush fill = new SolidBrush(Color.FromArgb(200, 0, 0, 0));

		public RouteStartShape() { }

		public RouteStartShape(Color stroke)
		{
			this.stroke = new Pen(stroke, 2);
		}

		public RouteStartShape(Color stroke, Color fill)
		{
			this.stroke = new Pen(stroke, 2);
			this.fill = new SolidBrush(fill);
		}

		protected override void Paint (Graphics graphics)
		{
            graphics.FillEllipse(fill, -2.8f, 6, 6, 6);
            graphics.DrawLine(stroke, 0, 0, 0, 6.25f);
		}
	}
}
