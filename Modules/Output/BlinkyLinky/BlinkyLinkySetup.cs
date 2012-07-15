using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace VixenModules.Output.BlinkyLinky
{
	public partial class BlinkyLinkySetup : Form
	{
		public BlinkyLinkySetup(BlinkyLinkyData data)
		{
			InitializeComponent();
			Address = data.Address;
			Port = data.Port;
			Stream = data.Stream;
		}

		public IPAddress Address
		{
			get
			{
				IPAddress result;
				if (IPAddress.TryParse(textBoxIPAddress.Text, out result)) {
					return result;
				}
				return null;
			}
			set
			{
				if (value == null)
					textBoxIPAddress.Text = "";
				else
					textBoxIPAddress.Text = value.ToString();
			}
		}

		public int Port
		{
			get { return (int)numericUpDownPort.Value; }
			set
			{
				if (value > numericUpDownPort.Maximum)
					value = (int)numericUpDownPort.Maximum;
				if (value < numericUpDownPort.Minimum)
					value = (int)numericUpDownPort.Minimum;

				numericUpDownPort.Value = value;
			}
		}

		public int Stream
		{
			get { return (int)numericUpDownStream.Value; }
			set
			{
				if (value > numericUpDownStream.Maximum)
					value = (int)numericUpDownStream.Maximum;
				if (value < numericUpDownStream.Minimum)
					value = (int)numericUpDownStream.Minimum;

				numericUpDownStream.Value = value;
			}
		}



	}
}
