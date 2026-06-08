using VixenModules.Property.State;

namespace VixenModules.Effect.State
{
	internal static class StateRenderPlanner
	{
		internal static IReadOnlyList<StateRenderInterval> CreateStateItemIntervals(
			StateDefinitionData? definition,
			Guid selectedStateItemId,
			PlaybackMode playbackMode,
			TimeSpan effectDuration)
		{
			if (definition == null || effectDuration <= TimeSpan.Zero)
			{
				return [];
			}

			var items = definition.Items ?? [];
			if (items.Count == 0)
			{
				return [];
			}

			if (selectedStateItemId != Guid.Empty)
			{
				return CreateSelectedItemIntervals(items, selectedStateItemId, effectDuration);
			}

			return playbackMode switch
			{
				PlaybackMode.Iterate => CreateIteratedIntervals(items, effectDuration),
				_ => CreateDefaultIntervals(items, effectDuration)
			};
		}

		private static IReadOnlyList<StateRenderInterval> CreateSelectedItemIntervals(
			IReadOnlyList<StateItemData> items,
			Guid selectedStateItemId,
			TimeSpan effectDuration)
		{
			var selectedItem = items.FirstOrDefault(item => item.Id == selectedStateItemId);
			if (selectedItem == null)
			{
				return [];
			}

			return items
				.Where(item => item.Name.Equals(selectedItem.Name, StringComparison.Ordinal))
				.Select(item => new StateRenderInterval(item, TimeSpan.Zero, effectDuration))
				.ToList();
		}

		private static IReadOnlyList<StateRenderInterval> CreateDefaultIntervals(
			IReadOnlyList<StateItemData> items,
			TimeSpan effectDuration)
		{
			return items
				.Select(item => new StateRenderInterval(item, TimeSpan.Zero, effectDuration))
				.ToList();
		}

		private static IReadOnlyList<StateRenderInterval> CreateIteratedIntervals(
			IReadOnlyList<StateItemData> items,
			TimeSpan effectDuration)
		{
			var orderedNames = GetUniqueStateItemNames(items);
			if (orderedNames.Count == 0)
			{
				return [];
			}

			var intervals = new List<StateRenderInterval>();
			var intervalStart = TimeSpan.Zero;

			for (var index = 0; index < orderedNames.Count; index++)
			{
				var duration = GetIntervalDuration(effectDuration, orderedNames.Count, index, intervalStart);
				foreach (var item in items.Where(item => item.Name.Equals(orderedNames[index], StringComparison.Ordinal)))
				{
					intervals.Add(new StateRenderInterval(item, intervalStart, duration));
				}

				intervalStart += duration;
			}

			return intervals;
		}

		private static TimeSpan GetIntervalDuration(
			TimeSpan effectDuration,
			int intervalCount,
			int intervalIndex,
			TimeSpan intervalStart)
		{
			return intervalIndex == intervalCount - 1
				? effectDuration - intervalStart
				: TimeSpan.FromTicks(effectDuration.Ticks / intervalCount);
		}

		private static IReadOnlyList<string> GetUniqueStateItemNames(IEnumerable<StateItemData> items)
		{
			var names = new List<string>();
			var knownNames = new HashSet<string>(StringComparer.Ordinal);

			foreach (var item in items)
			{
				if (knownNames.Add(item.Name))
				{
					names.Add(item.Name);
				}
			}

			return names;
		}
	}
}
