using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Vixen.Module.Editor;
using Vixen.Module.SequenceType;
using Vixen.Services;
using Vixen.Sys;

namespace VixenModules.App.Shows
{
	public partial class WebPageTypeEditor : UserControl
	{
		public ShowItem _showItem;

		public WebPageTypeEditor(ShowItem showItem)
		{
			InitializeComponent();
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
		}

		private void buttonTest_Click(object sender, EventArgs e)
		{
			WebPageTypeTester f = new WebPageTypeTester(textBoxWebsite.Text);
			f.ShowDialog();
		}

	}
}
