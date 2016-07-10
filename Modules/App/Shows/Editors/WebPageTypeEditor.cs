using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;
using Vixen.Module.Editor;
using Vixen.Module.SequenceType;
using Vixen.Services;
using Vixen.Sys;

namespace VixenModules.App.Shows
{
	public partial class WebPageTypeEditor : TypeEditorBase
	{
		public ShowItem _showItem;

		public WebPageTypeEditor(ShowItem showItem)
		{
			InitializeComponent();

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			ThemeUpdateControls.UpdateControls(this);
			buttonTest.Image = Tools.GetIcon(Resources.cog_go, iconSize);
			buttonTest.Text = "";

			_showItem = showItem;
		}

		private void SequenceTypeEditor_Load(object sender, EventArgs e)
		{
			textBoxWebsite.Text = _showItem.Website_URL;
		}

		private void textBoxWebsite_TextChanged(object sender, EventArgs e)
		{
			_showItem.Website_URL = textBoxWebsite.Text;
			_showItem.Name = "Web page: " + _showItem.Website_URL;
			FireChanged(_showItem.Name);
		}

		private void buttonTest_Click(object sender, EventArgs e)
		{
			WebPageTypeTester f = new WebPageTypeTester(textBoxWebsite.Text);
			f.ShowDialog();
		}

	}
}
