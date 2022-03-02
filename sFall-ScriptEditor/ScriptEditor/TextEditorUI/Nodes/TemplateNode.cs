using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScriptEditor.TextEditorUI.Nodes
{
    /// <summary>
    /// Class form for node templates code create
    /// </summary>
    public class TemplateNode
    {
        private Form form;
        private Button buttonOk;

        public delegate bool CreateClickHandler(object sender, string nodeName, string nodeCode);
        public event CreateClickHandler CreateClick;

        // Templates node code
        private Dictionary<string, string> templatesCode = new Dictionary<string, string>
        {
                {"<Empty>", ""},
                {"Template Code 1", "\r\n   Reply(100);\r\n   NOption(101, Node999, 4);\r\n   NLowOption(102, Node999);\r\n"},
                {"Template Code 2", "\r\n   Reply(100);\r\n   if (true) then\r\n      NOption(101, Node999, 4);\r\n   else\r\n      NOption(102, Node999, 4);\r\n"},
                {"Template Code 3", "\r\n   NMessage(100);\r\n"}
        };

        public TemplateNode()
        {
            this.form = new Form();
            TextBox tbName = new TextBox();
            TextBox tbNodeCode = new TextBox();
            ComboBox cmbTemplate = new ComboBox();
            buttonOk = new Button();
            Button buttonCancel = new Button();

            tbNodeCode.Multiline = true;
            tbNodeCode.ScrollBars = ScrollBars.Vertical;
            tbNodeCode.Enter += tbNodeCode_Enter;
            tbNodeCode.Leave += tbNodeCode_Leave;

            tbName.Text = "Node###";
            tbName.SelectionStart = 4;
            tbName.SelectionLength = 3;

            buttonOk.Text = "Create";
            buttonCancel.Text = "Cancel";
            //buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            cmbTemplate.DropDownStyle = ComboBoxStyle.DropDownList;
            string[] code = new string[templatesCode.Count];
            templatesCode.Keys.CopyTo(code, 0);
            cmbTemplate.Items.AddRange(code);
            cmbTemplate.SelectedIndex = 0;

            cmbTemplate.SelectedIndexChanged += delegate(object sender, EventArgs e) { tbNodeCode.Text = templatesCode[cmbTemplate.SelectedItem.ToString()]; };
            //buttonCancel.Click += delegate(object sender, EventArgs e) { form.Hide(); };
            buttonOk.Click += delegate(object sender, EventArgs e)
                                {
                                    if (tbName.Text.IndexOf('#') != -1) {
                                        MessageBox.Show("Invalid node name.");
                                        return;
                                    }
                                    if (CreateClick(this, tbName.Text, tbNodeCode.Text))
                                        form.Hide();
                                };

            tbName.SetBounds(5, 5, 390, 23);
            cmbTemplate.SetBounds(5, 30, 150, 23);
            tbNodeCode.SetBounds(5, 60, 390, 105);
            buttonOk.SetBounds(240, 170, 75, 23);
            buttonCancel.SetBounds(320, 170, 75, 23);

            form.Text = "Create new Node";
            form.ClientSize = new Size(400, 200);
            form.Controls.AddRange(new Control[] { tbName, tbNodeCode, cmbTemplate, buttonOk, buttonCancel });
            form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
        }

        void tbNodeCode_Enter(object sender, EventArgs e)
        {
            form.AcceptButton = null;
        }

        void tbNodeCode_Leave(object sender, EventArgs e)
        {
            form.AcceptButton = buttonOk;
        }

        public void ShowForm()
        {
            form.ShowDialog();
        }
    }
}
