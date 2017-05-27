using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using VixenModules.Output.LauncherController.Properties;
using Common.Resources.Properties;
using Resources = Common.Resources.Properties.Resources;

namespace VixenModules.Output.LauncherController
{
	public partial class SetupForm : BaseForm
	{
		public Data LauncherData { get; set; }

		public SetupForm(Data data)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			chkHideLaunchedWindows.Checked= data.HideLaunchedWindows;
		}

		private void chkHideLaunchedWindows_CheckedChanged(object sender, EventArgs e)
		{
            LauncherData.HideLaunchedWindows = chkHideLaunchedWindows.Checked;
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
