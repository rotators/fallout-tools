using System;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using System.IO;

namespace ScriptEditor
{
    public static class FileAssociation
    {
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const uint SHCNF_IDLIST = 0x0U;
        private const string USER_CLASSES_PATH = @"Software\Classes";
        private const string CAPABILITIES_PATH = @"Software\Rotators\SfallScriptEditor\Capabilities";
        private const string REGISTERED_APPLICATIONS_PATH = @"Software\RegisteredApplications";

        private static readonly string appName = "SfallScriptEditor";
        private static readonly string[] extAllowed = { ".ssl", ".msg", ".int", ".fcd", ".h", ".ini", ".txt", ".cfg", ".xshd" };
        private static readonly string[] associatedExtensions = { ".ssl", ".msg", ".int", ".fcd" };

        public static bool CheckFileAllow(string ext, out bool Exists)
        {
            if (File.Exists(ext))
                Exists = true;
            else 
                Exists = false;
            ext = Path.GetExtension(ext).ToLowerInvariant();
            bool result = (Array.IndexOf(extAllowed, ext) > -1);
            if (!result)
                ScriptEditor.ThemedMessageBox.Show("You cannot open this file type in the editor.", "Error - File type not allowed");
            return result;
        }

        public static void Associate(bool force = false)
        {
            try {
                RegisterPerUser();
                if (force) OpenDefaultApps();
            } catch (UnauthorizedAccessException ex) {
                ShowAssociationError(ex);
            } catch (SecurityException ex) {
                ShowAssociationError(ex);
            } catch (Exception ex) {
                ShowAssociationError(ex);
            }
        }

        private static void RegisterPerUser()
        {
            using (RegistryKey classes = Registry.CurrentUser.CreateSubKey(USER_CLASSES_PATH))
            using (RegistryKey capabilities = Registry.CurrentUser.CreateSubKey(CAPABILITIES_PATH))
            using (RegistryKey fileAssociations = capabilities.CreateSubKey("FileAssociations")) {
                capabilities.SetValue("ApplicationName", AboutBox.appName);
                capabilities.SetValue("ApplicationDescription", "Edit and compile Fallout SSL scripts.");
                capabilities.SetValue("ApplicationIcon", Application.ExecutablePath + ",0");

                foreach (string extension in associatedExtensions) {
                    string progId = appName + extension.Substring(1).ToUpperInvariant();
                    RegisterFileType(classes, extension, progId);
                    fileAssociations.SetValue(extension, progId);
                }
            }

            using (RegistryKey registeredApplications = Registry.CurrentUser.CreateSubKey(REGISTERED_APPLICATIONS_PATH))
                registeredApplications.SetValue(AboutBox.appName, CAPABILITIES_PATH);

            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }

        private static void RegisterFileType(RegistryKey classes, string extension, string progId)
        {
            using (RegistryKey extensionKey = classes.CreateSubKey(extension))
            using (RegistryKey openWith = extensionKey.CreateSubKey("OpenWithProgids"))
                openWith.SetValue(progId, string.Empty, RegistryValueKind.String);

            using (RegistryKey key = classes.CreateSubKey(progId)) {
                string extensionName = extension.Substring(1).ToUpperInvariant();
                key.SetValue("", AboutBox.appName + " " + extensionName + " file");
                key.SetValue("FriendlyTypeName", AboutBox.appName + " " + extensionName + " file");
                key.SetValue("AlwaysShowExt", string.Empty);

                using (RegistryKey icon = key.CreateSubKey("DefaultIcon"))
                    icon.SetValue("", Path.Combine(Settings.ResourcesFolder,
                        "icon_" + extension.Substring(1).ToLowerInvariant() + ".ico"));
                using (RegistryKey shell = key.CreateSubKey("Shell"))
                    shell.SetValue("", "OpenSSEditor");
                using (RegistryKey open = key.CreateSubKey(@"Shell\OpenSSEditor"))
                    open.SetValue("", "Open in Sfall Script Editor");
                using (RegistryKey command = key.CreateSubKey(@"Shell\OpenSSEditor\Command"))
                    command.SetValue("", "\"" + Application.ExecutablePath + "\" \"%1\"");
            }
        }

        private static void OpenDefaultApps()
        {
            EditorNotifications.Show(Form.ActiveForm,
                "Sfall Script Editor was added to Windows. Choose its file types on the Default apps page.",
                NotificationKind.Success, 7000);

            string appSettingsUri = "ms-settings:defaultapps?registeredAppUser=" +
                Uri.EscapeDataString(AboutBox.appName);
            try {
                Process.Start(new ProcessStartInfo(appSettingsUri) { UseShellExecute = true });
            } catch {
                Process.Start(new ProcessStartInfo("ms-settings:defaultapps") { UseShellExecute = true });
            }
        }

        private static void ShowAssociationError(Exception ex)
        {
            ScriptEditor.ThemedMessageBox.Show("Windows could not register the editor for file associations.\n\n" +
                ex.Message, "File association error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                ScriptEditor.ThemedMessageBox.Show("The script file for this flowchart was not found.", "Missing script file");

            return false;
        }
    }
}
