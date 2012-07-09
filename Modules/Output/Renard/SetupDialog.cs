using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace VixenModules.Output.Renard
{
	public partial class SetupDialog : Form {
		private Data _data;
		private SerialPort _port;

		public SetupDialog(Data data) {
			InitializeComponent();
			_data = data;
            if (data.ProtocolVersion > 0)
            {
                comboBoxProtocolVersion.SelectedIndex = _data.ProtocolVersion - 1;
            }
            if (_data.PortName != null)
            {
                _port = new SerialPort(_data.PortName, _data.BaudRate, _data.Parity,
                 _data.DataBits, _data.StopBits);
            }
		}

		private void buttonPortSetup_Click(object sender, EventArgs e) {
			using(Common.Controls.SerialPortConfig serialPortConfig = new Common.Controls.SerialPortConfig(_port)) {
				if(serialPortConfig.ShowDialog() == DialogResult.OK) {
					_port = serialPortConfig.SelectedPort;
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			_data.ProtocolVersion = comboBoxProtocolVersion.SelectedIndex + 1;

			_data.PortName = _port.PortName;
			_data.BaudRate = _port.BaudRate;
			_data.Parity = _port.Parity;
			_data.DataBits = _port.DataBits;
			_data.StopBits = _port.StopBits;
		}
	}
}
