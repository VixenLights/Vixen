using System.ComponentModel;

namespace VixenModules.Effect.State
{
	/// <summary>
	/// Specifies how the State effect determines active State item names.
	/// </summary>
	public enum StateRenderSource
	{
		/// <summary>
		/// Uses the selected State item or all State items for the effect duration.
		/// </summary>
		[Description("State Item")]
		StateItem,

		/// <summary>
		/// Uses Mark Collection text to activate State item names over time.
		/// </summary>
		[Description("Mark Collection")]
		MarkCollection,

		/// <summary>
		/// Uses the effect-owned custom State item row collection.
		/// </summary>
		[Description("Custom")]
		Custom
	}
}
