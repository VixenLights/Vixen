using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using Vixen.Module;
using Vixen.Sys;
using System.IO;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlModuleDataModelSerializer : IXmlSerializer<IModuleDataModel>
	{
		private static NLog.Logger logging = NLog.LogManager.GetCurrentClassLogger();

		private const string ELEMENT_MODULE = "Module";
		private const string ATTR_DATA_MODEL_TYPE = "dataModelType";
		private const string ATTR_MODULE_TYPE = "moduleType";
		private const string ATTR_MODULE_INSTANCE = "moduleInstance";
		private const string ATTR_DATA_MODEL_ASSEMBLY_NAME = "assemblyName";
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
					                    new XAttribute(ATTR_DATA_MODEL_ASSEMBLY_NAME, _GetDataModelTypeAssemblyString(value)),
										XElement.Parse(objectData));
				}
				catch (Exception ex) {
					Logging.Error(ex, $"Error when serializing data model of type {value.GetType().Name}");
					return null;
				}
			}
		}

		private string _GetDataModelTypeAssemblyString(IModuleDataModel dataModel)
		{
			Type dataModelType = dataModel.GetType();
			return dataModelType.Assembly.GetName().Name;
		}

		private string _GetDataModelTypeString(IModuleDataModel dataModel)
		{
			//This logic was modified on the conversion to .NET 6 to retain the old naming convention to maintain some degree of backwards compatibility. 
			Type dataModelType = dataModel.GetType();
			var assemblyName = dataModelType.Assembly.GetName().Name;
			if (!string.IsNullOrEmpty(assemblyName))
			{
				var assemblyShortName = assemblyName.Substring(assemblyName.LastIndexOf(".", StringComparison.Ordinal) + 1);
				return dataModelType.FullName + ", " + assemblyShortName;
			}

			//Should never get here unless something weird happened. 
			return dataModelType.FullName + ", " + dataModelType.Assembly.GetName().Name;
		}

		public IModuleDataModel ReadObject(XElement element)
		{
			try
			{
				string dataModelTypeString = null;
				string dataModelTypeXmlString = XmlHelper.GetAttribute(element, ATTR_DATA_MODEL_TYPE);
				string dataModelAssemblyString = XmlHelper.GetAttribute(element, ATTR_DATA_MODEL_ASSEMBLY_NAME);
				if (dataModelAssemblyString != null)
				{
					var temp = dataModelTypeXmlString.Split(",");
					if (temp.Length > 0)
					{
						dataModelTypeString = $"{temp[0].Trim()}, {dataModelAssemblyString}";
					}
					else
					{
						return null;
					}
				}
				else
				{
					//We are reading a config from and older version of Vixen and need to adjust the type assembly name.
					var temp = dataModelTypeXmlString.Split(",");
					if (temp.Length == 2)
					{


						var modelType = temp[0].Trim();
						if (modelType.StartsWith("VixenModules"))
						{
							var nmSpaces = modelType.Split('.');
							if (nmSpaces.Length > 1)
							{
								var moduleType = nmSpaces[1].Trim();
								if ("Output".Equals(moduleType))
								{
									//map Output to Controller due to a prior conversion
									moduleType = "Controller";
								}
								dataModelTypeString = $"{modelType}, Module.{moduleType}.{temp[1].Trim()}";
							}
						}

					}

					if (dataModelTypeString == null)
					{
						Logging.Warn($"Could not convert type for {dataModelTypeXmlString}");
						return null;
					}
				}

				Type dataModelType = Type.GetType(dataModelTypeString);
				if (dataModelType == null)
				{
					// VIX-3580 - A typo was introduced when the code base was converted from .NET Framework to .NET Core.
					// This barnacle detects and corrects the typo so that Intelligent Fixture Property type is found.		
					// The typo was introduced between releases 3.9 and 3.10.			
					if (dataModelTypeString == "VixenModules.Property.IntelligentFixture.IntelligentFixtureData, Module.Property.IntellegentFixture")
					{
						dataModelTypeString = "VixenModules.Property.IntelligentFixture.IntelligentFixtureData, Module.Property.IntelligentFixture";
						dataModelType = Type.GetType(dataModelTypeString);
					}
					
					if (dataModelType == null)
					{
						Logging.Warn($"Could not find type for {dataModelTypeString}");
						return null;
					}
				}

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