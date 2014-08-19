using System;

namespace Vixen.Sys.Output
{
	/// <summary>
	/// Core abstraction for the in-memory output device.
	/// </summary>
	public interface IOutputDevice : IHardware, IHasSetup
	{
		Guid Id { get; }
		Guid ModuleId { get; }
		string Name { get; set; }
		void Update();
		/// <summary>
		/// Only update the commands, do not send them out.
		/// </summary>
		void UpdateCommands();
		int UpdateInterval { get; set; }
		IOutputDeviceUpdateSignaler UpdateSignaler { get; }
	}
}