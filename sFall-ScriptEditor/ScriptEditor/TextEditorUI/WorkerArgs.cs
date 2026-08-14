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
        public readonly int textRevision;
        public int status;
        public bool parseIsFail = false;
        public ProgramInfo parseInfo;

        public WorkerArgs(string text, TabInfo tab)
        {
            this.text = text;
            this.tab = tab;
            textRevision = tab.TextRevision;
            previousParseInfo = tab.parseInfo == null ? null : tab.parseInfo.CreateSnapshot();
        }

        public bool IsCurrent
        {
            get { return tab != null && textRevision == tab.TextRevision; }
        }

        public override string ToString()
        {
            return status.ToString();
        }
    }
}
