namespace VixenModules.Editor.LayerEditor.ImportExport
{
	/// <summary>
	/// Describes the outcome of applying a <see cref="LayerImportPlan"/> to a <see cref="Vixen.Sys.LayerMixing.SequenceLayers"/>.
	/// </summary>
	public sealed class LayerImportResult
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LayerImportResult"/> class.
		/// </summary>
		/// <param name="importedCount">The number of layers that were added.</param>
		/// <param name="skippedLayers">The layer records that were not imported.</param>
		public LayerImportResult(int importedCount, IReadOnlyList<LayerImportSkippedRecord> skippedLayers)
		{
			ImportedCount = importedCount;
			SkippedLayers = skippedLayers;
		}

		/// <summary>
		/// Gets the number of layers that were added to the sequence.
		/// </summary>
		public int ImportedCount { get; }

		/// <summary>
		/// Gets the layer records that were not imported.
		/// </summary>
		public IReadOnlyList<LayerImportSkippedRecord> SkippedLayers { get; }
	}
}
