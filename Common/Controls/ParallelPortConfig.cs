using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using Common.Controls.Theme;

namespace Common.Controls
{
	public partial class ParallelPortConfig : Form
	{
		private int _OtherAddressIndex;
		private int _DefaultPort;

		public ParallelPortConfig(int portAddress)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Properties.Resources.Icon_Vixen3;

			_OtherAddressIndex = 3;
			_DefaultPort = portAddress;
			switch (portAddress) {
				case 0x278:
					portComboBox.SelectedIndex = 1;
					break;

				case 0x378:
					portComboBox.SelectedIndex = 0;
					break;

				case 0x3bc:
					portComboBox.SelectedIndex = 2;
					break;

				case 0:
					portComboBox.SelectedIndex = 0;
					break;

				default:
					portTextBox.Text = portAddress.ToString("X4");
					portComboBox.SelectedIndex = 3;
					break;
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			portTextBox.Enabled = portComboBox.SelectedIndex == _OtherAddressIndex;
		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			if (portComboBox.SelectedIndex == _OtherAddressIndex) {
				if (PortAddress != 0) {
					try {
						Convert.ToUInt16(portTextBox.Text, 0x10);
					}
					catch {
						MessageBox.Show("The port number is not a valid hexadecimal number.", "Parallel Port Setup", MessageBoxButtons.OK,
						                MessageBoxIcon.Hand);
						base.DialogResult = DialogResult.None;
					}
				}
				else {
					MessageBox.Show("The port address is 0.", "Parallel Port Setup", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					DialogResult = DialogResult.None;
				}
			}
		}

		public ushort PortAddress
		{
			get
			{
				switch (portComboBox.SelectedIndex) {
					case 0:
						return 0x378;

					case 1:
						return 0x278;

					case 2:
						return 0x3bc;
				}
				return Convert.ToUInt16(portTextBox.Text, 0x10);
			}
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = ThemeColorTable.newBackGroundImageHover ?? Resources.Properties.Resources.HeadingBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = ThemeColorTable.newBackGroundImage ?? Resources.Properties.Resources.HeadingBackgroundImage;
		}

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}
	}
}