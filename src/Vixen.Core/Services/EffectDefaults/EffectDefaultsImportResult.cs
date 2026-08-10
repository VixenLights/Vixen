namespace Vixen.Services.EffectDefaults
{
	/// <summary>
	/// Selects how <see cref="EffectDefaultsService.Import"/> merges entries from an imported file into the
	/// current store. Currently only one merge strategy is supported: entries in the imported file always
	/// overwrite any existing saved default for the same effect type, exactly like <see cref="EffectDefaultsService.SaveDefault"/>
	/// would. This is the "merge on import" behavior documented in the approved architecture review.
	/// </summary>
	public enum ImportMode
	{
		/// <summary>
		/// Every entry in the imported file is upserted into the current store, overwriting an existing entry for
		/// the same effect type if one exists. Entries already in the current store that are not present in the
		/// imported file are left untouched.
		/// </summary>
		Overwrite
	}

	/// <summary>
	/// Reports what happened when <see cref="EffectDefaultsService.Import"/> merged an imported file into the
	/// current store.
	/// </summary>
	public sealed class EffectDefaultsImportResult
	{
		/// <summary>
		/// Gets the number of entries from the imported file that did not already exist in the current store and
		/// were added.
		/// </summary>
		public int Imported { get; internal set; }

		/// <summary>
		/// Gets the number of entries from the imported file that replaced an existing saved default for the same
		/// effect type.
		/// </summary>
		public int Overwritten { get; internal set; }
	}
}
