using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VIVO_45_Board_Test
{
    public partial class ResultBox : Form
    {
        public ResultBox(String text, bool passed)
        {
            InitializeComponent();
            label1.Text = text;
            if(passed)
                label1.BackColor = Color.Green;
            else
                label1.BackColor = Color.Red;
            btnOK.Focus();
        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
