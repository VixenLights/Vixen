using System.ComponentModel;

namespace VixenModules.Effect.Twinkle
{
	public enum TwinkleColorHandling
	{
		[Description("Single Color")]
		StaticColor,
		[Description("Gradient Thru Effect")]
		GradientThroughWholeEffect,
		[Description("Gradient Per Pulse")]
		GradientForEachPulse,
		[Description("Gradient Across Items")]
		ColorAcrossItems
	}
}