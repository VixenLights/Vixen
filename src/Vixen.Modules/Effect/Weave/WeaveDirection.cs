using System.ComponentModel;

namespace VixenModules.Effect.Weave
{
	/// <summary>
	/// Defines the movement types for the Weave effect.
	/// </summary>
	public enum WeaveDirection
	{
		[Description("Moves Up")]
		Up,
		[Description("Moves Down")]
		Down,
		[Description("Vertical Expands")]
		Expand,
		[Description("Vertical Compress")]
		Compress,
		[Description("Moves Left")]
		Left,
		[Description("Moves Right")]
		Right,
		[Description("Horizontal Expand")]
		HorizontalExpand,
		[Description("Horizontal Compress")]
		HorizontalCompress,
		[Description("Center Compress")]
		CenterCompress,
		[Description("Center Expand")]
		CenterExpand,
	}
}