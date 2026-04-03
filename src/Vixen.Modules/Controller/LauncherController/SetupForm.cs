using Common.Controls;
using Common.Controls.Theme;

namespace VixenModules.Output.LauncherController
{
	public partial class SetupForm : BaseForm
	{
		public Data LauncherData { get; }

		public SetupForm(Data data)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			LauncherData = data;
			chkHideLaunchedWindows.Checked= data.HideLaunchedWindows;
		}

		private void chkHideLaunchedWindows_CheckedChanged(object sender, EventArgs e)
		{
			LauncherData.HideLaunchedWindows = chkHideLaunchedWindows.Checked;
		}
	}
}
