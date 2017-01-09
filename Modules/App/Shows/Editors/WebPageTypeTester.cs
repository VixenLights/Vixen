using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;

namespace VixenModules.App.Shows
{
	public partial class WebPageTypeTester : BaseForm
	{
		public WebPageTypeTester(string url)
		{
			InitializeComponent();
			webBrowser.Url = new Uri(url);
			labelURL.Text = "URL: " + url;
		}
	}
}
