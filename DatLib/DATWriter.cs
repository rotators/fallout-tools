using System;
using System.Collections.Generic;
using System.IO;

namespace DATLib
{
    #if SaveBuild

    public delegate void WriteFileEvent(FileEventArgs e);
    public delegate void RemoveFileEvent(FileEventArgs e);

    public class FileEventArgs
    {
        protected string file;

        public string File { get { return file; } }

        public FileEventArgs(string file)
        {
            this.file = file;
        }
    }

    internal static class DATWriter
    {
        // для исправления ошибки MS при сортировке с символом подчеркивания '_'
        static public int FilePathCompare(DATFile a, DATFile b)
        {
            int len = Math.Min(a.FilePath.Length, b.FilePath.Length);
            for (int i = 0; i < len; i++)
            {
                if (a.FilePath[i] == b.FilePath[i]) continue;

                return (a.FilePath[i] > b.FilePath[i]) ? 1 : -1;
            }
            return 0;
        }

        #region Fallout 1 dat format

        private static SortedDictionary<string, List<DATFile>> BuildDict(DAT dat)
        {
            SortedDictionary<string, List<DATFile>> data = new SortedDictionary<string, List<DATFile>>();

            foreach (var file in dat.FileList)
            {
                string dir = file.Path.TrimEnd('\\').ToUpperInvariant();
                if (dir == String.Empty) dir = ".";

                if (!data.ContainsKey(dir)) data.Add(dir, new List<DATFile>());
                data[dir].Add(file);
            }
            dat.DirCount = data.Count;

            return data;
        }

        private static void UpdateFileData(SortedDictionary<string, List<DATFile>> data, WBinaryBigEndian bw)
        {
            foreach (var files in data.Values)
            {
                bw.BaseStream.Position += 16;

                foreach (var file in files)
                {
                    bw.BaseStream.Position += file.FileName.Length + 1;
                    bw.WriteInt32BE((file.Compression) ? 0x40 : 0x20);
                    bw.WriteUInt32BE(file.Offset);
                    bw.WriteInt32BE(file.UnpackedSize);
                    bw.WriteInt32BE(file.PackedSize);
                }
            }
        }

        internal static void FO1_BuildDat(DAT dat)
        {
            bool hasVirtual = false;
            int countFiles = dat.FileList.Count;

            //dat.FileList.Sort((a, b) => a.FilePath.CompareTo(b.FilePath)); // сортировка для движка
            dat.FileList.Sort(FilePathCompare);

            for (int i = 0; i < countFiles; i++)
            {
                if (dat.FileList[i].IsVirtual) hasVirtual = true;
                if (!dat.FileList[i].IsDeleted) continue;

                dat.FileList.RemoveAt(i--);
                countFiles--;
            }

            SortedDictionary<string, List<DATFile>> data = BuildDict(dat);

            WBinaryBigEndian bw = new WBinaryBigEndian(File.Open(dat.DatFileName + ".tmp", FileMode.Create, FileAccess.Write));

            bool mod = (dat.FilesTotal > 1000);

            bw.WriteInt32BE(dat.DirCount);

            // Unknown fields
            bw.WriteInt32BE(dat.DirCount);
            bw.Write((UInt64)0); // 8-bytes

            // Write dirs
            foreach (var dir in data.Keys)
            {
                bw.Write((Byte)dir.Length);
                bw.Write(dir.ToCharArray()); // write in upper case
            }

            int startFileDataAddr = (Int32)bw.BaseStream.Position;

            // Write files data
            foreach (var files in data.Values)
            {
                bw.WriteInt32BE(files.Count);

                // Unknown fields
                bw.WriteInt32BE(files.Count);
                bw.WriteInt32BE(16); // 0x10
                bw.WriteInt32BE(0);

                foreach (var file in files)
                {
                    bw.Write((Byte)file.FileName.Length);
                    bw.Write(file.FileName.ToCharArray());
                    bw.BaseStream.Position += 16;
                }
            }

            // key offset => value index
            List<KeyValuePair<UInt32, int>> list = new List<KeyValuePair<UInt32, int>>();
            for (int i = 0; i < countFiles; i++)
            {
                if (!dat.FileList[i].IsVirtual) list.Add(new KeyValuePair<UInt32, int>(dat.FileList[i].Offset, i));
            }
            // сортируем по значению offset
            list.Sort((x, y) => x.Key.CompareTo(y.Key));

            // Copy and write files content from source dat
            foreach (var item in list)
            {
                int i = item.Value;

                if (!mod || dat.FileList[i].UnpackedSize > 10485760 || i % 5 == 0) DAT.OnWrite(dat.FileList[i].FilePath);

                UInt32 offset = (UInt32)bw.BaseStream.Position;
                bw.Write(dat.FileList[i].GetDirectFileData());
                dat.FileList[i].Offset = offset;
            }

            // Write virtual files content to saving dat
            if (hasVirtual) {
                for (int i = 0; i < countFiles; i++)
                {
                    if (dat.FileList[i].IsVirtual) {
                        if (!mod || dat.FileList[i].UnpackedSize > 1048576 || i % 5 == 0) DAT.OnWrite(dat.FileList[i].FilePath);

                        dat.FileList[i].Offset = (UInt32)bw.BaseStream.Position;
                        bw.Write(dat.FileList[i].GetCompressedData(), 0, dat.FileList[i].UnpackedSize);
                    }
                }
            }
            bw.Seek(startFileDataAddr, SeekOrigin.Begin);

            DAT.OnWrite("Finishing...");

            UpdateFileData(data, bw);

            bw.Flush();
            bw.Close();
            dat.br.Close();

            File.Delete(dat.DatFileName);
            File.Move(dat.DatFileName + ".tmp", dat.DatFileName);

            RBinaryBigEndian br = new RBinaryBigEndian(File.Open(dat.DatFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
            dat.br = br;
            for (int i = 0; i < countFiles; i++) dat.FileList[i].br = br;
        }

        #endregion

        #region Fallout 2 dat format

        private static void WriteDirTreeSub(DAT dat, BinaryWriter bw)
        {
            UInt32 startDirTreeAddr = (UInt32)bw.BaseStream.Position;

            //dat.FileList.Sort((a, b) => a.FilePath.CompareTo(b.FilePath)); // сортировка для движка
            dat.FileList.Sort(FilePathCompare);

            // Write DirTree
            for (int i = 0; i < dat.FilesTotal; i++) {
                bw.Write(dat.FileList[i].FileNameSize);
                bw.Write((dat.FileList[i].Path + dat.FileList[i].FileName).ToCharArray());
                bw.Write(dat.FileList[i].Compression);
                bw.Write(dat.FileList[i].UnpackedSize);
                bw.Write(dat.FileList[i].PackedSize);
                bw.Write(dat.FileList[i].Offset);
            }
            // TreeSize
            dat.TreeSize = (Int32)((UInt32)bw.BaseStream.Position - startDirTreeAddr + 4);
            bw.Write(dat.TreeSize);

            // DatSize
            UInt32 datSize = (UInt32)bw.BaseStream.Position + 4;
            bw.Write(datSize);

            if (dat.FileSizeFromDat > datSize && datSize != bw.BaseStream.Length) bw.BaseStream.SetLength(datSize); // truncate
            bw.Flush();

            dat.FileSizeFromDat = datSize;
        }

        private static void WriteAppendFilesData(DAT dat, BinaryWriter bw)
        {
            bool mod = (dat.AddedFiles > 1000);
            dat.AddedFiles = 0;

            int count = dat.FileList.Count;

            for (int i = 0; i < count; i++)
            {
                var file = dat.FileList[i];
                if (!file.IsVirtual) continue;

                if (!mod || file.UnpackedSize > 1048576 || i % 5 == 0) DAT.OnWrite(file.FilePath);

                file.Offset = (UInt32)bw.BaseStream.Position;

                bw.Write(file.GetCompressedData(), 0, file.PackedSize);
                file.br = dat.br;
            }
        }

        internal static void WriteDirTree(DAT dat)
        {
            BinaryWriter bw = new BinaryWriter(dat.br.BaseStream);

            bw.BaseStream.Seek(-(dat.TreeSize + 4), SeekOrigin.End);

            WriteDirTreeSub(dat, bw);
        }

        internal static void WriteAppendFilesDat(DAT dat)
        {
            BinaryWriter bw = new BinaryWriter(dat.br.BaseStream);

            bw.BaseStream.Seek(-(dat.TreeSize), SeekOrigin.End); // позиция FilesTotal

            WriteAppendFilesData(dat, bw);
            bw.Write(dat.FilesTotal);

            DAT.OnWrite("Finishing...");

            WriteDirTreeSub(dat, bw);
        }

        // Create new dat
        internal static void FO2_BuildDat(DAT dat)
        {
            BinaryWriter bw = new BinaryWriter(File.Open(dat.DatFileName + ".tmp", FileMode.Create, FileAccess.Write));

            bool mod = (dat.FilesTotal > 1000);
            int count = dat.FileList.Count;

            // Write DataBlock
            for (int i = 0; i < count; i++)
            {
                if (dat.FileList[i].IsVirtual) continue;

                if (!dat.FileList[i].IsDeleted) {
                    if (!mod || dat.FileList[i].UnpackedSize > 10485760 || i % 5 == 0) DAT.OnWrite(dat.FileList[i].FilePath);

                    UInt32 offset = (UInt32)bw.BaseStream.Position;

                    bw.Write(dat.FileList[i].GetDirectFileData());
                    dat.FileList[i].Offset = offset;
                } else {
                    dat.FileList.RemoveAt(i--); // remove deleted file from list
                    count--;
                }
            }
            WriteAppendFilesData(dat, bw); // write virtual files data

            DAT.OnWrite("Finishing...");

            bw.Write(dat.FilesTotal);

            WriteDirTreeSub(dat, bw);

            bw.Close();
            dat.br.Close();

            File.Delete(dat.DatFileName);
            File.Move(dat.DatFileName + ".tmp", dat.DatFileName);

            BinaryReader br = new BinaryReader(File.Open(dat.DatFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
            dat.br = br;
            for (int i = 0; i < count; i++) dat.FileList[i].br = br;
        }

        #endregion

        //public static void TruncateDat(DAT dat, long size)
        //{
        //    using (var file = File.Open(dat.DatFileName, FileMode.Open, FileAccess.ReadWrite)) {
        //        file.SetLength(file.Length - size);
        //    }
        //}
    }
    #endif
}
