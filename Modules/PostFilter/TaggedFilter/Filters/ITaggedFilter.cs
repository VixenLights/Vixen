using Vixen.Sys;

namespace VixenModules.OutputFilter.TaggedFilter.Filters
{
	/// <summary>
	/// Filters intents applicable to an output.	
	/// </summary>
	public interface ITaggedFilter
	{
		/// <summary>
		/// Returns the intent if it is applicable to the output or otherwise NULL.
		/// </summary>
		/// <param name="intentValue">Intent to process</param>
		/// <returns>Applicable Intent or NULL</returns>
		IIntentState Filter(IIntentState intentValue);

		/// <summary>
		/// Tag associated with the filter.
		/// </summary>
		string Tag { get; set; }
	}
}
