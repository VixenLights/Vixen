using Vixen.Data.Value;
using Vixen.Sys;

namespace VixenModules.OutputFilter.TaggedFilter.Filters
{
	/// <summary>
	/// Base class for a tagged filter that processes color intents.
	/// </summary>
	public abstract class TaggedColorFilterBase : Filters.TaggedFilter
	{
		#region Public Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			// Handle the discrete color value
			IntentValue = obj;
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Handle(IIntentState<RGBValue> obj)
		{
			// Handle the RGB color value
			IntentValue = obj;
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Handle(IIntentState<LightingValue> obj)
		{
			// Handle the lighting color value
			IntentValue = obj;
		}

		#endregion
	}
}
