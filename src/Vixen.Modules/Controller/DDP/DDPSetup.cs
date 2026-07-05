using Common.Controls;
using Common.Controls.Theme;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using Utilities;

namespace VixenModules.Output.DDP
{
	public partial class DDPSetup : BaseForm
	{
		private IPAddress _resolvedAddress;
		private string _resolvedHostName;

		/// <summary>
		/// Initializes a new instance of the <see cref="DDPSetup"/> class, pre-filling it from
		/// <paramref name="data"/>. If <see cref="DDPData.HostName"/> is set, "Host Name" mode is
		/// pre-selected and pre-filled with it; otherwise "IP Address" mode is pre-selected and
		/// pre-filled with <see cref="DDPData.Address"/>.
		/// </summary>
		/// <param name="data">The controller's current configuration.</param>
		public DDPSetup(DDPData data)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);

			if (!string.IsNullOrEmpty(data.HostName))
			{
				radioButtonHostName.Checked = true;
				textBoxHostName.Text = data.HostName;
			}
			else
			{
				radioButtonIPAddress.Checked = true;
				if (IPAddress.TryParse(data.Address, out var result))
				{
					textBoxIPAddress.Text = result.ToString();
				}
				else
				{
					textBoxIPAddress.Text = IPAddress.Loopback.ToString();
				}
			}
			UpdateModeControls();
		}

		/// <summary>
		/// Gets the IP address the user configured, either entered directly or resolved from the
		/// host name entered in "Host Name" mode. Set only after the user clicks OK.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IPAddress Address => _resolvedAddress ?? IPAddress.Loopback;

		/// <summary>
		/// Gets the DNS host name the user entered, or <see langword="null"/> if the user configured
		/// the controller with a literal IP address instead. Set only after the user clicks OK.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string HostName => _resolvedHostName;

		private void UpdateModeControls()
		{
			if (radioButtonHostName.Checked)
			{
				textBoxHostName.Enabled = true;
				textBoxHostName.BringToFront();
				textBoxIPAddress.Enabled = false;
				label1.Text = "Host Name:";
			}
			else
			{
				textBoxIPAddress.Enabled = true;
				textBoxIPAddress.BringToFront();
				textBoxHostName.Enabled = false;
				label1.Text = "IP Address:";
			}
		}

		private void Radio_CheckedChanged(object sender, EventArgs e)
		{
			UpdateModeControls();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (radioButtonHostName.Checked)
			{
				string hostName = textBoxHostName.Text.Trim();
				bool resolvedOk;
				IPAddress resolved;
				Cursor.Current = Cursors.WaitCursor;
				try
				{
					resolvedOk = HostNameResolver.TryResolveToIPv4(hostName, out resolved);
				}
				finally
				{
					Cursor.Current = Cursors.Default;
				}

				if (resolvedOk)
				{
					_resolvedAddress = resolved;
					_resolvedHostName = hostName;
				}
				else
				{
					MessageBoxForm mbf = new MessageBoxForm(
						$"Could not resolve host name '{hostName}' to an IP address. Check the spelling and your network connection, or switch to IP Address mode.",
						"DDP Setup Error", MessageBoxButtons.OK, SystemIcons.Error);
					mbf.ShowDialog(this);
					textBoxHostName.Focus();
					return;
				}
			}
			else
			{
				if (!IPAddress.TryParse(textBoxIPAddress.Text, out var ip))
				{
					ip = IPAddress.Loopback;
				}
				_resolvedAddress = ip;
				_resolvedHostName = null;
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}
