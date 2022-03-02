using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEditor.TextEditorUI
{
    public class WorkerArgs
    {
        public readonly string text;
        public readonly TabInfo tab;
        public int status;
        public bool parseIsFail = false;

        public WorkerArgs(string text, TabInfo tab)
        {
            this.text = text;
            this.tab = tab;
        }

        public override string ToString()
        {
            return status.ToString();
        }
    }
}
