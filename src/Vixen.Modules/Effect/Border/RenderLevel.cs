using System.ComponentModel;

namespace VixenModules.Effect.Border
{
	/// <summary>
	/// Render levels for the Marquee border effect.
	/// </summary>
	public enum RenderLevel
	{
		/// <summary>
		/// Level zero renders using the typical matrix of pixels.
		/// </summary>
		[Description("Level 0")]
		Level0,

		/// <summary>
		/// Level one renders using a single strand of pixels.
		/// The single strand zig zag's back and forth on the matrix.
		/// </summary>
		[Description("Level 1")]
		Level1,

		/// <summary>
		/// Level two renders using a single strand of pixels.
		/// The single strand is created using a type writer pattern
		/// where it restarts on the left side of the matrix.
		/// </summary>
		[Description("Level 2")]
		Level2,
	}
}
