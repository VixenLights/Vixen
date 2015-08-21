using System;
using System.IO.Ports;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Output.GenericSerial
{
	public partial class SetupDialog : Form
	{
		private Data _data;
		private SerialPort _serialPort = null;

		public SetupDialog(Data data)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);

			_data = data;
			tbHeader.Enabled = false;
			tbFooter.Enabled = false;

			if (!string.IsNullOrEmpty(data.Header)) {
				cbHeader.Checked = true;
				tbHeader.Enabled = true;
				tbHeader.Text = data.Header;
			}

			if (!string.IsNullOrEmpty(data.Footer)) {
				cbFooter.Checked = true;
				tbFooter.Enabled = true;
				tbFooter.Text = data.Footer;
			}

			if (!string.IsNullOrEmpty(data.PortName)) {
				Port = new SerialPort(data.PortName, data.BaudRate, data.Parity, data.DataBits, data.StopBits);
				updateSettingLabel();
				btnOkay.Enabled = true;
			}
			else {
				lblSettings.Text = "Not Set";
				btnOkay.Enabled = false;
			}
		}

		private void btnPortSetup_Click(object sender, EventArgs e)
		{
			using (Common.Controls.SerialPortConfig serialPortConfig = new Common.Controls.SerialPortConfig(Port)) {
				if (serialPortConfig.ShowDialog() == DialogResult.OK) {
					Port = serialPortConfig.SelectedPort;
					btnOkay.Enabled = true;
					updateSettingLabel();
				}
			}
		}

		private void cbHeader_CheckedChanged(object sender, EventArgs e)
		{
			tbHeader.Enabled = cbHeader.Checked;
		}

		private void cbFooter_CheckedChanged(object sender, EventArgs e)
		{
			tbFooter.Enabled = cbFooter.Checked;
		}

		private void btnOkay_Click(object sender, EventArgs e)
		{
			_data.Header = cbHeader.Checked ? tbHeader.Text : string.Empty;
			_data.Footer = cbFooter.Checked ? tbFooter.Text : string.Empty;

			_data.BaudRate = _serialPort.BaudRate;
			_data.DataBits = _serialPort.DataBits;
			_data.Parity = _serialPort.Parity;
			_data.PortName = _serialPort.PortName;
			_data.StopBits = _serialPort.StopBits;
		}

		private void updateSettingLabel()
		{
			lblSettings.Text = string.Format(
				"{0}: {1}, {2}, {3}, {4}",
				_serialPort.PortName,
				_serialPort.BaudRate,
				_serialPort.Parity,
				_serialPort.DataBits,
				_serialPort.StopBits);
		}

		public SerialPort Port
		{
			set { _serialPort = value; }
			get { return _serialPort; }
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