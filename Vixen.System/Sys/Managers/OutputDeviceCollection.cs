using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers {
	class OutputDeviceCollection<T> : IOutputDeviceCollection<T> 
		where T : class, IOutputDevice {
		private ConcurrentDictionary<Guid, T> _instances;

		public OutputDeviceCollection() {
			_instances = new ConcurrentDictionary<Guid, T>();
		}

		public void Add(T outputDevice) {
			_Add(outputDevice);
		}

		public bool Remove(T outputDevice) {
			return _Remove(outputDevice);
		}

		public T Get(Guid id) {
			T outputDevice;
			_instances.TryGetValue(id, out outputDevice);
			return outputDevice;
		}

		public IEnumerable<T> GetAll() {
			return _instances.Values.ToArray();
		}

		public IEnumerator<T> GetEnumerator() {
			// Enumerate against a copy of the collection.
			return _instances.Values.ToList().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		private void _Add(T outputDevice) {
			_instances[outputDevice.Id] = outputDevice;
		}

		private bool _Remove(T outputDevice) {
			T removedDevice;
			return _instances.TryRemove(outputDevice.Id, out removedDevice);
		}
	}
}
