using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO.Ports;

namespace VixenModules.Output.FGDimmer
{
	internal partial class SetupDialog : Form
	{
		private SerialPort _serialPort;
		private FGDimmerControlModule[] _modules;
		private FGDimmerData _data;

		public SetupDialog(FGDimmerData data)
		{
			InitializeComponent();
			_data = data;
			int startchannel;


			if (_data.PortName != null) {
				_serialPort = new SerialPort(_data.PortName, _data.BaudRate, _data.Parity,
				                             _data.DataBits, _data.StopBits);

				updateSettingLabel();
			}
			_modules = _data.Modules;

			if (_data.StartChannel == 0) {
				startchannel = _data.StartChannel + 1;
			}
			else {
				startchannel = _data.StartChannel;
			}

			for (; startchannel <= _data.EndChannel; startchannel++) {
				comboBoxModule1.Items.Add(startchannel);
				comboBoxModule2.Items.Add(startchannel);
				comboBoxModule3.Items.Add(startchannel);
				comboBoxModule4.Items.Add(startchannel);
			}

			if (_modules != null) {
				checkBoxModule1.Checked = _modules[0].Enabled;
				if (checkBoxModule1.Checked) {
					if (_modules[0].StartChannel >= (int) comboBoxModule1.Items[0]) {
						comboBoxModule1.SelectedItem = _modules[0].StartChannel;
					}
				}

				checkBoxModule2.Checked = _modules[1].Enabled;
				if (checkBoxModule2.Checked) {
					if (_modules[1].StartChannel >= (int) comboBoxModule2.Items[0]) {
						comboBoxModule2.SelectedItem = _modules[1].StartChannel;
					}
				}

				checkBoxModule3.Checked = _modules[2].Enabled;
				if (checkBoxModule3.Checked) {
					if (_modules[2].StartChannel >= (int) comboBoxModule3.Items[0]) {
						comboBoxModule3.SelectedItem = _modules[2].StartChannel;
					}
				}

				checkBoxModule4.Checked = _modules[3].Enabled;
				if (checkBoxModule4.Checked) {
					if (_modules[3].StartChannel >= (int) comboBoxModule4.Items[0]) {
						comboBoxModule4.SelectedItem = _modules[3].StartChannel;
					}
				}
			}

			checkBoxHoldPort.Checked = _data.HoldPortOpen;
			if (_data.AcOperation) {
				radioButtonAC.Checked = true;
			}
			else {
				radioButtonPWM.Checked = true;
			}
		}

		public SerialPort SelectedPort
		{
			get { return _serialPort; }
		}

		public FGDimmerControlModule[] Modules
		{
			get { return _modules; }
		}

		public bool UsingModule1
		{
			get { return checkBoxModule1.Checked; }
		}

		public bool UsingModule2
		{
			get { return checkBoxModule2.Checked; }
		}

		public bool UsingModule3
		{
			get { return checkBoxModule3.Checked; }
		}

		public bool UsingModule4
		{
			get { return checkBoxModule4.Checked; }
		}

		public int Module1StartChannel
		{
			get { return (int) comboBoxModule1.SelectedItem; }
		}

		public int Module2StartChannel
		{
			get { return (int) comboBoxModule2.SelectedItem; }
		}

		public int Module3StartChannel
		{
			get { return (int) comboBoxModule3.SelectedItem; }
		}

		public int Module4StartChannel
		{
			get { return (int) comboBoxModule4.SelectedItem; }
		}

		public bool HoldPort
		{
			get { return checkBoxHoldPort.Checked; }
		}

		public bool ACOperation
		{
			get { return radioButtonAC.Checked; }
		}

		private void buttonSerialSetup_Click(object sender, EventArgs e)
		{
			using (Common.Controls.SerialPortConfig serialSetupDialog = new Common.Controls.SerialPortConfig(_serialPort)) {
				if (serialSetupDialog.ShowDialog() == DialogResult.OK) {
					_serialPort = serialSetupDialog.SelectedPort;
					updateSettingLabel();
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (_modules[0].Enabled = checkBoxModule1.Checked) {
				_modules[0].StartChannel = (int) comboBoxModule1.SelectedItem;
			}
			if (_modules[1].Enabled = checkBoxModule2.Checked) {
				_modules[1].StartChannel = (int) comboBoxModule2.SelectedItem;
			}
			if (_modules[2].Enabled = checkBoxModule3.Checked) {
				_modules[2].StartChannel = (int) comboBoxModule3.SelectedItem;
			}
			if (_modules[3].Enabled = checkBoxModule4.Checked) {
				_modules[3].StartChannel = (int) comboBoxModule4.SelectedItem;
			}

			_data.BaudRate = _serialPort.BaudRate;
			_data.DataBits = _serialPort.DataBits;
			_data.StopBits = _serialPort.StopBits;
			_data.Parity = _serialPort.Parity;
			_data.PortName = _serialPort.PortName;

			_data.AcOperation = ACOperation;
			_data.HoldPortOpen = HoldPort;

			_data.Modules = _modules;
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
	}
}