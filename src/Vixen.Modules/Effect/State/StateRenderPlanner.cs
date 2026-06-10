using Vixen.Marks;
using VixenModules.Property.State;

namespace VixenModules.Effect.State
{
	internal static class StateRenderPlanner
	{
		internal static IReadOnlyList<StateRenderInterval> CreateStateItemIntervals(
			StateDefinitionData? definition,
			Guid selectedStateItemId,
			PlaybackMode playbackMode,
			int iterations,
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
				PlaybackMode.Iterate => CreateIteratedIntervals(items, iterations, effectDuration),
				_ => CreateDefaultIntervals(items, effectDuration)
			};
		}

		internal static IReadOnlyList<StateRenderInterval> CreateMarkCollectionIntervals(
			StateDefinitionData? definition,
			IEnumerable<IMark> marks,
			PlaybackMode playbackMode,
			int iterations,
			TimeSpan effectStart,
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

			var itemGroups = items
				.GroupBy(item => item.Name)
				.ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);
			var effectEnd = effectStart + effectDuration;
			var intervals = new List<StateRenderInterval>();

			foreach (var mark in marks)
			{
				var intervalStart = Max(mark.StartTime, effectStart) - effectStart;
				var intervalEnd = Min(mark.EndTime, effectEnd) - effectStart;
				var intervalDuration = intervalEnd - intervalStart;
				if (intervalDuration <= TimeSpan.Zero)
				{
					continue;
				}

				var names = StateMarkParser.ParseStateItemNames(mark.Text);
				if (playbackMode == PlaybackMode.Iterate)
				{
					AddIteratedMarkIntervals(intervals, itemGroups, names, iterations, intervalStart, intervalDuration);
				}
				else
				{
					AddDefaultMarkIntervals(intervals, itemGroups, names, intervalStart, intervalDuration);
				}
			}

			return intervals;
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
			int iterations,
			TimeSpan effectDuration)
		{
			var orderedNames = GetUniqueStateItemNames(items);
			if (orderedNames.Count == 0)
			{
				return [];
			}

			var intervals = new List<StateRenderInterval>();
			var intervalStart = TimeSpan.Zero;
			var normalizedIterations = StateData.NormalizeIterations(iterations);
			var intervalCount = orderedNames.Count * normalizedIterations;

			for (var index = 0; index < intervalCount; index++)
			{
				var duration = GetIntervalDuration(effectDuration, intervalCount, index, intervalStart);
				var name = orderedNames[index % orderedNames.Count];
				foreach (var item in items.Where(item => item.Name.Equals(name, StringComparison.Ordinal)))
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

		private static void AddDefaultMarkIntervals(
			ICollection<StateRenderInterval> intervals,
			IReadOnlyDictionary<string, List<StateItemData>> itemGroups,
			IEnumerable<string> names,
			TimeSpan intervalStart,
			TimeSpan intervalDuration)
		{
			var recognizedNames = new HashSet<string>(StringComparer.Ordinal);

			foreach (var name in names)
			{
				if (string.IsNullOrEmpty(name) ||
					!recognizedNames.Add(name) ||
					!itemGroups.TryGetValue(name, out var items))
				{
					continue;
				}

				foreach (var item in items)
				{
					intervals.Add(new StateRenderInterval(item, intervalStart, intervalDuration));
				}
			}
		}

		private static void AddIteratedMarkIntervals(
			ICollection<StateRenderInterval> intervals,
			IReadOnlyDictionary<string, List<StateItemData>> itemGroups,
			IReadOnlyList<string> names,
			int iterations,
			TimeSpan intervalStart,
			TimeSpan intervalDuration)
		{
			if (names.Count == 0)
			{
				return;
			}

			var segmentStart = intervalStart;
			var normalizedIterations = StateData.NormalizeIterations(iterations);
			var segmentCount = names.Count * normalizedIterations;
			for (var index = 0; index < segmentCount; index++)
			{
				var segmentDuration = GetIntervalDuration(intervalDuration, segmentCount, index, segmentStart - intervalStart);
				var name = names[index % names.Count];
				if (!string.IsNullOrEmpty(name) && itemGroups.TryGetValue(name, out var items))
				{
					foreach (var item in items)
					{
						intervals.Add(new StateRenderInterval(item, segmentStart, segmentDuration));
					}
				}

				segmentStart += segmentDuration;
			}
		}

		private static TimeSpan Max(TimeSpan first, TimeSpan second) => first > second ? first : second;

		private static TimeSpan Min(TimeSpan first, TimeSpan second) => first < second ? first : second;
	}
}
