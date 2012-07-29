using Vixen.Data.Value;

namespace Vixen.Sys.Dispatch {
	interface IAnyIntentHandler : IHandler<IIntent<PositionValue>>, IHandler<IIntent<CommandValue>>, IHandler<IIntent<ColorValue>>, IHandler<IIntent<LightingValue>> {
	}
}
