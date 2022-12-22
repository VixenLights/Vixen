using System.ComponentModel;

namespace VixenModules.Effect.Balls
{
	public enum BallFill
	{
		[Description("Fade")]
		Fade,
		[Description("Empty")]
		Empty,
		[Description("Solid")]
		Solid,
		[Description("Gradient")]
		Gradient,
		[Description("Inverse")]
		Inverse
	}
}