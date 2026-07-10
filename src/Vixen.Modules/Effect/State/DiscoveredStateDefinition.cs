using VixenModules.Property.State;

namespace VixenModules.Effect.State
{
	internal sealed record DiscoveredStateDefinition(
		Guid OwnerElementId,
		string OwnerElementName,
		Guid StatePropertyId,
		Guid StateDefinitionId,
		string StateDefinitionName,
		StateDefinitionData Definition,
		string DisplayLabel);
}
