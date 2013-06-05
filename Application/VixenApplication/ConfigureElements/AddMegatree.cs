using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenApplication.ConfigureElements
{
    public partial class AddMegatree : Form
    {
        public AddMegatree()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        public string TreeName
        {
            get { return textTreeName.Text; }
        }

        public int PixelsPerString
        {
            get { return Convert.ToInt32(LightsPerString.Value); }
        }

        public int StringCount
        {
            get { return Convert.ToInt32(Strings.Value); }
        }
    }
}
