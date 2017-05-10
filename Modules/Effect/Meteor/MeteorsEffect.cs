using System.ComponentModel;

namespace VixenModules.Effect.Meteors
{
	public enum MeteorsEffect
	{
		[Description("Falling")]
		None,
		[Description("Random")]
		RandomDirection,
		[Description("Explode")]
		Explode
	}
}