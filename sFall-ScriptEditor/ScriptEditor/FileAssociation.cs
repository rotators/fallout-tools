using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace ScriptEditor
{
    public static class FileAssociation
    {
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        private const string FILE_EXTENSION = ".ssl";
        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const uint SHCNF_IDLIST = 0x0U;

        private static readonly string appName = "SfallScriptEditor";
        private static readonly string[] extAllowed = { FILE_EXTENSION, ".msg", ".int", ".fcd", ".h", ".ini", ".txt", ".cfg", ".xshd" };

        public static bool CheckFileAllow(string ext, out bool Exists)
        {
            if (File.Exists(ext))
                Exists = true;
            else 
                Exists = false;
            ext = Path.GetExtension(ext).ToLowerInvariant();
            bool result = (Array.IndexOf(extAllowed, ext) > -1);
            if (!result)
                MessageBox.Show("You can not open this file type in the editor.", "Error - file is not allowed");
            return result;
        }

        public static void Associate(bool force = false)
        {
            if (!force && IsAssociated)
                return; 
  
            if (MessageBox.Show("Do you want to associate the files (.ssl .int and .msg) to the script editor?",
                "Associate files", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            for (int i = 0; i < 4; i++)
            {
                // Удалить ранее ассоциированные с файлом разделы
                var value = Registry.ClassesRoot.CreateSubKey(extAllowed[i]).GetValue("");
                if (value != null)
                    Registry.ClassesRoot.DeleteSubKeyTree(value.ToString(), false);
                
                // Создаем новый раздел
                Registry.ClassesRoot.CreateSubKey(extAllowed[i]).SetValue("", appName + extAllowed[i].Remove(0,1).ToUpper());
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(appName + extAllowed[i].Remove(0, 1).ToUpper()))
                {
                    key.SetValue("", "Sfall Script Editor v.4.0");
                    key.SetValue("AlwaysShowExt", "");
                    key.CreateSubKey("DefaultIcon").SetValue("", Settings.ResourcesFolder + "\\icon_" + extAllowed[i].Remove(0,1) + ".ico");
                    key.CreateSubKey("Shell").SetValue("", "OpenSSEditor");
                    key.CreateSubKey(@"Shell\OpenSSEditor").SetValue("", "Open in Sfall ScriptEditor");
                    key.CreateSubKey(@"Shell\OpenSSEditor\Command").SetValue("", Application.ExecutablePath + " \"%1\"");
                }
            }
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }

        private static bool IsAssociated
        {
            get {
                string value = "";
                var reg = Registry.ClassesRoot.OpenSubKey(FILE_EXTENSION, false);
                if (reg != null)
                     value = reg.GetValue("", string.Empty).ToString();  
                return (value == (appName + "SSL"));
            }
        }

        public static bool CheckFCDFile(ref string file)
        {
            string fltmp = file;
            if (Path.GetExtension(fltmp) == ".fcd") {
                    fltmp = Path.ChangeExtension(fltmp, ".ssl");
                    if (File.Exists(fltmp)){
                        file = fltmp;
                        return true;
                    } else {
                        int z = fltmp.LastIndexOf(Path.DirectorySeparatorChar);
                        if (z > 0) {
                            z = fltmp.LastIndexOf(Path.DirectorySeparatorChar, z - 1);
                            if (z > 0) {
                                string path = fltmp.Remove(z + 1);
                                fltmp = Path.Combine(path, Path.GetFileName(fltmp));
                                if (File.Exists(fltmp)) {
                                    file = fltmp;
                                    return true;
                                }
                            }
                        }
                    }
                    file = null;
            }
            if (file == null)
                MessageBox.Show("The script file for this flowchart was not found.", "Missing script file");

            return false;
        }
    }
}
