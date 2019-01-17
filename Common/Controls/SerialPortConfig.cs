using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using Common.Controls.Theme;

namespace Common.Controls
{
	public partial class SerialPortConfig : BaseForm
	{
		public SerialPortConfig(SerialPort serialPort, bool allowPortEdit = true, bool allowBaudEdit = true,
		                        bool allowParityEdit = true, bool allowDataEdit = true, bool allowStopEdit = true)
		{
            var logging = NLog.LogManager.GetCurrentClassLogger();
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Properties.Resources.Icon_Vixen3;

			//lets try and open the serial port if it can't be opened then it
			//must be in use so label it as in use
			foreach (string s in SerialPort.GetPortNames()) {
			    try
			    {
			        using (SerialPort checkPort = new SerialPort(s))
			        {
			            checkPort.Open();
			            checkPort.Close();
			        }
			        comboBoxPortName.Items.Add(s);
			    }

			    catch (Exception e)
			    {
                    if ((e is UnauthorizedAccessException) || 
                        (e is InvalidOperationException))
                    {
                        comboBoxPortName.Items.Add(s + " (IN USE)");
                        continue;
                    }
                    else
                    {
                        logging.Warn("Serial port exception encountered while scanning", e);
                        continue;
                    }
                }
			}
			comboBoxPortName.Enabled = allowPortEdit;
			comboBoxBaudRate.Enabled = allowBaudEdit;
			comboBoxParity.Enabled = allowParityEdit;
			textBoxDataBits.Enabled = allowDataEdit;
			comboBoxStopBits.Enabled = allowStopEdit;

			comboBoxParity.Items.AddRange(Enum.GetValues(typeof (Parity)).Cast<object>().ToArray());
			var stopBits = Enum.GetValues(typeof(StopBits)).Cast<object>().ToList();
			stopBits.Remove(StopBits.None);
			comboBoxStopBits.Items.AddRange(stopBits.ToArray());

			//set our text value
			if (serialPort != null) {
				configuredPortValueLabel.Text = serialPort.PortName;
			}
			else {
				configuredPortValueLabel.Text = "None";
			}

            if (serialPort == null && SerialPort.GetPortNames().Any())
            {
				serialPort = new SerialPort(SerialPort.GetPortNames().FirstOrDefault(), 57600, Parity.None, 8, StopBits.One);
			}

			SelectedPort = serialPort;

			//set our first item in the combobox if we have one
			if (comboBoxPortName.Items.Count > 0) {
				comboBoxPortName.SelectedIndex = 0;
			}
		}

		public SerialPort SelectedPort
		{
			get
			{
				if (_HavePorts) {
					return new SerialPort(_PortName, _BaudRate, _Parity, _DataBits, _StopBits);
				}
				return null;
			}
			set
			{
				if (value != null) {
					_HavePorts = true;
					_PortName = value.PortName;
					_BaudRate = value.BaudRate;
					_Parity = value.Parity;
					_DataBits = value.DataBits;
					_StopBits = value.StopBits;
				}
				else {
					_HavePorts = false;
				}
			}
		}

		private bool _HavePorts
		{
			get { return groupBox.Enabled; }
			set { groupBox.Enabled = value; }
		}

		private string _PortName
		{
			get
			{
				if (comboBoxPortName.SelectedItem != null) {
					//if the portname has (IN USE) we need to strip it so we have
					//a valid serial port name.
					string value;
					string port = comboBoxPortName.SelectedItem as string;
					if (port.Contains("(IN USE)")) {
						value = port.Replace("(IN USE)", string.Empty).Trim();
						return value;
					}
					else {
						return comboBoxPortName.SelectedItem as string;
					}
				}
				else {
					return null;
				}
			}
			set { comboBoxPortName.SelectedItem = value; }
		}

		private int _BaudRate
		{
			get
			{
				int value;
				if (int.TryParse(comboBoxBaudRate.SelectedItem as string, out value)) {
					return value;
				}
				return -1;
			}
			set { comboBoxBaudRate.SelectedItem = value.ToString(); }
		}

		private Parity _Parity
		{
			get { return (Parity) comboBoxParity.SelectedItem; }
			set { comboBoxParity.SelectedItem = value; }
		}

		private int _DataBits
		{
			get
			{
				int value;
				if (int.TryParse(textBoxDataBits.Text, out value)) {
					return value;
				}
				return -1;
			}
			set { textBoxDataBits.Text = value.ToString(); }
		}

		private StopBits _StopBits
		{
			get { return (StopBits) comboBoxStopBits.SelectedItem; }
			set { comboBoxStopBits.SelectedItem = value; }
		}

		private bool _Validate()
		{
			if (_HavePorts) {
				StringBuilder sb = new StringBuilder();

				if (string.IsNullOrWhiteSpace(_PortName)) {
					sb.AppendLine("* Port name has not been selected.");
				}
				if (_BaudRate == -1) {
					sb.AppendLine("* Baud rate has not been selected.");
				}
				if (_DataBits == -1) {
					sb.AppendLine("* Invalid value for data bits.");
				}

				if (sb.Length > 0) {
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Hand; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm(string.Format("The following items need to be resolved:{0}{0}{1}", Environment.NewLine, sb),
					                "Serial Port", false, false);
					messageBox.ShowDialog();
					return false;
				}
				else {
					return true;
				}
			}

			return false;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (!_Validate()) {
				DialogResult = DialogResult.None;
			}
			else {
				//Since we strip out the (IN USE) in the getter/setter
				//we need to go ahead and pull from the combo box to check
				//if we have an in use port.  If we do, just let the user know
				//and allow them to continue.  We will handle the access violations
				//in the module classes
				string port = comboBoxPortName.SelectedItem as string;
				if (port.Contains("(IN USE)")) {
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Serial Port may be in use, do you wish to continue?", "Warning", true, false);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.No)
					{
						DialogResult = DialogResult.None;
					}
				}
			}
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImage;

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