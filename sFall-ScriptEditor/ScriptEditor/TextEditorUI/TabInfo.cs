using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ICSharpCode.TextEditor;

using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI.Nodes;

namespace ScriptEditor.TextEditorUI
{
    public struct Position
    {
        public List<TextLocation> linePosition;
        public int prevPosition;
        public int pointerCur;
        public int pointerEnd;
    }

    /// <summary>
    /// Represents opened document tab.
    /// </summary>
    public class TabInfo
    {
        /// <summary>
        /// The tab index.
        /// </summary>
        public int index;

        /// <summary>
        /// The TextEditor control of this tab.
        /// </summary>
        public TextEditorControl textEditor;

        public string filepath;
        public string filename;

        public bool changed;

        public bool DisableParseAndStatusChange { get; set; }

        private DateTime fileTime;
        public DateTime FileTime
        {
            set { fileTime = value; }
        }

        public bool CheckFileTime()
        {
            DateTime time = File.GetLastWriteTime(filepath);
            return (time == fileTime);
        }

        internal void SaveInternal(string saveText, System.Text.Encoding encFile, bool isMsg = false, bool isClose = false, bool isScript = true)
        {
            File.WriteAllText(filepath, saveText, (isMsg) ? Settings.EncCodePage
                                                          : (isScript && Settings.saveScriptUTF8) ? new UTF8Encoding(false)
                                                                                                  : encFile);
            if (!isClose) fileTime = File.GetLastWriteTime(filepath);
        }

        /// <summary>
        /// The path to associated message file.
        /// </summary>
        public string msgFilePath;

        /// <summary>
        /// An opened tab with MSG file associated with currently opened SSL.
        /// Link to associated message tab.
        /// </summary>
        public TabInfo msgFileTab;

        /// <summary>
        /// Ассоциированные сообщения с данным скриптом, содержит номер строки и его aссоциированный текст
        /// </summary>
        public readonly Dictionary<int, string> messages = new Dictionary<int, string>();

        /// <summary>
        /// The node procedure TextEditor list control of this tab.
        /// </summary>
        public List<FlowchartTE> nodeFlowchartTE = new List<FlowchartTE>();

        /// <summary>
        /// Indicates that this tab requires you to update the nodes information in the flowcharts.
        /// </summary>
        //public bool shouldUpdate;

        /// <summary>
        /// Indicates whether parsing is required for this tab (usually true for .SSL or .H files).
        /// </summary>
        public bool shouldParse;

        /// <summary>
        /// Indicates whether this tab is pending parsing (eg. after text change).
        /// </summary>
        public bool needsParse;

        /// <summary>
        /// Storing history of navigation to document for function Back/Forward.
        /// </summary>
        public Position history;

        public string parserLog;
        public List<Error> parserErrors = new List<Error>();

        public string buildLog;
        public List<Error> buildErrors = new List<Error>();

        public Dictionary<string, bool> treeExpand = new Dictionary<string, bool>();

        public ProgramInfo parseInfo;
    }
}
