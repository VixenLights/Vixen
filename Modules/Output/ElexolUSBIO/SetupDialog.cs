using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO.Ports;

namespace VixenModules.Output.ElexolUSBIO
{
	public partial class SetupDialog : Form
	{
		private int _minIntensity = 1;
		private ElexolUSBData _data;
		private SerialPort _serialPort;

		public SetupDialog(ElexolUSBData data)
		{
			InitializeComponent();

			_data = data;
			if (_data.PortName != null) {
				_serialPort = new SerialPort(_data.PortName, _data.BaudRate, _data.Parity,
									   _data.DataBits, _data.StopBits);
				updateSettingLabel();
			} else {
				lblSettings.Text = "Not Set";
				buttonOK.Enabled = false;
			}

			if (_data.MinimumIntensity > 0)
			{
				_minIntensity = _data.MinimumIntensity;
			}
			sliderMinIntensity.Value = _minIntensity;
			lblMinIntensity.Text = _minIntensity.ToString();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			_data.PortName = _serialPort.PortName;
			_data.BaudRate = _serialPort.BaudRate;
			_data.Parity = _serialPort.Parity;
			_data.DataBits = _serialPort.DataBits;
			_data.StopBits = _serialPort.StopBits;

			_data.MinimumIntensity = _minIntensity;
		}

		private void sliderMinIntensity_ValueChanged(object sender, EventArgs e)
		{
			_minIntensity = sliderMinIntensity.Value;
			lblMinIntensity.Text = _minIntensity.ToString();
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


		private void buttonPortSetup_Click(object sender, EventArgs e)
		{
			using (Common.Controls.SerialPortConfig serialPortConfig = new Common.Controls.SerialPortConfig(_serialPort)) {
				if (serialPortConfig.ShowDialog() == DialogResult.OK) {
					_serialPort = serialPortConfig.SelectedPort;
					buttonOK.Enabled = true;
					updateSettingLabel();
				}
			}
		}
	}
}