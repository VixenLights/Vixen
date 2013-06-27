using Vixen.Data.Value;

namespace Vixen.Sys.Dispatch
{
	internal interface IAnyIntentSegmentHandler : IHandler<IIntentSegment<PositionValue>>,
	                                              IHandler<IIntentSegment<ColorValue>>,
	                                              IHandler<IIntentSegment<CommandValue>>,
	                                              IHandler<IIntentSegment<LightingValue>>
	{
	}
}