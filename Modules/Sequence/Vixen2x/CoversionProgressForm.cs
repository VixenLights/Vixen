using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.SequenceType.Vixen2x
{
    public partial class CoversionProgressForm : Form
    {
        public string StatusLineLabel { set { lblStatusLine.Text = value; } }


        public CoversionProgressForm()
        {
            InitializeComponent();
        }

        public void UpdateProgressBar(int value)
        {
            pbImport.Value = value;

        }

        public void SetupProgressBar(int min, int max)
        {
            pbImport.Minimum = min;
            pbImport.Maximum = max;
            pbImport.Value = 0;
        }
    }
}
