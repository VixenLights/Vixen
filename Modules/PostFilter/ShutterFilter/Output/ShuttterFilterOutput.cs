using Vixen.Commands;
using Vixen.Data.Evaluator;
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
		/// Open Shutter Index command value.
		/// </summary>
		private byte _openShutterIndexValue;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tag">Tag (name) of shutter function</param>
		/// <param name="convertColorIntoShutterIntents">Flag indicating whether to convert color into shutter intents</param>
		/// <param name="openShutterIndexValue">Index value for shutter open command</param>
		public ShutterFilterOutput(
			string tag, 
			bool convertColorIntoShutterIntents,
			byte openShutterIndexValue) : base(tag)			
		{
			// Store off whether to convert color into shutter intents
			_convertColorIntoShutterIntents = convertColorIntoShutterIntents;

			// Store off the open shutter index value
			_openShutterIndexValue = openShutterIndexValue;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Process the input intent.
		/// </summary>
		/// <param name="intent">Intent to process</param>
		protected override void ProcessInputDataInternal(IIntentState intent)
		{
			// If the intent is a discrete color then...
			if (intent is IIntentState<DiscreteValue> ||
			    intent is IIntentState<RGBValue> ||
			    intent is IIntentState<LightingValue>)
			{
				// Create an open shutter index command
				CreateOpenShutterCommand(intent.Layer);
			}
			else
			{
				// Otherwise just add the intent to the output states
				IntentData.Value.Add(intent);
			}
		}

		/// <summary>
		/// Converts the specified color into a shutter open command
		/// </summary>
		/// <param name="layer">Layer associated with the color intent</param>
		private void CreateOpenShutterCommand(ILayer layer)
		{
			// If automatically opening the shutter then...
			if (_convertColorIntoShutterIntents)
			{
				// Retrieve the command for the specified value
				ICommand command = CommandLookup8BitEvaluator.CommandLookup[_openShutterIndexValue];

				// Create a command value, wrapping the 8 bit command
				CommandValue commandValue = new CommandValue(command);

				// Create a new intent wrapping the command
				IIntentState<CommandValue> commandIntentState =
					new StaticIntentState<CommandValue>(commandValue, layer);

				// Add the command intent to the States associated with the output 
				IntentData.Value.Add(commandIntentState);
			}
		}

		#endregion
	}
}
