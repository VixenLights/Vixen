#nullable enable annotations

using Common.Controls.Timeline;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	internal sealed class MarkSnapPointRegistration(SnapDetails startSnapPoint, SnapDetails? endSnapPoint)
	{
		internal SnapDetails StartSnapPoint { get; } = startSnapPoint;
		internal SnapDetails? EndSnapPoint { get; } = endSnapPoint;
	}
}
