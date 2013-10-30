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
		private K8055ControlModule[] _modules;
		private K8055Data _data;

		public Setup(int channelCount, K8055Data data)
		{
			InitializeComponent();
			_data = data;
			int numberofChannels = 0;
			
			if (channelCount > 7)
			{
				numberofChannels = channelCount - 7;
			}
			else
			{
				numberofChannels =  32;
			}

			numericUpDownDev0.Maximum = numericUpDownDev1.Maximum = numericUpDownDev2.Maximum = numericUpDownDev3.Maximum = numberofChannels;

			_modules = data.Modules;

			if (_modules != null)
			{
				checkBoxDev0.Checked = _modules[0].Enabled;
				if (checkBoxDev0.Checked)
				{
					numericUpDownDev0.Value = Math.Min(_modules[0].StartChannel + 1, numberofChannels);
					UpdateRange(numericUpDownDev0, labelDev0);
				}
				checkBoxDev1.Checked = _modules[1].Enabled;
				if (checkBoxDev1.Checked)
				{
					numericUpDownDev1.Value = Math.Min(_modules[1].StartChannel + 1, numberofChannels);
					UpdateRange(numericUpDownDev1, labelDev1);
				}
				checkBoxDev2.Checked = _modules[2].Enabled;
				if (checkBoxDev2.Checked)
				{
					numericUpDownDev2.Value = Math.Min(_modules[2].StartChannel + 1, numberofChannels);
					UpdateRange(numericUpDownDev2, labelDev2);
				}
				checkBoxDev3.Checked = _modules[3].Enabled;
				if (checkBoxDev3.Checked)
				{
					numericUpDownDev3.Value = Math.Min(_modules[3].StartChannel + 1, numberofChannels);
					UpdateRange(numericUpDownDev3, labelDev3);
				}
			}

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

		public K8055ControlModule[] Modules
		{
			get { return _modules; }
		}

		public bool UsingModule1
		{
			get { return checkBoxDev0.Checked; }
		}

		public bool UsingModule2
		{
			get { return checkBoxDev1.Checked; }
		}

		public bool UsingModule3
		{
			get { return checkBoxDev2.Checked; }
		}

		public bool UsingModule4
		{
			get { return checkBoxDev3.Checked; }
		}

		public int Module1StartChannel
		{
			get { return (int)numericUpDownDev0.Value; }
		}

		public int Module2StartChannel
		{
			get { return (int)numericUpDownDev1.Value; }
		}

		public int Module3StartChannel
		{
			get { return (int)numericUpDownDev2.Value; }
		}

		public int Module4StartChannel
		{
			get { return (int)numericUpDownDev3.Value; }
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (_modules[0].Enabled = checkBoxDev0.Checked)
			{
				_modules[0].StartChannel = (int)numericUpDownDev0.Value;
			}
			if (_modules[1].Enabled = checkBoxDev1.Checked)
			{
				_modules[1].StartChannel = (int)numericUpDownDev1.Value;
			}
			if (_modules[2].Enabled = checkBoxDev2.Checked)
			{
				_modules[2].StartChannel = (int)numericUpDownDev2.Value;
			}
			if (_modules[3].Enabled = checkBoxDev3.Checked)
			{
				_modules[3].StartChannel = (int)numericUpDownDev3.Value;
			}

			_data.Modules = _modules;

		}
	}
}
