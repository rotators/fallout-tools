namespace ICSharpCode.ClassDiagram
{
	public class LinkTo
	{
		private string name;    // имя ноды к которой идет соединение

		private int line;       // номер строки в контенте переданной процедуры
		private int tempLine;

		private float point;    // смещение от которой будет начинаться соединяющая линия

		public LinkTo(string name, int line)
		{
			this.name = name;
			this.line = line;
		}

		public string NameTo
		{
			get { return name; }
		}

		public int ContentLine
		{
			get { return line; }
		}

		public float PointTo
		{
			get { return point; }
			set { point = value; }
		}

		public int SetLine
		{
			get { return (tempLine != 0) ? tempLine : line; }
			set { tempLine = value; }
		}

		/// <summary>
		/// Фиксирует установенный номер строки в SetLine для котента
		/// </summary>
		public void CommitLine()
		{
			if (tempLine != 0) {
				line = tempLine;
				tempLine = 0;
			}
		}
	}
}
