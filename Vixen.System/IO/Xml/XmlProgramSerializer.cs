using System.IO;
using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlProgramSerializer : FileSerializer<Program> {
		//private const string ATTR_VERSION = "version";

		protected override Program _Read(string filePath) {
			if(!Path.IsPathRooted(filePath)) filePath = Path.Combine(Program.Directory, filePath);
			filePath = Path.ChangeExtension(filePath, Program.Extension);

			Program program = new Program(Path.GetFileNameWithoutExtension(filePath));
			XElement content = _LoadFile(filePath);
			XmlProgramFilePolicy filePolicy = new XmlProgramFilePolicy(program, content);
			filePolicy.Read();

			return program;
		}

		protected override void _Write(Program value, string filePath) {
			XmlVersionedContent content = new XmlVersionedContent("Program");
			IFilePolicy filePolicy = new XmlProgramFilePolicy(value, content);
			content.Version = filePolicy.GetVersion();
			filePolicy.Write();

			//XElement content = new XElement("Program");
			//XmlProgramFilePolicy filePolicy = new XmlProgramFilePolicy(value, content);
			//XmlFileVersion fileVersioner = new XmlFileVersion();
			//fileVersioner.PutVersion(content, ATTR_VERSION, filePolicy.GetVersion());
			//filePolicy.Write();

			filePath = Path.Combine(Program.Directory, Path.GetFileName(filePath));
			filePath = Path.ChangeExtension(filePath, Program.Extension);
			content.Save(filePath);
		}

		private XElement _LoadFile(string filePath) {
			XmlFileLoader fileLoader = new XmlFileLoader();
			XElement content = Helper.Load(filePath, fileLoader);
			content = _EnsureContentIsUpToDate(content, filePath);
			return content;
		}

		private XElement _EnsureContentIsUpToDate(XElement content, string originalFilePath) {
			IMigrator sequenceMigrator = new XmlProgramMigrator(content);
			IFilePolicy filePolicy = new XmlProgramFilePolicy();
			XmlFileSerializationHelper serializationHelper = new XmlFileSerializationHelper();
			_AddResults(serializationHelper.EnsureContentIsUpToDate(content, originalFilePath, filePolicy, sequenceMigrator));

			//XmlFileVersion fileVersioner = new XmlFileVersion();
			//int fileVersion = fileVersioner.GetVersion(content, ATTR_VERSION);

			//XmlProgramFilePolicy filePolicy = new XmlProgramFilePolicy();
			//IMigrator migrator = new XmlProgramMigrator(content);
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
