using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VixenModules.Output.CommandController
{
	public partial class SetupForm : Form
	{
		public Data RdsData { get; set; }

		public SetupForm(Data data)
		{
			InitializeComponent();
			cboPortName.Items.Clear();

			var ports =	SerialPort.GetPortNames().OrderBy(o => o).ToList();
			ports.ForEach(a => cboPortName.Items.Add((a)));

			RdsData = data;
			chkRequiresAuthentication.Checked= data.RequireHTTPAuthentication;

			txtHttpUsername.Text= data.HttpUsername;
			txtHttpPassword.Text= data.HttpPassword;

			chkBiDirectional.Checked= data.BiDirectional;
			chkSlow.Checked= data.Slow;
			this.txtUrl.Text = data.HttpUrl ?? "http://127.0.0.1:8080/?action=update_rt&update_rt={text}";
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
					this.txtPSInterface.MaxLength=8;
					this.txtUrl.Enabled=	this.lblPassword.Enabled=this.lblUserName.Enabled=	this.chkRequiresAuthentication.Enabled=this.txtHttpPassword.Enabled=this.txtHttpUsername.Enabled=false;
					break;
				case Hardware.VFMT212R:
					this.txtUrl.Enabled=this.lblPassword.Enabled=this.lblUserName.Enabled=	this.chkRequiresAuthentication.Enabled=this.txtHttpPassword.Enabled=this.txtHttpUsername.Enabled=true;
					this.txtPSInterface.MaxLength=64;
					chkRequiresAuthentication.Checked=true;
					chkRequiresAuthentication.Enabled=false;
					if (!RdsData.HttpUrl.ToLower().EndsWith(@"?action=update_rt&update_rt={text}") || string.IsNullOrWhiteSpace(RdsData.HttpUrl))
						this.txtUrl.Text =   "http://127.0.0.1:8080/?action=update_rt&update_rt={text}";
					break;
				case Hardware.HTTP:
					this.txtUrl.Enabled=true;
					this.lblPassword.Enabled=this.lblUserName.Enabled=	this.chkRequiresAuthentication.Enabled=this.txtHttpPassword.Enabled=this.txtHttpUsername.Enabled=true;
					this.txtPSInterface.MaxLength=128;
					break;
				default:
					break;
			}
		}
		private void radioHttp_CheckedChanged(object sender, EventArgs e)
		{
			RdsData.HardwareID= Hardware.HTTP;
			this.txtUrl.Enabled = radioHttp.Checked;
			this.txtUrl.ReadOnly= false;
			SetFormDefaults();

		}

		private void radioPorts_CheckedChanged(object sender, EventArgs e)
		{
			var button = (RadioButton)sender;
			RdsData.PortName= button.Text;
			switch (button.Text) {
				case "LPT1":
					RdsData.PortNumber = 0x378;
					break;
				case "COM1":
					RdsData.PortNumber = 1;
					break;
				case "COM2":
					RdsData.PortNumber = 2;
					break;
				case "COM3":
					RdsData.PortNumber = 3;
					break;
				case "COM4":
					RdsData.PortNumber = 4;
					break;
				case "COM6":
					RdsData.PortNumber = 6;

					break;
				default:
					throw new NotImplementedException();

			}
		}

		private void btnTX_Click(object sender, EventArgs e)
		{
			if (Module.Send(RdsData, txtPSInterface.Text)) {
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
						NativeMethods.Disconnect();
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
			//Regex urlRx = new Regex(@"^((http|https)://)?([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?$", RegexOptions.IgnoreCase);

			//if (urlRx.IsMatch(txtUrl.Text)) {
			RdsData.HttpUrl= txtUrl.Text;
			//	StatusLbl1.Text="";
			//} else
			//	StatusLbl1.Text= "Http Url is NOT well formed and will not be saved";
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











	}
}
