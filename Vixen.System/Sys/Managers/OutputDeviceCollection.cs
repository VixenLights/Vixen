using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers
{
	internal class OutputDeviceCollection<T> : IOutputDeviceCollection<T>
		where T : class, IOutputDevice
	{
		private readonly OrderedDeviceDictionary<Guid, T> _instances;

		public OutputDeviceCollection()
		{
			_instances = new OrderedDeviceDictionary<Guid, T>();
		}

		public void Add(T outputDevice)
		{
			_Add(outputDevice);
		}

		public bool Remove(T outputDevice)
		{
			return _Remove(outputDevice);
		}

		public T Get(Guid id)
		{
			T outputDevice;
			lock (_instances)
			{
				_instances.TryGetValue(id, out outputDevice);	
			}
			return outputDevice;
		}

		public IEnumerable<T> GetAll()
		{
			lock (_instances)
			{
				return _instances.Values.ToArray();
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			// Enumerate against a copy of the collection.
			lock (_instances)
			{
				return _instances.Values.ToList().GetEnumerator();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void _Add(T outputDevice)
		{
			lock (_instances)
			{
				_instances[outputDevice.Id] = outputDevice;
			}
		}

		private bool _Remove(T outputDevice)
		{
			//T removedDevice;
			//return _instances.TryRemove(outputDevice.Id, out removedDevice);
			lock (_instances)
			{
				return _instances.Remove(outputDevice.Id);
			}
		}
	}
}