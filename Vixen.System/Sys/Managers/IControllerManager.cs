using System;
using Vixen.Data.Flow;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers
{
	internal interface IControllerManager<T> : IOutputDeviceManager<T>
		where T : class, IOutputDevice, IHasOutputs
	{
		T GetController(Guid id);
		IDataFlowComponent GetDataFlowComponentForOutput(T controller, int outputIndex);
	}
}