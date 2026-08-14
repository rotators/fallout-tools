using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ScriptEditor
{
    /// <summary>
    /// Handles enforcing single application instance and interacting with running instance.
    /// </summary>
    public static class SingleInstanceManager
    {
        private const int HWND_BROADCAST = 0xffff;

        private static readonly CommandLineQueue CommandLineArguments = new CommandLineQueue(
            Path.Combine(Path.GetTempPath(), "SfallScriptEditor-4-commandline"), TimeSpan.FromMinutes(5));

        [DllImport("user32")]
        private static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32")]
        private static extern int RegisterWindowMessage(string message);

        public static readonly int WM_SFALL_SCRIPT_EDITOR_OPEN = RegisterWindowMessage("WM_SFALL_SCRIPT_EDITOR_4_OPEN");

        public static void SendEditorOpenMessage()
        {
            SingleInstanceManager.PostMessage(
                    (IntPtr)SingleInstanceManager.HWND_BROADCAST,
                    SingleInstanceManager.WM_SFALL_SCRIPT_EDITOR_OPEN,
                    IntPtr.Zero,
                    IntPtr.Zero);
        }

        /// <summary>
        /// Saves command line arguments for use in another instance.
        /// </summary>
        /// <param name="args"></param>
        public static void SaveCommandLine(string[] args)
        {
            var fullArgs = new string[args.Length];
            for (int i = 0; i < args.Length; i++) {
                fullArgs[i] = Path.IsPathRooted(args[i])
                    ? args[i]
                    : Path.GetFullPath(args[i]);
            }
            CommandLineArguments.Enqueue(fullArgs);
        }

        /// <summary>
        /// Loads command line arguments previously saved by another instance.
        /// </summary>
        /// <returns></returns>
        public static string[] LoadCommandLine() 
        {
            return CommandLineArguments.DequeueAll();
        }

        public static void DeleteCommandLine()
        {
            CommandLineArguments.Clear();
        }
    }
}
