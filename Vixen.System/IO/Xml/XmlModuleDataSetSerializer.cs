using System.Xml.Linq;
using Vixen.Module;

namespace Vixen.IO.Xml {
	class XmlModuleDataSetSerializer<DataSet> : IXmlSerializer<DataSet>
		where DataSet : class, IModuleDataSet, new() {
		private const string ELEMENT_MODULE_DATA = "ModuleData";

		public XElement WriteObject(DataSet value) {
			XElement containerElement = new XElement(ELEMENT_MODULE_DATA);
			XmlModuleDataModelCollectionSerializer dataModelSerializer = new XmlModuleDataModelCollectionSerializer(containerElement);
			value.Serialize(dataModelSerializer);
			return containerElement;
		}

		public DataSet ReadObject(XElement element) {
			XElement containerElement = element.Element(ELEMENT_MODULE_DATA);
			if(containerElement == null) return null;
			DataSet dataSet = new DataSet();
			XmlModuleDataModelCollectionSerializer dataModelSerializer = new XmlModuleDataModelCollectionSerializer(containerElement);
			dataSet.Deserialize(dataModelSerializer);
			return dataSet;
		}
	}
}
