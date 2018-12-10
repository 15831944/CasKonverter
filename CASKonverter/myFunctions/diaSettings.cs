using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CASKonverter.myFunctions
{
    public partial class diaSettings : Form
    {
        private myUtilities.mySettings objSettings = myUtilities.mySettings.Instance;

        public diaSettings()
        {
            InitializeComponent();
            
            //Settings
            numericUpDown1.Value = objSettings.Nachkommastellen;
        }

        private void bt_OK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            objSettings.chgNachkommatellen((int) numericUpDown1.Value);
        }
    }
}
