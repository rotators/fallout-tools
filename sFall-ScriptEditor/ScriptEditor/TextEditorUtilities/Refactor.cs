using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI;

namespace ScriptEditor.TextEditorUtilities
{
    internal sealed class Refactor
    {
        internal static void Rename(IParserInfo item, IDocument document, TabInfo cTab, List<TabInfo> tabs)
        {
            string newName;
            switch ((NameType)item.Type())
            {
                case NameType.LVar: // local procedure variable
                    Variable lvar = (Variable)item;
                    newName = lvar.name;
                    if (!ProcForm.CreateRenameForm(ref newName, "Local Variable") || newName == lvar.name)
                        return;
                    if (cTab.parseInfo.CheckExistsName(newName, NameType.LVar, lvar.fdeclared, lvar.d.declared)) {
                        MessageBox.Show("The local variable this name already exists.", "Unable to rename");
                        return;
                    }
                    RenameVariable(lvar, newName, RegexOptions.IgnoreCase, document); // rename only via references
                    break;

                case NameType.GVar: // script variable
                    Variable gvar = (Variable)item;
                    newName = gvar.name;
                    if (!ProcForm.CreateRenameForm(ref newName, "Script Variable") || newName == gvar.name)
                        return;
                    if (cTab.parseInfo.CheckExistsName(newName, NameType.GVar)) {
                        MessageBox.Show("The variable/procedure or declared macro this name already exists.", "Unable to rename");
                        return;
                    }
                    RenameVariable(gvar, newName, RegexOptions.IgnoreCase, document); // rename only via references
                    break;

                case NameType.Proc:
                    RenameProcedure((Procedure)item, document, cTab, tabs);
                    return;

                case NameType.Macro:
                    Macro macros = (Macro)item;

                    bool isGlobal = ProgramInfo.macrosGlobal.ContainsKey(macros.token);
                    newName = macros.token;

                    if (!ProcForm.CreateRenameForm(ref newName, (isGlobal) ? "Global Macro" : "Local Macro") || newName == macros.token)
                        return;

                    if (cTab.parseInfo.CheckExistsName(newName, NameType.Macro)) {
                        MessageBox.Show("The variable/procedure or declared macro this name already exists.", "Unable to rename");
                        return;
                    }
                    int diff = newName.Length - macros.token.Length;

                    // Для глобальных требуется переименовать все макросы во всех открытых вкладках и во всех файлах проекта/мода
                    if (isGlobal) {
                        RenameGlobalMacros(macros, newName, cTab, tabs, diff);
                        // обновить макросы
                        GetMacros.GetGlobalMacros(Settings.pathHeadersFiles);
                        return;
                    }
                    RenameMacros(macros.token, newName, RegexOptions.None, document);
                    if (diff != 0) DefineMacroAdjustSpaces(macros, document, diff);
                    break;
            }
        }

        #region Переименование макросов

        private static void RenameMacros(string find, string newName, RegexOptions option, IDocument document)
        {
            document.UndoStack.StartUndoGroup();

            int offset = 0;
            while (offset < document.TextLength)
            {
                offset = Utilities.SearchWholeWord(document.TextContent, find, offset, option);
                if (offset == -1)
                    break;
                document.Replace(offset, find.Length, newName);
                offset += newName.Length;
            }
            document.UndoStack.EndUndoGroup();
        }

        public struct PreviewMatch
        {
            public string previewText;
            public Match  match;
            public TabInfo tab;

            public PreviewMatch(string previewText, Match match, TabInfo tab = null)
            {
                this.previewText = previewText;
                this.match = match;
                this.tab = tab;
            }

            public PreviewMatch(PreviewMatch obj)
            {
                this.previewText = obj.previewText;
                this.match = obj.match;
                this.tab = obj.tab;
            }
        }

        private static void MatchesCollect(ref Dictionary<string, List<PreviewMatch>> preview, Regex regex, string filename, string textContent, TabInfo tab = null)
        {
            MatchCollection matches = regex.Matches(textContent);

            if (matches.Count > 0) {
                List<PreviewMatch> list = new List<PreviewMatch>();
                foreach (Match match in matches)
                {
                    int pEnd = 0;
                    for (int i = match.Index + match.Length; i < textContent.Length; i++)
                    {
                        if (textContent[i] == '\n') {
                            pEnd = i;
                            break;
                        }
                    }
                    string previewText = textContent.Substring(match.Index, pEnd - match.Index).TrimEnd();
                    list.Add(new PreviewMatch(previewText, match, tab));
                }
                preview.Add(filename ?? tab.filepath, list);
            }
        }

        private static Dictionary<string, List<PreviewMatch>> PreviewRenameGlobalMacros(Regex regex, List<TabInfo> tabs)
        {
            Dictionary<string, List<PreviewMatch>> preview = new Dictionary<string, List<PreviewMatch>>(); // file => mathes

            foreach (var tab in tabs)
            {
                string textContent = tab.textEditor.Document.TextContent;
                MatchesCollect(ref preview, regex, null, textContent, tab);
            }
            return preview;
        }

        private static Dictionary<string, List<PreviewMatch>> PreviewRenameGlobalMacros(Regex regex, List<TabInfo> tabs, List<string> files)
        {
            ProgressBarForm pf = (files.Count > 100) ? new ProgressBarForm(Form.ActiveForm, files.Count, "Идет поиск совпадений в файлах проекта...") : null;
            Dictionary<string, List<PreviewMatch>> preview = new Dictionary<string, List<PreviewMatch>>(); // file => mathes

            foreach (var file in files)
            {
                if (pf != null) pf.IncProgress();
                if (TextEditor.CheckTabs(file, tabs)) continue; // next file

                string textContent = System.IO.File.ReadAllText(file);
                MatchesCollect(ref preview, regex, file, textContent);
            }
            if (pf != null) pf.Dispose();
            return preview;
        }

        private static void RenameGlobalMacros(Macro macros, string newName, TabInfo cTab, List<TabInfo> tabs, int diff)
        {
            Regex s_regex = new Regex(@"\b" + macros.token + @"\b", RegexOptions.None);

            // preview renamed macros open scripts
            Dictionary<string, List<PreviewMatch>> matchTabs = PreviewRenameGlobalMacros(s_regex, tabs);  // file => mathes

            List<string> files = Directory.GetFiles(Settings.solutionProjectFolder, "*.ssl", SearchOption.AllDirectories).ToList();
            files.AddRange(Directory.GetFiles(Settings.solutionProjectFolder, "*.h", SearchOption.AllDirectories).ToList());

            // preview renamed macros open scripts
            Dictionary<string, List<PreviewMatch>> matchFiles = PreviewRenameGlobalMacros(s_regex, tabs, files);

            // выбор совпадений
            var previewForm = new PreviewRename(macros.defname, macros.defname.Replace(macros.token, newName));
            previewForm.BuildTreeMatches(matchTabs, matchFiles);

            matchTabs.Clear();
            matchFiles.Clear();

            if (previewForm.ShowDialog() != DialogResult.OK) return;

            previewForm.GetSelectedMatches(ref matchTabs, ref matchFiles);
            previewForm.Dispose();

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            int isAdjustSpaces = diff;
            cTab.DisableParseAndStatusChange = true;

            int replaceLen = macros.token.Length;

            foreach (var tabMatches in matchTabs)
            {
                TabInfo tab = null;
                int replace_count = 0;
                foreach (var matches in tabMatches.Value)
                {
                    tab = matches.tab;
                    Match match = matches.match;
                    int offset = (diff * replace_count) + match.Index;
                    tab.textEditor.Document.Replace(offset, replaceLen, newName);
                    replace_count++;
                }
                if (tab != null) {
                    var document = tab.textEditor.Document;
                    if (isAdjustSpaces != 0 && string.Equals(tab.filepath, macros.fdeclared, StringComparison.OrdinalIgnoreCase)) {
                        DefineMacroAdjustSpaces(macros, document, diff);
                        isAdjustSpaces = 0;
                    }
                    document.UndoStack.ClearAll();
                    tab.SaveInternal(document.TextContent, tab.textEditor.Encoding); // сохранить изменения в файл
                }
            }

            if (matchFiles.Count == 0) {
                cTab.DisableParseAndStatusChange = false;
                return;
            }
            // замена в файлах проекта
            ProgressBarForm pf = (matchFiles.Count > 100) ? new ProgressBarForm(Form.ActiveForm, matchFiles.Count, "Идет замена в файлах проекта...") : null;

            int total = 0;
            foreach (var fileMatches in matchFiles)
            {
                string textContent = System.IO.File.ReadAllText(fileMatches.Key);
                total += fileMatches.Value.Count;

                int replace_count = 0;
                foreach (var matches in fileMatches.Value)
                {
                    int offset = (diff * replace_count) + matches.match.Index;
                    textContent = textContent.Remove(offset, matches.match.Length);
                    textContent = textContent.Insert(offset, newName);
                    replace_count++;
                }
                if (isAdjustSpaces != 0 && string.Equals(fileMatches.Key, macros.fdeclared, StringComparison.OrdinalIgnoreCase)) {
                    DefineMacroAdjustSpaces(macros, newName, ref textContent, diff);
                    isAdjustSpaces = 0;
                }
                if (replace_count > 0) {
                    File.WriteAllText(fileMatches.Key, textContent, (Settings.saveScriptUTF8) ? new UTF8Encoding(false) : Encoding.Default);
                }
                if (pf != null) pf.IncProgress();
            }
            if (pf != null) pf.Dispose();

            //MessageBox.Show(String.Format("Произведено переименование {0} макросов, в {1} файлах.", total, previewFiles.Count));
             cTab.DisableParseAndStatusChange = false;
        }

        // insert/delete spaces in define macro
        private static void DefineMacroAdjustSpaces(Macro macros, string newName, ref string textContent, int diff)
        {
            int offset = 0;
            while (true)
            {
                offset = textContent.IndexOf(" " + newName, offset); // ищем первое определение
                if (offset == -1 || offset >= textContent.Length) return;

                char c = textContent[offset + newName.Length + 1];
                if (c != '(' && !char.IsWhiteSpace(c)) continue; // перейти к следующему поиску если имя определения макроса не заканчивается символами пробела или открыващей скобкой

                if (textContent.IndexOf("#define", offset - 7, 7) != -1) break; // да, это строка определения макроса
            }
            offset += (macros.defname.Length + diff) + 1;

            if (diff > 0) {
                int removeCount = 0;
                for (int i = 0; i < diff; i++)
                {
                    if (!Char.IsWhiteSpace(textContent[offset + i + 1])) break;
                    removeCount++;
                }
                if (removeCount > 0) textContent = textContent.Remove(offset, removeCount);
            } else
                textContent = textContent.Insert(offset, new string(' ', Math.Abs(diff)));
        }

        // insert/delete spaces in define macro
        private static void DefineMacroAdjustSpaces(Macro macros, IDocument document, int diff)
        {
            int offset = document.PositionToOffset(new TextLocation(0, macros.declared - 1));
            offset += (macros.defname.Length + 8) + diff;

            if (diff > 0) {
                int removeCount = 0;
                for (int i = 0; i < diff; i++)
                {
                    if (!Char.IsWhiteSpace(document.GetCharAt(offset + i + 1))) break;
                    removeCount++;
                }
                if (removeCount > 0) document.Remove(offset, removeCount);
            } else
                document.Insert(offset, new string(' ',  Math.Abs(diff)));
        }
        #endregion

        private static void RenameVariable(Variable var, string newName, RegexOptions option, IDocument document)
        {
            Utilities.ReplaceByReferences(new Regex(@"\b" + var.name + @"\b", option), document, var, newName, newName.Length - var.name.Length);
        }

        // вызывается из редактора нодов диалога
        internal static string RenameProcedure(string name, IDocument document, TabInfo cTab)
        {
            Procedure proc = cTab.parseInfo.GetProcedureByName(name);
            return (proc != null) ? RenameProcedure(proc, document, cTab) : null;
        }

        // Search and replace procedure name in script text
        internal static string RenameProcedure(Procedure proc, IDocument document, TabInfo cTab, List<TabInfo> tabs = null)
        {
            string newName = proc.name;
            // form ini
            if (!ProcForm.CreateRenameForm(ref newName, "Procedure") || newName == proc.name)
                return null;

            if (cTab.parseInfo.CheckExistsName(newName, NameType.Proc)) {
                MessageBox.Show("The procedure/variable or declared macro with this name already exists.", "Unable to rename");
                return null;
            }

            bool extFile = false;
            if (tabs != null && proc.filename != cTab.filename.ToLowerInvariant()) { // не совсем понятно, при каких условиях это верно
                switch (MessageBox.Show("Also renaming the procedure in a file: " + proc.filename.ToUpper() + " ?", "Procedure rename", MessageBoxButtons.YesNoCancel)) {
                    case DialogResult.Cancel :
                        return null;
                    case DialogResult.Yes :
                        extFile = true;
                        break;
                }
            }
            int differ = newName.Length - proc.name.Length;

            Regex s_regex;
            if (proc.References().Length == 0) {
                s_regex = new Regex(@"(\bprocedure\s|\bcall\s|[=,(]\s*)\s*" + proc.Name + @"\b", // осуществить поиск всех процедур совпадающих по имени
                                    RegexOptions.Multiline | RegexOptions.IgnoreCase);
                Utilities.ReplaceIDocumentText(s_regex, document, newName, differ);
            } else {
                s_regex = new Regex(@"\b" + proc.Name + @"\b", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                Utilities.ReplaceByReferences(s_regex, document, proc, newName, differ); // rename by reference
            }

            // rename in other file/tab
            if (extFile) {
                string text = File.ReadAllText(proc.fstart);
                Utilities.ReplaceSpecialText(s_regex, ref text, newName, differ);
                File.WriteAllText(proc.fstart, text);

                TabInfo tab = TextEditor.CheckTabs(tabs, proc.fstart);
                if (tab != null) {
                    Utilities.ReplaceIDocumentText(s_regex, tab.textEditor.Document, newName, differ);
                    tab.FileTime = File.GetLastWriteTime(proc.fstart);
                }
            }
            TextEditor.currentHighlightProc = null;
            return newName;
        }
    }
}
