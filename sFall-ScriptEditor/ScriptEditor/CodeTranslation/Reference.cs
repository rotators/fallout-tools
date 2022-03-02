using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ScriptEditor.CodeTranslation
{
    public class Reference
    {
        public string file;
        public readonly int line;

        public Reference(string file, int line)
        {
            this.file = file;
            this.line = line;
            
            if (this.file != null) {
                this.file = Path.GetFullPath(this.file);
            }
        }

        public static Reference FromPtr(int line, int filePtr)
        {
            return new Reference(Marshal.PtrToStringAnsi(new IntPtr(filePtr)), line);
        }
    }
}
