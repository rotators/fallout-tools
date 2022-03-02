using System.Windows.Forms;
using System.Drawing;

namespace ScriptEditor.TextEditorUI
{
    internal class CommonDGV
    {
        internal static DataGridView DataGridCreate()
        {
            DataGridViewTextBoxColumn c1 = new DataGridViewTextBoxColumn(), c2 = new DataGridViewTextBoxColumn(), c3 = new DataGridViewTextBoxColumn();
            c1.HeaderText = "File";
            c1.ReadOnly = true;
            c1.Width = 150; //c1.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            c2.HeaderText = "Line";
            c2.ReadOnly = true;
            c2.Width = 40;
            c2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            c3.HeaderText = "Match";
            c3.ReadOnly = true;
            c3.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DataGridView dgv = new DataGridView();
            dgv.Name = "dgv";
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.BackgroundColor = SystemColors.Control;
            dgv.ColumnHeadersHeight = 20;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.Columns.Add(c1);
            dgv.Columns.Add(c2);
            dgv.Columns.Add(c3);
            dgv.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgv.GridColor = SystemColors.ControlDark;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.BorderStyle = BorderStyle.Fixed3D;
            return dgv;
        }
    }
}
