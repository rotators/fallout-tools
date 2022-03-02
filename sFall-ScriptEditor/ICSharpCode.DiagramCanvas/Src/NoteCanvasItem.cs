/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:03
 */

using System;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;

using System.Xml;
using System.Xml.XPath;

using System.Globalization;
using System.Windows.Forms;

namespace ICSharpCode.ClassDiagram
{
	//TODO - complete note item
	public class NoteCanvasItem : CanvasItem
	{
		private string noteText = " Click here to text input...";
		private TextBox editBox;
		private bool editing = false;

		public NoteCanvasItem() { }

		public string Note
		{
			get { return noteText; }
			set { noteText = value; }
		}

		public override void DrawToGraphics (Graphics graphics)
		{
			if (graphics == null) return;

			// Draw Shadow
			graphics.FillRectangle(CanvasItem.ShadowBrush, X + ActualWidth, Y + 3, 4, ActualHeight);
			graphics.FillRectangle(CanvasItem.ShadowBrush, X + 4, Y + ActualHeight, ActualWidth - 4, 3);

			// Draw Note Area
			graphics.FillRectangle(Brushes.LightYellow, X, Y, ActualWidth, ActualHeight);
			graphics.DrawRectangle(Pens.Black, X, Y, ActualWidth, ActualHeight);

			// Draw Note Text
			RectangleF rect = new RectangleF (X + 5, Y + 5, ActualWidth - 10, ActualHeight - 10);
			graphics.DrawString(noteText, MessagesFont, Brushes.Black, rect);

			base.DrawToGraphics(graphics);
		}

		protected override bool DragAreaHitTest(PointF pos)
		{
			return (pos.X > X && pos.Y > Y && pos.X < X + ActualWidth && pos.Y < Y + ActualHeight);
		}

		public override Control GetEditingControl()
		{
			editBox.ForeColor = Color.Red;
			editBox.BackColor = Color.LightYellow;
			editBox.BorderStyle = BorderStyle.None;
			editBox.Multiline = true;

			editBox.Width = (int) Width - 8;
			editBox.Height = (int) Height - 6;
			editBox.Left = (int) AbsoluteX + 5;
			editBox.Top = (int) AbsoluteY + 4;

			editBox.KeyUp += editBox_KeyUp;

			return editBox;
		}

		void editBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape) {
				if (editBox.Parent != null)
					editBox.Parent.Controls.Remove(editBox);
				editBox.Dispose();
				editBox = null;
				IsEditing = false;
			}
		}

		public bool IsEditing { get; private set; }

		public override bool StartEditing()
		{
			editBox = new TextBox();
			if (editing) {
				editBox.Text = noteText;
				editBox.SelectionStart = noteText.Length;
			}

			return IsEditing = true;
		}

		public override void StopEditing()
		{
			if (editBox == null)
				return;

			if (editBox.Text.Length > 0) {
				noteText = editBox.Text;
				editing = true;
			}
			if (editBox.Parent != null)
				editBox.Parent.Controls.Remove(editBox);

			editBox.Dispose();
			editBox = null;
			IsEditing = false;
		}

		public override bool IsVResizable
		{
			get { return true; }
		}

		#region Storage
		protected override XmlElement CreateXmlElement(XmlDocument doc)
		{
			return doc.CreateElement("Note");
		}

		protected override void FillXmlElement(XmlElement element, XmlDocument document)
		{
			base.FillXmlElement(element, document);
			element.SetAttribute("NoteText", (editing) ? Note : "");
		}

		protected override void FillXmlPositionElement(XmlElement position, XmlDocument document)
		{
			base.FillXmlPositionElement(position, document);
			position.SetAttribute("Height", (Height/100).ToString(CultureInfo.InvariantCulture));
		}

		public override void LoadFromXml (XPathNavigator navigator)
		{
			base.LoadFromXml(navigator);
			string text = navigator.GetAttribute("NoteText", "");
			if (text != String.Empty) {
				Note = text;
				editing = true;
			}
		}

		protected override void ReadXmlPositionElement(XPathNavigator navigator)
		{
			base.ReadXmlPositionElement(navigator);
			Height = 100 * float.Parse(navigator.GetAttribute("Height", ""), CultureInfo.InvariantCulture);
		}

		#endregion

		#region Geometry
		public override float Width
		{
			set { base.Width = Math.Max (value, 80.0f); }
		}

		public override float Height
		{
			set { base.Height = Math.Max (value, 30.0f); }
		}
		#endregion
	}
}
