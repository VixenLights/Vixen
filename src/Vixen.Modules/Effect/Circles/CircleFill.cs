using System.ComponentModel;

namespace VixenModules.Effect.Circles
{
	public enum CircleFill
	{
		[Description("Fade")]
		Fade,
		[Description("Empty")]
		Empty,
		[Description("Gradient over Time")]
		GradientOverTime,
		[Description("Gradient over Element")]
		GradientOverElement
	}
}