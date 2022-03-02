using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DATExplorer
{
    public partial class CustomMessageBox : Form
    {
        public CustomMessageBox(string message)
        {
            InitializeComponent();

            label1.Text = message;
        }
    }
}
