using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using VixenModules.App.WebServer.Properties;
using Resources = Common.Resources.Properties.Resources;

namespace VixenModules.App.WebServer
{

	public partial class Settings : BaseForm
	{
		public Settings()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
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
			System.Diagnostics.Process.Start(linkLabel1.Text);
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
