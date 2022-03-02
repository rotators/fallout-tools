using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ScriptEditor.TextEditorUI
{
    internal static class HandlerProcedure
    {
        const string phFile = "\\ProcHandlers";
        public const string def = ".ini";
        public const string lng = "_rus.ini";
        public const byte English = 0;
        
        private static TextEditor TE;

        public static void CreateProcHandlers(ContextMenuStrip Menu, TextEditor handle)
        {
            TE = handle;
            bool sub = false;
            string parentName = "";
            string file = phFile + ((Settings.hintsLang == English) ? def : lng);
            string[] lines = File.ReadAllLines(Settings.DescriptionsFolder + file);
            ToolStripMenuItem SubMenuItem = new ToolStripMenuItem();
            Menu.Items.Add(new ToolStripSeparator());
            for (int i = 0; i < lines.Length; i++) {
                lines[i] = lines[i].Trim();
                if (lines[i].StartsWith("//")) continue;
                if (lines[i].StartsWith("<m>")) {
                    if (sub) {
                        Menu.Items.Add(SubMenuItem);
                        Menu.Items[Menu.Items.Count - 1].Text = parentName;
                        SubMenuItem = new ToolStripMenuItem();
                    } else sub = true;
                    parentName = lines[i].Substring(3, lines[i].Length - 3);
                    continue;
                }
                int n = lines[i].IndexOf("<d>");
                if (n > 0 && !sub) {
                    int cnt = Menu.Items.Count;
                    Menu.Items.Add(lines[i].Substring(0, n));
                    Menu.Items[cnt].ToolTipText = lines[i].Substring(n + 3, lines[i].Length - (n + 3));
                    Menu.Items[cnt].Click += new EventHandler(HandlerProcedure_Click);
                    continue;
                } else if (n > 0) {
                    int cnt = SubMenuItem.DropDownItems.Count;
                    SubMenuItem.DropDownItems.Add(lines[i].Substring(0, n));
                    SubMenuItem.DropDownItems[cnt].ToolTipText = lines[i].Substring(n + 3, lines[i].Length - (n + 3));
                    SubMenuItem.DropDownItems[cnt].Click += new EventHandler(HandlerProcedure_Click);
                    continue;
                }
                if (lines[i] == "<-m>") {
                    if (sub) {
                        Menu.Items.Add(SubMenuItem);
                        Menu.Items[Menu.Items.Count - 1].Text = parentName;
                        SubMenuItem = new ToolStripMenuItem();
                        sub = false;
                    }
                    continue;
                }
                if (lines[i] == "<->") Menu.Items.Add(new ToolStripSeparator());
            }
        }

        static void HandlerProcedure_Click(object sender, EventArgs e)
        {
            TE.CreateProcBlock(((ToolStripMenuItem)sender).Text);
        }
     }
}
