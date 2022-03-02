/*
 * Created by SharpDevelop.
 * User: Phobos2077
 * Date: 21.07.2014
 * Time: 1:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScriptEditor
{
    /// <summary>
    /// Description of GoToLine.
    /// </summary>
    public partial class GoToLine : Form
    {
        public GoToLine()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
        }

        void GoToLineKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) {
                Close();
            }
        }

        void TbLineKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                bGo.PerformClick();
                Close();
            } else if (e.KeyCode == Keys.Escape) {
                Close();
            }
        }
    }
}
