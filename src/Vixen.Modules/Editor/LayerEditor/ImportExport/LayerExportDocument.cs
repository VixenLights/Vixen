using System.Text.Json.Serialization;

namespace VixenModules.Editor.LayerEditor.ImportExport
{
	/// <summary>
	/// Represents the top-level `.v3l` layer export document.
	/// </summary>
	public sealed class LayerExportDocument
	{
		/// <summary>
		/// The `.v3l` document format literal.
		/// </summary>
		public const string CurrentFormat = "Vixen3Layers";

		/// <summary>
		/// The current, and only currently supported, `.v3l` document version.
		/// </summary>
		public const int CurrentVersion = 1;

		/// <summary>
		/// Gets or sets the document format literal. Must equal <see cref="CurrentFormat"/> to be importable.
		/// </summary>
		[JsonPropertyName("format")]
		public string Format { get; set; } = CurrentFormat;

		/// <summary>
		/// Gets or sets the document version. Must equal <see cref="CurrentVersion"/> to be importable.
		/// </summary>
		[JsonPropertyName("version")]
		public int Version { get; set; } = CurrentVersion;

		/// <summary>
		/// Gets or sets the UTC timestamp when the document was exported.
		/// </summary>
		[JsonPropertyName("exportedUtc")]
		public DateTime ExportedUtc { get; set; }

		/// <summary>
		/// Gets or sets the exported standard layers, in top-to-bottom order.
		/// </summary>
		[JsonPropertyName("layers")]
		public List<LayerExportRecord> Layers { get; set; } = new();
	}
}
