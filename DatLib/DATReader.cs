using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DATLib
{
    internal enum DatError
    {
        InvalidDAT,
        IOError,
        Success,
        WrongSize
    };

    internal class DatReaderError
    {
        internal DatReaderError(DatError error, string Message)
        {
            this.Error = error;
            this.Message = Message;
        }
        internal DatError Error { get; set; }
        internal string Message { get; set; }
    }

    internal static class DATReader
    {
        // Based on code by Dims
        internal static DAT ReadDat(string filename, out DatReaderError error)
        {
            if (String.IsNullOrEmpty(filename))
            {
                error = new DatReaderError(DatError.IOError, "Invalid DAT filename.");
                return null;
            }

            DAT dat = new DAT();
            dat.DatFileName = filename;
            dat.DirCount = -1;

            BinaryReader br = null;
            try
            {
                br = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
            }
            catch (IOException io)
            {
                error = new DatReaderError(DatError.IOError, io.Message);
                return null;
            }
            if (br.BaseStream.Length == 0) {
                error = new DatReaderError(DatError.WrongSize, "Dat file size is zero.");
                return null;
            }

            dat.br = br;
            br.BaseStream.Seek(-8, SeekOrigin.End);
            dat.TreeSize = br.ReadInt32();
            dat.FileSizeFromDat = br.ReadUInt32();

            // Check Dat version
            if (br.BaseStream.Length != dat.FileSizeFromDat)
            {
                br.BaseStream.Seek(0, SeekOrigin.Begin);
                dat.DirCount = ToLittleEndian(br.ReadInt32());
                if ((dat.DirCount & 0xFF00000) == 0) { // it's Fallout 1 dat ?
                    dat.TreeSize = 0;
                    dat.FileSizeFromDat = 0;
                    br.BaseStream.Position += 12; // unknown1, unknown2, unknown3 fields

                    error = new DatReaderError(DatError.Success, string.Empty);
                    return dat;
                }
                error = new DatReaderError(DatError.WrongSize, "Dat file size is incorrect.");
                return null;
            }

            br.BaseStream.Seek(-(dat.TreeSize + 8), SeekOrigin.End);
            dat.FilesTotal = br.ReadInt32();

            // Read DirTree data
            //byte[] buff = new byte[dat.TreeSize];
            //br.Read(buff, 0, (int)(dat.TreeSize - 4));

            error = new DatReaderError(DatError.Success, string.Empty);
            return dat;
        }

        internal static List<DATFile> ReadFilesData(DAT dat)
        {
            List<DATFile> DatFiles = new List<DATFile>();
            BinaryReader br = dat.br;

            if (dat.IsFallout2Type) {
                uint fileIndex = 0;
                br.BaseStream.Seek(-(dat.TreeSize + 4), SeekOrigin.End);
                while (fileIndex < dat.FilesTotal) {
                    DATFile file = new DATFile();
                    file.br = br;

                    file.FileNameSize = br.ReadInt32();

                    char[] namebuf = new Char[file.FileNameSize];
                    br.Read(namebuf, 0, (int)file.FileNameSize);
                    string pathFile = new String(namebuf, 0, namebuf.Length);

                    file.FileName = Path.GetFileName(pathFile);
                    file.FilePath = pathFile.ToLowerInvariant();
                    file.Path = pathFile.Remove(pathFile.LastIndexOf('\\') + 1);

                    file.Compression = (br.ReadByte() == 0x1);
                    file.UnpackedSize = br.ReadInt32();
                    file.PackedSize = br.ReadInt32();
                    file.Offset = br.ReadUInt32();

                    if (!file.Compression && (file.UnpackedSize != file.PackedSize)) file.Compression = true;

                    DatFiles.Add(file);
                    fileIndex++;
                }
            } else {
                // Implement FO1 dat support: https://falloutmods.fandom.com/wiki/DAT_file_format
                List<string> directories = new List<string>();
                for (var i = 0; i < dat.DirCount; i++)
                {
                    directories.Add(ReadString(br) + '\\');
                }
                if (directories[0] == ".\\") directories[0] = String.Empty;

                br = new RBinaryBigEndian(br.BaseStream);
                for (var i = 0; i < dat.DirCount; i++)
                {
                    var fileCount = br.ReadInt32();
                    br.BaseStream.Position += 12; // unknown4, unknown5, unknown6
                    for (var n = 0; n < fileCount; n++)
                    {
                        DAT1File file = new DAT1File();
                        file.br = br;
                        file.FileName = ReadString(br);
                        file.Compression = (((RBinaryBigEndian)br).ReadUInt() == 0x40000000);
                        file.Offset = br.ReadUInt32();
                        file.UnpackedSize = br.ReadInt32();
                        file.PackedSize = br.ReadInt32();
                        file.Path = directories[i];
                        file.FilePath = (directories[i] + file.FileName).ToLowerInvariant();

                        DatFiles.Add(file);
                    }
                }
                dat.FilesTotal = DatFiles.Count;
            }
            return DatFiles;
        }

        /// <summary>
        /// Возвращает файл находящийся в DAT по его пути, поиск осуществляется без чувствительности к регистру
        /// </summary>
        /// <param name="dat"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        internal static DATFile GetFile(DAT dat, string filename)
        {
            filename = filename.ToLowerInvariant();

            foreach (DATFile file in dat.FileList)
            {
                if (file.FilePath == filename) return file;
            }
            return null;
        }

        private static Int32 ToLittleEndian(Int32 value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            return BitConverter.ToInt32(temp, 0);
        }

        private static string ReadString(BinaryReader br)
        {
            var len = br.ReadByte();
            return Encoding.ASCII.GetString(br.ReadBytes(len));
        }
    }
}
