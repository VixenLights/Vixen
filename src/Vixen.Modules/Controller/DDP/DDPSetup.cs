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
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);			
			Address = IPAddress.Parse(data.Address);
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