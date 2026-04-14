using Common.Controls;

namespace VixenModules.App.Shows.Editors
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
