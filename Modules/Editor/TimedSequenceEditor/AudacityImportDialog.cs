using System;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class AudacityImportDialog : BaseForm
	{
		public AudacityImportDialog()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
		}

		public bool IsVampBeatSelection
		{
			get
			{
				return radioBeats.Checked;
			}
		}

		public bool IsVampBarSelection
		{
			get
			{
				return radioBars.Checked;
			}
		}

		public bool IsAudacityBeatSelection
		{
			get
			{
				return radioAudacityBeats.Checked;
			}
		}

		public bool IsVixen3BeatSelection
		{
			get
			{
				return radioVixen3Beats.Checked;
			}
		}

		public bool IsPapagayoSelection => radioPapagayo.Checked;

		public bool IsXTimingSelection => radioXTiming.Checked;

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
