using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

using ICSharpCode.TextEditor.Document;

namespace ScriptEditor.TextEditorUI
{
    public enum ErrorType { None, Error, Warning, Message, Search, Parser }

    public class Error
    {                                //@"\[\w+\]\s*\<([^\>]+)\>\s*\:(\-?\d+):?(\-?\d+)?\:\s*(.*)"
        private const string pattern = @"(\[\w+\])?\s*\<?([^\>?]+)\>?\s*\:(\-?\d+):?(\-?\d+|\s\w+)?\:\s*(.*)";
        private const string pattern2 = @"\w+\s*([^\>?]+):\s*(\d+):";
        private static readonly System.Drawing.Color BuildErrorColor = System.Drawing.Color.OrangeRed;

        public ErrorType type = ErrorType.None;
        public string message;
        public string fileName;
        public int line;
        public int column = -1;
        public int len = -1;

        public Error(ErrorType type)
        {
            this.type = type;
        }

        public Error(int line, int len)
        {
            this.line = line;
            this.len = len;
        }

        public Error(string line, string column)
        {
            this.line = int.Parse(line) - 1;
            int col;
            int.TryParse(column, out col);
            this.column = col - 1;
        }

        public Error(ErrorType type, string message, string fileName, int line, int column = -1)
        {
            this.type = type;
            this.message = message;
            this.fileName = fileName;
            this.line = line;
            this.column = column;
        }

        public Error(string message, string fileName, int line, int column = -1, int len = -1)
        {
            this.message = message;
            this.fileName = fileName;
            this.line = line;
            this.column = column;
            this.len = len;
        }

        public override string ToString()
        {
            return message;
        }

        // for compile
        public static void BuildLog(List<Error> errors, string output, string srcfile)
        {
            BuildLog(errors, output, srcfile, DiagnosticSuppressionRules.DefaultPath);
        }

        internal static void BuildLog(List<Error> errors, string output, string srcfile, string suppressionPath)
        {
            errors.Clear();
            DiagnosticSuppressionRules suppressions = DiagnosticSuppressionRules.Load(suppressionPath);
            string[] log = output.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            for (int s = 0; s < log.Length; s++)
            {
                bool warning = log[s].IndexOf(": warning:", StringComparison.OrdinalIgnoreCase) > 0;
                bool compilerError = log[s].IndexOf(": error:", StringComparison.OrdinalIgnoreCase) > 0;
                if (log[s].StartsWith("[Error]") || log[s].StartsWith("[Warning]") || log[s].StartsWith("[Message]") || warning || compilerError) {
                    var error = new Error(ErrorType.Message);
                    if (warning || log[s][1] == 'W')
                        error.type = ErrorType.Warning;
                    else if (compilerError || log[s][1] == 'E')
                        error.type = ErrorType.Error;

                    GetLogText(log, s, error);

                    if (suppressions.IsIgnored(error))
                        continue;

                    // File path correct
                    if ((Settings.useMcpp || Settings.useWatcom) && error.fileName != "none") {
                        string scrName = Path.GetFileName(srcfile);
                        if (error.fileName.IndexOf(scrName) > 0)
                            error.fileName = srcfile;
                    }
                    if (error.fileName != "none" && !Path.IsPathRooted(error.fileName)) {
                        error.fileName = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(srcfile), error.fileName));
                    }
                    errors.Add(error);
                }
            }
        }

        private static void GetLogText(string[] log, int s, Error error)
        {
            Match m = Regex.Match(log[s], pattern);
            error.fileName = m.Groups[2].Value.Replace('/', '\\');
            error.line = int.Parse(m.Groups[3].Value);
            if (m.Groups[4].Value.Length > 0 && !char.IsWhiteSpace(m.Groups[4].Value[0]))
                error.column = int.Parse(m.Groups[4].Value);

            error.message = m.Groups[5].Value.TrimEnd();
            if (error.type == ErrorType.Warning && (log.Length - 1) > s)
                error.message += ": " + log[s + 1].Trim();
        }

        // for parser
        public static string ParserLog(string log, TabInfo tab)
        {
            ClearParserErrors(tab);
            DiagnosticSuppressionRules suppressions = DiagnosticSuppressionRules.Load(DiagnosticSuppressionRules.DefaultPath);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("------ Script: {0} < Parse Time: {1} > ------\r\n",
                            tab.filename, DateTime.Now.ToString("HH:mm:ss"));
            bool warn = false, errSection = false, suppressedErrorSection = false;
            string[] sLog = log.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < sLog.Length; i++)
            {
                if (suppressedErrorSection && sLog[i].TrimStart().StartsWith("from")) {
                    continue;
                }
                if (TextEditor.parsingErrors && errSection && sLog[i].TrimStart().StartsWith("from")) {
                    HighlightErrorFrom(sLog[i], tab);
                }
                else if (sLog[i].StartsWith("[Error]")) {
                    Error suppressionCandidate = GetParserSuppressionCandidate(sLog[i]);
                    suppressedErrorSection = suppressions.IsIgnored(suppressionCandidate);
                    if (suppressedErrorSection) {
                        warn = false;
                        errSection = false;
                        continue;
                    }
                    sb.AppendLine();
                    warn = false;
                    errSection = true;
                    HighlightError(sLog[i], tab);
                }
                else if (sLog[i].StartsWith("[Warning]")) {
                    suppressedErrorSection = false;
                    errSection = false;
                    if (!Settings.parserWarn) {
                        warn = true;
                        continue;
                    }
                    var error = new Error(ErrorType.Warning);
                    GetLogText(sLog, i, error);
                    //error.type = ErrorType.Parser;
                    tab.parserErrors.Add(error);
                    sb.AppendLine();
                }
                if (!warn) sb.AppendLine(sLog[i]);
            }
            tab.textEditor.Refresh();

            return sb.ToString();
        }

        private static Error GetParserSuppressionCandidate(string diagnostic)
        {
            Match match = Regex.Match(diagnostic ?? String.Empty, pattern);
            int line;
            if (!match.Success || !Int32.TryParse(match.Groups[3].Value, out line)) return null;

            int column;
            if (!Int32.TryParse(match.Groups[4].Value, out column)) column = 0;
            return new Error(ErrorType.Parser, match.Groups[5].Value.TrimEnd(),
                match.Groups[2].Value, line, column - 1);
        }

        public static void ClearParserErrors(TabInfo tab)
        {
            List<TextMarker> marker = tab.textEditor.Document.MarkerStrategy.TextMarker.ToList();

            foreach (TextMarker m in marker)
            {
                if (m.TextMarkerType == TextMarkerType.WaveLine && m.Color.ToArgb() != BuildErrorColor.ToArgb())
                    tab.textEditor.Document.MarkerStrategy.RemoveMarker(m);
            }

            if (tab.parserErrors.Count > 0) tab.parserErrors.Clear();
        }

        public static void ClearBuildErrorMarkers(TabInfo tab)
        {
            if (tab == null || tab.textEditor == null) return;

            List<TextMarker> markers = tab.textEditor.Document.MarkerStrategy.TextMarker.ToList();
            bool removed = false;
            foreach (TextMarker marker in markers) {
                if (marker.TextMarkerType == TextMarkerType.WaveLine && marker.Color.ToArgb() == BuildErrorColor.ToArgb()) {
                    tab.textEditor.Document.MarkerStrategy.RemoveMarker(marker);
                    removed = true;
                }
            }
            if (removed) tab.textEditor.Refresh();
        }

        public static void HighlightBuildErrors(TabInfo tab)
        {
            ClearBuildErrorMarkers(tab);
            if (tab == null || tab.filepath == null) return;

            foreach (Error error in tab.buildErrors) {
                if (error.type != ErrorType.Error || error.line < 1 || String.IsNullOrEmpty(error.fileName)) continue;

                bool sameFile;
                try {
                    sameFile = Path.IsPathRooted(error.fileName)
                        ? String.Equals(Path.GetFullPath(error.fileName), Path.GetFullPath(tab.filepath), StringComparison.OrdinalIgnoreCase)
                        : String.Equals(Path.GetFileName(error.fileName), tab.filename, StringComparison.OrdinalIgnoreCase);
                } catch {
                    sameFile = String.Equals(Path.GetFileName(error.fileName), tab.filename, StringComparison.OrdinalIgnoreCase);
                }

                if (sameFile) {
                    int markerLine = GetBuildErrorMarkerLine(tab, error);
                    int reportedLine = Math.Max(0, Math.Min(error.line - 1, tab.textEditor.Document.TotalNumberOfLines - 1));
                    error.message = CompilerDiagnosticLineResolver.Clarify(error.message, reportedLine, markerLine,
                        error.column, line => {
                            LineSegment segment = tab.textEditor.Document.GetLineSegment(line);
                            return tab.textEditor.Document.GetText(segment);
                        });
                    AddWaveMarker(tab, markerLine, error.message, BuildErrorColor);
                }
            }
            tab.textEditor.Refresh();
        }

        private static int GetBuildErrorMarkerLine(TabInfo tab, Error error)
        {
            int lastLine = Math.Max(0, tab.textEditor.Document.TotalNumberOfLines - 1);
            int reportedLine = Math.Max(0, Math.Min(error.line - 1, lastLine));
            if (reportedLine == 0 || String.IsNullOrEmpty(error.message)) return reportedLine;

            return CompilerDiagnosticLineResolver.Resolve(reportedLine, error.column, error.message, line => {
                LineSegment segment = tab.textEditor.Document.GetLineSegment(line);
                return tab.textEditor.Document.GetText(segment);
            });
        }

        private static void HighlightError(string error, TabInfo tab)
        {
            Match m = Regex.Match(error, pattern);
            int parserLine;
            if (!m.Success || !Int32.TryParse(m.Groups[3].Value, out parserLine)) return;

            int parserColumn;
            if (!Int32.TryParse(m.Groups[4].Value, out parserColumn)) parserColumn = 0;
            Error ePosition = new Error(parserLine - 1, -1) { column = parserColumn - 1 };
            string message = m.Groups[5].Value.TrimEnd();
            string fpath = m.Groups[2].Value;

            int total = tab.textEditor.Document.TotalNumberOfLines;
            if (ePosition.line < 0) ePosition.line = 0;
            if (ePosition.line >= total)
                ePosition.line = total - 1;

            if (TextEditor.parsingErrors && String.Equals(Path.GetFileName(fpath), tab.filename, StringComparison.OrdinalIgnoreCase)) {
                AddWaveMarker(tab, ePosition.line, message, ColorTheme.HighlightError);
                fpath = tab.filepath;
            }
            // add to error tab
            tab.parserErrors.Add(new Error(ErrorType.Error, message, fpath, ePosition.line + 1, ePosition.column));
        }

        private static void HighlightErrorFrom(string error, TabInfo tab)
        {
            Match m = Regex.Match(error, pattern2);
            int parserLine;
            if (!m.Success || !Int32.TryParse(m.Groups[2].Value, out parserLine)) return;

            Error ePosition = new Error(parserLine - 1, -1);
            string fpath = m.Groups[1].Value;

            int total = tab.textEditor.Document.TotalNumberOfLines;
            if (ePosition.line < 0) ePosition.line = 0;
            if (ePosition.line >= total) ePosition.line = total - 1;

            if (String.Equals(Path.GetFileName(fpath), tab.filename, StringComparison.OrdinalIgnoreCase))
                AddWaveMarker(tab, ePosition.line, "Error parsing the contents of the header file.", ColorTheme.HighlightIncludeError);
        }

        private static void AddWaveMarker(TabInfo tab, int line, string message, System.Drawing.Color color)
        {
            IDocument document = tab.textEditor.Document;
            if (document.TextLength == 0 || document.TotalNumberOfLines == 0) return;

            line = Math.Max(0, Math.Min(line, document.TotalNumberOfLines - 1));
            LineSegment segment = document.GetLineSegment(line);
            int offset = segment.Offset;
            int length = segment.Length;

            if (length == 0) {
                for (int i = line + 1; i < document.TotalNumberOfLines; i++) {
                    LineSegment next = document.GetLineSegment(i);
                    if (next.Length > 0) {
                        offset = next.Offset;
                        length = 1;
                        break;
                    }
                }

                if (length == 0) {
                    for (int i = line - 1; i >= 0; i--) {
                        LineSegment previous = document.GetLineSegment(i);
                        if (previous.Length > 0) {
                            offset = previous.Offset + previous.Length - 1;
                            length = 1;
                            break;
                        }
                    }
                }
            }

            if (length == 0) return;

            TextMarker marker = document.MarkerStrategy.GetMarkers(offset, length)
                .FirstOrDefault(item => item.TextMarkerType == TextMarkerType.WaveLine && item.Color.ToArgb() == color.ToArgb());
            if (marker == null) {
                marker = new TextMarker(offset, length, TextMarkerType.WaveLine, color);
                marker.ToolTip = message;
                document.MarkerStrategy.AddMarker(marker);
            } else if (!String.IsNullOrEmpty(message) && (marker.ToolTip == null || !marker.ToolTip.Contains(message))) {
                marker.ToolTip = String.IsNullOrEmpty(marker.ToolTip) ? message : marker.ToolTip + Environment.NewLine + message;
            }
        }
    }
}
