using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Text;

using ScriptEditor.TextEditorUtilities;
using ICSharpCode.TextEditor.Util;

namespace ScriptEditor
{
    public partial class RegisterScript : Form
    {
        private static TextEditor TE;
        private Encoding lstEncoding;

        private class Entry
        {
            public int row;
            public string script;
            public string desc;
            public string name;
            public int vars;
            public string msglip;

            public Entry(int row, string line)
            {
                this.row = row;
                int descPos = line.IndexOf(';');
                int varPos  = line.LastIndexOf("# local_vars=");
                script = line.Remove(descPos).Trim();
                descPos++;
                desc = line.Substring(descPos, varPos - descPos).Trim();
                int.TryParse(line.Substring(varPos + 13), out vars);
            }

            public string GetAsString()
            {
                return script.PadRight(16) + "; " + desc.PadRight(45) + " # local_vars=" + vars.ToString();
            }

            public string GetMsgAsString()
            {
                if (name != null && Settings.EncCodePage.CodePage == 866)
                    name = name.Replace('\u0425', '\u0058'); //Replacement of Russian letter "X", to English letter
                return ("{" + (row + 101) + "}{" + msglip + "}{" + name + "}").PadRight(60) + "# " + script.PadRight(16) + "; " + desc;
            }
        }

        private struct cell
        {
            public int row;
            public int col;
        }

        cell SelectLine = new cell();

        private readonly List<string> lines;
        private readonly List<string> linesMsg;

        private readonly string lstPath;
        private readonly string msgPath;
        private readonly string headerPath;
        private bool cancel = false;
        private bool doAdd = false;
        private int scriptNumb = -1;
        private bool returnLine;
        private bool notSaved;

        private bool NotSaved
        {
            get { return notSaved; }
            set {
                Save_button.Text = "Saved";
                notSaved = value;
                if (value)
                    Save_button.Image = imageList1.Images[1];
                else
                    Save_button.Image = imageList1.Images[0];
            }
        }

        const string DESCMSG = "#\r\n#   This file was built using Sfall Script Editor.\r\n#";
        const string SCRIPT_H = "SCRIPT_";

        private void AddRow(Entry e)
        {
            dgvScripts.Rows.Add(e, e.row + 1, e.script, e.desc, e.vars, e.name);
            dgvScripts.Rows[dgvScripts.Rows.Count - 1].Cells[1].ToolTipText = "Unpack script from .dat";
        }

        private RegisterScript(string script, string lst, string msg, string header)
        {
            lstPath = lst;
            msgPath = msg;
            headerPath = header;
            InitializeComponent();

            dgvScripts.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            char[] space = new char[] { ' ' };

            // определяем кодировку
            StreamReader fsr = FileReader.OpenStream(File.OpenRead(lst), Encoding.Default);
            lstEncoding = fsr.CurrentEncoding;
            fsr.Close();

            lines = new List<string>(File.ReadAllLines(lst, lstEncoding));

            if (script != null) {
                DefinetextBox.Text = SCRIPT_H + Path.GetFileNameWithoutExtension(script).ToUpper();
                for (int i = 0; i < lines.Count; i++)
                {
                    if (string.Compare(lines[i].Split(space, StringSplitOptions.RemoveEmptyEntries)[0], script, true) == 0) {
                        scriptNumb = i;
                        break;
                    }
                }   // new reg script
                if (scriptNumb == -1) {
                    scriptNumb = lines.Count;
                    lines.Add(script + "; # local_vars=0");
                    doAdd = true;
                    AllowCheckBox.Checked = Settings.allowDefine;
                    Save_button.Image = imageList1.Images[1];
                    notSaved = true;
                }
                AllowCheckBox.Enabled = true;
                DefinetextBox.Enabled = true;
            } else
                NotSaved = false;

            Entry[] entries = new Entry[lines.Count];
            for (int i = 0; i < lines.Count; i++)
                entries[i] = new Entry(i, lines[i]);
            lines.Clear();
            if (msgPath != null) {
                linesMsg = new List<string>(File.ReadAllLines(msg, Settings.EncCodePage));
                for (int i = 0; i < linesMsg.Count; i++)
                {
                    string[] line = linesMsg[i].Split('}');
                    if (line.Length != 4)
                        continue;
                    line[0] = line[0].TrimStart(' ', '{');
                    int lineno;
                    if (!int.TryParse(line[0], out lineno)) continue;
                    lineno -= 101;
                    if (lineno >= entries.Length) continue;
                    entries[lineno].name = line[2].TrimStart(' ', '{');
                    entries[lineno].msglip = line[1].TrimStart(' ', '{');
                }
                linesMsg.Clear();
            }
            for (int i = 0; i < entries.Length; i++)
            {
                AddRow(entries[i]);
            }
            if (scriptNumb != -1) {
                dgvScripts.FirstDisplayedScrollingRowIndex = (scriptNumb <= 5) ? scriptNumb : scriptNumb - 5;
                dgvScripts.Rows[scriptNumb].Selected = true;
            }
            dgvScripts.CellValueChanged += dgvScripts_CellValueChanged;
        }

        public static void Registration(string script)
        {
            if (Settings.outputDir == null) {
                MessageBox.Show("No output path selected.", "Error");
                return;
            }
            string lstPath = Path.Combine(Settings.outputDir, "scripts.lst");
            if (!File.Exists(lstPath)) {
                MessageBox.Show("scripts.lst does not exist in your scripts output directory", "Error");
                return;
            }
            string msgPath = Path.Combine(Settings.outputDir, "../text/english/game/scrname.msg");
            if (!File.Exists(msgPath)) {
                if (Settings.showWarnings) MessageBox.Show("Could not find file scrname.msg", "Warning");
                msgPath = null;
            }
            string scriptH = null;
            if (Settings.pathScriptsHFile == null) {
                if (Settings.showWarnings) MessageBox.Show("The path to scripts.h has not been set.", "Warning");
            } else {
                scriptH = Settings.pathScriptsHFile;
                if (!File.Exists(scriptH)) scriptH = null;
            }
            // Show form
            TE = (TextEditor)ActiveForm;
            TE.RegistredScriptDialogShow = true;
            (new RegisterScript(script, lstPath, msgPath, scriptH)).Show(ActiveForm);
        }

        private void RegisterScript_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cancel) return;
            if (NotSaved) {
                if (MessageBox.Show("Save all changed to files?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    Save_button_Click(null, null);
                }
            }
            UndatFile.selectDatFile = null;
            TE.RegistredScriptDialogShow = false;
        }

        private void Save_button_Click(object sender, EventArgs e)
        {
            if (!NotSaved) return;
            dgvScripts.EndEdit();
            Entry[] entries = new Entry[dgvScripts.Rows.Count];
            for (int i = 0; i < entries.Length; i++)
            {
                int index = (int)dgvScripts.Rows[i].Cells[1].Value;
                entries[index - 1] = (Entry)dgvScripts.Rows[i].Cells[0].Value;
            }

            foreach (Entry entry in entries)
                lines.Add(entry.GetAsString());

            File.WriteAllLines(lstPath, lines.ToArray(), (FileReader.IsUnicode(lstEncoding)) ? new UTF8Encoding(false) : lstEncoding);
            lines.Clear();

            if (msgPath != null) {
                linesMsg.Add(DESCMSG);
                foreach (Entry entry in entries)
                    linesMsg.Add(entry.GetMsgAsString());

                File.WriteAllLines(msgPath, linesMsg.ToArray(), Settings.EncCodePage);
                linesMsg.Clear();
            }

            NotSaved = false;

            if (AllowCheckBox.Checked && headerPath == null) {
                MessageBox.Show("The definition was not added in header file.\nCould not find file scripts.h", "Script header error");
                return;
            }
            if (doAdd && AllowCheckBox.Checked) {
                Entry entry = entries[scriptNumb];
                List<string> hlines = new List<string>(File.ReadAllLines(headerPath));
                for (int j = hlines.Count - 1; j >= 0; j--)
                {
                    if (hlines[j].IndexOf('#') != -1 || j == 0) {
                        hlines.Insert(j, ("#define " + DefinetextBox.Text.ToUpperInvariant()).PadRight(32) + ("(" + (entry.row + 1) + ")").PadRight(8) + "// " + entry.script.PadRight(16) + "; " + entry.desc);
                        break;
                    }
                }
                File.WriteAllLines(headerPath, hlines.ToArray());
                doAdd = false;
            }
        }

        private void RegisterScript_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) {
                cancel = true;
                e.Handled = true;
                Close();
            }
        }

        private void dgvScripts_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1 || returnLine)
                return;
            DataGridViewCell cell = dgvScripts.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Entry entry = (Entry)dgvScripts.Rows[e.RowIndex].Cells[0].Value;
            string val = (string)cell.Value;
            if (val == null) val = "";
            switch (e.ColumnIndex) {
                case 2:
                    if (Path.GetFileNameWithoutExtension(val).Length > 8) {
                        MessageBox.Show("Script file names must be 8 characters.", "Name Error");
                        returnLine = true;
                        cell.Value = entry.script;
                        break;
                    }
                    entry.script = val;
                    break;
                case 3:
                    entry.desc = val;
                    break;
                case 4:
                    int.TryParse(val, out entry.vars);
                    break;
                case 5:
                    if (val.IndexOfAny(new char[] { '{', '}' }) == -1)
                        entry.name = val;
                    break;
            }
            if (!returnLine) NotSaved = true;
            returnLine = false;
        }

        private cell Finds(int rowStart, int colStart, int rev = 1)
        {
            cell cell = new cell();
            string find_str = FindtextBox.Text.Trim();
            if (find_str.Length == 0) return cell;
            if (rev == -1 && rowStart == 0) rowStart = dgvScripts.RowCount - 1;
            for (int row = rowStart; row < dgvScripts.RowCount; row += rev)
            {
                if (row < 0) break;
                for (int col = colStart; col < dgvScripts.ColumnCount; col++)
                {
                    if (dgvScripts.Rows[row].Cells[col].Value == null)
                        continue;
                    string value = dgvScripts.Rows[row].Cells[col].Value.ToString();
                    if (value.IndexOf(find_str, 0, StringComparison.OrdinalIgnoreCase) != -1) {
                        cell.row = row;
                        cell.col = col;
                        break;
                    }
                }
                if (cell.col != 0) break;
                colStart = 1;
            }
            if (cell.col != 0) {
                dgvScripts.FirstDisplayedScrollingRowIndex = (cell.row <= 5) ? cell.row : cell.row - 5;
                dgvScripts.Rows[cell.row].Cells[cell.col].Selected = true;
            }
            return cell;
        }

        private void Addbutton_Click(object sender, EventArgs e)
        {
            dgvScripts.Sort(dgvScripts.Columns[1], System.ComponentModel.ListSortDirection.Ascending);
            Entry[] entries = new Entry[1];
            entries[0] = new Entry(dgvScripts.RowCount, "none.int; # local_vars=");
            AddRow(entries[0]);
            dgvScripts.FirstDisplayedScrollingRowIndex = dgvScripts.RowCount - 1;
            NotSaved = true;
        }

        private void Delbutton_Click(object sender, EventArgs e)
        {
            dgvScripts.Sort(dgvScripts.Columns[1], System.ComponentModel.ListSortDirection.Ascending);
            dgvScripts.Rows.RemoveAt(dgvScripts.RowCount - 1);
            dgvScripts.FirstDisplayedScrollingRowIndex = dgvScripts.RowCount - 1;
            NotSaved = true;
        }

        private void Downbutton_Click(object sender, EventArgs e)
        {
            cell curfind = Finds(SelectLine.row, SelectLine.col + 1);
            if (curfind.col == 0) MessageBox.Show("Nothing found.");
            else SelectLine = curfind;
        }

        private void Upbutton_Click(object sender, EventArgs e)
        {
            cell curfind = Finds(SelectLine.row, SelectLine.col + 1, -1);
            if (curfind.col == 0) MessageBox.Show("Nothing found.");
            else SelectLine = curfind;
        }

        private void dgvScripts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectLine.col = e.ColumnIndex;
            SelectLine.row = e.RowIndex;
        }

        private void dgvScripts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex > -1)
                OpenScript(dgvScripts.Rows[e.RowIndex].Cells[2].Value.ToString());
        }

        private void OpenScript(string scriptName)
        {
            if (Settings.outputDir == null)
                return;

            string pathScript = Path.Combine(Settings.outputDir, scriptName);
            if (!File.Exists(pathScript)) {
                var undat = new UndatFile();
                if (!undat.UnpackFile(ref pathScript)) {
                    if (pathScript != null)
                        MessageBox.Show("Unpack script file error.", "Open Script");
                    return;
                }
            }
            TE.Open(pathScript, TextEditor.OpenType.File, false, outputFolder : true);
        }
    }
}
