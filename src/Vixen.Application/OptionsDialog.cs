using Common.AudioPlayer;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenApplication
{
	public partial class OptionsDialog : BaseForm
	{
		public OptionsDialog()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
		}

		private void OptionsDialog_Load(object sender, EventArgs e)
		{
			ctlUpdateInteral.Value = Vixen.Sys.VixenSystem.DefaultUpdateInterval;
			wasapiLatency.Value = AudioOutputManager.Instance().Latency;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Vixen.Sys.VixenSystem.DefaultUpdateInterval = (int)ctlUpdateInteral.Value;
			AudioOutputManager.Instance().Latency = (int) wasapiLatency.Value;
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
