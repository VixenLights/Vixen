using System;
using System.Windows.Forms;
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
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
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
