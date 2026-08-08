namespace VixenModules.App.CustomPropEditor.Persistence.Documents;

internal sealed record VendorMetadataDocument
{
	public string Name { get; init; } = string.Empty;
	public string Website { get; init; } = string.Empty;
	public string Contact { get; init; } = string.Empty;
	public string Email { get; init; } = string.Empty;
	public string Phone { get; init; } = string.Empty;
}

internal sealed record PhysicalMetadataDocument
{
	public string Height { get; init; } = string.Empty;
	public string Width { get; init; } = string.Empty;
	public string Depth { get; init; } = string.Empty;
	public string Material { get; init; } = string.Empty;
	public string NodeCount { get; init; } = string.Empty;
	public string BulbType { get; init; } = string.Empty;
	public string ColorMode { get; init; } = string.Empty;
}

internal sealed record InformationMetadataDocument
{
	public string Notes { get; init; } = string.Empty;
}
