using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Document
{
	public class HighlightExtraWord
	{
		HashSet<string> extraWords = new HashSet<string>();

		static HighlightColor color = null;
		static bool isNoColor = false;

		internal HighlightColor WordColor {
			get {
				return color;
			}
		}

		public int CountWords()
		{
			return extraWords.Count;
		}

		public bool WordExist(string word)
		{
			return (!isNoColor) ? extraWords.Contains(word) : false;
		}

		public void AddToList(IDocument document, string word)
		{
			if (color == null && !isNoColor) UpdateColor(document, false);
			extraWords.Add(word);
		}

		public void RemoveFromList(string word)
		{
			extraWords.Remove(word);
		}

		public void WordListRefresh(HashSet<string> nword)
		{
			extraWords.IntersectWith(nword);
		}

		public void UpdateColor(IDocument document, bool refresh = true)
		{
			isNoColor = false;
			color = null;
			string clr;
			if (document.HighlightingStrategy.Properties.TryGetValue("ProceduresColor", out clr)) {
				color = new HighlightColor(Color.FromName(clr));
			} else {
				isNoColor = true;
			}
			if (refresh) document.HighlightingStrategy.MarkTokens(document);
		}

		public void WordListClear()
		{
			extraWords.Clear();
			color = null;
			isNoColor = false;
		}
	}
}
