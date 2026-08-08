namespace VixenModules.App.CustomPropEditor.Persistence.Documents;

internal sealed record ElementDocument
{
	public Guid Id { get; init; }
	public Guid StatePropertyId { get; init; }
	public string Name { get; init; } = string.Empty;
	public int Order { get; init; }
	public int LightSize { get; init; }
	public string ModelType { get; init; } = string.Empty;
	public FaceDefinitionDocument Face { get; init; } = new();
	public List<StateDefinitionDocument> StateDefinitions { get; init; } = [];
	public List<Guid> ChildIds { get; init; } = [];
	public List<LightDocument> Lights { get; init; } = [];
}
