using System;
using System.Collections.Generic;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers {
	public interface IOutputDeviceFacadeParticipant {
		void StartOnly(IEnumerable<IOutputDevice> outputDevices);
		void StartAll();
		void StopAll();
		void PauseAll();
		void ResumeAll();
		IEnumerable<IOutputDevice> Devices { get; }
		IOutputDevice GetDevice(Guid id);
	}
}
