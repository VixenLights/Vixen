using Vixen.Commands;
using Vixen.Data.Evaluator;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using VixenModules.OutputFilter.TaggedFilter.Filters;

namespace VixenModules.OutputFilter.PrismFilter.Filter
{
    /// <summary>
    /// Filter that converts prism intents into open prism commands.
    /// </summary>
    public class PrismFilter : TaggedFilter.Filters.TaggedFilter
	{
		#region Public Properties

		/// <summary>
		/// Flag which determines if prism intents are converted into open prism intents.
		/// </summary>
		public bool ConvertPrismIntentsIntoOpenPrismIntents { get; set; }

		/// <summary>
		/// Open Prism index command value.
		/// </summary>
		public byte OpenPrismIndexValue { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Handles a tagged command intent.
		/// </summary>
		/// <param name="intent">Intent to handle</param>
		public override void Handle(IIntentState<CommandValue> intent)
		{
			// Test to see if the intent is a tagged intent
			Named8BitCommand<FixtureIndexType> taggedCommand = intent.GetValue().Command as Named8BitCommand<FixtureIndexType>;

			// If the intent is a tagged command and
			// the tag matches then...
			if (taggedCommand != null &&
				taggedCommand.IndexType == FixtureIndexType.Prism)
			{
				// Save off the intent which indicates to the caller that the output associated with this filter handles this type of intent.
				IntentValue = HandleIntent();
			}
			else
			{
				// Otherwise let the base class handle the intent
				base.Handle(intent);
			}			
		}

		/// <summary>
		/// Handles a tagged range intent.
		/// </summary>
		/// <param name="intent">Intent to handle</param>
		public override void Handle(IIntentState<RangeValue<FunctionIdentity>> intent)
		{
			// If the intent is a Prism range then...
			if (intent.GetValue().TagType == FunctionIdentity.Prism)
			{
				// Save off the intent which indicates to the caller that the output associated with this filter handles this type of intent.
				IntentValue = HandleIntent();
			}
			else
			{
				// Otherwise let the base class handle the intent
				base.Handle(intent);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Handles prism intents by creating a prism open command.
		/// </summary>				
		private IIntentState HandleIntent()
		{
			// By default the intent is NOT handled
			IIntentState intent = null;

			// If automatically opening the prism then...
			if (ConvertPrismIntentsIntoOpenPrismIntents)
			{
				// Retrieve the command for the specified value
				ICommand command = CommandLookup8BitEvaluator.CommandLookup[OpenPrismIndexValue];

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