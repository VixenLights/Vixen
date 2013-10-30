using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.App.Shows
{
	public partial class WebPageTypeTester : Form
	{
		public WebPageTypeTester(string url)
		{
			InitializeComponent();
			webBrowser.Url = new Uri(url);
			labelURL.Text = "URL: " + url;
		}
	}
}
