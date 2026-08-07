namespace VixenModules.App.CustomPropEditor.Persistence.Documents;

internal sealed record PropDocument
{
	public Guid Id { get; init; }
	public string Type { get; init; } = string.Empty;
	public string CreatedBy { get; init; } = string.Empty;
	public DateTime CreationDate { get; init; }
	public DateTime ModifiedDate { get; init; }
	public double Opacity { get; init; }
	public double Width { get; init; }
	public double Height { get; init; }
	public VendorMetadataDocument Vendor { get; init; } = new();
	public PhysicalMetadataDocument Physical { get; init; } = new();
	public InformationMetadataDocument Information { get; init; } = new();
}
