using VixenModules.App.CustomPropEditor.Import.XLights;

namespace VixenModules.App.CustomPropEditor.Import.XLights.Circle
{
	internal static class CircleXModelGroupGenerator
	{
		internal static bool HasMatchingImportedCirclesSubModel(
			XModelImportModel importModel,
			CircleXModelConfiguration configuration)
		{
			var generatedCircleOrders = configuration.Rings
				.Select(ring => ring.NodeOrders)
				.ToList();

			return importModel.SubModels.Any(subModel =>
				"Circles".Equals(subModel.Name, StringComparison.OrdinalIgnoreCase) &&
				subModel.Type == ModelType.Ranges &&
				HasMatchingRingGroups(subModel, generatedCircleOrders));
		}

		internal static XModelGeneratedGroup CreateGeneratedCircleGroups(CircleXModelConfiguration configuration)
		{
			return new XModelGeneratedGroup
			{
				Name = $"{configuration.Name} {{1}} - Circles",
				Children = configuration.Rings
					.OrderBy(ring => ring.CircleNumber)
					.Select(ring => new XModelGeneratedGroup
					{
						Name = $"{configuration.Name} {{1}} - Circle {ring.CircleNumber}",
						NodeOrders = ring.NodeOrders
					})
					.ToList()
			};
		}

		private static bool HasMatchingRingGroups(SubModel subModel, IReadOnlyList<List<int>> generatedCircleOrders)
		{
			if (subModel.Ranges.Count != generatedCircleOrders.Count)
			{
				return false;
			}

			var unmatchedGeneratedCircles = generatedCircleOrders
				.Select(ringOrders => ringOrders.ToList())
				.ToList();

			foreach (var rangeGroup in subModel.Ranges)
			{
				var importedNodeOrders = rangeGroup.Ranges
					.SelectMany(EnumerateRange)
					.Where(order => order > 0)
					.ToList();
				var matchingCircleIndex = unmatchedGeneratedCircles.FindIndex(generatedNodeOrders =>
					generatedNodeOrders.SequenceEqual(importedNodeOrders));
				if (matchingCircleIndex < 0)
				{
					return false;
				}

				unmatchedGeneratedCircles.RemoveAt(matchingCircleIndex);
			}

			return !unmatchedGeneratedCircles.Any();
		}

		private static IEnumerable<int> EnumerateRange(Ranges.Range range)
		{
			var increment = range.Start > range.End ? -1 : 1;
			for (var order = range.Start; ; order += increment)
			{
				if (increment > 0 && order > range.End)
				{
					break;
				}

				if (increment < 0 && order < range.End)
				{
					break;
				}

				yield return order;
			}
		}
	}
}
