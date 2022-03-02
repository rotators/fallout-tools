using System.Collections.Generic;
using System.IO;

using ScriptEditor.CodeTranslation;

namespace ScriptEditor.TextEditorUI.Function {

    public class OpcodeTemplate
    {
        public OpcodeType opcode;
        public string opcodeName;
        public int msgFileArg, msgArg, nodeArg, iqArg;
        public int totalArgs;
        public bool isDefault = true;

        public OpcodeTemplate(OpcodeType opcode, string name, int msg, int msgFile = 0, int node = 0, int iq = 0, int total = 1) {
            this.opcode = opcode;
            opcodeName  = name;
            msgArg      = msg - 1;
            msgFileArg  = msgFile - 1;
            nodeArg     = node - 1;
            iqArg       = iq - 1;
            totalArgs   = total;
        }
    }

    public class DialogFunctionsRules
    {
        const string format = ";Format: Type, Name, CountArgs, MsgLine, MsgFile, Node, IQ";
        static readonly string RulesFile = Settings.SettingsFolder + "\\DialogFunc.cfg";

        // vanilla opcodes config
        private static readonly OpcodeTemplate[] templates = new OpcodeTemplate[]
        {
            new OpcodeTemplate(OpcodeType.gsay_reply,   "gsay_reply",   msg:2, msgFile:1, total:2),
            new OpcodeTemplate(OpcodeType.gsay_message, "gsay_message", msg:2, msgFile:1, total:2),
            new OpcodeTemplate(OpcodeType.gsay_option,  "gsay_option",  msg:2, msgFile:1, node:3, total:4),
            new OpcodeTemplate(OpcodeType.giq_option,   "giq_option",   msg:3, msgFile:2, node:4, iq:1, total:5),
        };

        internal static Dictionary<string, OpcodeTemplate> opcodeTemplates = new Dictionary<string,OpcodeTemplate>();

        public static void BuildOpcodesDictionary()
        {
            if (opcodeTemplates.Count > 0) return;
            // загружаем шаблоны из файла
            if (File.Exists(RulesFile)) {
                string[] fileBuf = File.ReadAllLines(RulesFile);
                foreach (string item in fileBuf)
                {
                    string line = item.TrimStart();
                    if (line.Length == 0 || line[0] == ';') continue;
                    string[] args = line.Split(',');

                    OpcodeType type = DialogueParser.GetOpcodeType(args[0]);
                    if (type == OpcodeType.None) continue;

                    var template = new OpcodeTemplate(type, args[1].Trim(),
                                        int.Parse(args[3]), // message
                                        int.Parse(args[4]), // file
                                        int.Parse(args[5]), // node
                                        int.Parse(args[6]), // iq
                                        int.Parse(args[2])  // total func args
                    );
                    template.isDefault = false;
                    opcodeTemplates.Add(template.opcodeName, template);
                }
            }
            foreach (var item in templates)
            {
                opcodeTemplates.Add(item.opcodeName, item);
            }
        }

        public static void SaveTemplates()
        {
            List<string> templates = new List<string>() { format };
            foreach (var item in opcodeTemplates)
            {
                if (item.Value.isDefault) continue;
                string line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}",
                        item.Value.opcode.ToString(),
                        item.Value.opcodeName,
                        item.Value.totalArgs,
                        item.Value.msgArg + 1,
                        item.Value.msgFileArg + 1,
                        item.Value.nodeArg + 1,
                        item.Value.iqArg + 1
                    );
                templates.Add(line);
            }
            File.WriteAllLines(RulesFile, templates);
        }
    }
}
