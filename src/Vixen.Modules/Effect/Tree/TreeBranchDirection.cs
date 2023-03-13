using System.ComponentModel;

namespace VixenModules.Effect.Tree
{
	public enum TreeBranchDirection
	{
		[Description("Up")]
		Up,
		[Description("Down")]
		Down,
		[Description("Left")]
		Left,
		[Description("Right")]
		Right,
		[Description("Up/Right")]
		UpRight,
		[Description("Up/Left")]
		UpLeft,
		[Description("Down/Right")]
		DownRight,
		[Description("Down/Left")]
		DownLeft,
		[Description("Alternate")]
		Alternate,
		[Description("None")]
		None
	}
}