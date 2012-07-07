using Vixen.IO.Xml.Template;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSystemContextSerializer : IVersionedFileSerializer {
		private XmlSerializerBehaviorTemplate<SystemContext> _serializerBehaviorTemplate;
		private XmlSystemContextSerializerContractFulfillment _serializerContractFulfillment;

		public XmlSystemContextSerializer() {
			_serializerBehaviorTemplate = new XmlSerializerBehaviorTemplate<SystemContext>();
			_serializerContractFulfillment = new XmlSystemContextSerializerContractFulfillment();
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
			_serializerBehaviorTemplate.Write((SystemContext)value, ref filePath, _serializerContractFulfillment);
		}
	}
}
