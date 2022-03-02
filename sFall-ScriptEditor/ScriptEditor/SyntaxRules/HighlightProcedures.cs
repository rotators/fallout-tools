using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using ScriptEditor.CodeTranslation;

using ICSharpCode.TextEditor.Document;

namespace ScriptEditor.SyntaxRules
{
    internal class HighlightProcedures
    {
        static public void AddAllToList(IDocument document, Procedure[] procs)
        {
            bool refresh = false;
            HighlightExtraWord procedures = document.ExtraWordList;
            foreach (var p in procs)
            {
                string name = p.Name;
                if (!p.IsStandart() && !procedures.WordExist(name)) { // ProgramInfo.opcodes_list.ContainsKey(name)
                    procedures.AddToList(document, name);
                    refresh = true;
                }
            }
            if (refresh) document.HighlightingStrategy.MarkTokens(document);
        }

        static public void AddToList(IDocument document, string name)
        {
            document.ExtraWordList.AddToList(document, name.ToLowerInvariant());
            document.HighlightingStrategy.MarkTokens(document);
        }

        static public void DeleteFromList(IDocument document, string name)
        {
            document.ExtraWordList.RemoveFromList(name.ToLowerInvariant());
            document.HighlightingStrategy.MarkTokens(document);
        }

        static public void UpdateList(IDocument document, Procedure[] procs)
        {
            var pList = new HashSet<string>();
            foreach (var p in procs)
            {
                if (p.IsStandart()) continue;

                string name = p.Name;
                pList.Add(name);
                if (!document.ExtraWordList.WordExist(name)) {
                    document.ExtraWordList.AddToList(document, name); // add procedure
                }
            }
            document.ExtraWordList.WordListRefresh(pList); // remove outdate procedures
            document.HighlightingStrategy.MarkTokens(document);
        }
    }
}
