using System;
using System.Text.RegularExpressions;

namespace ScriptEditor.TextEditorUI
{
    internal static class CompilerDiagnosticLineResolver
    {
        private static readonly Regex TrailingStrayIdentifier = new Regex(
            @"(?:^|[;{}])\s*[A-Za-z_][A-Za-z0-9_]*\s*;?\s*$",
            RegexOptions.Compiled);
        private static readonly Regex Identifier = new Regex(
            @"[A-Za-z_][A-Za-z0-9_]*",
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

        internal static string Clarify(string message, int reportedLine, int resolvedLine,
            int reportedColumn, Func<int, string> getLine)
        {
            if (String.IsNullOrEmpty(message) || getLine == null)
                return message;

            bool assignmentError = message.StartsWith("Assignment operator expected", StringComparison.OrdinalIgnoreCase);
            bool unknownNameError = message.StartsWith("Unknown name identifier", StringComparison.OrdinalIgnoreCase);
            if (!assignmentError && !unknownNameError)
                return message;

            string code = getLine(resolvedLine) ?? String.Empty;
            bool insideBlockComment = false;
            code = RemoveComments(code, ref insideBlockComment);
            MatchCollection identifiers = Identifier.Matches(code);
            if (identifiers.Count == 0)
                return message;

            Match offending = identifiers[identifiers.Count - 1];
            if (resolvedLine == reportedLine && reportedColumn > 0) {
                for (int i = 1; i < identifiers.Count; i++) {
                    Match lookahead = identifiers[i];
                    if (reportedColumn <= lookahead.Index + lookahead.Length + 1) {
                        offending = identifiers[i - 1];
                        break;
                    }
                }
            }

            return assignmentError
                ? String.Format("Unexpected identifier '{0}'; assignment operator expected.", offending.Value)
                : String.Format("Unknown identifier '{0}'.", offending.Value);
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
