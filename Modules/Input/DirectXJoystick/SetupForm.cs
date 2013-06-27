using System;
using System.Windows.Forms;

namespace VixenModules.Input.DirectXJoystick
{
	internal partial class SetupForm : Form
	{
		private DirectXJoystickData _data;

		public SetupForm(DirectXJoystickData data)
		{
			InitializeComponent();

			_data = data;

			comboBoxJoystick.DisplayMember = "DeviceName";
			comboBoxJoystick.ValueMember = "DeviceId";
			comboBoxJoystick.DataSource = Joystick.AllJoysticks();

			if (_data.DeviceId != Guid.Empty) {
				comboBoxJoystick.SelectedValue = _data.DeviceId;
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (comboBoxJoystick.SelectedItem != null) {
				_data.DeviceId = (Guid) comboBoxJoystick.SelectedValue;
			}
			else {
				_data.DeviceId = Guid.Empty;
			}
		}
	}
}