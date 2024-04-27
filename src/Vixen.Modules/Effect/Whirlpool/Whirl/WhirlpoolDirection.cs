using System.ComponentModel;

namespace VixenModules.Effect.Whirlpool
{
	/// <summary>
	/// Defines the direction of the whirlpool.
	/// </summary>
	public enum WhirlpoolDirection
	{
		[Description("In")]
		In,

		[Description("Out")]
		Out,

		[Description("In & Out")]
		InAndOut,
	}
}