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
	public enum MeteorStartPosition
	{
		[Description("Random")]
		Random,
		[Description("Random then Zero Position")]
		InitiallyRandom,
		[Description("Zero Position")]
		ZeroPosition
	}
}