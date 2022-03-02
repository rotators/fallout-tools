using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ScriptEditor;
using ScriptEditor.TextEditorUI;

namespace ScriptEditor.CodeTranslation
{
    /// <summary>
    /// Class for compiling and decompile SSL code. Interacts with SSLC compiler via command line (EXE version) and DLL imports.
    /// </summary>
    public class Compiler
    {
        private static readonly string decompilationPath = Path.Combine(Settings.scriptTempPath, "decomp.ssl");
        private static readonly string preprocessPath = Path.Combine(Settings.scriptTempPath, "preprocess.ssl");

        private string outputSSL;
        private bool tempOutput = false;

        public Compiler(bool isTemp) {
            tempOutput = isTemp;
        }

        public static string GetPreprocessedFile(string sName)
        {
            if (!File.Exists(preprocessPath))
                return null;

            sName = Path.Combine(Settings.scriptTempPath, Path.GetFileNameWithoutExtension(sName) + "_[preproc].ssl");
            File.Delete(sName);
            File.Move(preprocessPath, sName);

            return sName;
        }

        public string GetOutputPath(string infile, string sourceDir = "")
        {
            string outputFile = Path.GetFileNameWithoutExtension(infile);
            if (sourceDir.Length != 0 && (Settings.useMcpp || Settings.useWatcom))
                outputFile = outputFile.Remove(outputFile.Length - 6);

            outputFile = outputFile + ".int";

            if (tempOutput) return Path.Combine(Settings.scriptTempPath, outputFile);

            if (Settings.ignoreCompPath && sourceDir.Length == 0)
                sourceDir = Path.GetDirectoryName(infile);

            outputSSL = (Settings.ignoreCompPath)
                         ? Path.Combine(sourceDir, outputFile)
                         : Path.Combine(Settings.outputDir, outputFile);

            return outputSSL;
        }

#if DLL_COMPILER
        public static string[] GetSslcCommandLine(string infile, bool preprocess) {
            return new string[] {
                "--", "-q",
                Settings.preprocess?"-P":"-p",
                Settings.optimize?"-O":"--",
                Settings.showWarnings?"--":"-n ",
                Settings.showDebug?"-d":"--",
                "-l", /* no logo */
                Path.GetFileName(infile),
                "-o",
                preprocess?preprocessPath:GetOutputPath(infile),
                null
            };
        }
#else
        // командная строка для компилятора
        private string GetSslcCommandLine(string infile, bool preprocess, string sourceDir, bool shortCircuit)
        {
            string usePreprocess = string.Empty; // неиспользовать препроцессор компилятора, если используется внешний mcpp/wcc
            if (!Settings.useMcpp && !Settings.useWatcom) usePreprocess = preprocess ? "-P " : "-p ";

            string pfDir = "-I\"" + Settings.ProgramFolder + "\" ";
            string includeDirs = (Settings.IsSearchIncludes) ? "-I\"" + Settings.pathHeadersFiles + "\" " + pfDir : pfDir;

            return (includeDirs)
                + (usePreprocess)
                + ("-q -l -O" + Settings.optimize + " ")
                + ((Settings.compileBackwardMode > 0) ? "-b " : "")
                + (Settings.showWarnings ? "" : "-n ")
                + (Settings.showDebug ? "-d " : "")
                + ((Settings.preprocDef != null) ? ("-m" + Settings.preprocDef) : "")
                + ((Settings.shortCircuit || shortCircuit) ? " -s" : "")
                + " \"" + Path.GetFileName(infile) + "\" -o \"" + (preprocess ? preprocessPath : GetOutputPath(infile, sourceDir)) + "\"";
        }

        // командная строка для внешних препроцессоров
        private string GetCommandLine(string infile, string outfile, string sourceDir, bool preprocess) {
            string searchPaths = " .."; // parent directory "Resources" folder
            if (Settings.IsSearchIncludes) {
                searchPaths += " \"" + Settings.pathHeadersFiles + "\"";
            }

            return (Settings.useWatcom)
                    ? /* wcc command line: infile outfile -pc/l i-paths [-d"macro"] */
                    ("\"" + infile + "\" ..\\scrTemp\\" + outfile
                     + ((preprocess) ? " c" : " l")
                     + searchPaths
                     + ((Settings.preprocDef != null) ? " -d" + Settings.preprocDef : string.Empty))
                    : /* mcpp command line: infile outfile -W1/0 i-paths [-D"macro"] [-P] */
                    ("\"" + infile + "\" ..\\scrTemp\\" + outfile
                     + ((Settings.showWarnings) ? " 1" : " 0")
                     + searchPaths
                     + ((Settings.preprocDef != null) ? (" -D" + Settings.preprocDef) : string.Empty)
                     + ((preprocess) ? " -P" : string.Empty));
        }

        // командная строка для внешнего файла компиляции
        private string GetCommandLine(string infile, string sourceDir, bool shortCircuit) {
            sourceDir = " \"" + sourceDir + "\"";
            string headers = (Settings.pathHeadersFiles != null)
                            ? "\"" + Settings.pathHeadersFiles + "\"" : "..\\";
            string output = (!Settings.ignoreCompPath && Settings.outputDir != null)
                            ? " \"" + Settings.outputDir + "\"" : sourceDir;

            return ("\"" + infile + "\" "
                    + headers
                    + output
                    + " " + (Settings.preprocDef ?? "0")
                    + " " + (Settings.optimize)
                    + sourceDir
                    + ((Settings.shortCircuit || shortCircuit) ? " -s" : string.Empty));
        }
#endif

#if DLL_COMPILER
        [System.Runtime.InteropServices.DllImport("resources\\sslc.dll")]
        private static extern int compile_main(int argc, string[] argv);

        [System.Runtime.InteropServices.DllImport("resources\\sslc.dll")]
        private static extern IntPtr FetchBuffer();
#endif

        public bool Compile(string infile, out string output, List<Error> errors, bool preprocessOnly, bool shortCircuit = false, bool batch = false)
        {
            if (errors != null) errors.Clear();
            if (infile == null) {
                output = "No filename specified";
                return false;
            }
            bool success = false;
            string batPath = null;
            infile = Path.GetFullPath(infile);
            string srcfile = infile;
            string sourceDir = Path.GetDirectoryName(infile);

            if (!batch)
                output = "****** " + DateTime.Now.ToString("HH:mm:ss") + " ******\r\n" + new String('-', 22);
            else
                output = string.Empty;

            if (Settings.userCmdCompile && !preprocessOnly) {
                batPath = Path.Combine(Settings.ResourcesFolder, "usercomp.bat");
                ProcessStartInfo upsi = new ProcessStartInfo(batPath, GetCommandLine(infile, sourceDir, shortCircuit));

                if (Encoding.Default.WindowsCodePage == 1251) upsi.StandardOutputEncoding = Encoding.GetEncoding("cp866");

                success = RunProcess(upsi, Settings.ResourcesFolder, ref output, batch);
            } else {
                // use external preprocessor
                string outfile = (batch) ? Path.GetRandomFileName() : "preprocess.ssl"; // common preprocess file (generate random name for batch compile)
                if (Settings.useMcpp || Settings.useWatcom) {
                    if (!batch) {
                        output += Environment.NewLine + (Settings.useWatcom ? "Open Watcom C32 preprocessing script: " : "External MCPP preprocessing script: ");
                        output += Path.GetFileName(infile) + Environment.NewLine;
                        output += "Predefine: " + (Settings.preprocDef ?? string.Empty) + Environment.NewLine;
                    }
                    batPath = Path.Combine(Settings.ResourcesFolder, Settings.useWatcom ? "wcc.bat" : "mcpp.bat");

                    ProcessStartInfo ppsi = new ProcessStartInfo(batPath, GetCommandLine(infile, outfile, sourceDir, preprocessOnly));
                    success = RunProcess(ppsi, Settings.ResourcesFolder, ref output, batch);

                    if (!batch) {
                        output += new string('-', 22) + Environment.NewLine;
                        if (success) {
                            output += "Created preprocessing file: OK\r\n";
                            output += "[Done] Preprocessing script successfully completed.\r\n";
                        } else
                            output += "[Error] Preprocessing script failed...";
                    }
                }

                if (batPath != null) {
                    if (!success || preprocessOnly) return success;
                    // переименовать файл preprocess.ssl в <scriptname>_[pre].ssl
                    infile = Path.Combine(Settings.scriptTempPath, Path.GetFileNameWithoutExtension(infile) + "_[pre].ssl");
                    File.Delete(infile);
                    File.Move(Path.Combine(Settings.scriptTempPath, outfile), infile);
                }

#if DLL_COMPILER
                string origpath=Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(Path.GetDirectoryName(infile));
                string[] args=Settings.GetSslcCommandLine(infile, preprocessOnly);
                bool success=compile_main(args.Length, args)==0;
                output=System.Runtime.InteropServices.Marshal.PtrToStringAnsi(FetchBuffer());
                Directory.SetCurrentDirectory(origpath);
#else
                var exePath = Path.Combine(Settings.ResourcesFolder, "compile.exe");
                ProcessStartInfo psi = new ProcessStartInfo(exePath, GetSslcCommandLine(infile, preprocessOnly, sourceDir, shortCircuit));

                string backupIntFile = null;
                if (File.Exists(outputSSL)) {
                    backupIntFile = Path.Combine(Settings.scriptTempPath, Path.GetRandomFileName()); // generate random name for batch compile
                    File.Copy(outputSSL, backupIntFile, true);
                }

                success = RunProcess(psi, Path.GetDirectoryName(infile), ref output, batch);

                if (backupIntFile != null) {
                    if (success) {
                        File.Delete(backupIntFile);
                    } else if (File.Exists(backupIntFile)) {
                        File.Delete(outputSSL);
                        File.Move(backupIntFile, outputSSL); // restore
                    }
                }
#endif
            }
            if (errors != null && !Settings.userCmdCompile) Error.BuildLog(errors, output, srcfile); //(Settings.useWatcom) ? infile :

#if DLL_COMPILER
            output=output.Replace("\n", "\r\n");
#endif
            return success;
        }

        private bool RunProcess(ProcessStartInfo psi, string wDir, ref string output, bool batch)
        {
            if (!batch) {
                psi.RedirectStandardOutput = true;
                //psi.RedirectStandardError = true;
            }
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.WorkingDirectory = wDir;

            Process wp = Process.Start(psi);

            if (!batch) {
                output += /*wp.StandardError.ReadToEnd() +*/ Environment.NewLine;
                output += wp.StandardOutput.ReadToEnd();
                if (Settings.useMcpp || Settings.useWatcom) output += GetErrorLog();
            }
            wp.WaitForExit(1000);

            bool success = (wp.ExitCode == 0);

            wp.Dispose();
            return success;
        }

        private string GetErrorLog()
        {
            string err = string.Empty;
            string file = Path.Combine(Settings.ResourcesFolder, Settings.useMcpp ? "mcpp.err" : "wcc.err");
            if (File.Exists(file)) {
                err = File.ReadAllText(file);
                File.Delete(file);
            }
            return err;
        }

        public string Decompile(string infile, Form scrForm)
        {
            List<string> program = new List<string>{ "int2ssl.exe", "int2ssl_v35.exe" };
            if (Settings.oldDecompile) program.RemoveAt(0);

            foreach (string exe in program)
            {
                var exePath = Path.Combine(Settings.ResourcesFolder, exe);
                ProcessStartInfo psi = new ProcessStartInfo(exePath, (Settings.decompileF1 ? "-1": String.Empty)
                                                            + (Settings.tabsToSpaces ? " -s" + Settings.tabSize : String.Empty)
                                                            + " \"" + infile + "\" \"" + decompilationPath + "\"");
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;

                Process p = Process.Start(psi);
                p.OutputDataReceived += ((TextEditor)scrForm).PrintBuildLog;
                p.BeginOutputReadLine();

                while (true) {
                    Application.DoEvents();
                    if (p.HasExited) break;
                }
                if (p.ExitCode == 0) {
                    p.Close();
                    break;
                }
                p.Close();
            }
            if (!File.Exists(decompilationPath)) return null;

            string result = Path.Combine(Settings.scriptTempPath, Path.GetFileNameWithoutExtension(infile));
            result += (!tempOutput) ? "_[decomp].ssl" : "_[temp].ssl";

            File.Delete(result);
            File.Move(decompilationPath, result);
            return result;
        }
    }
}
