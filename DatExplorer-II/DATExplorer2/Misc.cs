using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DATExplorer
{
    static class Misc
    {
        internal static void BuildTreeSub(OpenDat dat, TreeNode root)
        {
            foreach (var folder in dat.Folders)
            {
                if (folder.Key.Length > 1) {
                    string[] dirs = folder.Key.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries); // dirs in lower case

                    TreeNode tn = root;
                    string parentsDir = String.Empty;

                    for (int i = 0; i < dirs.Length; i++)
                    {
                        parentsDir += dirs[i] + "\\";
                        TreeNode find = Misc.FindNode(dirs[i], tn);
                        if (find == null) {
                            tn = tn.Nodes.Add(folder.Value.FolderName(i)); // имя папки
                            tn.Name = parentsDir; // путь к папке (не должен содержать знак разделителя пути в начале)
                        } else
                            tn = find;
                    }
                }
            }
        }

        internal static string GetNodeFullPath(TreeNode node)
        {
            string full = node.FullPath;
            return full.Remove(0, full.IndexOf("]") + 2);
        }

        /// <summary>
        /// Возвращает имя и путь к файлу
        /// </summary>
        internal static string GetDatName(TreeNode node)
        {
            return (node.Parent == null) ? node.Name : GetRootNode(node.Parent).Name;
        }

        internal static TreeNode GetRootNode(TreeNode node)
        {
            if (node.Parent != null) {
                return GetRootNode(node.Parent);
            }
            return node;
        }

        // рекурсивно ищет ноду с указанным отображаемым именем папки
        internal static TreeNode FindNode(string name, TreeNode node)
        {
            foreach (TreeNode nd in node.Nodes)
            {
                if (nd.Text.Equals(name, StringComparison.OrdinalIgnoreCase)) {
                    return nd;
                }
                TreeNode find = FindNode(name, nd);
                if (find != null) return find;
            }
            return null;
        }

        // рекурсивно ищет ноду с указанным путем
        internal static TreeNode FindPathNode(string path, TreeNode fromNode)
        {
            foreach (TreeNode node in fromNode.Nodes)
            {
                if (node.Name == path) {
                    return node;
                }
                TreeNode find = FindPathNode(path, node);
                if (find != null) return find;
            }
            return null;
        }

        internal static void GetExpandedNodes(TreeNode nodes, ref List<TreeNode> expandedNodes)
        {
            foreach (TreeNode node in nodes.Nodes)
            {
                if (node.IsExpanded) expandedNodes.Add(node);
                if (node.Nodes.Count > 0) GetExpandedNodes(node, ref expandedNodes);
            }
        }

        internal static void ExpandNode(TreeNode expandNode, TreeNode inNodes)
        {
            foreach (TreeNode node in inNodes.Nodes)
            {
                if (node.Text == expandNode.Text) {
                    node.Expand();
                    return;
                }
                ExpandNode(expandNode, node);
            }
        }
    }
}
