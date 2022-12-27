using System.ComponentModel;

namespace VixenModules.Effect.Snowflakes
{
	public enum SnowflakeEffect
	{
		[Description("Falling")]
		None,
		[Description("Random")]
		RandomDirection,
		[Description("Explode")]
		Explode,
		[Description("Grid")]
		Grid,
		[Description("Grid Offset")]
		GridOffset
	}
}