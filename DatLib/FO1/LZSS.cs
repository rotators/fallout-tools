using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DATLib.FO1
{
    // Implemented by following https://falloutmods.fandom.com/wiki/DAT_file_format#Fallout_1_LZSS_uncompression_algorithm
    // https://en.wikipedia.org/wiki/Lempel-Ziv-Storer-Szymanski
    // https://web.archive.org/web/20160110174426/https://oku.edu.mie-u.ac.jp/~okumura/compression/history.html
    internal class LZSS
    {
        int uncompressedSize;
        RBinaryBigEndian stream;

        readonly short DICT_SIZE = 4096;
        readonly short MIN_MATCH = 3;
        readonly short MAX_MATCH = 18;

        private byte[] output;
        private byte[] dictionary;

        private short NR;     // bytes read from last block header
        private short DO = 0; // dictionary offset - for reading
        private short DI;     // dictionary index - for writing
        private int OI = 0;   // output index, used for writing to the output array.

        public LZSS(RBinaryBigEndian stream, int uncompressedSize)
        {
            this.uncompressedSize = uncompressedSize;
            this.stream = stream;
        }

        bool LastByte() { return stream.BaseStream.Position == stream.BaseStream.Length; }

        private void ClearDict()
        {
            for (var i = 0; i < DICT_SIZE; i++)
            {
                dictionary[i] = 0x20; // ' ' in ascii
            }
            DI = (short)(DICT_SIZE - MAX_MATCH);
        }

        private byte ReadByte()
        {
            NR++;
            return stream.ReadByte();
        }

        private byte[] ReadBytes(int bytes)
        {
            return stream.ReadBytes(bytes);
        }

        // Write to output and dictionary
        void WriteByte(byte b)
        {
            output[OI++] = b;
            dictionary[(DI++ % DICT_SIZE)] = b;
        }

        private void WriteBytes(byte[] bytes)
        {
            foreach (var b in bytes)
            {
                if (OI >= uncompressedSize) return;
                output[OI++] = b;
            }
        }

        private byte ReadDict()
        {
            return dictionary[(DO++ % DICT_SIZE)];
        }

        private Int16 ReadInt16()
        {
            return stream.ReadInt16();
        }

        private void ReadBlock(int N)
        {
            NR = 0; // The amount of bytes we have read in the current block so far
            if (N < 0) { // Uncompressed / literal block
                WriteBytes(ReadBytes(N * -1)); // We just read N bytes and write to the output buffer. Dictionary is untouched.
            } else { // N > 0
                ClearDict();
                // @Flag
                while (true)
                {
                    if (NR >= N || LastByte()) return; // Go to @Start

                    // Flags indicating the compression status of up to 8 next characters.
                    byte FL = ReadByte(); // Read flag byte
                    if (NR >= N || LastByte()) return; // Go to @Start

                    for (var x = 0; x < 8; x++)
                    {
                        if (FL % 2 == 1) { // @FLodd, normal byte
                            // Read byte from stream and put it in the output buffer and dictionary.
                            WriteByte(ReadByte());
                            if (NR >= N) return;
                        } else { // @FLeven, encoded dictionary offset
                            if (NR >= N) return;

                            // Read dictionary offset byte
                            DO = ReadByte();

                            if (NR >= N) return;

                            // Length byte
                            var LB = ReadByte();

                            DO |= (short)((LB & 0xF0) << 4);        // Prepend the high-nibble (first 4 bits) from LB to DO
                            // match length
                            int L = (int)((LB & 0x0F) + MIN_MATCH); // and remove it from LB and add MIN_MATCH
                            for (var i = 0; i < L; i++)
                            {
                                // Read a byte from the dictionary at DO, increment index and write to output and dictionary at DI.
                                WriteByte(ReadDict());
                            }
                        }
                        FL = (byte)(FL >> 1); // @flagNext
                        if (LastByte()) return;
                    }
                }
            }
        }

        public byte[] Decompress()
        {
            output = new byte[uncompressedSize];
            dictionary = new byte[DICT_SIZE];

            short N; // number of bytes to read
            while(!LastByte()) // @Start
            {
                N = ReadInt16();   // How many bytes to read in the next block
                if (N == 0) break; // No bytes, so exit
                ReadBlock(N);
            }
            return output;
        }

        //public byte[] Compress()
        //{
        //    output = new byte[uncompressedSize];
        //    dictionary = new byte[DICT_SIZE];
        //    return output;
        //}
    }
}
