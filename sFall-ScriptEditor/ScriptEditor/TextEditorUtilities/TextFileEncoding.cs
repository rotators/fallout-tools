using System;
using System.IO;
using System.Text;

namespace ScriptEditor.TextEditorUtilities
{
    internal sealed class TextFileContents
    {
        internal readonly string Text;
        internal readonly Encoding Encoding;

        internal TextFileContents(string text, Encoding encoding)
        {
            Text = text;
            Encoding = encoding;
        }
    }

    internal static class TextFileEncoding
    {
        internal static TextFileContents Read(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            int preambleLength;
            Encoding encoding = Detect(bytes, out preambleLength);
            return new TextFileContents(encoding.GetString(bytes, preambleLength, bytes.Length - preambleLength), encoding);
        }

        private static Encoding Detect(byte[] bytes, out int preambleLength)
        {
            if (HasPrefix(bytes, 0x00, 0x00, 0xFE, 0xFF)) {
                preambleLength = 4;
                return new UTF32Encoding(true, true);
            }
            if (HasPrefix(bytes, 0xFF, 0xFE, 0x00, 0x00)) {
                preambleLength = 4;
                return new UTF32Encoding(false, true);
            }
            if (HasPrefix(bytes, 0xEF, 0xBB, 0xBF)) {
                preambleLength = 3;
                return new UTF8Encoding(true);
            }
            if (HasPrefix(bytes, 0xFE, 0xFF)) {
                preambleLength = 2;
                return new UnicodeEncoding(true, true);
            }
            if (HasPrefix(bytes, 0xFF, 0xFE)) {
                preambleLength = 2;
                return new UnicodeEncoding(false, true);
            }

            preambleLength = 0;
            if (ContainsNonAscii(bytes)) {
                try {
                    new UTF8Encoding(false, true).GetString(bytes);
                    return new UTF8Encoding(false);
                }
                catch (DecoderFallbackException) { }
            }
            return Encoding.Default;
        }

        private static bool ContainsNonAscii(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
                if (bytes[i] >= 0x80)
                    return true;
            return false;
        }

        private static bool HasPrefix(byte[] bytes, params byte[] prefix)
        {
            if (bytes.Length < prefix.Length)
                return false;
            for (int i = 0; i < prefix.Length; i++)
                if (bytes[i] != prefix[i])
                    return false;
            return true;
        }
    }
}