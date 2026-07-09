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

		internal static IReadOnlyList<StateRenderInterval> CreateCustomIntervals(
			StateDefinitionData? definition,
			IReadOnlyList<CustomStateItemData> customStateItems,
			PlaybackMode playbackMode,
			int iterations,
			TimeSpan effectDuration)
		{
			return CreateCustomIntervals(
				definition,
				customStateItems,
				playbackMode,
				iterations,
				cycleIndividually: true,
				effectDuration);
		}

		internal static IReadOnlyList<StateRenderInterval> CreateCustomIntervals(
			StateDefinitionData? definition,
			IReadOnlyList<CustomStateItemData> customStateItems,
			PlaybackMode playbackMode,
			int iterations,
			bool cycleIndividually,
			TimeSpan effectDuration)
		{
			if (definition == null || effectDuration <= TimeSpan.Zero || customStateItems.Count == 0)
			{
				return [];
			}

			var itemsById = (definition.Items ?? [])
				.GroupBy(item => item.Id)
				.ToDictionary(group => group.Key, group => group.First());

			return playbackMode == PlaybackMode.Iterate
				? CreateIteratedCustomIntervals(itemsById, customStateItems, iterations, cycleIndividually, effectDuration)
				: CreateDefaultCustomIntervals(itemsById, customStateItems, effectDuration);
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

		private static IReadOnlyList<StateRenderInterval> CreateDefaultCustomIntervals(
			IReadOnlyDictionary<Guid, StateItemData> itemsById,
			IEnumerable<CustomStateItemData> customStateItems,
			TimeSpan effectDuration)
		{
			var intervals = new List<StateRenderInterval>();
			var renderedItemIds = new HashSet<Guid>();

			foreach (var customStateItem in customStateItems)
			{
				if (customStateItem.StateItemId == Guid.Empty ||
					!renderedItemIds.Add(customStateItem.StateItemId) ||
					!itemsById.TryGetValue(customStateItem.StateItemId, out var item))
				{
					continue;
				}

				intervals.Add(new StateRenderInterval(item, TimeSpan.Zero, effectDuration, customStateItem.Color));
			}

			return intervals;
		}

		private static IReadOnlyList<StateRenderInterval> CreateIteratedCustomIntervals(
			IReadOnlyDictionary<Guid, StateItemData> itemsById,
			IReadOnlyList<CustomStateItemData> customStateItems,
			int iterations,
			bool cycleIndividually,
			TimeSpan effectDuration)
		{
			if (!cycleIndividually)
			{
				return CreateGroupedCustomIntervals(itemsById, customStateItems, iterations, effectDuration);
			}

			var normalizedIterations = StateData.NormalizeIterations(iterations);
			var intervalCount = customStateItems.Count * normalizedIterations;
			var intervals = new List<StateRenderInterval>();
			var intervalStart = TimeSpan.Zero;

			for (var index = 0; index < intervalCount; index++)
			{
				var duration = GetIntervalDuration(effectDuration, intervalCount, index, intervalStart);
				var customStateItem = customStateItems[index % customStateItems.Count];
				if (customStateItem.StateItemId != Guid.Empty &&
					itemsById.TryGetValue(customStateItem.StateItemId, out var item))
				{
					intervals.Add(new StateRenderInterval(item, intervalStart, duration, customStateItem.Color));
				}

				intervalStart += duration;
			}

			return intervals;
		}

		private static IReadOnlyList<StateRenderInterval> CreateGroupedCustomIntervals(
			IReadOnlyDictionary<Guid, StateItemData> itemsById,
			IReadOnlyList<CustomStateItemData> customStateItems,
			int iterations,
			TimeSpan effectDuration)
		{
			var groups = CreateCustomStateItemGroups(itemsById, customStateItems);
			var normalizedIterations = StateData.NormalizeIterations(iterations);
			var intervalCount = groups.Count * normalizedIterations;
			var intervals = new List<StateRenderInterval>();
			var intervalStart = TimeSpan.Zero;

			for (var index = 0; index < intervalCount; index++)
			{
				var duration = GetIntervalDuration(effectDuration, intervalCount, index, intervalStart);
				var group = groups[index % groups.Count];
				foreach (var customStateItem in group)
				{
					if (customStateItem.StateItemId != Guid.Empty &&
						itemsById.TryGetValue(customStateItem.StateItemId, out var item))
					{
						intervals.Add(new StateRenderInterval(item, intervalStart, duration, customStateItem.Color));
					}
				}

				intervalStart += duration;
			}

			return intervals;
		}

		private static IReadOnlyList<IReadOnlyList<CustomStateItemData>> CreateCustomStateItemGroups(
			IReadOnlyDictionary<Guid, StateItemData> itemsById,
			IReadOnlyList<CustomStateItemData> customStateItems)
		{
			var groups = new List<IReadOnlyList<CustomStateItemData>>();
			var currentGroup = new List<CustomStateItemData>();
			string? currentKey = null;

			foreach (var customStateItem in customStateItems)
			{
				var key = GetCustomStateItemGroupKey(itemsById, customStateItem);
				if (currentGroup.Count > 0 && !key.Equals(currentKey, StringComparison.Ordinal))
				{
					groups.Add(currentGroup);
					currentGroup = [];
				}

				currentGroup.Add(customStateItem);
				currentKey = key;
			}

			if (currentGroup.Count > 0)
			{
				groups.Add(currentGroup);
			}

			return groups;
		}

		private static string GetCustomStateItemGroupKey(
			IReadOnlyDictionary<Guid, StateItemData> itemsById,
			CustomStateItemData customStateItem)
		{
			if (customStateItem.StateItemId == Guid.Empty)
			{
				return "None:";
			}

			return itemsById.TryGetValue(customStateItem.StateItemId, out var item)
				? $"Name:{item.Name}"
				: $"Missing:{customStateItem.StateItemId:N}";
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
