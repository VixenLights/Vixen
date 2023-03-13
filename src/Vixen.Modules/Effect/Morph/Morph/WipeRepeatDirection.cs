using System.ComponentModel;

namespace VixenModules.Effect.Morph
{
	/// <summary>
	/// Defines the repeat direction of the wipe.
	/// </summary>
	public enum WipeRepeatDirection
	{
		[Description("Left")]
		Left,

		[Description("Right")]
		Right,

		[Description("Up")]
		Up,

		[Description("Down")]
		Down,
	};

}
