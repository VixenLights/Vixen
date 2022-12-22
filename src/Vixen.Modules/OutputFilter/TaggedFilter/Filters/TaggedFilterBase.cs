using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace VixenModules.OutputFilter.TaggedFilter.Filters
{
	/// <summary>
	/// Base class for a tagged filter.
	/// This filter will only process intents with a matching tag.
	/// </summary>
	public abstract class TaggedFilterBase : IntentStateDispatch, ITaggedFilter
	{						
		#region Protected Properties

		/// <summary>
		/// This property is used to determine if the output associated with this filter
		/// supports the intent being filtered.
		/// </summary>
		protected IIntentState IntentValue { get; set; }
		
		#endregion

		#region IFixtureFilter 

		/// <summary>
		/// Main entry point that filters intents applicable to the associated output.
		/// </summary>
		/// <param name="intent">Intent to filter</param>
		/// <returns>The intent if supported otherwise null</returns>
		public IIntentState Filter(IIntentState intent)
		{
			// Initialize the member to null to indicate the intent is NOT suported
			IntentValue = null;

			// Dispatch the intent, to determine if it is supported
			intent.Dispatch(this);

			// Return whether the intent is applicable to the associated output
			return IntentValue;
		}

		/// <summary>
		/// Tag associated with the filter and associated output.
		/// </summary>
		public string Tag { get; set; }

		#endregion
	}
}
