using System;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers
{
	internal interface IControllerLinkManager<T>
		where T : class, IOutputDevice, IHasOutputs
	{
		T GetController(Guid id);
		T GetNext(T controller);
		T GetPrior(T controller);
	}
}