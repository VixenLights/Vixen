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
			chkBoxClearCacheOnExit.Checked = Vixen.Sys.VixenSystem.VideoEffect_ClearCacheOnExit;
			cmbBoxCacheFileType.Text = Vixen.Sys.VixenSystem.VideoEffect_CacheFileType;

			ToolTip toolTipVideoEffectCache = new ToolTip();
			toolTipVideoEffectCache.AutoPopDelay = 5000;
			toolTipVideoEffectCache.InitialDelay = 1000;
			toolTipVideoEffectCache.ReshowDelay = 500;
			toolTipVideoEffectCache.ShowAlways = true;

			toolTipVideoEffectCache.SetToolTip(this.chkBoxClearCacheOnExit, "Frees disk space, but adds some\ntime to rebuild at sequence open");
			toolTipVideoEffectCache.SetToolTip(this.cmbBoxCacheFileType, "bmp - Fastest/Bigger\npng - Fast/Smaller");
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Vixen.Sys.VixenSystem.DefaultUpdateInterval = (int)ctlUpdateInteral.Value;
			AudioOutputManager.Instance().Latency = (int)wasapiLatency.Value;
			Vixen.Sys.VixenSystem.VideoEffect_ClearCacheOnExit = chkBoxClearCacheOnExit.Checked;
			Vixen.Sys.VixenSystem.VideoEffect_CacheFileType = cmbBoxCacheFileType.Text;
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
