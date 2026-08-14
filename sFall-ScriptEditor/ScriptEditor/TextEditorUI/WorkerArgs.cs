using System;
using System.Collections.Generic;
using System.Text;
using ScriptEditor.CodeTranslation;

namespace ScriptEditor.TextEditorUI
{
    public class WorkerArgs
    {
        public readonly string text;
        public readonly TabInfo tab;
        public readonly ProgramInfo previousParseInfo;
        public int status;
        public bool parseIsFail = false;
        public ProgramInfo parseInfo;

        public WorkerArgs(string text, TabInfo tab)
        {
            this.text = text;
            this.tab = tab;
            previousParseInfo = tab.parseInfo == null ? null : tab.parseInfo.CreateSnapshot();
        }

        public override string ToString()
        {
            return status.ToString();
        }
    }
}
