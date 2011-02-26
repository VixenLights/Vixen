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
    public class ModuleDataSet : IModuleDataSet {
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
			_GetModuleData(module, new Tuple<Guid, Guid>(module.TypeId, module.TypeId));
		}

		/// <summary>
		/// Instantiates the data model for the module from the data in this data set.
		/// Accounts for the module's instance id so there can be multiple instances
		/// of a module type in the dataset.
		/// </summary>
		/// <param name="module"></param>
		public void GetModuleInstanceData(IModuleInstance module) {
			_GetModuleData(module, new Tuple<Guid,Guid>(module.TypeId,module.InstanceId));
		}

		private void _GetModuleData(IModuleInstance module, Tuple<Guid,Guid> key) {
			IModuleDataModel dataModel = null;

			// If there isn't any data, create it and add it to the dataset.
			if(!_dataModels.TryGetValue(key, out dataModel)) {
				// The module may not have defined a data model because it may not have a
				// need for data.
				Type moduleDataType = Modules.GetDescriptorById(module.TypeId).ModuleDataClass;
				if(moduleDataType != null) {
					dataModel = Activator.CreateInstance(moduleDataType) as IModuleDataModel;
					_SetPedigree(dataModel, module.TypeId, key.Item2);
					module.ModuleData = dataModel;
					_Add(this, key, module);
				} else {
					module.ModuleData = null;
				}
			} else {
				module.ModuleData = dataModel;
			}
		}

		private void _SetPedigree(IModuleDataModel dataModel, Guid owningModuleTypeId, Guid moduleInstanceId) {
			// Where the data came from...
			dataModel.ModuleDataSet = this;
			dataModel.ModuleInstanceId = moduleInstanceId;
			// Type of module it belongs to...
			dataModel.ModuleTypeId = owningModuleTypeId;
		}
		public void Add(IModuleDataModel data) {
			_Add(this, new Tuple<Guid,Guid>(data.ModuleTypeId, data.ModuleInstanceId), data);
		}

        static private void _Add(ModuleDataSet dataSet, Tuple<Guid,Guid> key, IModuleInstance module) {
			_Add(dataSet, key, module.ModuleData);
		}

		static private void _Add(ModuleDataSet dataSet, Tuple<Guid,Guid> key, IModuleDataModel moduleData) {
			if(dataSet == null) throw new ArgumentException("Cannot be null.", "dataSet");
			if(key == null) throw new ArgumentException("Cannot be empty.", "key");
			if(moduleData == null) throw new ArgumentException("Cannot be null.", "moduleData");

			if(!dataSet._dataModels.ContainsKey(key)) {
				dataSet._dataModels[key] = moduleData;
				moduleData.ModuleDataSet = dataSet;
			}
		}

		static private void _Remove(ModuleDataSet dataSet, Tuple<Guid,Guid> key) {
			if(dataSet == null) throw new ArgumentException("Cannot be null.", "dataSet");
			if(key == null) throw new ArgumentException("Cannot be null.", "module");

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

        public void SaveToParent(XElement parentNode) {
            parentNode.RemoveNodes();
            string moduleData = Serialize();
            if(!string.IsNullOrEmpty(moduleData)) {
                parentNode.Add(XElement.Parse(moduleData));
            }
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
                        serializer = new DataContractSerializer(descriptor.ModuleDataClass);
                        dataModel = null;
                        using(MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(moduleElement.FirstNode.ToString()))) {
							dataModel = serializer.ReadObject(stream) as IModuleDataModel;
                        }
						
						_SetPedigree(dataModel, moduleTypeId, moduleInstanceId);

						_Add(this, new Tuple<Guid, Guid>(moduleTypeId, moduleInstanceId), dataModel);
                    }
                }
            }
        }

		public void Remove(Guid moduleTypeId) {
			Remove(moduleTypeId, moduleTypeId);
		}

		public void Remove(Guid moduleTypeId, Guid moduleInstanceId) {
			_dataModels.Remove(new Tuple<Guid,Guid>(moduleTypeId, moduleInstanceId));
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

		public IEnumerable<Tuple<Guid, Guid>> GetInstances() {
			return _dataModels.Keys;
		}

		public IEnumerable<IModuleDataModel> GetData() {
			return _dataModels.Values;
		}

		public void Clear() {
			_dataModels.Clear();
		}

		public IModuleDataSet Clone() {
			// Clone exactly, assuming unchanged type and instance ids for the
			// modules the data belongs to.
			ModuleDataSet newDataSet = new ModuleDataSet();
			IModuleDataModel newModel;
			foreach(IModuleDataModel dataModel in _dataModels.Values) {
				newModel = dataModel.Clone();
				newModel.ModuleTypeId = dataModel.ModuleTypeId;
				newModel.ModuleInstanceId = dataModel.ModuleInstanceId;
				newDataSet.Add(newModel);
			}
			return newDataSet;
		}

		/// <summary>
		/// Clones an IModuleDataModel.
		/// This instance is created as orphaned, expecting to be added to an IModuleDataSet.
		/// </summary>
		/// <param name="dataModel"></param>
		/// <returns></returns>
		public IModuleDataModel CloneTypeData(IModuleInstance sourceModule) {
			if(_dataModels.ContainsKey(new Tuple<Guid, Guid>(sourceModule.TypeId, sourceModule.TypeId))) {
				IModuleDataModel newInstance = sourceModule.ModuleData.Clone();
				newInstance.ModuleTypeId = sourceModule.TypeId;
				newInstance.ModuleInstanceId = sourceModule.TypeId;
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
			if(_dataModels.ContainsKey(new Tuple<Guid, Guid>(sourceModule.TypeId, sourceModule.InstanceId))) {
				IModuleDataModel newInstance = sourceModule.ModuleData.Clone();
				newInstance.ModuleTypeId = sourceModule.TypeId;
				newInstance.ModuleInstanceId = destinationModule.InstanceId;
				return newInstance;
			}
			return null;
		}
		
		static public void CreateEmptyDataSetFile(string fileName) {
			ModuleDataSet dataSet = new ModuleDataSet();
			XElement element = XElement.Parse(dataSet.Serialize());
			element.Save(fileName);
		}

		/// <summary>
		/// Used only for XML serialization.
		/// </summary>
		[DataMember]
		public string SerializationGraph {
			get {
				string s = Serialize();
				return s;
			}
			set {
				_dataModels = new Dictionary<Tuple<Guid, Guid>, IModuleDataModel>();
				Deserialize(value); 
			}
		}
	}
}
