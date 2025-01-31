using System.Windows.Forms;
using System.Net;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Output.DDP
{
	public partial class DDPSetup : BaseForm
	{
		public DDPSetup(DDPData data)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			if (IPAddress.TryParse(data.Address, out var result))
			{
				Address = result;
			}
			else
			{
				Address = IPAddress.Loopback;
			}
		}

		public IPAddress Address
		{
			get
			{
				IPAddress result;
				if (IPAddress.TryParse(textBoxIPAddress.Text, out result)) {
					return result;
				}
				return IPAddress.Loopback;
			}
			set
			{
				if (value == null)
					textBoxIPAddress.Text = IPAddress.Loopback.ToString();
				else
					textBoxIPAddress.Text = value.ToString();
			}
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}