using Vixen.Module.MixingFilter;

namespace VixenModules.Editor.LayerEditor.ImportExport
{
	/// <summary>
	/// Represents a single validated, importable layer produced by <see cref="Services.ILayerImportExportService.ReadImportPlanAsync"/>.
	/// </summary>
	public sealed class LayerImportEntry
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LayerImportEntry"/> class.
		/// </summary>
		/// <param name="name">The layer name to apply on import.</param>
		/// <param name="order">The zero-based, top-to-bottom position among the imported layers.</param>
		/// <param name="layerMixingFilter">The resolved and restored layer mixing filter instance.</param>
		public LayerImportEntry(string name, int order, ILayerMixingFilterInstance layerMixingFilter)
		{
			Name = name;
			Order = order;
			LayerMixingFilter = layerMixingFilter;
		}

		/// <summary>
		/// Gets the layer name to apply on import.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the zero-based, top-to-bottom position among the imported layers.
		/// </summary>
		public int Order { get; }

		/// <summary>
		/// Gets the resolved layer mixing filter instance, with module data already restored.
		/// </summary>
		public ILayerMixingFilterInstance LayerMixingFilter { get; }
	}
}
