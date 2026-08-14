using System.Drawing;

using System;
using System.Text;
using System.Text.RegularExpressions;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUtilities;

namespace ScriptEditor.TextEditorUI.ToolTips
{
    // Tooltip for opcodes/macros and message
    static class ToolTipRequest
    {
        private static readonly Regex MessageStrCall = new Regex(
            @"\bmessage_str\s*\(\s*([A-Za-z_][A-Za-z0-9_]*|\d+)\s*,\s*(\d+)\s*\)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex MessageWrapperCall = new Regex(
            @"\b([A-Za-z_][A-Za-z0-9_]*)\s*\(\s*(\d+)\s*\)",
            RegexOptions.Compiled);

        private static readonly Regex MessageWrapperDefinition = new Regex(
            @"\bmessage_str\s*\(\s*([A-Za-z_][A-Za-z0-9_]*|\d+)\s*,\s*([A-Za-z_][A-Za-z0-9_]*)\s*\)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex SingleParameterMacro = new Regex(
            @"^[A-Za-z_][A-Za-z0-9_]*\s*\(\s*([A-Za-z_][A-Za-z0-9_]*)\s*\)$",
            RegexOptions.Compiled);

        private static readonly Regex RandomCall = new Regex(
            @"\brandom\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private const int MaxMessageRangeEntries = 100;

        public static void Show(TabInfo ti, IDocument document, ToolTipRequestEventArgs args)
        {
            if (ColorTheme.CheckColorPosition(document, new TextLocation(args.LogicalPosition.Column, args.LogicalPosition.Line), true))
                return;
            
            string word = TextUtilities.GetWordAt(document, document.PositionToOffset(args.LogicalPosition));
            if (word.Length == 0 ) 
                return;
            
            int msg;
            if (int.TryParse(word, out msg)) {
                string scriptToken;
                string explicitMessage;
                int hoverOffset = document.PositionToOffset(args.LogicalPosition);
                int firstMessage;
                int lastMessage;
                string rangeText;
                if (TryGetRandomMessageRange(document.TextContent, hoverOffset, out firstMessage, out lastMessage)
                    && TryGetMessageRangeText(ti, firstMessage, lastMessage, out rangeText)) {
                    document.TextEditorProperties.BoldFontTipsTile = true;
                    args.ShowToolTip(rangeText);
                    return;
                }
                if (TryGetMessageScriptToken(ti, document.TextContent, hoverOffset, msg, out scriptToken)
                    && MessageFile.TryGetMessageText(ti, scriptToken, msg, out explicitMessage)) {
                    args.ShowToolTip("\"" + explicitMessage + "\"");
                    return;
                }
                if (ti.messages.Count == 0)
                    MessageFile.TryLoadMessagesForScriptIdentity(ti);
                if (ti.messages.ContainsKey(msg)) {
                    args.ShowToolTip("\"" + ti.messages[msg] + "\"");
                    return;
                }
            }

            document.TextEditorProperties.BoldFontTipsTile = true;
            string lookup = ProgramInfo.LookupOpcodesToken(word); // show opcodes help
            if (lookup == null && ti.parseInfo != null) {
                lookup = ti.parseInfo.LookupToken(word, ti.filepath, args.LogicalPosition.Line + 1);
                document.TextEditorProperties.BoldFontTipsTile = false;
            } if (lookup != null) {
                args.ShowToolTip(lookup);
            }
        }

        internal static bool TryGetMessageStrScriptToken(string source, int hoverOffset, int messageNumber, out string scriptToken)
        {
            scriptToken = null;
            if (String.IsNullOrEmpty(source) || hoverOffset < 0 || hoverOffset > source.Length)
                return false;

            int start = Math.Max(0, hoverOffset - 256);
            int length = Math.Min(source.Length - start, 512);
            string window = source.Substring(start, length);
            foreach (Match match in MessageStrCall.Matches(window)) {
                Group number = match.Groups[2];
                int numberStart = start + number.Index;
                int numberEnd = numberStart + number.Length;
                int parsedNumber;
                if (hoverOffset >= numberStart && hoverOffset <= numberEnd
                    && int.TryParse(number.Value, out parsedNumber) && parsedNumber == messageNumber) {
                    scriptToken = match.Groups[1].Value;
                    return true;
                }
            }
            return false;
        }

        internal static bool TryGetRandomMessageRange(string source, int hoverOffset, out int firstMessage, out int lastMessage)
        {
            firstMessage = -1;
            lastMessage = -1;
            if (String.IsNullOrEmpty(source) || hoverOffset < 0 || hoverOffset > source.Length)
                return false;

            int start = Math.Max(0, hoverOffset - 256);
            int length = Math.Min(source.Length - start, 512);
            string window = source.Substring(start, length);
            foreach (Match match in RandomCall.Matches(window)) {
                Group first = match.Groups[1];
                Group last = match.Groups[2];
                int firstStart = start + first.Index;
                int firstEnd = firstStart + first.Length;
                int lastStart = start + last.Index;
                int lastEnd = lastStart + last.Length;
                if ((hoverOffset < firstStart || hoverOffset > firstEnd)
                    && (hoverOffset < lastStart || hoverOffset > lastEnd))
                    continue;

                if (!int.TryParse(first.Value, out firstMessage)
                    || !int.TryParse(last.Value, out lastMessage)
                    || firstMessage > lastMessage
                    || (long)lastMessage - firstMessage + 1 > MaxMessageRangeEntries) {
                    firstMessage = -1;
                    lastMessage = -1;
                    return false;
                }
                return true;
            }
            return false;
        }

        internal static bool TryGetMessageRangeText(TabInfo tab, int firstMessage, int lastMessage, out string text)
        {
            text = null;
            if (tab == null || firstMessage < 0 || lastMessage < firstMessage
                || (long)lastMessage - firstMessage + 1 > MaxMessageRangeEntries)
                return false;

            if (tab.messages.Count == 0)
                MessageFile.TryLoadMessagesForScriptIdentity(tab);

            var result = new StringBuilder();
            result.AppendFormat("Messages {0}-{1}", firstMessage, lastMessage);
            int found = 0;
            for (int message = firstMessage; message <= lastMessage; message++) {
                string messageText;
                if (!tab.messages.TryGetValue(message, out messageText))
                    continue;
                result.Append(Environment.NewLine).Append(message).Append(": ")
                    .Append((char)34).Append(messageText).Append((char)34);
                found++;
            }

            if (found == 0)
                return false;
            text = result.ToString();
            return true;
        }

        internal static bool TryGetMessageScriptToken(TabInfo tab, string source, int hoverOffset, int messageNumber, out string scriptToken)
        {
            if (TryGetMessageStrScriptToken(source, hoverOffset, messageNumber, out scriptToken))
                return true;

            scriptToken = null;
            if (tab == null || tab.parseInfo == null || String.IsNullOrEmpty(source)
                || hoverOffset < 0 || hoverOffset > source.Length)
                return false;

            int start = Math.Max(0, hoverOffset - 256);
            int length = Math.Min(source.Length - start, 512);
            string window = source.Substring(start, length);
            foreach (Match call in MessageWrapperCall.Matches(window)) {
                Group number = call.Groups[2];
                int numberStart = start + number.Index;
                int numberEnd = numberStart + number.Length;
                int parsedNumber;
                if (hoverOffset < numberStart || hoverOffset > numberEnd
                    || !int.TryParse(number.Value, out parsedNumber) || parsedNumber != messageNumber)
                    continue;

                Macro macro;
                if (!tab.parseInfo.macros.TryGetValue(call.Groups[1].Value, out macro))
                    continue;

                Match declaration = SingleParameterMacro.Match(macro.defname);
                Match definition = MessageWrapperDefinition.Match(macro.code);
                if (!declaration.Success || !definition.Success
                    || !String.Equals(declaration.Groups[1].Value, definition.Groups[2].Value,
                        StringComparison.OrdinalIgnoreCase))
                    continue;

                scriptToken = definition.Groups[1].Value;
                return true;
            }
            return false;
        }
    }
}
