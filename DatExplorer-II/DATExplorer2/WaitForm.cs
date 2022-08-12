using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;

using DATLib;

namespace DATExplorer
{
    public partial class WaitForm : Form
    {
        public BackgroundWorker bwork;

        private string cutPath;
        private string unpackPath;
        private string nameDat;
        private List<string> listFiles;

        enum WorkState {
            Extract,
            ExtractSingle,
            Delete,
            Save,
            SaveAppend
        }

        private WorkState state = WorkState.Extract;

        ExplorerForm ownerFrm;
        ListView.SelectedListViewItemCollection list;

        public WaitForm(Form owner)
        {
            this.ownerFrm = (ExplorerForm)owner;

            InitializeComponent();
        }

        public void Unpack(string unpackPath, List<string> listFiles, string nameDat, string cutPath)
        {
            this.state = WorkState.Extract;

            this.unpackPath = unpackPath + '\\';
            this.listFiles = listFiles;
            this.nameDat = nameDat;
            this.cutPath = cutPath;

            this.ShowDialog(ownerFrm);
        }

        public void UnpackFile(string unpackPath, string listFiles, string nameDat)
        {
            this.state = WorkState.ExtractSingle;

            this.unpackPath = unpackPath + '\\';
            this.listFiles = new List<string>() { listFiles };
            this.nameDat = nameDat;

            this.ShowDialog(ownerFrm);
        }

        public void RemoveFile(string file)
        {
            this.state = WorkState.Delete;
            this.listFiles = new List<string>() { file };

            this.ShowDialog(ownerFrm);
        }

        public void RemoveFile(ListView.SelectedListViewItemCollection list)
        {
            this.state = WorkState.Delete;
            this.list = list;

            this.ShowDialog(ownerFrm);
        }

        public void SaveDat(string nameDat, bool isAppend)
        {
            this.state = (isAppend) ? WorkState.SaveAppend : WorkState.Save;
            this.nameDat = nameDat;

            this.ShowDialog(ownerFrm);
        }

        private void WaitForm_Shown(object sender, EventArgs e)
        {
            switch (this.state)
            {
                case WorkState.Extract:
                case WorkState.ExtractSingle:
                    Extraction();
                    break;

                case WorkState.Delete:
                    Removing();
                    break;

                case WorkState.Save:
                case WorkState.SaveAppend:
                    Saving();
                    break;
            }
        }

        private void WorkerRun()
        {
            bwork = new BackgroundWorker();
            bwork.RunWorkerCompleted += bwork_RunWorkerCompleted;
            bwork.DoWork += bwork_DoWork;
            bwork.RunWorkerAsync();
        }

        private void bwork_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (this.state)
            {
                case WorkState.Extract:
                    if (listFiles != null) {
                        if (cutPath != string.Empty)
                            DATManage.ExtractFileList(unpackPath, listFiles.ToArray(), nameDat, cutPath);
                        else
                            DATManage.ExtractFileList(unpackPath, listFiles.ToArray(), nameDat);
                    } else {
                        DATManage.ExtractAllFiles(unpackPath, nameDat);
                    }
                    break;
                case WorkState.Save:
                    DATManage.SaveDAT(nameDat);
                    break;
                case WorkState.SaveAppend:
                    DATManage.AppendFilesDAT(nameDat);
                    break;
            }
        }

        private void bwork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispose();
        }

        private void Extraction()
        {
            string msgFMT;

            if (ExplorerForm.LocaleRU) {
                label1.Text = "Подождите идет извлечение файлов...";
                msgFMT = "Файл {0} уже существует.\nПерезаписать файл?";
            } else {
                label1.Text = "Please wait extraction of files...";
                msgFMT = "The file {0} already exists.\nOverwrite the file?";
            }
            if (this.state == WorkState.ExtractSingle) {
                Application.DoEvents();

                DATManage.ExtractFile(unpackPath, listFiles[0], nameDat);

                this.Dispose();
            } else {
                List<string> list = new List<string>();

                int len = (cutPath != null) ? cutPath.Length : 0;
                foreach (var f in listFiles)
                {
                    string cfile = Path.GetFullPath(Path.Combine(unpackPath, (len != 0) ? f.Substring(len) : f));
                    if (File.Exists(cfile)) {
                        this.Activate();
                        var result = MessageBox.Show(String.Format(msgFMT, cfile), "DAT Explorer II", MessageBoxButtons.YesNoCancel);
                        if (result == DialogResult.No) continue;
                        if (result == DialogResult.Cancel) break;
                    }
                    list.Add(f);
                }
                if (list.Count != listFiles.Count) listFiles = list;

                WorkerRun();
            }
        }

        private void Removing()
        {
            if (ExplorerForm.LocaleRU)
                label1.Text = "Подождите идет удаление файлов...";
            else
                label1.Text = "Wait for the files to be deleted...";

            Application.DoEvents();

            if (listFiles != null)
                ownerFrm.DeleteFiles(listFiles[0]);
            else
                ownerFrm.DeleteFiles(list);

            this.Dispose();
        }

        private void Saving()
        {
            if (ExplorerForm.LocaleRU)
                label1.Text = "Подождите идет сохранение DAT файла...";
            else
                label1.Text = "Wait for saving the DAT file ...";

            WorkerRun();
        }
    }
}
