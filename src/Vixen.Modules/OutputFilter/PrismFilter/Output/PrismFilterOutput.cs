using Vixen.Commands;
using Vixen.Data.Evaluator;
using Vixen.Data.Flow;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using VixenModules.OutputFilter.TaggedFilter.Outputs;

namespace VixenModules.OutputFilter.PrismFilter.Output
{
	/// <summary>
	/// Prism filter output.  This output contains special conversion logic for fixtures.
	/// This output detects prism intents and creates open prism commands.
	/// </summary>
	public class PrismFilterOutput : TaggedFilterOutputBase<Filter.PrismFilter>
	{
		#region Fields

		/// <summary>
		/// Flag determines if the output converts prism intents into open prism intents.
		/// </summary>
		private bool _convertPrismIntentsIntoOpenPrismIntents;

		/// <summary>
		/// Open prism index command value.
		/// </summary>
		private byte _openPrismIndexValue;

		/// <summary>
		/// Close prism index command value.
		/// </summary>
		private byte _closePrismIndexValue;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tag">Tag (name) of open prism function</param>
		/// <param name="convertPrismIntentsIntoOpenPrismIntents">Flag indicating whether to convert prism into open prism intents</param>
		/// <param name="openPrismIndexValue">Index value for prism open command</param>
		/// <param name="closePrismIndexValue">Index value for prism close command</param>
		public PrismFilterOutput(
			string tag, 
			bool convertPrismIntentsIntoOpenPrismIntents,
			byte openPrismIndexValue,
			byte closePrismIndexValue) : base(tag)			
		{
			// Store off whether to convert prism intents into open prism intents
			_convertPrismIntentsIntoOpenPrismIntents = convertPrismIntentsIntoOpenPrismIntents;

			// Store off the open prism index value
			_openPrismIndexValue = openPrismIndexValue;

			// Store off the close prism index value
			_closePrismIndexValue = closePrismIndexValue;			
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void ConfigureFilter()
        {
			// Configure whether the filter should convert prism intents into open prism commands
			((Filter.PrismFilter)Filter).ConvertPrismIntentsIntoOpenPrismIntents = _convertPrismIntentsIntoOpenPrismIntents;

			// Configure the prism open index DMX value
			((Filter.PrismFilter)Filter).OpenPrismIndexValue = _openPrismIndexValue;
		}
						
		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		/// <remarks>This override add the close prism command when there is no derived intent data.</remarks>
		protected override IntentsDataFlowData IntentData
        {
            get
            {
				// Get the base class intent data
				IntentsDataFlowData intentData = base.IntentData;

				// If the intent data collection is empty then...
				if (intentData.Value.Count == 0)
                {
					// Add a close prism command
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
			// If automatically opening the prism then...
			if (_convertPrismIntentsIntoOpenPrismIntents)
			{
				// Retrieve the command for the specified value
				ICommand command = CommandLookup8BitEvaluator.CommandLookup[_closePrismIndexValue];

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
