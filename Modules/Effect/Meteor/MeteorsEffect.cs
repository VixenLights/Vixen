using System.ComponentModel;

namespace VixenModules.Effect.Meteors
{
	public enum MeteorsEffect
	{
		[Description("None")]
		None,
		[Description("Random")]
		RandomDirection,
		[Description("Explode")]
		Explode
	}
}