using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ScriptEditor.CodeTranslation
{
    public enum ValueType : int { Int = 1, Float = 2, String = 3 }
    public enum VarType : int { Local = 1, Global = 2, Import = 3, Export = 4 }

    [Flags]
    public enum ProcType : int { None = 0, Timed = 0x01, Conditional = 0x02, Import = 0x04, Export = 0x08, Critical = 0x10, Pure = 0x20, Inline = 0x40 }
    public enum NameType { None, Macro, LVar, GVar, Proc }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcedureData
    {
        public int name;
        public ProcType type;
        public int time;
        private readonly int unused0; // larger union
        private readonly int unused1; // namelist
        public int args;

        // line num of the procedure definition (тоже самое что и d.start)
        // NOTE: может иметь значение -1 если отсутстует объявление процедуры в скрипте, в этом случае d.declared будет иметь значение d.start
        public int defined;
        private readonly int unused2;
        public int numVariables;
        private readonly int unused3;
        public int numRefs;
        private readonly int unused4;
        public int declared;          // Объявление (declaration)
        public IntPtr fdeclared;
        public int start;
        public IntPtr fstart;
        public int end;
        public IntPtr fend;

        public int Declaration {
            get { return (defined == -1) ? defined : declared; }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct VariableData
    {
        [FieldOffset(0)]
        public int name;
        [FieldOffset(8)]
        public int numRefs;
        [FieldOffset(12)]
        public ValueType initialType;
        [FieldOffset(16)]
        public int intValue;
        [FieldOffset(16)]
        public float floatValue;
        [FieldOffset(20)]
        public VarType type;
        [FieldOffset(24)]
        public int arrayLen;
        [FieldOffset(28)]
        public int declared;
        [FieldOffset(32)]
        public IntPtr fdeclared;
        [FieldOffset(40)]
        public int initialized;
    }

    public class ProgramInfo
    {
        public static readonly SortedDictionary<string, Macro> macrosGlobal = new SortedDictionary<string, Macro>();

        public /*readonly*/ Procedure[] procs;
        public readonly Variable[] vars;
        public static Dictionary<string, string> opcodes;
        public static SortedList<string, string> opcodes_list;
        private readonly Dictionary<string, Procedure> procLookup;
        private readonly Dictionary<string, Variable> varLookup;
        public readonly SortedDictionary<string, Macro> macros;

        public bool parsed = false;
        public bool parseData = false;   // Data Variables and Procedures received.
        public bool parseError = false;
        public bool reParseData = false; // true - обновляет информацию о процедурах и переменных для случаев когда работа внешенго парсера невозможна

        public IParserInfo Lookup(string _token, string file, int line)
        {
            string token = _token.ToLowerInvariant();
            if (file != null) {
                file = file.ToLowerInvariant();
                for (int i = 0; i < procs.Length; i++) {
                    if ((line == procs[i].d.declared || line >= procs[i].d.start && line <= procs[i].d.end)
                        && String.Compare(file, procs[i].fdeclared, true) == 0) {
                        foreach (Variable var in procs[i].variables) {
                            if (string.Compare(var.name, token, true) == 0)
                                return var;
                        }
                    }
                }
            }
            if (procLookup.ContainsKey(token))
                return procLookup[token];
            else if (varLookup.ContainsKey(token))
                return varLookup[token];

            return Macros(_token); // все макросы регистро зависимы, поэтому их поиск должен быть с учетом регистра
        }

        public Macro Macros(string _token) {
            Macro macro;

            if (!macros.TryGetValue(_token, out macro)) {
                macrosGlobal.TryGetValue(_token, out macro);
            }
            return macro;
        }

        public static string LookupOpcodesToken(string token)
        {
            token = token.ToLowerInvariant();
            if (opcodes_list.ContainsKey(token)) {
                return opcodes[opcodes_list[token]];
            } else {
                return null;
            }
        }

        public string LookupToken(string token, string file, int line, bool shortDesc = false)
        {
            IParserInfo pi = Lookup(token, file, line);
            if (pi == null)
                return null;
            else
                return shortDesc ? pi.ToString(true) : pi.ToString();
        }

        public NameType LookupTokenType(string token, string file, int line)
        {
            IParserInfo pi = Lookup(token, file, line);
            if (pi == null)
                return NameType.None;
            else
                return pi.Type();
        }

        public Reference[] LookupReferences(string token, string file, int line)
        {
            IParserInfo pi = Lookup(token, file, line);
            if (pi == null)
                return null;
            else
                return pi.References();
        }

        public void LookupDecleration(string token, string file, int line, out string ofile, out int oline)
        {
            IParserInfo pi = Lookup(token, file, line);
            if (pi == null) {
                ofile = null;
                oline = -1;
            } else
                pi.Deceleration(out ofile, out oline);
        }

        public void LookupDefinition(string token, out string ofile, out int oline)
        {
            token = token.ToLowerInvariant();
            oline = -1;
            ofile = null;
            if (procLookup.ContainsKey(token)) {
                ofile = procLookup[token].fstart;
                oline = procLookup[token].d.start;
            }
        }

        public void BuildDictionaries()
        {
            for (int i = 0; i < procs.Length; i++) {
                procLookup[procs[i].Name] = procs[i];
            }
            for (int i = 0; i < vars.Length; i++) {
                varLookup[vars[i].name.ToLowerInvariant()] = vars[i];
            }
        }

        public void RebuildProcedureDictionary()
        {
            procLookup.Clear();
            for (int i = 0; i < procs.Length; i++)
                procLookup.Add(procs[i].Name, procs[i]);
        }

        public ProgramInfo(int procs, int vars)
        {
            this.procs = new Procedure[procs];
            this.vars = new Variable[vars];
            procLookup = new Dictionary<string, Procedure>(procs);
            varLookup = new Dictionary<string, Variable>(vars);
            macros = new SortedDictionary<string, Macro>();
        }

        public bool ProcedureIsExist(string name)
        {
            return (GetProcedureIndex(name, procs) != -1);
        }

        public int GetProcedureIndex(string name)
        {
            return GetProcedureIndex(name, procs);
        }

        public int GetProcedureIndex(string name, Procedure[] proc)
        {
            for (int i = 0; i < proc.Length;  i++)
            {
                if (proc[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }

        public Procedure GetProcedureByIndex(int index, Procedure[] proc = null)
        {
            return (proc == null) ? procs[index] : proc[index];
        }

        public Procedure GetProcedureByName(string name)
        {
            var i = GetProcedureIndex(name);
            return (i != -1) ? GetProcedureByIndex(i) : null;
        }

        /// <summary>
        /// Получить процедуру по номеру строки из скрипта.
        /// </summary>
        /// <param name="lineNumber">Строка</param>
        /// <returns></returns>
        public Procedure GetProcedureFromPosition(int lineNumber)
        {
            lineNumber++;
            foreach (Procedure p in procs)
            {
                //int pLine = (p.d.defined != -1) ? p.d.defined: p.d.declared;
                if (lineNumber >= p.d.start & lineNumber <= p.d.end)
                    return p;
            }
            return null;
        }

        public Procedure GetNearProcedure(int lineNumber)
        {
            if (procs.Length == 0) return null;

            lineNumber++;
            List<KeyValuePair<int, Procedure>> values = new List<KeyValuePair<int, Procedure>>();

            foreach (Procedure p in procs)
            {
                values.Add(new KeyValuePair<int, Procedure>(p.d.start, p));
            }
            values.Sort((a, b) => { return a.Key.CompareTo(b.Key); });

            int index;
            for (index = 0; index < values.Count; index++)
            {
                if (lineNumber < values[index].Key) return values[index].Value; // возвращает следующую процедуру
            }
            return values[index - 1].Value; // возвращает самую последнию
        }

        /// <summary>
        /// Возвращает самую первую (верхнию) по расположению скрипта процедуру
        /// </summary>
        /// <returns></returns>
        public Procedure GetTopProcedure()
        {
            Procedure top = null;
            foreach (Procedure p in procs)
            {
                if (p.d.start == -1)
                    continue;
                if (top == null || p.d.start < top.d.start) top = p;
            }
            return top;
        }

        /// <summary>
        /// Проверить объявлено ли имя в коде скрипта
        /// </summary>
        /// <param name="name">Проверяемое имя</param>
        /// <param name="renType">Тип проверки</param>
        /// <returns>True - Имя в коде уже объявлено</returns>
        public bool CheckExistsName(string name, NameType renType, string file = null, int line = 0)
        {
            if (parseData) {
                NameType type = LookupTokenType(name, file, line);
                if (type == NameType.None)
                    return false;

                switch (renType)
                {
                    case NameType.Macro:
                    case NameType.GVar:
                    case NameType.Proc:
                        return (type != NameType.None && type != NameType.LVar);
                    case NameType.LVar:
                        return (type == NameType.LVar);
                    default :
                        return false;
                }
            } else
                return CheckExistsName(name);
        }

        // for unparsed data
        public bool CheckExistsName(string pName, bool checkMacros = true)
        {
            foreach (var p in procs)
                if (p.name.Equals(pName, StringComparison.OrdinalIgnoreCase))
                    return true; // found

            // check script macros
            return (checkMacros) ? macros.ContainsKey(pName) : false; // false - not found
        }

        public static void LoadOpcodes()
        {
            string[] lines;
            opcodes = new Dictionary<string, string>();
            opcodes_list = new SortedList<string, string>();
            try {
                lines = File.ReadAllLines(Path.Combine(Settings.DescriptionsFolder, (Settings.hintsLang == 0) ? "opcodes.txt" : "opcodes_rus.txt"));
            } catch (FileNotFoundException) {
                return;
            }
            foreach (string line in lines) {
                Match m = Regex.Match(line, @"^[\w\|]+\*?\s+(\w+).*");
                if (m.Success) {
                    // wrap words
                    string[] words = line.Split(' ');
                    string wrapped = "";
                    int lineLen = 0, wrap = 0;
                    foreach (String word in words) {
                        if ((lineLen + word.Length) > 150 || word == "|" || word == "||") {
                            wrapped += (word == "||") ? "\n\n" : "\n";
                            lineLen = 0;
                            if (word == "|" || word == "||") {
                                if (Settings.shortDesc && ++wrap > 1) {
                                    wrap = 0;
                                    break;
                                } else
                                    continue;
                            }
                        }
                        if (wrapped != "")
                            wrapped += " ";
                        wrapped += word;
                        lineLen += (word.Length + 1);
                    }
                    opcodes[m.Groups[1].ToString()] = wrapped;
                }
            }
            foreach (var entry in opcodes)
                opcodes_list.Add(entry.Key.ToLowerInvariant(), entry.Key);
        }

        public List<string> LookupAutosuggest(string part)
        {
            //FIXED: возникает ошибка "коллекция была изменена после создания экземпляра перечислителя"
            while (TextEditor.parserIsRunning)
                System.Threading.Thread.Sleep(50); //Avoid stomping on files while the parser is running

            List<string> matches = LookupOpcode(part);
            part = part.ToLowerInvariant();
            foreach (var entry in new Dictionary<string, Procedure>(procLookup)) {
                if (entry.Key.IndexOf(part) == 0) {
                    matches.Add(entry.Value.name + "|" + entry.Value.ToString(true) + "|P");
                }
            }

            foreach (var entry in new Dictionary<string, Variable>(varLookup)) {
                if (entry.Key.IndexOf(part) == 0) {
                    matches.Add(entry.Value.name + "|" + entry.Value.ToString() + "|V");
                }
            }

            SortedList<string, string> _macros = new SortedList<string, string>(StringComparer.Ordinal);
            foreach (var entry in new SortedDictionary<string, Macro>(macros)) {
                if (entry.Key.IndexOf(part, StringComparison.OrdinalIgnoreCase) == 0) {
                    AddMacrosList(_macros, entry);
                }
            }
            foreach (var entry in macrosGlobal) {
                if (entry.Key.IndexOf(part, StringComparison.OrdinalIgnoreCase) == 0) {
                    if (_macros.ContainsKey(entry.Value.defname))
                        continue;
                    AddMacrosList(_macros, entry);
                }
            }
            foreach (var entry in _macros)
                matches.Add(entry.Key + entry.Value);

            // remove dublicates
            for (int i = 0; i < matches.Count; i++)
            {
                string token = matches[i].Substring(0, matches[i].IndexOf('|'));
                for (int j = i + 1; j < matches.Count; j++) {
                    string check = matches[j].Substring(0, matches[j].IndexOf('|'));
                    if (check == token)
                        matches.RemoveAt(j--);
                }
            }
            return matches;
        }

        private static void AddMacrosList(SortedList<string, string> _macros, KeyValuePair<string, Macro> entry)
        {
            string def = (entry.Value.code.Length > 300) ? "No preview macros." : entry.Value.code;
            _macros.Add(entry.Value.defname, "|Define:\n" + def + "|M");
        }

        public static List<string> LookupOpcode(string part)
        {
            var matches = new List<string>();
            foreach (string key in opcodes_list.Keys) {
                if (key.IndexOf(part, StringComparison.OrdinalIgnoreCase) == 0) {
                    string value = opcodes_list[key];
                    matches.Add(value + "|" + opcodes[value]);
                }
            }
            return matches;
        }

        public bool ShortCircuitEvaluation  // это не может быть использовано при массовой компиляции
        {
            get {
                string token = "SFALL_SC_EVALUATION";
                if (macros.ContainsKey(token)) {
                    MacrosGetValue(ref token);
                    return Convert.ToBoolean(token);
                }
                return false;
            }
        }

        public int ScriptNameID
        {
            get {
                //int loop = 1;
                string token = "NAME";
            loop:
                if (macros.ContainsKey(token)) {
                    MacrosGetValue(ref token);
                    int result;
                    if (int.TryParse(token, out result))
                        return result;
                    else
                        goto loop;
                }
                // UNDONE: дополнительная проверка в нижнем регистре
                //if (--loop > 0) {
                //    token = token.ToLowerInvariant();
                //    goto loop;
                //}
                return -1;
            }
        }

        public void MacrosGetValue(ref string token) {
            token = macros[token].code.Trim('(', ')');
            int i = token.IndexOf(')');
            if (i > 0)
                token = token.Remove(i);
        }
    }
}
