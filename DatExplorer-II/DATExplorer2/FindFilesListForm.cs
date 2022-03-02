using System.Collections.Generic;
using System.Windows.Forms;

namespace DATExplorer
{
    internal partial class FindFilesListForm : Form
    {
        internal delegate void GotoFileHandler(sFile file);
        internal event GotoFileHandler GotoFile;

        internal FindFilesListForm(List<sFile> list)
        {
            InitializeComponent();

            foreach (var file in list)
	        {
		        ListViewItem item = new ListViewItem(file.file.name);
                item.SubItems.Add(file.file.pathTree);
                item.Tag = file;
                
                lstViewFiles.Items.Add(item);
	        }
        }

        private void lstViewFiles_DoubleClick(object sender, System.EventArgs e)
        {
            sFile item = (sFile)lstViewFiles.SelectedItems[0].Tag;
            if (GotoFile != null) GotoFile(item);
        }
    }
}
