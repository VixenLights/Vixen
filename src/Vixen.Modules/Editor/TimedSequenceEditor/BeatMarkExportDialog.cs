using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class BeatMarkExportDialog : BaseForm
	{
		public BeatMarkExportDialog()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			radioVixen3Format.Checked = true;
		}

		public bool IsVixen3Selection
		{
			get
			{
				return radioVixen3Format.Checked;
			}
		}

			public bool IsAudacitySelection
		{
			get 
			{
				return radioAudacityFormat.Checked;
			}
		}

			private void BeatMarkExportDialog_Load(object sender, EventArgs e)
			{
				radioVixen3Format.Checked = true;
			}
	}
}
