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
	public class OptionsCircleShape : VectorShape
	{
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
            graphics.FillEllipse(Brushes.Black, 1.2f, 0.6f, 1.5f, 1.5f);
		}
		
		public override float ShapeWidth
		{
			get { return 4.0f; }
		}
		
		public override float ShapeHeight
		{
			get { return 3.0f; }
		}
	}
}
