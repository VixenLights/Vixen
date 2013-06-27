using System.Threading;

namespace Vixen.Sys.Output
{
	public interface IOutputDeviceUpdateSignaler
	{
		IOutputDevice OutputDevice { set; }
		EventWaitHandle UpdateSignal { set; }
		void RaiseSignal();
	}
}