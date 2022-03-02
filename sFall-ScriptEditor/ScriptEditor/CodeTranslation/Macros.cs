using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScriptEditor.CodeTranslation
{
    internal class GetMacros
    {
        private static string[] ReadAllLines(string file)
        {
            return ICSharpCode.TextEditor.Util.FileReader.ReadFileContent(file).Split('\n');
        }

        public static string[] GetHeadersFiles(string dirHeaders)
        {
            if (dirHeaders == null || !Directory.Exists(dirHeaders)) {
                Program.printLog("   <GetHeaders> Directory of headers not found: '" + (dirHeaders ?? "<unset>") + "'");
                return null;
            }
            return Directory.GetFiles(dirHeaders, "*.h", SearchOption.AllDirectories);
        }

        public static void GetGlobalMacros(string dirHeaders)
        {
            string[] headerFiles = GetHeadersFiles(dirHeaders);
            if (headerFiles == null) return;

            ProgramInfo.macrosGlobal.Clear();
            foreach (string file in headerFiles)
            {
                new GetMacros(ReadAllLines(file), file, "", ProgramInfo.macrosGlobal, false);
            }
        }

        public GetMacros(string file, string dir, SortedDictionary<string, Macro> macros)
        {
            if (!File.Exists(file)) {
                Program.printLog("   <GetMacros> File not found: '" + file + "'");
                return;
            }
            new GetMacros(ReadAllLines(file), file, dir, macros);
        }

        public GetMacros(string[] lines, string file, string dir, SortedDictionary<string, Macro> macros, bool include = true)
        {
            if (dir == null) dir = Path.GetDirectoryName(file);

            bool commentBlock = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length < 2) continue;
                lines[i] = lines[i].Replace('\t',' ').Trim(); // убираем все лишнее

                #region Пропускаем закоментированные макросы

                if (lines[i].StartsWith("//")) continue;
                if (!commentBlock) {
                    if (lines[i].StartsWith("/*")) {
                        if (lines[i].LastIndexOf("*/") == -1) commentBlock = true; // в встроке нет закрывающего тэга
                        continue;
                    }
                } else {
                    int close = lines[i].IndexOf("*/");
                    if (close == -1) continue;
                    commentBlock = false;
                    close += 2;
                    if (close == lines[i].Length) continue;
                    lines[i] = lines[i].Remove(0, close).TrimStart();
                }
                if (lines[i].Length <= 8) continue; // минимальное необходимое значение для ключевого слова #define

                #endregion

                if (include && lines[i].StartsWith(ParserInternal.INCLUDE)) {
                    string[] text = lines[i].Split('"');
                    if (text.Length < 2)
                        continue;
                    if (text[1].IndexOfAny(Path.GetInvalidPathChars()) != -1)
                        continue;
                    ParserInternal.GetIncludePath(ref text[1], dir);
                    new GetMacros(text[1], null, macros);
                }
                else if (lines[i].StartsWith(ParserInternal.DEFINE)) {
                    // описание к макросу
                    List<string> desc = new List<string>();
                    int descBlock = 0;
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (descBlock <= 0 && lines[j].Length == 0) break;

                        string line = lines[j];
                        int n = line.LastIndexOf(" **/");
                        if (descBlock == 0 && n == -1) break;

                        if (n != -1) {
                            descBlock++;
                            line = line.Remove(n);
                        }

                        n = line.IndexOf("/** ");
                        if (n != -1) {
                            descBlock--;
                            if (descBlock < 0) break;
                            line = line.Substring(n + 4);
                        }
                        desc.Add(line.Trim());
                    }
                    string description = null;
                    if (desc.Count > 0) {
                        var sbDesc = new StringBuilder();
                        desc.Reverse();
                        foreach (var item in desc)
                        {
                            sbDesc.AppendLine(item);
                        }
                        description = sbDesc.ToString();
                    }

                    if (lines[i].EndsWith(@"\")) {
                        var sb = new StringBuilder();
                        int lineno = i;
                        lines[i] = lines[i].Substring(8);
                        do {
                            sb.Append(lines[i].Remove(lines[i].Length - 1).TrimEnd()); // удаляем пробелы и символ '\' в конце макроса
                            sb.Append(Environment.NewLine);
                            i++;
                            lines[i] = lines[i].TrimEnd();
                        } while (lines[i].EndsWith(@"\"));
                        sb.Append(lines[i]);
                        AddMacro(sb.ToString(), macros, file, lineno, description);
                    } else
                        AddMacro(lines[i].Substring(8), macros, file, i, description);
                }
            }
        }

        private void AddMacro(string line, SortedDictionary<string, Macro> macros, string file, int lineno, string description)
        {
            string token, macro, def;
            line = line.TrimStart();
            int firstspace = line.IndexOf(' ');

            if (firstspace == -1) return;

            int firstbracket = line.IndexOf('(');

            if (firstbracket != -1 && firstbracket < firstspace) {
                int closebracket = line.IndexOf(')');
                if (line.Length == closebracket + 1) return; //second check, because spaces are allowed between macro arguments

                macro = line.Remove(closebracket + 1);
                token = line.Remove(firstbracket);
                def = MacroFormat(line.Substring(closebracket + 1), macro.Length);
            } else {
                macro = line.Remove(firstspace);
                token = macro;
                def = MacroFormat(line.Substring(firstspace), macro.Length);
            }
            macros[token] = new Macro(token, macro, def, file, lineno + 1, description); // макросы записываются в том регистре в котором они объявлены
        }

        private string MacroFormat(string defmacro, int macrolen)
        {
            string[] macroline = defmacro.Split(new char[]{'\n'}, 42);
            if (macroline.Length > 1) {
                int indent = -1;
                macroline[0] = macroline[0].TrimEnd();
                if (macroline[0].Length > 1) {
                    indent = (8 + macrolen) + macroline[0].Length - macroline[0].TrimStart().Length;
                }
                for (int i = 1; i < macroline.Length; i++)
                {
                    macroline[i] = macroline[i].TrimEnd();
                    if (indent == -1 && macroline[i].Length > 1) {
                        indent = macroline[i].Length - macroline[i].TrimStart().Length;
                    } else if (indent == -1 || macroline[i].Length == 0 )
                                continue;
                    try {
                        int adjust = macroline[i].Length - macroline[i].TrimStart().Length;
                        if (adjust > indent) adjust = indent;
                        else if (i == 1) indent = adjust;
                        macroline[i] = macroline[i].Remove(0, adjust);
                    }
                    catch { Program.printLog("   <MacroFormat> Exception in line " + macroline[i] + " | Macros: " + defmacro); }
                    if (i > 40) { // tip text size
                        macroline[i] = "...continue macro...";
                        break;
                    }
                }
                return String.Join("\n", macroline).TrimStart();
            }
            return defmacro.TrimStart();
        }
    }
}
