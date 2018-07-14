using System.ComponentModel;

namespace VixenModules.Effect.CountDown
{
	public enum CountDownDirection
	{
		[Description("Left")]
		Left,
		[Description("Right")]
		Right,
		[Description("Up")]
		Up,
		[Description("Down")]
		Down,
		[Description("Rotate")]
		Rotate,
		[Description("None")]
		None
		
	}
}