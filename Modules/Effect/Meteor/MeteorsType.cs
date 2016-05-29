using System.ComponentModel;

namespace VixenModules.Effect.Meteors
{
	public enum MeteorsType
	{
		[Description("Standard")]
		Standard,
		[Description("Random")]
		RandomDirection,
		[Description("Explode")]
		Explode
	}
}