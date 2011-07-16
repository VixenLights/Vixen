using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Property;

namespace Vixen.Sys {
	public class PropertyManager : IEnumerable<IPropertyModuleInstance> {
		private Dictionary<Guid, IPropertyModuleInstance> _items = new Dictionary<Guid, IPropertyModuleInstance>();
		private IModuleDataSet _propertyData;
		private ChannelNode _owner;

		public PropertyManager(ChannelNode owner) {
			_owner = owner;
			PropertyData = new ModuleDataSet();
		}

		public IPropertyModuleInstance Add(Guid id) {
			IPropertyModuleInstance instance = null;

			if(!_items.ContainsKey(id)) {
				instance = Modules.ModuleManagement.GetProperty(id);
				if(instance != null) {
					instance.Owner = _owner;
					_items[id] = instance;
					PropertyData.GetModuleTypeData(instance);
				}
			}

			return instance;
		}

		public void Remove(Guid id) {
			IPropertyModuleInstance instance;
			if(_items.TryGetValue(id, out instance)) {
				instance.Owner = null;
				_items.Remove(id);
				PropertyData.Remove(id);
			}
		}

		public void Clear() {
			_items.Clear();
			PropertyData.Clear();
		}

		public IPropertyModuleInstance Get(Guid propertyTypeId) {
			IPropertyModuleInstance instance;
			_items.TryGetValue(propertyTypeId, out instance);
			return instance;
		}

		public IModuleDataSet PropertyData {
			get { return _propertyData; }
			private set {
				_propertyData = value;
				// Update any properties we already have.
				foreach(IPropertyModuleInstance propertyModule in _items.Values) {
					_propertyData.GetModuleTypeData(propertyModule);
				}
			}
		}

		public IEnumerator<IPropertyModuleInstance> GetEnumerator() {
			return _items.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
