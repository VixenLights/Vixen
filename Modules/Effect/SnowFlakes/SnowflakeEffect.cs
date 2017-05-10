using System.ComponentModel;

namespace VixenModules.Effect.Snowflakes
{
	public enum SnowflakeEffect
	{
		[Description("Falling")]
		None,
		[Description("Random Direction")]
		RandomDirection,
		[Description("Explode")]
		Explode
	}
}