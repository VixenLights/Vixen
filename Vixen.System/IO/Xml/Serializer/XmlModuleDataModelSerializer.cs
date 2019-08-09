using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using Vixen.Module;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlModuleDataModelSerializer : IXmlSerializer<IModuleDataModel>
	{
		private static NLog.Logger logging = NLog.LogManager.GetCurrentClassLogger();

		private const string ELEMENT_MODULE = "Module";
		private const string ATTR_DATA_MODEL_TYPE = "dataModelType";
		private const string ATTR_MODULE_TYPE = "moduleType";
		private const string ATTR_MODULE_INSTANCE = "moduleInstance";
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public XElement WriteObject(IModuleDataModel value)
		{
			return _SerializeDataModel(value);
		}

		private XElement _SerializeDataModel(IModuleDataModel value)
		{
			using (MemoryStream stream = new MemoryStream()) {
				// Serializing each data object as the data object's type.
				DataContractSerializer serializer = new DataContractSerializer(value.GetType());
				try {
					serializer.WriteObject(stream, value);
					string objectData = Encoding.ASCII.GetString(stream.ToArray()).Trim();
					return new XElement(ELEMENT_MODULE,
					                    new XAttribute(ATTR_DATA_MODEL_TYPE, _GetDataModelTypeString(value)),
					                    new XAttribute(ATTR_MODULE_TYPE, value.ModuleTypeId),
					                    new XAttribute(ATTR_MODULE_INSTANCE, value.ModuleInstanceId),
					                    XElement.Parse(objectData));
				}
				catch (Exception ex) {
					Logging.Error(ex, $"Error when serializing data model of type {value.GetType().Name}");
					return null;
				}
			}
		}

		private string _GetDataModelTypeString(IModuleDataModel dataModel)
		{
			//return dataModel.GetType().AssemblyQualifiedName;
			Type dataModelType = dataModel.GetType();
			return dataModelType.FullName + ", " + dataModelType.Assembly.GetName().Name;
		}

		public IModuleDataModel ReadObject(XElement element)
		{
			try {
				string dataModelTypeString = XmlHelper.GetAttribute(element, ATTR_DATA_MODEL_TYPE);
				if (dataModelTypeString == null)
					return null;

				Type dataModelType = Type.GetType(dataModelTypeString);
				if (dataModelType == null)
					return null;

				Guid? moduleTypeId = XmlHelper.GetGuidAttribute(element, ATTR_MODULE_TYPE);
				if (moduleTypeId == null)
					return null;

				Guid? moduleInstanceId = XmlHelper.GetGuidAttribute(element, ATTR_MODULE_INSTANCE);
				if (moduleInstanceId == null)
					return null;

				// Get the descriptor for the type.
				IModuleDescriptor descriptor = Modules.GetDescriptorById(moduleTypeId.Value);
				if (descriptor == null) {
					Logging.Error("Could not get module data for module type " + moduleTypeId.Value +
					              " because the module type does not exist.");
					return null;
				}

				IModuleDataModel dataModel;

				try {
					dataModel = _DeserializeDataModel(dataModelType, element);
				}
				catch (Exception ex) {
					Logging.Error(ex, "The data for module \"" + descriptor.TypeName + "\" was not loaded due to errors.");
					return null;
				}

				if (dataModel != null) {
					dataModel.ModuleTypeId = moduleTypeId.Value;
					dataModel.ModuleInstanceId = moduleInstanceId.Value;
				}

				return dataModel;
			} catch (Exception e) {
				logging.Error(e, "Error loading Module Data Model from XML");
				return null;
			}
		}

		private IModuleDataModel _DeserializeDataModel(Type dataModelType, XElement element)
		{
			DataContractSerializer serializer = new DataContractSerializer(dataModelType);
			using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(element.FirstNode.ToString()))) {
				return serializer.ReadObject(stream) as IModuleDataModel;
			}
		}
	}
}