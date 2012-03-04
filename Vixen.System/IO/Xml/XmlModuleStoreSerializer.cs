using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlModuleStoreSerializer : FileSerializer<ModuleStore> {
		//private const string ATTR_VERSION = "version";

		protected override ModuleStore _Read(string filePath) {
			ModuleStore moduleStore = new ModuleStore();
			XElement content = _LoadFile(filePath);
			XmlModuleStoreFilePolicy filePolicy = new XmlModuleStoreFilePolicy(moduleStore, content);
			filePolicy.Read();

			moduleStore.LoadedFilePath = filePath;

			return moduleStore;
		}

		protected override void _Write(ModuleStore value, string filePath) {
			XmlVersionedContent content = new XmlVersionedContent("ModuleStore");
			IFilePolicy filePolicy = new XmlModuleStoreFilePolicy(value, content);
			content.Version = filePolicy.GetVersion();
			filePolicy.Write();
			content.Save(filePath);

			//XElement content = new XElement("ModuleStore");
			//XmlModuleStoreFilePolicy filePolicy = new XmlModuleStoreFilePolicy(value, content);
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
			IMigrator sequenceMigrator = new XmlModuleStoreMigrator(content);
			IFilePolicy filePolicy = new XmlModuleStoreFilePolicy();
			XmlFileSerializationHelper serializationHelper = new XmlFileSerializationHelper();
			_AddResults(serializationHelper.EnsureContentIsUpToDate(content, originalFilePath, filePolicy, sequenceMigrator));

			//XmlFileVersion fileVersioner = new XmlFileVersion();
			//int fileVersion = fileVersioner.GetVersion(content, ATTR_VERSION);

			//XmlModuleStoreFilePolicy filePolicy = new XmlModuleStoreFilePolicy();
			//IMigrator migrator = new XmlModuleStoreMigrator(content);
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
