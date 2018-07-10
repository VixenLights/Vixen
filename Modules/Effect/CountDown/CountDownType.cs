using System.ComponentModel;

namespace VixenModules.Effect.CountDown
{
	public enum CountDownType
	{
		[Description("Countdown to end of Effect")]
		Effect,
		[Description("Count Down from specified Time")]
		CountDown,
		[Description("Count Up from specified Time")]
		CountUp
	}
}
