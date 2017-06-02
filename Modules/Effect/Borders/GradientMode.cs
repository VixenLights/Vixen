using System.ComponentModel;

namespace VixenModules.Effect.Borders
{
	public enum GradientMode
	{
		[Description("Over Time")]
		OverTime,
		[Description("Across Element")]
		AcrossElement,
		[Description("Vertical Across Element")]
		VerticalAcrossElement,
		[Description("Diagonal Bottom-Top Element")]
		DiagonalBottomTopElement,
		[Description("Diagonal Top-Bottom Element")]
		DiagonalTopBottomElement
	}
}