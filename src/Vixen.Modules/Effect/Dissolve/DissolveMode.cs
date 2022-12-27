using System.ComponentModel;

namespace VixenModules.Effect.Dissolve
{
	public enum DissolveMode
	{
		[Description("Effect Duration")]
		TimeInterval,
		[Description("Mark Collection")]
		MarkCollection
	}
}
