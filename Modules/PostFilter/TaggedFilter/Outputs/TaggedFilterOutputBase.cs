using System.Collections.Generic;
using System.Linq;
using Vixen.Data.Flow;
using Vixen.Sys;
using VixenModules.OutputFilter.TaggedFilter.Filters;

namespace VixenModules.OutputFilter.TaggedFilter.Outputs
{
	/// <summary>
	/// Base class for a tagged filter output.
	/// </summary>
	/// <typeparam name="TFilter">Filter associated with the output</typeparam>
	public abstract class TaggedFilterOutputBase<TFilter> : IDataFlowOutput<IntentsDataFlowData>, ITaggedFilterOutput
		where TFilter : ITaggedFilter, new()
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tag">Tag associated with the output</param>
		public TaggedFilterOutputBase(string tag)
		{
			// Create the filter associated with the output
			Filter = new TFilter();

			// Give the filter the function tag
			Filter.Tag = tag;

			// Use the tag for the output name
			Name = tag;

			// Create the data structure for holding intent output state
			IntentData = new IntentsDataFlowData(Enumerable.Empty<IIntentState>().ToList())
			{
				Value = new List<IIntentState>()
			};
		}

		#endregion

		#region Protected Properties

		/// <summary>
		/// Filter associated with the output.
		/// </summary>
		protected ITaggedFilter Filter { get; set; }

		/// <summary>
		/// Output intent data associated with the output.
		/// </summary>
		protected  IntentsDataFlowData IntentData { get; private set; }
		
		#endregion
				
		#region IDataFlowOutput

		public string Name { get; private set; }

		IDataFlowData IDataFlowOutput.Data => Data;

		public IntentsDataFlowData Data => IntentData;

		#endregion

		#region Protected Methods

		/// <summary>
		/// Returns true when the specified intent is applicable to the output.
		/// </summary>
		/// <param name="intentState">Intent state to analyze</param>
		/// <returns>True when the specified intent is applicable to the output</returns>
		protected bool IsIntentApplicable(IIntentState intentState)
		{
			// Send the intent to the filter and see if it comes back non null
			return Filter.Filter(intentState) != null;
		}

		/// <summary>
		/// Processes the intent state.
		/// </summary>
		/// <param name="intent">Intent to process</param>
		protected virtual void ProcessInputDataInternal(IIntentState intent)
		{
			// By default just move the intent to the output
			IntentData.Value.Add(intent);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Processes input intent data.
		/// </summary>
		/// <param name="data">Intent data to process</param>
		public void ProcessInputData(IntentsDataFlowData data)
		{
			// Clear the outputs
			IntentData.Value.Clear();

			// Loop over the intents in the collection
			foreach (IIntentState intentState in data.Value)
			{
				// If the intent is applicable to the output then...
				if (IsIntentApplicable(intentState))
				{
					// Process the intent (add to the output)
					ProcessInputDataInternal(intentState);
				}
			}
		}

		#endregion
	}
}
