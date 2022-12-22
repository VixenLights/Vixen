namespace VixenModules.OutputFilter.TaggedFilter.Outputs
{
	/// <summary>
	/// Maintains data for a tagged filter.
	/// </summary>
	public interface ITaggedFilterData
	{
		/// <summary>
		/// Tag associated with the filter.
		/// </summary>
		string Tag { get; set; }
	}
}
