using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ScriptEditor.CodeTranslation
{
    public class Macro : IParserInfo
    {
        public readonly string token;
        public readonly string defname;
        public readonly string code;
        public readonly int declared;
        public readonly string fdeclared;
        public readonly string desc;

        public string toString = null;

        public NameType Type() { return NameType.Macro; }
        public Reference[] References() { return null; }

        public void Deceleration(out string file, out int line)
        {
            file = fdeclared;
            line = declared;
        }

        public Macro(string token, string defname, string code, string file, int line, string desc)
        {
            this.defname = defname;
            this.code = code;
            this.fdeclared = file;
            this.declared = line;
            this.token = token;
            this.desc = desc;
        }

        public override string ToString()
        {
            if (toString == null) {
                string declare = fdeclared.Substring(fdeclared.LastIndexOf('\\') + 1);
                if (declare == "parser.ssl")
                    declare = string.Empty;
                else
                    declare = "\n\nDeclare file: " + declare;

                toString = "Define: " + defname + ((desc != null) ? "\n - " + desc.TrimEnd() + "\n\n" : "\n") + code + declare;
            }
            return toString;
        }

        public string ToString(bool a)
        {
            return "Define: " + defname;
        }

        public bool IsImported { get { return false; } }

        public bool IsExported { get { return false; } }

        public bool IsStandart() { return false; }
    }
}
