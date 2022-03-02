using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ScriptEditor.SyntaxRules;

namespace ScriptEditor.TextEditorUI
{
    public static class Functions
    {
        const string fnFile = "Functions";
        const string fnUserFile = "UserFunctions.ini";

        public static void CreateTree(TreeView Tree)
        {
            int Node = -1, mNode = -1, sNode = -1, ssNode = -1, aNode = 0;

            if (!File.Exists(fnUserFile))
                File.WriteAllText(fnUserFile, Properties.Resources.UserFunctions);

            string file = Path.Combine(Settings.DescriptionsFolder, fnFile) + ((Settings.hintsLang == HandlerProcedure.English) ? HandlerProcedure.def : HandlerProcedure.lng);
            BuildFunctionTree(Tree, ref Node, ref mNode, ref sNode, ref ssNode, ref aNode, file);
            BuildFunctionTree(Tree, ref Node, ref mNode, ref sNode, ref ssNode, ref aNode, fnUserFile, true);
        }

        private static void BuildFunctionTree(TreeView Tree, ref int Node, ref int mNode, ref int sNode, ref int ssNode, ref int aNode, string file, bool user = false)
        {
            TreeNode ND;
            string codeName;
            string[] lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++) 
            {
                lines[i] = lines[i].Trim();
                if (lines[i].Length == 0 || lines[i].StartsWith("//")) continue;
                if (lines[i].StartsWith("<m>")) {
                    codeName = lines[i].Substring(3, lines[i].Length - 3);
                    ND = new TreeNode(codeName);
                    if (user)
                        ND.Name = "user";
                    Tree.Nodes.Add(ND);
                    Node++;
                    sNode = -1;
                    mNode = -1;
                    aNode = 0;
                    continue;
                } else if (lines[i].StartsWith("<m+>")) {
                    codeName = lines[i].Substring(4, lines[i].Length - 4);
                    ND = new TreeNode(codeName);
                    mNode++;
                    if (mNode <= 0) {
                        Tree.Nodes[Node].Nodes.Add(ND);
                        sNode++;
                        if (aNode > 0) {
                            sNode += aNode;
                            aNode = 0;
                        }
                    } else {
                        Tree.Nodes[Node].Nodes[sNode].Nodes.Add(ND);
                        ssNode++;
                    }
                    continue;
                } else if (lines[i].StartsWith("<m->")) {
                    mNode--;
                    if (mNode < 0)
                        ssNode = 0;
                    continue;
                } else {
                    int n = lines[i].IndexOf("<d>");
                    if (n > 0) {
                        ND = new TreeNode(lines[i].Substring(0, n));
                        int m = lines[i].IndexOf("<s>");
                        ND.ToolTipText = lines[i].Substring(n + 3, m - (n + 3));
                        ND.Tag = lines[i].Substring(m + 3, lines[i].Length - (m + 3));
                        ND.NodeFont = new Font("Arial", 8, FontStyle.Bold);
                        ND.ForeColor = ColorTheme.TreeNameFunction;
                        switch (mNode) {
                            case -1:
                                Tree.Nodes[Node].Nodes.Add(ND);
                                aNode++;
                                break;
                            case 0:
                                Tree.Nodes[Node].Nodes[sNode].Nodes.Add(ND);
                                ssNode++;
                                break;
                            case 1:
                                Tree.Nodes[Node].Nodes[sNode].Nodes[ssNode - 1].Nodes.Add(ND);
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (lines[i] == "<->") {
                    Tree.Nodes.Add(new String('-', 50));
                    Node++;
                }
            }
        }

        internal static bool NodeHitCheck(Point location, Rectangle bounds)
        {
            return (location.X >= bounds.X && location.X <= bounds.Right &&
                    location.Y >= bounds.Y && location.Y <= bounds.Bottom);
        }

        internal static void AddFunction(TreeNode node)
        { 
            string name = string.Empty;
            string desc = string.Empty;
            string code = string.Empty;
            
            if (!new UserFunction(true).InitShow(ref name, ref desc, ref code))
                return;

            string targetName;
            int index = 0;
            TreeNode pn = node.Parent;
            if (node.Tag == null /*|| pn == null*/) {
                pn = node;
                targetName = (pn.Level > 0) ? targetName = "<m+>": targetName = "<m>";
                targetName += pn.Text;
            } else {
                index = node.Index + 1;
                targetName = node.Text + "<d>";
            }

            if (!AddFuncToFile(targetName, name, desc, code, fnUserFile)) {
                MessageBox.Show("Could not add function.", "Error");
                return;
            }

            if (UserFunction.highlight)
                SyntaxFile.AddKeyWord(GetKeyWord(code));

            var newNode = new TreeNode(name);
            newNode.ToolTipText = desc;
            newNode.Tag = code;
            newNode.NodeFont = new Font("Arial", 8, FontStyle.Bold);
            newNode.ForeColor = ColorTheme.TreeNameFunction;

            pn.Nodes.Insert(index, newNode);
            newNode.EnsureVisible();
        }

        internal static void EditFunction(TreeNode node)
        {
            if (node.Tag == null)
                return;

            string name = node.Text;
            string desc = node.ToolTipText;
            string code = (string)node.Tag;

            if (!new UserFunction(false).InitShow(ref name, ref desc, ref code))
                return;

            if (!IsUserFunction(node)) {
                if (MessageBox.Show("This function is not user-defined.\nDo you really want to change this function?",
                    "Warning", MessageBoxButtons.YesNo) == DialogResult.No) { 
                    return;
                }
                string file = Path.Combine(Settings.DescriptionsFolder, fnFile) + ((Settings.hintsLang == HandlerProcedure.English) ? HandlerProcedure.def : HandlerProcedure.lng);
                EditFuncToFile(node.Text, name, desc, code, file);
            } else {
                if (!EditFuncToFile(node.Text, name, desc, code, fnUserFile)) {
                    MessageBox.Show("Could not change this function.", "Error");
                    return;
                }
            }
            node.Text = name;
            node.Tag = code;
            node.ToolTipText = desc;
        }

        internal static void AddNode(TreeNode node)
        {
            string name = String.Empty;
            if (InputBox.ShowDialog("Add function node", ref name, 20) == DialogResult.Cancel || name.Length == 0)
                return;

            var lastNode = node;
            GetLastNode(ref lastNode);

            bool inNode = false;
            string targetName;
            if (lastNode.Tag == null)
                targetName = "<m+>" + lastNode.Text;
            else {
                targetName = lastNode.Text + "<d>";
                if (node.Level > 0)
                    inNode = true;
            }

            if (!AddNodeToFile(targetName, inNode, name, fnUserFile)) {
                MessageBox.Show("Could not add node.", "Error");
                return;
            }

            var newNode = new TreeNode(name);
            node.Nodes.Add(newNode);
            newNode.EnsureVisible();
        }

        internal static void RenameNode(TreeNode node)
        {
            string name = node.Text;
            if (InputBox.ShowDialog("Rename function node", ref name, 20) == DialogResult.Cancel || name.Length == 0)
                return;

            string type = (node.Level == 0) ? "<m>" : "<m+>";

            if (!RenNodeToFile(type, node.Text, name, fnUserFile)) {
                 MessageBox.Show("It was not possible to rename this node.", "Error");
                 return;
            }
            node.Text = name;
        }

        internal static void DeleteNode(TreeNode node)
        {
            if (MessageBox.Show("Do you want to delete this item?", "Delete", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            bool IsNode = (node.Tag == null);

            if (!DeleteNodeToFile(IsNode, node.Text, fnUserFile)) {
                 MessageBox.Show("It was not possible to delete this item.", "Error");
                 return;
            }
            if (!IsNode)
                SyntaxFile.RemoveKeyWord(GetKeyWord((string)node.Tag));

            node.Remove();
        }
        
        internal static bool IsUserFunction(TreeNode node)
        {
            int level = node.Level;
            for (int i = level; i > 0; i--)
                node = node.Parent;

            return (node.Name == "user");
        }

        private static void GetLastNode(ref TreeNode node)
        {
            var last = node.LastNode;
            if (last != null) {
                node = last;
                GetLastNode(ref node);
            }
        }

        private static string GetKeyWord(string code)
        {
            for (int i = 0; i < code.Length; i++)
            {
                if (!Char.IsLetterOrDigit(code[i]) && code[i] != '_') {
                    code = code.Remove(i);
                    break;
                }
            }
            return code;
        }

        #region Save to File
        private static bool EditFuncToFile(string func, string name, string desc, string code, string file)
        {
            string[] lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(func + "<d>")) {
                    lines[i] = name + "<d>" + desc + "<s>" + code;
                    File.WriteAllLines(file, lines);
                    return true;
                }
            }
            return false; 
        }

        private static bool RenNodeToFile(string type, string node, string name, string file)
        {
            string[] lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(type + node)) {
                    lines[i] = type + name;
                    File.WriteAllLines(file, lines);
                    return true;
                }
            }
            return false;
        }

        private static bool DeleteNodeToFile(bool type, string name, string file)
        {
            string prefix = (type) ? "<m+>" : String.Empty;

            List<string> lines = File.ReadAllLines(file).ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith(prefix + name)) {
                    lines.RemoveAt(i);
                    if (type)
                        lines.RemoveAt(i);
                    File.WriteAllLines(file, lines);
                    return true;
                }
            }
            return false; 
        }

        private static bool AddFuncToFile(string func, string name, string desc, string code, string file)
        {
            List<string> lines = File.ReadAllLines(file).ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith(func)) {
                    lines.Insert(i + 1, name + "<d>" + desc + "<s>" + code);
                    File.WriteAllLines(file, lines);
                    return true;
                }
            }
            return false; 
        }

        private static bool AddNodeToFile(string node, bool inNode, string name, string file)
        {
            bool found = false;
            List<string> lines = File.ReadAllLines(file).ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                if (!found && lines[i].StartsWith(node))
                    found = true;
                else if (found && (inNode || lines[i].StartsWith("<m->"))) {
                    if (inNode)
                        i--;
                    lines.Insert(++i, "<m+>" + name);
                    lines.Insert(++i, "<m->");
                    File.WriteAllLines(file, lines);
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
