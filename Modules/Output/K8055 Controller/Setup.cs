using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.Output.K8055_Controller
{
	public partial class Setup : Form
	{
		public Setup(int channelCount, int[] deviceStartChannels)
		{
			InitializeComponent();
			int numberofChannels = channelCount - 7;
			numericUpDownDev0.Maximum = numericUpDownDev1.Maximum = numericUpDownDev2.Maximum = numericUpDownDev3.Maximum = numberofChannels;
			numericUpDownDev0.Value = Math.Min(deviceStartChannels[0] + 1, numberofChannels);
			UpdateRange(numericUpDownDev0, labelDev0);
			numericUpDownDev1.Value = Math.Min(deviceStartChannels[1] + 1, numberofChannels);
			UpdateRange(numericUpDownDev1, labelDev1);
			numericUpDownDev2.Value = Math.Min(deviceStartChannels[2] + 1, numberofChannels);
			UpdateRange(numericUpDownDev2, labelDev2);
			numericUpDownDev3.Value = Math.Min(deviceStartChannels[3] + 1, numberofChannels);
			UpdateRange(numericUpDownDev3, labelDev3);
			SearchDevices();
		}

		private void driverVersionButton_Click(object sender, EventArgs e)
		{
			K8055DLLWrapper.Version();
		}

		private void searchDevicesButton_Click(object sender, EventArgs e)
		{
			SearchDevices();
		}

		private void numericUpDownDev0_ValueChanged(object sender, EventArgs e)
		{
			UpdateRange(numericUpDownDev0, labelDev0);
		}

		private void numericUpDownDev1_ValueChanged(object sender, EventArgs e)
		{
			UpdateRange(numericUpDownDev1, labelDev1);
		}

		private void numericUpDownDev2_ValueChanged(object sender, EventArgs e)
		{
			UpdateRange(numericUpDownDev2, labelDev2);
		}

		private void numericUpDownDev3_ValueChanged(object sender, EventArgs e)
		{
			UpdateRange(numericUpDownDev3, labelDev3);
		}

		private void SearchDevices()
		{
			long deviceNum = 0L;
			Cursor = Cursors.WaitCursor;
			try
			{
				deviceNum = K8055DLLWrapper.SearchDevices();
				checkBoxDev0.Checked = (deviceNum & 1L) != 0L;
				checkBoxDev1.Checked = (deviceNum & 2L) != 0L;
				checkBoxDev2.Checked = (deviceNum & 4L) != 0L;
				checkBoxDev3.Checked = (deviceNum & 8L) != 0L;
			}
			finally
			{
				Cursor = Cursors.Default;
			}
			if ((deviceNum & 15L) == 0L)
			{
				MessageBox.Show("No devices were found.", "Vixen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void UpdateRange(NumericUpDown upDownStart, Label labelEnd)
		{
			labelEnd.Text = string.Format("to {0}", upDownStart.Value + 7M);
		}

		public int[] DeviceStartChannels
		{
			get
			{
				return new int[]{
					(((int)numericUpDownDev0.Value)-1), (((int)numericUpDownDev1.Value)-1),(((int)numericUpDownDev2.Value)-1),(((int)numericUpDownDev3.Value)-1)};
			}
		}
	}
}
