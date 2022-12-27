using System.ComponentModel;

namespace VixenModules.Effect.Bars
{
	public enum BarDirection
	{
		[Description("Moves Up")]
		Up,
		[Description("Moves Down")]
		Down,
		[Description("Expands")]
		Expand,
		[Description("Compresses")]
		Compress,
		[Description("Moves Left")]
		Left,
		[Description("Moves Right")]
		Right,
		[Description("Horizontal Expand")]
		HExpand,
		[Description("Horizontal Compress")]
		HCompress,
		[Description("Alternate Up")]
		AlternateUp,
		[Description("Alternate Down")]
		AlternateDown,
		[Description("Alternate Left")]
		AlternateLeft,
		[Description("Alternate Right")]
		AlternateRight
	}
}