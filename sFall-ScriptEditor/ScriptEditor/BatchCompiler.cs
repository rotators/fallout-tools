using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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
        private readonly Queue<string> pendingFiles = new Queue<string>();
        private readonly Queue<BatchResult> pendingResults = new Queue<BatchResult>();
        private readonly object pendingFilesLock = new object();
        private readonly object pendingResultsLock = new object();
        private readonly Timer progressTimer;
        private readonly System.Diagnostics.Stopwatch compileStopwatch = new System.Diagnostics.Stopwatch();

        private sealed class BatchResult
        {
            internal readonly string File;
            internal readonly string Error;
            internal readonly bool Failed;

            internal BatchResult(string file, bool failed, string error = null)
            {
                File = file;
                Failed = failed;
                Error = error;
            }
        }

        private BatchCompiler(string[] files)
        {
            InitializeComponent();
            InterfaceTheme.ApplyOnLoad(this);
            progressTimer = new Timer { Interval = 75 };
            progressTimer.Tick += delegate { FlushPendingResults(); };
            textBox.ScrollBars = ScrollBars.None;
            Load += delegate { UpdateOutputScrollbar(); };

            found = files.Length;
            progressBar1.Maximum = found;

            label1.Text = "Failed count: 0";
            textBox.Text = String.Format("{0} scripts found.\r\n{1}", found, textBox.Text);
            foreach (string file in files)
                pendingFiles.Enqueue(file);

            int workerCount = Settings.multiThreaded ? Math.Min(Math.Min(Environment.ProcessorCount, 4), found) : 1;
            workers = new BackgroundWorker[workerCount];
            for (int i = 0; i < workers.Length; i++) {
                workers[i] = new BackgroundWorker();
                workers[i].RunWorkerCompleted += new RunWorkerCompletedEventHandler(BatchCompiler_RunWorkerCompleted);
                workers[i].DoWork += new DoWorkEventHandler(BatchCompiler_DoWork);
                workers[i].WorkerSupportsCancellation = true;
            }
            if (workers.Length > 1)
                Text += String.Format(" [Threads: {0}]", workers.Length);
            compileStopwatch.Start();
            progressTimer.Start();
            foreach (BackgroundWorker worker in workers)
                worker.RunWorkerAsync();
        }

        void BatchCompiler_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            while (!worker.CancellationPending) {
                string file;
                lock (pendingFilesLock) {
                    if (pendingFiles.Count == 0)
                        break;
                    file = pendingFiles.Dequeue();
                }

                bool failedCompile = false;
                string error = null;
                string unused;
                try {
                    failedCompile = !new Compiler(false).Compile(file, out unused, null, false, Settings.shortCircuit, true);
                } catch (Exception ex) {
                    failedCompile = true;
                    error = ex.Message;
                }

                lock (pendingResultsLock)
                    pendingResults.Enqueue(new BatchResult(file, failedCompile, error));
            }

            if (worker.CancellationPending)
                e.Cancel = true;
        }

        void BatchCompiler_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                AppendOutput("Compiler worker error: " + e.Error.Message + "\r\n");
            if (++completed != workers.Length)
                return;

            progressTimer.Stop();
            compileStopwatch.Stop();
            FlushPendingResults();
            int skipped = Math.Max(0, found - (failed + compiled));
            bCancel.Visible = false;
            bClose.Visible = true;
            AppendOutput(String.Format("--------------------\r\n{0} successfully compiled.\r\n{1} failed to compile.\r\n{2} skipped.\r\nCompile time: {3}",
                compiled, failed, skipped, FormatCompileTime(compileStopwatch.Elapsed)));
        }

        private void FlushPendingResults()
        {
            List<BatchResult> results = new List<BatchResult>();
            lock (pendingResultsLock) {
                while (pendingResults.Count > 0)
                    results.Add(pendingResults.Dequeue());
            }
            if (results.Count == 0)
                return;

            progressBar1.Value = Math.Min(progressBar1.Maximum, progressBar1.Value + results.Count);
            StringBuilder output = new StringBuilder();
            foreach (BatchResult result in results) {
                if (result.Failed) {
                    failed++;
                    output.Append("Failed: ").Append(System.IO.Path.GetFileName(result.File));
                    if (!String.IsNullOrEmpty(result.Error))
                        output.Append(" (").Append(result.Error).Append(')');
                    output.AppendLine();
                } else {
                    compiled++;
                }
            }
            label1.Text = "Failed count: " + failed;
            if (output.Length > 0)
                AppendOutput(output.ToString());
        }

        private static string FormatCompileTime(TimeSpan elapsed)
        {
            int minutes = (int)elapsed.TotalMinutes;
            return minutes > 0
                ? minutes.ToString() + "m " + elapsed.Seconds.ToString() + "s"
                : elapsed.Seconds.ToString() + "s";
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
            foreach (BackgroundWorker worker in workers)
                worker.CancelAsync();
            bCancel.Enabled = false;
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            progressTimer.Stop();
            foreach (BackgroundWorker worker in workers) {
                if (worker.IsBusy)
                    worker.CancelAsync();
            }
            base.OnFormClosing(e);
        }
    }
}
