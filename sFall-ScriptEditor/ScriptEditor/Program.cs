using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;

namespace ScriptEditor
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "SFALL_SCRIPT_EDITOR_4");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // reset working folder to EXE directory (to resolve possible issues in parse_main)
            Directory.SetCurrentDirectory(Application.StartupPath);
            
            if (args.Length > 0 && mutex.WaitOne(TimeSpan.Zero, true) 
                && Path.GetExtension(args[0]).ToLowerInvariant() == ".msg") {
                mutex.Close();
                // run only Messages editor
                printLog("Run only message editor...");
                Settings.Load();
                MessageEditor me = new MessageEditor(args[0].ToString());
                Application.Run(me);
            } else {
                // check if another instance is already running
                if (mutex.WaitOne(TimeSpan.Zero, true)) {
                    File.Delete("sse.log");    
                    printLog("Run main editor...");                 
                    Settings.Load();
                    // pass arguments of command line to opening
                    TextEditor te = new TextEditor(args);
                    Application.Run(te);
                    mutex.ReleaseMutex();
                    SingleInstanceManager.DeleteCommandLine();
                    printLog("Exit main editor.");    
                } else {
                    // only show message if opened normally without command line arguments
                    if (args.Length == 0) 
                        MessageBox.Show("Another instance is already running!", "Sfall Script Editor");
                    else {
                        printLog("   Passed command argument to main editor.");
                        // pass command line arguments via file
                        SingleInstanceManager.SaveCommandLine(args);
                        // send message to other instance
                        SingleInstanceManager.SendEditorOpenMessage();
                    }
                }
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern Int16 GetKeyState(Keys keys);

        public static bool KeyHook(Keys key)
        {
            return ((GetKeyState(key) & 256) == 256);
        }

        public static void SetDoubleBuffered(Control cnt)
        {
            typeof (Control).InvokeMember("DoubleBuffered",
                    BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, cnt, new object[] {true});
        }

        public static void printLog(string log) { File.AppendAllText(Application.StartupPath + "\\sse.log", log + Environment.NewLine); }
    }
}
