using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSystemContextSerializer : FileSerializer<SystemContext> {
		//private const string ATTR_VERSION = "version";

		protected override SystemContext _Read(string filePath) {
			SystemContext systemContext = new SystemContext();
			XElement content = _LoadFile(filePath);
			XmlSystemContextFilePolicy filePolicy = new XmlSystemContextFilePolicy(systemContext, content);
			filePolicy.Read();

			return systemContext;
		}

		protected override void _Write(SystemContext value, string filePath) {
			XmlVersionedContent content = new XmlVersionedContent("SystemContext");
			IFilePolicy filePolicy = new XmlSystemContextFilePolicy(value, content);
			content.Version = filePolicy.GetVersion();
			filePolicy.Write();
			//XElement content = new XElement("SystemContext");
			//XmlSystemContextFilePolicy filePolicy = new XmlSystemContextFilePolicy(value, content);
			//filePolicy.Write();
			content.Save(filePath);
		}

		private XElement _LoadFile(string filePath) {
			XmlFileLoader fileLoader = new XmlFileLoader();
			XElement content = Helper.Load(filePath, fileLoader);
			content = _EnsureContentIsUpToDate(content, filePath);
			return content;
		}

		private XElement _EnsureContentIsUpToDate(XElement content, string originalFilePath) {
			IMigrator sequenceMigrator = new XmlSystemContextMigrator(content);
			IFilePolicy filePolicy = new XmlSystemContextFilePolicy();
			XmlFileSerializationHelper serializationHelper = new XmlFileSerializationHelper();
			_AddResults(serializationHelper.EnsureContentIsUpToDate(content, originalFilePath, filePolicy, sequenceMigrator));

			//int fileVersion = _GetVersion(content);

			//XmlSystemContextFilePolicy filePolicy = new XmlSystemContextFilePolicy();
			//IMigrator migrator = new XmlSystemContextMigrator(content);
			//GeneralMigrationPolicy<XElement> migrationPolicy = new GeneralMigrationPolicy<XElement>(filePolicy, migrator);
			//content = migrationPolicy.MatureContent(fileVersion, content, originalFilePath);

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
