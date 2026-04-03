using Common.Controls;
using Common.Controls.Theme;
using System.IO.Ports;
using System.Windows.Forms;

namespace VixenModules.Output.GenericSerial
{
	public partial class SetupDialog : BaseForm
	{
		private Data _data;

		public SetupDialog(Data data)
		{
			InitializeComponent();
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
				UpdateSettingLabel();
				btnOkay.Enabled = true;
			}
			else {
				lblSettings.Text = "Not Set";
				btnOkay.Enabled = false;
			}
		}

		private void btnPortSetup_Click(object sender, EventArgs e)
		{
			using (SerialPortConfig serialPortConfig = new SerialPortConfig(Port)) {
				if (serialPortConfig.ShowDialog() == DialogResult.OK) {
					Port = serialPortConfig.SelectedPort;
					btnOkay.Enabled = true;
					UpdateSettingLabel();
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

			_data.BaudRate = Port.BaudRate;
			_data.DataBits = Port.DataBits;
			_data.Parity = Port.Parity;
			_data.PortName = Port.PortName;
			_data.StopBits = Port.StopBits;
		}

		private void UpdateSettingLabel()
		{
			lblSettings.Text = string.Format(
				"{0}: {1}, {2}, {3}, {4}",
				Port.PortName,
				Port.BaudRate,
				Port.Parity,
				Port.DataBits,
				Port.StopBits);
		}

		public SerialPort Port { private set; get; }

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}