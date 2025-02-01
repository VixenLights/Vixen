using System.Diagnostics;
using Common.Controls;
using Common.Controls.Theme;
using Resources = Common.Resources.Properties.Resources;

namespace VixenModules.App.WebServer
{

	public partial class Settings : BaseForm
	{
		public Settings()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			label4.MaximumSize = new Size(Width-(int)(.1*Width), 0);
		}

		public Settings(Data _data)
			: this()
		{
			Port = _data.HttpPort;
			WebServerEnabled = _data.IsEnabled;
		}

		public event EventHandler<WebSettingsEventArgs> SettingsChanged;
		
		private void OnSettingsChanged()
		{
			if (SettingsChanged != null)
				SettingsChanged(this, new WebSettingsEventArgs(Port, WebServerEnabled));

		}
		private void setLinkLabel()
		{
			label3.Visible = linkLabel1.Visible = WebServerEnabled;
			linkLabel1.Text = string.Format("http://{0}:{1}/", System.Net.Dns.GetHostName(), Port);
		}

		public int Port
		{
			get { return (int)this.numericUpDown1.Value; }
			set
			{

				this.numericUpDown1.Value = value;
				setLinkLabel();
				OnSettingsChanged();
			}
		}
		public bool WebServerEnabled
		{
			get { return this.checkBox1.Checked; }
			set
			{
				this.checkBox1.Checked = value;
				setLinkLabel();
				OnSettingsChanged();
			}
		}

		private void ValueChanged(object sender, EventArgs e)
		{
			setLinkLabel();

		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var pi = new ProcessStartInfo()
			{
				FileName = linkLabel1.Text,
				UseShellExecute = true
			};
			Process.Start(pi);
		}
	}
}
