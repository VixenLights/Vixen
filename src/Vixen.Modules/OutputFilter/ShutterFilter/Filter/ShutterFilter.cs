using Vixen.Commands;
using Vixen.Data.Evaluator;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;

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

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		/// <param name="intent">Intent to handle</param>
		public override void Handle(IIntentState<CommandValue> intent)
		{			
			// Test to see if the intent is a tagged intent
			Named8BitCommand<FixtureIndexType> taggedCommand = intent.GetValue().Command as Named8BitCommand<FixtureIndexType>;

			// If the intent is a tagged command and
			// the tag is a color wheel then...
			if (taggedCommand != null &&
				taggedCommand.IndexType == FixtureIndexType.ColorWheel)
			{
				// Save off the intent which indicates to the caller that the output associated with this filter handles this type of intent.
				// When spinning the color wheel the shutter is automatically opened
				IntentValue = HandleIntent();
			}
			else
			{
				// Otherwise call the base class
				base.Handle(intent);
			}
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