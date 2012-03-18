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
			systemConfig.LoadedFilePath = filePath;
			return systemConfig;
			//SystemConfig systemConfig = new SystemConfig();
			//XElement content = _LoadFile(filePath);
			//IFilePolicy filePolicy = new XmlSystemConfigFilePolicy(systemConfig, content);
			//filePolicy.Read();

			//systemConfig.LoadedFilePath = filePath;

			//return systemConfig;
		}

		override protected void _Write(SystemConfig value, string filePath) {
			_templatedSerializer.Write(value, ref filePath, _serializerTemplate);
			value.LoadedFilePath = filePath;

			//XmlVersionedContent content = new XmlVersionedContent("SystemConfig");
			//IFilePolicy filePolicy = new XmlSystemConfigFilePolicy(value, content);
			//content.Version = filePolicy.GetVersion();
			//filePolicy.Write();
			//content.Save(filePath);

			//value.LoadedFilePath = filePath;
		}

		//private XElement _LoadFile(string filePath) {
		//    XmlFileLoader fileLoader = new XmlFileLoader();
		//    XElement content = Helper.Load(filePath, fileLoader);
		//    content = _EnsureContentIsUpToDate(content, filePath);
		//    return content;
		//}

		//private XElement _EnsureContentIsUpToDate(XElement content, string originalFilePath) {
		//    IMigrator sequenceMigrator = new XmlSystemConfigMigrator(content);
		//    IFilePolicy filePolicy = new XmlSystemConfigFilePolicy();
		//    XmlFileSerializationHelper serializationHelper = new XmlFileSerializationHelper();
		//    _AddResults(serializationHelper.EnsureContentIsUpToDate(content, originalFilePath, filePolicy, sequenceMigrator));

		//    return content;
		//}
	}
}
