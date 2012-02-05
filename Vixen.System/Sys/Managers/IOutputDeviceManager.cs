using System;
using System.Collections.Generic;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers {
	public interface IOutputDeviceManager : IEnumerable<IOutputDevice> {
		void Start(IOutputDevice outputDevice);
		void Stop(IOutputDevice outputDevice);
		void StartAll();
		void StopAll();
		void StartAll(IEnumerable<IOutputDevice> outputDevices);
		void StopAll(IEnumerable<IOutputDevice> outputDevices);
		void Add(IOutputDevice outputDevice);
		void AddRange(IEnumerable<IOutputDevice> outputDevices);
		void Remove(IOutputDevice outputDevice);
		IOutputDevice Get(Guid id);
		IEnumerable<IOutputDevice> GetAll();
	}
}
