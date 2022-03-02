using System.Collections.Generic;

using ScriptEditor.CodeTranslation;

namespace ICSharpCode.ClassDiagram
{
	public class ContentBody
	{
		public string scrText;      // исходный код строки в процедуре
		public string msgText;      // текст в строке, если строка содержит необходимый опкод, иначе null
		public OpcodeType type;     // тип опкода
		public int msgNum;          // номер Message строки
		public int msgFileNum;      // номер Message файла
		public int index;           // номер ?

		public ContentBody(string scrText, string msgText, OpcodeType type) : this (scrText, msgText, type, -1, -1, -1) { }

		public ContentBody(string scrText, string msgText, OpcodeType type, int msgNum, int msgFileNum, int index)
		{
			this.scrText    = scrText;
			this.type       = type;
			this.msgText    = msgText;
			this.msgNum     = msgNum;
			this.msgFileNum = msgFileNum;
			this.index      = index;
		}
	}

	public class DataNode : INode
	{
		string name;                                            // this node name
		NodesType nodeType;                                     // визуальный тип ноды
		List<LinkTo> linkedTo;                                  // список нод на которые ссылается эта нода
		List<string> linkedForm;                                // список имен нод которые ссылается на эту ноду
		List<ContentBody> content = new List<ContentBody> ();   // контент ноды (по строковое содержимое процедуры)

		// конструктор
		public DataNode(string name, List<LinkTo> linkedTo, List<string> linkedForm , List<ContentBody> content, NodesType nodeType)
		{
			this.name       = name;
			this.nodeType   = nodeType;
			this.linkedTo   = linkedTo;
			this.linkedForm = linkedForm;
			this.content    = content;
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public NodesType NodeType {
			get { return nodeType; }
			//set { nodeType = value; }
		}

		public List<LinkTo> LinkedToNodes {
			get { return linkedTo; }
			set { linkedTo = value; }
		}

		public List<string> LinkedFromNodes {
			get { return linkedForm; }
			set { linkedForm = value; }
		}

		public List<ContentBody> NodeContent {
			get { return content; }
			set { content = value; }
		}

		// bit 1 - состояние кнопки
		// bit 2 - флаг пользовательского состоянии кнопки
		private int showAllNodeCode;

		public bool ShowCodeNodeButton
		{
			get { return (showAllNodeCode & 0x1) != 0; }
			set {
					if (value)
						showAllNodeCode |= 0x1;
					else
						showAllNodeCode &= ~0x1;
				}
		}

		public void SetStateShowNodeCodeButton()
		{
			showAllNodeCode |= 0x2;
		}

		public bool GetStateShowNodeCodeButton()
		{
			return (showAllNodeCode & 0x2) != 0;
		}
	}
}
