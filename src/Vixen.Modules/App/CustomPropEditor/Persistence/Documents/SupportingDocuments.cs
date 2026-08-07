namespace VixenModules.App.CustomPropEditor.Persistence.Documents;

internal sealed record LightDocument
{
	public Guid Id { get; init; }
	public double X { get; init; }
	public double Y { get; init; }
	public double Z { get; init; }
	public double Size { get; init; }
}

internal sealed record ImageDocument
{
	public string EntryName { get; init; } = "background.jpg";
	public string MediaType { get; init; } = "image/jpeg";
}

internal sealed record FaceDefinitionDocument
{
	public string Component { get; init; } = "None";
	public string DefaultColor { get; init; } = "#FFFFFFFF";
}

internal sealed record StateDefinitionDocument
{
	public Guid Id { get; init; }
	public string Name { get; init; } = string.Empty;
	public string Description { get; init; } = string.Empty;
	public List<StateItemDocument> Items { get; init; } = [];
}

internal sealed record StateItemDocument
{
	public Guid Id { get; init; }
	public string Name { get; init; } = string.Empty;
	public string Color { get; init; } = "#FFFFFFFF";
	public List<Guid> ElementIds { get; init; } = [];
}
