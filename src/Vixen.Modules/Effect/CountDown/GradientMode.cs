using System.ComponentModel;

namespace VixenModules.Effect.CountDown
{
	public enum GradientMode
	{
		[Description("Across Text")]
		AcrossText,
		[Description("Across Element")]
		AcrossElement,
		[Description("Vertical Across Text")]
		VerticalAcrossText,
		[Description("Vertical Across Element")]
		VerticalAcrossElement,
		[Description("Diagonal Across Text")]
		DiagonalAcrossText,
		[Description("Diagonal Across Element")]
		DiagonalAcrossElement,
		[Description("Reverse Diagonal Across Text")]
		BackwardDiagonalAcrossText,
		[Description("Reverse Diagonal Across Element")]
		BackwardDiagonalAcrossElement
		
	}
}