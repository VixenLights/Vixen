namespace VixenModules.Effect.State
{
	internal static class StateRenderSegmentCoalescer
	{
		internal static IReadOnlyList<StateRenderSegment> Coalesce(IEnumerable<StateRenderSegment> segments)
		{
			var coalescedSegments = new List<StateRenderSegment>();
			var latestSegmentIndexes = new Dictionary<(Guid StateItemId, Guid ElementId, int Color), int>();

			foreach (var segment in segments)
			{
				var key = (segment.StateItemId, segment.ElementId, segment.ResolvedColor.ToArgb());
				if (latestSegmentIndexes.TryGetValue(key, out var previousIndex) &&
					coalescedSegments[previousIndex].CanCoalesceWith(segment))
				{
					coalescedSegments[previousIndex] = coalescedSegments[previousIndex].Extend(segment.Duration);
					continue;
				}

				latestSegmentIndexes[key] = coalescedSegments.Count;
				coalescedSegments.Add(segment);
			}

			return coalescedSegments;
		}
	}
}
