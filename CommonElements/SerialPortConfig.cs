using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace CommonElements {
	public partial class SerialPortConfig : Form {
		public SerialPortConfig(SerialPort serialPort, bool allowPortEdit = true, bool allowBaudEdit = true, bool allowParityEdit = true, bool allowDataEdit = true, bool allowStopEdit = true) {
			InitializeComponent();

			comboBoxPortName.Items.AddRange(SerialPort.GetPortNames());
			comboBoxPortName.Enabled = allowPortEdit;
			comboBoxBaudRate.Enabled = allowBaudEdit;
			comboBoxParity.Enabled = allowParityEdit;
			textBoxDataBits.Enabled = allowDataEdit;
			comboBoxStopBits.Enabled = allowStopEdit;

			comboBoxParity.Items.AddRange(Enum.GetValues(typeof(Parity)).Cast<object>().ToArray());
			comboBoxStopBits.Items.AddRange(Enum.GetValues(typeof(StopBits)).Cast<object>().ToArray());

			if(serialPort == null) {
				serialPort = new SerialPort("COM1", 38400, Parity.None, 8, StopBits.One);
			}

			SelectedPort = serialPort;

			
			
			
			
		}

		public SerialPort SelectedPort {
			get { return new SerialPort(_PortName, _BaudRate, _Parity, _DataBits, _StopBits); }
			set {
				_PortName = value.PortName;
				_BaudRate = value.BaudRate;
				_Parity = value.Parity;
				_DataBits = value.DataBits;
				_StopBits = value.StopBits;
			}
		}

		private string _PortName {
			get { return comboBoxPortName.SelectedItem as string; }
			set { comboBoxPortName.SelectedItem = value; }
		}

		private int _BaudRate {
			get {
				int value;
				if(int.TryParse(comboBoxBaudRate.SelectedItem as string, out value)) {
					return value;
				}
				return -1;
			}
			set { comboBoxBaudRate.SelectedItem = value.ToString(); }
		}

		private Parity _Parity {
			get { return (Parity)comboBoxParity.SelectedItem; }
			set { comboBoxParity.SelectedItem = value; }
		}

		private int _DataBits {
			get {
				int value;
				if(int.TryParse(textBoxDataBits.Text, out value)) {
					return value;
				}
				return -1;
			}
			set { textBoxDataBits.Text = value.ToString(); }
		}

		private StopBits _StopBits {
			get { return (StopBits)comboBoxStopBits.SelectedItem; }
			set { comboBoxStopBits.SelectedItem = value; }
		}

		private bool _Validate() {
			StringBuilder sb = new StringBuilder();

			if(string.IsNullOrWhiteSpace(_PortName)) {
				sb.AppendLine("* Port name has not been selected.");
			}
			if(_BaudRate == -1) {
				sb.AppendLine("* Baud rate has not been selected.");
			}
			if(_DataBits == -1) {
				sb.AppendLine("* Invalid value for data bits.");
			}

			if(sb.Length > 0) {
				MessageBox.Show("The following items need to be resolved:" + Environment.NewLine + Environment.NewLine + sb, "Serial Port", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				return false;
			}

			return true;
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			if(!_Validate()) {
				DialogResult = DialogResult.None;
			}
		}
	}
}
