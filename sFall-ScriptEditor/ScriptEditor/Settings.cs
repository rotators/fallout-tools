using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ScriptEditor
{
    public enum SavedWindows { Main, Count }
    public enum  EncodingType : byte { Default, OEM866 }

    public static class Settings
    {
        public static Encoding EncCodePage;
        public static Size HeadersFormSize;

        public static readonly string ProgramFolder = Application.StartupPath;
        public static readonly string SettingsFolder = Path.Combine(ProgramFolder, "settings");
        public static readonly string ResourcesFolder = Path.Combine(ProgramFolder, "resources");
        public static readonly string DescriptionsFolder = Path.Combine(ProgramFolder, "descriptions");
        public static readonly string scriptTempPath = Path.Combine(ProgramFolder, "scrTemp");

        private static readonly string RecentPath = Path.Combine(SettingsFolder, "recent.dat");
        private static readonly string SettingsPath = Path.Combine(SettingsFolder, "settings.dat");

        public static readonly string SearchHistoryPath = Path.Combine(SettingsFolder, "SearchHistory.ini");
        public static readonly string SearchFoldersPath = Path.Combine(SettingsFolder, "SearchPaths.ini");
        public static readonly string PreprocDefPath = Path.Combine(SettingsFolder, "PreprocDefine.ini");

        public static PrivateFontCollection Fonts = new PrivateFontCollection();
        public static readonly Dictionary<string, float> FontAdjustSize = new Dictionary<string, float>() {
            {"Anonymous Pro", 10.5f},       {"Consolas", 10.5f},            {"Cousine", 10.5f},
            {"InconsolataCyr", 11.0f},      {"InputMono", 9.5f},            {"InputMonoCondensed", 9.5f},
            {"Liberation Mono", 10.25f},    {"Meslo LG S DZ", 9.75f},       {"Ubuntu Mono",  11.75f}
        };

        public static List<string> searchListPath = new List<string>();
        public static readonly List<string> msgListPath = new List<string>();

        private static List<string> recent = new List<string>();
        private static List<string> recentMsg = new List<string>();
        private static Dictionary<string, ushort> scriptPosition = new Dictionary<string, ushort>();
        private static readonly WindowPos[] windowPositions = new WindowPos[(int)SavedWindows.Count];

        const int MAX_RECENT = 40;

        public static byte optimize = 1;
        public static bool showWarnings = true;
        public static bool showDebug = true; //show additional information when compiling script
        public static bool searchIncludePath = true;
        public static bool warnOnFailedCompile = true;
        public static bool multiThreaded = true;
        public static bool autoOpenMsgs = false;
        public static bool openMsgEditor = false;
        public static string outputDir;
        public static string pathScriptsHFile;
        public static string lastMassCompile;
        public static string lastSearchPath;
        public static int editorSplitterPosition = -1;
        public static int editorSplitterPosition2 = -1;
        public static string language = "english";
        public static bool tabsToSpaces = true;
        public static int tabSize = 3;
        public static bool enableParser = true;
        public static bool shortCircuit = false;
        public static bool autocomplete = true;
        public static bool showLog = true;
        public static byte hintsLang = 0;
        public static byte highlight = 2; // 0 = Original, 1 = FGeck, 2 = Dark
        public static byte encoding = (byte)EncodingType.Default; // 0 = DEFAULT, 1 = DOS(cp866)
        public static bool allowDefine = false;
        public static bool parserWarn = true;
        public static bool useWatcom = false;
        public static string preprocDef = null;
        public static bool ignoreCompPath = true;
        public static bool userCmdCompile = false;
        public static bool msgHighlightComment = true;
        public static byte msgHighlightColor = 0;
        public static byte msgFontSize = 0;
        public static string pathHeadersFiles;
        public static bool associateID = false;
        public static bool useMcpp = true;
        public static bool autocompleteColor = true;
        public static bool autoInputPaired = true;
        public static bool showTabsChar = false;
        public static bool autoTrailingSpaces = true;
        public static bool showTips = true;
        public static bool shortDesc = false;
        public static byte selectFont = 11; // 0 - default
        public static sbyte sizeFont = 0; // 0 - default
        public static bool showVRuler = true;
        public static bool storeLastPosition = true;
        public static bool saveScriptUTF8 = false;
        public static bool decompileF1 = false;
        public static bool oldDecompile = false;
        public static bool winAPITextRender = true;
        public static int compileBackwardMode = 0;
        private static string ExternalEditorExePath;
        public static bool searchIgnoreCase = false;
        public static bool searchWholeWord = false;
        public static string solutionProjectFolder;

        // for Flowchart
        public static bool autoUpdate = false;
        public static bool autoSaveChart = true;
        public static bool autoArrange = false;
        public static bool woExitNode = true;
        public static bool autoHideNodes = false;

        // no saved settings
        public static bool msgLipColumn = false;
        public static bool firstRun = false;

        public static void SetupWindowPosition(SavedWindows window, Form f)
        {
            WindowPos wp = windowPositions[(int)window];
            if (wp.width == 0)
                return;
            f.Location = new System.Drawing.Point(wp.x, wp.y);
            if (wp.maximized)
                f.WindowState = FormWindowState.Maximized;
            else
                f.ClientSize = new System.Drawing.Size(wp.width, wp.height);
            f.StartPosition = FormStartPosition.Manual;
        }

        public static void SaveWindowPosition(SavedWindows window, Form f)
        {
            WindowPos wp = new WindowPos();
            wp.maximized = f.WindowState == FormWindowState.Maximized;
            wp.x = f.Location.X;
            wp.y = f.Location.Y;
            wp.width = f.ClientSize.Width;
            wp.height = f.ClientSize.Height;
            windowPositions[(int)window] = wp;
        }

        public static void SetLastScriptPosition(string script, int line)
        {
            if (!storeLastPosition || line < 10)
                return;
            if (scriptPosition.ContainsKey(script))
                scriptPosition[script] = (ushort)line;
            else
                scriptPosition.Add(script, (ushort)line);
        }

        public static int GetLastScriptPosition(string script)
        {
            if (scriptPosition.ContainsKey(script))
                return (ushort)scriptPosition[script];
            else
                return 0;
        }

        public static void AddMsgRecentFile(string s, bool b = false)
        {
            SubRecentFile(ref recentMsg, s, b);
        }

        public static void AddRecentFile(string s, bool b = false)
        {
            SubRecentFile(ref recent, s, b, true);
        }

        public static void SubRecentFile(ref List<string> recent, string s, bool b, bool p = false)
        {
            for (int i = 0; i < recent.Count; i++) {
                if (string.Compare(recent[i], s, true) == 0)
                    recent.RemoveAt(i--);
            }
            if (!b && recent.Count >= MAX_RECENT) {
                if (p) scriptPosition.Remove(Path.GetFileName(recent[0]));
                recent.RemoveAt(0);
            }
            if (!b) recent.Add(s);
        }

        public static void ClearRecent()
        {
            recent.Clear();
        }

        public static bool IsSearchIncludes
        {
            get { return (searchIncludePath && pathScriptsHFile != null); }
        }

        public static string[] GetRecent() { return recent.ToArray(); }
        public static string[] GetMsgRecent() { return recentMsg.ToArray(); }

        private static void LoadWindowPos(BinaryReader br, int i)
        {
            windowPositions[i].maximized = br.ReadBoolean();
            windowPositions[i].x = br.ReadInt32();
            windowPositions[i].y = br.ReadInt32();
            windowPositions[i].width = br.ReadInt32();
            windowPositions[i].height = br.ReadInt32();
        }

        private static void LoadInternal(BinaryReader br, BinaryReader brRecent)
        {
            if (br != null) {
                string temp;
                try {
                    firstRun = br.ReadBoolean();
                    allowDefine = br.ReadBoolean();
                    hintsLang = br.ReadByte();
                    highlight = br.ReadByte();
                    encoding = br.ReadByte();
                    optimize = br.ReadByte();
                    showWarnings = br.ReadBoolean();
                    showDebug = br.ReadBoolean();
                    searchIncludePath = br.ReadBoolean();

                    temp = br.ReadString();
                    if (temp.Length > 0) outputDir = temp;

                    warnOnFailedCompile = br.ReadBoolean();
                    multiThreaded = br.ReadBoolean();

                    temp = br.ReadString();
                    if (temp.Length > 0) lastMassCompile = temp;

                    temp = br.ReadString();
                    if (temp.Length > 0) lastSearchPath = temp;

                    LoadWindowPos(br, 0);
                    editorSplitterPosition = br.ReadInt32();
                    autoOpenMsgs = br.ReadBoolean();
                    editorSplitterPosition2 = br.ReadInt32();

                    temp = br.ReadString();
                    if (temp.Length > 0) pathHeadersFiles = temp;

                    language = br.ReadString();
                    if (language.Length == 0) language = "english";

                    tabsToSpaces = br.ReadBoolean();
                    tabSize = br.ReadInt32();
                    enableParser = br.ReadBoolean();
                    shortCircuit = br.ReadBoolean();
                    autocomplete = br.ReadBoolean();
                    showLog = br.ReadBoolean();
                    parserWarn = br.ReadBoolean();
                    useWatcom = br.ReadBoolean();

                    temp = br.ReadString();
                    if (temp.Length > 0) preprocDef = temp;

                    ignoreCompPath = br.ReadBoolean();

                    byte MsgItems = br.ReadByte();
                    for (byte i = 0; i < MsgItems; i++) msgListPath.Add(br.ReadString());

                    openMsgEditor = br.ReadBoolean();
                    userCmdCompile = br.ReadBoolean();
                    msgHighlightComment = br.ReadBoolean();
                    msgHighlightColor = br.ReadByte();
                    msgFontSize = br.ReadByte();

                    temp = br.ReadString();
                    if (temp.Length > 0) pathScriptsHFile = temp;

                    associateID = br.ReadBoolean();
                    //
                    autoUpdate = br.ReadBoolean();
                    autoSaveChart = br.ReadBoolean();
                    autoArrange = br.ReadBoolean();
                    woExitNode = br.ReadBoolean();
                    useMcpp = br.ReadBoolean();
                    autocompleteColor = br.ReadBoolean();
                    autoInputPaired = br.ReadBoolean();
                    showTabsChar = br.ReadBoolean();
                    autoTrailingSpaces = br.ReadBoolean();
                    showTips = br.ReadBoolean();
                    shortDesc = br.ReadBoolean();
                    autoHideNodes = br.ReadBoolean();
                    selectFont = br.ReadByte();
                    sizeFont = br.ReadSByte();
                    showVRuler = br.ReadBoolean();
                    storeLastPosition = br.ReadBoolean();
                    saveScriptUTF8 = br.ReadBoolean();
                    decompileF1 = br.ReadBoolean();
                    winAPITextRender = br.ReadBoolean();

                    temp = br.ReadString();
                    if (temp.Length > 0) ExternalEditorExePath = temp;

                    oldDecompile = br.ReadBoolean();
                    searchIgnoreCase = br.ReadBoolean();
                    searchWholeWord = br.ReadBoolean();

                    temp = br.ReadString();
                    if (temp.Length > 0) solutionProjectFolder = temp;
                } catch {
                    MessageBox.Show("An error occurred while reading configuration file.\n"
                                    + "File setting.dat may be in wrong format.", "Setting read error");
                }
                br.Close();
            }
            // Recent files
            if (brRecent == null) return;
            int recentItems = brRecent.ReadByte();
            int recentMsgItems = brRecent.ReadByte();
            for (int i = 0; i < recentItems; i++)
                recent.Add(brRecent.ReadString());
            for (int i = 0; i < recentMsgItems; i++)
                recentMsg.Add(brRecent.ReadString());
            //
            int positionItems = brRecent.ReadByte();
            for (int i = 0; i < positionItems; i++)
                scriptPosition.Add(brRecent.ReadString(), brRecent.ReadUInt16());
            brRecent.Close();
        }

        public static void Load()
        {
            Program.printLog("   Load configuration setting.");

            if (!Directory.Exists(scriptTempPath)) {
                Directory.CreateDirectory(scriptTempPath);
            } else
                foreach (string file in Directory.GetFiles(scriptTempPath))
                    File.Delete(file);
            File.Delete("errors.txt");

            if (!Directory.Exists(SettingsFolder)) {
                Directory.CreateDirectory(SettingsFolder);
            }
            if (!Directory.Exists(ResourcesFolder)) {
                Directory.CreateDirectory(ResourcesFolder);
            }
            var templatesFolder = Path.Combine(ResourcesFolder, "templates");
            if (!Directory.Exists(templatesFolder)) {
                Directory.CreateDirectory(templatesFolder);
            }

            BinaryReader brRecent = null, brSettings = null;
            if (File.Exists(RecentPath))
                brRecent = new BinaryReader(File.OpenRead(RecentPath));
            if (File.Exists(SettingsPath))
                brSettings = new BinaryReader(File.OpenRead(SettingsPath));
            LoadInternal(brSettings, brRecent);

            LoadScriptsProceduresFolding();

            if (!File.Exists(SearchHistoryPath))
                File.Create(SearchHistoryPath).Close();
            if (!File.Exists(PreprocDefPath))
                File.Create(PreprocDefPath).Close();

            if (!firstRun) {
                var culture = System.Globalization.CultureInfo.CurrentCulture;
                Settings.hintsLang = (byte)((culture.ToString() == "ru-RU") ? 1 : 0);
                FileAssociation.Associate();
            }

            EncCodePage = (encoding == (byte)EncodingType.OEM866) ? Encoding.GetEncoding("cp866") : Encoding.Default;

            //Load custom fonts
            try {
                foreach (string file in Directory.GetFiles(Settings.ResourcesFolder + @"\fonts\", "*.ttf"))
                    Fonts.AddFontFile(file);
            } catch (DirectoryNotFoundException ) { }
        }

        public static void SetTextAreaFont(ICSharpCode.TextEditor.TextEditorControl TE)
        {
            if (Fonts.Families.Length == 0)
                return;

            int indexFont = selectFont - 1;
            Font font;
            if (indexFont > -1) {
                FontFamily family = Fonts.Families[indexFont];
                float sz;
                if (!FontAdjustSize.TryGetValue(family.Name, out sz))
                    sz = 10.0f;
                font = new Font(family, sz + sizeFont, FontStyle.Regular, GraphicsUnit.Point);
            } else
                font = new Font("Courier New", 10.0f + sizeFont, FontStyle.Regular);
            TE.TextEditorProperties.Font = font;
        }

        private static void WriteWindowPos(BinaryWriter bw, int i)
        {
            bw.Write(windowPositions[i].maximized);
            bw.Write(windowPositions[i].x);
            bw.Write(windowPositions[i].y);
            bw.Write(windowPositions[i].width);
            bw.Write(windowPositions[i].height);
        }

        public static void Save()
        {
            if (!Directory.Exists(SettingsFolder))
                Directory.CreateDirectory(SettingsFolder);
            BinaryWriter bw = new BinaryWriter(File.Create(SettingsPath));
            bw.Write((byte)255);
            bw.Write(allowDefine);
            bw.Write(hintsLang);
            bw.Write(highlight);
            bw.Write(encoding);
            bw.Write(optimize);
            bw.Write(showWarnings);
            bw.Write(showDebug);
            bw.Write(searchIncludePath);
            bw.Write(outputDir ?? "");
            bw.Write(warnOnFailedCompile);
            bw.Write(multiThreaded);
            bw.Write(lastMassCompile ?? "");
            bw.Write(lastSearchPath ?? "");
            WriteWindowPos(bw, 0);
            bw.Write(editorSplitterPosition);
            bw.Write(autoOpenMsgs);
            bw.Write(editorSplitterPosition2);
            bw.Write(pathHeadersFiles ?? "");
            bw.Write(language ?? "english");
            bw.Write(tabsToSpaces);
            bw.Write(tabSize);
            bw.Write(enableParser);
            bw.Write(shortCircuit);
            bw.Write(autocomplete);
            bw.Write(showLog);
            bw.Write(parserWarn);
            bw.Write(useWatcom);
            bw.Write(preprocDef ?? string.Empty);
            bw.Write(ignoreCompPath);
            bw.Write((byte)msgListPath.Count);
            for (int i = 0; i < msgListPath.Count; i++)
                bw.Write(msgListPath[i]);
            bw.Write(openMsgEditor);
            bw.Write(userCmdCompile);
            bw.Write(msgHighlightComment);
            bw.Write(msgHighlightColor);
            bw.Write(msgFontSize);
            bw.Write(pathScriptsHFile ?? "");
            bw.Write(associateID);
            //
            bw.Write(autoUpdate);
            bw.Write(autoSaveChart);
            bw.Write(autoArrange);
            bw.Write(woExitNode);
            bw.Write(useMcpp);
            bw.Write(autocompleteColor);
            bw.Write(autoInputPaired);
            bw.Write(showTabsChar);
            bw.Write(autoTrailingSpaces);
            bw.Write(showTips);
            bw.Write(shortDesc);
            bw.Write(autoHideNodes);
            bw.Write(selectFont);
            bw.Write(sizeFont);
            bw.Write(showVRuler);
            bw.Write(storeLastPosition);
            bw.Write(saveScriptUTF8);
            bw.Write(decompileF1);
            bw.Write(winAPITextRender);
            bw.Write(ExternalEditorExePath ?? "");
            bw.Write(oldDecompile);
            bw.Write(searchIgnoreCase);
            bw.Write(searchWholeWord);
            bw.Write(solutionProjectFolder ?? "");
            bw.Close();

            // Recent files
            BinaryWriter bwRecent = new BinaryWriter(File.Create(RecentPath));
            bwRecent.Write((byte)recent.Count);
            bwRecent.Write((byte)recentMsg.Count);
            for (int i = 0; i < recent.Count; i++)
                bwRecent.Write(recent[i]);
            for (int i = 0; i < recentMsg.Count; i++)
                bwRecent.Write(recentMsg[i]);
            //
            string[] key = new string[scriptPosition.Count];
            ushort[] value = new ushort[scriptPosition.Count];
            scriptPosition.Keys.CopyTo(key, 0);
            scriptPosition.Values.CopyTo(value, 0);
            bwRecent.Write((byte)scriptPosition.Count);
            for (int i = 0; i < scriptPosition.Count; i++) {
                bwRecent.Write(key[i]);
                bwRecent.Write(value[i]);
            }
            bwRecent.Close();
            // Store folding procedures
            SaveScriptsProceduresFolding();
        }

        public static void SaveSettingData(Form mainfrm)
        {
            TextEditor frm = mainfrm as TextEditor;
            StreamWriter sw = new StreamWriter(Settings.SearchHistoryPath);
            int capHSearchHistory = 150;
            foreach (var item in frm.SearchTextComboBox.Items)
            {
                sw.WriteLine(item.ToString());
                if (--capHSearchHistory < 0) break;
            }
            sw.Close();
            File.WriteAllLines(Settings.SearchFoldersPath, searchListPath);
            openMsgEditor = frm.msgAutoOpenEditorStripMenuItem.Checked;
            if (frm.WindowState != FormWindowState.Minimized) SaveWindowPosition(SavedWindows.Main, mainfrm);
            Save();
            Directory.Delete(scriptTempPath, true);
        }

        public static void OpenInExternalEditor(string file)
        {
            if (ExternalEditorExePath != null && File.Exists(ExternalEditorExePath))
                System.Diagnostics.Process.Start(ExternalEditorExePath, file);
            else {
                var ofd = new OpenFileDialog() {
                    InitialDirectory = Settings.ProgramFolder,
                    DefaultExt = "exe",
                    Filter = "Executable file (.exe)|*.exe",
                    Title = "Select executable file"
                };
                if (ofd.ShowDialog() == DialogResult.OK) {
                    ExternalEditorExePath = ofd.FileName;
                    OpenInExternalEditor(file);
                }
            }
        }

        struct WindowPos
        {
            public bool maximized;
            public int x, y, width, height;
        }

        #region Folding save/load data
        private static readonly string FoldingPath = Path.Combine(SettingsFolder, "folding.dat");

        private static Dictionary<string, int> scriptsFolding = new Dictionary<string,int>(); // имя скрипта => позиция в массиве proceduresFolding
        private static HashSet<string>[] proceduresFolding; // имена процедур которые являются свернутыми

        public static void SetScriptProcedureFold(string scriptName, string procedure)
        {
            int index;
            if (!scriptsFolding.TryGetValue(scriptName, out index)) {
                index = proceduresFolding.Length;
                Array.Resize(ref proceduresFolding, index + 1);
                proceduresFolding[index] = new HashSet<string>();
                proceduresFolding[index].Add(procedure);
                scriptsFolding.Add(scriptName, index);
            } else {
                proceduresFolding[index].Add(procedure);
            }
        }

        public static void UsSetScriptProcedureFold(string scriptName, string procedure)
        {
            int index;
            if (!scriptsFolding.TryGetValue(scriptName, out index)) return;
            proceduresFolding[index].Remove(procedure);
        }

        public static bool ScriptProcedureIsFold(string scriptName, string procedure)
        {
            int index;
            if (!scriptsFolding.TryGetValue(scriptName, out index)) return false;
            return proceduresFolding[index].Contains(procedure);
        }

        private static void SaveScriptsProceduresFolding()
        {
            int sCount = scriptsFolding.Count;
            if (sCount == 0) {
                File.Delete(FoldingPath);
                return;
            }
            string[] keys = new string[sCount];
            scriptsFolding.Keys.CopyTo(keys, 0);
            int[] indexes = new int[sCount];
            scriptsFolding.Values.CopyTo(indexes, 0);

            BinaryWriter bwFolding = new BinaryWriter(File.Create(FoldingPath));
            bwFolding.Write(sCount);
            for (int i = 0; i < sCount; i++) {
                bwFolding.Write(keys[i]);
                int pCount = proceduresFolding[indexes[i]].Count;
                bwFolding.Write(pCount);
                string[] procedures = new string[pCount];
                proceduresFolding[indexes[i]].CopyTo(procedures, 0);
                for (int j = 0; j < pCount; j++) {
                    bwFolding.Write(procedures[j]);
                }
            }
            bwFolding.Close();
        }

        private static void LoadScriptsProceduresFolding()
        {
            if (File.Exists(FoldingPath)) {
                BinaryReader brFolding = new BinaryReader(File.OpenRead(FoldingPath));
                if (brFolding.BaseStream.Length == 0) {
                    brFolding.Close();
                    proceduresFolding = new HashSet<string>[0];
                    return;
                }
                int sCount = brFolding.ReadInt32();
                proceduresFolding = new HashSet<string>[sCount];
                for (int i = 0; i < sCount; i++) {
                    string script = brFolding.ReadString();
                    int pCount =  brFolding.ReadInt32();
                    if (pCount == 0) continue;
                    proceduresFolding[i] = new HashSet<string>();
                    for (int j = 0; j < pCount; j++) {
                        proceduresFolding[i].Add(brFolding.ReadString());
                    }
                    scriptsFolding.Add(script, i);
                }
                brFolding.Close();
            } else {
                proceduresFolding = new HashSet<string>[0];
            }
        }
        #endregion
    }
}
