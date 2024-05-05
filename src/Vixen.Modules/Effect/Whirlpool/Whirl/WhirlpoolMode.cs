using System.ComponentModel;

namespace VixenModules.Effect.Whirlpool
{
	/// <summary>
	/// Defines the mode of the whirlpool.
	/// </summary>
	public enum WhirlpoolMode
	{
		[Description("Continuous Whirls")]
		RecurrentWhirls,

		[Description("Concentric Whirls")]
		SymmetricalWhirls,

		[Description("Meteor")]
		Meteor,
	}
}
