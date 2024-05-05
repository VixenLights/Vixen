using System.ComponentModel;

namespace VixenModules.Effect.Whirlpool
{
	/// <summary>
	/// Defines the color modes of the whirlpool effect.
	/// </summary>
	public enum WhirlpoolColorMode
	{
		[Description("Single Color")]
		GradientOverTime,
		
		[Description("Side Colors")]
		LegColors,

		[Description("Color Rings")]
		RectangularRings,

		[Description("Color Bands")]
		Bands
	}
}