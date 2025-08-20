using System.Globalization;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class CreateEvenMarksForm : BaseForm
	{
		public CreateEvenMarksForm(TimeSpan? startTime, TimeSpan? endTime, TimeSpan sequenceLength)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			Start = startTime == null ? TimeSpan.Zero : (TimeSpan)startTime;
			txtStartTime.Maximum = sequenceLength;
			End = endTime == null ? TimeSpan.Zero : (TimeSpan)endTime;
			txtEndTime.Maximum = sequenceLength;
		}

		public TimeSpan End
		{
			get
			{
				return txtEndTime.TimeSpan;
			}
			set
			{
				txtEndTime.TimeSpan = value;
			}
		}
		public TimeSpan Start
		{
			set
			{
				txtStartTime.TimeSpan = value;
			}
			get
			{
				return txtStartTime.TimeSpan;
			}
		}

		public int Divisions => (int)updownDivide.Value;
	}
}
