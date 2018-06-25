using System.ComponentModel;

namespace VixenModules.Effect.CountDown
{
	public enum CountDownType
	{
		[Description("Calculate to end of Effect")]
		Effect,
		[Description("Calculate to end of Sequence")]
		Sequence
	}
}
