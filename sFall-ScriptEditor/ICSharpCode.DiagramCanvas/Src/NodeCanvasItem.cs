/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;

using System.Xml;
using System.Xml.XPath;

using ICSharpCode.Diagrams;
using ICSharpCode.Diagrams.Drawables;

using ScriptEditor.CodeTranslation;

namespace ICSharpCode.ClassDiagram
{
	/// <summary>
	/// This class was built from ClassCanvasItem 
	/// </summary>
	public class NodeCanvasItem : CanvasItem, IDisposable
	{
		public delegate void ContentClickHandler(object sender, TextSegment ts);
		public event ContentClickHandler ContentClick;

		public delegate void ShowCodeClickHandler(NodeCanvasItem node);
		public event ShowCodeClickHandler ShowCodeButtonClick;

		///<summary>
		/// состояние кнопки "ShowCode" при загрузки сохрененного значения
		/// -1 неопределенное состояние
		///</summary>
		public static int showCode = -1;

		INode dataNode;
		
		bool nodeLowDetails = false;

		int lineHeaderContent = 0;

		public bool IsEditing { get; private set; }
		public bool StartEditingNode() { IsEditing = true; return false; }
		public void StopEditingNode() { IsEditing = false;}

		#region Graphics related variables
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font TitleFont = new Font (FontFamily.GenericSansSerif, 18, FontStyle.Bold, GraphicsUnit.Pixel);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font SubtextFont = new Font (FontFamily.GenericSansSerif, 12, FontStyle.Regular, GraphicsUnit.Pixel);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font GroupTitleFont = new Font (FontFamily.GenericMonospace, 14, FontStyle.Bold, GraphicsUnit.Pixel);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font LinkedFont = new Font (FontFamily.GenericSansSerif, 12, FontStyle.Bold, GraphicsUnit.Pixel);
		public static readonly Font ScriptFont = new Font (FontFamily.GenericMonospace, 14, FontStyle.Regular, GraphicsUnit.Pixel);
		
		LinearGradientBrush grad;
		GraphicsPath shadowpath;
		DrawableRectangle containingShape;
		
		#endregion

		InteractiveHeaderedItem classItemHeaderedContent;
		DrawableItemsStack classItemContainer = new DrawableItemsStack();

		CollapseExpandShape collapseExpandShape = new CollapseExpandShape();
		ViewCodeShape viewCodeShape;

		DrawableItemsStack titles = new DrawableItemsStack();
		DrawableItemsStack titlesCollapsed = new DrawableItemsStack();
		DrawableItemsStack titlesExpanded = new DrawableItemsStack();
		
		InteractiveItemsStack items = new InteractiveItemsStack();		
		
		DrawableRectangle titlesBackgroundCollapsed;
		DrawableRectangle titlesBackgroundExpanded;
		
		DrawableItemsStack<InteractiveHeaderedItem> nodeContents = new DrawableItemsStack<InteractiveHeaderedItem>();
		Dictionary<InteractiveHeaderedItem, string> nodeContentNames = new Dictionary<InteractiveHeaderedItem, string>(); 
		Dictionary<string, InteractiveHeaderedItem> nodeContentsByName = new Dictionary<string, InteractiveHeaderedItem>();
		
		DrawableItemsStack<TextSegment> nodeDescription = new DrawableItemsStack<TextSegment>();
		
		#region Graphics related members
		
		static Color titlesBG = Color.FromArgb(217, 225, 241);

		protected virtual Color TitleBackground
		{
			get { return titlesBG; }
		}

		protected Color TitleBackgroundRed
		{
			get { return Color.MistyRose; }
		}

		protected Color TitleBackgroundGreen
		{
			get { return Color.LightGreen; }
		}

		protected Color TitleBackgroundYellow
		{
			get { return Color.Moccasin; }
		}

		protected Color TitleBackgroundGray
		{
			get { return Color.LightGray; }
		}

		protected virtual LinearGradientBrush TitleBG
		{
			get { return grad; }
		}

		//Reply Background
		static Brush replyTitlesBG = new SolidBrush(Color.FromArgb(255, 220, 220, 250));
		protected virtual Brush ReplyTitlesBackground
		{
			get { return replyTitlesBG; }
		}

		static Brush replyTextTitlesBG = new SolidBrush(Color.FromArgb(255, 230, 230, 255));
		protected virtual Brush ReplyTextTitlesBackground
		{
			get { return replyTextTitlesBG; }
		}

		//Options Background
		static Brush optionTitlesBG = new SolidBrush(Color.FromArgb(255, 233, 227, 165)); //PaleGoldenrod
		protected virtual Brush OptionTitlesBackground
		{
			get { return optionTitlesBG; }
		}

		static Brush optionTextTitlesBG = new SolidBrush(Color.FromArgb(255, 243, 237, 175));
		protected virtual Brush OptionTextTitlesBackground
		{
			get { return optionTextTitlesBG; }
		}

		//Call Background
		static Brush callTextTitlesBG = new SolidBrush(Color.FromArgb(255, 235, 235, 210)); //DarkBeige
		protected virtual Brush CallTextTitlesBackground
		{
			get { return callTextTitlesBG; }
		}
		
		static Brush contentBG = new SolidBrush(Color.FromArgb(255, 250, 250, 255));
		protected virtual Brush ContentBG
		{
			get { return contentBG; }
		}
		
		protected virtual bool RoundedCorners
		{
			get { return true; }
		}
		
		protected virtual int CornerRadius
		{
			get { return 15; }
		}
		#endregion
		
		protected override bool AllowHeightModifications()
		{
			return false;
		}
		
		public override float Width
		{
			set
			{
				base.Width = Math.Max (value, 100.0f);
				PrepareFrame();
			}
		}
		
		public override float GetAbsoluteContentWidth()
		{
			return classItemHeaderedContent.GetAbsoluteContentWidth();
		}
		
		public override float GetAbsoluteContentHeight()
		{
			return classItemHeaderedContent.GetAbsoluteContentHeight();
		}
		
		public INode GetNodeData
		{
			get { return dataNode; }
		}
		
		#region Constructors
		// Создание ноды (start)
		public NodeCanvasItem(INode dataNode, bool nodeLowDetails)
		{
			this.dataNode = dataNode;
			this.nodeLowDetails = nodeLowDetails;

			// Header gradient
			Color titleBackground;
			switch (dataNode.NodeType) {
				case NodesType.DialogEnd:
					titleBackground = TitleBackgroundRed;
					break;
				case NodesType.DialogStart:
					titleBackground = TitleBackgroundGreen;
					break;
				case NodesType.NoFromLink:
					titleBackground = TitleBackgroundYellow;
					break;
				case NodesType.Unused:
					titleBackground = TitleBackgroundGray;
					break;
				default:
					titleBackground = TitleBackground;
					break;
			}
			grad = new LinearGradientBrush(new PointF(0, 0), new PointF(1, 0), titleBackground, Color.White);
			
			classItemHeaderedContent = new InteractiveHeaderedItem(titlesCollapsed, titlesExpanded, InitContentContainer(InitContent()));
			classItemContainer.Container = this;
			classItemContainer.Add(classItemHeaderedContent);
			
			Pen outlinePen = GetNodeOutlinePen();

			if (RoundedCorners) {
				int radius = CornerRadius;
				containingShape = new DrawableRectangle(null, outlinePen, radius, radius, radius, radius);
				
				titlesBackgroundCollapsed = new DrawableRectangle(grad, null, radius, radius, radius, radius);
				titlesBackgroundExpanded = new DrawableRectangle(grad, null, radius, radius, 0, 0);

			} else {
				containingShape = new DrawableRectangle(null, outlinePen, 0, 0, 0, 0);
				
				titlesBackgroundCollapsed = new DrawableRectangle(grad, null, 0, 0, 0, 0);
				titlesBackgroundExpanded = new DrawableRectangle(grad, null, 0, 0, 0, 0);
			}

			classItemContainer.Add(containingShape);
			classItemContainer.OrientationAxis = Axis.Z;
			
			titles.Border = 5;
			
			titlesCollapsed.Add(titlesBackgroundCollapsed);
			titlesCollapsed.Add(titles);
			titlesCollapsed.OrientationAxis = Axis.Z;
			
			titlesExpanded.Add(titlesBackgroundExpanded);
			titlesExpanded.Add(titles);
			titlesExpanded.OrientationAxis = Axis.Z;
		}
		#endregion
		
		// Stroke type for node
		private Pen GetNodeOutlinePen()
		{
			Pen pen = new Pen(Color.LightSlateGray, 2);

			switch (dataNode.NodeType) {
				case NodesType.DialogStart:
					pen.Color = Color.DarkGreen;
					break;
				case NodesType.DialogEnd:
					pen.Color = Color.DarkRed;
					break;
				case NodesType.NoFromLink:
					pen.Color = Color.Peru;
					break;
				case NodesType.Unused:
					pen.Color = Color.Gray;
					pen.DashStyle = DashStyle.Dash;
					break;
			}
			return pen;
		}
		
		protected virtual DrawableRectangle InitContentBackground()
		{
			if (RoundedCorners) {
				int radius = CornerRadius;
				return new DrawableRectangle(ContentBG, null, 0, 0, radius, radius);
			} else
				return new DrawableRectangle(ContentBG, null, 0, 0, 0, 0);
		}
		
		// помещение содержимого контента в отрисованный контейнер под заголовком
		// определнение цвета заливки для фона контейнера
		protected virtual DrawableItemsStack InitContentContainer(params IDrawableRectangle[] items)
		{
			DrawableItemsStack content = new DrawableItemsStack();
			content.OrientationAxis = Axis.Z;
			content.Add(InitContentBackground());
			
			foreach (IDrawableRectangle item in items)
				content.Add(item);

			return content;
		}
		
		protected virtual IDrawableRectangle InitContent ()
		{
			items.MinWidth = 80;
			items.Border = 5;
			items.Spacing = 3;
			//items.Padding = 1;

			return items;
		}
		
		public void Initialize ()
		{
			PrepareNodeContent();
			PrepareTitles();
			Width = Math.Min(GetAbsoluteContentWidth(), 350.0f);
			//OffsetPointTo();
		}
		
		/*public void UpdateNodeName(string nName)
		{
			GetNodeData.Name = nName;
			foreach (var t in titles)
			{
				foreach (var item in (DrawableItemsStack)t)
				{
					if (item is TextSegment) {
						((TextSegment)item).Text = GetNodeData.Name;
						return;
					}
				}
			}
		}*/

		public int NodeContentsCount
		{
			get { return nodeContents.Count; }
		}

		public void SetContentCollapsed(string line, bool value)
		{
			nodeContentsByName[line].Collapsed = value;
		}

		public bool ContentIsCollapsed(string line)
		{
			if (nodeContentsByName.ContainsKey(line))
				return nodeContentsByName[line].Collapsed;
			else
			return true; // если такой строки контента уже несуществует
		}

		public void ContentCollapseAll(bool value)
		{
			foreach (var cnt in nodeContents)
				cnt.Collapsed = value;
			
			OffsetPointTo();
		}

		#region Preparations
		//создание заголовока для ноды
		protected virtual void PrepareTitles ()
		{
			if (dataNode == null) return;
			
			DrawableItemsStack title = new DrawableItemsStack();
			title.OrientationAxis = Axis.X;
			
			//Название
			TextSegment titleString = new TextSegment(base.Graphics, dataNode.Name, TitleFont, true, StringAlignment.Center);

			//title.Add(new NodeCircleShape(dataNode.NodeType));
			viewCodeShape = new ViewCodeShape(dataNode.ShowCodeNodeButton);
			title.Add(viewCodeShape);

			title.Add(titleString);
			title.Add(collapseExpandShape);
			
			collapseExpandShape.Collapsed = Collapsed = (dataNode.NodeType == NodesType.Unused || dataNode.NodeContent.Count == 0);

			titles.OrientationAxis = Axis.Y;
			titles.Add(title);

			if (!nodeLowDetails) {
				//titles.Add(new TextSegment(base.Graphics, "Linked from:", LinkedFont, true));
				//Список Нод которые ссылаются на эту ноду
				DrawableItemsStack linkedFrom = new DrawableItemsStack();
				linkedFrom.OrientationAxis = Axis.Y;

				if (dataNode.NodeType == NodesType.DialogStart) {
					linkedFrom.Add(new TextSegment(base.Graphics, "Start Dialogue Procedure", LinkedFont, true));
					linkedFrom.Border = 8;
				} else if (dataNode.NodeType == NodesType.NoFromLink) {
					linkedFrom.Add(new TextSegment(base.Graphics, "No Link Nodes", LinkedFont, true));
					linkedFrom.Border = 8;
				} else if (dataNode.NodeType == NodesType.Unused) {
					linkedFrom.Add(new TextSegment(base.Graphics, "Unused Node", LinkedFont, true));
					linkedFrom.Border = 8;
				} else {
					foreach (string linkF in dataNode.LinkedFromNodes) {
						linkedFrom.Add(new TextSegment(base.Graphics, linkF, SubtextFont, true));
					}
				}
				titles.Add(linkedFrom);
			}
			//добавление интерфейса (можно это использовать для короткого описания к ноде)
			//string type = null;
			//if (dataNode.NodeType == NodesType.DialogStart)
			//    type = "Start Dialog";
			//else if (dataNode.NodeType == NodesType.DialogEnd)
			//    type = "Exit Dialog";
			//if (type != null)
			//    interfaces.Add(new TextSegment(base.Graphics, "test", LinkedFont, true));
		}

		public void RemoveItemContex()
		{
			List<IDrawableRectangle> removeItems = new List<IDrawableRectangle>();
			List<int> removeIndexLine = new List<int>();

			int line = 0;
			foreach (var item in items)
			{
				line++;
				if (item is TextSegment) {
					removeItems.Add(item);
					removeIndexLine.Add(line);
				}
			}
			// Корректировака для правильного позиционирования исходящих линий ноды
			for (int i = 0; i < removeItems.Count; i++)
			{
				for (int j = 0; j < dataNode.LinkedToNodes.Count; j++)
				{
					var link = dataNode.LinkedToNodes[j];
					if (removeIndexLine[i] > link.ContentLine) continue;

					link.SetLine = link.SetLine - 1;
				}
				items.Remove(removeItems[i]);
			}
			foreach (var item in dataNode.LinkedToNodes) item.CommitLine();

			OffsetPointTo();
			HandleRedraw(null, null);
		}

		// создание заголовка раскрывающихся полей, и кнопки +/- для раскрытия Reply/Option
		protected InteractiveHeaderedItem PrepareMessagesHeader (ContentBody content, IDrawableRectangle item, bool reply)
		{
			#region Prepare Container
			DrawableItemsStack headerPlus = new DrawableItemsStack();
			DrawableItemsStack headerMinus = new DrawableItemsStack();
			
			headerPlus.OrientationAxis = Axis.X;
			headerMinus.OrientationAxis = Axis.X;
			#endregion
			
			#region Create Header

			TextSegment titleSegment = new TextSegment(Graphics, (reply) ? @"""" + content.msgText + @""""
			                                                     : (nodeLowDetails) ? "\u25CF " + content.msgText : content.msgText,
			                                                     content.index, MessagesFont, true, reply); // GroupTitleFont

			if (!nodeLowDetails) { 
				PlusShape plus = new PlusShape();
				plus.Border = 4;
				plus.ScaleShape = false;
				plus.Width = plus.ShapeWidth;
				plus.Height = plus.ShapeHeight;
				headerPlus.Add(plus);

				MinusShape minus = new MinusShape();
				minus.Border = 4;
				minus.ScaleShape = false;
				minus.Width = minus.ShapeWidth;
				minus.Height = minus.ShapeHeight;
				headerMinus.Add(minus);
			}
			headerPlus.Add(titleSegment);
			headerMinus.Add(titleSegment);
			
			//пиктограмма
			//if (!reply) {
			//    DrawableItemsStack<VectorShape> image = new DrawableItemsStack<VectorShape>();
			//    image.OrientationAxis = Axis.X;
			//    image.KeepAspectRatio = true;
			//    image.Border = -3.0f;
			//    image.Add(new OptionsCircleShape()); 

			//    headerPlus.Add(image);
			//    headerMinus.Add(image);
			//}

			DrawableItemsStack headerCollapsed = new DrawableItemsStack();
			DrawableItemsStack headerExpanded = new DrawableItemsStack();
			
			headerCollapsed.OrientationAxis = Axis.Z;
			headerExpanded.OrientationAxis = Axis.Z;

			headerCollapsed.Add (new DrawableRectangle((reply) ? ReplyTitlesBackground : OptionTitlesBackground,null, 5, 5, 5, 5));
			headerCollapsed.Add (headerPlus);
			
			headerExpanded.Add (new DrawableRectangle((reply) ? ReplyTitlesBackground : OptionTitlesBackground, null, 5, 5, 0, 0));
			headerExpanded.Add (headerMinus);
			
			#endregion

			InteractiveHeaderedItem tg = new InteractiveHeaderedItem(headerCollapsed, headerExpanded, item);

			tg.Collapsed = true;

			// событие, клик мышкой по элементу строки кода 
			tg.HeaderClicked += delegate(object sender, PointF e)
			{
				if (Math.Abs(tg.AbsoluteX - e.X) < 20 && Math.Abs(tg.AbsoluteY - e.Y) < 20) {
					tg.Collapsed = !tg.Collapsed;
				} else {
					ContentClick(this, titleSegment);
				}
			};

			// старый вариант
			//IMouseInteractable interactive = content as IMouseInteractable;
			//if (interactive != null) {
			//    tg.ContentClicked += delegate { tgContentClicked(interactive, tg); }; 
			//    //оригинальное событие
			//    //tg.ContentClicked += delegate (object sender, PointF pos) {
			//    //    interactive.HandleMouseClick(pos); 
			//    //};
			//}

			tg.RedrawNeeded += HandleRedraw;

			return tg;
		}

		/*
		void tgContentClicked(IMouseInteractable interactive, InteractiveHeaderedItem tg)
		{
			if (ContentClick == null) return;
			
			foreach (var i in (InteractiveItemsStack)interactive)
			{
				if (!(i is InteractiveItemsStack)) continue;
				foreach (var ts in (InteractiveItemsStack)i)
				{
					if (ts is TextSegment)
						ContentClick(this, (TextSegment)ts);
				}
			}
		}
		*/

		protected virtual void PrepareNodeContent ()
		{
			if (dataNode == null) return;
			
			InteractiveItemsStack itemReply;
			InteractiveItemsStack itemOptions;
			DrawableItemsStack<InteractiveHeaderedItem> tileHeader;

			foreach (ContentBody content in dataNode.NodeContent)
			{
				switch (content.type) 
				{
					case OpcodeType.Reply:
					case OpcodeType.Message:
						itemReply = PrepareMessageContent(content.scrText, true); 

						tileHeader = new DrawableItemsStack<InteractiveHeaderedItem>();
						tileHeader.Add(MessageToContent(content, itemReply, true));

						items.Add(tileHeader);
						break;
					case OpcodeType.Option:
					case OpcodeType.giq_option:
					case OpcodeType.gsay_option:
						itemOptions = PrepareMessageContent(content.scrText, false); 

						tileHeader = new DrawableItemsStack<InteractiveHeaderedItem>();
						tileHeader.Add(MessageToContent(content, itemOptions, false));

						items.Add(tileHeader);
						break;
					case OpcodeType.call:
						//var callContent = new DrawableItemsStack(); //InteractiveItemsStack();
						//callContent.OrientationAxis = Axis.Z;
						
						//callContent.Add(new DrawableRectangle(CallTextTitlesBackground, null, 8, 8, 8, 8));
						//callContent.Add(new TextSegment(Graphics, content.scrText, GroupTitleFont, true));
						/* 
						 * Код метода выше вызывает некорректную отрисовку высоты для элемента DrawableRectangle
						 * поэтому был использован нижний способ добавления элементов
						 */
						var callText = new DrawableItemsStack();
						callText.Add(new TextSegment(Graphics, content.scrText, GroupTitleFont, true));

						var background = new DrawableItemsStack();
						background.OrientationAxis = Axis.Z;
						background.Add(new DrawableRectangle(CallTextTitlesBackground, null, 8, 8, 8, 8));
						background.Add(callText);

						var header = new InteractiveHeaderedItem(background, new DrawableItemsStack(), new DrawableItemsStack());
						header.Collapsed = true;
						//header.RedrawNeeded += HandleRedraw;

						items.Add(header);
						break;
					default:
						items.Add(new TextSegment(Graphics, content.scrText, ScriptFont, true));
						break;
				}
			}
		}

		protected virtual InteractiveItemsStack PrepareMessageContent(string message, bool reply)
		{
			// пиктограмма стрелки
			DrawableItemsStack<VectorShape> image = new DrawableItemsStack<VectorShape>();
			image.OrientationAxis = Axis.X; // stack image components one on top of the other
			image.KeepAspectRatio = true;
			image.Add(new ArrowShape()); //OptionsCircleShape
			image.Border = 1;
			
			// текст
			InteractiveItemsStack replyText = new InteractiveItemsStack();
			replyText.OrientationAxis = Axis.X;
			replyText.Border = 1;
			replyText.Add(image);
			
			TextSegment text = new TextSegment(Graphics, message, GroupTitleFont, true);
			text.Brush = (reply) ? Brushes.DarkBlue : Brushes.Crimson;
			replyText.Add(text);

			// Background контейнер
			InteractiveItemsStack content = new InteractiveItemsStack();
			content.OrientationAxis = Axis.Z;
			content.Add(new DrawableRectangle((reply)? ReplyTextTitlesBackground : OptionTextTitlesBackground, null, 0, 0, 5, 5));
			content.Add(replyText);
			
			return content;
		}

		private InteractiveHeaderedItem MessageToContent(ContentBody content, InteractiveItemsStack item, bool reply)
		{
			InteractiveHeaderedItem headerContent = PrepareMessagesHeader(content, item, reply);
			
			nodeContents.Add(headerContent);
			nodeContentNames.Add(headerContent, lineHeaderContent.ToString());
			nodeContentsByName.Add(lineHeaderContent.ToString(), headerContent);

			lineHeaderContent++;

			return headerContent;
		}

		protected virtual void PrepareFrame ()
		{
			ActualHeight = classItemContainer.GetAbsoluteContentHeight();

			if (Container != null) return;
			
			shadowpath = new GraphicsPath();
			if (RoundedCorners)
			{
				int radius = CornerRadius;
				shadowpath.AddArc(ActualWidth-radius + 4, 3, radius, radius, 300, 60);
				shadowpath.AddArc(ActualWidth-radius + 4, ActualHeight - radius + 3, radius, radius, 0, 90);
				shadowpath.AddArc(4, ActualHeight-radius + 3, radius, radius, 90, 45);
				shadowpath.AddArc(ActualWidth-radius, ActualHeight - radius, radius, radius, 90, -90);
			}
			else
			{
				shadowpath.AddPolygon(new PointF[] {
				                      new PointF(ActualWidth, 3),
				                      new PointF(ActualWidth + 4, 3),
				                      new PointF(ActualWidth + 4, ActualHeight + 3),
				                      new PointF(4, ActualHeight + 3),
				                      new PointF(4, ActualHeight),
				                      new PointF(ActualWidth, ActualHeight)
				});
			}
			shadowpath.CloseFigure();
		}
		#endregion

		// вычисление положения точки для начала соединяющей линии
		public void OffsetPointTo()
		{
			if (dataNode.LinkedToNodes.Count == 0) return;
			int line = 0;
			float offsetY = (!classItemHeaderedContent.Collapsed) ? titles.GetAbsoluteContentHeight() + 5 : 0;
			foreach (var item in items)
			{
				if (!classItemHeaderedContent.Collapsed) offsetY += item.GetAbsoluteContentHeight() + items.Spacing;
				SetPointTo(++line, offsetY); 
			}
		}

		private void SetPointTo(int line, float offset)
		{
			List<LinkTo> pointList = dataNode.LinkedToNodes;
			for (int i = 0; i < pointList.Count; i++)
			{
				if (pointList[i].ContentLine > line) return;
				if (pointList[i].ContentLine == line) {
					pointList[i].PointTo = offset;
					return;
				}
			}
		}

		public override void DrawToGraphics (Graphics graphics)
		{
			grad.ResetTransform();
			grad.TranslateTransform(AbsoluteX, AbsoluteY);
			grad.ScaleTransform(ActualWidth, 1);
			
			GraphicsState state = graphics.Save();
			graphics.TranslateTransform (AbsoluteX, AbsoluteY);
			
			if (Container == null) {
				//Draw Shadow
				graphics.FillPath(CanvasItem.ShadowBrush, shadowpath);
			}
			
			classItemContainer.Width = Width;
			classItemContainer.Height = Height;
				
			graphics.Restore(state);
			classItemContainer.DrawToGraphics(graphics);
			
			#region Draw interfaces lollipops
			//TODO - should be converted to an headered item.
			if (nodeDescription.Count > 0)
			{
				nodeDescription.X = AbsoluteX + 15;
				nodeDescription.Y = AbsoluteY - nodeDescription.ActualHeight - 3;
				nodeDescription.DrawToGraphics(graphics);
				
				//graphics.DrawEllipse(Pens.Black, AbsoluteX + 9, AbsoluteY - interfaces.ActualHeight - 11, 10, 10);
				//graphics.DrawLine(Pens.Black, AbsoluteX + 14, AbsoluteY - interfaces.ActualHeight - 1, AbsoluteX + 14, AbsoluteY);
			}
			#endregion

			base.DrawToGraphics(graphics);
		}
		
		#region Behaviour
		public bool Hidden { set; get; }

		public bool Collapsed
		{
			get { return classItemHeaderedContent.Collapsed; }
			set
			{
				classItemHeaderedContent.Collapsed = value;
				collapseExpandShape.Collapsed = value;
				PrepareFrame();
				EmitLayoutUpdate();

				OffsetPointTo();
			}
		}
		
		public bool ViewAllNodeCode
		{
			get { return viewCodeShape.IsPressed; }
			set	{ viewCodeShape.IsPressed = value; }
		}

		private void HandleRedraw (object sender, EventArgs args)
		{
			PrepareFrame();
			EmitLayoutUpdate();
		}
		
		public override void HandleMouseClick (PointF pos)
		{
			base.HandleMouseClick(pos);

			if (collapseExpandShape.IsInside(pos.X, pos.Y)) {
				Collapsed = !Collapsed;
			}
			else if (viewCodeShape.IsInside(pos.X, pos.Y)) {
				ViewAllNodeCode = !ViewAllNodeCode;
				dataNode.SetStateShowNodeCodeButton();
				dataNode.ShowCodeNodeButton = ViewAllNodeCode;

				ShowCodeButtonClick(this);
			} else {
				foreach (var item in items)
				{
					if ((item is DrawableItemsStack<InteractiveHeaderedItem>) == false) continue;
					foreach (InteractiveHeaderedItem header in (DrawableItemsStack<InteractiveHeaderedItem>)item)
					{
						if (header.HitTest(pos)) {
							header.HandleMouseClick(pos);
							OffsetPointTo();
						}
					}
				}
			}
		}
		#endregion
		
		#region Storage
		
		protected override XmlElement CreateXmlElement(XmlDocument doc)
		{
			return doc.CreateElement("Node");
		}
		
		protected override void FillXmlElement(XmlElement element, XmlDocument document)
		{
			base.FillXmlElement(element, document);
			element.SetAttribute("Name", dataNode.Name);
			element.SetAttribute("Collapsed", Collapsed.ToString());
			element.SetAttribute("Hidden", Hidden.ToString());
			if (dataNode.GetStateShowNodeCodeButton()) element.SetAttribute("ShowCode", dataNode.ShowCodeNodeButton.ToString());

			//<Contents>
			if (nodeContents.Count == 0)
				return;
			
			XmlElement nodeContentDoc = document.CreateElement("Contents");
			
			foreach (InteractiveHeaderedItem tg in nodeContents)
			{
				XmlElement grp = document.CreateElement("Content");
				grp.SetAttribute("Line", nodeContentNames[tg]);
				grp.SetAttribute("Collapsed", tg.Collapsed.ToString());
				nodeContentDoc.AppendChild(grp);
			}
			element.AppendChild(nodeContentDoc);
		}
		
		public override void LoadFromXml (XPathNavigator navigator)
		{
			base.LoadFromXml(navigator);
			
			Collapsed = bool.Parse(navigator.GetAttribute("Collapsed", ""));
			Hidden = bool.Parse(navigator.GetAttribute("Hidden", ""));

			showCode = -1;
			string sc = navigator.GetAttribute("ShowCode", "");
			if (!String.IsNullOrEmpty(sc)) {
				showCode = bool.Parse(sc) ? 1 : 0;
			}

			XPathNodeIterator contentNI = navigator.Select("Contents/Content");
			
			while (contentNI.MoveNext())
			{
				XPathNavigator compNav = contentNI.Current;
				InteractiveHeaderedItem grp;
				if (nodeContentsByName.TryGetValue(compNav.GetAttribute("Line", ""), out grp))
				{
					grp.Collapsed = bool.Parse(compNav.GetAttribute("Collapsed", ""));
				}
			}
		}
		#endregion
		
		public void Dispose()
		{
			grad.Dispose();
			if (shadowpath != null)
				shadowpath.Dispose();
		}
		
		public override string ToString()
		{
			return "\"" + dataNode.Name + "\"";
		}
	}
}
