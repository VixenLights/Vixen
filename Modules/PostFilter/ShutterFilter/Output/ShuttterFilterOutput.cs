using Vixen.Commands;
using Vixen.Data.Evaluator;
using Vixen.Data.Flow;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;
using VixenModules.OutputFilter.TaggedFilter.Outputs;

namespace VixenModules.OutputFilter.ShutterFilter.Output
{
	/// <summary>
	/// Shutter filter output.  This output contains special conversion logic for fixtures.
	/// This output extract the intensity from color intents and creates open shutter commands.
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
