using System.Text.Json;
using System.Text.Json.Serialization;

namespace VixenModules.Editor.LayerEditor.ImportExport
{
	/// <summary>
	/// Represents a single exported standard layer within a <see cref="LayerExportDocument"/>.
	/// </summary>
	public sealed class LayerExportRecord
	{
		/// <summary>
		/// Gets or sets the layer name at the time it was exported.
		/// </summary>
		[JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the zero-based, top-to-bottom position of the layer among the exported layers.
		/// </summary>
		[JsonPropertyName("order")]
		public int Order { get; set; }

		/// <summary>
		/// Gets or sets the layer mixing filter module type identifier.
		/// </summary>
		[JsonPropertyName("filterTypeId")]
		public Guid FilterTypeId { get; set; }

		/// <summary>
		/// Gets or sets the display name of the layer mixing filter type, recorded for human readability only.
		/// </summary>
		[JsonPropertyName("filterName")]
		public string FilterName { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the fully qualified filter module data type name, recorded for human readability only.
		/// </summary>
		[JsonPropertyName("filterDataType")]
		public string FilterDataType { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the layer mixing filter module data, serialized with <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>.
		/// </summary>
		[JsonPropertyName("filterData")]
		public JsonElement FilterData { get; set; }
	}
}
