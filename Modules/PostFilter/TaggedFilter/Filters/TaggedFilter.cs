using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace VixenModules.OutputFilter.TaggedFilter.Filters
{
	/// <summary>
	/// Filter for a tagged intents.
	/// This filter will only process intents with a matching tag.
	/// </summary>
	public class TaggedFilter : TaggedFilterBase
	{
		#region Public Methods

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
				taggedCommand.Tag == Tag)
			{
				// Save off the intent which indicates to the caller that the output associated with this filter handles this type of intent.
				IntentValue = intent;
			}
		}

		/// <summary>
		/// Handles a tagged range intent.
		/// </summary>
		/// <param name="intent">Intent to handle</param>
		public override void Handle(IIntentState<RangeValue<FunctionIdentity>> intent)
		{
			// If the intent's tag matches then...
			if (intent.GetValue().Tag == Tag)
			{
				// Save off the intent which indicates to the caller that the output associated with this filter handles this type of intent.
				IntentValue = intent;
			}
		}

		#endregion
	}
}
