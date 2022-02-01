using Vixen.Data.Value;

namespace Vixen.Sys.Dispatch
{
	internal interface IAnyIntentStateHandler : IHandler<IIntentState<RGBValue>>, 
		                                        IHandler<IIntentState<LightingValue>>,
	                                            IHandler<IIntentState<RangeValue>>, 
		                                        IHandler<IIntentState<CommandValue>>,
												IHandler<IIntentState<DiscreteValue>>, 
		                                        IHandler<IIntentState<IntensityValue>>
	{
	}
}