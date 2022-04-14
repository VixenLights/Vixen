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
		public abstract void Handle(IIntentState<DiscreteValue> obj);

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public abstract void Handle(IIntentState<RGBValue> obj);

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public abstract void Handle(IIntentState<LightingValue> obj);
		
		#endregion		
	}
}
