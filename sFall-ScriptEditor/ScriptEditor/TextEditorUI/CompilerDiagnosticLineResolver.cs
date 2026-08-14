using System;
using System.Text.RegularExpressions;

namespace ScriptEditor.TextEditorUI
{
    internal static class CompilerDiagnosticLineResolver
    {
        private static readonly Regex TrailingStrayIdentifier = new Regex(
            @"(?:^|[;{}])\s*[A-Za-z_][A-Za-z0-9_]*\s*;?\s*$",
            RegexOptions.Compiled);

        internal static int Resolve(int reportedLine, int reportedColumn, string message, Func<int, string> getLine)
        {
            if (reportedLine <= 0 || String.IsNullOrEmpty(message) || getLine == null)
                return reportedLine;

            bool lookaheadError = message.StartsWith("Assignment operator expected", StringComparison.OrdinalIgnoreCase)
                || message.StartsWith("Unknown name identifier", StringComparison.OrdinalIgnoreCase);
            if (!lookaheadError)
                return reportedLine;

            string reportedCode = getLine(reportedLine) ?? String.Empty;
            Match firstToken = Regex.Match(reportedCode, @"^\s*[A-Za-z_][A-Za-z0-9_]*");
            if (reportedColumn > 0 && firstToken.Success
                && reportedColumn <= firstToken.Index + firstToken.Length + 1)
                return PreviousCodeLine(reportedLine, getLine);

            bool insideBlockComment = false;
            for (int line = reportedLine - 1; line >= 0; line--) {
                string code = RemoveComments(getLine(line), ref insideBlockComment);
                if (String.IsNullOrWhiteSpace(code))
                    continue;

                return TrailingStrayIdentifier.IsMatch(code.Trim()) ? line : reportedLine;
            }
            return reportedLine;
        }

        private static int PreviousCodeLine(int reportedLine, Func<int, string> getLine)
        {
            bool insideBlockComment = false;
            for (int line = reportedLine - 1; line >= 0; line--) {
                string code = RemoveComments(getLine(line), ref insideBlockComment);
                if (!String.IsNullOrWhiteSpace(code))
                    return line;
            }
            return reportedLine;
        }

        private static string RemoveComments(string line, ref bool insideBlockComment)
        {
            string code = line ?? String.Empty;
            while (true) {
                if (insideBlockComment) {
                    int blockStart = code.LastIndexOf("/*", StringComparison.Ordinal);
                    if (blockStart < 0)
                        return String.Empty;
                    code = code.Substring(0, blockStart);
                    insideBlockComment = false;
                }

                int lineComment = code.IndexOf("//", StringComparison.Ordinal);
                if (lineComment >= 0)
                    code = code.Substring(0, lineComment);

                int blockEnd = code.LastIndexOf("*/", StringComparison.Ordinal);
                if (blockEnd < 0)
                    return code;

                int sameLineStart = code.LastIndexOf("/*", blockEnd, StringComparison.Ordinal);
                if (sameLineStart >= 0) {
                    code = code.Remove(sameLineStart, blockEnd + 2 - sameLineStart);
                    continue;
                }

                code = code.Substring(0, blockEnd);
                insideBlockComment = true;
            }
        }
    }
}