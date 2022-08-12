using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DATLib;

namespace DATExplorer
{
    public partial class ExplorerForm : Form
    {
        private static readonly string tmpAppFolder = Application.StartupPath + "\\tmp" + Path.GetRandomFileName() + "\\";

        public static bool LocaleRU { get; private set; }

        private string currentDat;
        private TreeNode currentNode;

        private string extractFolder;
        private string dropExtractPath;
        private uint successExtracted;

        private FileWatcher dragDropFileWatcher;

        private bool skipKeyEvent;
        private string arg;

        public static void SetDoubleBuffered(Control cnt)
        {
            typeof (Control).InvokeMember("DoubleBuffered",
                    System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                    null, cnt, new object[] {true});
        }

        public ExplorerForm(string[] args)
        {
            if (args.Length > 0) this.arg = args[0];

            InitializeComponent();

            this.Text += Application.ProductVersion + " - by Mr.Stalin";

            SetDoubleBuffered(folderTreeView);
            SetDoubleBuffered(filesListView);

            LocaleRU = System.Globalization.CultureInfo.CurrentCulture.Name == "ru-RU";

            foreach (var dirPath in Directory.GetDirectories(Application.StartupPath))
            {
                var dir = Path.GetFileName(dirPath);
                if (dir.StartsWith("tmp")) Directory.Delete(dirPath, true);
            }

            dragDropFileWatcher = new FileWatcher(tmpAppFolder);

            dragDropFileWatcher.DragDrop += new FileWatcher.DropExplorerEvent(DropHandler);

            DAT.ExtractUpdate += ExtractUpdateEvent;
            DAT.RemoveFile += FileEvent;
            DAT.SavingFile += InvokeFileEvent;
        }

        private void ExplorerForm_Shown(object sender, EventArgs e)
        {
            if (arg != null) {
                Application.DoEvents();
                OpenDatFile(arg);
            } else  if (FileAssociation.GetConfig("Association") != "1") {
                FileAssociation.Associate();
                FileAssociation.SetConfig("Association", "1");
            }
        }

        private void ExplorerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            dragDropFileWatcher.Dispose();
        }

        private void OpenDatFile(string pathDat)
        {
            string message;
            if (!DATManage.OpenDatFile(pathDat, out message)) {
                MessageBox.Show(message, "Open Error");
                return;
            }

            OpenDat dat = ControlDat.OpeningDat(pathDat);
            if (currentDat == null) currentDat = dat.DatName;

            BuildTree(dat);

            if (currentNode != null) currentNode.ForeColor = Color.White;
            TreeNode[] node = folderTreeView.Nodes.Find(dat.DatName, false);
            SelectTreeNode(node[0]);
            folderTreeView.SelectedNode = node[0];

            totalToolStripStatusLabel.Text = dat.TotalFiles.ToString();
        }

        private void OpenDat(string pathDat)
        {
            if (ControlDat.DatIsOpen(pathDat)) {
                MessageBox.Show((LocaleRU) ? "Этот DAT файл уже открыт!" : "This DAT file is already open!");
                return;
            }
            OpenDatFile(pathDat);
        }

        private void GetFolderFiles(List<String> listFiles, string folderPath)
        {
            OpenDat dat = ControlDat.GetDat(currentDat);
            dat.GetFilesFromFolder(listFiles, folderPath);
        }

        private void ExtractUpdateEvent(ExtractEventArgs e)
        {
            Action action = () =>
            {
                textToolStripStatusLabel.Text = e.Name;
                toolStripProgressBar.Value++;
            };
            EndInvoke(BeginInvoke(action));
            if (e.Result) successExtracted++;
        }

        void FileEvent(FileEventArgs e)
        {
            //textToolStripStatusLabel.Text = e.File;
            toolStripProgressBar.Value++;
        }

        void InvokeFileEvent(FileEventArgs e)
        {
            Action action = () =>
            {
                textToolStripStatusLabel.Text = e.File;
                toolStripProgressBar.PerformStep();
            };
            EndInvoke(BeginInvoke(action));
        }

        /// <summary>
        /// Распаковывает список файлов с сохранением структуры каталогов
        /// </summary>
        /// <param name="listFiles"></param>
        private void ExtractFiles(List<string> listFiles)
        {
            extractFolderBrowser.SelectedPath = extractFolder;
            if (extractFolderBrowser.ShowDialog() == DialogResult.Cancel) return;
            extractFolder = extractFolderBrowser.SelectedPath;

            ExtractFiles(listFiles, extractFolder, string.Empty);
        }

        /// <summary>
        /// Распаковывает список файлов с
        /// </summary>
        /// <param name="listFiles"></param>
        /// <param name="extractToPath"></param>
        /// <param name="cutPath"></param>
        private void ExtractFiles(List<string> listFiles, string extractToPath, string cutPath)
        {
            successExtracted = 0;
            statusToolStripStatusLabel.Text = "Extracted:";
            toolStripProgressBar.Maximum = (listFiles != null) ? listFiles.Count
                                         : ControlDat.GetDat(currentDat).TotalFiles;

            new WaitForm(this).Unpack(extractToPath, listFiles, currentDat, cutPath);

            textToolStripStatusLabel.Text = successExtracted + " of " + toolStripProgressBar.Maximum + " files.";
            toolStripProgressBar.Value = 0;
        }

        private void ExtractFolder(string fullPath, string extractToPath, string folder)
        {
            List<String> listFiles = new List<String>();
            GetFolderFiles(listFiles, fullPath);

            if (listFiles.Count == 0) return;

            int cut = fullPath.LastIndexOf("\\" + folder);
            ExtractFiles(listFiles, extractToPath, ((cut > 0) ? fullPath.Remove(cut + 1) : String.Empty));
        }

        private void ImportFiles(string[] list)
        {
            OpenDat dat = ControlDat.GetDat(currentDat);

            string treeFolder = GetCurrentTreeFolder();
            if (treeFolder.Length > 0) treeFolder += '\\';

            foreach (var file in list)
            {
                dat.AddVirtualFile(file, treeFolder);
            }
            dat.OverwriteAll = false;

            // обновление списка
            FindFiles(currentDat, treeFolder);

            SaveToolStripButton.Enabled = true;
        }

        private void ImportFilesWithFolders(string[] list, string rootFolder)
        {
            OpenDat dat = ControlDat.GetDat(currentDat);
            string treeFolder = GetCurrentTreeFolder();

            foreach (var file in list)
            {
                if (rootFolder != null) {
                    string folder = file.Substring(rootFolder.Length);
                    int i = folder.LastIndexOf('\\') + 1;
                    folder = folder.Remove(i);

                    if (folder.Length > 0) {
                        if (treeFolder.Length > 0) {
                            //if (folder[0] != '\\')
                            //    folder = folder.Insert(0, "\\"); // добавить в начало
                        } else {
                            if (folder[0] == '\\') folder = folder.Substring(1); // удалить '\' в начале
                        }
                    }
                    dat.AddVirtualFile(file, treeFolder + folder);
                } else {
                    if (treeFolder.Length > 0) {
                        dat.AddVirtualFile(file, treeFolder + '\\');
                    } else {
                        dat.AddVirtualFile(file, treeFolder);
                    }
                }
            }
            dat.OverwriteAll = false;

            // обновление списка
            if (rootFolder != null) ReBuildTreeNode(dat);
            if (treeFolder.Length > 0) treeFolder += '\\';
            FindFiles(currentDat, treeFolder);

            SaveToolStripButton.Enabled = true;
        }

        private void ReBuildTreeNode(OpenDat dat)
        {
            List<TreeNode> expandNodes = new List<TreeNode>();
            Misc.GetExpandedNodes(currentNode, ref expandNodes);

            folderTreeView.BeginUpdate();
            currentNode.Nodes.Clear();

            if (currentNode.Parent == null) {
                Misc.BuildTreeSub(dat, currentNode);
            } else {
                foreach (var item in dat.Folders)
                {
                    if (item.Key.Length > currentNode.Name.Length && item.Key.StartsWith(currentNode.Name)) {
                        string[] dirs = item.Key.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                        TreeNode tn = currentNode;
                        string parentsDir = dirs[0] + "\\";

                        for (int i = 1; i < dirs.Length; i++)
                        {
                            parentsDir += dirs[i] + "\\";

                            if (tn.Text != dirs[i]) {
                                TreeNode find = Misc.FindNode(dirs[i], tn);
                                if (find == null) {
                                    tn = tn.Nodes.Add(item.Value.FolderName(i)); // имя папки
                                    tn.Name = parentsDir; // путь к папке (не должен содержать знак разделителя пути в начале)
                                } else
                                    tn = find;
                            }
                        }
                    }
                }
            }
            foreach (TreeNode node in expandNodes) Misc.ExpandNode(node, currentNode);

            currentNode.Expand();
            folderTreeView.EndUpdate();
        }

        private void BuildTree(OpenDat dat)
        {
            folderTreeView.BeginUpdate();

            TreeNode root = folderTreeView.Nodes.Add(dat.DatName, string.Format("[F{0}] ", dat.IsFO2Type() ? 2 : 1) + dat.DatName);
            root.NodeFont = new Font(folderTreeView.Font, FontStyle.Bold);
            root.SelectedImageIndex = root.ImageIndex = 2;

            Misc.BuildTreeSub(dat, root);
            root.Expand();
            folderTreeView.EndUpdate();
        }

        private void FindFiles(string datPath, TreeNode node)
        {
            string path = Misc.GetNodeFullPath(node);
            if (path.Length > datPath.Length)
                path = path.Remove(0, datPath.Length + 1) + '\\';
            else
                path = string.Empty; // for root folder

            FindFiles(datPath, path);
        }

        private void FindFiles(string datPath, string pathFolder)
        {
            string path = pathFolder.ToLowerInvariant();

            OpenDat dat = ControlDat.GetDat(datPath);

            List<String> subDirs = new List<String>();
            int len = path.Length;

            foreach (var item in dat.Folders)
            {
                if (item.Key.StartsWith(path)) {
                    string dirs = item.Value.FolderPath.TrimEnd('\\');
                    if (dirs.Length == 0 || len > dirs.Length) continue;

                    int i = (len > 0) ? dirs.LastIndexOf('\\') : dirs.IndexOf('\\');
                    if (i > len) {
                        string sub = dirs.Substring(len, i - len);
                        if (!subDirs.Contains(sub)) subDirs.Add(sub);
                    } else
                        subDirs.Add(dirs.Substring(len));
                }
            }

            filesListView.BeginUpdate();
            filesListView.Items.Clear();

            foreach (string dir in subDirs)
            {
                if (dir.Length > 0) {
                    ListViewItem lwItem = new ListViewItem(dir, 0);
                    lwItem.Name = path + dir.ToLowerInvariant() + "\\";
                    lwItem.SubItems.Add("<DIR>");
                    filesListView.Items.Add(lwItem);
                }
            }

            int dirCount = filesListView.Items.Count;

            // add files
            if (dat.Folders.ContainsKey(path)) {
                var datFolders = dat.Folders[path];
                foreach (sFile el in datFolders.GetFiles())
                {
                    ListViewItem lwItem = new ListViewItem(el.file.name, (el.file.info.PackedSize == -1) ? 2 : 1);
                    lwItem.Tag = el;
                    if (filesListView.View == View.Details) {
                        lwItem.SubItems.Add((el.file.info.IsPacked) ? "Packed" : (el.file.info.PackedSize == -1)  ? "Virtual" : string.Empty);
                        lwItem.SubItems.Add(el.file.info.Size.ToString());
                        lwItem.SubItems.Add((el.file.info.PackedSize != -1) ? el.file.info.PackedSize.ToString() : "N/A");
                    }
                    filesListView.Items.Add(lwItem);
                }
            }
            filesListView.EndUpdate();

            toolStripStatusLabelEmpty.Text = string.Format("{0} folder(s), {1} file(s).", dirCount, filesListView.Items.Count - dirCount);
            totalToolStripStatusLabel.Text = dat.TotalFiles.ToString();
            dirToolStripStatusLabel.Text = "Directory: " + path;
        }

        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            if (openDatFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            string pathDat = openDatFileDialog.FileName;
            if (pathDat != string.Empty) OpenDat(pathDat);
        }

        /// <summary>
        /// Возвращает путь с сохранением регистра
        /// </summary>
        private string GetCurrentTreeFolder()
        {
            string path = Misc.GetNodeFullPath(folderTreeView.SelectedNode);
            if (path.Length > currentDat.Length) {
                return path.Remove(0, currentDat.Length + 1);
            }
            return string.Empty;
        }

        private void folderTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SelectTreeNode(e.Node);
        }

        private void SelectTreeNode(TreeNode node)
        {
            upToolStripButton.Enabled = (node.Parent != null);

            currentNode = node;
            node.ForeColor = Color.Yellow;

            string datPath = Misc.GetDatName(node);

            if (currentDat != datPath) {
                SaveToolStripButton.Enabled = ControlDat.GetDat(datPath).ShouldSave();
                SaveToolStripButton.ToolTipText = "Save: " + datPath;
            }
            currentDat = datPath;
            FindFiles(datPath, node);

            extractFolderToolStripMenuItem.Enabled = true;
            closeToolStripButton.Enabled = true;
        }

        #region Menu control
        private void ListViewStyleCheck(View type)
        {
            switch (type) {
            case View.LargeIcon:
                largeToolStripMenuItem.Checked = true;
                listToolStripMenuItem.Checked = false;
                detailsToolStripMenuItem.Checked = false;
                break;
            case View.List:
                largeToolStripMenuItem.Checked = false;
                listToolStripMenuItem.Checked = true;
                detailsToolStripMenuItem.Checked = false;
                break;
            case View.Details:
                largeToolStripMenuItem.Checked = false;
                listToolStripMenuItem.Checked = false;
                detailsToolStripMenuItem.Checked = true;

                if (folderTreeView.SelectedNode != null) {
                    FindFiles(currentDat, folderTreeView.SelectedNode);
                }
                break;
            }
        }

        private void largeToolStripMenuItem_Click(object sender, EventArgs e) {
            filesListView.View = View.LargeIcon;
            ListViewStyleCheck(View.LargeIcon);
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e) {
            filesListView.View = View.Details;
            ListViewStyleCheck(View.Details);
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e) {
            filesListView.View = View.List;
            ListViewStyleCheck(View.List);
        }
        #endregion

        private void extractFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedFiles = filesListView.SelectedItems;
            List<String> listFiles = new List<String>();

            foreach (ListViewItem item in selectedFiles)
            {
                if (item.Tag != null) {
                    listFiles.Add(((sFile)item.Tag).path);
                } else { // selected folder
                    GetFolderFiles(listFiles, item.Name);
                }
            }
            ExtractFiles(listFiles);
        }

        private void extractFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<String> listFiles = new List<String>();
            GetFolderFiles(listFiles, folderTreeView.SelectedNode.Name);
            ExtractFiles(listFiles);
        }

        private void extractAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentDat == null) currentDat = Misc.GetRootNode(folderTreeView.SelectedNode).Text;
            ExtractFiles(null);
        }

        private void OpenFile()
        {
            if (filesListView.SelectedItems.Count == 0) return;

            var item = filesListView.SelectedItems[0];
            if (item.Tag != null) { // open file
                sFile sfile = (sFile)item.Tag;
                if (sfile.isVirtual) {
                    string realfile = ControlDat.GetDat(currentDat).GetFile(sfile.path).RealFile;
                    if (File.Exists(realfile)) System.Diagnostics.Process.Start("explorer", realfile);
                    return;
                }

                if (sfile.file.info.Size > 1048576) { // 1mb
                    new WaitForm(this).UnpackFile(tmpAppFolder, sfile.path, currentDat);
                } else {
                    DATManage.ExtractFile(tmpAppFolder, sfile.path, currentDat);
                }

                string ofile = tmpAppFolder + sfile.path;
                if (File.Exists(ofile)) System.Diagnostics.Process.Start("explorer", ofile);
            } else { // folder
                foreach (TreeNode node in folderTreeView.SelectedNode.Nodes)
                {
                    if (node.Text == item.Text) {
                        folderTreeView.SelectedNode = node;
                        break;
                    }
                }

                if (currentNode != null) currentNode.ForeColor = Color.White;
                currentNode = folderTreeView.SelectedNode;
                currentNode.ForeColor = Color.Yellow;

                FindFiles(currentDat, item.Name);

                upToolStripButton.Enabled = (currentNode.Parent != null);
            }
        }

        private void filesListView_DoubleClick(object sender, EventArgs e)
        {
            //if (!filesListView.CheckBoxes) {
            OpenFile();
            //}
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void filesListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (skipKeyEvent) {
                skipKeyEvent = false;
                return;
            }
            if (e.KeyData == Keys.Enter) {
                OpenFile();
                if (filesListView.Items.Count > 0) {
                     filesListView.Items[0].Selected = true;
                }
            }
        }

        private void closeDATToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseDat();
        }

        private void CloseDat()
        {
            if (currentDat != null && folderTreeView.SelectedNode != null &&
                MessageBox.Show(this, String.Format((LocaleRU) ? "Вы действительно хотите закрыть\n{0} файл?" : "Do you want to close\n{0} file?", currentDat), "Dat Explorer II", MessageBoxButtons.YesNo) == DialogResult.Yes) {

                ControlDat.CloseDat(currentDat);
                currentDat = null;

                filesListView.Items.Clear();
                folderTreeView.Nodes.RemoveAt(Misc.GetRootNode(folderTreeView.SelectedNode).Index);

                closeToolStripButton.Enabled = (folderTreeView.Nodes.Count > 0);
            }
        }

        private void cmsFolderTree_Opening(object sender, CancelEventArgs e)
        {
            bool state = (folderTreeView.SelectedNode != null && folderTreeView.SelectedNode.Parent == null);
            extractAllFilesToolStripMenuItem.Enabled = state;
            extractFolderToolStripMenuItem.Enabled = !state && folderTreeView.SelectedNode != null;

            createFolderToolStripMenuItem.Enabled = (folderTreeView.Nodes.Count != 0 && currentNode != null);
            renameFolderToolStripMenuItem.Enabled = (folderTreeView.SelectedNode != null && folderTreeView.SelectedNode.Parent != null);
            deleteFolderToolStripMenuItem.Enabled = renameFolderToolStripMenuItem.Enabled;

            addFoldersToolStripMenuItem.Enabled = createFolderToolStripMenuItem.Enabled;

            closeDATToolStripMenuItem.Enabled = state;

            currentDat = (folderTreeView.SelectedNode != null) ? Misc.GetDatName(folderTreeView.SelectedNode) : null;
        }

        private void listViewContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            extractFilesToolStripMenuItem.Enabled = (filesListView.SelectedItems.Count != 0);
            openToolStripMenuItem.Enabled = (filesListView.SelectedItems.Count == 1);
            renameToolStripMenuItem.Enabled = openToolStripMenuItem.Enabled;

            bool state = (currentDat != null);
            importFilesToolStripMenuItem.Enabled = state;
            importFoldersToolStripMenuItem.Enabled = state;
            createFolderToolStripMenuItem1.Enabled = state;
        }

        #region Drag list items

        bool dragListActive = false;

        void DropHandler(FileWatcher.DropEventArgs e)
        {
            dragDropFileWatcher.StopWatcher();
            dropExtractPath = e.PathDrop;
        }

        // Drop from List
        private void filesListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left) {

                List<String> dropSelected = new List<String>();
                foreach (ListViewItem item in filesListView.SelectedItems)
                {
                    if (item.Tag != null) {
                        sFile file = (sFile)item.Tag;
                        if (!file.isVirtual) dropSelected.Add(file.path);
                    } else { // for selected folder
                        GetFolderFiles(dropSelected, item.Name);
                    }
                }
                if (dropSelected.Count == 0) {
                    filesListView.DoDragDrop(new DataObject(DataFormats.FileDrop, ""), DragDropEffects.None);
                    return;
                }
                dragListActive = true;
                dropExtractPath = string.Empty;

                String[] dummyDropFile = new String[] { String.Empty };
                dragDropFileWatcher.StartWatcher(ref dummyDropFile[0]);

                IDataObject obj = new DataObject(DataFormats.FileDrop, dummyDropFile);
                DragDropEffects result = filesListView.DoDragDrop(obj, DragDropEffects.Copy);

                if (dragDropFileWatcher.IsRunning) dragDropFileWatcher.StopWatcher();
                dragListActive = false;

                if (dropExtractPath == string.Empty) return;

                string fullPath = (folderTreeView.SelectedNode.Parent != null) ? folderTreeView.SelectedNode.Name : String.Empty;

                ExtractFiles(dropSelected, dropExtractPath, fullPath);
            }
        }

        // Drop to List
        private void filesListView_DragDrop(object sender, DragEventArgs e)
        {
            if (dragListActive) return;

            string rootFolder = null;
            List<string> addFilesToDat = new List<string>();

            foreach (string file in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                if (Directory.Exists(file)) {
                    if (rootFolder == null) rootFolder = Path.GetDirectoryName(file);
                    addFilesToDat.AddRange(Directory.GetFiles(file, "*.*", SearchOption.AllDirectories));
                } else {
                    addFilesToDat.Add(file);
                }
            }
            ImportFilesWithFolders(addFilesToDat.ToArray(), rootFolder);
        }

        private void filesListView_DragEnter(object sender, DragEventArgs e)
        {
            if (treeDragActive || currentDat == null)
                e.Effect = DragDropEffects.None;
            else
                e.Effect = DragDropEffects.Copy;
        }
        #endregion

        #region Create DAT / Add files / Remove files

        private void fallout1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNew(DAT.FalloutType.Fallout1);
        }

        private void fallout2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNew(DAT.FalloutType.Fallout2);
        }

        private void CreateNew(DAT.FalloutType type)
        {
            if (CreateNewDatDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            string newDat = CreateNewDatDialog.FileName;
            if (newDat == string.Empty) return;

            if (ControlDat.DatIsOpen(newDat)) {
                MessageBox.Show((LocaleRU) ? "Данный DAT файл уже открыт!" : "This DAT file is already open!");
                return;
            }

            DATManage.CreateDatFile(newDat, type);
            OpenDat dat = ControlDat.OpeningDat(newDat, true); // empty

            BuildTree(dat);

            if (currentNode != null) currentNode.ForeColor = Color.White;
            TreeNode[] node = folderTreeView.Nodes.Find(dat.DatName, false);
            SelectTreeNode(node[0]);
            folderTreeView.SelectedNode = node[0];

            totalToolStripStatusLabel.Text = "0";
        }

        private void importFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (importFilesDialog.ShowDialog() == DialogResult.OK) ImportFiles(importFilesDialog.FileNames);
        }

        private void importFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK) return;
            ImportFilesWithFolders(Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.*", SearchOption.AllDirectories), folderBrowserDialog.SelectedPath);
        }

        bool createFolder = false;
        string createFolderPath; // путь с сохранением регистра

        private void CreateFolder()
        {
            int num = 0;
        exist:
            TreeNode addNode = new TreeNode((num == 0) ? "NewFolder" : "NewFolder" + num.ToString());

            string fullPath = Misc.GetNodeFullPath(currentNode);
            if (fullPath.Length > currentDat.Length)
                createFolderPath = fullPath.Substring(currentDat.Length + 1) + "\\";
            createFolderPath += addNode.Text  + "\\";

            OpenDat dat = ControlDat.GetDat(currentDat);
            if (dat.FolderExist(createFolderPath)) {
                num++;
                addNode = null;
                goto exist;
            }
            currentNode.Nodes.Add(addNode);

            currentNode.Expand();
            folderTreeView.SelectedNode = addNode;

            createFolder = true;

            SelectTreeNode(addNode);

            folderTreeView.LabelEdit = true;
            folderTreeView.SelectedNode.BeginEdit();
        }

        private void createFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateFolder();
        }

        private void createFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int num = 0;
        exist:
            string folderName = (num == 0) ? "NewFolder" : "NewFolder" + num.ToString();

            foreach (ListViewItem item in filesListView.Items)
            {
                if (item.Tag == null && folderName.Equals(item.Text, StringComparison.OrdinalIgnoreCase)) {
                    num++;
                    goto exist;
                }
            }

            ListViewItem lwItem = new ListViewItem(folderName, 0);
            lwItem.SubItems.Add("<DIR>");
            filesListView.Items.Add(lwItem);

            createFolder = true;
            filesListView.LabelEdit = true;
            lwItem.Selected = true;
            lwItem.BeginEdit();
        }

        #endregion

        #region Drag Tree nodes
        bool treeDragActive = false;

        private void folderTreeView_DragEnter(object sender, DragEventArgs e)
        {
            if (dragListActive)
                e.Effect = DragDropEffects.None;
            else {
                if (!treeDragActive) {
                    var drop = (string[])e.Data.GetData(DataFormats.FileDrop);
                    e.Effect = (drop[0].EndsWith(".dat", StringComparison.OrdinalIgnoreCase)) ? DragDropEffects.Copy : DragDropEffects.None;
                } else {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void folderTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ((TreeNode)e.Item).Parent != null)
            {
                currentDat = Misc.GetDatName((TreeNode)e.Item);

                treeDragActive = true;
                dropExtractPath = string.Empty;

                String[] dummyDropFile = new String[] { String.Empty };
                dragDropFileWatcher.StartWatcher(ref dummyDropFile[0]);

                IDataObject obj = new DataObject(DataFormats.FileDrop, dummyDropFile);
                DragDropEffects result = folderTreeView.DoDragDrop(obj, DragDropEffects.Copy);

                if (dragDropFileWatcher.IsRunning) dragDropFileWatcher.StopWatcher();
                treeDragActive = false;

                if (dropExtractPath == string.Empty) return;

                ExtractFolder(((TreeNode)e.Item).Name, dropExtractPath, ((TreeNode)e.Item).Text.ToLowerInvariant()); // без '/' в конце
            }
        }

        private void folderTreeView_DragDrop(object sender, DragEventArgs e)
        {
            if (treeDragActive) return;

            var drop = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var file in drop)
            {
                OpenDat(file);
            }
        }
        #endregion

        private void folderTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (currentNode != null) currentNode.ForeColor = Color.White;
        }

        private void upToolStripButton_Click(object sender, EventArgs e)
        {
            folderTreeView.SelectedNode = currentNode.Parent;
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filesListView.LabelEdit = true;
            filesListView.SelectedItems[0].BeginEdit();

            deleteFilesToolStripMenuItem.Enabled = false;
        }

        private void renameFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderTreeView.LabelEdit = true;
            folderTreeView.SelectedNode.BeginEdit();
        }

        private void folderTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            folderTreeView.LabelEdit = false;

            OpenDat dat = ControlDat.GetDat(currentDat);

            if (createFolder) {
                dat.AddEmptyFolder(createFolderPath);
                e.Node.Name = createFolderPath.ToLowerInvariant();
                createFolderPath = null;
                createFolder = false;
            }

            if (e.Label == null) return;
            if (e.Label.Equals(e.Node.Text)) {
                e.CancelEdit = true;
                return;
            }

            string folderPath = e.Node.Name; // in lower case;

            int i = folderPath.LastIndexOf(e.Node.Text.ToLowerInvariant() + '\\');
            string renameFolderPath = folderPath.Remove(i) + e.Label + '\\';

            if (dat.FolderExist(renameFolderPath, false)) {
                MessageBox.Show((LocaleRU)
                                ? "Директория с таким именем уже существует."
                                : "This directory already exists.",
                                "Dat Explorer II", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.CancelEdit = true;
                return;
            }
            e.Node.Name = renameFolderPath.ToLowerInvariant();

            dat.RenameFolder(folderPath, e.Label);

            SaveToolStripButton.Enabled = dat.ShouldSave();
            dirToolStripStatusLabel.Text = "Directory: " + e.Node.Name;
        }

        private void filesListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            deleteFilesToolStripMenuItem.Enabled = true;
            filesListView.LabelEdit = false;
            skipKeyEvent = true;

            if (!createFolder && (e.Label == null || e.Label.Equals(filesListView.Items[e.Item].Text))) {
                e.CancelEdit = true;
                return;
            }

            string newName = e.Label ?? filesListView.Items[e.Item].Text;

            if (e.Label != null) {
                bool isSelfRename = e.Label.Equals(filesListView.Items[e.Item].Text, StringComparison.OrdinalIgnoreCase);
                if (!isSelfRename) {
                    foreach (ListViewItem item in filesListView.Items)
                    {
                        if (e.Label.Equals(item.Text, StringComparison.OrdinalIgnoreCase)) {
                            if (item.Tag == null) {
                                MessageBox.Show((LocaleRU)
                                                 ? "Директория с таким именем уже существует."
                                                 : "This directory already exists.",
                                                 "Dat Explorer II", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                if (createFolder) {
                                   newName = filesListView.Items[e.Item].Text;
                                   e.CancelEdit = true;
                                   break;
                                }
                            } else { // file
                                MessageBox.Show((LocaleRU)
                                                ? "Файл с таким именем уже существует."
                                                : "The file with the same name already exists.",
                                                "Dat Explorer II", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            e.CancelEdit = true;
                            return;
                        }
                    }
                }
            }

            OpenDat dat = ControlDat.GetDat(currentDat);
            string fullPath = Misc.GetNodeFullPath(currentNode);

            if (createFolder) {
                string folderPath = fullPath + '\\' + newName + '\\';
                folderPath = folderPath.Substring(currentDat.Length + 1);

                TreeNode addNode = new TreeNode(newName);
                addNode.Name = folderPath.ToLowerInvariant();
                currentNode.Nodes.Add(addNode);

                filesListView.Items[e.Item].Name = addNode.Name;

                dat.AddEmptyFolder(folderPath);
                createFolder = false;
            } else {
                string pathTo = (fullPath + '\\' + filesListView.Items[e.Item].Text).ToLowerInvariant();
                pathTo = pathTo.Substring(currentDat.Length + 1);

                if (filesListView.Items[e.Item].Tag == null) { // folder
                    pathTo += '\\';
                    string renamePath = dat.RenameFolder(pathTo, e.Label);
                    if (renamePath == null) return;

                    filesListView.Items[e.Item].Name = renamePath;

                    // rename path and name for tree node
                    TreeNode node = Misc.FindPathNode(pathTo, currentNode);
                    node.Name = renamePath;
                    node.Text = e.Label;
                } else {
                    filesListView.Items[e.Item].Tag = dat.RenameFile(pathTo, e.Label);
                }
            }
            SaveToolStripButton.Enabled = dat.ShouldSave();
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            OpenDat dat = ControlDat.GetDat(currentDat);

            string message = (LocaleRU) ? "Сохранить изменения в DAT файл?" : "Save changes to a DAT file?";
            if (!dat.IsFO2Type())
                message += (LocaleRU)
                          ? "\n\nПримечание: Данная версия программы не поддерживает сжатие добавленных файлов для DAT формата Fallout 1."
                          : "\n\nNote: This version does not support the compression of the added files for DAT Fallout 1 format.";
            if (MessageBox.Show(message, "Dat Explorer II", MessageBoxButtons.YesNo) == DialogResult.No) return;

            statusToolStripStatusLabel.Text = "Saving:";
            textToolStripStatusLabel.Text = "Prepare...";

            int count = dat.TotalFiles - dat.AddedFiles;
            if (count > 1000) count /= 5;
            count += (dat.AddedFiles > 1000) ? dat.AddedFiles / 5 : dat.AddedFiles;
            toolStripProgressBar.Maximum = count;

            if (dat.SaveDat()) {
               FindFiles(currentDat, folderTreeView.SelectedNode);
            }

            SaveToolStripButton.Enabled = false;
            toolStripProgressBar.Value = 0;
            textToolStripStatusLabel.Text = "Done.";
        }

        private void ExplorerForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized) {
                splitContainer1.SplitterDistance = (int)(splitContainer1.Width * 0.2f);
            }
        }

        private void deleteFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderTreeView.SelectedNode != null) Delete(false);
        }

        private void deleteFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filesListView.SelectedItems.Count > 0) Delete(true);
        }

        private void Delete(bool isList)
        {
            if (MessageBox.Show((LocaleRU)
                                ? "Вы действительно хотите это удалить?"
                                : "Do you want to delete it?",
                                "Dat Explorer II", MessageBoxButtons.YesNo) == DialogResult.No) return;

            statusToolStripStatusLabel.Text = "Deleting:";
            textToolStripStatusLabel.Text = "Prepare...";

            if (isList) {
                new WaitForm(this).RemoveFile(filesListView.SelectedItems);
                FindFiles(currentDat, currentNode);
            } else {
                new WaitForm(this).RemoveFile(currentNode.Name);
                folderTreeView.Nodes.Remove(currentNode);
            }

            textToolStripStatusLabel.Text = toolStripProgressBar.Value + " file(s)";
            toolStripProgressBar.Value = 0;

            var dat = ControlDat.GetDat(currentDat);
            totalToolStripStatusLabel.Text = dat.TotalFiles.ToString();

            if (dat.ShouldSave()) SaveToolStripButton.Enabled = true;
        }

        internal void DeleteFiles(string path)
        {
            List<String> listFiles = new List<String>();
            GetFolderFiles(listFiles, path);

            OpenDat dat = ControlDat.GetDat(currentDat);

            if (listFiles.Count > 0) {
                toolStripProgressBar.Maximum = listFiles.Count;
                dat.DeleteFile(listFiles, true);
            } else {
                dat.RemoveEmptyFolder(path);
            }
        }

        internal void DeleteFiles(ListView.SelectedListViewItemCollection listPath)
        {
            List<String> listFiles = new List<String>();
            foreach (ListViewItem item in listPath)
            {
                if (item.Tag == null) { // remove folder and all files in sub folders
                    GetFolderFiles(listFiles, item.Name);
                } else {
                    var file = (sFile)item.Tag;
                    listFiles.Add(file.path);
                }
            }

            if (listFiles.Count > 0) {
                toolStripProgressBar.Maximum = listFiles.Count;
                ControlDat.GetDat(currentDat).DeleteFile(listFiles, false);
            }
        }

        private void assosToolStripButton_Click(object sender, EventArgs e)
        {
            FileAssociation.Associate(true);
        }

        private bool CheckElement(int i)
        {
            if (filesListView.Items[i].Text.IndexOf(stbFindFile.Text, StringComparison.OrdinalIgnoreCase) != -1) {
                for (int j = 0; j < filesListView.SelectedIndices.Count; j++)
                {
                    int s = filesListView.SelectedIndices[j];
                    filesListView.Items[s].Selected = false;
                }
                filesListView.Items[i].Selected = true;
                filesListView.Items[i].EnsureVisible();
                return true;
            }
            return false;
        }

        private void stbFindFile_KeyPress(object sender, KeyPressEventArgs e)
        {
            stbFindFile.BackColor = SystemColors.Window;
            if ((e.KeyChar != 13  && e.KeyChar != 10) || stbFindFile.Text.Length == 0 || filesListView.Items.Count == 0) return;

            if (e.KeyChar == 10) { // back
                int start = (filesListView.SelectedIndices.Count != 0) ? filesListView.SelectedIndices[filesListView.SelectedIndices.Count - 1] : filesListView.Items.Count;
                for (int i = start - 1; i > 0;)
                {
                    if (CheckElement(--i)) {
                        e.Handled = true;
                        return;
                    }
                }
            } else {
                int start = (filesListView.SelectedIndices.Count != 0) ? filesListView.SelectedIndices[filesListView.SelectedIndices.Count - 1] + 1 : 0;
                for (int i = start; i < filesListView.Items.Count; i++)
                {
                    if (CheckElement(i)) {
                        e.Handled = true;
                        return;
                    }
                }
            }
            stbFindFile.BackColor = Color.MistyRose;
        }

        private void tsBtnSearch_Click(object sender, EventArgs e)
        {
            if (currentDat == null) return;

            if (stbFindFile.Text.Length <= 1) {
                stbFindFile.BackColor = Color.MistyRose;
                return;
            }

            List<sFile> list = ControlDat.GetDat(currentDat).FindFilesByPattern(stbFindFile.Text);
            if (list.Count == 0) {
                stbFindFile.BackColor = Color.MistyRose;
                return;
            }
            FindFilesListForm listForm = new FindFilesListForm(list);
            listForm.GotoFile += GotoListFile;
            listForm.Show(this);
        }

        private void GotoListFile(sFile file)
        {
            TreeNode node = Misc.FindPathNode(file.file.pathTree.ToLowerInvariant(), Misc.GetRootNode(currentNode));
            if (node != null) {
                folderTreeView.SelectedNode = node;
                if (currentNode != null) currentNode.ForeColor = Color.White;
                currentNode = folderTreeView.SelectedNode;
                currentNode.ForeColor = Color.Yellow;

                FindFiles(currentDat, node);

                foreach (ListViewItem item in filesListView.Items)
                {
                    if (item.Text == file.file.name) {
                        item.Selected = true;
                        //item.Focused = true;
                        item.EnsureVisible();
                        break;
                    }
                }
                upToolStripButton.Enabled = (currentNode.Parent != null);
            }
        }

        private void infoToolStripButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/FakelsHub/DatExplorer-II");
        }
    }
}
