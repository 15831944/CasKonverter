using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CASKonverter.myFormats;

namespace CASKonverter.myFunctions
{
    public partial class Height_Offset : Form
    {
        private double m_Offset;

        public Height_Offset()
        {
            InitializeComponent();
        }

        //Properties
        public double Offset
        {
            get { return m_Offset; }
        }

        private void tb_HeightOffset_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                try
                {
                    tb_HeightOffset.Text = tb_HeightOffset.Text.Replace('.', ',');
                    m_Offset = Convert.ToDouble(tb_HeightOffset.Text);
                    Close();
                }

                catch 
                {
                    MessageBox.Show("falsches Fortmat!");
                }
            }
        }
    }
}
