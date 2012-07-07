using Vixen.IO.Xml.Template;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlChannelNodeTemplateSerializer : IVersionedFileSerializer {
		private XmlSerializerBehaviorTemplate<ChannelNodeTemplate> _serializerBehaviorTemplate;
		private XmlChannelNodeTemplateSerializerContractFulfillment _serializerContractFulfillment;

		public XmlChannelNodeTemplateSerializer() {
			_serializerBehaviorTemplate = new XmlSerializerBehaviorTemplate<ChannelNodeTemplate>();
			_serializerContractFulfillment = new XmlChannelNodeTemplateSerializerContractFulfillment();
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
			_serializerBehaviorTemplate.Write((ChannelNodeTemplate)value, ref filePath, _serializerContractFulfillment);
		}
	}
}
