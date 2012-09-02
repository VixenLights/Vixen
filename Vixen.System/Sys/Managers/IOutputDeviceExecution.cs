using System.Collections.Generic;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers {
	interface IOutputDeviceExecution<in T>
		where T : class, IOutputDevice {
		void Start(T outputDevice);
		void Stop(T outputDevice);
		void Pause(T outputDevice);
		void Resume(T outputDevice);
		void StartAll();
		void StartAll(IEnumerable<T> outputDevices);
		void StopAll();
		void StopAll(IEnumerable<T> outputDevices);
		void PauseAll();
		void PauseAll(IEnumerable<T> outputDevices);
		void ResumeAll();
		void ResumeAll(IEnumerable<T> outputDevices);
		ExecutionState ExecutionState { get; }
	}
}
