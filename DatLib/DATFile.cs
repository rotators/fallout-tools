using System;
using System.IO;

namespace DATLib
{
    public class DATFile
    {
        protected static byte[] tempBuffer; // temp buffer for extracted file

        protected byte[] fileBuffer;

        internal BinaryReader br;

        /// <summary>
        /// Path and name in lower case
        /// </summary>
        internal String FilePath  { get; set; }

        /// <summary>
        /// File name with case letters
        /// </summary>
        internal String FileName  { get; set; }

        /// <summary>
        /// Only path to file
        /// </summary>
        internal String Path      { get; set; }

        /// <summary>
        /// Length of FilePath
        /// </summary>
        internal int  FileNameSize { get; set; }

        internal bool   Compression  { get; set; }
        internal Int32  UnpackedSize { get; set; }
        internal Int32  PackedSize   { get; set; }
        internal UInt32 Offset       { get; set; }

        internal string ErrorMsg  { get; set; }

        internal bool IsDeleted { get;  set; }                         // True - файл будет удален из DAT при сохранении

        #region Save
        #if SaveBuild

        public String RealFile  { get; set; } // path to file on disk

        internal bool IsVirtual { get { return PackedSize == -1; } }   // True - файл расположен вне DAT

        internal void Rename(string newName)
        {
            int l = FilePath.Length;
            FilePath = FilePath.Remove(l - FileName.Length) + newName.ToLowerInvariant();
            FileNameSize = FilePath.Length;
            FileName = newName;
        }

        private byte[] compressStream(FileStream file)
        {
            MemoryStream outStream = new MemoryStream();
            zlib.ZOutputStream outZStream = new zlib.ZOutputStream(outStream, zlib.zlibConst.Z_BEST_COMPRESSION);
            try
            {
                byte[] buffer = new byte[512];
                int len;
                while ((len = file.Read(buffer, 0, 512)) > 0)
                {
                    outZStream.Write(buffer, 0, len);
                }
                outZStream.finish();
                tempBuffer = outStream.ToArray();
            }
            finally
            {
                outZStream.Close();
                outStream.Close();
            }
            return tempBuffer;
        }

        internal virtual byte[] GetCompressedData()
        {
            if (RealFile == null) return null;

            using (FileStream file = new FileStream(RealFile, FileMode.Open, FileAccess.Read)) {
                byte[] data = compressStream(file);
                if (UnpackedSize <= data.Length) { // bad compressed, the compressed file size is greater or equal to the uncompressed file
                    PackedSize = UnpackedSize;
                    file.Position = 0;
                    file.Read(data, 0, UnpackedSize);
                } else {
                    PackedSize = data.Length;
                    Compression = true;
                }
                RealFile = null;
                return data;
            }
        }

        #endif
        #endregion

        protected virtual byte[] DecompressStream()
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (zlib.ZOutputStream outZStream = new zlib.ZOutputStream(outStream))
                {
                    byte[] buffer = new byte[512];
                    int len;
                    try
                    {
                        while ((len = br.Read(buffer, 0, 512)) > 0) outZStream.Write(buffer, 0, len);
                        outZStream.Flush();
                        fileBuffer = outStream.ToArray();
                    }
                    catch (zlib.ZStreamException ex)
                    {
                        ErrorMsg = ex.Message;
                        return null;
                    }
                    finally
                    {
                        outZStream.finish();
                    }
                }
            }
            return fileBuffer;
        }

        protected virtual byte[] DecompressData()
        {
            byte[] data;
            using (MemoryStream outStream = new MemoryStream())
            {
                using (zlib.ZOutputStream outZStream = new zlib.ZOutputStream(outStream))
                {
                    try
                    {
                        outZStream.Write(tempBuffer, 0, tempBuffer.Length);
                        outZStream.Flush();
                        data = outStream.ToArray();
                    }
                    catch(zlib.ZStreamException ex)
                    {
                        ErrorMsg = ex.Message;
                        return null;
                    }
                    finally
                    {
                        outZStream.finish();
                    }
                }
            }
            return data;
        }

        // Read whole file into a temp buffer
        internal byte[] GetFileData()
        {
            if (fileBuffer != null) return fileBuffer;

            if (br == null) return null;
            br.BaseStream.Seek(Offset, SeekOrigin.Begin);
            int size = (Compression) ? PackedSize : UnpackedSize;

            if (tempBuffer == null || size != tempBuffer.Length) tempBuffer = new byte[size];

            br.Read(tempBuffer, 0, size);
            return (Compression) ? DecompressData() : tempBuffer;
        }

        // Read whole file into a file buffer
        internal byte[] GetFile()
        {
            if (fileBuffer != null) return fileBuffer;

            if (br == null) return null;
            br.BaseStream.Seek(Offset, SeekOrigin.Begin);

            if (!Compression) {
                if (fileBuffer == null) fileBuffer = new byte[UnpackedSize];
                br.Read(fileBuffer, 0, UnpackedSize);
                return fileBuffer;
            }
            return DecompressStream();
        }

        // Read file content from dat
        internal byte[] GetDirectFileData()
        {
            if (br == null) return null;

            br.BaseStream.Seek(Offset, SeekOrigin.Begin);
            return br.ReadBytes((Compression) ? PackedSize : UnpackedSize);
        }
    }
}
