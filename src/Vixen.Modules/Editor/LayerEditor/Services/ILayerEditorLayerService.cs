using Vixen.Module.MixingFilter;
using Vixen.Sys.LayerMixing;

namespace VixenModules.Editor.LayerEditor.Services
{
	/// <summary>
	/// Provides layer operations for the Layer Editor.
	/// </summary>
	public interface ILayerEditorLayerService
	{
		/// <summary>
		/// Adds a standard layer using a new filter instance for the specified filter type.
		/// </summary>
		/// <param name="layers">The sequence layer collection to update.</param>
		/// <param name="filterTypeId">The layer mixing filter type identifier.</param>
		/// <returns>The added layer.</returns>
		ILayer AddLayer(SequenceLayers layers, Guid filterTypeId);

		/// <summary>
		/// Adds a standard layer using an existing filter instance.
		/// </summary>
		/// <param name="layers">The sequence layer collection to update.</param>
		/// <param name="layerMixingFilter">The filter instance to attach to the new layer.</param>
		/// <returns>The added layer.</returns>
		ILayer AddLayer(SequenceLayers layers, ILayerMixingFilterInstance layerMixingFilter);

		/// <summary>
		/// Removes a standard layer.
		/// </summary>
		/// <param name="layers">The sequence layer collection to update.</param>
		/// <param name="layer">The layer to remove.</param>
		/// <returns><see langword="true" /> if the layer was removed; otherwise, <see langword="false" />.</returns>
		bool RemoveLayer(SequenceLayers layers, ILayer layer);

		/// <summary>
		/// Moves a standard layer to a new index.
		/// </summary>
		/// <param name="layers">The sequence layer collection to update.</param>
		/// <param name="layer">The layer to move.</param>
		/// <param name="newIndex">The destination index.</param>
		/// <returns><see langword="true" /> if the layer was moved; otherwise, <see langword="false" />.</returns>
		bool MoveLayer(SequenceLayers layers, ILayer layer, int newIndex);

		/// <summary>
		/// Runs setup for a layer mixing filter.
		/// </summary>
		/// <param name="layerMixingFilter">The filter to configure.</param>
		/// <returns><see langword="true" /> if setup completed successfully; otherwise, <see langword="false" />.</returns>
		bool ConfigureLayer(ILayerMixingFilterInstance layerMixingFilter);

		/// <summary>
		/// Renames a layer to the display name of its selected filter type.
		/// </summary>
		/// <param name="layers">The sequence layer collection containing the layer.</param>
		/// <param name="layer">The layer to rename.</param>
		/// <returns><see langword="true" /> if the layer was renamed; otherwise, <see langword="false" />.</returns>
		bool QuickRenameLayer(SequenceLayers layers, ILayer layer);

		/// <summary>
		/// Creates a layer name that does not collide with the existing layer names.
		/// </summary>
		/// <param name="layers">The existing layers.</param>
		/// <param name="desiredName">The requested layer name.</param>
		/// <returns>A unique layer name.</returns>
		string CreateUniqueLayerName(IEnumerable<ILayer> layers, string desiredName);

		/// <summary>
		/// Gets a value that indicates whether the sequence has non-default layers to export.
		/// </summary>
		/// <param name="layers">The sequence layer collection to inspect.</param>
		/// <returns><see langword="true" /> if at least one standard layer exists; otherwise, <see langword="false" />.</returns>
		bool HasExportableLayers(SequenceLayers layers);
	}
}
