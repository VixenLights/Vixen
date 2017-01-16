using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.IO;
using Vixen.Sys;

namespace Vixen.Module
{
	[DataContract]
	public abstract class ModuleDataSet : IModuleDataSet, IDisposable
	{
		// Type data     = (type id, type id)     : data model
		// Instance data = (type id, instance id) : data model
		private Dictionary<Tuple<Guid,Guid>, IModuleDataModel> _dataModels;

		protected ModuleDataSet()
		{
			_Init();
		}

		/// <summary>
		/// Instantiates the data model for the module from the data in this data set.
		/// Does not account for the module's instance id, therefore only one instance of a module
		/// type can be in the dataset.
		/// </summary>
		/// <param name="module"></param>
		public void AssignModuleTypeData(IModuleInstance module)
		{
			if (module != null) {
				// 1. Module has a data object, we don't.
				//    - Add the module's data object
				// 2. Module has no data object, we do.
				//    - Get our data object and assign it in the module
				// 3. We and the module have a data object.
				//    - Get our data object and assign it in the module
				// 4. Neither has a data object.
				//    - We create one within ourselves and assign it in the module.
				IModuleDataModel dataModel = _GetDataInstance(module);
				if (!_ContainsTypeData(module.Descriptor.TypeId) && dataModel != null) {
					// We have no data, but the module does.  Add it.
					_AddAsTypeData(dataModel, module);
				}
				else {
					// In every other case, we have or can create data.
					//module.ModuleData = RetrieveTypeData(module.Descriptor);
					module.ModuleData = _GetOrCreateAsTypeData(module);
				}
				//* This behavior was put in for a reason now lost to history.
				//* KEEP THIS CODE, in case the reason is discovered.
				//// If the module already has data, add it, don't overwrite it.
				//IModuleDataModel dataModel = _GetDataInstance(module);
				//if(dataModel == null) {
				//    module.ModuleData = RetrieveTypeData(module.Descriptor);
				//} else {
				//    _Add(this, module.Descriptor.TypeId, module.InstanceId, dataModel);
				//}
			}
		}

		/// <summary>
		/// Retrieves the module type data without assigning it to a module instance.
		/// </summary>
		public IModuleDataModel GetTypeData(IModuleInstance module)
		{
			return _GetOrCreateAsTypeData(module);
		}

		/// <summary>
		/// Retrieves the module type data without assigning it to a module instance.
		/// </summary>
		public IModuleDataModel GetTypeData(Guid id)
		{
			return _GetOrCreateAsTypeData(id);
		}

		/// <summary>
		/// Instantiates the data model for the module from the data in this data set.
		/// Accounts for the module's instance id so there can be multiple instances
		/// of a module type in the dataset.
		/// </summary>
		/// <param name="module"></param>
		public void AssignModuleInstanceData(IModuleInstance module)
		{
			if (module != null) {
				// 1. Module has a data object, we don't.
				//    - Add the module's data object
				// 2. Module has no data object, we do.
				//    - Get our data object and assign it in the module
				// 3. We and the module have a data object.
				//    - Get our data object and assign it in the module
				// 4. Neither has a data object.
				//    - We create one within ourselves and assign it in the module.
				IModuleDataModel dataModel = _GetDataInstance(module);
				if (!_ContainsInstanceData(module.Descriptor.TypeId, module.InstanceId) && dataModel != null) {
					// We have no data, but the module does.  Add it.
					_AddAsInstanceData(dataModel, module);
				}
				else {
					// In every other case, we have or can create data.
					module.ModuleData = _GetOrCreateAsInstanceData(module);
				}
				//* This behavior was put in for a reason now lost to history.
				//* KEEP THIS CODE, in case the reason is discovered.
				//// If the module already has data, add it, don't overwrite it.
				//IModuleDataModel dataModel = _GetDataInstance(module);
				//if(dataModel == null) {
				//    module.ModuleData = RetrieveInstanceData(module);
				//} else {
				//    _Add(this, module.Descriptor.TypeId, module.InstanceId, dataModel);
				//}
			}
		}

		/// <summary>
		/// Retrieves the module data without assigning it to a module instance.
		/// </summary>
		public IModuleDataModel GetInstanceData(IModuleInstance module)
		{
			return _GetOrCreateAsInstanceData(module);
		}

		private bool _ContainsTypeData(Guid moduleTypeId)
		{
			return _dataModels.ContainsKey(Tuple.Create(moduleTypeId, moduleTypeId));
		}

		private bool _ContainsInstanceData(Guid moduleTypeId, Guid instanceId)
		{
			return _dataModels.ContainsKey(Tuple.Create(moduleTypeId, instanceId));
		}

		private IModuleDataModel _GetAsTypeData(IModuleInstance module)
		{
			IModuleDataModel model = null;
			_dataModels.TryGetValue(Tuple.Create(module.Descriptor.TypeId, module.Descriptor.TypeId), out model);
			return model;
		}

		private IModuleDataModel _GetAsTypeData(Guid id)
		{
			IModuleDataModel model = null;
			_dataModels.TryGetValue(Tuple.Create(id, id), out model);
			return model;
		}

		private IModuleDataModel _GetAsInstanceData(IModuleInstance module)
		{
			IModuleDataModel model = null;
			_dataModels.TryGetValue(Tuple.Create(module.TypeId, module.InstanceId), out model);
			return model;
		}

		private void _AddAsTypeData(IModuleDataModel dataModel, IModuleInstance module)
		{
			_Add(dataModel, module.Descriptor.TypeId, module.Descriptor.TypeId);
		}

		private void _AddAsTypeData(IModuleDataModel dataModel, Guid typeId)
		{
			_Add(dataModel, typeId, typeId);
		}

		private void _AddAsInstanceData(IModuleDataModel dataModel, IModuleInstance module)
		{
			_Add(dataModel, module.Descriptor.TypeId, module.InstanceId);
		}

		private void _Add(IModuleDataModel dataModel, Guid moduleTypeId, Guid moduleInstanceId)
		{
			if (dataModel == null) return; // throw new ArgumentNullException("dataModel");

			_dataModels.Add(Tuple.Create(moduleTypeId,moduleInstanceId),dataModel);
			dataModel.ModuleTypeId = moduleTypeId;
			dataModel.ModuleInstanceId = moduleInstanceId;
			dataModel.ModuleDataSet = this;
		}

		private void _RemoveTypeData(IModuleInstance module)
		{
			IModuleDataModel dataModel = _GetAsTypeData(module);
			if (dataModel != null) {
				_dataModels.Remove(Tuple.Create(module.Descriptor.TypeId, module.Descriptor.TypeId));
			}
		}

		private void _RemoveInstanceData(IModuleInstance module)
		{
			IModuleDataModel dataModel = _GetAsInstanceData(module);
			if (dataModel != null) {
				_dataModels.Remove(Tuple.Create(module.TypeId, module.InstanceId));
			}
		}

		internal void RemoveDataModel(Tuple<Guid, Guid> model)
		{
			_dataModels.Remove(model);
		}

		public void RemoveModuleTypeData(IModuleInstance module)
		{
			_RemoveTypeData(module);
		}

		public void RemoveModuleInstanceData(IModuleInstance module)
		{
			_RemoveInstanceData(module);
		}

		public void Clear()
		{
			_dataModels.Clear();
		}

		public IModuleDataSet Clone()
		{
			ModuleDataSet newDataSet = Activator.CreateInstance(GetType()) as ModuleDataSet;
			_Clone(this, newDataSet);
			return newDataSet;
		}

		public void Clone(IModuleDataSet sourceDataSet)
		{
			Clear();
			_Clone(sourceDataSet as ModuleDataSet, this);
		}

		private void _Clone(ModuleDataSet source, ModuleDataSet destination)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (destination == null) throw new ArgumentNullException("destination");

			// Clone exactly, assuming unchanged type and instance ids for the
			// modules the data belongs to.
			foreach (IModuleDataModel dataModel in source._dataModels.Values) {
				destination._Add(dataModel, dataModel.ModuleTypeId, dataModel.ModuleInstanceId);
			}
		}

		protected IModuleDataModel _GetOrCreateAsTypeData(IModuleInstance module)
		{
			IModuleDataModel dataModel = _GetAsTypeData(module);
			if (dataModel == null) {
				dataModel = _CreateDataModel(module);
				_AddAsTypeData(dataModel, module);
			}
			return dataModel;
		}

		protected IModuleDataModel _GetOrCreateAsTypeData(Guid typeId)
		{
			IModuleDataModel dataModel = _GetAsTypeData(typeId);
			if (dataModel == null) {
				dataModel = _CreateDataModel(typeId);
				_AddAsTypeData(dataModel, typeId);
			}
			return dataModel;
		}

		protected IModuleDataModel _GetOrCreateAsInstanceData(IModuleInstance module)
		{
			IModuleDataModel dataModel = _GetAsInstanceData(module);
			if (dataModel == null) {
				dataModel = _CreateDataModel(module);
				_AddAsInstanceData(dataModel, module);
			}
			return dataModel;
		}

		protected IModuleDataModel _CreateDataModel(IModuleInstance module)
		{
			Type dataModelType = _GetDataModelType(module.Descriptor);
			return _CreateDataModel(dataModelType);
		}

		protected IModuleDataModel _CreateDataModel(Guid typeId)
		{
			Type dataModelType = _GetDataModelType(Modules.GetDescriptorById(typeId));
			return _CreateDataModel(dataModelType);
		}

		protected static IModuleDataModel _CreateDataModel(Type dataModelType)
		{
			IModuleDataModel dataModel = null;

			if (dataModelType != null) {
				dataModel = Activator.CreateInstance(dataModelType) as IModuleDataModel;
			}

			return dataModel;
		}

		protected abstract Type _GetDataModelType(IModuleDescriptor descriptor);
		protected abstract IModuleDataModel _GetDataInstance(IModuleInstance module);

		public void Serialize(IModuleDataModelCollectionSerializer dataModelCollectionSerializer)
		{
			dataModelCollectionSerializer.Write(_dataModels.Values.ToList());
		}

		public void Deserialize(IModuleDataModelCollectionSerializer dataModelCollectionSerializer)
		{
			_dataModels.Clear();
			IEnumerable<IModuleDataModel> dataModels = dataModelCollectionSerializer.Read();
			foreach (IModuleDataModel dataModel in dataModels) {
				_Add(dataModel, dataModel.ModuleTypeId, dataModel.ModuleInstanceId);
			}
		}

		private void _Init()
		{
			_dataModels = new Dictionary<Tuple<Guid,Guid>, IModuleDataModel>();
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext streamingContext)
		{
			_Init();
		}

		internal IEnumerable<IModuleDataModel> DataModels
		{
			get { return _dataModels.Values.ToList(); }
			set { 
				//_dataModels = value.ToList();
				_dataModels.Clear();
				foreach(IModuleDataModel dataModel in value)
				{
					_Add(dataModel, dataModel.ModuleTypeId, dataModel.ModuleInstanceId);
				}
			}
		}

		~ModuleDataSet()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing) {
				if (_dataModels != null)
					_dataModels.Clear();
				_dataModels = null;
			}
		}
	}
}