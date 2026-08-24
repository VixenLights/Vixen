namespace VixenModules.OutputFilter.CoarseFineBreakdown.Setup.ViewModels
{
	/// <summary>
	/// Represents the accepted coarse/fine breakdown setup values.
	/// </summary>
	internal readonly record struct CoarseFineBreakdownSetupResult(
		bool EnableDefaultValueMapping,
		byte RestingCoarseValue,
		byte RestingFineValue);
}
