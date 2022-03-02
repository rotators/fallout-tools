using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ScriptEditor.CodeTranslation;

namespace ScriptEditor.TextEditorUI.Function {

    public partial class FunctionsRules : Form
    {
        public FunctionsRules()
        {
            InitializeComponent();

            if (Settings.hintsLang != 0) label.Text = label.Tag.ToString();

            foreach (var item in DialogFunctionsRules.opcodeTemplates)
            {
                var v = item.Value;
                if (v.isDefault) continue;

                dgvTemplates.Rows.Add(v.opcode.ToString(), v.opcodeName, v.totalArgs, v.msgArg + 1, v.nodeArg + 1, v.iqArg + 1, v.msgFileArg + 1);
                dgvTemplates.Rows[dgvTemplates.Rows.Count - 2].Tag = item;
            }
        }

        private void ChangeToTemplate(DataGridViewRow row, string name) {
            KeyValuePair<string, OpcodeTemplate> pair = (KeyValuePair<string, OpcodeTemplate>)row.Tag;

            string key = pair.Key;
            if (key != name) {
                KeyValuePair<string, OpcodeTemplate> p = new KeyValuePair<string, OpcodeTemplate>(name, pair.Value);
                pair = p;
                DialogFunctionsRules.opcodeTemplates.Remove(key);
                DialogFunctionsRules.opcodeTemplates.Add(p.Key, p.Value);
            }

            pair.Value.opcode = DialogueParser.GetOpcodeType(row.Cells[0].Value.ToString());
            pair.Value.opcodeName = row.Cells[1].Value.ToString();
            pair.Value.totalArgs = Convert.ToInt32(row.Cells[2].Value);
            pair.Value.msgArg = Convert.ToInt32(row.Cells[3].Value) - 1;
            pair.Value.nodeArg = Convert.ToInt32(row.Cells[4].Value) - 1;
            pair.Value.iqArg = Convert.ToInt32(row.Cells[5].Value) - 1;
            pair.Value.msgFileArg = Convert.ToInt32(row.Cells[6].Value) - 1;
        }

        private void AddToTemplate(DataGridViewRow row, string key) {
            OpcodeTemplate template = new OpcodeTemplate(
                    DialogueParser.GetOpcodeType(row.Cells[0].Value.ToString()),
                    row.Cells[1].Value.ToString(),
                    int.Parse(row.Cells[3].Value.ToString()),
                    int.Parse(row.Cells[6].Value.ToString()),
                    int.Parse(row.Cells[4].Value.ToString()),
                    int.Parse(row.Cells[5].Value.ToString()),
                    int.Parse(row.Cells[2].Value.ToString())
                );
            template.isDefault = false;

            if (DialogFunctionsRules.opcodeTemplates.ContainsKey(key))
                DialogFunctionsRules.opcodeTemplates[key] = template;
            else
                DialogFunctionsRules.opcodeTemplates.Add(key, template);
        }

        private void RulesDialog_FormClosing(object sender, FormClosingEventArgs e) {
            bool needSave = false;
            int last = dgvTemplates.Rows.Count -1;

            List<string> keyList = new List<string>();

            foreach (DataGridViewRow row in dgvTemplates.Rows)
            {
                if (row.Index == last) break;
                if (row.Cells[0].Value == null || row.Cells[1].Value == null) continue;

                for (int i = 2; i < row.Cells.Count; i++) {
                    if (row.Cells[i].Value == null) row.Cells[i].Value = 0;
                }

                string key = row.Cells[1].Value.ToString().ToLowerInvariant();
                keyList.Add(key);

                if (row.Tag == null) {
                    AddToTemplate(row, key);
                    needSave = true;
                    continue;
                }

                KeyValuePair<string, OpcodeTemplate> pair = (KeyValuePair<string, OpcodeTemplate>)row.Tag;
                if (row.Cells[0].Value.ToString() != pair.Value.opcode.ToString()
                    || key != pair.Key
                    || Convert.ToInt32(row.Cells[2].Value) != pair.Value.totalArgs
                    || Convert.ToInt32(row.Cells[3].Value) != pair.Value.msgArg + 1
                    || Convert.ToInt32(row.Cells[4].Value) != pair.Value.nodeArg + 1
                    || Convert.ToInt32(row.Cells[5].Value) != pair.Value.iqArg + 1
                    || Convert.ToInt32(row.Cells[6].Value) != pair.Value.msgFileArg + 1)
                {
                    ChangeToTemplate(row, key);
                    needSave = true;
                }
            }
            // remove
            List<string> removeList = new List<string>();
            foreach (var item in DialogFunctionsRules.opcodeTemplates) {
                if (item.Value.isDefault == false && !keyList.Exists(key => (key == item.Key))) removeList.Add(item.Key);
            }
            foreach (var key in removeList) {
                DialogFunctionsRules.opcodeTemplates.Remove(key);
            }
            // save to file
            if (needSave || removeList.Count > 0) {
                DialogFunctionsRules.SaveTemplates();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            if (dgvTemplates.SelectedRows.Count == 0 || dgvTemplates.CurrentRow.Index == dgvTemplates.Rows.Count - 1) return;
            dgvTemplates.Rows.Remove(dgvTemplates.CurrentRow);

        }

        private readonly string[] helpDesc =
        {
            "Args:\tThe total number of arguments used in the function.\n" +
            "Message:\tArgument number of the function that specifies the number of the text message.\n" +
            "Node:\tNumber of the argument of the function that specifies the node procedure. Set the value to 0 if the argument is not used.\n" +
            "IQ:\tThe argument number of the function that specifies the IQ parameter. Set the value to 0 if the argument is not used.\n" +
            "File:\tArgument number of the function that specifies the number of the message file. Set the value to 0 if the argument is not used.\n\n" +
            "Example of setting parameters for a macro: NOption(Message, Node, IQ);\n" +
            "Args=3, Message=1, Node=2, IQ=3, File=0",

            "Args:\tОбщее число аргументов используемых в функции.\n" +
            "Message:\tПозиция, указывающая на аргумент функции с номером текстового сообщения.\n" +
            "Node:\tПозиция, указывающая на аргумент функции к процедуре перехода. Значение 0, аргумент не используется.\n" +
            "IQ:\tПозиция, указывающая на аргумент функции с IQ. Значение 0, если аргумент не используется.\n" +
            "File:\tПозиция, указывающая на аргумент с номером файла сообщения. Значение 0, если агрумент не используется.\n\n" +
            "Пример настройки параметров для марокса: NOption(Message, Node, IQ);\n" +
            "Args=3, Message=1, Node=2, IQ=3, File=0"
        };

        private void bHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(helpDesc[Settings.hintsLang], "Help");
        }
    }
}
