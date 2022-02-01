using Vixen.Data.Value;

namespace Vixen.Sys.Dispatch
{
	internal interface IAnyIntentHandler : IHandler<IIntent<RangeValue>>, 
		                                   IHandler<IIntent<CommandValue>>,
	                                       IHandler<IIntent<RGBValue>>, 
		                                   IHandler<IIntent<LightingValue>>,
										   IHandler<IIntent<DiscreteValue>>, 
		                                   IHandler<IIntent<IntensityValue>>
	{
	}
}