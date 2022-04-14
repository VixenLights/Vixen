using Vixen.Commands;
using Vixen.Data.Evaluator;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using VixenModules.OutputFilter.TaggedFilter.Filters;

namespace VixenModules.OutputFilter.ShutterFilter.Filter
{
    /// <summary>
    /// Filter that converts color intents into open shutter commands.
    /// </summary>
    public class ShutterFilter : TaggedFilter.Filters.TaggedFilter
	{
		#region Public Properties

		/// <summary>
		/// Flag which determines if color intents are converted into shutter intents.
		/// </summary>
		public bool ConvertColorIntoShutterIntents { get; set; }

		/// <summary>
		/// Open Shutter index command value.
		/// </summary>
		public byte OpenShutterIndexValue { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			// Handle the discrete color value
			IntentValue = HandleIntent(obj);
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Handle(IIntentState<RGBValue> obj)
		{
			// Handle the RGB color value
			IntentValue = HandleIntent(obj);
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Handle(IIntentState<LightingValue> obj)
		{
			// Handle the lighting color value
			IntentValue = HandleIntent(obj);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Handles color intents by creating a shutter open command.
		/// </summary>
		/// <param name="intent">Intent to handle</param>
		/// <returns>Handled intent or null if shutter conversion is configured OFF</returns>
		private IIntentState HandleIntent(IIntentState intent)
		{
			// By default the intent is NOT handled
			intent = null;

			// If automatically opening the shutter then...
			if (ConvertColorIntoShutterIntents)
			{
				// Retrieve the command for the specified value
				ICommand command = CommandLookup8BitEvaluator.CommandLookup[OpenShutterIndexValue];

				// Create a command value, wrapping the 8 bit command
				CommandValue commandValue = new CommandValue(command);

				// Create a new intent wrapping the command
				IIntentState<CommandValue> commandIntentState =
					new StaticIntentState<CommandValue>(commandValue);

				// Add the command intent to the States associated with the output 
				intent = commandIntentState;
			}

			return intent;
		}

		#endregion
	}
}