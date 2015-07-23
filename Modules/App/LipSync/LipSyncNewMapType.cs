using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Resources.Properties;

namespace VixenModules.App.LipSyncApp
{
    public partial class LipSyncNewMapType : Form
    {
        public LipSyncNewMapType()
        {
			Location = ActiveForm != null ? new Point(ActiveForm.Location.X + 130, ActiveForm.Location.Y + 90) : new Point(500, 200);
            InitializeComponent();
			buttonOk.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonCancel.BackgroundImage = Resources.HeadingBackgroundImage;
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

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.HeadingBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.HeadingBackgroundImage;
		}

		#region Draw lines and GroupBox borders
		//set color for box borders.
		private Color _borderColor = Color.FromArgb(136, 136, 136);

		public Color BorderColor
		{
			get { return _borderColor; }
			set { _borderColor = value; }
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			//used to draw the boards and text for the groupboxes to change the default box color.
			//get the text size in groupbox
			Size tSize = TextRenderer.MeasureText((sender as GroupBox).Text, Font);

			e.Graphics.Clear(BackColor);
			//draw the border
			Rectangle borderRect = e.ClipRectangle;
			borderRect.Y = (borderRect.Y + (tSize.Height / 2));
			borderRect.Height = (borderRect.Height - (tSize.Height / 2));
			ControlPaint.DrawBorder(e.Graphics, borderRect, _borderColor, ButtonBorderStyle.Solid);

			//draw the text
			Rectangle textRect = e.ClipRectangle;
			textRect.X = (textRect.X + 6);
			textRect.Width = tSize.Width + 10;
			textRect.Height = tSize.Height;
			e.Graphics.FillRectangle(new SolidBrush(BackColor), textRect);
			e.Graphics.DrawString((sender as GroupBox).Text, Font, new SolidBrush(Color.FromArgb(221, 221, 221)), textRect);
		}
		#endregion

	}
}
