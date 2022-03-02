using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScriptEditor.TextEditorUI
{
    static class InputBox
    {
        public static DialogResult ShowDialog(string title, ref string value, int tSize = 75)
        {
            Form form = new Form() {Text = title, Width = 400, Height = tSize + 60, MinimizeBox = false, MaximizeBox = false };
            TextBox textBox = new TextBox() { Multiline = true, ScrollBars  = ScrollBars.Vertical };
            Button buttonOk = new Button() { Text = "Save", DialogResult = DialogResult.OK };
            Button buttonCancel = new Button() {  Text = "Cancel", DialogResult = DialogResult.Cancel };

            textBox.Text = value;
            textBox.SelectionStart = value.Length;

            textBox.SetBounds(5, 5, 390, tSize);
            buttonOk.SetBounds(232, tSize + 8, 75, 23);
            buttonCancel.SetBounds(312, tSize + 8, 75, 23);

            form.Controls.AddRange(new Control[] { textBox, buttonOk, buttonCancel });
            form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ShowIcon = form.ShowInTaskbar = false;
            //form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text.Trim();

            return dialogResult;
        }
    }
}
