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
		}

		private void ReleaseNotes_Load(object sender, EventArgs e)
		{
			textBoxReleaseNotes.Text = File.ReadAllText(Paths.BinaryRootPath + "//Release Notes.txt");
		}
	}
}
