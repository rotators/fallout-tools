using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ScriptEditor.TextEditorUI
{
    static class ColorTheme
    {
        public static bool IsDarkTheme;

        public static HighlightColor TipGradient;
        public static HighlightColor SelectedHighlight;
        public static Color          CodeFunctions;
        public static Color          HighlightError;
        public static Color          HighlightIncludeError;
        public static Color          IncludeHighlight;
        public static Brush          TipText = Brushes.AliceBlue;

        public static Color          TreeNameFunction;

        private static TextEditor mainForm;

        public static Color TipBorderFrame {
            get { return (IsDarkTheme) ? Color.FromArgb(0x70, 0x70, 0x80) : Color.Black; }
        }

        public static Color HighlightProcedureTree {
            get { return (IsDarkTheme) ? Color.Yellow : Color.Blue; }
        }

        public static string HighlightingScheme
        {
            get {
                string sslSchema;
                switch (Settings.highlight) {
                    case 1: sslSchema = "F-Geck";
                        break;
                    case 2: sslSchema = "Dark";
                        break;
                    default: sslSchema = "Original";
                        break;
                }
                return sslSchema;
            }
        }
        
        public static void InitTheme(bool isDark, TextEditor form)
        {
            if (form != null) 
                mainForm = form;
            
            IsDarkTheme = isDark;
            if (isDark)
                SetDarkTheme();
            else
                SetDefaultTheme((!isDark & form != null));
        }

        public static void SetTheme()
        {
            bool isDark = (Settings.highlight == 2);
            if (isDark == IsDarkTheme)
                return;

            mainForm.Invalidate(true);

            InitTheme(isDark, null);

            mainForm.FunctionTreeLeft.BeginUpdate();
            mainForm.FunctionsTree.BeginUpdate();
            FunctionNodes(mainForm.FunctionTreeLeft.Nodes);
            FunctionNodes(mainForm.FunctionsTree.Nodes);
            mainForm.FunctionTreeLeft.EndUpdate();
            mainForm.FunctionsTree.EndUpdate();
        }

        private static void SetDarkTheme()
        {
            IHighlightingStrategy scheme = HighlightingStrategyFactory.CreateHighlightingStrategy("Dark");
            TipGradient = scheme.GetColorFor("TipsGradient");

            SelectedHighlight = new HighlightColor(Color.White, Color.MediumVioletRed);
            CodeFunctions  = Color.FromArgb(64, 64, 90);
            HighlightError = Color.Red;
            HighlightIncludeError = Color.Lime;
            IncludeHighlight = Color.DarkSlateGray;
            TreeNameFunction = Color.FromArgb(243, 233, 122); //LightKhaki

            Color back = Color.FromArgb(40, 40, 42);
            mainForm.FunctionTreeLeft.BackColor = back;
            mainForm.FunctionTreeLeft.ForeColor = Color.Gainsboro;
            mainForm.FunctionTreeLeft.LineColor = Color.DimGray;

            mainForm.FunctionsTree.BackColor = back;
            mainForm.FunctionsTree.ForeColor = Color.Gainsboro;
            mainForm.FunctionsTree.LineColor = Color.DimGray;

            mainForm.ProcTree.BackColor = back;
            mainForm.ProcTree.ForeColor = Color.Gainsboro;
            mainForm.ProcTree.LineColor = Color.Gray;

            mainForm.VarTree.BackColor = back;
            mainForm.VarTree.ForeColor = Color.Gainsboro;
            mainForm.VarTree.LineColor = Color.Gray;
        }

        private static void SetDefaultTheme(bool isDefault)
        {
            if (Settings.highlight == 0) {
                TipGradient = new HighlightColor(Color.White, Color.FromArgb(255, 245, 190));
            } else {
                IHighlightingStrategy scheme = HighlightingStrategyFactory.CreateHighlightingStrategy("F-Geck");
                TipGradient = scheme.GetColorFor("TipsGradient");
            }

            SelectedHighlight = new HighlightColor(Color.Black, Color.GreenYellow);
            CodeFunctions = Color.LightGray;
            HighlightError = Color.FromArgb(160, Color.Red);
            HighlightIncludeError = Color.Blue;
            IncludeHighlight = Color.Beige;
            TreeNameFunction = Color.FromArgb(100, 0, 100); //DarkPurple

            // for form
            if (isDefault) return;

            mainForm.FunctionTreeLeft.BackColor = Color.GhostWhite;
            mainForm.FunctionTreeLeft.ForeColor = SystemColors.WindowText;
            mainForm.FunctionTreeLeft.LineColor = Color.Gray;

            Color back = Color.FromArgb(250, 250, 255);
            mainForm.FunctionsTree.BackColor = back;
            mainForm.FunctionsTree.ForeColor = SystemColors.WindowText;
            mainForm.FunctionsTree.LineColor = Color.Gainsboro;

            mainForm.ProcTree.BackColor = back;
            mainForm.ProcTree.ForeColor = SystemColors.WindowText;
            mainForm.ProcTree.LineColor = SystemColors.WindowText;

            mainForm.VarTree.BackColor = back;
            mainForm.VarTree.ForeColor = SystemColors.WindowText;
            mainForm.VarTree.LineColor = SystemColors.WindowText;
        }

        private static void FunctionNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
                NodeColorUpdate(node);
        }

        private static void NodeColorUpdate(TreeNode node)
        {
            foreach (TreeNode nd in node.Nodes)
            {
                if (nd.Tag != null)
                    nd.ForeColor = TreeNameFunction;
                else
                    NodeColorUpdate(nd);
            }  
        }

        // Check for specific color text
        // TODO: переписать чтобы цвет можно было получать из файла подсветки
        internal static bool CheckColorPosition(IDocument document, TextLocation tl, bool result = false)
        { 
            HighlightColor hc = document.GetLineSegment(tl.Line).GetColorForPosition(tl.Column);
            if (hc != null) {
                if (IsDarkTheme) {
                    if (hc.Color == Color.PaleGreen || hc.Color == Color.FromArgb(0x7A, 0xAA, 0x7A)
                        || hc.BackgroundColor == CodeFunctions)
                        return true;
                } else {
                    if (hc.Color == Color.DarkSlateGray || hc.Color == Color.Green || hc.Color == Color.Brown || hc.Color == Color.DarkGreen
                        || hc.BackgroundColor == CodeFunctions || hc.BackgroundColor == Color.FromArgb(0xFF, 0xFF, 0xDA)
                        || hc.BackgroundColor == Color.FromArgb(0xFF, 0xFF, 0xD0))
                        return true;
                }

            } else if (result)
                return true;

            return false;
        }
    }
}
