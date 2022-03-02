using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.TextEditorUI;

namespace ScriptEditor.TextEditorUtilities
{
    internal static class MessageStructure
    {
        const int Open = 0, Close = 1;
        static char[] Curves = new char[] { '{', '}' };

        public static List<Error> CheckStructure(TextAreaControl TAC, string file)
        {
            TAC.Document.MarkerStrategy.RemoveAll(delegate(TextMarker tm) { return true; });
            
            int openCurve = 0, closeCurve = 0;
            
            int[] openOffsetCurve = new int[2];
            int closeOffsetCurve = 0;

            int offsetError = -1;
            int offsetOpen = -1;
            int openCount = 0;
            
            //bool errorOpenCurve = false, errorCloseCurve = false;
            bool only_once = false, new_line = false;

            List<int> numbersLine = new List<int>();

            List<int> warning_comment = new List<int>();
            List<Error> error_number = new List<Error>();
            List<Error> warning = new List<Error>();
            List<Error> report = new List<Error>();

            char[] chars = new char[] { '.', ',', '!', '?'};

            int len = TAC.Document.TextContent.Length;
            for (int offset = 0; offset < len; offset++)
	        {
                char ch = TAC.Document.TextContent[offset];
                if (ch == '\r' || ch == '\n') {
                    //Check whitespace on new line
                    if (!new_line && openCurve == 1 && closeCurve == 0 && openCount == 3) {
                        int i = -1;
                        bool ws = false;
                        do {
                            ws = (TAC.Document.TextContent[offset + i] == ' ');
                            i++;
                        } while (!ws && i < 3);
                        if (!ws)
                            warning.Add(new Error(ErrorType.Message, "New line the whitespace character is missing.", null, offset - 1));
                    }
                    new_line = true;
                    only_once = false;
                    continue;
                }

                if (ch == Curves[Open]) {
                    openOffsetCurve[1] = openOffsetCurve[0];
                    openOffsetCurve[0] = offset;
                    openCurve++;
                    if (closeCurve > 0)
                        closeCurve--;

                    openCount++;
                    if (openCount == 1) //записываем offset кавычки в первой группе
                        offsetOpen = offset + 1;

                } else if (ch == Curves[Close]) {
                    closeOffsetCurve = offset;
                    closeCurve++;
                    openCurve--;

                    if (openCount == 1) { //проверка корректности номера строки в первой группе кавычек
                        int ln = offset - offsetOpen;
                        string str = TAC.Document.GetText(offsetOpen, ln);
                        
                        if (str.Trim().Length > 0) {
                            int number;
                            if (int.TryParse(str, out number) && number > 0) {
                                if (numbersLine.Contains(number))
                                    error_number.Add(new Error(
                                                     ErrorType.Warning, "Duplicate message line number: " + number, null, offsetOpen, ln));
                                else
                                    numbersLine.Add(number);
                            } else
                                error_number.Add(new Error(
                                                 ErrorType.Error, "Invalid line number of the message.", null, offsetOpen, ln));
                        } else
                            error_number.Add(new Error(
                                                 ErrorType.Error, "Missing line number of the message.", null, offsetOpen, ln));

                    } else if (openCount == 3)
                        openCount = 0;
                }
     
                if (closeCurve > 1) {
                    //errorOpenCurve = true;
                    offsetError = closeOffsetCurve;
                    break;
                }
                if (openCurve > 1) {
                    //errorCloseCurve = true;
                    offsetError = openOffsetCurve[1];
                    break;
                } else if (openCurve < 0) {
                    //errorCloseCurve = true;
                    offsetError = closeOffsetCurve;
                    break;
                }

                //Check whitespace on punctuation
                if (openCurve == 1 && closeCurve == 0 && Utilities.IsAnyChar(ch, chars)) {
                    char chCheck = TAC.Document.TextContent[offset + 1];
                    if (char.IsLetter(chCheck))
                        warning.Add(new Error(ErrorType.Message, "The whitespace character is missing.", null, offset));

                    if (char.IsWhiteSpace(chCheck) && TAC.Document.TextContent[offset - 1] != chars[0] 
                        && TAC.Document.TextContent[offset] != chars[1] && char.IsLower(TAC.Document.TextContent, offset + 2))
                        warning.Add(new Error(ErrorType.Message, "The sentence begins with a small letter instead of the capital letter.", null, offset + 2));
                }

                // Comment check characters
                if (openCurve == 0 && closeCurve == 1) {
                    if (!only_once && !char.IsWhiteSpace(ch) && ch != Curves[Open] && ch != Curves[Close]) {
                        only_once = true;
                        if (ch != '#')
                            warning_comment.Add(offset);
                    }
                } else
                    only_once = false;

                new_line = false;
            }
            
            #region Build reporting error
            foreach (Error error in error_number) 
            {
                TextLocation tLoc =  TAC.Document.OffsetToPosition(error.line);
                if (error.column == 0) {
                    error.column = 2;
                    error.line--;
                }
                TextMarker tm = new TextMarker(error.line, error.column, TextMarkerType.WaveLine, Color.Red);
                tm.ToolTip = error.message;
                TAC.Document.MarkerStrategy.AddMarker(tm);

                report.Add(new Error(error.type, error.message, file, tLoc.Line + 1, tLoc.Column));
                if (report.Count == 1)
                    TAC.Caret.Position = tLoc;
            }

            if (offsetError > -1) {
                TextLocation tLoc =  TAC.Document.OffsetToPosition(offsetError);
                TAC.Caret.Position = tLoc;
                LineSegment ls = TAC.Document.GetLineSegment(tLoc.Line);
                TextMarker tm = new TextMarker(ls.Offset, ls.Length, TextMarkerType.WaveLine, Color.Red);
                tm.ToolTip = "Wrong structure of the paired curves brackets.";
                TAC.Document.MarkerStrategy.AddMarker(tm);

                report.Add(new Error(ErrorType.Error, tm.ToolTip, file, tLoc.Line + 1, 0));
            }

            foreach (Error error in warning) 
            {
                TextMarker tm = new TextMarker(error.line - 1, 3, TextMarkerType.WaveLine, Color.Blue);
                tm.ToolTip = error.message; //"The white space character is missing.";
                TAC.Document.MarkerStrategy.AddMarker(tm);

                TextLocation tl = TAC.Document.OffsetToPosition(error.line);
                report.Add(new Error(error.type, error.message, file, tl.Line + 1, tl.Column));
            }

            foreach (int offset in warning_comment) 
            {
                TextLocation tLoc =  TAC.Document.OffsetToPosition(offset);
                LineSegment ls = TAC.Document.GetLineSegment(tLoc.Line);
                int length = offset - ls.Offset;
                length = ls.Length - length;
                TextMarker tm = new TextMarker(offset, length, TextMarkerType.Underlined, Color.ForestGreen);
                tm.ToolTip = "Missing '#' character of comment?";
                TAC.Document.MarkerStrategy.AddMarker(tm);

                report.Add(new Error(ErrorType.None, tm.ToolTip, file, tLoc.Line + 1, tLoc.Column));
            }
            #endregion

            TAC.Refresh();
            return report;
        }
    }
}
