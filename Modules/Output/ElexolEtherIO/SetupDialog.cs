using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Output.ElexolEtherIO
{
	public partial class SetupDialog : BaseForm
	{
		private int m_MinIntensity = 1;
		private IPAddress m_IPAddress = null;
		private ElexolEtherIOData _data;


		public SetupDialog(ElexolEtherIOData data)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);

			_data = data;

			if (_data.Address == null)
			{
				ipAddressTextBox.Text = "127.0.0.1";
			}
			else
			{
				ipAddressTextBox.Text = _data.Address.ToString();
			}

			if (_data.Port <= 0)
			{
				portTextBox.Text = "2424";
			}
			else
			{
				portTextBox.Text = _data.Port.ToString();
			}

			if (_data.MinimumIntensity > 1)
			{
				m_MinIntensity = _data.MinimumIntensity;
			}
			sliderMinIntensityTrackBar.Value = m_MinIntensity;
			minIntensityLabel.Text = m_MinIntensity.ToString();
		}

		private void sliderMinIntensityTrackBar_ValueChanged(object sender, EventArgs e)
		{
			m_MinIntensity = sliderMinIntensityTrackBar.Value;
			minIntensityLabel.Text = m_MinIntensity.ToString();

		}

		public IPAddress IPAddr
		{
			get;
			set;
		}
		public int MinIntensity
		{
			get;
			set;
		}

		public int DataPort
		{
			get;
			set;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (IPAddress.TryParse(ipAddressTextBox.Text, out m_IPAddress))
			{
				IPAddr = m_IPAddress;
				MinIntensity = m_MinIntensity;
				DataPort = int.Parse(portTextBox.Text);

				DialogResult = System.Windows.Forms.DialogResult.OK;
			}
			else
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Invalid IP Address.", "Error", false, false);
				messageBox.ShowDialog();
			}
		}

		private void testButton_Click(object sender, EventArgs e)
		{
			using (UdpClient client = new UdpClient())
			{
				IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddressTextBox.Text), int.Parse(portTextBox.Text));
				client.Connect(remoteEndPoint);

				// Use "`" command (0x60) to cause device to echo byte back.
				byte[] sendPckt = new byte[] { 0x60, 0x69 };
				DateTime begin = DateTime.Now;
				client.Send(sendPckt, sendPckt.Length);
				
				// Wait for data to be available or a timeout to ocurr.
				while ((client.Available == 0) && (((TimeSpan)(DateTime.Now - begin)).TotalSeconds < 1))
				{
					Thread.Sleep(10);
				}

				if (client.Available == 0)
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("No Reply.", "Error", false, false);
					messageBox.ShowDialog();
				}
				else
				{
					IPEndPoint rcvdEP = new IPEndPoint(IPAddress.Any, 0);
					byte[] rcvdPckt = client.Receive(ref rcvdEP);
					if (arrayEqual(rcvdPckt, sendPckt))
					{
						//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
						MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
						var messageBox = new MessageBoxForm("Connection Successful!", "Error", false, false);
						messageBox.ShowDialog();
					}
					else
					{
						//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
						MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
						var messageBox = new MessageBoxForm("Device replied, but with incorrect data:\n" + printBytes(sendPckt), "Error", false, false);
						messageBox.ShowDialog();
					}
				}

			}
		}
			private static string printBytes(byte[] array)
        {
            string retval = "";
            foreach(byte c in array)
                retval += String.Format("[0x{0:X2}]", c);
            return retval;
        }

        private static bool arrayEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    return false;
            return true;
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
