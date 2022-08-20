using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Data.Evaluator;
using Vixen.Data.Flow;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using VixenModules.OutputFilter.TaggedFilter.Outputs;

namespace VixenModules.OutputFilter.ShutterFilter.Output
{
	/// <summary>
	/// Shutter filter output.  This output contains special conversion logic for fixtures.
	/// This output detects color intents or color wheel intents and creates open shutter commands.
	/// </summary>
	public class ShutterFilterOutput : TaggedFilterOutputBase<Filter.ShutterFilter>
	{
		#region Fields

		/// <summary>
		/// Flag determines if the output converts color intents into open shutter intents.
		/// </summary>
		private bool _convertColorIntoShutterIntents;

		/// <summary>
		/// Open shutter index command value.
		/// </summary>
		private byte _openShutterIndexValue;

		/// <summary>
		/// Close shutter index command value.
		/// </summary>
		private byte _closeShutterIndexValue;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tag">Tag (name) of shutter function</param>
		/// <param name="convertColorIntoShutterIntents">Flag indicating whether to convert color into shutter intents</param>
		/// <param name="openShutterIndexValue">Index value for shutter open command</param>
		/// <param name="closeShutterIndexValue">Index value for shutter close command</param>
		public ShutterFilterOutput(
			string tag, 
			bool convertColorIntoShutterIntents,
			byte openShutterIndexValue,
			byte closeShutterIndexValue) : base(tag)			
		{
			// Store off whether to convert color into shutter intents
			_convertColorIntoShutterIntents = convertColorIntoShutterIntents;

			// Store off the open shutter index value
			_openShutterIndexValue = openShutterIndexValue;

			// Store off the close shutter index value
			_closeShutterIndexValue = closeShutterIndexValue;			
		}

		#endregion
		
		#region Public Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void ConfigureFilter()
        {
			// Configure whether the filter should convert color intents into shutter commands
			((Filter.ShutterFilter)Filter).ConvertColorIntoShutterIntents = _convertColorIntoShutterIntents;

			// Configure the shutter open index DMX value
			((Filter.ShutterFilter)Filter).OpenShutterIndexValue = _openShutterIndexValue;
		}
						
		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		/// <remarks>This override add the close shutter command when there is no derived intent data.</remarks>
		protected override IntentsDataFlowData IntentData
        {
            get
            {
				// Get the base class intent data
				IntentsDataFlowData intentData = base.IntentData;

				// If there is more than one shutter intent then...
				if (intentData.Value.Count > 1)
				{
					// Declare a collection of intents to remove
					List<IIntentState> intentsToRemove = new List<IIntentState>();

					// Create a flag to keep track if a strobe intent was found
					bool foundStrobe = false;

					// Loop over the intents
					foreach (IIntentState intentState in intentData.Value)
					{
						// If the intent is a strobe intent then...
						if (((CommandValue)intentState.GetValue()).Command is Named8BitCommand<FixtureIndexType> &&
							(((CommandValue)intentState.GetValue()).Command as Named8BitCommand<FixtureIndexType>).IndexType == FixtureIndexType.Strobe)
						{
							// Remember that a strobe intent was found
							foundStrobe = true;
						}
						else
						{
							// Keep track of the other shutter intents to remove
							intentsToRemove.Add(intentState);
						}
					}

					// If a strobe shutter intent was found then...
					if (foundStrobe)
					{
						// Loop over the shutter intents to remove
						foreach (IIntentState intentState in intentsToRemove)
						{
							// Remove the open shutter intents
							intentData.Value.Remove(intentState);
						}
					}										
				}
				
				// If the intent data collection is empty then...
				if (intentData.Value.Count == 0)
                {
					// Add a close shutter command
					CreateCloseShutterCommand(intentData);
                }

				// Return the intent data
				return intentData;
			}
			set
            {
				// Store off the intent data
				base.IntentData = value;
            }
        }

		#endregion

		#region Private Methdos
		
		/// <summary>
		/// Creates a shutter close command.
		/// </summary>		
		/// <param name="intentData">Intent data collection to populate</param>
		private void CreateCloseShutterCommand(IntentsDataFlowData intentData)
		{
			// If automatically opening the shutter then...
			if (_convertColorIntoShutterIntents)
			{
				// Retrieve the command for the specified value
				ICommand command = CommandLookup8BitEvaluator.CommandLookup[_closeShutterIndexValue];

				// Create a command value, wrapping the 8 bit command
				CommandValue commandValue = new CommandValue(command);

				// Create a new intent wrapping the command
				IIntentState<CommandValue> commandIntentState =
					new StaticIntentState<CommandValue>(commandValue);

				// Add the command intent to the States associated with the output 
				intentData.Value.Add(commandIntentState);
			}
		}

		#endregion
	}
}
