using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlModuleStoreFilePolicy : ModuleStoreFilePolicy {
		private ModuleStore _moduleStore;
		private XElement _content;

		public XmlModuleStoreFilePolicy() {
		}

		public XmlModuleStoreFilePolicy(ModuleStore moduleStore, XElement content) {
			_moduleStore = moduleStore;
			_content = content;
		}

		protected override void WriteModuleDataSet() {
			XmlModuleStaticDataSetSerializer dataSetSerializer = new XmlModuleStaticDataSetSerializer();
			_content.Add(dataSetSerializer.WriteObject(_moduleStore.Data));
		}

		protected override void ReadModuleDataSet() {
			XmlModuleStaticDataSetSerializer dataSetSerializer = new XmlModuleStaticDataSetSerializer();
			_moduleStore.Data = dataSetSerializer.ReadObject(_content);
		}
	}
}
