using Vixen.IO.Xml.Template;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlOutputFilterTemplateSerializer : FileSerializer<OutputFilterTemplate> {
		private XmlTemplatedSerializer<OutputFilterTemplate> _templatedSerializer;
		private XmlOutputFilterTemplateSerializerTemplate _serializerTemplate;

		public XmlOutputFilterTemplateSerializer() {
			_templatedSerializer = new XmlTemplatedSerializer<OutputFilterTemplate>();
			_serializerTemplate = new XmlOutputFilterTemplateSerializerTemplate();
		}

		protected override OutputFilterTemplate _Read(string filePath) {
			OutputFilterTemplate template = _templatedSerializer.Read(ref filePath, _serializerTemplate);
			if(template != null) {
				template.FilePath = filePath;
			}
			return template;
		}

		protected override void _Write(OutputFilterTemplate value, string filePath) {
			_templatedSerializer.Write(value, ref filePath, _serializerTemplate);
			value.FilePath = filePath;
		}
	}
}
