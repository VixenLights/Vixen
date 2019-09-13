using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using FTD2XX_NET;
using NLog;

namespace VixenModules.Controller.OpenDMX
{
	internal partial class SetupDialog : BaseForm
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private OpenDMXData _data;

		public SetupDialog(OpenDMXData data)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			_data = data;
			var devices = OpenDmx.GetDeviceList();

			if (devices.Any())
			{
				foreach (var device in devices)
				{
					ComboBoxItem item = new ComboBoxItem($"{device.Description} - {device.SerialNumber} - {device.Id}", device); 
					cmbDeviceList.Items.Add(item);
				}

				if (_data.Device != null)
				{
					var deviceIndex = devices.FindIndex(x => x.SerialNumber == _data.Device.SerialNumber &&
					                                x.Id == _data.Device.Id &&
					                                x.Description == _data.Device.Description);
					if (deviceIndex>=0)
					{
						cmbDeviceList.SelectedIndex = deviceIndex;
					}
					else
					{
						cmbDeviceList.SelectedIndex = 0;
					}
				}
				else
				{
					cmbDeviceList.SelectedIndex = 0;
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (cmbDeviceList.Items.Count > 0)
			{
				var item = cmbDeviceList.SelectedItem as ComboBoxItem;
				_data.Device = item.Value as Device;
			}
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