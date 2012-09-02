using System;
using System.Collections.Generic;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers {
	interface IOutputDeviceCollection<T> : IEnumerable<T> 
		where T : class, IOutputDevice {
		void Add(T outputDevice);
		bool Remove(T outputDevice);
		T Get(Guid id);
		IEnumerable<T> GetAll();
	}
}
