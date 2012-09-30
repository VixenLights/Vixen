using System.Linq;
using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.Sys;

namespace Vixen.IO.Xml.Policy {
	class XmlModuleStoreFilePolicy : ModuleStoreFilePolicy {
		private ModuleStore _moduleStore;
		private XElement _content;

		public XmlModuleStoreFilePolicy() {
		}

		public XmlModuleStoreFilePolicy(ModuleStore moduleStore, XElement content) {
			_moduleStore = moduleStore;
			_content = content;
		}

		protected override void WriteModuleTypeDataSet() {
			XmlModuleStaticDataSetSerializer dataSetSerializer = new XmlModuleStaticDataSetSerializer();
			_content.Add(dataSetSerializer.WriteObject(_moduleStore.TypeData));
		}

		protected override void WriteModuleInstanceDataSet() {
			XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
			_content.Add(dataSetSerializer.WriteObject(_moduleStore.InstanceData));
		}

		protected override void ReadModuleTypeDataSet() {
			XElement typeDataElement = _GetTypeData(_content);
			if(typeDataElement != null) {
				XmlModuleStaticDataSetSerializer dataSetSerializer = new XmlModuleStaticDataSetSerializer();
				_moduleStore.TypeData = dataSetSerializer.ReadObject(typeDataElement);
			}
		}

		protected override void ReadModuleInstanceDataSet() {
			XElement instanceDataElement = _GetInstanceData(_content);
			if(instanceDataElement != null) {
				XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
				_moduleStore.InstanceData = dataSetSerializer.ReadObject(instanceDataElement);
			}
		}

		private XElement _GetTypeData(XElement contentElement) {
			XElement dataSetElement = contentElement.Elements().FirstOrDefault();
			if(dataSetElement != null) {
				dataSetElement = new XElement("wrapper", dataSetElement);
			}
			return dataSetElement;
		}

		private XElement _GetInstanceData(XElement contentElement) {
			XElement dataSetElement = contentElement.Elements().Skip(1).FirstOrDefault();
			if(dataSetElement != null) {
				dataSetElement = new XElement("wrapper", dataSetElement);
			}
			return dataSetElement;
		}
	}
}
