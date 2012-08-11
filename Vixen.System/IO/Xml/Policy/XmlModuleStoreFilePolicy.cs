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
			XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
			_moduleStore.InstanceData = dataSetSerializer.ReadObject(_content);
		}

		protected override void ReadModuleInstanceDataSet() {
		}
	}
}
