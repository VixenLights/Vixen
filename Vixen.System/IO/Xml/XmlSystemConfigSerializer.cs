using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSystemConfigSerializer : FileSerializer<SystemConfig> {
		//private const string ATTR_VERSION = "version";

		override protected SystemConfig _Read(string filePath) {
			SystemConfig systemConfig = new SystemConfig();
			XElement content = _LoadFile(filePath);
			IFilePolicy filePolicy = new XmlSystemConfigFilePolicy(systemConfig, content);
			filePolicy.Read();

			systemConfig.LoadedFilePath = filePath;

			return systemConfig;
		}

		override protected void _Write(SystemConfig value, string filePath) {
			XmlVersionedContent content = new XmlVersionedContent("SystemConfig");
			IFilePolicy filePolicy = new XmlSystemConfigFilePolicy(value, content);
			content.Version = filePolicy.GetVersion();
			filePolicy.Write();
			content.Save(filePath);

			//XElement content = new XElement("SystemConfig");
			//XmlSystemConfigFilePolicy filePolicy = new XmlSystemConfigFilePolicy(value, content);
			//XmlFileVersion fileVersioner = new XmlFileVersion();
			//fileVersioner.PutVersion(content, ATTR_VERSION, filePolicy.GetVersion());
			//filePolicy.Write();
			//content.Save(filePath);

			value.LoadedFilePath = filePath;
		}

		private XElement _LoadFile(string filePath) {
			XmlFileLoader fileLoader = new XmlFileLoader();
			XElement content = Helper.Load(filePath, fileLoader);
			content = _EnsureContentIsUpToDate(content, filePath);
			return content;
		}

		private XElement _EnsureContentIsUpToDate(XElement content, string originalFilePath) {
			IMigrator sequenceMigrator = new XmlSystemConfigMigrator(content);
			IFilePolicy filePolicy = new XmlSystemConfigFilePolicy();
			XmlFileSerializationHelper serializationHelper = new XmlFileSerializationHelper();
			_AddResults(serializationHelper.EnsureContentIsUpToDate(content, originalFilePath, filePolicy, sequenceMigrator));

			//XmlFileVersion fileVersioner = new XmlFileVersion();
			//int fileVersion = fileVersioner.GetVersion(content, ATTR_VERSION);

			//XmlSystemConfigFilePolicy filePolicy = new XmlSystemConfigFilePolicy();
			//IMigrator migrator = new XmlSystemConfigMigrator(content);
			//GeneralMigrationPolicy migrationPolicy = new GeneralMigrationPolicy(filePolicy, migrator);
			//migrationPolicy.MatureContent(fileVersion, originalFilePath);
			
			//_AddResults(migrationPolicy.MigrationResults);

			return content;
		}

		//private int _GetVersion(XElement content) {
		//    XAttribute versionAttribute = content.Attribute(ATTR_VERSION);
		//    if(versionAttribute != null) {
		//        int version;
		//        if(int.TryParse(versionAttribute.Value, out version)) {
		//            return version;
		//        }
		//        throw new SerializationException("File version could not be determined.");
		//    }
		//    throw new SerializationException("File does not have a version.");
		//}
	}
}
