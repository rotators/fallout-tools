using System;
using System.Collections.Generic;
using System.IO;

namespace ScriptEditor.TextEditorUI
{
    internal sealed class DiagnosticSuppressionRules
    {
        internal const string ConfigFileName = "IgnoredDiagnostics.ini";

        private readonly List<Rule> rules = new List<Rule>();

        internal static string DefaultPath {
            get { return Path.Combine(Settings.ProgramFolder, ConfigFileName); }
        }

        internal static DiagnosticSuppressionRules Load(string path)
        {
            var result = new DiagnosticSuppressionRules();
            if (String.IsNullOrEmpty(path) || !File.Exists(path)) return result;

            try {
                foreach (string rawLine in File.ReadAllLines(path)) {
                    Rule rule;
                    if (Rule.TryParse(rawLine, out rule)) result.rules.Add(rule);
                }
            } catch (IOException) { }
              catch (UnauthorizedAccessException) { }

            return result;
        }

        internal bool IsIgnored(Error error)
        {
            if (error == null) return false;
            foreach (Rule rule in rules) {
                if (rule.Matches(error)) return true;
            }
            return false;
        }

        private sealed class Rule
        {
            private ErrorType? severity;
            private string file;
            private int? line;
            private string message;

            internal static bool TryParse(string rawLine, out Rule rule)
            {
                rule = null;
                string text = (rawLine ?? String.Empty).Trim();
                if (text.Length == 0 || text.StartsWith("#") || text.StartsWith(";")) return false;

                string[] fields = text.Split(new char[] {'|'}, 4, StringSplitOptions.None);
                if (fields.Length != 4) return false;
                for (int i = 0; i < fields.Length; i++) fields[i] = fields[i].Trim();

                var parsed = new Rule();
                if (!IsWildcard(fields[0])) {
                    ErrorType severity;
                    if (!Enum.TryParse(fields[0], true, out severity)) return false;
                    parsed.severity = severity;
                }
                if (!IsWildcard(fields[1])) parsed.file = fields[1];
                if (!IsWildcard(fields[2])) {
                    int line;
                    if (!Int32.TryParse(fields[2], out line) || line < 1) return false;
                    parsed.line = line;
                }
                if (!IsWildcard(fields[3])) parsed.message = fields[3];

                rule = parsed;
                return true;
            }

            internal bool Matches(Error error)
            {
                if (severity.HasValue && severity.Value != error.type) return false;
                if (file != null && !String.Equals(file, Path.GetFileName(error.fileName), StringComparison.OrdinalIgnoreCase)) return false;
                if (line.HasValue && line.Value != error.line) return false;
                return message == null || (error.message != null
                    && error.message.IndexOf(message, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            private static bool IsWildcard(string value)
            {
                return String.IsNullOrWhiteSpace(value) || value == "*";
            }
        }
    }
}
