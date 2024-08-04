using System.ComponentModel;

namespace VixenModules.Effect.Fan
{
	/// <summary>
	/// Defines the center options for when there is an odd number of Intelligent Fixtures involved in the Fan effect.
	/// </summary>
	public enum CenterOptions
	{
		[Description("Off")]
		Off,
		[Description("Centered")]
		Centered,
		[Description("Pan Left")]
		Left,
		[Description("Pan Right")]
		Right,		
	}
}