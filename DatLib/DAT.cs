using System;
using System.Collections.Generic;
using System.IO;

using DATLib.FO1;

namespace DATLib
{
    // This is a helper class for handling multiple DAT files.
    // https://fallout.fandom.com/wiki/DAT_file

    public class DAT
    {
        internal BinaryReader br { get; set; }

        public String DatFileName { get; set;}

        internal List<DATFile> FileList { get; set; }

        internal int  FilesTotal { get; set; }

        // only for Fallout 2 DAT
        internal uint FileSizeFromDat { get; set; }
        internal int  TreeSize { get; set; }

        // only for Fallout 1 DAT
        internal int DirCount { get; set; }

        public bool IsFallout2Type
        {
            get { return DirCount == -1; }
        }

        internal void Close()
        {
            br.Close();
        }

        //private int CountChar(string s, char c)
        //{
        //    int count = 0;
        //    foreach (char ch in s) {
        //        if (ch == c) count++;
        //    }
        //    return count;
        //}

        //public List<DATFile> GetFilesByPattern(string pattern)
        //{

        //    List<DATFile> Files = new List<DATFile>();
        //    foreach (DATFile file in FileList)
        //    {
        //        if (file.FilePath.Contains(pattern) && ((CountChar(file.FilePath, '\\') - 1 == CountChar(pattern, '\\'))))
        //            Files.Add(file);
        //    }
        //    return Files;
        //}

        /// <summary>
        /// Возвращает файл находящийся в DAT по его пути
        /// </summary>
        /// <param name="fileName">Путь и имя файла</param>
        public DATFile GetFileByName(string fileName)
        {
            foreach (DATFile file in FileList) {
                if (!file.IsDeleted && file.FilePath == fileName) return file;
            }
            return null;
        }

        #if SaveBuild
        public enum FalloutType { Fallout1 = 1, Fallout2 = 2 }

        public int  AddedFiles { get; internal set; }

        public DAT() {}

        public DAT(string datFile, FalloutType type)
        {
            br = new BinaryReader(File.Open(datFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None));

            DatFileName = datFile;
            DirCount = (type == FalloutType.Fallout2) ? -1 : 0;
            FileList = new List<DATFile>();
        }

        public void AddFile(string realFile, FileInfo virtualFile)
        {
            DATFile file = (IsFallout2Type) ? new DATFile() : new DAT1File();

            file.Path = virtualFile.pathTree;
            file.FileName = virtualFile.name;
            file.FilePath = (virtualFile.pathTree + virtualFile.name).ToLowerInvariant();

            file.FileNameSize = System.Text.ASCIIEncoding.ASCII.GetByteCount(file.FilePath);

            file.RealFile = realFile;

            file.UnpackedSize = virtualFile.info.Size;
            file.PackedSize = -1;
            file.Compression = false;
            FileList.Add(file);

            FilesTotal++;
            AddedFiles++;
        }

        public bool RemoveFiles(List<string> filesList)
        {
            FilesTotal -= filesList.Count;

            bool realDeleted = false;

            for (int i = 0; i < FileList.Count; i++)
            {
                if (FileList[i].IsDeleted) continue;

                for (int j = 0; j < filesList.Count; j++)
                {
                    if (FileList[i].FilePath == filesList[j]) {
                        if (FileList[i].IsVirtual) {
                            FileList.RemoveAt(i--);
                            AddedFiles--;
                        } else {
                            FileList[i].IsDeleted = true;
                            realDeleted = true;
                        }
                        OnRemove(filesList[j]);
                        filesList.RemoveAt(j);
                        break;
                    }
                }
                if (filesList.Count == 0) break;
            }
            return realDeleted;
        }

        public void RenameFile(string filePath, string newName)
        {
            GetFileByName(filePath).Rename(newName);
        }

        public static event RemoveFileEvent RemoveFile;
        public static event WriteFileEvent  SavingFile;

        internal static void OnRemove(string file)
        {
            if (RemoveFile != null) {
                RemoveFile(new FileEventArgs(file));
            }
        }

        internal static void OnWrite(string file)
        {
            if (SavingFile != null) {
                SavingFile(new FileEventArgs(file));
            }
        }
        #endif

        #region Event
        public static event ExtractFileEvent ExtractUpdate;

        internal static void OnExtracted(string file, bool result)
        {
            if (ExtractUpdate != null) {
                ExtractUpdate(new ExtractEventArgs(file, result));
            }
        }
        #endregion
    }
}
