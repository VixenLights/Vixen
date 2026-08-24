namespace VixenModules.OutputFilter.CoarseFineBreakdown
{
	/// <summary>
	/// Represents the immutable configuration used by a coarse/fine output.
	/// </summary>
	internal readonly record struct CoarseFineBreakdownOutputConfiguration(
		bool EnableDefaultValueMapping,
		ushort DefaultInputValue,
		byte RestingCoarseValue,
		byte RestingFineValue)
	{
		/// <summary>
		/// Gets the value to split for a canonical input value.
		/// </summary>
		/// <param name="canonicalValue">The normalized 16-bit input value.</param>
		/// <returns>The resting value for an exact enabled default-value match; otherwise, the canonical input value.</returns>
		public ushort EffectiveValue(ushort canonicalValue) =>
			EnableDefaultValueMapping && canonicalValue == DefaultInputValue
				? (ushort)((RestingCoarseValue << 8) | RestingFineValue)
				: canonicalValue;
	}
}
