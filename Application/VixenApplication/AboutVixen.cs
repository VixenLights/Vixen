using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Sys;

namespace VixenApplication
{
	public partial class AboutVixen : BaseForm
	{
		public AboutVixen(string currentVersion, bool devBuild)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
			pictureBoxIcon.Image = Resources.VixenImage;
			labelHeading.Font = new Font(labelHeading.Font.Name, 20F);
			string currentVersionType = devBuild ? " Build " : " Release ";
			labelHeading.Text += currentVersionType + currentVersion;
		}

		private void AboutVixen_Load(object sender, EventArgs e)
		{
			textBoxLicense.Text = File.ReadAllText(Paths.BinaryRootPath + "//License.txt");
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			
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
