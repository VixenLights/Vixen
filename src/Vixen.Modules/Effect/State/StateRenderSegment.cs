using System.Drawing;
using Vixen.Sys;

namespace VixenModules.Effect.State
{
	internal sealed record StateRenderSegment(
		Guid StateItemId,
		IElementNode LeafNode,
		Guid ElementId,
		Color ResolvedColor,
		TimeSpan Start,
		TimeSpan Duration)
	{
		internal bool CanCoalesceWith(StateRenderSegment next)
		{
			return StateItemId == next.StateItemId &&
				ElementId == next.ElementId &&
				ResolvedColor == next.ResolvedColor &&
				Start + Duration == next.Start;
		}

		internal StateRenderSegment Extend(TimeSpan duration)
		{
			return this with { Duration = Duration + duration };
		}
	}
}
