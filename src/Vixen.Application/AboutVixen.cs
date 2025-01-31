using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Sys;

namespace VixenApplication
{
	public partial class AboutVixen : BaseForm
	{
		public AboutVixen(string currentVersion, bool devBuild)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			pictureBoxIcon.Image = Resources.VixenImage;
			labelHeading.Font = new Font(labelHeading.Font.Name, 20F);
			string currentVersionType = devBuild ? " Build " : " Release ";
			labelHeading.Text += currentVersionType + currentVersion;
		}

		private void AboutVixen_Load(object sender, EventArgs e)
		{
			textBoxLicense.Text = File.ReadAllText(Paths.BinaryRootPath + "//License.txt");
		}
	}
}
