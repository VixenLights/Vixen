namespace VixenModules.Editor.LayerEditor.ImportExport
{
	/// <summary>
	/// Describes a layer record from a `.v3l` file that could not be imported.
	/// </summary>
	public sealed class LayerImportSkippedRecord
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LayerImportSkippedRecord"/> class.
		/// </summary>
		/// <param name="name">The exported layer name.</param>
		/// <param name="reason">A human-readable explanation of why the layer was skipped.</param>
		public LayerImportSkippedRecord(string name, string reason)
		{
			Name = name;
			Reason = reason;
		}

		/// <summary>
		/// Gets the exported layer name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets a human-readable explanation of why the layer was skipped.
		/// </summary>
		public string Reason { get; }
	}
}
