using System.Runtime.Serialization;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSystemConfigSerializer : FileSerializer<SystemConfig> {
		private const string ATTR_VERSION = "version";

		override protected SystemConfig _Read(string filePath) {
			SystemConfig systemConfig = new SystemConfig();
			XElement content = _LoadFile(filePath);
			XmlSystemConfigFilePolicy filePolicy = new XmlSystemConfigFilePolicy(systemConfig, content);
			filePolicy.Read();

			return systemConfig;
		}

		override protected void _Write(SystemConfig value, string filePath) {
			XElement content = new XElement("SystemConfig");
			XmlSystemConfigFilePolicy filePolicy = new XmlSystemConfigFilePolicy(value, content);
			filePolicy.Write();
			content.Save(filePath);
		}

		private XElement _LoadFile(string filePath) {
			XmlFileLoader fileLoader = new XmlFileLoader();
			XElement content = Helper.Load(filePath, fileLoader);
			content = _EnsureContentIsUpToDate(content, filePath);
			return content;
		}

		private XElement _EnsureContentIsUpToDate(XElement content, string originalFilePath) {
			int fileVersion = _GetVersion(content);

			XmlSystemConfigFilePolicy filePolicy = new XmlSystemConfigFilePolicy();
			IMigrator migrator = new XmlSystemConfigMigrator(content);
			GeneralMigrationPolicy<XElement> migrationPolicy = new GeneralMigrationPolicy<XElement>(filePolicy, migrator);
			content = migrationPolicy.MatureContent(fileVersion, content, originalFilePath);
			
			_AddResults(migrationPolicy.MigrationResults);

			return content;
		}

		private int _GetVersion(XElement content) {
			XAttribute versionAttribute = content.Attribute(ATTR_VERSION);
			if(versionAttribute != null) {
				int version;
				if(int.TryParse(versionAttribute.Value, out version)) {
					return version;
				}
				throw new SerializationException("File version could not be determined.");
			}
			throw new SerializationException("File does not have a version.");
		}
	}
}
