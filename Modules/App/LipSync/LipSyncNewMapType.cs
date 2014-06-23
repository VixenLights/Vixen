using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.App.LipSyncApp
{
    public partial class LipSyncNewMapType : Form
    {
        public LipSyncNewMapType()
        {
            InitializeComponent();
        }

        public int StringCount { get; set; }
        public int PixelsPerString { get; set; }

        private void stringsUpDown_ValueChanged(object sender, EventArgs e)
        {
            StringCount = Convert.ToInt32(stringsUpDown.Value);
            PixelsPerString = Convert.ToInt32(pixelsUpDown.Value);
        }

        private void pixelsUpDown_ValueChanged(object sender, EventArgs e)
        {
            StringCount = Convert.ToInt32(stringsUpDown.Value);
            PixelsPerString = Convert.ToInt32(pixelsUpDown.Value);
        }

        private void matrixMappingRadio_CheckedChanged(object sender, EventArgs e)
        {
            stringsUpDown.Enabled = matrixMappingRadio.Checked;
            pixelsUpDown.Enabled = matrixMappingRadio.Checked;
        }

        private void LipSyncNewMapType_Load(object sender, EventArgs e)
        {
            stringMappingRadio.Checked = true;

            stringsUpDown.Enabled = false;
            pixelsUpDown.Enabled = false;
        }


    }
}
