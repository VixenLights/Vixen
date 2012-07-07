using Vixen.IO.Xml.Template;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlModuleStoreSerializer : IVersionedFileSerializer {
		private XmlSerializerBehaviorTemplate<ModuleStore> _serializerBehaviorTemplate;
		private XmlModuleStoreSerializerContractFulfillment _serializerContractFulfillment;

		public XmlModuleStoreSerializer() {
			_serializerBehaviorTemplate = new XmlSerializerBehaviorTemplate<ModuleStore>();
			_serializerContractFulfillment = new XmlModuleStoreSerializerContractFulfillment();
		}

		public int FileVersion {
			get { return _serializerBehaviorTemplate.FileVersion; }
			//set { _serializerBehaviorTemplate.FileVersion = value; }
		}

		public int ClassVersion {
			get { return _serializerContractFulfillment.GetEmptyFilePolicy().Version; }
		}

		public object Read(string filePath) {
			return _serializerBehaviorTemplate.Read(ref filePath, _serializerContractFulfillment);
		}

		public void Write(object value, string filePath) {
			_serializerBehaviorTemplate.Write((ModuleStore)value, ref filePath, _serializerContractFulfillment);
		}
	}
}
