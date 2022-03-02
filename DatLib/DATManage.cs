using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DATLib
{
    public struct Info
    {
        public int Size;
        public int PackedSize; // -1 - для файла находящегося вне dat
        public bool IsPacked;
    }

    public struct FileInfo
    {
        public string pathTree; // in case, в конце содержит разделитель пути '\'
        public string name;     // in case
        public Info info;
    }

    public static class DATManage
    {
        static private List<DAT> openDat = new List<DAT>();
        private static string lastCheckFolder;

        private static void UnsetCheckFolder()
        {
            lastCheckFolder = null;
        }

        public static bool OpenDatFile(string datFile, out string eMessage)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) {
                    eMessage = string.Empty;
                    return true; // DAT file is already open
                }
            }
            DatReaderError err;
            DAT datData = DATReader.ReadDat(datFile, out err);
            if (datData != null) {
                datData.FileList = DATReader.ReadFilesData(datData);
                openDat.Add(datData);
            }
            eMessage = err.Message;
            return (err.Error == DatError.Success);
        }

        private static bool SaveExtractFile(string filePath, DATFile datfile)
        {
            byte[] data = datfile.GetFileData();
            if (data == null) return false;

            if (lastCheckFolder == null || lastCheckFolder != datfile.Path) {
                string path = Path.GetFullPath(Path.GetDirectoryName(filePath));
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                lastCheckFolder = datfile.Path;
            }
            File.WriteAllBytes(filePath, data);
            return true;
        }

        /// <summary>
        /// Раскаковывает все файлы из DAT
        /// </summary>
        /// <param name="unpackPath"></param>
        /// <param name="datFile"></param>
        /// <returns></returns>
        public static bool ExtractAllFiles(string unpackPath, string datFile)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) {
                    UnsetCheckFolder();
                    foreach (DATFile datfile in dat.FileList)
                    {
                        DAT.OnExtracted(datfile.FilePath, true);
                        SaveExtractFile(unpackPath + datfile.Path + datfile.FileName, datfile);
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Распаковывает все файлы из указанной папки DAT c сохранением структуры каталогов
        /// </summary>
        /// <param name="unpackPath">Путь куда необходимо распаковать</param>
        /// <param name="datFolder">Папка DAT которую необходимо распаковать</param>
        /// <param name="datFile"></param>
        /// <returns></returns>
        //public static bool ExtractFolder(string unpackPath, string datFolder, string datFile)
        //{
        //    foreach (DAT dat in openDat)
        //    {
        //        if (dat.DatFileName == datFile) {
        //            UnsetCheckFolder();

        //            // TODO

        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public static bool ExtractFile(string unpackPath, string file, string datFile)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) {
                    UnsetCheckFolder();
                    DATFile datfile = DATReader.GetFile(dat, file);
                    return (datfile != null) ? SaveExtractFile(unpackPath + datfile.Path + datfile.FileName, datfile) : false;
                }
            }
            return false;
        }

        public static bool ExtractFileList(string unpackPath, string[] files, string datFile)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) {
                    UnsetCheckFolder();
                    foreach (var file in files)
                    {
                        DATFile datfile = DATReader.GetFile(dat, file);

                        DAT.OnExtracted(file, datfile != null);
                        if (datfile == null) continue;

                        SaveExtractFile(unpackPath + datfile.Path + datfile.FileName, datfile);
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Распаковывает список файлов в указанной папку
        /// </summary>
        /// <param name="unpackPath">Папка назначения</param>
        /// <param name="files">Список файлов</param>
        /// <param name="datFile"></param>
        /// <param name="cutoffPath"></param>
        /// <returns></returns>
        public static bool ExtractFileList(string unpackPath, string[] files, string datFile, string cutoffPath)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) {
                    UnsetCheckFolder();

                    int len = cutoffPath.Length;

                    foreach (var file in files)
                    {
                        DATFile datfile = DATReader.GetFile(dat, file);

                        DAT.OnExtracted(file, datfile != null);
                        if (datfile == null) continue;

                        string unpackedPathFile = unpackPath + datfile.Path.Substring(len) + datfile.FileName;
                        SaveExtractFile(unpackedPathFile, datfile);
                    }
                    return true;
                }
            }
            return false;
        }

        public static bool FileExists(string filePath, string datFile)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) return (DATReader.GetFile(dat, filePath.TrimStart('\\')) != null);
            }
            return false;
        }

        /// <summary>
        /// Получает массив данных файла.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="datFile"></param>
        /// <returns></returns>
        public static byte[] GetFileData(string filePath, string datFile)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) {
                    UnsetCheckFolder();
                    DATFile datfile = DATReader.GetFile(dat, filePath);
                    return (datfile != null) ? datfile.GetFile() : null;
                }
            }
            return null;
        }

        public static Dictionary<String, String> GetFileList(string datFile)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) {
                    Dictionary<String, String> fileList = new Dictionary<String, String>();
                    foreach (var file in dat.FileList)
                    {
                        fileList.Add(file.FilePath, file.FileName);
                    }
                    return fileList;
                }
            }
            return null;
        }

        public static Dictionary<String, FileInfo> GetFiles(string datFile)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) {
                    Dictionary<String, FileInfo> fileList = new Dictionary<String, FileInfo>();
                    foreach (var file in dat.FileList)
                    {
                        FileInfo _file = new FileInfo();
                        _file.name            = file.FileName;
                        _file.info.Size       = file.UnpackedSize;
                        _file.info.IsPacked   = file.Compression;
                        _file.info.PackedSize = file.PackedSize;
                        _file.pathTree        = file.Path;
                        fileList.Add(file.FilePath, _file);
                    }
                    return fileList;
                }
            }
            return null;
        }

        public static long GetFileTotal(string datFile)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) return dat.FilesTotal;
            }
            return 0;
        }

        public static Info GetFileInfo(string datFile, string file)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) {
                    DATFile _file = dat.GetFileByName(file);
                    return new Info() {
                        Size       = _file.UnpackedSize,
                        PackedSize = _file.PackedSize,
                        IsPacked   = _file.Compression
                    };
                }
            }
            return new Info();
        }

        public static DAT GetDat(string datFile)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) return dat;
            }
            return null;
        }

        public static void CloseDatFile(string datFile)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) {
                    dat.Close();
                    openDat.Remove(dat);
                    break;
                }
            }
        }

        public static void CloseAllDatFiles()
        {
            foreach (DAT dat in openDat) dat.Close();
            openDat.Clear();
        }

        #region Save
        #if SaveBuild

        public static void CreateDatFile(string datFile, DAT.FalloutType type)
        {
            DAT datData = new DAT(datFile, type);
            openDat.Add(datData);
        }

        // Преименовывает пути к файлам (без сохранения в dat)
        public static void RenameFolder(string datFile, string oldFolder, string newFolder)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) {
                    int stopLen = oldFolder.Length - 1;
                    int last = oldFolder.LastIndexOf('\\', stopLen - 1) + 1;
                    foreach (var file in dat.FileList)
                    {
                        if (file.FilePath.StartsWith(oldFolder)) {
                            file.Path = file.Path.Remove(last) + newFolder + file.Path.Substring(stopLen);
                            file.FilePath = (file.Path + file.FileName).ToLowerInvariant();
                            file.FileNameSize = file.FilePath.Length;
                        }
                    }
                }
            }
        }

        public static bool SaveDAT(string datFile)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) {
                    if (dat.IsFallout2Type)
                        DATWriter.FO2_BuildDat(dat);
                    else
                        DATWriter.FO1_BuildDat(dat);
                    return true;
                }
            }
            return false;
        }

        public static bool AppendFilesDAT(string datFile)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) {
                    if (dat.IsFallout2Type) {
                        DATWriter.WriteAppendFilesDat(dat);
                    } else {
                        DATWriter.FO1_BuildDat(dat);
                    }
                    return true;
                }
            }
            return false;
        }

        public static bool SaveDirectoryStructure(string datFile)
        {
            foreach (DAT dat in openDat)
            {
                if (dat.DatFileName == datFile) {
                    if (dat.IsFallout2Type) {
                        DATWriter.WriteDirTree(dat);
                    } else {
                        DATWriter.FO1_BuildDat(dat);
                    }
                    return true;
                }
            }
            return false;
        }

        #endif
        #endregion
    }
}
