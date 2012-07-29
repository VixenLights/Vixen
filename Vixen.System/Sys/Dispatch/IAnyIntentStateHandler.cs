using Vixen.Data.Value;

namespace Vixen.Sys.Dispatch {
	interface IAnyIntentStateHandler : IHandler<IIntentState<ColorValue>>, IHandler<IIntentState<LightingValue>>, IHandler<IIntentState<PositionValue>>, IHandler<IIntentState<CommandValue>> {
	}
}
