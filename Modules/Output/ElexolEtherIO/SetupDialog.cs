using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace VixenModules.Output.ElexolEtherIO
{
	public partial class SetupDialog : Form
	{
		private int m_MinIntensity = 1;
		private IPAddress m_IPAddress = null;
		private ElexolEtherIOData _data;


		public SetupDialog(ElexolEtherIOData data)
		{
			InitializeComponent();

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
				MessageBox.Show("Invalid IP Address.");
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
					MessageBox.Show("No Reply.");
				}
				else
				{
					IPEndPoint rcvdEP = new IPEndPoint(IPAddress.Any, 0);
					byte[] rcvdPckt = client.Receive(ref rcvdEP);
					if (arrayEqual(rcvdPckt, sendPckt))
					{
						MessageBox.Show("Connection Successful!");
					}
					else
					{
						MessageBox.Show("Device replied, but with incorrect data:\n" + printBytes(sendPckt));
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
	}
}
