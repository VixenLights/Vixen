using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using Vixen.Module;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlModuleDataSetSerializer : IXmlSerializer<IModuleDataSet> {
		private const string ELEMENT_MODULE_DATA = "ModuleData";

		public XElement WriteObject(IModuleDataSet value) {
			string dataSetAsString = _SerializeDataSetToString();
			XElement element = XElement.Parse(dataSetAsString);
			return new XElement(ELEMENT_MODULE_DATA, element);
		}

		public IModuleDataSet ReadObject(XElement element) {
			string moduleDataString = element.Element(ELEMENT_MODULE_DATA).InnerXml();
			ModuleLocalDataSet dataSet = new ModuleLocalDataSet();
			_DeserializeDataSetFromString(dataSet, moduleDataString);
			return dataSet;
		}

		private string _SerializeDataSetToString() {
			XElement moduleDataSetElement = new XElement(ROOT_ELEMENT);
			DataContractSerializer serializer;

			// Module type id : module type data object
			foreach(KeyValuePair<Tuple<Guid, Guid>, IModuleDataModel> kvp in _dataModels) {
				using(MemoryStream stream = new MemoryStream()) {
					// Serializing each data object as the data object's type.
					serializer = new DataContractSerializer(kvp.Value.GetType());
					try {
						serializer.WriteObject(stream, kvp.Value);
						string objectData = Encoding.ASCII.GetString(stream.ToArray()).Trim();
						moduleDataSetElement.Add(
							new XElement(ELEMENT_MODULE,
										 new XAttribute(ATTR_MODULE_TYPE, kvp.Key.Item1),
										 new XAttribute(ATTR_MODULE_INSTANCE, kvp.Key.Item2),
										 XElement.Parse(objectData))
							);
					} catch(Exception ex) {
						VixenSystem.Logging.Error("Error when serializing data model of type " + kvp.Value.GetType().Name, ex);
					}
				}
			}

			return moduleDataSetElement.ToString();
		}

		private void _DeserializeDataSetFromString(IModuleDataSet dataSet, string xmlText) {
			//*** this requires exposing everything in the data set so something external can
			//    populate it; you avoid that by...passing something into the dataset that the
			//    dataset can read from and populate itself
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
								_SetPedigree(this, dataModel, moduleTypeId, moduleInstanceId);
								_Add(this, moduleTypeId, moduleInstanceId, dataModel);
							}
						}
					}
				}
			}
		}

		private bool _IsInstanceData(Guid typeId, Guid instanceId) {
			return typeId == instanceId;
		}

	}
}
