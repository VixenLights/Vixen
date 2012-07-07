using Vixen.IO.Xml.Template;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlOutputFilterTemplateSerializer : IVersionedFileSerializer {
		private XmlSerializerBehaviorTemplate<OutputFilterTemplate> _serializerBehaviorTemplate;
		private XmlOutputFilterTemplateSerializerContractFulfillment _serializerContractFulfillment;

		public XmlOutputFilterTemplateSerializer() {
			_serializerBehaviorTemplate = new XmlSerializerBehaviorTemplate<OutputFilterTemplate>();
			_serializerContractFulfillment = new XmlOutputFilterTemplateSerializerContractFulfillment();
		}

		public int FileVersion {
			get { return _serializerBehaviorTemplate.FileVersion; }
			//set { _serializerBehaviorTemplate.FileVersion = value; }
		}

		public int ClassVersion {
			get { return _serializerContractFulfillment.GetEmptyFilePolicy().Version; }
		}

		public object Read(string filePath) {
			OutputFilterTemplate template = _serializerBehaviorTemplate.Read(ref filePath, _serializerContractFulfillment);
			if(template != null) {
				template.FilePath = filePath;
			}
			return template;
		}

		public void Write(object value, string filePath) {
			OutputFilterTemplate outputFilterTemplate = (OutputFilterTemplate)value;
			_serializerBehaviorTemplate.Write(outputFilterTemplate, ref filePath, _serializerContractFulfillment);
			outputFilterTemplate.FilePath = filePath;
		}
	}
}
