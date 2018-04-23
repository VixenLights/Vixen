using Common.Controls.Timeline;

namespace Common.Controls.TimelineControl
{
	public sealed class MarksBar:TimelineControlBase
	{
		/// <inheritdoc />
		public MarksBar(TimeInfo timeinfo) : base(timeinfo)
		{
		}

		public List<MarkCollection> MarkCollection { get; set; }
	}
}
