using System.ComponentModel;

namespace VixenModules.Effect.Meteors
{
	public enum MeteorMovement
	{
		[Description("None")]
		None,
		[Description("Bounce")]
		Bounce,
		[Description("Wrap")]
		Wrap,
		[Description("Speed")]
		Speed,
		[Description("Wobble")]
		Wobble,
		[Description("Wobble Both Directions")]
		Wobble2
	}
}