using System.ComponentModel;
using Vixen.TypeConverters;

namespace VixenModules.Effect.Wipe
{
	public enum WipeMovement
	{
		[Description("Count")]
		Count,
		[Description("Pulse Length")]
		PulseLength,
		[Description("Movement")]
		Movement
	}
}
