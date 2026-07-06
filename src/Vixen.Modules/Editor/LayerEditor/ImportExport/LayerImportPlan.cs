namespace VixenModules.Editor.LayerEditor.ImportExport
{
	/// <summary>
	/// Represents the validated result of reading a `.v3l` file, before any layers are mutated.
	/// </summary>
	public sealed class LayerImportPlan
	{
		private LayerImportPlan(bool isValid, string failureReason, IReadOnlyList<LayerImportEntry> importableLayers,
			IReadOnlyList<LayerImportSkippedRecord> skippedLayers)
		{
			IsValid = isValid;
			FailureReason = failureReason;
			ImportableLayers = importableLayers;
			SkippedLayers = skippedLayers;
		}

		/// <summary>
		/// Gets a value that indicates whether the file was well-formed and supported. A valid plan may still
		/// contain skipped layers.
		/// </summary>
		public bool IsValid { get; }

		/// <summary>
		/// Gets a human-readable explanation of why the file could not be read, or <see langword="null" /> when
		/// <see cref="IsValid"/> is <see langword="true" />.
		/// </summary>
		public string FailureReason { get; }

		/// <summary>
		/// Gets the layers that can be imported, in exported top-to-bottom order.
		/// </summary>
		public IReadOnlyList<LayerImportEntry> ImportableLayers { get; }

		/// <summary>
		/// Gets the layer records that could not be imported.
		/// </summary>
		public IReadOnlyList<LayerImportSkippedRecord> SkippedLayers { get; }

		/// <summary>
		/// Gets a value that indicates whether any layer record was skipped.
		/// </summary>
		public bool HasSkippedLayers => SkippedLayers.Count > 0;

		/// <summary>
		/// Creates a plan describing a file-level failure. Nothing in this plan should be imported.
		/// </summary>
		/// <param name="failureReason">A human-readable explanation of the failure.</param>
		/// <returns>The failed <see cref="LayerImportPlan"/>.</returns>
		public static LayerImportPlan Failed(string failureReason)
		{
			return new LayerImportPlan(false, failureReason, Array.Empty<LayerImportEntry>(), Array.Empty<LayerImportSkippedRecord>());
		}

		/// <summary>
		/// Creates a plan describing a successfully parsed and validated file.
		/// </summary>
		/// <param name="importableLayers">The layers that can be imported, in exported top-to-bottom order.</param>
		/// <param name="skippedLayers">The layer records that could not be imported.</param>
		/// <returns>The successful <see cref="LayerImportPlan"/>.</returns>
		public static LayerImportPlan Succeeded(IReadOnlyList<LayerImportEntry> importableLayers, IReadOnlyList<LayerImportSkippedRecord> skippedLayers)
		{
			return new LayerImportPlan(true, null, importableLayers, skippedLayers);
		}
	}
}
