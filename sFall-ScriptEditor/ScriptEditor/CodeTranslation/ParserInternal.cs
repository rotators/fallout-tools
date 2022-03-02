using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ScriptEditor.TextEditorUI;

using ICSharpCode.TextEditor.Document;

namespace ScriptEditor.CodeTranslation
{
    public struct ProcedureBlock
    {
        public int declar;      // Строка декларации процедуры
        public int begin;       // Строка указывающая на begin процедуры
        public int end;         // Строка указывающая на end процедуры
        public bool copy;
    }

    /// <summary>
    /// Class for parsing procedures SSL code w/o external parser.dll
    /// </summary>
    public class ParserInternal
    {
        private static readonly char[] pathSeparators = { '\\', '/' };

        private static List<string> procNameList = new List<string>();
        private static TextEditor scrptEditor;

        private static string[] bufferSSLCode; // text buffer for ssl script code

        public const int PROC_LEN = 10;
        public const string PROCEDURE = "procedure ";
        public const string BEGIN = "begin";
        private const string END = "end";
        public const string INCLUDE = "#include ";
        public const string DEFINE = "#define ";
        private const string COMMENT = "//";
        public const string VARIABLE = "variable ";

        public static List<string> ProceduresListName
        {
            get {
                GetNamesProcedures();
                return procNameList;
            }
        }

        private static int CountProcedures
        {
            get {
                GetNamesProcedures();
                return procNameList.Count;
            }
        }

        private static void ProcedureRemoveSpec(ref string sslCode)
        {
            if (sslCode.Length < 15) return;
            int lookLoop = 0;
            do {
                if (sslCode.StartsWith("import ") || sslCode.StartsWith("export ") || sslCode.StartsWith("inline ")) {
                    sslCode = sslCode.Remove(0, 7).TrimStart();
                    lookLoop++;
                } else if (sslCode.StartsWith("critical ")) {
                    sslCode = sslCode.Remove(0, 9).TrimStart();
                    lookLoop++;
                } else if (sslCode.StartsWith("pure ")) {
                    sslCode = sslCode.Remove(0, 5).TrimStart();
                    lookLoop++;
                }
            } while (--lookLoop >= 0);
        }

        /// <summary>
        /// Internal parse script
        /// </summary>
        /// <param name="_ti"></param>
        /// <param name="frm"></param>
        public ParserInternal(TabInfo _ti, Form frm)
        {
            scrptEditor = frm as TextEditor;
            TextEditor.parserIsRunning = true; // internal parse work

            UpdateParseBuffer(_ti.textEditor.Text, false);

            ProgramInfo _pi = new ProgramInfo(CountProcedures, 0);
            _ti.parseInfo = InternalProcParse(_pi, _ti.textEditor.Text, _ti.filepath);

            TextEditor.parserIsRunning = false;
        }

        public static ProgramInfo UpdatePI(ProgramInfo pi, string filepath)
        {
            string name = Path.GetFileName(filepath);

            for (int i = 0; i < pi.procs.Length; i++)
            {
                var p = pi.procs[i];
                p.fdeclared = filepath;
                p.fstart = filepath;
                p.filename = name;

                for (int j = 0; j < p.references.Length; j++)
                {
                    p.references[j].file = filepath;
                }
                for (int j = 0; j < p.variables.Length; j++)
                {
                    p.variables[j].fdeclared = filepath;
                    //p.variables[j].filename = name;
                    for (int n = 0; n < p.variables[j].references.Length; n++)
                    {
                        p.variables[j].references[n].file = filepath;
                    }
                }
            }

            for (int i = 0; i < pi.vars.Length; i++)
            {
                var v = pi.vars[i];
                v.fdeclared = filepath;
                v.filename = name;

                for (int j = 0; j < v.references.Length; j++)
                {
                    v.references[j].file = filepath;
                }
            }
            return pi;
        }

        /// <summary>
        /// Обновляет данные о процедурах, удаляет устаревшие или добавляет новые
        /// </summary>
        /// <param name="_pi"></param>
        /// <param name="textscript"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static ProgramInfo UpdateProcsPI(ProgramInfo _pi, string textscript, string filepath)
        {
            List<Procedure> _proc = _pi.procs.ToList();

            UpdateParseBuffer(textscript, false);
            ProgramInfo update_pi = InternalProcParse(new ProgramInfo(CountProcedures, 0), textscript, filepath);

            for (int i = 0; i < _proc.Count; i++)
            {
                bool exist = false;
                foreach (Procedure p in update_pi.procs)
                {
                    if (p.name.Equals(_proc[i].name, StringComparison.OrdinalIgnoreCase)
                        && _proc[i].fdeclared == p.fdeclared) {
                        _proc[i].d.start = p.d.start;
                        _proc[i].d.end = p.d.end;
                        _proc[i].d.declared = p.d.declared;
                        _proc[i].d.defined = p.d.defined;
                        exist = true;
                        break;
                    }
                }
                if (!exist && _proc[i].IsLocal(filepath)) _proc.RemoveAt(i--); // remove unused local procedure
            }

            for (int i = 0; i < update_pi.procs.Length; i++)
            {
                bool exist = false;
                for (int j = 0; j < _proc.Count; j++)
                {
                    if (update_pi.procs[i].name.Equals(_proc[j].name, StringComparison.OrdinalIgnoreCase)
                        && update_pi.procs[i].fdeclared == _proc[j].fdeclared) {
                            exist = true;
                            break;
                    }
                }
                if (!exist) _proc.Insert(i, update_pi.procs[i]); // Add new procedure
            }
            _pi.procs = _proc.ToArray();
            _pi.RebuildProcedureDictionary();

            return _pi;
        }

        /// <summary>
        /// Получает новые данные о процедурах из кода скрипта
        /// </summary>
        /// <param name="_pi"></param>
        /// <param name="text">Текущий текст скрипта</param>
        /// <param name="scriptFile">Файл скрипта</param>
        /// <returns></returns>
        private static ProgramInfo InternalProcParse(ProgramInfo _pi, string text, string scriptFile, bool bufferUpdate = true)
        {
            #region Procedures info data
            /*  pi.procs[].d.start        - номер строки начала тела процедуры
             *  pi.procs[].d.end          - номер строки конца тела процедуры
             *  pi.procs[].d.declared     - номер строки с объявление процедуры
             *  pi.procs[].name           - имя процедуры
             *  pi.procs[].fdeclared      - путь и имя к файлу где объявлена процедура
             *  pi.procs[].fstart         - путь и имя к файлу где расположана процедура
             *  pi.procs[].filename       - имя файла скрипта */
            #endregion

            if (bufferUpdate) UpdateParseBuffer(text);

            List<string> procedureNames;
            List<ProcedureBlock> listBlock = GetAllProceduresBlock(out procedureNames);
            for (int i = 0; i < procNameList.Count; i++)
            {
                _pi.procs[i] = new Procedure();
                _pi.procs[i].name = procNameList[i];
                int n = procedureNames.FindIndex(name => name == procNameList[i].ToLowerInvariant());
                if (n == -1 || n >= listBlock.Count) {
                    MessageBox.Show(String.Format("Error: The procedure '{0}' was not found in the checklist.", procNameList[i]), "Internal Parser");
                    continue;
                }
                _pi.procs[i].d.declared = listBlock[n].declar + 1;
                _pi.procs[i].d.start = (listBlock[n].begin >= 0) ? listBlock[n].begin + 1 : -1;
                _pi.procs[i].d.end = (listBlock[n].end >= 0) ? listBlock[n].end + 1 : -1;
                _pi.procs[i].d.defined = _pi.procs[i].d.start;
                _pi.procs[i].fdeclared = Path.GetFullPath(scriptFile);
                _pi.procs[i].fstart = _pi.procs[i].fdeclared;
                _pi.procs[i].filename = Path.GetFileName(scriptFile).ToLowerInvariant();
                _pi.procs[i].references = new Reference[0];  // empty not used
                _pi.procs[i].variables = new Variable[0];    // empty not used
            }
            _pi.parsed = true;

            bufferSSLCode = null;

            return _pi;
        }

        /// <summary>
        /// Обновляет данные для процедур позицию строк деклараций и блока begin/end из кода скрипта
        /// </summary>
        /// <param name="_pi">Данные которые требуется обновить</param>
        /// <param name="text">Текущий текст скрипта</param>
        /// <param name="scriptFile">Файл скрипта</param>
        public static void UpdateProcInfo(ref ProgramInfo _pi, string text, string scriptFile)
        {
            UpdateParseBuffer(text);

            List<string> procedureNames;
            List<ProcedureBlock> listBlock = GetAllProceduresBlock(out procedureNames);

            for (int i = 0; i < _pi.procs.Length; i++)
            {
                if (_pi.procs[i].fdeclared != scriptFile) continue;

                string pName = _pi.procs[i].Name;
                int n = procedureNames.FindIndex(name => name == pName);
                if (n == -1 || n >= listBlock.Count) continue;

                _pi.procs[i].d.declared = listBlock[n].declar + 1;
                _pi.procs[i].d.start = (listBlock[n].begin >= 0) ? listBlock[n].begin + 1 : -1;
                _pi.procs[i].d.end = (listBlock[n].end >= 0) ? listBlock[n].end + 1 : -1;
                _pi.procs[i].d.defined = _pi.procs[i].d.start;
            }
            _pi.RebuildProcedureDictionary();

            bufferSSLCode = null;
        }

        // Get new procedure data of script
        public static Procedure[] GetProcsData(string textscript, string filepath)
        {
            UpdateParseBuffer(textscript, false); // Не переводить в нижний регист для получения правильных имен процедур

            ProgramInfo _pi = InternalProcParse(new ProgramInfo(CountProcedures, 0), textscript, filepath);

            return _pi.procs;
        }

        // Получить список всех процедур из текущего буфера
        private static void GetNamesProcedures()
        {
            int _comm = 0;
            procNameList.Clear();

            for (int i = 0; i < bufferSSLCode.Length; i++)
            {
                bool addList = true;
                bufferSSLCode[i] = bufferSSLCode[i].TrimStart();

                if (CommentBlockParse(ref bufferSSLCode[i], ref _comm)) continue;

                ProcedureRemoveSpec(ref bufferSSLCode[i]);
                if (bufferSSLCode[i].StartsWith(PROCEDURE, StringComparison.OrdinalIgnoreCase)) {
                    // get name procedure
                    string pName = bufferSSLCode[i].Substring(PROC_LEN, bufferSSLCode[i].Length - PROC_LEN).TrimEnd();
                    if (pName.Length == 0) continue;

                    RemoveCommentLine(ref pName, 0);

                    // delete Begin or other information from procedure name
                    bool cut = false;
                    int z = pName.IndexOf('(');
                    if (z > 0) {
                        pName = pName.Remove(z);
                        cut = true;
                    } else {
                        z = pName.IndexOf(';');
                        if (z > 0) {
                            pName = pName.Remove(z);
                            cut = true;
                        }
                    }
                    if (!cut) {
                        z = pName.ToLowerInvariant().IndexOf(" " + BEGIN);
                        if (z > 0) pName = pName.Remove(z);
                    }
                    pName = pName.TrimEnd();

                    foreach (string name in procNameList)
                    {
                        if (String.Equals(pName, name, StringComparison.OrdinalIgnoreCase)) {
                            addList = false;
                            break;
                        }
                    }
                    if (addList) procNameList.Add(pName);
                }
            }
        }

        /// <summary>
        /// Получает правильное(регистрово зависимое) имя процедуры определенное в скрипте
        /// </summary>
        /// <param name="procName"></param>
        /// <returns></returns>
        public static string GetCaseCorrectProcedureName(string procName)
        {
            foreach (string name in procNameList) {
                if (String.Equals(procName, name, StringComparison.OrdinalIgnoreCase)) {
                    return name;
                }
            }
            return null;
        }

        /// <summary>
        /// Получает список блоков с номерами строк для всех процедур скрипта
        /// </summary>
        /// <param name="procNameLst"></param>
        /// <returns></returns>
        private static List<ProcedureBlock> GetAllProceduresBlock(out List<string> procNameLst)
        {
            procNameLst = new List<string>();
            List<ProcedureBlock> procBlockLst = new List<ProcedureBlock>();
            Dictionary<string, int> declarLst = new Dictionary<string, int>();

            int  declarLine, beginLine, commFlag = 0;

            for (int i = 0; i < bufferSSLCode.Length; i++)
            {
                bufferSSLCode[i] = bufferSSLCode[i].TrimStart(); // буфер должен быть в нижнем регистре

                if (CommentBlockParse(ref bufferSSLCode[i], ref commFlag)) continue;

                bufferSSLCode[i] = bufferSSLCode[i].Replace('\t', ' ');

                ProcedureRemoveSpec(ref bufferSSLCode[i]);
                if (bufferSSLCode[i].StartsWith(PROCEDURE)) {
                    // get name procedure
                    string pName = bufferSSLCode[i].Substring(PROC_LEN, bufferSSLCode[i].Length - PROC_LEN).TrimStart();
                    if (pName.Length == 0) continue;

                    RemoveCommentLine(ref pName, 0); // удаляем комментарии с конца строки

                    declarLine = -1;
                    beginLine = -1;

                    // delete Begin or other information from procedure name
                    int z = pName.IndexOf(';');
                    if (z > 0) {
                        pName = pName.Remove(z);
                        declarLine = i; // это строка обявления
                    } else {
                        beginLine = i; // это начало блока процедуры
                        z = pName.IndexOf(" " + BEGIN);
                        if (z > 0) {
                            pName = pName.Remove(z);
                            //beginLine = i; // это начало блока процедуры
                        }/* else {
                            while (++i < bufferSSLCode.Length)
                            {
                                 if (CommentBlockParse(ref bufferSSLCode[i], ref commFlag)) continue;
                                 // в процессе проверяем и любое объявление процедур
                                 if (bufferSSLCode[i].StartsWith(PROCEDURE)) break;
                                 if (bufferSSLCode[i].TrimStart().StartsWith(BEGIN)) {
                                    beginLine = i; // это начало блока процедуры
                                    break;
                                 }
                            }
                        }*/
                    }
                    z = pName.IndexOf('(');
                    if (z > 0) pName = pName.Remove(z);

                    pName = pName.TrimEnd();

                    if (declarLine != -1) {
                        if (!declarLst.ContainsKey(pName)) declarLst.Add(pName, declarLine);
                        continue;
                    }
                    int lstIndex = -1;
                    for (int j = 0; j < procNameLst.Count; j++ ) {
                        if (pName == procNameLst[j]) {
                            lstIndex = j;
                            break;
                        }
                    }
                    if (lstIndex == -1) {
                        procNameLst.Add(pName);
                        ProcedureBlock block = new ProcedureBlock() { begin = beginLine, end = -1 };
                        block.declar = (declarLst.ContainsKey(pName)) ? declarLst[pName] : -1;
                        procBlockLst.Add(block);
                    } else if (beginLine != -1) {
                        ProcedureBlock block = procBlockLst[lstIndex];
                        block.begin = beginLine;
                        procBlockLst[lstIndex] = block;
                    }
                }
            }
            // получаем строки конца блока процедур
            for (int i = 0; i < procBlockLst.Count; i++)
            {
                beginLine = procBlockLst[i].begin;
                if (beginLine != -1) {
                    int nextBegin;
                    if (i == procBlockLst.Count - 1) {
                        nextBegin = bufferSSLCode.Length - 1; // для последней процедуры в скрипте
                    } else {
                        nextBegin = procBlockLst[i + 1].begin;
                    }
                    if (nextBegin == -1) {
                        ProcedureBlock block = procBlockLst[i];
                        block.end = beginLine + 1; // +2 ??
                        procBlockLst[i] = block;
                    } else {
                        for (int j = nextBegin; j > beginLine; j--) { // back find
                            if (bufferSSLCode[j].StartsWith(END)) {   // found "end"
                                if (bufferSSLCode[j].Length > 3 && !char.IsWhiteSpace(bufferSSLCode[j][3])) continue;
                                ProcedureBlock block = procBlockLst[i];
                                block.end = j;
                                procBlockLst[i] = block;
                                break;
                            }
                        }
                        if (procBlockLst[i].end == -1) {  // procedure block end is broken
                            scrptEditor.intParserPrint(String.Format("[Error] <Internal Parser> Line: {0} : When parsing of procedure '{1}' of the keyword 'end' was not found.\r\n", beginLine + 1, GetCaseCorrectProcedureName(procNameLst[i])));
                        }
                    }
                }
            }
            // добавить процедуры не имеющие блока кода
            foreach (var el in declarLst)
            {
                if (!procNameLst.Contains(el.Key)) {
                    procNameLst.Add(el.Key);
                    procBlockLst.Add(new ProcedureBlock() { begin =-1, end = -1, declar = el.Value });

                    scrptEditor.intParserPrint(String.Format("[Warning] <Internal Parser> Line: {0} : When parsing the procedure '{1}' an unexpected error occurred," +
                                                             " the construction of the code was not determined.\r\n", el.Value + 1, GetCaseCorrectProcedureName(el.Key)));
                }
            }
            return procBlockLst;
        }

        /// <summary>
        /// Получить номер строки объявления для процедуры
        /// </summary>
        /// <param name="pName">Имя процедуры</param>
        /// <returns>Номер строки в коде скрипта (-1 если строка декларация процедуры не найдена)</returns>
        public static int GetDeclarationProcedureLine(string pName)
        {
            pName = pName.ToLowerInvariant();
            int pLen = pName.Length;

            for (int i = 0; i < bufferSSLCode.Length; i++)
            {
                bufferSSLCode[i] = bufferSSLCode[i].Trim();

                // TODO: возможно тут нужна проверка на закоментированный блок /* */

                ProcedureRemoveSpec(ref bufferSSLCode[i]);
                if (IsProcedure(ref bufferSSLCode[i], pName)) {
                    if (bufferSSLCode[i].Length <= (PROC_LEN + pLen))
                        continue; // broken declare

                    RemoveCommentLine(ref bufferSSLCode[i], PROC_LEN + pLen);

                    if (bufferSSLCode[i].LastIndexOf(';') >= (PROC_LEN + pLen))
                        return i; //found
                }
            }
            return -1; // not found
        }

        /// <summary>
        /// Получить номер последней строки в списке процедурных деклараций
        /// </summary>
        /// <returns>Номер строки в коде (-1 если строку не удалось получить)</returns>
        public static int GetEndLineProcDeclaration()
        {
            int _comm = 0;

            for (int i = 0; i < bufferSSLCode.Length; i++)
            {
                bufferSSLCode[i] = bufferSSLCode[i].Trim();
                if (CommentBlockParse(ref bufferSSLCode[i], ref _comm)) continue;

                // убираем лишнее
                RemoveCommentLine(ref bufferSSLCode[i], 0);

                if (bufferSSLCode[i].EndsWith(BEGIN)) {
                    ProcedureRemoveSpec(ref bufferSSLCode[i]);

                    if (!bufferSSLCode[i].StartsWith(PROCEDURE) && !bufferSSLCode[i - 1].StartsWith(PROCEDURE)) continue;
                    for (int j = i - 1; j >= 0; j--) {
                        if (bufferSSLCode[j].StartsWith(PROCEDURE)) {
                            return (bufferSSLCode[j].IndexOf(';') > 0) ? j + 1 : j - 1;  // имеется ли в строке знак ';'
                        }
                    }
                    if (++i <= bufferSSLCode.Length) continue;
                    return -2;  // procedure block is broken
                }
            }
            return -1; // not found procedure declaration
        }

        /// <summary>
        /// Получить для указанной процедуры номера ее строк Begin...End блока
        /// </summary>
        /// <param name="pName">Имя процедуры</param>
        /// <param name="startline">Строка в коде с которой необходимо начать поиск</param>
        /// <param name="procedureLine">procBegin=True - получить номер строки процедуры, а не строки Begin блока</param>
        public static ProcedureBlock GetProcedureBlock(string pName, int startline = 0, bool procedureLine = false)
        {
            int _begin = 0, _proc = 0, _comm = 0, lineProc = 0;
            ProcedureBlock procBlock = new ProcedureBlock() { declar = -1 };

            pName = pName.ToLowerInvariant();
            int pLen = pName.Length;
            if (startline < 0) startline = 0;

            for (int i = startline; i < bufferSSLCode.Length; i++)
            {
                bufferSSLCode[i] = bufferSSLCode[i].Trim();
                if (CommentBlockParse(ref bufferSSLCode[i], ref _comm)) continue;

                bufferSSLCode[i] = bufferSSLCode[i].Replace('\t', ' ');

                ProcedureRemoveSpec(ref bufferSSLCode[i]);

                // ищем начало процедуры с искомым именем
                if (_proc == 0 && IsProcedure(ref bufferSSLCode[i], pName))
                {   // нашли procedure name, проверяем 100% совпадает c искомым?
                    procBlock.declar = i;
                    string s = " ";
                    if (bufferSSLCode[i].Length > PROC_LEN + pLen)
                        s = bufferSSLCode[i].Substring(PROC_LEN + pLen, 1);
                    if (s != " " && s != "(")
                        continue; //не совпадает, ищем дальше
                    _proc++; // совпадает, проверяем это процедура или ее объявление
                    // убираем лишнее
                    RemoveCommentLine(ref bufferSSLCode[i], PROC_LEN + pLen);
                    if (bufferSSLCode[i].EndsWith(BEGIN)) {
                        procBlock.begin = i; //да это процедура, присваеваем значение строки в begin
                        _begin++;
                        continue;
                    } else { // нет, продолжаем искать begin
                        lineProc = i; // save for procBegin, строка Procedure name
                        continue;
                    }
                } // ищем begin от процедуры.
                else if (_proc > 0 && _begin == 0) {
                    if (bufferSSLCode[i].StartsWith(BEGIN)) {
                        _begin++; // нашли begin
                        procBlock.begin = (procedureLine) ? lineProc : i; // возвращаем номер строки с процедурой
                        continue;
                    }
                    // в процессе проверяем и любое объявление процедур
                    if (bufferSSLCode[i].StartsWith(PROCEDURE)) {
                        // нашли - откат назад, будем продолжать искать нашу "procedure Name"
                        _proc--;
                        i--;
                        continue;
                    }
                }
                // нашли begin, теперь ищем начало следующей процедуры
                // и от ее позиции будем искать 'END' принадлежащий к искомой "procedure Name"
                if (_proc > 0 && _begin > 0 && bufferSSLCode[i].StartsWith(PROCEDURE)) {
                    // нашли следующую процедуру
                    for (int j = i - 1; j > 0; j--) // back find
                    {
                        if (bufferSSLCode[j].StartsWith(END)) {
                            // found "end"
                            procBlock.end = j;
                            return procBlock ; // return
                        } else if (j <= procBlock.begin) {
                            procBlock.end = -1; //i -
                            scrptEditor.intParserPrint(String.Format("[Error] <Internal Parser> Line: {0} : When parsing of procedure '{1}'" +
                                                                     " of the keyword 'end' was not found.\r\n", procBlock.begin + 1, GetCaseCorrectProcedureName(pName)));
                            return procBlock; // procedure block end is broken
                        }
                    }
                }
            }
            // обработка вслучае последней процедуры в скрипте
            if (procBlock.end == 0 && _proc > 0 && _begin > 0) {
                for (int i = bufferSSLCode.Length - 1; i > 0; i--) // back find
                {
                    if (bufferSSLCode[i].StartsWith(END)) {
                        procBlock.end = i;
                        return procBlock;
                    }
                }
            }
            procBlock.begin = -1;
            procBlock.end = -1;
            scrptEditor.intParserPrint(String.Format("[Warning] <Internal Parser> Line: {0} : When parsing the procedure '{1}' an unexpected error occurred," +
                                                     " the construction of the code was not determined.\r\n", procBlock.declar + 1, GetCaseCorrectProcedureName(pName)));
            return procBlock;
        }

        /// <summary>
        /// Удаляет все комментарии из текстовой строки
        /// </summary>
        /// <param name="pLen">Позиция в строке откуда будет начат поиск</param>
        private static void RemoveCommentLine(ref string buff, int pLen)
        {
            int z = CheckAtComment(buff, pLen);
            if (z > 0)
                buff = buff.Remove(z).TrimEnd();
        }

        private static int CheckAtComment(string buff, int pLen)
        {
            int z = buff.IndexOf(COMMENT, pLen);
            int y = buff.IndexOf("/*", pLen);
            if (z > 0 && y > 0)
                z = Math.Min(z, y);
            else
                z = Math.Max(z, y);

            return z;
        }

        // Comment block parse
        public static bool CommentBlockParse(ref string sLine, ref int _comm)
        {
            if (sLine.Length < 2 || sLine.StartsWith(COMMENT))
                return true;

            int cStart = sLine.IndexOf("/*");
            if (cStart != -1 && _comm == 0) {
                // удаление из строки закомментированного блока '/* ... */'
                int cEnd = sLine.IndexOf("*/");
                if (cEnd < 0) {
                    _comm++;
                    sLine = sLine.Remove(cStart); // clear comment line
                    return true;
                } else
                    sLine = sLine.Remove(cStart, (cEnd + 2) - cStart).Insert(cStart, " ").TrimStart();
            }
            else if (_comm > 0) {
                int cEnd = sLine.IndexOf("*/");
                if (cEnd != -1) {
                    _comm--;
                    if (sLine.Length > 2) {
                        // удаление комментария из строки с закрывающим тэгом '*/'
                        sLine = sLine.Remove(0, cEnd + 2).TrimStart();
                    }
                } else {
                    sLine = string.Empty; // clear comment line
                    return true;
                }
            }
            return false;
        }

        // Определить содержит ли проверяемая строка коючевое слово "procedure " c указанным именем
        private static bool IsProcedure(ref string sLine, string pName)
        {
            if (sLine.StartsWith(PROCEDURE)) {
                // удаление двойных пробелов между словом процедура и ее именем
                int z = sLine.IndexOf("  ", 9);
                if (z > 0) {
                    int x = CheckAtComment(sLine, 9);

                    int y = sLine.IndexOfAny(new char[] {';', ')'}, 9);
                    if (y > 0) { // определяем наименьшее значение x и y
                        if (x == -1 || y < x) // позиция скобки не находится в комментариях
                            x = y;
                    }
                    if (x == -1 || z < x) // двойные пробелы расположены не за пределами проверки
                        sLine = RemoveDoubleWhiteSpaces(sLine, z, x);
                }
                if (sLine.StartsWith(PROCEDURE + pName)) {
                    int ePos = PROC_LEN + pName.Length;
                    // проверяем следующий символ за именем процедуры
                    if (ePos >= sLine.Length || sLine[ePos] == ';' || sLine[ePos] == '('
                        || (!char.IsLetterOrDigit(sLine[ePos]) && !char.IsPunctuation(sLine[ePos])))
                        return true; // строка с именем процедуры совпадает
                }
            }
            return false;
        }

        internal static string RemoveDoubleWhiteSpaces(string sLine, int start, int end)
        {
            char[] ch  = sLine.ToCharArray();
            end = (end > 0) ? end : ch.Length -1;
            for (int i = start; i < end; i++)
            {
                if (char.IsWhiteSpace(ch[i]) && char.IsWhiteSpace(ch[i + 1]))
                    ch[i] = '\0';
            }
            return new string(ch).Replace("\0", string.Empty);
        }

        /// <summary>
        /// Занести во временный буфер текст скрипта
        /// </summary>
        /// <param name="sText">Код скрипта</param>
        /// <param name="lower"></param>
        public static void UpdateParseBuffer(string sText, bool lower = true)
        {
            char delimeter = '\n';
            bufferSSLCode = (lower)
                            ? sText.ToLowerInvariant().Split(delimeter)
                            : sText.Split(delimeter);
        }

        public static void FixProcedureBegin(string file)
        {
            List<string> script = File.ReadAllLines(file, Encoding.Default).ToList();
            for (int i = 0; i < script.Count; i++)
            {
                if (script[i].StartsWith(ParserInternal.PROCEDURE, StringComparison.OrdinalIgnoreCase)
                    || script[i].StartsWith("critical ", StringComparison.OrdinalIgnoreCase))
                {
                    if (script[i + 1].StartsWith(ParserInternal.BEGIN, StringComparison.OrdinalIgnoreCase)) {
                        script[i] += '\u0020' + script[i + 1];
                        script.RemoveAt(i + 1);
                    }
                }
            }
            File.WriteAllLines(file, script, Encoding.Default);
        }

        #region Get declaration region
        /// <summary>
        /// Получить номера строк для региона деклараций
        /// </summary>
        public static ProcedureBlock GetRegionDeclaration(string text, int endLine)
        {
            bufferSSLCode = text.Split(new char[]{'\n'}, endLine + 1);
            int lenBuff = bufferSSLCode.Length - 1;

            int _comm = 0;
            bool ret = false;
            bool check = false;

            ProcedureBlock procBlock = new ProcedureBlock() { begin = 0, end = -1 };

            for (int i = 0; i < lenBuff; i++)
            {
                bufferSSLCode[i] = bufferSSLCode[i].Trim();
                if (ret) {
                    if (bufferSSLCode[i].Length == 0) {
                        check = false;
                        continue;
                    } else {
                        procBlock.begin = (check) ? i - 1 : i; // found begin declaration
                        break;
                    }
                }
                check = (bufferSSLCode[i].Length > 0);
                if (CommentBlockParse(ref bufferSSLCode[i], ref _comm))
                    continue;
                ret = true;
            }

            for (int i = endLine - 1; i > procBlock.begin; i--)
            {
                if (bufferSSLCode[i].Trim().Length > 0) {
                    procBlock.end = i;  // found end declaration
                    break;
                }
            }
            return procBlock;
        }

        /// <summary>
        /// Получает список номеров строк для variable и #if блоков
        /// </summary>
        /// <param name="text"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static List<ProcedureBlock> GetFoldingBlock(string text, int start = 0)
        {
            List<ProcedureBlock> list = new List<ProcedureBlock>();
            ProcedureBlock procBlock = new ProcedureBlock() { begin = -1, end = -1 };
            ProcedureBlock ifBlock = new ProcedureBlock() { begin = -1, end = -1, copy = true};

            int _comm = 0;
            bufferSSLCode = text.Split(new char[]{'\n'});
            int lenBuff = bufferSSLCode.Length;

            for (int i = start; i < lenBuff; i++)
            {
                string buffer = bufferSSLCode[i].TrimStart().ToLowerInvariant();
                if (CommentBlockParse(ref buffer, ref _comm))
                    continue;
                RemoveCommentLine(ref buffer, 0);

                if (ifBlock.begin == -1 && buffer.StartsWith("#if ")) {
                    ifBlock.begin = i;
                    continue;
                } else if (ifBlock.begin > -1 && buffer.StartsWith("#endif")) {
                    ifBlock.end = i;
                    list.Add(ifBlock);
                    ifBlock = new ProcedureBlock() { begin = -1, end = -1, copy = true};
                    continue;
                }

                if (procBlock.begin == -1 && buffer.StartsWith(VARIABLE)) {
                    int boffset = buffer.IndexOf(" " + BEGIN) + 1;
                    if (boffset > 0) {
                        int len = boffset + BEGIN.Length;
                        if (buffer.Length > len) buffer = buffer.Remove(len);

                        int z = buffer.IndexOf("  ", 8);
                        if (z > 0)
                            buffer = RemoveDoubleWhiteSpaces(buffer, z, boffset);

                        if (buffer.StartsWith(VARIABLE + BEGIN)) {
                            procBlock.begin = i;
                            continue;
                        }
                    }
                }
                else if (procBlock.begin > -1 && buffer.StartsWith(END) && (buffer.Length == 3
                         || (buffer.Length > 3 && !TextUtilities.IsLetterDigitOrUnderscore(buffer[3])))) {
                    procBlock.end = i;
                    list.Add(procBlock);
                    procBlock = new ProcedureBlock() { begin = -1, end = -1 };
                }
                else if (procBlock.begin > -1 && (buffer.StartsWith(VARIABLE) || buffer.StartsWith(PROCEDURE)))
                    procBlock.begin = -1;
            }
            return list;
        }
        #endregion

        #region Include files
        public static List<string> GetAllIncludes(TabInfo tab)
        {
            List<string> include = new List<string>();
            string[] lines = tab.textEditor.Document.TextContent.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            string dir = Path.GetDirectoryName(tab.filepath);

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
                if (lines[i].StartsWith(INCLUDE, StringComparison.OrdinalIgnoreCase)) {
                    string[] text = lines[i].Split('"');
                    if (text.Length < 2)
                        continue;
                    if (text[1].IndexOfAny(Path.GetInvalidPathChars()) != -1)
                        continue;
                    GetIncludePath(ref text[1], dir);
                    include.Add(text[1]);
                }
            }
            return include;
        }

        public static void GetIncludePath(ref string iPath, string dir)
        {
            string scrPath = Path.GetFullPath(Path.Combine(dir, iPath)); // исходная папка скрипта (высший приоритетет)
            string temp = scrPath;
            if (Settings.IsSearchIncludes && !Path.IsPathRooted(iPath) && !File.Exists(scrPath)) {
                temp = Path.GetFullPath(Path.Combine(Settings.pathHeadersFiles, iPath));
            }
            if (!File.Exists(temp)) {
                temp = Path.GetFullPath(Path.Combine(Settings.ProgramFolder, iPath)); // директория приложения (низший приоритет)
                if (!File.Exists(temp)) iPath = scrPath;
            }
            iPath = temp;
        }
        #endregion
    }
}
