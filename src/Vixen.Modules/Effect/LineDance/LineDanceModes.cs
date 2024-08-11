using System.ComponentModel;

namespace VixenModules.Effect.Fan
{
	/// <summary>
	/// Defines the center options for when there is an odd number of Intelligent Fixtures involved in the Fan effect.
	/// </summary>
	public enum FanCenterOptions
	{		
		[Description("Centered")]
		Centered,
		[Description("Left")]
		Left,
		[Description("Right")]
		Right,		
	}
}