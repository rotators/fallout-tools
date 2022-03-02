using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI;

namespace ScriptEditor.TextEditorUtilities
{
    /// <summary>
    /// Class for text editor functions.
    /// </summary>
    internal sealed class Utilities
    {
        private const string endKeyWord = "end";

        internal struct Quote
        {
            public int Open;
            public int Close;
        }

    #region Formating code functions
        // for selected code
        public static void FormattingCode(TextAreaControl TAC)
        {
            string textCode;
            int offset;
            if (TAC.SelectionManager.HasSomethingSelected) {
                ISelection select = TAC.SelectionManager.SelectionCollection[0];
                textCode = select.SelectedText;
                offset = select.Offset;
            } else {
                textCode = TextUtilities.GetLineAsString(TAC.Document, TAC.Caret.Line);
                offset = TAC.Caret.Offset - TAC.Caret.Column;
            }
            TAC.Document.Replace(offset, textCode.Length, FormattingCode(textCode));
        }

        public static void FormattingCodeSmart(TextAreaControl TAC)
        {
            string textCode = TextUtilities.GetLineAsString(TAC.Document, TAC.Caret.Line);

            int col = TAC.Caret.Column;
            int offset = TAC.Caret.Offset - col;

            if (col < textCode.Length)
                textCode = textCode.Remove(col);

            string formatText = FormattingCode(textCode);

            int diffLen = formatText.Length - textCode.Length;
            if (diffLen > 0) {
                TAC.Document.Replace(offset, textCode.Length, formatText);
                TAC.Caret.Column += diffLen;
            }
        }

        public static string FormattingCode(string textCode)
        {
            string[] pattern = { ":=", "!=", "==", ">=", "<=", "+=", "-=", "*=", "/=", "%=", ",", ">", "<", "+", "-", "*", "/", "%" };
            char[] excludeR = { ' ', '=', '+', '-', '*', '/' };
            char[] excludeL = { ' ', '=', '>', '<', '+', '-', '!', ':', '*', '/', '%' };
            char[] excludeD = { ' ', ',', '(' };
            const string space = " ";

            List<Quote> Quotes = new  List<Quote>();

            string[] linecode = textCode.Split('\n');
            for (int i = 0; i < linecode.Length; i++)
            {
                string tmp = linecode[i].TrimStart();
                if (tmp.Length < 3 || (tmp[0] == '/' && (tmp[1] == '/' || tmp[1] == '*'))) continue;
                // check Quotes
                GetQuotesPosition(linecode[i], Quotes);
                foreach (string p in pattern)
                {
                    int n = 0;
                    do {
                        n = linecode[i].IndexOf(p, n);
                        if (n < 0)
                            break;

                        // skiping quotes "..."
                        bool inQuotes = false;
                        foreach (Quote q in Quotes)
                        {
                            if (n > q.Open && n < q.Close) {
                                n = q.Close + 1;
                                inQuotes = true;
                                break;
                            }
                        }
                        if (inQuotes) {
                            if (n >= linecode[i].Length)
                                break;
                            continue;
                        }

                        // insert right space
                        int cPos = n + p.Length;
                        if (linecode[i].Length > cPos && !Char.IsWhiteSpace(linecode[i], cPos)) {
                            if (p.Length == 2)
                                linecode[i] = linecode[i].Insert(n + 2, space);
                            else {
                                if (linecode[i].IndexOfAny(excludeR, n + 1, 1) == -1) {
                                    if ((p == "-" && Char.IsDigit(linecode[i], n + 1)
                                    && linecode[i].IndexOfAny(excludeD, n - 1, 1) != -1) == false               // check NegDigit
                                    && ((p == "+" || p == "-") && linecode[i][n - 1].ToString() == p) == false) // check '++/--'
                                        linecode[i] = linecode[i].Insert(n + 1, space);
                                }
                            }
                        }
                        // insert left space
                        if (n > 0 && p != "," && !Char.IsWhiteSpace(linecode[i], n - 1)) {
                            if (p.Length == 2)
                                linecode[i] = linecode[i].Insert(n, space);
                            else {
                                if (linecode[i].IndexOfAny(excludeL, n - 1, 1) == -1) {
                                    if ((p == "-" && Char.IsDigit(linecode[i], n + 1)
                                        && linecode[i][n - 1] == '(') == false                                      // check NegDigit
                                        && ((p == "+" || p == "-") && linecode[i][n + 1].ToString() == p) == false) // check '++/--'
                                        linecode[i] = linecode[i].Insert(n, space);
                                }
                            }
                        }

                        n += p.Length;
                    } while (n < linecode[i].Length);
                }
                Quotes.Clear();
            }
            return string.Join("\n", linecode);
        }

        public static void DecIndent(TextAreaControl TAC)
        {
            int indent = -1;
            if (TAC.SelectionManager.HasSomethingSelected) {
                ISelection position = TAC.SelectionManager.SelectionCollection[0];

                for (int i = position.StartPosition.Line; i <= position.EndPosition.Line; i++)
                {
                    CheckSpacesIndent(i, ref indent, TAC.Document);
                }

                if (indent <= 0)
                    return;
                TAC.Document.UndoStack.StartUndoGroup();

                for (int i = position.StartPosition.Line; i <= position.EndPosition.Line; i++)
                {
                    SubDecIndent(i, indent, TAC.Document);
                }

                TAC.Document.UndoStack.EndUndoGroup();
                TextLocation srtSel = TAC.SelectionManager.SelectionCollection[0].StartPosition;
                TextLocation endSel = TAC.SelectionManager.SelectionCollection[0].EndPosition;
                srtSel.Column -= indent;
                endSel.Column -= indent;
                TAC.SelectionManager.SetSelection(srtSel, endSel);
            } else {
                int line = TAC.Caret.Line;
                CheckSpacesIndent(line, ref indent, TAC.Document);
                if (indent <= 0 || SubDecIndent(line, indent, TAC.Document))
                    return;
            }
            TAC.Caret.Column -= indent;
            //TAC.Refresh();
        }

        private static void CheckSpacesIndent(int line, ref int indent, IDocument document)
        {
            string LineText = TextUtilities.GetLineAsString(document, line);
            int len = LineText.Length;
            int trimlen = LineText.TrimStart().Length;
            if (len == 0 || trimlen == 0)
                return;

            int spacesLen = (len - trimlen);
            if (indent == -1) {
                // Adjust indent
                int adjust = spacesLen % Settings.tabSize;
                indent = (adjust > 0) ? adjust : Settings.tabSize;
            }
            if (spacesLen < indent)
                indent = spacesLen;
        }

        private static bool SubDecIndent(int line, int indent, IDocument document)
        {
            if (TextUtilities.GetLineAsString(document, line).TrimStart().Length == 0)
                return true;
            document.Remove(document.LineSegmentCollection[line].Offset, indent);
            return false;
        }

        public static void CommentText(TextAreaControl TAC)
        {
            string commentLine = TAC.Document.HighlightingStrategy.Properties["LineComment"];

            if (TAC.SelectionManager.HasSomethingSelected) {
                TAC.Document.UndoStack.StartUndoGroup();
                ISelection position = TAC.SelectionManager.SelectionCollection[0];
                for (int i = position.StartPosition.Line; i <= position.EndPosition.Line; i++)
                {
                    string LineText = TextUtilities.GetLineAsString(TAC.Document, i);
                    string TrimText = LineText.TrimStart();
                    if (TrimText.StartsWith(commentLine))
                        continue;
                    int offset = LineText.Length - TrimText.Length;
                    offset += TAC.Document.LineSegmentCollection[i].Offset;
                    TAC.Document.Insert(offset, commentLine);
                }
                TAC.Document.UndoStack.EndUndoGroup();
                TAC.SelectionManager.ClearSelection();
            } else {
                string LineText = TextUtilities.GetLineAsString(TAC.Document, TAC.Caret.Line);
                string TrimText = LineText.TrimStart();
                if (TrimText.StartsWith(commentLine))
                    return;
                int offset = LineText.Length - TrimText.Length;
                offset += TAC.Document.LineSegmentCollection[TAC.Caret.Line].Offset;
                TAC.Document.Insert(offset, commentLine);
            }
            TAC.Caret.Column += commentLine.Length;
        }

        public static void UnCommentText(TextAreaControl TAC)
        {
            string commentLine = TAC.Document.HighlightingStrategy.Properties["LineComment"];
            int lenComment = commentLine.Length;

            if (TAC.SelectionManager.HasSomethingSelected) {
                TAC.Document.UndoStack.StartUndoGroup();
                ISelection position = TAC.SelectionManager.SelectionCollection[0];
                for (int i = position.StartPosition.Line; i <= position.EndPosition.Line; i++)
                {
                    string LineText = TextUtilities.GetLineAsString(TAC.Document, i);
                    if (!LineText.TrimStart().StartsWith(commentLine))
                        continue;
                    int n = LineText.IndexOf(commentLine);
                    int offset_str = TAC.Document.LineSegmentCollection[i].Offset;
                    TAC.Document.Remove(offset_str + n, lenComment);
                }
                TAC.Document.UndoStack.EndUndoGroup();
                TAC.SelectionManager.ClearSelection();
            } else {
                string LineText = TextUtilities.GetLineAsString(TAC.Document, TAC.Caret.Line);
                if (!LineText.TrimStart().StartsWith(commentLine))
                    return;
                int n = LineText.IndexOf(commentLine);
                int offset_str = TAC.Document.LineSegmentCollection[TAC.Caret.Line].Offset;
                TAC.Document.Remove(offset_str + n, lenComment);
            }
            TAC.Caret.Column -= lenComment;
        }

        public static void AlignToLeft(TextAreaControl TAC)
        {
            if (TAC.SelectionManager.HasSomethingSelected) {
                ISelection position = TAC.SelectionManager.SelectionCollection[0];
                string LineText = TextUtilities.GetLineAsString(TAC.Document, position.StartPosition.Line);
                int Align = LineText.Length - LineText.TrimStart().Length; // узнаем длину отступа
                TAC.Document.UndoStack.StartUndoGroup();
                for (int i = position.StartPosition.Line + 1; i <= position.EndPosition.Line; i++)
                {
                    LineText = TextUtilities.GetLineAsString(TAC.Document, i);
                    int len = LineText.Length - LineText.TrimStart().Length;
                    if (len == 0 || len <= Align) continue;
                    int offset = TAC.Document.LineSegmentCollection[i].Offset;
                    TAC.Document.Remove(offset, len-Align);
                }
                TAC.Document.UndoStack.EndUndoGroup();
            }
        }
    #endregion

    # region Search Function
        public static bool Search(string text, string str, Regex regex, int start, bool restart, bool mcase, out int mstart, out int mlen)
        {
            if (start >= text.Length) start = 0;
            mstart = 0;
            mlen = str.Length;
            if (regex != null) {
                Match m = regex.Match(text, start);
                if (m.Success) {
                    mstart = m.Index;
                    mlen = m.Length;
                    return true;
                }
                if (!restart) return false;
                m = regex.Match(text);
                if (m.Success) {
                    mstart = m.Index;
                    mlen = m.Length;
                    return true;
                }
            } else {
                int i = text.IndexOf(str, start, (mcase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
                if (i != -1) {
                    mstart = i;
                    return true;
                }
                if (!restart) return false;
                i = text.IndexOf(str, (mcase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
                if (i != -1) {
                    mstart = i;
                    return true;
                }
            }
            return false;
        }

        public static bool Search(string text, string str, Regex regex, bool mcase)
        {
            return (regex == null)
                    ? (text.IndexOf(str, (mcase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) != -1)
                    : regex.IsMatch(text);
        }

        public static bool SearchAndScroll(TextAreaControl TAC, Regex regex, string searchText, bool mcase, ref ScriptEditor.TextEditor.PositionType type, bool searchContinue = true)
        {
            int start, len;
            if (Search(TAC.Document.TextContent, searchText, regex, (searchContinue) ? TAC.Caret.Offset + 1 : 0, true, mcase, out start, out len)) {
                FindSelected(TAC, start, len, ref type);
                return true;
            }
            return false;
        }

        public static void FindSelected(TextAreaControl TAC, int start, int len, ref ScriptEditor.TextEditor.PositionType type, string replace = null)
        {
            type = ScriptEditor.TextEditor.PositionType.NoStore;

            TextLocation locstart = TAC.Document.OffsetToPosition(start);
            TextLocation locend = TAC.Document.OffsetToPosition(start + len);
            TAC.SelectionManager.SetSelection(locstart, locend);
            if (replace != null) {
                TAC.Document.Replace(start, len, replace);
                locend = TAC.Document.OffsetToPosition(start + replace.Length);
                TAC.SelectionManager.SetSelection(locstart, locend);
            }
            TAC.Caret.Position = locstart;
            TAC.CenterViewOn(locstart.Line, Math.Max(2, TAC.TextArea.TextView.VisibleLineCount / 4));
        }

        public static void SearchForAll(TabInfo tab, string searchText, Regex regex, bool mcase, DataGridView dgv, List<int> offsets, List<int> lengths)
        {
            int start, len, line, lastline = -1;
            int offset = 0;
            while (Search(tab.textEditor.Text, searchText, regex, offset, false, mcase, out start, out len))
            {
                offset = start + 1;
                line = tab.textEditor.Document.OffsetToPosition(start).Line;
                if (offsets != null) {
                    offsets.Add(start);
                    lengths.Add(len);
                }
                if (line != lastline) {
                    lastline = line;
                    string message = TextUtilities.GetLineAsString(tab.textEditor.Document, line).Trim();
                    Error error = new Error(message, tab.filepath, line + 1, tab.textEditor.Document.OffsetToPosition(start).Column + 1, len);
                    dgv.Rows.Add(tab.filename, error.line.ToString(), error);
                }
            }
        }

        public static void SearchForAll(string[] text, string file, string searchText, Regex regex, bool mcase, DataGridView dgv)
        {
            bool matched;
            for (int i = 0; i < text.Length; i++)
            {
                if (regex != null)
                    matched = regex.IsMatch(text[i]);
                else
                    matched = text[i].IndexOf(searchText, (mcase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) != -1;
                if (matched) {
                    Error error = new Error(text[i].Trim(), file, i + 1);
                    dgv.Rows.Add(Path.GetFileName(file), (i + 1).ToString(), error);
                }
            }
        }

        public static int SearchPanel(string text, string find, int start, bool icase, bool wholeword, bool back = false)
        {
            int z;
            if (wholeword) {
                RegexOptions option = RegexOptions.Multiline;
                if (!icase)
                    option |= RegexOptions.IgnoreCase;
                if (back)
                    option |= RegexOptions.RightToLeft;
                z = SearchWholeWord(text, find, start, option);
            } else {
                StringComparison sc = (!icase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                z = (back) ? text.LastIndexOf(find, start, sc)
                           : text.IndexOf(find, start, sc);
            }
            return z;
        }

        public static int SearchWholeWord(string text, string find, int start, RegexOptions option)
        {
            int z, x;
            string search = @"\b" + Regex.Escape(find) + @"\b";
            Regex s_regex = new Regex(search, option);

            if (!Search(text, find, s_regex, start, false, false, out z, out x))
                return -1;

            return z;
        }

        internal static bool IsAnyChar(char ch, char[] chars)
        {
            bool result = false;
            foreach (char c in chars)
            {
                if (c == ch)
                    result = true;
            }
            return result;
        }
    #endregion

    #region Create/Delete procedure function
        internal static void PrepareDeleteProcedure(Procedure proc, IDocument document)
        {
            string def_poc;

            ParserInternal.UpdateParseBuffer(document.TextContent);
            ProcedureBlock block = ParserInternal.GetProcedureBlock(proc.name, 0, true);
            block.declar = proc.d.declared;

            document.UndoStack.StartUndoGroup();
            DeleteProcedure(document, block, out def_poc);
            document.UndoStack.EndUndoGroup();
        }

        // Remove multi procedures
        internal static void DeleteProcedure(List<string> procName, IDocument document)
        {
            document.UndoStack.StartUndoGroup();
            foreach (var name in procName)
            {
                string def_poc;

                ParserInternal.UpdateParseBuffer(document.TextContent);
                ProcedureBlock block = ParserInternal.GetProcedureBlock(name, 0, true);
                block.declar = ParserInternal.GetDeclarationProcedureLine(name) + 1;
                DeleteProcedure(document, block, out def_poc);
            }
            document.UndoStack.EndUndoGroup();
        }

        internal static void DeleteProcedure(IDocument document, ProcedureBlock block, out string def_poc)
        {
            ISegment segmentS = document.GetLineSegment(block.begin);
            ISegment segmentE = document.GetLineSegment(block.end);
            int len = (segmentE.Offset + segmentE.Length) - segmentS.Offset;

            int inc_s = 0;
            int inc_e = 2;
            if (block.end < document.TotalNumberOfLines - 1) {
                if (segmentS.Offset + len + 4 <= document.TextLength) {
                    if (document.GetLineSegment(block.end + 1).Length == 0) {
                        if (block.begin > 0 && document.GetLineSegment(block.begin - 1).Length == 0) {
                            len += 2;
                            inc_s = -2;
                        }
                    }
                }
                else
                    inc_e = 0;
            } else
                inc_e = 0;
            document.Remove(segmentS.Offset + inc_s, len + inc_e);

            // declare
            int declarLine = block.declar - 1;
            if (declarLine > -1 & declarLine != block.begin) {
                def_poc = TextUtilities.GetLineAsString(document, declarLine);
                int offset = document.PositionToOffset(new TextLocation(0, declarLine));
                document.Remove(offset, def_poc.Length + 2);
            } else
                def_poc = null;
        }

        // Create procedure block
        internal static void InsertProcedure(TextAreaControl TAC, string name, string procblock, int declrLine, int procLine, ref int caretline)
        {
            TAC.Document.UndoStack.StartUndoGroup();
            TAC.SelectionManager.ClearSelection();

            // proc body paste
            int len = TextUtilities.GetLineAsString(TAC.Document, procLine).Trim().Length;
            if (len > 0) {
                procblock = Environment.NewLine + procblock;
                caretline++;
            }
            int offset = TAC.Document.PositionToOffset(new TextLocation(len, procLine));
            TAC.Document.Insert(offset, procblock);

            // declared paste
            offset = TAC.Document.PositionToOffset(new TextLocation(0, declrLine));
            TAC.Document.Insert(offset, "procedure " + name + ";" + Environment.NewLine);

            TAC.Document.UndoStack.EndUndoGroup();
        }
    #endregion

    #region Misc code function
        internal static void HighlightingSelectedText(TextAreaControl TAC)
        {
            List<TextMarker> marker = TAC.Document.MarkerStrategy.GetMarkers(0, TAC.Document.TextLength);
            foreach (TextMarker m in marker)
            {
                if (m.TextMarkerType == TextMarkerType.SolidBlock)
                    TAC.Document.MarkerStrategy.RemoveMarker(m);
            }
            if (!TAC.SelectionManager.HasSomethingSelected) return;

            string sWord = TAC.SelectionManager.SelectedText.Trim();
            int wordLen = sWord.Length;
            if (wordLen == 0 || (wordLen <= 2 && !Char.IsLetterOrDigit(sWord[0]) && wordLen == 2 && !Char.IsLetterOrDigit(sWord[1]))) return;

            string word = TAC.Document.LineSegmentCollection[TAC.Caret.Line].GetWord(TAC.SelectionManager.SelectionCollection[0].StartPosition.Column).Word;
            bool isWordHighlighting = (wordLen == word.Length && sWord == word);

            int seek = 0;
            while (seek < TAC.Document.TextLength) {
                seek = TAC.Document.TextContent.IndexOf(sWord, seek);
                if (seek == -1) break;
                char chS = '\0', chE = '\0';
                if (isWordHighlighting) {
                    chS = (seek > 0) ? TAC.Document.GetCharAt(seek - 1) : ' ';
                    chE = ((seek + wordLen) < TAC.Document.TextLength) ? TAC.Document.GetCharAt(seek + wordLen) : ' ';
                }
                if (!isWordHighlighting || !(Char.IsLetterOrDigit(chS) || chS == '_') && !(Char.IsLetterOrDigit(chE) || chE == '_'))
                    TAC.Document.MarkerStrategy.AddMarker(new TextMarker(seek, sWord.Length, TextMarkerType.SolidBlock,
                                                          ColorTheme.SelectedHighlight.BackgroundColor, ColorTheme.SelectedHighlight.Color));
                seek += wordLen;
            }
            TAC.SelectionManager.ClearSelection();
        }

        // Auto selected text color region
        internal static void SelectedTextColorRegion(TextLocation position, TextAreaControl TAC)
        {
            if (position.IsEmpty) position = TAC.Caret.Position;

            HighlightColor hc = TAC.Document.GetLineSegment(position.Line).GetColorForPosition(position.Column);
            if (hc != null && hc.BackgroundColor == ColorTheme.CodeFunctions) {
                int sEnd  = TextUtilities.SearchBracketForward(TAC.Document, TAC.Caret.Offset + 1, '{', '}');
                char c = TAC.Document.GetCharAt(TAC.Caret.Offset);
                if (sEnd == -1) {
                    if (c != '}') return;
                    sEnd = TAC.Caret.Offset;
                }
                int sStart = TextUtilities.SearchBracketBackward(TAC.Document, TAC.Caret.Offset - 1, '{', '}');
                if (sStart == -1) {
                    if (c != '{') return;
                    sStart = TAC.Caret.Offset;
                }
                int len = sEnd - sStart;
                if (len < 2) return;
                TextLocation sSel = TAC.Document.OffsetToPosition(sStart);
                TextLocation eSel = TAC.Document.OffsetToPosition(sEnd + 1);
                if (sSel.Line != eSel.Line || TAC.Document.GetText(sStart, len).IndexOfAny(new char[] { ':', ',' }) != -1) return;
                TAC.SelectionManager.SetSelection(sSel, eSel);
            }
        }

        // Paste autocomplete KeyWord construction code
        internal static bool AutoCompleteKeyWord(TextAreaControl TAC)
        {
            Caret caret = TAC.Caret;
            if (ColorTheme.CheckColorPosition(TAC.Document, caret.Position))
                return false;

            int offsetShift = 1;
            if (TAC.Document.TextLength == caret.Offset) offsetShift++;

            string keyword = TextUtilities.GetWordAt(TAC.Document, caret.Offset - offsetShift);
            if (keyword.Length < 2)
                return false;

            string lineText = TextUtilities.GetLineAsString(TAC.Document, caret.Line);
            int totalWS = 0, totalTab = 0;
            for (int i = 0; i < lineText.Length; i++)
            {
                if (lineText[i] == ' ')
                    totalWS++;
                else if (lineText[i] == '\t')
                    totalTab++;
                else
                     break;
            }

            int indentationSize = TAC.TextEditorProperties.IndentationSize;
            bool convertTab = TAC.TextEditorProperties.ConvertTabsToSpaces;
            if (convertTab) {
                totalTab *= indentationSize;
            } else {
                totalWS /= indentationSize;
            }
            int columnStart = totalWS + totalTab;

            int keyStart = TextUtilities.FindWordStart(TAC.Document, caret.Offset - offsetShift);
            int keyLen = caret.Offset - keyStart;
            bool cutKeyWord = keyword.Length > keyLen;
            if (cutKeyWord) keyword = keyword.Remove(keyLen);

            string beginIndent = String.Empty;
            if (caret.Column > keyword.Length) beginIndent = new String((convertTab) ? ' ' : '\t', columnStart);

            string indentation = beginIndent;
            if (convertTab)
                indentation += new String(' ', indentationSize);
            else
                indentation += '\t';

            int lineShift = 2, columnShift = 0;
            if (convertTab) {
                if (totalTab > 0) {
                    columnShift -= (totalTab / indentationSize);
                    columnShift += columnShift;
                }
            } else if (totalWS > 0) columnShift = (totalWS * indentationSize) - totalWS;

            bool keyWordMatch = true;
            string code = null;
            TAC.Document.UndoStack.StartUndoGroup();
            switch (keyword)
            {
                case "for":
                    code = " ({iterator} := 0; {condition}; {iterator}++) begin\r\n" + indentation + "\r\n";
                    columnShift += 2;
                    break;
                case "foreach":
                    code = " ({iterator}: {item} in {array}) begin\r\n" + indentation + "\r\n";
                    columnShift += 6;
                    break;
                case "while":
                    code = " ({condition}) do begin\r\n" + indentation + "\r\n";
                    columnShift += 4;
                    break;
                case "switch":
                    code = " ({condition}) begin\r\n" + indentation
                           + "case {constant} : {code}\r\n" + indentation
                           + "default         : {code}\r\n";
                    lineShift++;
                    columnShift += 5;
                    break;
                case "if":
                    code = " ({condition}) then begin\r\n" + indentation + "\r\n";
                    columnShift++;
                    break;
                case "ifel":
                    code = "({condition}) then begin" + String.Format("{0}{2}{0}{1}else{0}{2}{0}",
                                                        "\r\n", beginIndent, indentation);
                    lineShift = 4;
                    columnShift += 2;
                    TAC.Document.Remove(caret.Offset - 2, 2);
                    break;
                case "elif":
                    code = " if ({condition}) then begin\r\n" + indentation + "\r\n";
                    columnShift += 6;
                    TAC.Document.Replace(caret.Offset - 2, 2, "se");
                    break;
                case "else":
                    code = " begin\r\n" + indentation + "\r\n";
                    lineShift = 1;
                    int isize = (convertTab) ? indentationSize : 1;
                    columnShift = (columnStart + isize) - (columnStart + 3);
                    break;
                case "then":
                    code = " begin";
                    lineShift = 0;
                    columnShift = 0;
                    break;
                case "var":
                    code = "iable ";
                    lineShift = 0;
                    columnShift = 0;
                    break;
                case "proc":
                    code = "edure ";
                    lineShift = 0;
                    columnShift = 0;
                    break;
                default:
                    keyWordMatch = false;
                    break;
            }
            if (keyWordMatch) {
                if (lineShift != 0) {
                    code += beginIndent + endKeyWord + ((cutKeyWord) ? " " : String.Empty);
                }
                TAC.TextArea.InsertString(code);
                TAC.Caret.Position = new TextLocation(caret.Column + columnShift, caret.Line - lineShift); //+ keyword.Length - columnShift
                Utilities.SelectedTextColorRegion(caret.Position, TAC);
            }
            TAC.Document.UndoStack.EndUndoGroup();

            return keyWordMatch;
        }

        internal static void PasteIncludeFile(string sHeaderfile, TextAreaControl TAC)
        {
            if (sHeaderfile != null) {
                int beginLine = 1, endLine = 1;
                foreach (FoldMarker fm in TAC.Document.FoldingManager.FoldMarker) {
                    if (fm.FoldType == FoldType.Region) {
                        beginLine = fm.StartLine;
                        endLine = fm.EndLine;
                        break;
                    }
                }

                string includeText = "#include ";
                var caret = TAC.Caret.Line;
                if (caret >= beginLine && caret <= endLine) {
                    beginLine = caret;
                } else {
                    for (int line = endLine; line > 0; line--) {
                        string ln = TextUtilities.GetLineAsString(TAC.Document, line);
                        if (ln.TrimStart().StartsWith(includeText, StringComparison.OrdinalIgnoreCase)) {
                            beginLine = line + 1;
                            break;
                        }
                    }
                }

                includeText += "\"" + sHeaderfile + "\"" + Environment.NewLine;
                int offset = TAC.Document.PositionToOffset(new TextLocation(0, beginLine));
                TAC.Document.Insert(offset, includeText);
                TAC.Document.MarkerStrategy.AddMarker(new TextMarker(offset, includeText.Length, TextMarkerType.SolidBlock, ColorTheme.IncludeHighlight));
                TAC.ScrollTo(beginLine);
                TAC.Refresh();
            }
        }

    #endregion

    #region Script code text functions
        public static int ReplaceCommonText(Regex s_regex, ref string document, string newText, int differ)
        {
            MatchCollection matches = s_regex.Matches(document);

            int replace_count = 0;
            foreach (Match m in matches)
            {
                int offset = (differ * replace_count) + m.Index;
                document = document.Remove(offset, m.Length);
                document = document.Insert(offset, newText);
                replace_count++;
            }
            return matches.Count;
        }

        // используется для замены имени процедур
        public static int ReplaceSpecialText(Regex s_regex, ref string document, string newText, int differ)
        {
            MatchCollection matches = s_regex.Matches(document);

            int replace_count = 0;
            foreach (Match m in matches)
            {
                var shiftOffset = (m.Groups.Count > 1) ? m.Groups[1].Length : 0;
                int offset = (differ * replace_count) + (m.Index + shiftOffset);

                document = document.Remove(offset, (m.Length - shiftOffset));
                document = document.Insert(offset, newText);
                replace_count++;
            }
            return matches.Count;
        }

        public static void ReplaceIDocumentText(Regex s_regex, IDocument document, string newText, int differ)
        {
            MatchCollection matches = s_regex.Matches(document.TextContent);
            document.UndoStack.StartUndoGroup();

            int replace_count = 0;
            foreach (Match m in matches)
            {
                ReplaceMatch(document, newText, differ * replace_count++, m);
            }
            document.UndoStack.EndUndoGroup();
        }

        public static void ReplaceByReferences(Regex s_regex, IDocument document, IParserInfo pi, string newText, int differ)
        {
            MatchCollection matches = s_regex.Matches(document.TextContent);

            bool isVariable = (pi is Variable);

            bool decl = false, define = isVariable;
            int declared = (isVariable) ? (((Variable)pi).IsArgument) ? ((Variable)pi).adeclared : ((Variable)pi).d.declared
                                        : ((Procedure)pi).d.declared;

            List<Reference> references = pi.References().ToList();
            List<Match> renameReferences = new List<Match>();

            foreach (Match m in matches)
            {
                int line = document.OffsetToPosition(m.Index).Line + 1;

                if (!decl && declared == line) {
                    renameReferences.Add(m);
                    decl = true;
                } else if (!define && ((Procedure)pi).d.defined == line) { // может быть -1 если declared для процедуры не был объявлен
                    renameReferences.Add(m);
                    define = true;
                } else {
                    for (int i = 0; i < references.Count; i++)
                    {
                        if (references[i].line == line) {
                            renameReferences.Add(m);
                            references.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            document.UndoStack.StartUndoGroup();

            int replace_count = 0;
            foreach (Match refMatch in renameReferences)
            {
                ReplaceMatch(document, newText, differ * replace_count++, refMatch);
            }
            document.UndoStack.EndUndoGroup();

            if (references.Count > 0) MessageBox.Show("Some of the references have not been renamed.", "Warning");
        }

        private static void ReplaceMatch(IDocument document, string newText, int replacedCount, Match m)
        {
            var shiftOffset = (m.Groups.Count > 1) ? m.Groups[1].Length : 0;
            int offset = replacedCount + (m.Index + shiftOffset);
            document.Replace(offset, (m.Length - shiftOffset), newText);
        }

        internal static string GetProcedureCode(IDocument document, Procedure curProc)
        {
            if (curProc.d.start == -1 || curProc.d.end == -1) // for imported or w/o body procedure
                return null;

            LineSegment start = document.GetLineSegment(curProc.d.start);
            LineSegment end = document.GetLineSegment(curProc.d.end - 1);
            int length = end.Offset - start.Offset - 2; // -2 не захватываем символы CRLF

            return (length < 0) ? String.Empty : document.GetText(start.Offset, length);
        }

        internal static bool ReplaceProcedureCode(IDocument document,  ProgramInfo pi, string name, string code)
        {
            int index = pi.GetProcedureIndex(name);
            if (index == -1)
                return true; // procedure not found

            LineSegment start = document.GetLineSegment(pi.procs[index].d.start);
            LineSegment end = document.GetLineSegment(pi.procs[index].d.end - 1);
            int length = end.Offset - start.Offset - 2; // -2 не заменяем символы CRLF

            if (length < 0 && code.Length > 0 && !code[code.Length - 1].Equals('\n'))
                code += Environment.NewLine;

            if (length < 0)
                document.Insert(start.Offset, code);
            else
                document.Replace(start.Offset, length, code);

            return false;
        }

        internal static void InsertText(string iText, TextAreaControl TAC)
        {
            TAC.Document.UndoStack.StartUndoGroup();
            if (TAC.SelectionManager.HasSomethingSelected) {
                TextLocation selStart = TAC.SelectionManager.SelectionCollection[0].StartPosition;
                TAC.TextArea.SelectionManager.RemoveSelectedText();
                TAC.Caret.Position = selStart;
            }
            TAC.TextArea.InsertString(iText);
            TAC.Document.UndoStack.EndUndoGroup();
        }

        //Get block text
        internal static string GetRegionText(IDocument document, int _begin, int _end, int _ecol = 0, int _bcol = 0)
        {
            ISegment segmentB = document.GetLineSegment(_begin);
            ISegment segmentE = document.GetLineSegment(_end);

            int Offset = segmentB.Offset + _bcol;
            int Length;
            if (_ecol > 0)
                Length = segmentE.Offset + _ecol;
            else
                Length = segmentE.Offset + segmentE.Length;
            Length -= Offset;

            return document.GetText(Offset, Length);
        }

        //Selected and return block text [NOT USED]
        internal static string GetSelectBlockText(TextAreaControl TAC, int _begin, int _end, int _ecol = -1, int _bcol = 0)
        {
            if (_ecol < 0)
                _ecol = TAC.Document.GetLineSegment(_end).Length;
            TAC.SelectionManager.SetSelection(new TextLocation(_bcol, _begin), new TextLocation(_ecol, _end));
            return TAC.SelectionManager.SelectedText;
        }

        internal static string NormalizeNewLine(string text)
        {
            char[] delimetr = new char[] {'\r', '\n'};

            int offset = 0;
            while (offset < text.Length) {
                offset = text.IndexOfAny(delimetr, offset);
                if (offset == -1)
                    break;
                switch (text[offset]) {
                    case '\r':
                        if (offset + 1 < text.Length) {
                            if (text[++offset] != delimetr[1])
                                text = text.Insert(offset, delimetr[1].ToString());
                        }
                        break;
                    case '\n':
                        if (offset > 0 && text[offset - 1] != delimetr[0])
                            text = text.Insert(offset, delimetr[0].ToString());
                        break;
                }
                offset++;
            };
            return text;
        }

        internal static void ConvertToUnixPlatform(ref string text)
        {
            if (Environment.OSVersion.Platform != PlatformID.Unix) return;
            int offset = 0;
            while (offset < text.Length)
            {
                offset = text.IndexOf('\r', offset);
                if (offset == -1) break;
                text = text.Remove(offset, 1);
                offset++;
            };
        }

        internal static void GetQuotesPosition(string textCode, List<Quote> quotes)
        {
            int openQuotes = textCode.IndexOf('"');
            while (openQuotes > -1)
            {
                Quote position;
                position.Open = openQuotes++;
                position.Close = textCode.IndexOf('"', openQuotes);
                if (position.Close == -1)
                    break;
                quotes.Add(position);
                openQuotes = textCode.IndexOf('"', position.Close + 1);
            };
        }
    #endregion
    }
}
