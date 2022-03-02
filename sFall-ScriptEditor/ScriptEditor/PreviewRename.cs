using ScriptEditor.TextEditorUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ScriptEditor
{
    public partial class PreviewRename : Form
    {
        public PreviewRename(string oldName, string newName)
        {
            InitializeComponent();

            this.Text += String.Format(" {0} to {1}", oldName, newName);

            button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private int BuildSub(Dictionary<string, List<Refactor.PreviewMatch>> matchesList, int root)
        {
            int totalMatches = 0;
            foreach (var matches in matchesList)
            {
                treeView1.Nodes[root].Nodes.Add((root == 0) ? matches.Value[0].tab.filename : matches.Key);
                int c = treeView1.Nodes[root].Nodes.Count - 1;
                treeView1.Nodes[root].Nodes[c].Checked = true;
                int s = 0;
                foreach (var match in matches.Value)
                {
                    treeView1.Nodes[root].Nodes[c].Nodes.Add(match.previewText);
                    treeView1.Nodes[root].Nodes[c].Nodes[s].Tag = match;
                    treeView1.Nodes[root].Nodes[c].Nodes[s++].Checked = true;
                    totalMatches++;
                }
            }
            return totalMatches;
        }

        internal void BuildTreeMatches(Dictionary<string, List<Refactor.PreviewMatch>> tabs,
                                       Dictionary<string, List<Refactor.PreviewMatch>> files)
        {
            treeView1.Nodes.Add("Curren open scripts");
            treeView1.Nodes[0].Checked = true;
            int totalMatches = BuildSub(tabs, 0);

            treeView1.Nodes.Add("Files in project folder");
            treeView1.Nodes[1].Checked = true;
            totalMatches += BuildSub(files, 1);

            treeView1.Nodes[0].Expand();
            treeView1.Nodes[1].Expand();

            label1.Text = String.Format("Matches found: {0}, in {1} files.", totalMatches, tabs.Count() + files.Count());
        }

        internal void GetSelectedMatches(ref Dictionary<string, List<Refactor.PreviewMatch>> tabs,
                                         ref Dictionary<string, List<Refactor.PreviewMatch>> files)
        {
            GetMatches(treeView1.Nodes[0].Nodes, ref tabs);
            GetMatches(treeView1.Nodes[1].Nodes, ref files);
        }

        private List<Refactor.PreviewMatch> GetMatchesSub(TreeNode nodes)
        {
            List<Refactor.PreviewMatch> list = new List<Refactor.PreviewMatch>();
            foreach (TreeNode node in nodes.Nodes)
            {
                if (!node.Checked) continue;
                list.Add((Refactor.PreviewMatch)node.Tag);
            }
            return list;
        }

        private void GetMatches(TreeNodeCollection nodes, ref Dictionary<string, List<Refactor.PreviewMatch>> list)
        {
            foreach (TreeNode node in nodes)
            {
                if (!node.Checked) continue;
                list.Add(node.Text, GetMatchesSub(node));
            }
        }

        private void SetChildCheck(TreeNodeCollection nodes, bool state)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = state;
                if (node.Nodes.Count != 0) SetChildCheck(node.Nodes, state);
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (this.Visible && e.Action != TreeViewAction.Unknown) {
                SetChildCheck(e.Node.Nodes, e.Node.Checked);
            }
        }
    }
}
