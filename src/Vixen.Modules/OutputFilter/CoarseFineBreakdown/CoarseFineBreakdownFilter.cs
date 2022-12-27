using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace VixenModules.OutputFilter.CoarseFineBreakdown
{
	/// <summary>
	/// Filter determines which intents to process.
	/// </summary>
	internal class CoarseFineBreakdownFilter : IntentStateDispatch
	{
		#region Fields
		
		/// <summary>
		/// Temporary state used by the handler methods to indicate if an intent should be processed.
		/// </summary>
		private IIntentState _intentValue = null;

		#endregion

		#region Public Methods

		/// <summary>
		/// Determines if the specified intent state is supported.
		/// </summary>
		/// <param name="intentValue">Intent state to filter</param>
		/// <returns>The intent if it is supported otherwise null</returns>
		public IIntentState Filter(IIntentState intentValue)
		{
			// Initialize the return value to null
			_intentValue = null;

			// Dispatch the intent
			intentValue.Dispatch(this);

			// Return whether the intent is supported
			return _intentValue;
		}

		/// <summary>
		/// Method called as a result of dispatching.
		/// </summary>
		/// <param name="intent">Intent to handle</param>
		public override void Handle(IIntentState<RangeValue<FunctionIdentity>> intent)
		{			
			// Indicate that position intents are supported
			_intentValue = intent;			
		}

		#endregion
	}
}