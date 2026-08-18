using Common.Controls;
using Common.Controls.Theme;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class MarkCollectionImportDialog : BaseForm
	{
		public MarkCollectionImportDialog()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
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

		/// <summary>
		/// Gets a value that indicates whether Pangolin Beyond import is selected.
		/// </summary>
		/// <value><see langword="true" /> if Pangolin Beyond import is selected; otherwise, <see langword="false" />.</value>
		public bool IsPangolinBeyondSelection => radioPangolinBeyond.Checked;

        public bool IsTimingTrackBrowserSelection => radioTimingTrackBrowser.Checked;

        public bool IsPapagayoSelection => radioPapagayo.Checked;

		public bool IsXTimingSelection => radioXTiming.Checked;
	}
}
