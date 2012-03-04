using System;
using System.IO;
using System.Xml.Linq;
using Vixen.Module.Sequence;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSequenceSerializer : FileSerializer<Sequence> {
		//private const string ATTR_VERSION = "version";

		protected override Sequence _Read(string filePath) {
			if(!Path.IsPathRooted(filePath)) {
				filePath = Path.Combine(Sequence.DefaultDirectory, filePath);
			}

			Sequence sequence = _CreateSequenceFor(filePath);
			XElement content = _LoadFile(filePath);
			XmlSequenceFilePolicy filePolicy = new XmlSequenceFilePolicy(sequence, content);
			filePolicy.Read();

			sequence.FilePath = filePath;

			return sequence;
		}

		protected override void _Write(Sequence value, string filePath) {
			XmlVersionedContent content = new XmlVersionedContent("Sequence");
			IFilePolicy filePolicy = new XmlSequenceFilePolicy(value, content);
			content.Version = filePolicy.GetVersion();
			filePolicy.Write();

			//XElement content = new XElement("Sequence");
			//XmlFileVersion fileVersioner = new XmlFileVersion();
			//XmlSequenceFilePolicy filePolicy = new XmlSequenceFilePolicy(value, content);
			//fileVersioner.PutVersion(content, ATTR_VERSION, filePolicy.GetVersion());
			//filePolicy.Write();

			filePath = Path.Combine(Sequence.DefaultDirectory, Path.GetFileName(filePath));
			content.Save(filePath);

			value.FilePath = filePath;
		}

		private XElement _LoadFile(string filePath) {
			XmlFileLoader fileLoader = new XmlFileLoader();
			XElement content = Helper.Load(filePath, fileLoader);
			content = _EnsureContentIsUpToDate(content, filePath);
			return content;
		}

		private XElement _EnsureContentIsUpToDate(XElement content, string originalFilePath) {
			IMigrator sequenceMigrator = new XmlSequenceMigrator(content);
			IFilePolicy filePolicy = new XmlSequenceFilePolicy();
			XmlFileSerializationHelper serializationHelper = new XmlFileSerializationHelper();
			_AddResults(serializationHelper.EnsureContentIsUpToDate(content, originalFilePath, filePolicy, sequenceMigrator));

			//XmlFileVersion fileVersioner = new XmlFileVersion();
			//int fileVersion = fileVersioner.GetVersion(content, ATTR_VERSION);
			//XmlSequenceFilePolicy filePolicy = new XmlSequenceFilePolicy();
			//IMigrator sequenceMigrator = new XmlSequenceMigrator(content);
			//GeneralMigrationPolicy<XElement> migrationPolicy = new GeneralMigrationPolicy<XElement>(filePolicy, sequenceMigrator);
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

		private Sequence _CreateSequenceFor(string filePath) {
			// Get the specific sequence module manager.
			SequenceModuleManagement manager = Modules.GetManager<ISequenceModuleInstance, SequenceModuleManagement>();

			// Get an instance of the appropriate sequence module.
			Sequence sequence = (Sequence)manager.Get(filePath);
			if(sequence == null) throw new InvalidOperationException("No sequence type defined for file " + filePath);

			return sequence;
		}
	}
}
