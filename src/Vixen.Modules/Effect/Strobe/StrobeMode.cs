using System.ComponentModel;

namespace VixenModules.Effect.Strobe
{
	public enum StrobeSource
	{
		[Description("Time Interval")]
		TimeInterval,
		[Description("Mark Collection - Label Value")]
		MarkCollection,
		[Description("Mark Collection - Label Duration")]
		MarkCollectionLabelDuration
	}
	public enum StrobeMode
	{
		[Description("Simple")]
		Simple,
		[Description("Advanced")]
		Advanced
	}
}
