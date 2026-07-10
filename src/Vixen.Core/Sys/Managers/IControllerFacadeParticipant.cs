using Vixen.Data.Flow;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers
{
	public interface IControllerFacadeParticipant : IOutputDeviceFacadeParticipant
	{
		IDataFlowComponent GetDataFlowComponentForOutput(IOutputDevice controller, int outputIndex);
	}
}