using System.ComponentModel;

namespace VixenModules.Effect.CountDown
{
	public enum CountDownType
	{
		[Description("Calculate to end of Effect")]
		Effect,
		[Description("Calculate to user specified Sequence Time")]
		Sequence
	}
}
