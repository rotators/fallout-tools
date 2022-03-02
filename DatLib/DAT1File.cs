using System.IO;
using DATLib.FO1;

namespace DATLib
{
    internal class DAT1File : DATFile
    {
        protected override byte[] DecompressStream()
        {
            RBinaryBigEndian bbr = new RBinaryBigEndian(base.br.BaseStream);
            //bbr.BaseStream.Seek(0, SeekOrigin.Begin);

            var LZSS = new LZSS(bbr, base.UnpackedSize);
            base.fileBuffer = LZSS.Decompress();

            bbr.Dispose();
            return base.fileBuffer;
        }

        protected override byte[] DecompressData()
        {
            using (MemoryStream st = new MemoryStream(tempBuffer))
            {
                RBinaryBigEndian bbr = new RBinaryBigEndian(st);
                //bbr.BaseStream.Seek(0, SeekOrigin.Begin);

                var LZSS = new LZSS(bbr, base.UnpackedSize);
                var bytes = LZSS.Decompress();

                bbr.Dispose();
                return bytes;
            }
        }

        #region Save
        #if SaveBuild

        internal override byte[] GetCompressedData()
        {
            if (base.RealFile == null) return null;

            using (FileStream file = new FileStream(base.RealFile, FileMode.Open, FileAccess.Read)) {
                byte[] data = compressStream(file);
                base.PackedSize = 0; //data.Length;
                base.RealFile = null;
                return data;
            }
        }

        private byte[] compressStream(FileStream stream)
        {
            var r = new RBinaryBigEndian(stream);
            r.BaseStream.Seek(0, SeekOrigin.Begin);
            return r.ReadBytes(base.UnpackedSize);
        }

        /*
        private byte[] compressStream(FileStream stream)
        {
            stream.Seek(base.Offset, SeekOrigin.Begin);

            // Create a new stream so that we are thread safe.
            MemoryStream s = new MemoryStream();

            // Copy packedSize amount of bytes from the original dat stream to our new memory stream.
            for (int i = 0; i < base.UnpackedSize; i++) s.WriteByte((byte)stream.ReadByte());

            BinaryBigEndian bbr = new BinaryBigEndian(s);
            bbr.BaseStream.Seek(0, SeekOrigin.Begin);

            var LZSS = new LZSS(bbr, base.UnpackedSize);
            var bytes = LZSS.Compress();

            s.Dispose();
            bbr.Dispose();

            return bytes;
        }
        */
        #endif
        #endregion
    }
}
