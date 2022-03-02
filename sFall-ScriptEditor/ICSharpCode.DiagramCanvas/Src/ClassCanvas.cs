// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Itai Bar-Haim" email=""/>
//     <version>$Revision: 2231 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

using System.Xml;
using System.Xml.XPath;

using ICSharpCode.Diagrams;
using ICSharpCode.Diagrams.Drawables;

namespace ICSharpCode.ClassDiagram
{
	public partial class ClassCanvas
	{
		private class CanvasItemData : IDisposable
		{
			public CanvasItemData (CanvasItem item,
			                       EventHandler<SizeGripEventArgs> SizeGripMouseEntered,
			                       EventHandler<SizeGripEventArgs> SizeGripMouseLeft)
			{
				this.item = item;
				
				focusDecorator = new FocusDecorator(item);
				sizeGripDecorator = new SizeGripDecorator(item);
				
				sizeGripDecorator.SizeGripMouseEnter += SizeGripMouseEntered;
				sizeGripDecorator.SizeGripMouseLeave += SizeGripMouseLeft;
				
				item.AddDecorator(focusDecorator);
				item.AddDecorator(sizeGripDecorator);
			}
			
			CanvasItem item;
			FocusDecorator focusDecorator;
			SizeGripDecorator sizeGripDecorator;
			
			public CanvasItem Item
			{
				get { return item; }
			}
			
			public bool Focused
			{
				get { return focusDecorator.Active; }
				set
				{
					focusDecorator.Active = value;
					sizeGripDecorator.Active = value;
				}
			}
			
			bool justGainedFocus;
			public bool JustGainedFocus
			{
				get { return justGainedFocus; }
				set { justGainedFocus = value; }
			}
			
			public void Dispose()
			{
				item.RemoveDecorator(focusDecorator);
				item.RemoveDecorator(sizeGripDecorator);
			}
		}
		
		List<LinkedListNode<CanvasItemData>> dragItemNode = new List<LinkedListNode<CanvasItemData>>();
		LinkedListNode<CanvasItemData> hoverItemNode;

		LinkedList<CanvasItemData> itemsList = new LinkedList<CanvasItemData>();

		Dictionary<CanvasItem, CanvasItemData> itemsData = new Dictionary<CanvasItem, CanvasItemData>();
		Dictionary<INode, CanvasItem> nodesToData = new Dictionary<INode, CanvasItem>();

		DiagramRouter diagramRouter = new DiagramRouter();
		
		public event EventHandler ZoomChanged = delegate { };

		readonly int pictureHWSize = 11100;

		float zoom = 1.0f;
		bool ctrlDown;
		bool holdRedraw;
		bool redrawNeeded;
		bool focused;
		bool highQuality = true;
		bool wireOnlySelect;

		PointF lastMouseClickPosition;
		PointF lastMouseDownPosition;

		Point locationCursor;
		Point locationScrollLast;

		PointF percent;
		#if DEBUG
			public PointF Percent
			{
				get { return percent; }
			}
		#endif

		public bool CtrlDown
		{
			set { ctrlDown = value; }
		}

		/// <summary>
		/// Sets location of auto-scroll position. 
		/// </summary>
		public Point SetCanvasScrollPosition
		{
			set {
				this.AutoScrollPosition = value;
				this.OnScroll(null);
			}
		}

		private LinkedListNode<CanvasItemData> HoverItemNode
		{
			get { return hoverItemNode; }
			set { 
				hoverItemNode = value;
				
				CanvasItem item = null;
				if (hoverItemNode != null)
					item = hoverItemNode.Value.Item;
				CanvasItemHover (this, new CanvasItemEventArgs (item));
			}
		}

		public bool HighQuality
		{
			set { highQuality = value; }
		}

		public bool WireOnlySelect
		{
			set { wireOnlySelect = value; }
		}

		public int NodesTotalCount
		{
			get { return itemsList.Count; }
		}

		// for last item
		public bool NodeIsSelected
		{
			get {
				if (itemsList.Last.Value.Item is NodeCanvasItem)
					return itemsList.Last.Value.Focused;
				else
					return false;
			}
		}

		public static bool NodeLowDetails { set; private get; }

		public ClassCanvas()
		{
			InitializeComponent();

			// Set picture size
			CanvasPicture.Width = pictureHWSize;
			CanvasPicture.Height = pictureHWSize;
		}
		
		#region Diagram Activities
		public float Zoom
		{
			get { return zoom; }
			set
			{
				if (IsEditingNote()) {
					ZoomChanged(this, null); // используется для предотвращения маштабирования во время редактирования
					return;
				}
				float zoomDiff = value - zoom;
				if (zoomDiff == 0)
					return;

				zoom = value;
				bool ZoomIn = (zoomDiff > 0);
				
				this.Invalidate(); // предотвратить преждевременное перерисовку контрола

				/* рассчитать смещение на которое нужно изменить положение позунков при изменении зума
				 * (взависимости от процентного расположения ползунков и значения фактора зума).
				 * чем ближе расположение ползунков к 100%, тем больше нужно увеличивать значение для смещения 
				 * при значении зума 1.0 (100%), смещение в 55 единиц, является идельным при положения ползунков в 50% 
				 * тогда нижней границей значения смещения при положении ползунков к 0%, является 5 единиц,
				 * а верхней границей при положении ползунков к 100% является 105 единиц.
				 */

				// расчет процента от расположнения ползунков скролла
				// учитываем текуший уровень зума в подсчете процента положения
				Point pos = new Point(HorizontalScroll.Value, VerticalScroll.Value);
				percent = new PointF(((pos.X * 100) / (HorizontalScroll.Maximum - Width)) / zoom,
				                     ((pos.Y * 100) / (VerticalScroll.Maximum - Height)) / zoom);

				// высчитываем смещение с учетом значения фактора зума
				zoomDiff = Math.Abs(zoomDiff * 100f);
				int xOffset = (int)((55 + ((int)Math.Round(percent.X) - 50)) * zoomDiff);
				int yOffset = (int)((55 + ((int)Math.Round(percent.Y) - 50)) * zoomDiff);
				
				if (ZoomIn)
					pos.Offset(xOffset, yOffset);
				else
					pos.Offset(-xOffset, -yOffset);
				SetCanvasScrollPosition = pos;

				CanvasPicture.Invalidate();
				LayoutChanged (this, EventArgs.Empty);
				ZoomChanged(this, EventArgs.Empty);
				//CanvasScroll(this, null);
			}
		}
		
		public void CollapseAll (bool type)
		{
			foreach (CanvasItemData item in itemsList)
			{
				NodeCanvasItem classitem = item.Item as NodeCanvasItem;
				if (classitem != null)
					if (type)
						classitem.ContentCollapseAll(true);
					else
						classitem.Collapsed = true;
			}
			LayoutChanged (this, EventArgs.Empty);
		}
		
		public void ExpandAll (bool type)
		{
			foreach (CanvasItemData item in itemsList)
			{
				NodeCanvasItem classitem = item.Item as NodeCanvasItem;
				if (classitem != null)
					if (type)
						classitem.ContentCollapseAll(false);
					else
						classitem.Collapsed = false;
			}
			LayoutChanged (this, EventArgs.Empty);
		}
		
		public void MatchAllWidths ()
		{
			foreach (CanvasItemData item in itemsList)
			{
				NodeCanvasItem classitem = item.Item as NodeCanvasItem;
				if (classitem != null)
					classitem.Width = classitem.GetAbsoluteContentWidth();
			}
			LayoutChanged (this, EventArgs.Empty);
		}
		
		public void ShrinkAllWidths ()
		{
			foreach (CanvasItemData item in itemsList)
			{
				NodeCanvasItem classitem = item.Item as NodeCanvasItem;
				if (classitem != null)
					classitem.Width = Math.Min(classitem.GetAbsoluteContentWidth(), 350);
			}
			LayoutChanged (this, EventArgs.Empty);
		}
		
		#endregion
		
		public SizeF GetDiagramLogicalSize ()
		{
			float w = 1, h = 1;
			foreach (CanvasItemData item in itemsList)
			{
				w = Math.Max(w, item.Item.X + item.Item.ActualWidth + item.Item.Border);
				h = Math.Max(h, item.Item.Y + item.Item.ActualHeight + item.Item.Border);
			}
			return new SizeF(w, h); // +50
		}
		
		public Size GetDiagramPixelSize ()
		{
			float zoom = Math.Max(this.zoom, 0.1f);
			SizeF size = GetDiagramLogicalSize();
			return new Size((int)(size.Width * zoom), (int)(size.Height * zoom));
		}
		
		public void SetRecommendedGraphicsAttributes (Graphics graphics, bool quality)
		{
			if (graphics == null) return;

			graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			graphics.TextContrast = 0;

			if (quality) {
				graphics.CompositingQuality = CompositingQuality.AssumeLinear;
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				graphics.PixelOffsetMode = PixelOffsetMode.Half;
			} else {
				graphics.CompositingQuality = CompositingQuality.HighSpeed;
				graphics.SmoothingMode = SmoothingMode.HighSpeed;
				graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
				//graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
			}
			graphics.PageUnit = GraphicsUnit.Pixel;
		}
		
		public void DrawToGraphics(Graphics graphics)
		{
			List<Route> routes;
			List<NodeCanvasItem> itemFocused = new List<NodeCanvasItem>();
			
			// отрисовываем ноды
			foreach (CanvasItemData item in itemsList)
			{
				NodeCanvasItem node = item.Item as NodeCanvasItem;
				if (node != null && item.Focused) // выбранные ноды отрисуем после соединяющих линий
					itemFocused.Add(node);
				else
					item.Item.DrawToGraphics(graphics);
			}
			
			// отрисовываем соединяющие линии 
			DrawRoutes(graphics, itemFocused, out routes);
			
			// отрисовать выбранные ноды поверх линий
			foreach (NodeCanvasItem item in itemFocused)
				item.DrawToGraphics(graphics);

			// дорисовываем линии на ноде (для эстетики)
			foreach (Route route in routes)
			{
				if (route.StartShape != null && ((NodeCanvasItem)route.From).Collapsed == false) {
					RouteStartShape shape = (RouteStartShape)route.StartShape;
					shape = new RouteStartShape(Color.FromArgb(200, 255, 69, 0));
					shape.Draw(graphics, route, false);
				}
			}
		}
		
		private void PictureBoxPaint (object sender, PaintEventArgs e)
		{
			//System.Diagnostics.Debug.WriteLine("ClassCanvas.PictureBox1Paint");

			//Size bbox = GetDiagramPixelSize();
			//pictureBox1.Width = Math.Min(10000, bbox.Width + 100);
			//pictureBox1.Height = Math.Min(10000, bbox.Height + 100);

			e.Graphics.PageScale = zoom;
			SetRecommendedGraphicsAttributes(e.Graphics, highQuality);
			DrawToGraphics(e.Graphics);
		}
		
		private void DrawRoutes (Graphics g, List<NodeCanvasItem> itemFocused, out List<Route> routes)
		{
			routes = new List<Route>();

			float thickness = Math.Min((2 / zoom), 4);
			Pen pen = new Pen(Color.FromArgb(128, 0, 0, 0), thickness);
			Pen penTo = new Pen(Color.OrangeRed, thickness + 0.5f);
			Pen penFrom = new Pen(Color.Blue, thickness + 0.5f);

			bool nodeSelected = wireOnlySelect && NodeIsSelected;

			foreach (Route route in diagramRouter.Routes)
			{
				bool ifocus = false;
				Pen pn = pen;
				// подсветка линий для выбранных нод
				foreach (NodeCanvasItem item in itemFocused)
				{
					if (item.Equals((NodeCanvasItem)route.To)) {
						pn = penFrom;
						break;
					}
					if (item.Equals((NodeCanvasItem)route.From)) {
						routes.Add(route);
						pn = penTo;
						ifocus = true;
						break;
					}
				}
				
				route.Recalc(itemsList as IEnumerable<IRectangle>);
				PointF origin = route.GetStartPoint();
				RouteSegment[] segments = route.RouteSegments;

				if (nodeSelected && pn.Equals(pen))
					continue;
				
				foreach (RouteSegment rs in segments)
				{
					PointF dest = rs.CreateDestinationPoint(origin);
					g.DrawLine(pn, origin, dest);
					origin = dest;
				}
				
				if (route.EndShape != null)
					((RouteShape)route.EndShape).Draw(g, route, true);

				if (!ifocus && route.StartShape != null && ((NodeCanvasItem)route.From).Collapsed == false)
					((RouteShape)route.StartShape).Draw(g, route, false);
			}
		}
		
		private LinkedListNode<CanvasItemData> FindCanvasItemNode (PointF pos)
		{
			LinkedListNode<CanvasItemData> itemNode = itemsList.Last;
			while (itemNode != null)
			{
				if (itemNode.Value.Item.HitTest(pos))
				{
					return itemNode;
				}
				itemNode = itemNode.Previous;
			}
			return null;
		}
		
		#region Diagram Items Drag and Selection
		// run before HandleMouseUp
		private void PictureBoxMouseClick (object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			PointF pos = new PointF(e.X / zoom, e.Y / zoom);
			lastMouseClickPosition = pos;

			if (ClickMouseMoveBias(pos))
				return; // выходим если позиция несоответсвует клику 

			LinkedListNode<CanvasItemData> itemNode = FindCanvasItemNode(pos);
			if (itemNode != null) {
				if (ctrlDown && focused && dragItemNode.Count > 1) {
					dragItemNode.Remove(itemNode);
					itemNode.Value.Focused = focused = false;
				}

				if (itemNode.Value.Focused) {
					if (!ctrlDown || dragItemNode.Count <= 1)
						itemNode.Value.Item.HandleMouseClick(pos); //события для ноды
					
					if (itemNode.Value.JustGainedFocus)
					{
						itemNode.Value.JustGainedFocus = false;
					}
					else if (itemNode.Value.Item.StartEditing())
					{
						Control ec = itemNode.Value.Item.GetEditingControl();
						if (ec != null)
						{
							//TODO - refactor this damn thing... why couldn't they make the "Scale" scale the font as well?
							ec.Scale(new SizeF(zoom, zoom));
							Font ecf = ec.Font;
							ec.Font = new Font(ecf.FontFamily,
							                   ecf.Size * (zoom * 1.25f),
							                   ecf.Style, ec.Font.Unit,
							                   ecf.GdiCharSet, ec.Font.GdiVerticalFont);
							//ec.Hide();
							//ec.VisibleChanged += delegate { if (!ec.Visible) ec.Font = ecf; };
							this.Controls.Add(ec); //canvasPanel
							ec.Top -= this.VerticalScroll.Value;
							ec.Left -= this.HorizontalScroll.Value;
							//ec.Show();
							this.Controls.SetChildIndex(ec, 0); //canvasPanel
							this.ActiveControl = ec;
							ec.Focus();
						}
					}
				}
			}
		}
		
		private void PictureBoxMouseDown (object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle && CanvasPicture.Cursor != Cursors.SizeAll) {
				CanvasPicture.Cursor = Cursors.SizeAll;
				locationCursor = Cursor.Position;
				return;
			}
			else if (e.Button != MouseButtons.Left)
					return;

			HoldRedraw = true;
			PointF pos = new PointF(e.X / zoom, e.Y / zoom);
			lastMouseDownPosition = pos;

			LinkedListNode<CanvasItemData> itemNode = FindCanvasItemNode(pos);
			
			if (!ctrlDown) // ctrl not pressed 
				ClearAllItemsFocus(itemNode);
			
			if (itemNode != null) {
				focused = itemNode.Value.Focused;
				if (!focused) {
					SetItemFocus(itemNode);
					itemsList.Remove(itemNode);
					itemsList.AddLast(itemNode);
				}
				itemNode.Value.Item.HandleMouseDown(pos); //в событии итема устанавливаем свойство dragged = true
				
				if (!dragItemNode.Contains(itemNode))
					dragItemNode.Add(itemNode); //добавление ноды в лист для перетаскивания
				
				if (dragItemNode.Count > 1) {
					foreach (var drag in dragItemNode)
						drag.Value.Item.DragMousePos(pos); // изменяем смещение для ранее добавленных нод
				}
			} else
				CanvasItemUnSelected (this, EventArgs.Empty);

			HoldRedraw = false;
		}
		
		private void PictureBoxMouseMove (object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle) {
				Point locationCursorOffset = Cursor.Position;
				locationCursorOffset.X -= locationCursor.X;
				locationCursorOffset.Y -= locationCursor.Y;
				locationCursor = Cursor.Position;

				locationScrollLast.X -= locationCursorOffset.X;
				locationScrollLast.Y -= locationCursorOffset.Y;
				this.AutoScrollPosition = locationScrollLast;

				if (locationScrollLast.X < this.AutoScrollPosition.X)
					locationScrollLast.X = 0;
				if (locationScrollLast.Y < this.AutoScrollPosition.Y)
					locationScrollLast.Y = 0;

				if ((0 - locationScrollLast.Y) < this.AutoScrollPosition.Y)
					locationScrollLast.Y = 1 + ((-1) - this.AutoScrollPosition.Y);
				if ((0 - locationScrollLast.X) < this.AutoScrollPosition.X)
					locationScrollLast.X = 1 + ((-1) - this.AutoScrollPosition.X);

				this.OnScroll(null);
					
				return;
			}

			HoldRedraw = true;

			PointF pos = new PointF(e.X / zoom, e.Y / zoom);
			LinkedListNode<CanvasItemData> itemNode = FindCanvasItemNode(pos);
				
			if (hoverItemNode != itemNode) {
				if (hoverItemNode != null && hoverItemNode.Value != null)
					hoverItemNode.Value.Item.HandleMouseLeave();
				HoverItemNode = itemNode; // текущая нода под курсором
			}

			if (dragItemNode.Count > 0) {
				foreach (var drag in dragItemNode)
					drag.Value.Item.HandleMouseMove(pos); // событие для перемещения выбраных нод
			}/* else if (itemNode != null)
					itemNode.Value.Item.HandleMouseMove(pos);
			}*/

			HoldRedraw = false;
		}
		
		private void PictureBoxMouseUp (object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle && CanvasPicture.Cursor == Cursors.SizeAll) {
				CanvasPicture.Cursor = Cursors.Default;
				return;
			}
			
			PointF pos = new PointF(e.X / zoom, e.Y / zoom);
			
			//LinkedListNode<CanvasItemData> itemNode = FindCanvasItemNode(pos);
			//if (itemNode != null)
			//    itemNode.Value.Item.HandleMouseUp(pos);

			foreach (var drag in dragItemNode)
				drag.Value.Item.HandleMouseUp(pos); // сбрасываем всем свойство dragged
		}
		
		private void ClearAllItemsFocus(LinkedListNode<CanvasItemData> itemNode)
		{
			foreach (CanvasItemData item in itemsList)
			{
				item.Item.StopEditing();
				if (itemNode == null || item != itemNode.Value)
					item.Focused = false;
			}
			
			ClearDragItems(); // очистить список, если был клик без зажатого ctrl
		}

		private void SetItemFocus(LinkedListNode<CanvasItemData> itemNode)
		{
			itemNode.Value.JustGainedFocus = true;
			itemNode.Value.Focused = true;
			CanvasItemSelected(this, new CanvasItemEventArgs(itemNode.Value.Item));
		}

		private bool ClickMouseMoveBias(PointF pos)
		{
			int bias = 10;
			if ((pos.X > (lastMouseDownPosition.X - bias) && pos.X < (lastMouseDownPosition.X + bias)) &&
				(pos.Y > (lastMouseDownPosition.Y - bias) && pos.Y < (lastMouseDownPosition.Y + bias)))
				return false;

			return true;
		}
		
		#endregion
		
		private bool HoldRedraw
		{
			get { return holdRedraw; }
			set
			{
				holdRedraw = value;
				if (!value && redrawNeeded)
				{
					redrawNeeded = false;
					HandleRedraw (this, EventArgs.Empty);
				}
			}
		}

		private void HandleItemLayoutChange (object sender, EventArgs args)
		{
			LayoutChanged (this, args);
			if (HoldRedraw)
				redrawNeeded = true;
			else
				HandleRedraw (sender, args);
		}

		private void HandleRedraw (object sender, EventArgs args)
		{
			if (HoldRedraw)
			{
				redrawNeeded = true;
				return;
			}
//			System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
//			System.Diagnostics.Debug.WriteLine(st.ToString());
			
			CanvasPicture.Invalidate(); //this.Invalidate(true);
		}
		
		private void HandleItemPositionChange (object sender, ValueChangingEventArgs<PointF> args)
		{
			PointF pos = new PointF(args.Value.X, args.Value.Y);
			
			pos.X = Math.Max ((float) Math.Round(pos.X / 5.0f)* 5.0f, 10.0f);
			pos.Y = Math.Max ((float) Math.Round(pos.Y / 5.0f)* 5.0f, 10.0f);
			
			args.Cancel = (pos.X == args.Value.X) && (pos.Y == args.Value.Y);
			args.Value = pos;
		}
		
		private void HandleItemSizeChange (object sender, ValueChangingEventArgs<SizeF> args)
		{
			SizeF size = new SizeF(args.Value);
			
			size.Width = (float) Math.Round(size.Width / 5.0f) * 5.0f;
			size.Height = (float) Math.Round(size.Height / 5.0f) * 5.0f;
			
			// TODO: bug - при округление размера и совпадении его, далее не происходит изменение размера
			//args.Cancel = (size.Width == args.Value.Width) && (size.Height == args.Value.Height);
			args.Value = size;
		}
				
		private void SizeGripMouseEntered (object sender, SizeGripEventArgs e)
		{
			if ((e.GripPosition & SizeGripPositions.EastWest) != SizeGripPositions.None)
			{
				CanvasPicture.Cursor = Cursors.SizeWE;
			}
			else if ((e.GripPosition & SizeGripPositions.NorthSouth) != SizeGripPositions.None)
			{
				CanvasPicture.Cursor = Cursors.SizeNS;
			}
		}

		private void SizeGripMouseLeft (object sender, SizeGripEventArgs e)
		{
			CanvasPicture.Cursor = Cursors.Default;
		}
		
		public void ReAddedCanvasItem (CanvasItem item)
		{
			item.ClearDecorator();
			item.RedrawNeeded -= HandleRedraw;
			item.LayoutChanged -= HandleItemLayoutChange;
			item.PositionChanging -= HandleItemPositionChange;
			item.SizeChanging -= HandleItemSizeChange;

			AddCanvasItem(item);
		}
		
		/// <summary>
		/// добавление новой ноды
		/// </summary>
		/// <param name="item"></param>
		public void AddCanvasItem (CanvasItem item, CanvasItem itemAt = null)
		{
			NodeCanvasItem classItem = item as NodeCanvasItem;
			if (classItem != null) classItem.OffsetPointTo();
			
			diagramRouter.AddItem(item);
			CanvasItemData itemData = new CanvasItemData(item, SizeGripMouseEntered, SizeGripMouseLeft);
			itemsData[item] = itemData;
			
			if (classItem != null)
			{
				nodesToData.Add(classItem.GetNodeData, item);
				foreach (CanvasItemData ci in itemsList)
				{
					NodeCanvasItem cci = ci.Item as NodeCanvasItem;
					if (cci != null)
					{
						//Route r = null;
						foreach (LinkTo link in cci.GetNodeData.LinkedToNodes)
						{
							if (link.NameTo == classItem.GetNodeData.Name) {
								//                  link: from  >>>  to
								Route r = diagramRouter.AddRoute(cci, classItem, link.ContentLine);
								r.EndShape = new RouteInheritanceShape();
								r.StartShape = new RouteStartShape();
							}
						}
						//if (r != null) {}
						foreach (LinkTo link in classItem.GetNodeData.LinkedToNodes)
						{
							if (link.NameTo == cci.GetNodeData.Name) {
								//                  link: from  >>>  to
								Route r = diagramRouter.AddRoute(classItem, cci, link.ContentLine);
								r.EndShape = new RouteInheritanceShape();
								r.StartShape = new RouteStartShape();
							}
						}
						//if (r != null && r.EndShape == null) {}
					}
				}
			}

			// добавление ноды в лист
			if (itemAt != null) {
				LinkedListNode<CanvasItemData> find = itemsList.Find(itemsData[itemAt]);
				if (find != null)
					itemsList.AddBefore(find, itemData);
				else
					itemsList.AddLast(itemData);
			} else
				itemsList.AddLast(itemData);

			item.RedrawNeeded += HandleRedraw;
			item.LayoutChanged += HandleItemLayoutChange;
			item.PositionChanging += HandleItemPositionChange;
			item.SizeChanging += HandleItemSizeChange;
		}
		
		public void RemoveCanvasItem (CanvasItem item)
		{
			if (item == null) return;

			itemsList.Remove(itemsData[item]);
			Stack<Route> routesToRemove = new Stack<Route>();
			foreach (Route r in diagramRouter.Routes)
			{
				if (r.From == item || r.To == item)
					routesToRemove.Push(r);
			}
			
			foreach (Route r in routesToRemove)
				diagramRouter.RemoveRoute(r);

			diagramRouter.RemoveItem (item);
			
			NodeCanvasItem classItem = item as NodeCanvasItem;
			if (classItem != null)
			{
				nodesToData.Remove (classItem.GetNodeData);
			}
			
			itemsData.Remove(item);
			
			LayoutChanged(this, EventArgs.Empty);
		}
		
		public void ClearCanvas()
		{
			itemsList.Clear();
			nodesToData.Clear();
			itemsData.Clear();
			dragItemNode.Clear();
			hoverItemNode = null;
			diagramRouter.Clear();
		}
		
		public void ClearAllItemsFocus()
		{
			foreach (CanvasItemData item in itemsList)
			{
				item.Item.StopEditing();
				item.Focused = false;
			}
			ClearDragItems(); // очистить список, если был клик без зажатого ctrl
		}
		
		// очистить список перетаскиваемых объектов
		public void ClearDragItems()
		{ 
			if (dragItemNode.Count > 0)
				dragItemNode.Clear();
		}
		
		public void SetFocusedCanvasItem(CanvasItem ci)
		{
			CanvasItemData value = null;
			if (itemsData.TryGetValue(ci, out value)) {
				LinkedListNode<CanvasItemData> itemNode = new LinkedListNode<CanvasItemData>(value);
				SetItemFocus(itemNode);
				ClearAllItemsFocus(itemNode);
				if (itemsList.Last.Value != itemNode.Value) {
					itemsList.Remove(itemNode.Value);
					itemsList.AddLast(itemNode.Value);
				}
			}
		}
		
		public void SetJustLastFocus()
		{
			itemsList.Last.Value.Focused = true;
		}
		
		/// <summary>
		/// Returns last selected canvas item
		/// </summary>
		public CanvasItem GetLastFocusItem()
		{
			if (itemsList.Count == 0) return null;
			CanvasItemData item = itemsList.Last.Value;
			if (item.Focused)
				return item.Item;
			
			return null;
		}
		
		/// <summary>
		/// Returns an canvas node by its node name.
		/// </summary>
		public CanvasItem GetNodeCanvasItem(string nodeName)
		{
			foreach (CanvasItemData item in itemsList)
			{
				NodeCanvasItem node = item.Item as NodeCanvasItem;
				if (node != null) {
					if (node.GetNodeData.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase))
						return node;
				}
			}
			
		return null;
		}
		
		public bool IsEditingNote()
		{
			var item = GetLastFocusItem();
			if (item != null && item is NoteCanvasItem) {
				return ((NoteCanvasItem)item).IsEditing;
			}
			return false;
		}
		
		/// <summary>
		/// Retruns a copy of the the canvas items list as an array.
		/// </summary>
		public CanvasItem[] GetCanvasItems()
		{
			CanvasItem[] items = new CanvasItem[itemsList.Count];
			int i = 0;
			foreach (CanvasItemData item in itemsList)
				items[i++] = item.Item;
			return items;
		}
		
		public bool Contains (INode ct)
		{
			return nodesToData.ContainsKey(ct);
		}
		
		public bool Contains (string name)
		{
			foreach (INode node in nodesToData.Keys)
				if (name == node.Name)
					return true;
			
			return false;
		}
		
		public void RemoveUnusedNodes(List<INode> iNode)
		{
			List<INode> unusedNodes = new List<INode>();
			
			foreach (INode node in nodesToData.Keys)
			{
				if (!iNode.Contains(node)) unusedNodes.Add(node);
			}
			foreach (INode node in unusedNodes)
			{
				RemoveCanvasItem(nodesToData[node]);
			}
		}

		public void AutoArrange ()
		{
			diagramRouter.RecalcPositions();
		}
		
		/// <summary>
		/// создание объекта из передаваемого типа
		/// </summary>
		public static NodeCanvasItem CreateItemFromType (INode ct)
		{
			if (ct == null) return null;

			NodeCanvasItem item = new NodeCanvasItem(ct, NodeLowDetails);
			item.Initialize();
			return item;
		}
		
		public XmlDocument WriteToXml (XmlDocument doc)
		{
			foreach (CanvasItemData item in itemsList)
				item.Item.WriteToXml(doc);
			
			return doc;
		}
		
		public event EventHandler<MouseEventArgs> CanvasMouseWheel;
		public event EventHandler LayoutChanged = delegate {};
		public event EventHandler<CanvasItemEventArgs> CanvasItemHover = delegate {};
		public event EventHandler<CanvasItemEventArgs> CanvasItemSelected = delegate {};
		public event EventHandler CanvasItemUnSelected;
		
		public Bitmap GetAsBitmap ()
		{
			Size bbox = GetDiagramPixelSize();
			Bitmap bitmap = new Bitmap(Math.Min(bbox.Width + 50, pictureHWSize),
									   Math.Min(bbox.Height + 50, pictureHWSize));
			Graphics g = Graphics.FromImage(bitmap);
			g.PageScale = zoom;
			SetRecommendedGraphicsAttributes(g, true);
			DrawToGraphics(g);
			return bitmap;
		}
		
		public void SaveToImage (string filename)
		{
			GetAsBitmap().Save(filename);
		}
		
		public PointF LastMouseClickPosition
		{
			get { return lastMouseClickPosition; }
		}
		
		#region Drag/Drop from Class Browser Handling
		
		private void ClassCanvasDragOver(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Link;
		}
		
		private void ClassCanvasDragDrop(object sender, DragEventArgs e)
		{
			
		}
		
		#endregion
		
		void ClassCanvasKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ControlKey)
				ctrlDown = true;
		}
		
		void ClassCanvasKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ControlKey)
				ctrlDown = false;
		}

		void CanvasScroll(object sender, ScrollEventArgs e)
		{
			if (CanvasPicture.Cursor == Cursors.SizeAll)
				return;

			locationScrollLast.Y = this.VerticalScroll.Value;
			locationScrollLast.X = this.HorizontalScroll.Value;
		}
		
		// переопределяем событие
		protected override void OnMouseWheel(MouseEventArgs e)
		{ 
			//base.OnMouseWheel(e);
			((HandledMouseEventArgs)e).Handled = true;
			
			CanvasMouseWheel(this, e);
		}

		protected override void OnScroll(ScrollEventArgs se)
		{
			base.OnScroll(se);
			CanvasScroll(this, se);
		}
	}

	public class CanvasItemEventArgs : EventArgs
	{
		public CanvasItemEventArgs (CanvasItem canvasItem)
		{
			this.canvasItem = canvasItem;
		}
		
		private CanvasItem canvasItem;
		
		public CanvasItem CanvasItem
		{
			get { return canvasItem; }
		}
	}
}
