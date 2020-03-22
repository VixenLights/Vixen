using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Output.Olsen595
{
	internal partial class SetupDialog : BaseForm
	{
		private Data _data;

		public SetupDialog(Data data)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			_data = data;
			_PortAddress = _data.Port;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (_PortAddress == 0) 
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Hand; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("The port address is 0.", "595 Setup", false, false);
				messageBox.ShowDialog();
				messageBox.DialogResult = DialogResult.None;
			}
			else {
				_data.Port = _PortAddress;
			}
		}

		private ushort _PortAddress
		{
			get
			{
				ushort value;
				if (ushort.TryParse(textBoxPortAddress.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value)) {
				}
				return value;
			}
			set { textBoxPortAddress.Text = value.ToString("X"); }
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
	}
}