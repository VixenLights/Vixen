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
		protected virtual IntentsDataFlowData IntentData 
		{
			get;
			set;            
		}
		
		#endregion
				
		#region IDataFlowOutput

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		IDataFlowData IDataFlowOutput.Data => Data;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public IntentsDataFlowData Data => IntentData;

		#endregion

		#region Protected Methods

		/// <summary>
		/// Returns true when the specified intent is applicable to the output.
		/// </summary>
		/// <param name="intentState">Intent state to analyze</param>
		/// <param name="handledIntent">The intent may change type when processed.  This second return value is the handled intent 
		/// that should be exposed on the output</param>
		/// <returns>True when the specified intent is applicable to the output</returns>
		protected bool IsIntentApplicable(IIntentState intentState, out IIntentState handledIntent)
		{
			// Dispatch the intent to determine if it is supported
			// If the return value is non null the intent is applicable
			handledIntent = Filter.Filter(intentState);

			// If the intent is not null then it is applicable
			return handledIntent != null;
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
		/// Configures the filter.  Allows the derived outputs the opportunity to configure the filter.
		/// </summary>
		public virtual void ConfigureFilter()
		{
			// By default no configuring is required			
		}

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
				IIntentState handledIntent;

				// If the intent is applicable to the output then...
				if (IsIntentApplicable(intentState, out handledIntent))
				{
					// Process the intent (add to the output)
					ProcessInputDataInternal(handledIntent);
				}
			}					
		}

		#endregion
	}
}
