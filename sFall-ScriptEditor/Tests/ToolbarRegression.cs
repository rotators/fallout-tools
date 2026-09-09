using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ScriptEditor;

class ToolbarRegression
{
    static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static object Field(object owner, string name) { return owner.GetType().GetField(name, Private).GetValue(owner); }
    static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    static bool Key(Form editor, Keys key) {
        object[] args = { Message.Create(IntPtr.Zero, 0x100, (IntPtr)(int)key, IntPtr.Zero), key };
        return (bool)editor.GetType().GetMethod("ProcessCmdKey", Private).Invoke(editor, args);
    }
    [STAThread]
    static int Main() {
        try {
            Application.EnableVisualStyles();
            Directory.CreateDirectory(Settings.SettingsFolder);
            Directory.CreateDirectory(Path.Combine(Settings.ResourcesFolder, "templates"));
            File.WriteAllText(Settings.PreprocDefPath, "default\n");
            File.WriteAllText(Settings.SearchHistoryPath, "");
            Settings.EncCodePage = Encoding.Default;
            Settings.enableParser = false; Settings.firstRun = true;
            Type type = typeof(Settings).Assembly.GetType("ScriptEditor.TextEditor");
            using (Form editor = (Form)Activator.CreateInstance(type, new object[] { new string[0] }))
            { Form host = editor;
                ToolStrip toolbar = (ToolStrip)Field(editor, "ToolStripMain");
                ToolStripSplitButton compile = (ToolStripSplitButton)Field(editor, "qCompile_toolStripSplitButton");
                ToolStripMenuItem command = (ToolStripMenuItem)Field(editor, "Compile_ToolStripMenuItem");
                int clicks = 0;
                command.Click += delegate { clicks++; };
                command.Click -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), editor, type.GetMethod("compileToolStripMenuItem1_Click", Private));
                host.StartPosition = FormStartPosition.Manual;
                host.Location = new Point(10, 10); host.Opacity = 0;
                host.ShowInTaskbar = false;
                host.Size = new Size(1600, 768);
                editor.Shown -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), editor, type.GetMethod("TextEditor_Shown", Private)); host.Show(); host.Opacity = 0;
                Application.DoEvents();
                Check(Key(editor, Keys.F8) && clicks == 1, "F8 before overflow");
                foreach (int width in new int[] { 1024, 800, 640 }) {
                    host.Width = width;
                    toolbar.PerformLayout();
                    Check(compile.Placement == ToolStripItemPlacement.Overflow, "Compile must overflow at " + width);
                    toolbar.OverflowButton.ShowDropDown();
                    Application.DoEvents();
                    ToolStripDropDown overflow = toolbar.OverflowButton.DropDown;
                    ToolStripItem[] rows = toolbar.Items.Cast<ToolStripItem>().Where(i => i.GetCurrentParent() == overflow && i.Visible && !(i is ToolStripSeparator)).OrderBy(i => i.Bounds.Top).ToArray();
                    Check(rows.Length >= 3, "Expected several overflow rows");
                    Check(rows[0].Bounds.Top <= overflow.Padding.Top + 3, "Blank space above overflow commands");
                    Check(rows[rows.Length - 1].Bounds.Bottom <= overflow.ClientSize.Height, "Clipped overflow commands");
                    Check(rows.SequenceEqual(toolbar.Items.Cast<ToolStripItem>().Where(i => rows.Contains(i))), "Overflow must follow toolbar order");
                    for (int i = 1; i < rows.Length; i++)
                        Check(rows[i].Bounds.Top >= rows[i - 1].Bounds.Bottom, "Overflow items share a row at " + width);
                    int before = clicks;
                    Check(Key(editor, Keys.F8) && clicks == before + 1, "F8 with overflow open at " + width);
                    using (Bitmap bitmap = new Bitmap(overflow.Width, overflow.Height)) {
                        overflow.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                        bitmap.Save(Path.Combine(Application.StartupPath, "overflow-" + width + ".png"));
                    }
                    toolbar.OverflowButton.HideDropDown();
                    Application.DoEvents();
                    before = clicks;
                    Check(Key(editor, Keys.F8) && clicks == before + 1, "F8 after overflow closes at " + width);
                    Console.WriteLine("PASS overflow layout and F8 open/closed at " + width);
                }
                command.Enabled = false;
                int disabledClicks = clicks;
                Key(editor, Keys.F8);
                Check(clicks == disabledClicks, "Disabled Compile must not run");
                command.Enabled = true;
                compile.Enabled = false;
                Key(editor, Keys.F8);
                Check(clicks == disabledClicks, "Disabled Compile parent must not run");
                compile.Enabled = true;
                Key(editor, Keys.Shift | Keys.F8);
                Check(clicks == disabledClicks, "Modified F8 must not compile");
                host.Width = 1600;
                toolbar.PerformLayout();
                Check(compile.Placement == ToolStripItemPlacement.Main, "Compile returns to toolbar");
                Check(Key(editor, Keys.F8) && clicks == disabledClicks + 1, "F8 after widening");
                Console.WriteLine("PASS disabled commands, modified F8, and widening");
                // The harness intentionally bypasses document startup and its shutdown lifecycle.
                Environment.Exit(0);
            }
            Environment.Exit(0); return 0;
        } catch (Exception ex) { Console.WriteLine(ex); return 1; }
    }
}