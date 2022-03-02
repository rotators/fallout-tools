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

        private BatchCompiler(string[] files)
        {
            InitializeComponent();

            found = files.Length;
            progressBar1.Maximum = found;

            label1.Text = "Failed count: 0";
            textBox.Text = String.Format("{0} scripts found.\r\n{1}", found, textBox.Text);

            workers = new BackgroundWorker[Settings.multiThreaded ? Math.Min(Environment.ProcessorCount, found) : 1];
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
                if (new Compiler(false).Compile(s, out unused, null, false, Settings.shortCircuit, true))
                    failed = 0;
                else
                    failed = 1;
                worker.ReportProgress(failed, s);
            }
        }

        void BatchCompiler_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (++completed == workers.Length) {
                int skipped = (bCancel.Enabled) ? found - (failed + compiled) : 0;
                bCancel.Visible = false;
                bClose.Visible = true;
                textBox.Text += String.Format("--------------------\r\n{0} successfully compiled.\r\n{1} failed to compile.\r\n{2} skipped.", compiled, failed, skipped);
            }
        }

        void BatchCompiler_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value++;
            if (e.ProgressPercentage == 1) {
                failed++;
                label1.Text = "Failed count: " + failed;
                textBox.Text += "Failed: " + System.IO.Path.GetFileName(e.UserState.ToString()) + "\r\n";
            } else
                compiled++;
        }

        public static void CompileFolder(string path)
        {
            string[] infiles = System.IO.Directory.GetFiles(path, "*.ssl", System.IO.SearchOption.AllDirectories);
            if (infiles.Length == 0) {
                MessageBox.Show("Nothing found to compile", "Warning");
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
    }
}
