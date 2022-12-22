using System.ComponentModel;

namespace VixenModules.Effect.PinWheel
{
	public enum PinWheelColorType
	{
		[Description("Gradient over time")]
		Standard,
		[Description("Gradient Across Arms")]
		Gradient,
		[Description("Random")]
		Random,
		[Description("Rainbow")]
		Rainbow
	}
}