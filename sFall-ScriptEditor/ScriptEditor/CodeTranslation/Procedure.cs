using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEditor.CodeTranslation
{
    public class Procedure : IParserInfo
    {
        private string nameLCase;

        // регистро-зависимое имя процедуры
        public string name;
        public string fdeclared;
        public string fstart;
        //public string fend; //just assume this is the same file as fstart
        public string filename;

        public ProcedureData d;
        public Variable[] variables;
        public Reference[] references;

        int isStandart = -1;
        int isLocal = -1;

        /// <summary>
        /// Имя процедуры в нижнем регистре
        /// </summary>
        public string Name
        {
            get {
                if (nameLCase == null) nameLCase = name.ToLowerInvariant();
                return nameLCase;
            }
        }

        public NameType Type() { return NameType.Proc; }
        public Reference[] References() { return references; }
        public void Deceleration(out string file, out int line)
        {
            file = fdeclared;
            line = d.declared;
        }

        public string ToString(bool fullSpec)
        {
            string s = string.Empty;
            if (fullSpec) {
                if ((d.type & ProcType.Import) != ProcType.None)
                    s += "imported ";
                if ((d.type & ProcType.Export) != ProcType.None)
                    s += "exported ";
                if ((d.type & ProcType.Pure) != ProcType.None)
                    s += "pure ";
                if ((d.type & ProcType.Inline) != ProcType.None)
                    s += "inline ";
                if ((d.type & ProcType.Timed) != ProcType.None)
                    s += "timed ";
                if ((d.type & ProcType.Conditional) != ProcType.None)
                    s += "conditional ";
                if ((d.type & ProcType.Critical) != ProcType.None)
                    s += "critical ";
                s += "procedure ";
            }
            s += name;
            string args = " (";
            for (int i = 0; i < d.args; i++) {
                if (i > 0)
                    args += ", ";
                args += (i >= variables.Length) ? "x" : variables[i].name;
                if (fullSpec && i < variables.Length && variables[i].initialValue != null)
                    args += " := " + variables[i].initialValue;
            }
            args += ")";
            if (fullSpec || args.Length > 2)
                s += args;

            #if DEBUG
                if (fullSpec)
                    s += String.Format(" [start:{0} end:{1} decl:{2}]", d.start, d.end, d.declared);
            #endif

            return s;
        }

        public bool IsImported
        {
            get { return (d.type & ProcType.Import) > 0; }
        }

        public bool IsExported
        {
            get {return (d.type & ProcType.Export) > 0; }
        }

        public bool IsStandart()
        {
            if (isStandart != -1) return (isStandart == 1);
            if (name != null) {
                switch (Name) {
                case "combat_is_over_p_proc" :
                case "combat_is_starting_p_proc" :
                case "combat_p_proc" :
                case "critter_p_proc" :
                case "damage_p_proc" :
                case "desc_p_proc" : // Fallout 1
                case "description_p_proc" :
                case "destroy_p_proc" :
                case "drop_p_proc" :
                case "is_dropping_p_proc" :
                case "look_at_p_proc" :
                case "map_enter_p_proc" :
                case "map_exit_p_proc" :
                case "map_update_p_proc" :
                case "pickup_p_proc" :
                case "push_p_proc" :
                case "spatial_p_proc" :
                case "start" :
                case "talk_p_proc" :
                case "timed_event_p_proc" :
                case "use_obj_on_p_proc" :
                case "use_p_proc" :
                case "use_skill_on_p_proc" :
                    isStandart = 1;
                    return true;
                default:
                    isStandart = 0;
                    break;
                }
            }
            return false;
        }

        public bool IsLocal(string script)
        {
            if (isLocal != -1) return (isLocal == 1);
            isLocal = Convert.ToInt32(fstart.ToLowerInvariant() == script.ToLowerInvariant());
            return (isLocal == 1);
        }

        public override string ToString()
        {
            return ToString(true);
        }
    }
}
