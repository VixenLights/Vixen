namespace VixenModules.OutputFilter.CoarseFineBreakdown
{
	/// <summary>
	/// Represents the immutable configuration used by a coarse/fine output.
	/// </summary>
	internal readonly record struct CoarseFineBreakdownOutputConfiguration(
		bool EnableDefaultValueMapping,
		byte RestingCoarseValue,
		byte RestingFineValue)
	{
		/// <summary>
		/// Gets the configured resting output value.
		/// </summary>
		/// <value>The high and low resting bytes combined into a 16-bit value.</value>
		public ushort RestingValue => (ushort)((RestingCoarseValue << 8) | RestingFineValue);
	}
}
