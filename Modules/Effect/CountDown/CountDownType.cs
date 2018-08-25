using System.ComponentModel;

namespace VixenModules.Effect.CountDown
{
	public enum CountDownType
	{
		[Description("To end of effect")]
		Effect,
		[Description("Down from time")]
		CountDown,
		[Description("Up from time")]
		CountUp
	}

	public enum SizeMode
	{
		[Description("None")]
		None,
		[Description("Shrink")]
		Shrink,
		[Description("Grow")]
		Grow
	}
}
