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
			IntentValue = HandleIntent();
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Handle(IIntentState<RGBValue> obj)
		{
			// Handle the RGB color value
			IntentValue = HandleIntent();
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Handle(IIntentState<LightingValue> obj)
		{
			// Handle the lighting color value
			IntentValue = HandleIntent();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Handles color intents by creating a shutter open command.
		/// </summary>				
		private IIntentState HandleIntent()
		{
			// By default the intent is NOT handled
			IIntentState intent = null;

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

				// Return the open shutter command
				intent = commandIntentState;
			}

			return intent;
		}

		#endregion
	}
}