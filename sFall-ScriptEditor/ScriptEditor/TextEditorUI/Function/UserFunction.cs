using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScriptEditor.TextEditorUI
{
    public partial class UserFunction : Form
    {
        bool done;
        
        internal static bool highlight;

        public UserFunction(bool add)
        {
            InitializeComponent();

            cbHighlight.Enabled = add;
            cbHighlight.Checked = highlight;
        }

        public bool InitShow(ref string name, ref string desc, ref string code)
        {
            tbName.Text = name;
            tbDesc.Text = desc;
            tbCode.Text = code;

            this.ShowDialog();

            name = tbName.Text.Trim();
            desc = tbDesc.Text.Trim();
            code = tbCode.Text.Trim();

            return done;
        }

        private void bDone_Click(object sender, EventArgs e)
        {
            done = true;
            this.Close();
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbHighlight_Click(object sender, EventArgs e)
        {
            highlight = cbHighlight.Checked;
        }
    }
}
