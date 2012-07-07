using System;

namespace Vixen.Sys.Output {
	//*** clarify use of this and IOutputModule
	public interface IOutputDevice : IHardware, IHasSetup {
		Guid Id { get; }
		string Name { get; set; }
		int UpdateInterval { get; set; }
		void Update();
		IOutputDeviceUpdateSignaler UpdateSignaler { get; }
	}
}
