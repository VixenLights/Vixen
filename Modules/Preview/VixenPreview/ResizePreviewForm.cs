using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Preview.VixenPreview
{
	public partial class ResizePreviewForm : BaseForm
	{
		private int _origWidth, _origHeight;
		private int _newWidth, _newHeight;
        private bool _lockAspect = true;

		public ResizePreviewForm(int width, int height)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			_origWidth = width;
			_origHeight = height;
		}

		public new int Width
		{
			get { return _newWidth; }
		}

		public new int Height
		{
			get { return _newHeight; }
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			_newWidth = (int) numericWidth.Value;
			_newHeight = (int) numericHeight.Value;
			DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void ResizePreviewForm_Load(object sender, EventArgs e)
		{
			numericWidth.Value = _origWidth;
			numericHeight.Value = _origHeight;
			labelWidth.Text = _origWidth.ToString();
			labelHeight.Text = _origHeight.ToString();
            SetupAspect();
        }

		private void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Preview_Background);
		}

        private void pictureBoxLock_Click(object sender, EventArgs e)
        {
            _lockAspect = !_lockAspect;
            SetupAspect();
        }

        private void SetupAspect()
        {
            if (_lockAspect)
            {
                pictureBoxLock.Image = imageListLocks.Images["link"];
                numericHeight.Enabled = false;
            }
            else
            {
                pictureBoxLock.Image = imageListLocks.Images["unlink"];
                numericHeight.Enabled = true;
            }
        }

        private void numericWidth_ValueChanged(object sender, EventArgs e)
        {
            if (numericHeight.Value < 10) numericHeight.Value = 10;
            if (_lockAspect) { 
                double aspect = (double)numericWidth.Value / (double)_origWidth;
                numericHeight.Value = (int)((double)_origHeight * aspect);
            }
        }

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;

		}
	}
}