using Vixen.IO.Xml.Template;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSystemConfigSerializer : FileSerializer<SystemConfig> {
		private XmlTemplatedSerializer<SystemConfig> _templatedSerializer;
		private XmlSystemConfigSerializerTemplate _serializerTemplate;

		public XmlSystemConfigSerializer() {
			_templatedSerializer = new XmlTemplatedSerializer<SystemConfig>();
			_serializerTemplate = new XmlSystemConfigSerializerTemplate();
		}

		override protected SystemConfig _Read(string filePath) {
			SystemConfig systemConfig = _templatedSerializer.Read(ref filePath, _serializerTemplate);
			if(systemConfig != null) {
				systemConfig.LoadedFilePath = filePath;
			}
			return systemConfig;
		}

		override protected void _Write(SystemConfig value, string filePath) {
			_templatedSerializer.Write(value, ref filePath, _serializerTemplate);
			value.LoadedFilePath = filePath;
		}
	}
}
