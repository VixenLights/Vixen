using System;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class SelectedMarkMoveEventArgs
	{
		public SelectedMarkMoveEventArgs(bool waveFormMark, TimeSpan selectedMark)
		{
			WaveFormMark = waveFormMark;
			SelectedMark = selectedMark;
		}

		public bool WaveFormMark { get; set; }

		public TimeSpan SelectedMark { get; set; }
	}
}