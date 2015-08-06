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

namespace VixenApplication
{
	public partial class OptionsDialog : Form
	{
		public OptionsDialog()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
		}

		private void OptionsDialog_Load(object sender, EventArgs e)
		{
			ctlUpdateInteral.Value = Vixen.Sys.VixenSystem.DefaultUpdateInterval;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Vixen.Sys.VixenSystem.DefaultUpdateInterval = (int)ctlUpdateInteral.Value;
		}
		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = ThemeColorTable.newBackGroundImage ?? Resources.HeadingBackgroundImage;
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = ThemeColorTable.newBackGroundImageHover ?? Resources.HeadingBackgroundImageHover;
		}
	}
}
