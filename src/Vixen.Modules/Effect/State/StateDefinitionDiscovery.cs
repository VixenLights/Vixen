using Vixen.Sys;
using VixenModules.Property.State;

namespace VixenModules.Effect.State
{
	internal static class StateDefinitionDiscovery
	{
		internal static IReadOnlyList<DiscoveredStateDefinition> Discover(IEnumerable<IElementNode> targetNodes)
		{
			if (targetNodes == null)
			{
				return [];
			}

			var discoveredDefinitions = new List<DiscoveredStateDefinition>();
			var visitedNodeIds = new HashSet<Guid>();

			foreach (var node in targetNodes.Where(node => node != null).SelectMany(Traverse))
			{
				if (!visitedNodeIds.Add(node.Id))
				{
					continue;
				}

				var stateModule = StateModule.GetStateModuleForElement(node);
				if (stateModule == null)
				{
					continue;
				}

				foreach (var definition in stateModule.StateDefinitions.Where(definition => definition != null))
				{
					discoveredDefinitions.Add(new DiscoveredStateDefinition(
						node.Id,
						node.Name,
						stateModule.Id,
						definition.Id,
						definition.Name,
						definition,
						definition.Name));
				}
			}

			return ApplyDisplayLabels(discoveredDefinitions);
		}

		private static IEnumerable<IElementNode> Traverse(IElementNode node)
		{
			yield return node;

			foreach (var child in node.Children.Where(child => child != null))
			{
				foreach (var descendant in Traverse(child))
				{
					yield return descendant;
				}
			}
		}

		private static IReadOnlyList<DiscoveredStateDefinition> ApplyDisplayLabels(
			IReadOnlyList<DiscoveredStateDefinition> definitions)
		{
			var duplicateNames = definitions
				.GroupBy(definition => definition.StateDefinitionName, StringComparer.Ordinal)
				.Where(group => group.Count() > 1)
				.Select(group => group.Key)
				.ToHashSet(StringComparer.Ordinal);

			var labeledDefinitions = definitions
				.Select(definition =>
				{
					var label = duplicateNames.Contains(definition.StateDefinitionName)
						? $"{definition.StateDefinitionName} ({definition.OwnerElementName})"
						: definition.StateDefinitionName;

					return definition with { DisplayLabel = label };
				})
				.ToList();

			var duplicateLabels = labeledDefinitions
				.GroupBy(definition => definition.DisplayLabel, StringComparer.Ordinal)
				.Where(group => group.Count() > 1)
				.Select(group => group.Key)
				.ToHashSet(StringComparer.Ordinal);

			return labeledDefinitions
				.Select(definition => duplicateLabels.Contains(definition.DisplayLabel)
					? definition with { DisplayLabel = $"{definition.DisplayLabel} [{ToShortId(definition.StateDefinitionId)}]" }
					: definition)
				.ToList();
		}

		internal static string ToShortId(Guid id) => id.ToString("N")[..8];
	}
}
