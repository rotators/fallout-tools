using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;

namespace ScriptEditor
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "SFALL_SCRIPT_EDITOR_4");
        private static int crashReportWritten;
        private static readonly string RunMarkerPath = Path.Combine(Application.StartupPath, "last-run.marker");

        private static void BeginRunDiagnostics(string[] args)
        {
            try {
                if (File.Exists(RunMarkerPath)) {
                    string previousPath = Path.Combine(Application.StartupPath,
                        "abnormal-termination-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + ".log");
                    File.Move(RunMarkerPath, previousPath);
                }

                File.WriteAllText(RunMarkerPath,
                    "Sfall Script Editor run marker" + Environment.NewLine
                    + "UTC start: " + DateTime.UtcNow.ToString("O") + Environment.NewLine
                    + "Version: " + Application.ProductVersion + Environment.NewLine
                    + "Command line: " + Environment.CommandLine + Environment.NewLine
                    + "Arguments: " + String.Join(" | ", args ?? new string[0]) + Environment.NewLine);
                Application.ApplicationExit += delegate { CompleteRunDiagnostics(); };
            } catch {
                // Diagnostics must never prevent the editor from starting.
            }
        }

        private static void AppendRunMarker(string message)
        {
            try {
                if (!File.Exists(RunMarkerPath))
                    return;
                File.AppendAllText(RunMarkerPath, DateTime.UtcNow.ToString("O") + "  " + message + Environment.NewLine);
            } catch {
                // The process may be terminating or the install folder may be unavailable.
            }
        }

        private static void CompleteRunDiagnostics()
        {
            try {
                if (File.Exists(RunMarkerPath))
                    File.Delete(RunMarkerPath);
            } catch {
                // A marker left behind will be reported as an abnormal termination next launch.
            }
        }

        private static void ConfigureCrashDiagnostics(string[] args)
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += delegate(object sender, ThreadExceptionEventArgs e) {
                WriteCrashReport("Windows Forms UI thread", e.Exception, args);
            };
            AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e) {
                WriteCrashReport("AppDomain", e.ExceptionObject as Exception, args);
            };
            TaskScheduler.UnobservedTaskException += delegate(object sender, UnobservedTaskExceptionEventArgs e) {
                WriteCrashReport("unobserved task", e.Exception, args);
                e.SetObserved();
            };
        }

        private static void WriteCrashReport(string source, Exception exception, string[] args)
        {
            if (Interlocked.Exchange(ref crashReportWritten, 1) != 0)
                return;

            try {
                string timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
                string path = Path.Combine(Application.StartupPath, "crash-" + timestamp + ".log");
                string report = "Sfall Script Editor crash report" + Environment.NewLine
                    + "UTC: " + DateTime.UtcNow.ToString("O") + Environment.NewLine
                    + "Source: " + source + Environment.NewLine
                    + "Version: " + Application.ProductVersion + Environment.NewLine
                    + "OS: " + Environment.OSVersion + Environment.NewLine
                    + "Command line: " + Environment.CommandLine + Environment.NewLine
                    + "Arguments: " + String.Join(" | ", args ?? new string[0]) + Environment.NewLine
                    + Environment.NewLine
                    + (exception == null ? "No managed exception object was supplied." : exception.ToString())
                    + Environment.NewLine;
                File.WriteAllText(path, report);
            } catch {
                // The process may be terminating or its directory may be unavailable.
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ConfigureCrashDiagnostics(args);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // reset working folder to EXE directory (to resolve possible issues in parse_main)
            Directory.SetCurrentDirectory(Application.StartupPath);
            
            if (args.Length > 0 && mutex.WaitOne(TimeSpan.Zero, true) 
                && Path.GetExtension(args[0]).ToLowerInvariant() == ".msg") {
                mutex.Close();
                BeginRunDiagnostics(args);
                // run only Messages editor
                printLog("Run only message editor...");
                Settings.Load();
                InterfaceTheme.Start();
                MessageEditor me = new MessageEditor(args[0].ToString());
                Application.Run(me);
            } else {
                // check if another instance is already running
                if (mutex.WaitOne(TimeSpan.Zero, true)) {
                    BeginRunDiagnostics(args);
                    File.Delete("sse.log");    
                    printLog("Run main editor...");                 
                    Settings.Load();
                    InterfaceTheme.Start();
                    // pass arguments of command line to opening
                    TextEditor te = new TextEditor(args);
                    Application.Run(te);
                    mutex.ReleaseMutex();
                    SingleInstanceManager.DeleteCommandLine();
                    printLog("Exit main editor.");    
                } else {
                    // only show message if opened normally without command line arguments
                    if (args.Length == 0) 
                        ScriptEditor.ThemedMessageBox.Show("Another instance is already running!", "Sfall Script Editor");
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

        public static void printLog(string log) {
            File.AppendAllText(Application.StartupPath + "\\sse.log", log + Environment.NewLine);
            AppendRunMarker(log);
        }
    }
}
