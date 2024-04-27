using System.ComponentModel;

namespace VixenModules.Effect.Whirlpool
{
	/// <summary>
	/// Defines the start location of the whirlpool.
	/// </summary>
	public enum WhirlpoolStartLocation
	{
		[Description("Top Left")]
		TopLeft,

		[Description("Bottom Left")]
		BottomLeft,

		[Description("Top Right")]
		TopRight,

		[Description("Bottom Right")]
		BottomRight,
	}
}
