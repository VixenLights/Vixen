using System.ComponentModel;

namespace VixenModules.Effect.LineDance
{
	/// <summary>
	/// Defines the fan modes.
	/// </summary>
	public enum FanModes
	{
		/// <summary>
		/// Synchronized fan all nodes start and end panning at the same time.
		/// </summary>
		[Description("Synchronized")]
		Synchronized,

		/// <summary>
		/// Only two nodes are panning at the same time.  Movement expands from the center of the fan to the edges.
		/// </summary>
		[Description("Staggered")]
		Stagger,

		/// <summary>
		/// All nodes start panning at the beginning of the effect and finish at different times based on how they have to rotate.
		/// </summary>
		[Description("Concurrent")]
		Concurrent,
	}
}
