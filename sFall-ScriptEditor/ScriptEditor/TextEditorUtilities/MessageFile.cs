using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Linq;

using ScriptEditor.TextEditorUI;

namespace ScriptEditor.TextEditorUtilities
{
    public static class MessageFile
    {
        public const string messageError = "[Error] <Text was not found in msg file>";
        public const string msgfileError = "[Error] MsgID:{0} <Not found message file>";

        public const string MissingFile = "The associated message file of the script could not be found.";
        public const string WrongTypeFile = "This file type can't have an associated message file.";

        public static string MessageTextSubPath = "..\\text\\" + Settings.language + "\\dialog\\";

        static bool onlyOnce = false;

        static List<string> missingFile = new List<string>();

        public static void UpdateMessageTextLangPath()
        {
            MessageTextSubPath = string.Format("..\\text\\{0}\\dialog\\", Settings.language);
        }

        public static void ShowMissingFiles()
        {
            foreach (string file in missingFile)
                MessageBox.Show("The message file for the dialog was not found.\n" + file, "File Missing");
            missingFile.Clear();
        }

        public static bool GetAssociatePath(TabInfo tab, bool create, out string path)
        {
            string defaultDir;
            string fileName = null;
            path = null;

            if (Settings.associateID && Settings.outputDir != null && tab.parseInfo != null) {
                int nameID = tab.parseInfo.ScriptNameID;
                if (nameID != -1) {
                    int error;
                    fileName = GetMessageFileNameID(Settings.outputDir, nameID, out error);
                    switch (error)
                    {
                        case -1:
                            MessageBox.Show("Scripts.lst does not exist in scripts output directory.", "Error");
                            break;
                        case 1:
                            MessageBox.Show("Failed to get the associated name from Scripts.lst.", "Scripts.lst error");
                            return false;
                    }
                } else
                    MessageBox.Show("The macro NAME was not found in this script or does not contain the script number.\n" +
                                    "If necessary, association with the message file will be carried out by name of script.",
                                    "Warning: Define NAME", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (fileName == null)
                fileName = Path.ChangeExtension(tab.filename, ".msg");

            bool found = CheckPath(tab.filepath, fileName, out path, out defaultDir);

            if (!found && create) {
                found = MessageBox.Show(String.Format("The message file {0} associated with this script could not be found." +
                                                      "\nDo you want to create a new file in {1} directory?", fileName, defaultDir),
                                                      "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes;
                if (found) {
                    if (!Directory.Exists(defaultDir)) Directory.CreateDirectory(defaultDir);
                    path = Path.Combine(defaultDir, fileName);
                    File.WriteAllText(path, "{100}{}{}");
                }
            }
            return found;
        }

        public static bool TryLoadMessagesForScriptIdentity(TabInfo tab)
        {
            if (tab == null || tab.parseInfo == null || Settings.outputDir == null || String.IsNullOrEmpty(tab.filepath))
                return false;

            int nameID = tab.parseInfo.ScriptNameID;
            if (nameID <= 0)
                return false;

            int error;
            string fileName = GetMessageFileNameID(Settings.outputDir, nameID, out error);
            if (error != 0 || String.IsNullOrEmpty(fileName))
                return false;

            string path;
            string defaultDir;
            if (!CheckPath(tab.filepath, fileName, out path, out defaultDir))
                return false;

            try {
                ParseMessages(tab, File.ReadAllLines(path, Settings.EncCodePage));
                tab.msgFilePath = path;
                return tab.messages.Count > 0;
            } catch (IOException) {
                return false;
            } catch (UnauthorizedAccessException) {
                return false;
            }
        }

        public static bool TryGetMessageText(TabInfo tab, string scriptToken, int messageNumber, out string text)
        {
            text = null;
            string path;
            if (!TryGetMessagePath(tab, scriptToken, out path))
                return false;

            try {
                var messageTab = new TabInfo();
                ParseMessages(messageTab, File.ReadAllLines(path, Settings.EncCodePage));
                return messageTab.messages.TryGetValue(messageNumber, out text);
            } catch (IOException) {
                return false;
            } catch (UnauthorizedAccessException) {
                return false;
            }
        }

        public static bool TryGetMessageLocation(TabInfo tab, string scriptToken, int messageNumber,
            out string path, out int line)
        {
            line = -1;
            if (!TryGetMessagePath(tab, scriptToken, out path))
                return false;

            try {
                string source = null;
                if (tab.msgFileTab != null
                    && String.Equals(tab.msgFileTab.filepath, path, StringComparison.OrdinalIgnoreCase))
                    source = tab.msgFileTab.textEditor.Document.TextContent;
                if (source == null)
                    source = File.ReadAllText(path, Settings.EncCodePage);
                return TryFindMessageLine(source, messageNumber, out line);
            } catch (IOException) {
                return false;
            } catch (UnauthorizedAccessException) {
                return false;
            }
        }

        internal static bool TryFindMessageLine(string source, int messageNumber, out int line)
        {
            line = -1;
            if (String.IsNullOrEmpty(source))
                return false;

            string[] lines = source.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++) {
                string value = lines[i].TrimStart();
                if (!value.StartsWith("{", StringComparison.Ordinal))
                    continue;
                int close = value.IndexOf('}');
                int parsed;
                if (close > 1 && int.TryParse(value.Substring(1, close - 1).Trim(), out parsed)
                    && parsed == messageNumber) {
                    line = i + 1;
                    return true;
                }
            }
            return false;
        }

        private static bool TryGetMessagePath(TabInfo tab, string scriptToken, out string path)
        {
            path = null;
            if (tab == null || String.IsNullOrEmpty(tab.filepath))
                return false;

            if (String.IsNullOrWhiteSpace(scriptToken)
                && !String.IsNullOrEmpty(tab.msgFilePath) && File.Exists(tab.msgFilePath)) {
                path = tab.msgFilePath;
                return true;
            }

            string fileName = null;
            int scriptID;
            if (!String.IsNullOrWhiteSpace(scriptToken)) {
                if (!TryResolveScriptID(tab, scriptToken, out scriptID))
                    return false;
            } else {
                scriptID = tab.parseInfo == null ? -1 : tab.parseInfo.ScriptNameID;
            }

            if (scriptID > 0) {
                string outputDir = Settings.outputDir ?? Path.GetDirectoryName(tab.filepath);
                int error;
                fileName = GetMessageFileNameID(outputDir, scriptID, out error);
                if (error != 0)
                    return false;
            } else if (String.IsNullOrWhiteSpace(scriptToken)) {
                fileName = Path.ChangeExtension(tab.filename, ".msg");
            }

            if (String.IsNullOrEmpty(fileName))
                return false;

            string defaultDir;
            return CheckPath(tab.filepath, fileName, out path, out defaultDir);
        }

        private static bool TryResolveScriptID(TabInfo tab, string token, out int scriptID)
        {
            scriptID = -1;
            if (tab == null || tab.parseInfo == null || String.IsNullOrWhiteSpace(token))
                return false;

            string current = token.Trim();
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int depth = 0; depth < 32; depth++) {
                string numeric = current.Trim().Trim('(', ')').Trim();
                if (int.TryParse(numeric, out scriptID))
                    return scriptID > 0;
                if (!visited.Add(current) || !tab.parseInfo.macros.ContainsKey(current))
                    return false;
                tab.parseInfo.MacrosGetValue(ref current);
            }
            return false;
        }

        public static bool GetPath(TabInfo tab, int msgNumber, out string path, bool report = false)
        {
            string defaultDir;
            string fileName;
            path = null;

            string outputDir = Settings.outputDir;
            if (outputDir == null) {
                outputDir = Path.GetDirectoryName(tab.filepath);
                if (!onlyOnce)
                    MessageBox.Show("The output script path for the required Scripts.lst file is not specified,\n" +
                                    "so by default the current script folder is used.", "Path missing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                onlyOnce = true;
            }

            int error;
            fileName = GetMessageFileNameID(outputDir, msgNumber, out error);
            if (error != 0)
                return false;

            bool found = CheckPath(tab.filepath, fileName, out path, out defaultDir);
            if (report && !found && !missingFile.Contains(path))
                missingFile.Add(path);

            return found;
        }

        private static string GetMessageFileNameID(string path, int nameID, out int error)
        {
            error = 0;
            string name = null;

            string lstPath = Path.Combine(path, "scripts.lst");

            if (File.Exists(lstPath)) {
                List<string> scriptsLST = new List<string>(File.ReadAllLines(lstPath));
                try
                {
                    string scriptID = scriptsLST[nameID - 1].TrimStart();
                    name = scriptID.Remove(scriptID.IndexOf('.') + 1) + "msg";
                }
                catch
                {
                    error = 1;
                }
            } else
                error = -1;

            return name;
        }

        /// <summary>
        /// Возвращает раcположение .msg файла
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="path">Возвращаемый полный путь к .msg файлу</param>
        /// <param name="defaultDir">Возвращемый путь к папке в которой будет сохранен .msg файл диалога</param>
        private static bool CheckPath(string filePath, string fileName, out string path, out string defaultDir)
        {
            if (Settings.outputDir == null)
                defaultDir = Path.GetDirectoryName(filePath);
            else
                defaultDir = Path.GetFullPath(Path.Combine(Settings.outputDir, MessageTextSubPath));

            // primary check in output dir
            path = Path.Combine(defaultDir, fileName);

            bool found = File.Exists(path);
            if (!found) {
                // second check in msg list path
                for (int i = 0; i < Settings.msgListPath.Count; i++)
                {
                    if (!Directory.Exists(Settings.msgListPath[i])) continue;

                    string pth = Path.Combine(Settings.msgListPath[i], fileName);
                    if (File.Exists(pth)) {
                        path = pth;
                        return true;
                    }
                    // проверить файлы в под папках
                    foreach (var subFolder in Directory.GetDirectories(Settings.msgListPath[i]))
                    {
                        pth = Path.Combine(subFolder, fileName);
                        if (File.Exists(pth)) {
                            path = pth;
                            return true;
                        }
                    }
                }
                // проверить текущую папку расположения .ssl скрипта
                if (Settings.outputDir != null) {
                    path = Path.Combine(Path.GetDirectoryName(filePath), fileName);
                    found = File.Exists(path);
                }
            }
            return found;
        }

        public static void ParseMessages(TabInfo ti)
        {
            ParseMessages(ti, ti.msgFileTab.textEditor.Document.TextContent.Split(
                          new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries));
        }

        public static void ParseMessages(TabInfo ti, string[] linesMsg)
        {
            ti.messages.Clear();
            char[] split = new char[] { '}' };
            for (int i = 0; i < linesMsg.Length; i++)
            {
                string[] line = linesMsg[i].Split(split, StringSplitOptions.RemoveEmptyEntries);
                if (subParse(line))
                    continue;
                int index;
                if (!int.TryParse(line[0], out index))
                    continue;
                ti.messages.Add(index, getText(linesMsg, i, line[2]));
            }
        }

        private static bool subParse(string[] line)
        {
            if (line.Length < 3)
                return true;
            for (int j = 0; j < 3; j += 2)
            {
                line[j] = line[j].Trim();
                if (line[j].Length == 0 /*|| line[j][0] != '{'*/)
                    continue;
                line[j] = line[j].Trim('{', ' ');
            }
            return false;
        }

        private static string getText(string[] linesMsg, int i, string text)
        {
            int z = linesMsg[i].LastIndexOf('{') + 1;
            int y = -1;
            do {
                y = linesMsg[i].IndexOf("}", z);
                if (z == 0){
                    text += Environment.NewLine + linesMsg[i].TrimEnd('}');
                    z = -1; // Z используем в качестве флага
                }
                if (y == -1) { // если Y равен -1, значит строка переходит на новую строку
                    i++;
                    z = 0; // Z используем в качестве флага
                    if (i == linesMsg.Length)
                        return text; // выходим, достигнут конец строк
                }
            } while (y == -1);

            if (z == 0)
                text += Environment.NewLine + linesMsg[i].Remove(y);

            return text;
        }

        /// <summary>
        /// Возвращает строку текста с указаным номером из переданного буфера.
        /// Должно использоваться для получения строки текста из не ассоциированого Msg-файла.
        /// </summary>
        public static string GetMessages(string[] linesMsg, int messageNum)
        {
            char[] split = new char[] { '}' };
            for (int i = 0; i < linesMsg.Length; i++)
            {
                string[] line = linesMsg[i].Split(split, StringSplitOptions.RemoveEmptyEntries);
                if (subParse(line)) continue;
                int index;
                if (int.TryParse(line[0], out index)) {
                    if (index == messageNum)
                        return getText(linesMsg, i, line[2]);
                }
            }
            return null;
        }

        private static bool PutMessages(ref string[] MsgData, string text, int messageNum)
        {
            List<string> linesMsg = MsgData.ToList();

            char[] split = new char[] { '}' };
            for (int i = 0; i < linesMsg.Count; i++)
            {
                string[] line = linesMsg[i].Split(split, 2, StringSplitOptions.RemoveEmptyEntries);
                if (line.Length != 2)continue;

                int index;
                if (int.TryParse(line[0].Trim('{', ' ', '\t'), out index)) {
                    if (index == messageNum) {
                        int z = linesMsg[i].LastIndexOf('{') + 1;
                        //содержимое строки до текста
                        string prefix = linesMsg[i].Remove(z);

                        int y = -1;
                        do {
                            y = linesMsg[i].IndexOf("}", z);
                            if (y == -1) { // если Y равен -1, значит строка переходит на новую строку
                                linesMsg.RemoveAt(i); // удаляем строку
                                z = 0; // Z используем в качестве флага
                            }
                        } while (y == -1);

                        // содержимое строки после текста
                        string suffix = linesMsg[i].Substring(y, linesMsg[i].Length - y);
                        if (z == 0) // удаляем последнию строку
                            linesMsg.RemoveAt(i);

                        // сцепляем все в строку
                        linesMsg[i] = prefix + text + suffix;

                        MsgData = linesMsg.ToArray();
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Сохраняет текстовую строку с номеров в указанный файл сообщения
        /// </summary>
        public static bool SaveToMessageFile(string msgFilePath, string text, int msgNum)
        {
            string[] MessagesData = File.ReadAllLines(msgFilePath, Settings.EncCodePage);
            return SaveToMessageFile(ref MessagesData, msgFilePath, text, msgNum);
        }

        /// <summary>
        /// Записывает в буфер текстовую строку с указанным номеров и сохранияет буфер в указанный файл сообщения
        /// </summary>
        public static bool SaveToMessageFile(ref string[] MessagesData, string msgFilePath, string text, int msgNum)
        {
            bool result = PutMessages(ref MessagesData, text, msgNum);
            if (result)
                File.WriteAllLines(msgFilePath, MessagesData, Settings.EncCodePage);
            else
                MessageBox.Show("Can't change this text in the message file.", "Put message error");

            return result;
        }
    }
}
