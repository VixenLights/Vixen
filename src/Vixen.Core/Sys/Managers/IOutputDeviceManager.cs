using Vixen.Sys.Output;

namespace Vixen.Sys.Managers
{
	internal interface IOutputDeviceManager<T> : IOutputDeviceCollection<T>, IOutputDeviceExecution<T>
		where T : class, IOutputDevice
	{
	}
}