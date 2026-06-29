using System.Drawing;
using VixenModules.Property.State;

namespace VixenModules.Effect.State
{
	internal sealed record StateRenderInterval(
		StateItemData Item,
		TimeSpan Start,
		TimeSpan Duration,
		Color? ColorOverride = null);
}
