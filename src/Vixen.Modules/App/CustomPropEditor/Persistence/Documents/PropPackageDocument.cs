namespace VixenModules.App.CustomPropEditor.Persistence.Documents;

internal sealed record PropPackageDocument
{
	public const string CurrentFormat = "vixen.custom-prop";
	public const int CurrentSchemaVersion = 1;

	public string Format { get; init; } = CurrentFormat;
	public int SchemaVersion { get; init; } = CurrentSchemaVersion;
	public PropDocument Prop { get; init; } = new();
	public Guid RootElementId { get; init; }
	public List<ElementDocument> Elements { get; init; } = [];
	public ImageDocument Image { get; init; } = new();
}
