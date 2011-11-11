using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Media;
using System.IO;

namespace Vixen.Sys {
	public class MediaCollection : IList<IMediaModuleInstance> {
		private List<IMediaModuleInstance> _modules = new List<IMediaModuleInstance>();
		private IModuleDataSet _moduleDataSet;
		
		public MediaCollection(IModuleDataSet moduleDataSet) {
			if(moduleDataSet == null) throw new ArgumentNullException("moduleDataSet");
			_moduleDataSet = moduleDataSet;
			// Create populated instances for each media module in the dataset.
			_modules.AddRange(_moduleDataSet.GetInstances<IMediaModuleInstance>());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath"></param>
		/// <exception cref="NotImplementedException">There is no media module to support the selected file.</exception>
		public IMediaModuleInstance Add(string filePath)
		{
			MediaModuleManagement manager = Modules.GetManager<IMediaModuleInstance, MediaModuleManagement>();
			IMediaModuleInstance instance = manager.Get(filePath);
			if(instance != null) {
				// Add new instance data to the dataset.
				_moduleDataSet.GetModuleInstanceData(instance);
				// Set the file in the instance.
				instance.MediaFilePath = filePath;
				// Add to the collection.
				_modules.Add(instance);
			}

			return instance;
		}

		public int IndexOf(IMediaModuleInstance item) {
			return _modules.IndexOf(item);
		}

		public void Insert(int index, IMediaModuleInstance item) {
			_moduleDataSet.GetModuleInstanceData(item);
			_modules.Insert(index, item);
		}

		public void RemoveAt(int index) {
			if(index < _modules.Count) {
				IMediaModuleInstance instance = _modules[index];
				_moduleDataSet.RemoveModuleInstanceData(instance.Descriptor.TypeId, instance.InstanceId);
				_modules.RemoveAt(index);
			}
		}

		public void RemoveAll() {
			_modules.Clear();
			_moduleDataSet.Clear();
		}

		public IMediaModuleInstance this[int index] {
			get { return _modules[index]; }
			set {
				IMediaModuleInstance instance = _modules[index];

				// Remove the data of the existing instance.
				_moduleDataSet.RemoveModuleInstanceData(instance.Descriptor.TypeId, instance.InstanceId);
				// Add data for the new instance.
				_moduleDataSet.GetModuleInstanceData(value);

				_modules[index] = value;
			}
		}

		public void Add(IMediaModuleInstance item) {
			throw new NotImplementedException();
		}

		public void Clear() {
			_moduleDataSet.Clear();
			_modules.Clear();
		}

		public bool Contains(IMediaModuleInstance item) {
			return _modules.Contains(item, new MediaEqualityComparer());
		}

		public void CopyTo(IMediaModuleInstance[] array, int arrayIndex) {
			_modules.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return _modules.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove(IMediaModuleInstance item) {
			if(_modules.Remove(item)) {
				_moduleDataSet.RemoveModuleInstanceData(item.Descriptor.TypeId, item.InstanceId);
				return true;
			}
			return false;
		}

		public IEnumerator<IMediaModuleInstance> GetEnumerator() {
			return _modules.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}

	class MediaEqualityComparer : IEqualityComparer<IMediaModuleInstance> {
		public bool Equals(IMediaModuleInstance x, IMediaModuleInstance y) {
			return x.MediaFilePath == y.MediaFilePath;
		}

		public int GetHashCode(IMediaModuleInstance obj) {
			return obj.MediaFilePath.GetHashCode();
		}
	}
}
