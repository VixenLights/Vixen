using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.Module {
	[DataContract]
    public abstract class ModuleDataSet : IModuleDataSet {
		// Type data     = (type id, type id)     : data model
		// Instance data = (type id, instance id) : data model
		private Dictionary<Tuple<Guid, Guid>, IModuleDataModel> _dataModels = new Dictionary<Tuple<Guid, Guid>, IModuleDataModel>();

		private const string ROOT_ELEMENT = "ModuleDataSet";
		private const string ELEMENT_MODULE = "Module";
		private const string ATTR_MODULE_TYPE = "moduleType";
		private const string ATTR_MODULE_INSTANCE = "moduleInstance";

		/// <summary>
		/// Instantiates the data model for the module from the data in this data set.
		/// Does not account for the module's instance id, therefore only instance of a module
		/// type can be in the dataset.
		/// </summary>
		/// <param name="module"></param>
		public void GetModuleTypeData(IModuleInstance module) {
			if(module != null) {
				// 1. Module has a data object, we don't.
				//    - Add the module's data object
				// 2. Module has no data object, we do.
				//    - Get our data object and assign it in the module
				// 3. We and the module have a data object.
				//    - Get our data object and assign it in the module
				// 4. Neither has a data object.
				//    - We create one within ourselves and assign it in the module.
				if(!Contains(module.Descriptor.TypeId) && module.ModuleData != null) {
					// We have no data, but the module does.  Add it.
					IModuleDataModel dataModel = _GetDataInstance(module);
					_Add(this, module.Descriptor.TypeId, module.Descriptor.TypeId, dataModel);
				} else {
					// In every other case, we have or can create data.
					module.ModuleData = RetrieveTypeData(module.Descriptor);
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
		/// <param name="instance"></param>
		/// <returns></returns>
		public IModuleDataModel RetrieveTypeData(IModuleDescriptor descriptor) {
			return _GetModuleData(descriptor, descriptor.TypeId);
		}

		/// <summary>
		/// Instantiates the data model for the module from the data in this data set.
		/// Accounts for the module's instance id so there can be multiple instances
		/// of a module type in the dataset.
		/// </summary>
		/// <param name="module"></param>
		public void GetModuleInstanceData(IModuleInstance module) {
			if(module != null) {
				// 1. Module has a data object, we don't.
				//    - Add the module's data object
				// 2. Module has no data object, we do.
				//    - Get our data object and assign it in the module
				// 3. We and the module have a data object.
				//    - Get our data object and assign it in the module
				// 4. Neither has a data object.
				//    - We create one within ourselves and assign it in the module.
				if(!Contains(module.Descriptor.TypeId, module.InstanceId) && module.ModuleData != null) {
					// We have no data, but the module does.  Add it.
					IModuleDataModel dataModel = _GetDataInstance(module);
					_Add(this, module.Descriptor.TypeId, module.InstanceId, dataModel);
				} else {
					// In every other case, we have or can create data.
					module.ModuleData = RetrieveInstanceData(module);
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
		/// <param name="instance"></param>
		/// <returns></returns>
		public IModuleDataModel RetrieveInstanceData(IModuleInstance instance) {
			return _GetModuleData(instance.Descriptor, instance.Descriptor.TypeId, instance.InstanceId);
		}

		private IModuleDataModel _GetModuleData(IModuleDescriptor descriptor, Guid moduleTypeId) {
			return _GetModuleData(descriptor, moduleTypeId, moduleTypeId);
		}

		private IModuleDataModel _GetModuleData(IModuleDescriptor descriptor, Guid moduleTypeId, Guid moduleInstanceId) {
			IModuleDataModel dataModel = null;

			Tuple<Guid, Guid> key = new Tuple<Guid, Guid>(moduleTypeId, moduleInstanceId);
			
			// If there isn't any data -- and the module type actually has a data class -- create it and add it to the dataset.
			if(!_dataModels.TryGetValue(key, out dataModel)) {
				Type dataModelClass = _GetDataSetType(descriptor);
				if (dataModelClass != null) {
					dataModel = _CreateModuleDataInstance(dataModelClass, moduleTypeId, moduleInstanceId);
					_Add(this, moduleTypeId, moduleInstanceId, dataModel);
				}
			}

			return dataModel;
		}

		static protected IModuleDataModel _CreateModuleDataInstance(Type dataModelClass, Guid moduleTypeId, Guid moduleInstanceId) {
			IModuleDataModel dataModel = null;

			// The module may not have defined a data model because it may not have a
			// need for data.
			if(dataModelClass != null) {
				dataModel = Activator.CreateInstance(dataModelClass) as IModuleDataModel;
				dataModel.ModuleTypeId = moduleTypeId;
				dataModel.ModuleInstanceId = moduleInstanceId;
			}

			return dataModel;
		}

		private void _SetPedigree(IModuleDataModel dataModel, Guid owningModuleTypeId, Guid moduleInstanceId) {
			// Where the data came from...
			dataModel.ModuleDataSet = this;
			dataModel.ModuleInstanceId = moduleInstanceId;
			// Type of module it belongs to...
			dataModel.ModuleTypeId = owningModuleTypeId;
		}

		static private void _Add(ModuleDataSet dataSet, Guid moduleTypeId, Guid moduleInstanceId, IModuleDataModel moduleData) {
			if(dataSet == null) throw new ArgumentNullException("dataSet");
			if(moduleData == null) throw new ArgumentNullException("moduleData");

			Tuple<Guid, Guid> key = new Tuple<Guid, Guid>(moduleTypeId, moduleInstanceId);

			if(!dataSet._dataModels.ContainsKey(key)) {
				dataSet._dataModels[key] = moduleData;
				moduleData.ModuleDataSet = dataSet;
			}
		}

		static private void _Remove(ModuleDataSet dataSet, Tuple<Guid,Guid> key) {
			if(dataSet == null) throw new ArgumentNullException("dataSet");
			if(key == null) throw new ArgumentNullException("key");

			IModuleDataModel dataModel;
			if(dataSet._dataModels.TryGetValue(key, out dataModel)) {
				dataModel.ModuleDataSet = null;
				dataSet._dataModels.Remove(key);
			}
		}


		/// <summary>
		/// Use when not using XElement for serialization.
		/// </summary>
		/// <returns></returns>
        public string Serialize() {
			XElement moduleDataSetElement = new XElement(ROOT_ELEMENT);
            DataContractSerializer serializer;

			// Module type id : module type data object
			foreach(KeyValuePair<Tuple<Guid, Guid>, IModuleDataModel> kvp in _dataModels) {
				using(MemoryStream stream = new MemoryStream()) {
					// Serializing each data object as the data object's type.
					serializer = new DataContractSerializer(kvp.Value.GetType());
					serializer.WriteObject(stream, kvp.Value);
					string objectData = Encoding.ASCII.GetString(stream.ToArray()).Trim();
					moduleDataSetElement.Add(
						new XElement(ELEMENT_MODULE,
							new XAttribute(ATTR_MODULE_TYPE, kvp.Key.Item1),
							new XAttribute(ATTR_MODULE_INSTANCE, kvp.Key.Item2),
							XElement.Parse(objectData))
						);
				}
			}

            return moduleDataSetElement.ToString();
        }

		public XElement ToXElement() {
			return XElement.Parse(Serialize());
		}

		public void Deserialize(string xmlText) {
            _dataModels.Clear();

            if(!string.IsNullOrEmpty(xmlText)) {
                XElement moduleDataSetElement = XElement.Parse(xmlText);
                // Three possibilities:
                // 1. The consumer gave us exactly what we needed, our ModuleDataSet element.
                // 2. The consumer gave us our node plus their wrapping parent node.
                // 3. The consumer shouldn't be doing what they're doing.
				if(moduleDataSetElement.Name != ROOT_ELEMENT) {
                    // They didn't give us just our node.
					if((moduleDataSetElement = moduleDataSetElement.Element(ROOT_ELEMENT)) == null) {
                        // They didn't give us anything we could quickly find, so leave.
                        return;
                    }
                }

				Guid moduleTypeId;
				Guid moduleInstanceId;
                IModuleDescriptor descriptor;
                DataContractSerializer serializer;
				IModuleDataModel dataModel;
				foreach(XElement moduleElement in moduleDataSetElement.Elements(ELEMENT_MODULE)) {
                    //Storing the module type id...
                    // Get the module type.
					moduleTypeId = new Guid(moduleElement.Attribute(ATTR_MODULE_TYPE).Value);
					moduleInstanceId = new Guid(moduleElement.Attribute(ATTR_MODULE_INSTANCE).Value);
                    // Get the descriptor for the type.
                    descriptor = Modules.GetDescriptorById(moduleTypeId);
                    if(descriptor != null) {
                        // Create the serializer for the module type's data model type.
						//serializer = new DataContractSerializer(descriptor.ModuleDataClass);
						Type dataSetType = _GetDataSetType(descriptor);
						// If the module previously had a data object but no longer does,
						// dataSetType may be null.
						if(dataSetType != null) {
							serializer = new DataContractSerializer(dataSetType);
							dataModel = null;
							using(MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(moduleElement.FirstNode.ToString()))) {
								try {
									dataModel = serializer.ReadObject(stream) as IModuleDataModel;
								} catch(Exception ex) {
									string instanceDataSpecifier = _IsInstanceData(moduleTypeId, moduleInstanceId) ? "(instance data)" : "(type data)";
									VixenSystem.Logging.Error("The data for module \"" + descriptor.TypeName + "\" was not loaded due to errors.");
									VixenSystem.Logging.Debug("Module \"" + descriptor.TypeName + "\" had data " + instanceDataSpecifier + " that was invalid.", ex);
								}
							}

							if(dataModel != null) {
								_SetPedigree(dataModel, moduleTypeId, moduleInstanceId);
								_Add(this, moduleTypeId, moduleInstanceId, dataModel);
							}
						}
                    }
                }
            }
        }

		/// <summary>
		/// Adds the module's data object as type data.  Does not overwrite any existing data.
		/// </summary>
		/// <param name="instance"></param>
		public void AddModuleTypeData(IModuleInstance instance) {
			if(instance.ModuleData != null) {
				_Add(this, instance.Descriptor.TypeId, instance.Descriptor.TypeId, instance.ModuleData);
			}
		}

		/// <summary>
		/// Adds the module's data object as instance data.  Does not overwrite any existing data.
		/// </summary>
		/// <param name="instance"></param>
		public void AddModuleInstanceData(IModuleInstance instance) {
			if(instance.ModuleData != null) {
				_Add(this, instance.Descriptor.TypeId, instance.InstanceId, instance.ModuleData);
			}
		}

		public void RemoveModuleTypeData(Guid moduleTypeId) {
			_Remove(this, new Tuple<Guid, Guid>(moduleTypeId, moduleTypeId));
		}

		public void RemoveModuleInstanceData(Guid moduleTypeId, Guid moduleInstanceId) {
			_Remove(this, new Tuple<Guid, Guid>(moduleTypeId, moduleInstanceId));
		}

		public bool Contains(Guid moduleTypeId) {
			return Contains(moduleTypeId, moduleTypeId);
		}

		public bool Contains(Guid moduleTypeId, Guid moduleInstanceId) {
			return _dataModels.ContainsKey(new Tuple<Guid,Guid>(moduleTypeId, moduleInstanceId));
		}

		public IEnumerable<Guid> GetModuleTypes() {
			return _dataModels.Keys.Select(x => x.Item1);
		}

		public IEnumerable<T> GetInstances<T>()
			where T : class, IModuleInstance {
			T instance;
			foreach(Tuple<Guid, Guid> typeInstance in _dataModels.Keys) {
				if(Modules.IsOfType(typeInstance.Item1, typeof(T))) {
					IModuleManagement moduleManager = Modules.GetManager<T>();
					instance = moduleManager.Get(typeInstance.Item1) as T;
					instance.InstanceId = typeInstance.Item2;
					GetModuleInstanceData(instance);
					yield return instance;
				}
			}
		}

		public IEnumerable<IModuleDataModel> GetData() {
			return _dataModels.Values;
		}

		public void Clear() {
			_dataModels.Clear();
		}

		public IModuleDataSet Clone() {
			//ModuleDataSet newDataSet = new ModuleDataSet();
			ModuleDataSet newDataSet = Activator.CreateInstance(GetType()) as ModuleDataSet;
			_Clone(this, newDataSet);
			return newDataSet;
		}

		public void Clone(IModuleDataSet sourceDataSet) {
			Clear();
			_Clone(sourceDataSet as ModuleDataSet, this);
		}

		private void _Clone(ModuleDataSet source, ModuleDataSet destination) {
			if(source == null) throw new ArgumentNullException("source");
			if(destination == null) throw new ArgumentNullException("destination");

			// Clone exactly, assuming unchanged type and instance ids for the
			// modules the data belongs to.
			IModuleDataModel newModel;
			foreach(IModuleDataModel dataModel in source._dataModels.Values) {
				newModel = dataModel.Clone();
				newModel.ModuleTypeId = dataModel.ModuleTypeId;
				newModel.ModuleInstanceId = dataModel.ModuleInstanceId;
				_Add(destination, newModel.ModuleTypeId, newModel.ModuleInstanceId, newModel);
			}
		}

		/// <summary>
		/// Clones an IModuleDataModel.
		/// This instance is created as orphaned, expecting to be added to an IModuleDataSet.
		/// </summary>
		/// <param name="dataModel"></param>
		/// <returns></returns>
		public IModuleDataModel CloneTypeData(IModuleInstance sourceModule) {
			if(_dataModels.ContainsKey(new Tuple<Guid, Guid>(sourceModule.Descriptor.TypeId, sourceModule.Descriptor.TypeId))) {
				IModuleDataModel newInstance = sourceModule.ModuleData.Clone();
				newInstance.ModuleTypeId = sourceModule.Descriptor.TypeId;
				newInstance.ModuleInstanceId = sourceModule.Descriptor.TypeId;
				return newInstance;
			}
			return null;
		}

		/// <summary>
		/// Clones an IModuleDataModel.
		/// This instance is created as orphaned, expecting to be added to an IModuleDataSet.
		/// </summary>
		/// <param name="dataModel"></param>
		/// <returns></returns>
		public IModuleDataModel CloneInstanceData(IModuleInstance sourceModule, IModuleInstance destinationModule) {
			if(_dataModels.ContainsKey(new Tuple<Guid, Guid>(sourceModule.Descriptor.TypeId, sourceModule.InstanceId))) {
				IModuleDataModel newInstance = sourceModule.ModuleData.Clone();
				newInstance.ModuleTypeId = sourceModule.Descriptor.TypeId;
				newInstance.ModuleInstanceId = destinationModule.InstanceId;
				return newInstance;
			}
			return null;
		}
		
		/// <summary>
		/// Used only for XML serialization.
		/// </summary>
		[DataMember]
		public string SerializationGraph {
			get {
				return Serialize();
			}
			set {
				_dataModels = new Dictionary<Tuple<Guid, Guid>, IModuleDataModel>();
				Deserialize(value); 
			}
		}

		abstract protected Type _GetDataSetType(IModuleDescriptor descriptor);
		abstract protected IModuleDataModel _GetDataInstance(IModuleInstance module);

		private bool _IsInstanceData(Guid typeId, Guid instanceId) {
			return typeId == instanceId;
		}
	}
}
