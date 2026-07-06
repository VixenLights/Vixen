using Vixen.Sys.LayerMixing;
using VixenModules.Editor.LayerEditor.ImportExport;

namespace VixenModules.Editor.LayerEditor.Services
{
	/// <summary>
	/// Exports and imports non-default sequence layers as `.v3l` files.
	/// </summary>
	public interface ILayerImportExportService
	{
		/// <summary>
		/// Writes every non-default layer, in top-to-bottom order, to a `.v3l` file.
		/// </summary>
		/// <param name="layers">The sequence layer collection to export.</param>
		/// <param name="filePath">The destination file path.</param>
		/// <param name="cancellationToken">A token to cancel the write operation.</param>
		/// <returns>A task that completes when the file has been written.</returns>
		Task ExportAsync(SequenceLayers layers, string filePath, CancellationToken cancellationToken = default);

		/// <summary>
		/// Reads and validates a `.v3l` file without mutating any sequence layers.
		/// </summary>
		/// <param name="filePath">The source file path.</param>
		/// <param name="cancellationToken">A token to cancel the read operation.</param>
		/// <returns>The resulting <see cref="LayerImportPlan"/>.</returns>
		Task<LayerImportPlan> ReadImportPlanAsync(string filePath, CancellationToken cancellationToken = default);

		/// <summary>
		/// Adds the importable layers from a plan above the existing layers, preserving their exported relative order.
		/// </summary>
		/// <param name="layers">The sequence layer collection to update.</param>
		/// <param name="plan">A previously validated import plan.</param>
		/// <returns>The resulting <see cref="LayerImportResult"/>.</returns>
		LayerImportResult Import(SequenceLayers layers, LayerImportPlan plan);
	}
}
