using Vixen.Data.Value;

namespace Vixen.Sys.Dispatch
{
	internal interface IAnyIntentSegmentHandler : IHandler<IIntentSegment<RangeValue<FunctionIdentity>>>,
	                                              IHandler<IIntentSegment<RGBValue>>,
	                                              IHandler<IIntentSegment<CommandValue>>,
	                                              IHandler<IIntentSegment<LightingValue>>		                                         
	{
	}
}