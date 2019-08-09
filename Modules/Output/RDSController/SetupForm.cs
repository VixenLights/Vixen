using System;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Resources = Common.Resources.Properties.Resources;

namespace VixenModules.Output.CommandController
{
	public partial class SetupForm : BaseForm
	{
		public Data RdsData { get; set; }

		public SetupForm(Data data)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			cboPortName.Items.Clear();

			var ports =	SerialPort.GetPortNames().OrderBy(o => o).ToList();
			ports.ForEach(a => cboPortName.Items.Add((a)));

			RdsData = data;
			chkRequiresAuthentication.Checked= data.RequireHTTPAuthentication;

			txtHttpUsername.Text= data.HttpUsername;
			txtHttpPassword.Text= data.HttpPassword;

			chkBiDirectional.Checked= data.BiDirectional;
			chkSlow.Checked= data.Slow;
			txtUrl.Text = data.HttpUrl ?? "http://127.0.0.1:8080/?action=update_rt&update_rt={text}";
			if (ports.Contains(data.PortName))
			{
				cboPortName.SelectedItem = data.PortName;
			}
			 
			switch (data.HardwareID) {
				case Hardware.MRDS1322:
					radioMRDS1322.Checked=true;
					break;
				case Hardware.MRDS192:
					radioMRDS192.Checked=true;
					break;
				case Hardware.VFMT212R:
					radioVFMT212R.Checked=true;
					break;
				case Hardware.HTTP:
					radioHttp.Checked=true;
					break;
			}
			chkHideLaunchedWindows.Checked= data.HideLaunchedWindows;
		}

		private void radioVFMT212R_CheckedChanged(object sender, EventArgs e)
		{
			cboPortName.Enabled= !radioVFMT212R.Checked;
			RdsData.HardwareID =  Hardware.VFMT212R;
			SetFormDefaults();
		}

		private void radioMRDS1322_CheckedChanged(object sender, EventArgs e)
		{
			RdsData.HardwareID =  Hardware.MRDS1322;
			RdsData.ConnectionMode= 1;
			SetFormDefaults();
		}

		private void radioMRDS192_CheckedChanged(object sender, EventArgs e)
		{
			RdsData.HardwareID =   Hardware.MRDS192;
			RdsData.ConnectionMode=0;
			SetFormDefaults();
		}

		private void SetFormDefaults()
		{
			switch (RdsData.HardwareID) {
				case Hardware.MRDS192:
				case Hardware.MRDS1322:
					txtPSInterface.MaxLength=8;
					txtUrl.Enabled=	lblUrl.Enabled=lblPassword.Enabled=lblUserName.Enabled=chkRequiresAuthentication.Enabled=txtHttpPassword.Enabled=txtHttpUsername.Enabled=false;
					break;
				case Hardware.VFMT212R:
					txtUrl.Enabled=lblUrl.Enabled=lblPassword.Enabled=lblUserName.Enabled=	chkRequiresAuthentication.Enabled=txtHttpPassword.Enabled=txtHttpUsername.Enabled=true;
					txtPSInterface.MaxLength=64;
					chkRequiresAuthentication.Checked=true;
					chkRequiresAuthentication.Enabled=false;
					if (!RdsData.HttpUrl.ToLower().EndsWith(@"?action=update_rt&update_rt={text}") || string.IsNullOrWhiteSpace(RdsData.HttpUrl))
						txtUrl.Text =   "http://127.0.0.1:8080/?action=update_rt&update_rt={text}";
					break;
				case Hardware.HTTP:
					txtUrl.Enabled=lblUrl.Enabled=true;
					lblPassword.Enabled=lblUserName.Enabled=chkRequiresAuthentication.Enabled=txtHttpPassword.Enabled=txtHttpUsername.Enabled=true;
					txtPSInterface.MaxLength=128;
					break;
			}
		}
		private void radioHttp_CheckedChanged(object sender, EventArgs e)
		{
			RdsData.HardwareID= Hardware.HTTP;
			txtUrl.Enabled = lblUrl.Enabled = radioHttp.Checked;
			txtUrl.ReadOnly= false;
			SetFormDefaults();

		}

		private void btnTX_Click(object sender, EventArgs e)
		{
			if (Module.Send(RdsData, txtPSInterface.Text, true)) {
				StatusLbl1.Text="Data Sent";
			} else {
				StatusLbl1.Text="Data Not Sent";
			}
		}

		private void chkHideLaunchedWindows_CheckedChanged(object sender, EventArgs e)
		{
			RdsData.HideLaunchedWindows= chkHideLaunchedWindows.Checked;
		}

		private void chkBiDirectional_CheckedChanged(object sender, EventArgs e)
		{
			RdsData.BiDirectional=chkBiDirectional.Checked;
		}

		private void chkSlow_CheckedChanged(object sender, EventArgs e)
		{
			RdsData.Slow=chkSlow.Checked;
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			try {
				switch (RdsData.HardwareID) {
					case Hardware.MRDS192:
					case Hardware.MRDS1322:
						//NativeMethods.Disconnect();
						break;
					case Hardware.VFMT212R:
						break;
				}

			} catch (Exception) { }

			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void txtUrl_TextChanged(object sender, EventArgs e)
		{
			RdsData.HttpUrl= txtUrl.Text;
		}

		private void chkRequiresAuthentication_CheckedChanged(object sender, EventArgs e)
		{
			RdsData.RequireHTTPAuthentication= chkRequiresAuthentication.Checked;
		}

		private void txtHttpUsername_TextChanged(object sender, EventArgs e)
		{
			RdsData.HttpUsername=txtHttpUsername.Text;
		}

		private void txtHttpPassword_TextChanged(object sender, EventArgs e)
		{
			RdsData.HttpPassword=txtHttpPassword.Text;
		}

		private void cboPortName_SelectedIndexChanged(object sender, EventArgs e)
		{
			RdsData.PortNumber = int.Parse(cboPortName.SelectedItem.ToString().Replace("COM", ""));
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

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}
