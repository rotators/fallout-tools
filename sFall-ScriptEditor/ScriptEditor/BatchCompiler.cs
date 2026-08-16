using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScriptEditor.CodeTranslation;

namespace ScriptEditor
{
    public partial class BatchCompiler : Form
    {
        private int found;
        private int failed;
        private int compiled;
        private readonly BackgroundWorker[] workers;
        private int completed;
        string[][] _lock;

        private sealed class BatchResult
        {
            internal readonly string File;
            internal readonly string Error;

            internal BatchResult(string file, string error = null)
            {
                File = file;
                Error = error;
            }
        }

        private BatchCompiler(string[] files)
        {
            InitializeComponent();
            InterfaceTheme.ApplyOnLoad(this);
            textBox.ScrollBars = ScrollBars.None;
            Load += delegate { UpdateOutputScrollbar(); };

            found = files.Length;
            progressBar1.Maximum = found;

            label1.Text = "Failed count: 0";
            textBox.Text = String.Format("{0} scripts found.\r\n{1}", found, textBox.Text);

            int workerCount = Settings.multiThreaded ? Math.Min(Math.Min(Environment.ProcessorCount, 4), found) : 1;
            workers = new BackgroundWorker[workerCount];
            for (int i = 0; i < workers.Length; i++) {
                workers[i] = new BackgroundWorker();
                workers[i].ProgressChanged += new ProgressChangedEventHandler(BatchCompiler_ProgressChanged);
                workers[i].RunWorkerCompleted += new RunWorkerCompletedEventHandler(BatchCompiler_RunWorkerCompleted);
                workers[i].DoWork += new DoWorkEventHandler(BatchCompiler_DoWork);
                workers[i].WorkerSupportsCancellation = true;
                workers[i].WorkerReportsProgress = true;
            }
            if (workers.Length == 1) {
                workers[0].RunWorkerAsync(files);
            } else {
                this.Text += String.Format(" [Threads: {0}]", workers.Length);
                int threadswithextras = found % workers.Length;
                int filesperthread = (found - (threadswithextras)) / workers.Length;
                int upto = 0;

                _lock = new string[workers.Length][];
                for (int i = 0; i < workers.Length; i++)
                {
                    string[] subblock = new string[filesperthread + (i < threadswithextras ? 1 : 0)];
                    for (int j = 0; j < subblock.Length; j++)
                    {
                        subblock[j] = files[upto++];
                    }
                    _lock[i] = subblock;
                    workers[i].RunWorkerAsync(subblock);
                }
            }
        }

        void BatchCompiler_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] files = (string[])e.Argument;
            BackgroundWorker worker = (BackgroundWorker)sender;
            int failed;
            string unused;
            foreach (string s in files) {
                if (worker.CancellationPending) {
                    e.Cancel = true;
                    break;
                }
                string error = null;
                try {
                    failed = new Compiler(false).Compile(s, out unused, null, false, Settings.shortCircuit, true) ? 0 : 1;
                } catch (Exception ex) {
                    failed = 1;
                    error = ex.Message;
                }
                worker.ReportProgress(failed, new BatchResult(s, error));
            }
        }

        void BatchCompiler_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                AppendOutput("Compiler worker error: " + e.Error.Message + "\r\n");
            if (++completed == workers.Length) {
                int skipped = Math.Max(0, found - (failed + compiled));
                bCancel.Visible = false;
                bClose.Visible = true;
                AppendOutput(String.Format("--------------------\r\n{0} successfully compiled.\r\n{1} failed to compile.\r\n{2} skipped.", compiled, failed, skipped));
            }
        }

        void BatchCompiler_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            BatchResult result = e.UserState as BatchResult;
            if (result == null)
                return;

            progressBar1.Value++;
            if (e.ProgressPercentage == 1) {
                failed++;
                label1.Text = "Failed count: " + failed;
                AppendOutput("Failed: " + System.IO.Path.GetFileName(result.File));
                if (!String.IsNullOrEmpty(result.Error))
                    AppendOutput(" (" + result.Error + ")");
                AppendOutput("\r\n");
            } else
                compiled++;
        }

        private void AppendOutput(string value)
        {
            textBox.AppendText(value);
            UpdateOutputScrollbar();
        }

        private void UpdateOutputScrollbar()
        {
            if (!textBox.IsHandleCreated)
                return;

            System.Drawing.Size measured = TextRenderer.MeasureText(textBox.Text + " ", textBox.Font,
                new System.Drawing.Size(Math.Max(1, textBox.ClientSize.Width), Int32.MaxValue),
                TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);
            ScrollBars target = measured.Height > textBox.ClientSize.Height ? ScrollBars.Vertical : ScrollBars.None;
            if (textBox.ScrollBars == target)
                return;

            textBox.ScrollBars = target;
            InterfaceTheme.Apply(textBox);
        }

        public static void CompileFolder(string path)
        {
            string[] infiles = System.IO.Directory.GetFiles(path, "*.ssl", System.IO.SearchOption.AllDirectories);
            if (infiles.Length == 0) {
                EditorNotifications.Show(Form.ActiveForm, "No .ssl files were found to compile.", NotificationKind.Warning);
                return;
            }
            BatchCompiler bc = new BatchCompiler(infiles);
            bc.Show();
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < workers.Length; i++)
                workers[i].CancelAsync();
            bCancel.Enabled = false;

        }

        private void bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            foreach (BackgroundWorker worker in workers) {
                if (worker.IsBusy)
                    worker.CancelAsync();
            }
            base.OnFormClosing(e);
        }
    }
}
