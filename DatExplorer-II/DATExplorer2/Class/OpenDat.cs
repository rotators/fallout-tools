using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using DATLib;

namespace DATExplorer
{
    internal static class ControlDat
    {
        static List<OpenDat> openDat = new List<OpenDat>();

        static internal List<OpenDat> OpenDat()
        {
            return openDat;
        }

        static internal OpenDat OpeningDat(string pathDat, bool create = false)
        {
            OpenDat dat = new OpenDat(pathDat, (!create) ? DATManage.GetFiles(pathDat) : new Dictionary<string, DATLib.FileInfo>());
            OpenDat().Add(dat);
            return dat;
        }

        static internal OpenDat GetDat(string datName)
        {
            return openDat.Find(x => x.DatName.Equals(datName, StringComparison.OrdinalIgnoreCase));
        }

        static internal bool DatIsOpen(string datName)
        {
            return openDat.Exists(x => x.DatName.Equals(datName, StringComparison.OrdinalIgnoreCase));
        }

        static internal void CloseDat(string datName)
        {
            OpenDat dat = GetDat(datName);
            dat.CloseDat();
            openDat.Remove(dat);
        }
    }

    internal class OpenDat
    {
        [Flags]
        enum SaveType {
            None      = 0x0,
            UpdateDir = 0x1,
            Append    = 0x2,
            Full      = 0x4,
        }

        private string datFile;
        private DAT dat;

        // key - путь к папке в нижнем регистре (без разделеителя в начале)
        private SortedDictionary<String, TreeFiles> treeFiles;

        private SaveType shouldSave = SaveType.None; // указывает, что данные изменились и требуется сохранение

        public int TotalFiles { set; get; }

        public int AddedFiles { get { return dat.AddedFiles; } }

        public SortedDictionary<String, TreeFiles> Folders { get { return treeFiles; } }

        public string DatName { get { return datFile; } }

        public bool ShouldSave() { return shouldSave != SaveType.None; }

        public bool IsFO2Type() { return dat.IsFallout2Type; }

        internal OpenDat(string datFile, Dictionary<String, DATLib.FileInfo> files)
        {
            this.datFile = datFile;
            dat = DATManage.GetDat(datFile);
            TotalFiles = files.Count();
            treeFiles = new SortedDictionary<string, TreeFiles>();
            BuildFolderTree(files);
        }

        internal void CloseDat()
        {
            DATManage.CloseDatFile(datFile);
        }

        private void BuildFolderTree(Dictionary<String, DATLib.FileInfo> files)
        {
            foreach (var item in files)
            {
                string pathfile = item.Value.pathTree.ToLowerInvariant();
                if (!treeFiles.ContainsKey(pathfile)) {
                    treeFiles.Add(pathfile, new TreeFiles(item.Value.pathTree));
                }
                treeFiles[pathfile].AddFile(item);
            }
            files.Clear();
        }

        private void UpdateTreeFiles()
        {
            foreach (var item in treeFiles)
            {
                for (int i = 0; i < item.Value.GetFiles().Count; i++)
                {
                    var file = item.Value.GetFiles()[i];
                    if (file.file.info.PackedSize == -1) {
                        item.Value.UpdateFileInfo(i, DATManage.GetFileInfo(datFile, file.path));
                    }
                }
            }
        }

        internal void GetFilesFromFolder(List<String> listFiles, string folderName, bool includeSubDirs = true)
        {
            foreach (var folder in Folders.Keys)
            {
                if (includeSubDirs) {
                    if (!folder.StartsWith(folderName)) {
                         continue;
                    }
                } else if (folder != folderName) continue;

                foreach (var file in Folders[folder].GetFiles())
                {
                    listFiles.Add(file.path);
                }
            }
        }

        internal bool FolderExist(string folderPath, bool ignoreCase = true)
        {
            int len = folderPath.Length;
            if (ignoreCase) folderPath = folderPath.ToLowerInvariant();

            foreach (var folder in treeFiles.Keys)
            {
                if (folder.StartsWith(folderPath)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Создает пустую дерикторию
        /// </summary>
        /// <param name="folderPath">Путь и имя дериктории</param>
        internal void AddEmptyFolder(string folderNamePath)
        {
            string dirPath = folderNamePath.ToLowerInvariant();
            if (!treeFiles.ContainsKey(dirPath)) {
                treeFiles.Add(dirPath, new TreeFiles(folderNamePath));
            } else {
                // error add
            }
        }

        internal bool OverwriteAll { set; get; }

        internal void AddVirtualFile(string realPathFile, string treeFolderPath)
        {
            string folderPath = treeFolderPath.ToLowerInvariant();
            string fileName = Path.GetFileName(realPathFile);

            bool folderExist = treeFiles.ContainsKey(folderPath);
            if (folderExist && treeFiles[folderPath].FileExist(fileName)) {

                DialogResult result = (OverwriteAll) ? DialogResult.Retry
                                                     : new CustomMessageBox(String.Format((ExplorerForm.LocaleRU)
                                                                             ? "Файл: {0}{1} уже существует!\nПерезаписать файл?"
                                                                             : "File: {0}{1} already exist!\nOverwrite file?",
                                                                             folderPath, fileName)).ShowDialog();
                switch (result) {
                    case DialogResult.Retry: // all
                    case DialogResult.Yes:
                        OverwriteAll = (result == DialogResult.Retry);
                        var list = new List<string>();
                        list.Add(folderPath + fileName.ToLowerInvariant());
                        DeleteFile(list, false);
                        break;
                    default:
                        return; // не добавляем дубликаты
                }
            }

            System.IO.FileInfo file = new System.IO.FileInfo(realPathFile);

            DATLib.FileInfo fileDat = new DATLib.FileInfo();
            fileDat.name = file.Name;
            fileDat.info.Size = (int)file.Length;
            fileDat.info.PackedSize = -1;
            fileDat.pathTree = treeFolderPath;

            dat.AddFile(realPathFile, fileDat);

            if (!folderExist) {
                treeFiles.Add(folderPath, new TreeFiles(treeFolderPath));
            }

            treeFiles[folderPath].AddFile(new KeyValuePair<string, DATLib.FileInfo>(folderPath + fileDat.name.ToLowerInvariant(), fileDat));

            TotalFiles++;

            /*if (shouldSave != SaveType.Full)*/ shouldSave |= SaveType.Append;
        }

        internal sFile RenameFile(string pathFile, string newName)
        {
            sFile file = new sFile();
            int i = pathFile.LastIndexOf('\\') + 1;
            string folderFile = pathFile.Remove(i);

            foreach (var folder in treeFiles.Keys)
            {
                if (folderFile == folder) {
                    file = treeFiles[folder].RenameFile(pathFile, newName);
                    break;
                }
            }
            dat.RenameFile(pathFile, newName);
            if (shouldSave == SaveType.None) shouldSave = SaveType.UpdateDir;
            return file;
        }

        internal string RenameFolder(string pathFolder, string newNameFolder)
        {
            int pLen = pathFolder.Length;
            int sLen = pLen - 1;
            int last = pathFolder.LastIndexOf('\\', sLen - 1) + 1;

            string newPath = pathFolder.Remove(last) + newNameFolder.ToLowerInvariant() + '\\';

            List<string> removeKeys = new List<string>();
            SortedDictionary<String, TreeFiles> addFiles = new SortedDictionary<string,TreeFiles>();

            foreach (var item in treeFiles.Keys)
            {
                if (item.StartsWith(pathFolder)) {
                    string newPathKey = newPath + item.Substring(pLen);

                    string folder = treeFiles[item].FolderPath;
                    string pre = folder.Remove(last) + newNameFolder;
                    TreeFiles tFiles = new TreeFiles(pre + folder.Substring(sLen));

                    foreach (var file in treeFiles[item].GetFiles())
                    {
                        tFiles.AddFile(new sFile(file, tFiles.FolderPath));
                    }
                    removeKeys.Add(item);
                    addFiles.Add(newPathKey, tFiles);
                }
            }
            if (addFiles.Count == 0) return null; // внесение изменений в дат не требуется

            foreach (var item in removeKeys) treeFiles.Remove(item);
            foreach (var pair in addFiles) treeFiles.Add(pair.Key, pair.Value);

            DATManage.RenameFolder(DatName, pathFolder, newNameFolder);

            if (shouldSave == SaveType.None) shouldSave = SaveType.UpdateDir;
            return newPath;
        }

        internal void DeleteFile(List<string> pathFileList, bool alsoFolder)
        {
            // удаление файлов из дерева (остаются только пустые папки если в них нет файлов)
            string folder = null;
            TreeFiles files = null;

            List<string> folders = new List<string>();
            for (int i = 0; i < pathFileList.Count; i++)
            {
                string file = pathFileList[i];
                int n = file.LastIndexOf('\\') + 1;

                if (folder == null) {
                    folder = file.Remove(n);
                    files = treeFiles[folder];
                    folders.Add(folder);
                }

                string f = file.Remove(n);
                if (f != folder) {
                    folder = f;
                    files = treeFiles[folder];
                    folders.Add(folder);
                }
                files.RemoveFile(file);
            }
            if (alsoFolder) {
                foreach (var f in folders) RemoveEmptyFolder(f);
            }

            TotalFiles -= pathFileList.Count;

            // удаление файлов из Dat
            if (dat.RemoveFiles(pathFileList)) shouldSave |= SaveType.Full;
        }

        internal void RemoveEmptyFolder(string folder)
        {
            if (treeFiles[folder].GetFiles().Count == 0) treeFiles.Remove(folder);
        }

        internal bool SaveDat(bool quick)
        {
            if (quick && shouldSave == SaveType.Full) { // if only deleting
                shouldSave = SaveType.UpdateDir;
            }

            if (shouldSave == SaveType.UpdateDir) {
                DATManage.SaveDirectoryStructure(DatName);
            }
            else if ((shouldSave & (SaveType.Full | SaveType.Append)) != 0) {
                bool append = quick || shouldSave == SaveType.Append;

                new WaitForm(ExplorerForm.ActiveForm).SaveDat(DatName, append);
                UpdateTreeFiles();
            }

            bool refresh = (shouldSave != SaveType.UpdateDir);
            shouldSave = SaveType.None;

            return refresh;
        }

        internal DATFile GetFile(string fileName)
        {
            return dat.GetFileByName(fileName);
        }

        internal List<sFile> FindFilesByPattern(string pattern)
        {
            List<sFile> list = new List<sFile>();

            foreach (var folder in Folders)
            {
                foreach (var file in folder.Value.GetFiles())
                {
                    if (file.file.name.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) != -1) {
                        list.Add(file);
                    }
                }
            }
            return list;
        }
    }
}
