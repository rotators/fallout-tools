using System;
using System.Windows.Forms;
using Path = System.IO.Path;
using Directory = System.IO.Directory;
using SearchOption = System.IO.SearchOption;
using System.Drawing;

using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.TextEditor;

using ScriptEditor.TextEditorUtilities;

namespace ScriptEditor
{
    public partial class Headers : Form
    {
        public delegate void ListClickHandler(string hfile);
        public event ListClickHandler SelectHeaderFile;

        private Point xy_pos;
        private string hFile;

        bool shiftDown, ctrlDown;

        protected override CreateParams CreateParams {
			get {
				CreateParams p = base.CreateParams;
				ShadowWindow.AddShadowToWindow(p);
				return p;
			}
		}

        public Headers(Point xy_pos)
        {
            InitializeComponent();

            if (!Settings.HeadersFormSize.IsEmpty)
                this.Size = Settings.HeadersFormSize;

            this.xy_pos = xy_pos;

            foreach (string file in Directory.GetFiles(Settings.pathHeadersFiles, "*.h", SearchOption.AllDirectories)) {
                ListViewItem lw = new ListViewItem();
                lw.ImageIndex = 0;
                string fileName = Path.GetFileName(file).ToLowerInvariant();
                string c = fileName[0].ToString().ToUpperInvariant();
                lw.Text = fileName.Remove(0, 1).Insert(0, c);
                lw.Tag = lw.ToolTipText = file;
                headersFilelistView.Items.Add(lw);
            }
        }

        private void Headers_Load(object sender, EventArgs e)
        {
            xy_pos.X += TextEditor.ActiveForm.Bounds.X - (int)(this.Size.Width / 2.5f);
            xy_pos.Y += TextEditor.ActiveForm.Bounds.Y + 60;
            this.Location = xy_pos;
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            hFile = e.Item.Tag.ToString();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (headersFilelistView.FocusedItem.Selected) {
                if (shiftDown && this.Tag != null) {
                    Utilities.PasteIncludeFile(hFile, (TextAreaControl)this.Tag);
                } else {
                    SelectHeaderFile(hFile);
                }
                if (!ctrlDown) Headers_Deactivate(null, EventArgs.Empty);
            }
        }

        private void Headers_Deactivate(object sender, EventArgs e)
        {
            Settings.HeadersFormSize = this.Size;
            this.Tag = null;
            Close();
            Dispose();
        }

        private void headersFilelistView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                if (e.Shift && this.Tag != null) {
                    Utilities.PasteIncludeFile(hFile, (TextAreaControl)this.Tag);
                } else {
                    SelectHeaderFile(hFile);
                }
                Headers_Deactivate(null, EventArgs.Empty);
            }
        }

        private void Headers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift) shiftDown = true;
            if (e.Control) ctrlDown = true;
        }

        private void Headers_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Shift) shiftDown = false;
            if (e.Control) ctrlDown = false;
        }
    }
}
