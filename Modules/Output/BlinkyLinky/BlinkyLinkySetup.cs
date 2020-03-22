using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Output.BlinkyLinky
{
	public partial class BlinkyLinkySetup : BaseForm
	{
		public BlinkyLinkySetup(BlinkyLinkyData data)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
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
					textBoxIPAddress.Text = string.Empty;
				else
					textBoxIPAddress.Text = value.ToString();
			}
		}

		public int Port
		{
			get { return (int) numericUpDownPort.Value; }
			set
			{
				if (value > numericUpDownPort.Maximum)
					value = (int) numericUpDownPort.Maximum;
				if (value < numericUpDownPort.Minimum)
					value = (int) numericUpDownPort.Minimum;

				numericUpDownPort.Value = value;
			}
		}

		public int Stream
		{
			get { return (int) numericUpDownStream.Value; }
			set
			{
				if (value > numericUpDownStream.Maximum)
					value = (int) numericUpDownStream.Maximum;
				if (value < numericUpDownStream.Minimum)
					value = (int) numericUpDownStream.Minimum;

				numericUpDownStream.Value = value;
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

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}