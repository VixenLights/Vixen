using Vixen.IO.Xml.Template;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSystemConfigSerializer : IVersionedFileSerializer {
		private XmlSerializerBehaviorTemplate<SystemConfig> _serializerBehaviorTemplate;
		private XmlSystemConfigSerializerContractFulfillment _serializerContractFulfillment;

		public XmlSystemConfigSerializer() {
			_serializerBehaviorTemplate = new XmlSerializerBehaviorTemplate<SystemConfig>();
			_serializerContractFulfillment = new XmlSystemConfigSerializerContractFulfillment();
		}

		public int FileVersion {
			get { return _serializerBehaviorTemplate.FileVersion; }
			//set { _serializerBehaviorTemplate.FileVersion = value; }
		}

		public int ClassVersion {
			get { return _serializerContractFulfillment.GetEmptyFilePolicy().Version; }
		}

		public object Read(string filePath) {
			SystemConfig systemConfig = _serializerBehaviorTemplate.Read(ref filePath, _serializerContractFulfillment);
			if(systemConfig != null) {
				systemConfig.LoadedFilePath = filePath;
			}
			return systemConfig;
		}

		public void Write(object value, string filePath) {
			SystemConfig systemConfig = (SystemConfig)value;
			_serializerBehaviorTemplate.Write(systemConfig, ref filePath, _serializerContractFulfillment);
			systemConfig.LoadedFilePath = filePath;
		}
	}
}
