using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Document;

namespace ScriptEditor
{
    internal enum InsertAt {
        End   = 0,
        After = 1,
        Caret = 2
    }

    public partial class ProcForm : Form
    {
        private bool isCreateProcedure;
        private bool isSetName;

        public string CheckName { get; private set; }

        public string ProcedureName
        {
            get { return tbName.Text; }
        }

        internal InsertAt PlaceAt
        {
            get {
                if (!groupBoxProcedure.Enabled) return InsertAt.Caret;
                return (rbPasteAtEnd.Checked) ? InsertAt.End : InsertAt.After;
            }
        }

        public bool SetInsertAtArter
        {
            set { rbAfterSelProcedure.Checked = value; }
        }

        /// <summary>
        /// Получает установленное значение копировать ли тело процедуры.
        /// Включает или Выключает элемент управления.
        /// </summary>
        public bool CopyProcedure
        {
            get { return cbCopyBodyProc.Checked; }
            set { cbCopyBodyProc.Enabled = value; }
        }

        public ProcForm(string name, bool readOnly = false, bool proc = false)
        {
            InitializeComponent();

            this.isCreateProcedure = proc;

            if (proc && name != null)
                IncrementNumber(ref name);

            tbName.Text = name;
            tbName.ReadOnly = readOnly;
        }

        private void IncrementNumber(ref string name)
        {
            int lenName = name.Length - 1;
            if (Char.IsDigit(name[lenName])) {
                int i;
                for (i = lenName; i > 0; i--) {
                    if (!Char.IsDigit(name[i]))
                        break;
                }
                int numZero = lenName - i;
                int numb = int.Parse(name.Substring(++i));
                numb++;
                name = name.Remove(i) + numb.ToString(new string('0', numZero));
            }
        }

        private void ProcForm_Shown(object sender, EventArgs e)
        {
            if (tbName.Text.Length == 0
               || tbName.Text.IndexOf(' ') > -1
               || tbName.Text == "procedure"
               || tbName.Text == "begin"
               || tbName.Text == "end"
               || tbName.Text == "variable")
            {
                tbName.Text = "Example(arg0, arg1)";
                tbName.ForeColor = System.Drawing.Color.Gray;
                tbName.SelectionStart = 0;
            } else {
                isSetName = true;
                tbName.SelectionStart = tbName.Text.Length;
            }
            tbName.Focus();
            if (isCreateProcedure) tbName.Enter += tbName_Enter;

        }

        private void ProcForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isSetName || DialogResult != DialogResult.OK || tbName.Text.Length == 0) return;

            tbName.Text = tbName.Text.Trim();
            string name = tbName.Text;

            int z = name.IndexOf('(');
            if (z > 0) {
                while (true)
                {
                    z = name.IndexOf("variable ", z, StringComparison.OrdinalIgnoreCase);
                    if (z == -1) break;
                    name = name.Remove(z, 9);
                }
            }
            // удаление пробелов
            name = name.Replace(" ", "");

            CheckName = name;
            z = CheckName.IndexOf('(');
            if (z > -1) CheckName = CheckName.Remove(z);

            // проверка корректности имени
            for (int i = 0; i < CheckName.Length; i++)
            {
                char ch = CheckName[i];
                if (!TextUtilities.IsLetterDigitOrUnderscore(ch)) {
                    e.Cancel = true;
                    break;
                }
            }

            if (isCreateProcedure && z > 0) {
                int pairCount = 0, pair = 0;
                // проверка корректности аргументов
                for (int i = z; i < name.Length; i++)
                {
                    char ch = name[i];
                    if (ch == ',') continue;
                    if (pair > 0 && ch == ')') pairCount++;
                    if (ch == '(') pair++;
                    if (ch == ')') pair--;
                    if ( ch == '(' || ch == ')') continue;

                    if (!TextUtilities.IsLetterDigitOrUnderscore(ch)) {
                        e.Cancel = true;
                        break;
                    }
                }
                if (pair != 0 || pairCount > 1) e.Cancel = true;
            }

            if (e.Cancel)
                MessageBox.Show("Was used incorrect name.\nThe name can only contain alphanumeric characters and the underscore character.", "Incorrect name");
            else {
                // вставляем ключевые слова 'variable' для аргументов процедуры
                if (z != -1) {
                    z++;
                    int y = name.LastIndexOf(')');
                    if (z == y) return; // no args

                    string pName = name.Substring(0, z - 1);

                    List<byte> args = new List<byte>();
                    for (byte i = (byte)z; i < y; i++) {
                        if (name[i] == ',') args.Add(i);
                    }
                    args.Add((byte)y);

                    // извлекаем имена аргументов
                    string argNames = string.Empty;
                    for (byte i = 0; i < args.Count; i++)
                    {
                        int x = args[i];
                        string argName = name.Substring(z, x - z).Trim();
                        z = x + 1;

                        if (argName.Length == 0) continue;

                        if (!argName.StartsWith("variable ")) argName = argName.Insert(0, "variable ");
                        if (argNames != string.Empty) argNames += ", ";
                        argNames += argName;
                    }
                    tbName.Text = string.Format("{0}({1})", pName, argNames);
                }
            }
        }

        internal static bool CreateRenameForm(ref string name, string tile = "")
        {
            ProcForm RenameFrm = new ProcForm(name);
            RenameFrm.groupBoxProcedure.Enabled = false;
            RenameFrm.Text = "Rename " + tile;
            RenameFrm.Create.Text = "OK";
            if (RenameFrm.ShowDialog() == DialogResult.Cancel || RenameFrm.ProcedureName.Length == 0) {
                return false;
            }
            name = RenameFrm.ProcedureName.Trim();
            RenameFrm.Dispose();
            return true;
        }

        private void tbName_Leave(object sender, EventArgs e)
        {
            if (isCreateProcedure && (!isSetName || tbName.Text.Length == 0)) {
                tbName.Text = "Example(arg0, arg1)";
                tbName.ForeColor = System.Drawing.Color.Gray;
                isSetName = false;
            }
        }

        private void tbName_Enter(object sender, EventArgs e)
        {
            if (!isSetName && tbName.Text.Length > 0) {
                tbName.ResetText();
                tbName.ForeColor = System.Drawing.SystemColors.ControlText;
            }
        }

        private void tbName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (!isSetName) {
                tbName.ResetText();
                tbName.ForeColor = System.Drawing.SystemColors.ControlText;
                isSetName = true;
            }
        }

        private void tbName_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isSetName) {
                tbName.ResetText();
                tbName.ForeColor = System.Drawing.SystemColors.ControlText;
                isSetName = true;
            }
        }
    }
}
