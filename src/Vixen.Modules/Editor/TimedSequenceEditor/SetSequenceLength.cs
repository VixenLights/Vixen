using Common.Controls.Theme;

namespace TimedSequenceEditor
{
	public partial class SetSequenceLength : Form
	{
		public TimeSpan SequenceLength
		{
			get => timeControl.TimeSpan;
		}

		public SetSequenceLength(TimeSpan sequenceLength)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);

			timeControl.TimeSpan = sequenceLength;
		}
	}
}
