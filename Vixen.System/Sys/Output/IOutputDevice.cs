using System;

namespace Vixen.Sys.Output {
	/// <summary>
	/// Core abstraction for the in-memory output device.
	/// </summary>
	public interface IOutputDevice : IHardware, IHasSetup {
		Guid Id { get; }
		Guid ModuleId { get; }
		string Name { get; set; }
		void Update();
		int UpdateInterval { get; set; }
		IOutputDeviceUpdateSignaler UpdateSignaler { get; }
	}
}
