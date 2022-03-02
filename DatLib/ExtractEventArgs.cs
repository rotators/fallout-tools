using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DATLib
{
    public delegate void ExtractFileEvent(ExtractEventArgs e);

    public class ExtractEventArgs
    {
        protected string name;
        protected bool result;

        public string Name { get { return name; } }
        public bool Result { get { return result; } }

        public ExtractEventArgs(string name, bool result)
        {
            this.name = name;
            this.result = result;
        }
    }
}
