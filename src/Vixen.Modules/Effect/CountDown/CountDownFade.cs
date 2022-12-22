using System.ComponentModel;

namespace VixenModules.Effect.CountDown
{
	public enum CountDownFade
	{
		[Description("None")]
		None,
		[Description("In")]
		In,
		[Description("Out")]
		Out,
		[Description("In/Out")]
		InOut
	}
}
