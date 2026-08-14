using System;
using System.Text.RegularExpressions;

namespace ScriptEditor.TextEditorUI
{
    internal static class CompilerDiagnosticLineResolver
    {
        private static readonly Regex TrailingStrayIdentifier = new Regex(
            @"(?:^|[;{}])\s*[A-Za-z_][A-Za-z0-9_]*\s*;?\s*$",
            RegexOptions.Compiled);

        internal static int Resolve(int reportedLine, string message, string previousLine)
        {
            if (reportedLine <= 0 || String.IsNullOrEmpty(message) || String.IsNullOrEmpty(previousLine))
                return reportedLine;

            bool lookaheadError = message.StartsWith("Assignment operator expected", StringComparison.OrdinalIgnoreCase)
                || message.StartsWith("Unknown name identifier", StringComparison.OrdinalIgnoreCase);
            if (!lookaheadError)
                return reportedLine;

            return TrailingStrayIdentifier.IsMatch(previousLine.Trim()) ? reportedLine - 1 : reportedLine;
        }
    }
}