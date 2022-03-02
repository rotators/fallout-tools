using System;
using System.IO;

namespace DATLib
{
    internal class RBinaryBigEndian : BinaryReader
    {
        public RBinaryBigEndian(System.IO.Stream stream) : base(stream) { }

        public override Int16 ReadInt16()
        {
            var data = base.ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public override int ReadInt32()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        //public override Int64 ReadInt64()
        //{
        //    var data = base.ReadBytes(8);
        //    Array.Reverse(data);
        //    return BitConverter.ToInt64(data, 0);
        //}

        public override UInt32 ReadUInt32()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        public UInt32 ReadUInt()
        {
            return base.ReadUInt32();
        }

    }

    internal class WBinaryBigEndian : BinaryWriter
    {
        public WBinaryBigEndian(System.IO.Stream stream) : base(stream) { }

        public void WriteInt32BE(int value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }

        public void WriteUInt32BE(uint value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }
    }
}
