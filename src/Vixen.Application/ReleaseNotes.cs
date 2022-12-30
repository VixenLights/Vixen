using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Sys;

namespace VixenApplication
{
	public partial class ReleaseNotes : BaseForm
	{
		public ReleaseNotes()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
		}

		private void ReleaseNotes_Load(object sender, EventArgs e)
		{
			textBoxReleaseNotes.Text = File.ReadAllText(Paths.BinaryRootPath + "//Release Notes.txt");
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
