using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ScriptEditor.TextEditorUtilities
{
    public class UndatFile
    {
        private static string TempDatFile = null;
        internal static string selectDatFile = null;

        private string dat2exe;
        private string selectDatPath;

        public UndatFile()
        {
            dat2exe = Path.Combine(Settings.ResourcesFolder, "dat2.exe");
        }

        public bool UnpackFile(ref string file)
        {
            bool success = false;

            selectDatPath = selectDatFile;
            if (selectDatFile == null) {
                var ofd = new OpenFileDialog() {
                     InitialDirectory = TempDatFile ?? Settings.ProgramFolder,
                     DefaultExt = "dat",
                     Filter = "Fallout dat file (.dat)|*.dat",
                     Title = "Select fallout dat file"
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                    selectDatPath = selectDatFile = TempDatFile = ofd.FileName;
                else {
                    file = null;
                    return success;
                }
            }
            //check .dat file
            if (File.Exists(selectDatPath)) {
                string sFile = Path.GetFileName(file);

                //unpack from .dat
                ProcessStartInfo psi = new ProcessStartInfo(dat2exe, GetCommandLine(sFile));
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.WorkingDirectory = Settings.ResourcesFolder;
                Process wp = Process.Start(psi);
                wp.WaitForExit(1000);
                success = (wp.ExitCode == 0);
                wp.Dispose();

                if (success) {
                    file = Settings.scriptTempPath + @"\" + sFile;
                    if (!File.Exists(file))
                        success = false;
                }
            }
            if (!success) selectDatFile = null;
            return success;
        }

        private string GetCommandLine(string sFile)
        {
            return "x -p -d \"" + Settings.scriptTempPath + "\" \""
                    + selectDatPath + "\" "
                    + @"scripts\" + sFile;
        }
    }
}
