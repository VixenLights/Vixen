using System;
using System.Drawing;

namespace Common.Controls.TimelineControl
{
	public class LabeledMark: IComparable<LabeledMark>
	{
		public string Text { get; set; }

		public TimeSpan StartTime { get; set; }

		public TimeSpan Duration { get; set; }

		public TimeSpan EndTime => StartTime + Duration;

		internal int StackIndex { get; set; }
		internal int StackCount { get; set; }

		public int DisplayTop { get; set; }
		public int RowTopOffset { get; set; }
		public int DisplayHeight { get; set; }
		public Rectangle DisplayRect { get; set; }

		#region Implementation of IComparable<in LabeledMark>

		/// <inheritdoc />
		public int CompareTo(LabeledMark other)
		{
			int rv = StartTime.CompareTo(other.StartTime);
			if (rv != 0)
			{
				return rv;
			}
		
			return EndTime.CompareTo(other.EndTime);
		}

		#endregion
	}
}
