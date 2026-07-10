namespace VixenModules.Effect.State
{
	internal static class StateRenderSegmentCoalescer
	{
		internal static IReadOnlyList<StateRenderSegment> Coalesce(IEnumerable<StateRenderSegment> segments)
		{
			var coalescedSegments = new List<StateRenderSegment>();

			foreach (var segment in segments)
			{
				if (coalescedSegments.Count > 0 &&
					coalescedSegments[^1].CanCoalesceWith(segment))
				{
					coalescedSegments[^1] = coalescedSegments[^1].Extend(segment.Duration);
					continue;
				}

				coalescedSegments.Add(segment);
			}

			return coalescedSegments;
		}
	}
}
