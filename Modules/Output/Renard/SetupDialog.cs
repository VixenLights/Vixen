using System;
using System.Windows.Forms;
using System.IO.Ports;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Output.Renard
{
	public partial class SetupDialog : BaseForm
	{
		private Data _data;
		private SerialPort _port;

		public SetupDialog(Data data)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			_data = data;
			if (data.ProtocolVersion > 0) {
				comboBoxProtocolVersion.SelectedIndex = _data.ProtocolVersion - 1;
			}
			if (_data.PortName != null) {
				_port = new SerialPort(_data.PortName, _data.BaudRate, _data.Parity,
				                       _data.DataBits, _data.StopBits);
			}
		}

		private void buttonPortSetup_Click(object sender, EventArgs e)
		{
			using (Common.Controls.SerialPortConfig serialPortConfig = new Common.Controls.SerialPortConfig(_port)) {
				if (serialPortConfig.ShowDialog() == DialogResult.OK) {
					_port = serialPortConfig.SelectedPort;
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			_data.ProtocolVersion = comboBoxProtocolVersion.SelectedIndex + 1;

			_data.PortName = _port.PortName;
			_data.BaudRate = _port.BaudRate;
			_data.Parity = _port.Parity;
			_data.DataBits = _port.DataBits;
			_data.StopBits = _port.StopBits;
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

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}
	}
}