using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Runtime.ExceptionServices;

namespace ScriptEditor.CodeTranslation
{
    /// <summary>
    /// Class for parsing SSL code. Interacts with SSLC compiler via DLL imports.
    /// </summary>
    internal class ParserExternal
    {
        #region Imports from SSLC DLL

        [DllImport("resources\\parser.dll")]
        private static extern int parse_main(string file, string orig, string dir, string def, string includePath, Int32 backMode);
        [DllImport("resources\\parser.dll")]
        private static extern int numProcs();
        [DllImport("resources\\parser.dll")]
        private static extern void getProc(int proc, out ProcedureData data);
        [DllImport("resources\\parser.dll")]
        private static extern int getProcNamespaceSize(int proc);
        [DllImport("resources\\parser.dll")]
        private static extern void getProcNamespace(int proc, byte[] names);
        [DllImport("resources\\parser.dll")]
        private static extern int numVars();
        [DllImport("resources\\parser.dll")]
        private static extern void getVar(int var, out VariableData data);
        //[DllImport("resources\\parser.dll")]
        //private static extern int numExternals();
        //[DllImport("resources\\parser.dll")]
        //private static extern void getExternal(int var, out VariableData data);
        [DllImport("resources\\parser.dll")]
        private static extern void getProcVar(int proc, int var, out VariableData data);
        [DllImport("resources\\parser.dll")]
        private static extern int namespaceSize();
        [DllImport("resources\\parser.dll")]
        private static extern void getNamespace(byte[] names);
        [DllImport("resources\\parser.dll")]
        private static extern int stringspaceSize();
        [DllImport("resources\\parser.dll")]
        private static extern void getStringspace(byte[] names);
        [DllImport("resources\\parser.dll")]
        private static extern void getProcRefs(int proc, int[] refs);
        [DllImport("resources\\parser.dll")]
        private static extern void getVarRefs(int var, int[] refs);
        [DllImport("resources\\parser.dll")]
        private static extern void getProcVarRefs(int proc, int var, int[] refs);

        #endregion

        private static readonly string parserPath = Path.Combine(Settings.scriptTempPath, "parser.ssl");

        private bool firstParse;
        private int lastStatus = 1;

        public int LastStatus
        {
            get { return lastStatus; }
        }

        public ParserExternal(bool firstPass)
        {
            this.firstParse = firstPass;
        }

        [HandleProcessCorruptedStateExceptions]
        public ProgramInfo Parse(string text, string filepath, ProgramInfo prev_pi)
        {
            // Parse disabled, get only macros
            if (Settings.enableParser && filepath != null) {
                File.WriteAllText(parserPath, text, Encoding.Default);
                try {
                    lastStatus = parse_main(parserPath, filepath, Path.GetDirectoryName(filepath), Settings.preprocDef,
                                           (Settings.IsSearchIncludes) ? Settings.pathHeadersFiles : null, Settings.compileBackwardMode);
                } catch {
                    lastStatus = 3;
                    MessageBox.Show("An unexpected error occurred while parsing text of the script.\n" +
                                    "It is recommended that you save all unsaved documents and restart application,\n" +
                                    "in order to avoid further incorrect operation of the application.", "Error: Parser.dll");
                };
            }

            ProgramInfo pi = (lastStatus >= 1)
                              ? prev_pi //new ProgramInfo(0, 0)
                              : new ProgramInfo(numProcs(), numVars());

            if (lastStatus >= 1 && prev_pi != null) { // preprocess error - store previous data Procs/Vars
                if (prev_pi.parsed) //.parseData
                    if (Settings.enableParser) pi = (!pi.reParseData) ? ParserInternal.UpdateProcsPI(prev_pi, text, filepath) : ParserInternal.UpdatePI(prev_pi, filepath);
                else if (firstParse) {
                    pi.RebuildProcedureDictionary();
                }
                if (!pi.reParseData) pi.macros.Clear();
            }
            pi.parseError = (lastStatus != 0 & Settings.enableParser);
            //
            // Macros
            //
            string[] scriptCode = text.Split('\n');
            if (!pi.reParseData) new GetMacros(scriptCode, filepath, Path.GetDirectoryName(filepath), pi.macros);

            pi.parsed = true;
            pi.reParseData = false;
            if (lastStatus >= 1) return pi; // parse failed, return macros and previous parsed data Procs/Vars
            //
            // Getting data of variables/procedures
            //
            pi.parseData = true; // received data from parser.dll
            byte[] names = new byte[namespaceSize()];
            int stringsSize = stringspaceSize();
            getNamespace(names);
            byte[] strings = null;
            if (stringsSize > 0) {
                strings = new byte[stringsSize];
                getStringspace(strings);
            }
            // Variables
            for (int i = 0; i < pi.vars.Length; i++) {
                pi.vars[i] = new Variable();
                getVar(i, out pi.vars[i].d);
                pi.vars[i].name = ParseName(names, pi.vars[i].d.name);
                if (pi.vars[i].d.initialized != 0) {
                    switch (pi.vars[i].d.initialType) {
                        case ValueType.Int:
                            pi.vars[i].initialValue = pi.vars[i].d.intValue.ToString();
                            break;
                        case ValueType.Float:
                            pi.vars[i].initialValue = pi.vars[i].d.floatValue.ToString();
                            break;
                        case ValueType.String:
                            pi.vars[i].initialValue = '"' + ParseName(strings, pi.vars[i].d.intValue) + '"';
                            break;
                    }
                }
                if (pi.vars[i].d.fdeclared != IntPtr.Zero) {
                    pi.vars[i].fdeclared = Path.GetFullPath(Marshal.PtrToStringAnsi(pi.vars[i].d.fdeclared));
                    pi.vars[i].filename = Path.GetFileName(pi.vars[i].fdeclared).ToLowerInvariant();
                }
                if (pi.vars[i].d.numRefs == 0) {
                    pi.vars[i].references = new Reference[0];
                } else {
                    int[] tmp = new int[pi.vars[i].d.numRefs * 2];
                    getVarRefs(i, tmp);
                    pi.vars[i].references = new Reference[pi.vars[i].d.numRefs];
                    for (int j = 0; j < pi.vars[i].d.numRefs; j++)
                        pi.vars[i].references[j] = Reference.FromPtr(tmp[j * 2], tmp[j * 2 + 1]);
                }
            }
            // Procedures
            for (int i = 0; i < pi.procs.Length; i++) {
                pi.procs[i] = new Procedure();
                getProc(i, out pi.procs[i].d);
                pi.procs[i].name = ParseName(names, pi.procs[i].d.name);
                if (pi.procs[i].d.fdeclared != IntPtr.Zero) {
                    //pi.procs[i].fdeclared=Marshal.PtrToStringAnsi(pi.procs[i].d.fdeclared);
                    pi.procs[i].fdeclared = Path.GetFullPath(Marshal.PtrToStringAnsi(pi.procs[i].d.fdeclared));
                    pi.procs[i].filename = Path.GetFileName(pi.procs[i].fdeclared).ToLowerInvariant();
                }
                if (pi.procs[i].d.fstart != IntPtr.Zero) {
                    //pi.procs[i].fstart = Marshal.PtrToStringAnsi(pi.procs[i].d.fstart);
                    pi.procs[i].fstart = Path.GetFullPath(Marshal.PtrToStringAnsi(pi.procs[i].d.fstart));
                }
                //pi.procs[i].fend=Marshal.PtrToStringAnsi(pi.procs[i].d.fend);
                if (pi.procs[i].d.numRefs == 0) {
                    pi.procs[i].references = new Reference[0];
                } else {
                    int[] tmp = new int[pi.procs[i].d.numRefs * 2];
                    getProcRefs(i, tmp);
                    pi.procs[i].references = new Reference[pi.procs[i].d.numRefs];
                    for (int j = 0; j < pi.procs[i].d.numRefs; j++)
                        pi.procs[i].references[j] = Reference.FromPtr(tmp[j * 2], tmp[j * 2 + 1]);
                }
                // Procedure variables
                if (getProcNamespaceSize(i) == -1) {
                    pi.procs[i].variables = new Variable[0];
                } else {
                    byte[] procnames = new byte[getProcNamespaceSize(i)];
                    getProcNamespace(i, procnames);
                    pi.procs[i].variables = new Variable[pi.procs[i].d.numVariables];
                    for (int j = 0; j < pi.procs[i].variables.Length; j++) {
                        Variable var = pi.procs[i].variables[j] = new Variable();
                        getProcVar(i, j, out var.d);
                        var.name = ParseName(procnames, var.d.name);
                        if (var.d.initialized != 0) {
                            switch (var.d.initialType) {
                                case ValueType.Int:
                                    var.initialValue = var.d.intValue.ToString();
                                    break;
                                case ValueType.Float:
                                    var.initialValue = var.d.floatValue.ToString();
                                    break;
                                case ValueType.String:
                                    var.initialValue = '"' + ParseName(strings, var.d.intValue) + '"';
                                    break;
                            }
                        }
                        var.fdeclared = Marshal.PtrToStringAnsi(var.d.fdeclared);
                        if (var.d.numRefs == 0) {
                            var.references = new Reference[0];
                        } else {
                            int[] tmp = new int[var.d.numRefs * 2];
                            getProcVarRefs(i, j, tmp);
                            var.references = new Reference[var.d.numRefs];
                            for (int k = 0; k < var.d.numRefs; k++)
                                var.references[k] = Reference.FromPtr(tmp[k * 2], tmp[k * 2 + 1]);
                        }
                        var.adeclared = ParseProcedureArguments(pi.procs[i].d.start, var.d.declared,
                                                                var.name.ToLowerInvariant(), scriptCode);
                    }
                }
            }
            pi.BuildDictionaries();
            return pi;
        }

        private string ParseName(byte[] namelist, int name)
        {
            int strlen = (namelist[name - 5] << 8) + namelist[name - 6];
            return Encoding.ASCII.GetString(namelist, name - 4, strlen).TrimEnd('\0');
        }

        private int ParseProcedureArguments(int start, int end, string vName, string[] code)
        {
            if (start > code.Length) return -1;

            int len = ParserInternal.VARIABLE.Length + vName.Length;
            for (int i = start - 1; i > end; i--)
            {
                string line = code[i].TrimStart().ToLowerInvariant();
                if (len > line.Length)
                    continue;

                int y = line.IndexOf(ParserInternal.VARIABLE);
                if (y == -1)
                    continue;
                line = ParserInternal.RemoveDoubleWhiteSpaces(line, y, 0);

         TryPass:
                int x = line.IndexOf(ParserInternal.VARIABLE + vName, y);
                if (x > -1) {
                    char c = line[x + len];
                    if (c == '_' || Char.IsLetterOrDigit(c)) {
                        line = line.Remove(x, len);
                        goto TryPass;
                    }

                    int z = line.IndexOf(ParserInternal.BEGIN);
                    if (z > -1 && x > z)
                        break; // переменная находится за пределами begin

                    return i + 1;
                }
                if (line.StartsWith(ParserInternal.PROCEDURE))
                     break; // найдена процедура, прерываем цикл
            }
            return -1;
        }
    }
}
