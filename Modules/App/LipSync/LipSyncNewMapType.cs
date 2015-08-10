using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.App.LipSyncApp
{
    public partial class LipSyncNewMapType : Form
    {
        public LipSyncNewMapType()
        {
			Location = ActiveForm != null ? new Point(ActiveForm.Location.X + 130, ActiveForm.Location.Y + 90) : new Point(500, 200);
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
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

		private void button_Paint(object sender, PaintEventArgs e)
		{
			ThemeButtonRenderer.OnPaint(sender, e, null);
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			ThemeButtonRenderer.ButtonHover = true;
			var btn = sender as Button;
			btn.Invalidate();
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			ThemeButtonRenderer.ButtonHover = false;
			var btn = sender as Button;
			btn.Invalidate();
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}
