using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Module;
using Vixen.Module.Property;

namespace Vixen.Sys
{
	public class PropertyManager : IEnumerable<IPropertyModuleInstance>
	{
		private Dictionary<Guid, IPropertyModuleInstance> _items = new Dictionary<Guid, IPropertyModuleInstance>();
		//private ModuleLocalDataSet _propertyData;
		private IElementNode _owner;

		public PropertyManager(IElementNode owner)
		{
			_owner = owner;
			//PropertyData = new ModuleLocalDataSet();
		}

		public IPropertyModuleInstance Add(IPropertyModuleInstance instance)
		{
			//IPropertyModuleInstance instance = null;

			if (!_items.ContainsKey(instance.TypeId)) {
				//instance = Modules.ModuleManagement.GetProperty(id);
				if (instance != null) {
					instance.Owner = _owner;
					instance.SetDefaultValues();
					_items[instance.TypeId] = instance;
					PropertyData.AssignModuleInstanceData(instance);
				}
			}

			return instance;
		}

		public IPropertyModuleInstance Add(Guid id)
		{
			IPropertyModuleInstance instance = null;

			if (!_items.ContainsKey(id)) {
				instance = Modules.ModuleManagement.GetProperty(id);
				if (instance != null) {
					instance.Owner = _owner;
					instance.SetDefaultValues();
					_items[id] = instance;
					PropertyData.AssignModuleInstanceData(instance);
				}
			}

			return instance;
		}

		public void Remove(Guid id)
		{
			IPropertyModuleInstance instance;
			if (_items.TryGetValue(id, out instance)) {
				instance.Owner = null;
				_items.Remove(id);
				PropertyData.RemoveModuleInstanceData(instance);
			}
		}

		public void Clear()
		{
			_items.Clear();
			PropertyData.Clear();
		}

		public IPropertyModuleInstance Get(Guid propertyTypeId)
		{
			IPropertyModuleInstance instance;
			_items.TryGetValue(propertyTypeId, out instance);
			return instance;
		}

		public bool Contains(Guid propertyTypeId)
		{
			return _items.ContainsKey(propertyTypeId);
		}

		public ModuleLocalDataSet PropertyData
		{
			get { return VixenSystem.ModuleStore.InstanceData; }
		}

		//This is a pure hack to clean up orphaned property data sets.
		private static HashSet<Guid> _typeIds = new HashSet<Guid>
		{
				new Guid("{BFF34727-6B88-4F87-82B7-68424498C725}"), //Color property
				new Guid("{3FB53423-DD2A-4719-B3E3-19AA6F062F62}")};  //Location property


		//This is a pure hack to clean up orphaned property data sets.
		public static void RemoveOrphanedProperties()
		{
			var propertyDataModules = VixenSystem.ModuleStore.InstanceData.DataModels.Where(x => _typeIds.Contains(x.ModuleTypeId));
			var instanceIds = new HashSet<Guid>(VixenSystem.Nodes.GetLeafNodes().SelectMany(x => x.Properties._items.Values).Select(p => p.InstanceId));

			var orphanedData = propertyDataModules.Where(d => !instanceIds.Contains(d.ModuleInstanceId));

			foreach (var property in orphanedData)
			{
				VixenSystem.ModuleStore.InstanceData.RemoveDataModel(Tuple.Create(property.ModuleTypeId, property.ModuleInstanceId));
			}
		}

		//public ModuleLocalDataSet PropertyData {
		//    get { return _propertyData; }
		//    set {
		//        _propertyData = value;
		//        // Update any properties we already have.
		//        foreach(IPropertyModuleInstance propertyModule in _items.Values) {
		//            _propertyData.AssignModuleTypeData(propertyModule);
		//        }
		//    }
		//}

		public IEnumerator<IPropertyModuleInstance> GetEnumerator()
		{
			return _items.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}