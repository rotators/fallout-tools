using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEditor.CodeTranslation
{
    public interface IParserInfo
    {
        bool IsImported { get; }
        bool IsExported { get; }
        bool IsStandart();

        NameType Type();

        Reference[] References();
        void Deceleration(out string file, out int line);
        string ToString(bool b);
    }
}
