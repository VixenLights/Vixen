using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Vixen.Module.Sequence;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	//class XmlSequenceSerializer : IFileSerializer<Sequence, SequenceSerializationResult> {
	class XmlSequenceSerializer : FileSerializer<Sequence> {
		private const string ATTR_VERSION = "version";
		//private List<IFileOperationResult> _results;

		protected override Sequence _Read(string filePath) {
			Sequence sequence = _CreateSequenceFor(filePath);
			XElement content = _LoadFile(filePath);
			XmlSequenceFilePolicy filePolicy = new XmlSequenceFilePolicy(sequence, content);
			filePolicy.Read();

			return sequence;
		}

		protected override void _Write(Sequence value, string filePath) {
			XElement content = new XElement("Sequence");
			XmlSequenceFilePolicy filePolicy = new XmlSequenceFilePolicy(value, content);
			filePolicy.Write();
			content.Save(filePath);
		}

		//public XmlSequenceSerializer() {
		//    _results = new List<IFileOperationResult>();
		//}

		//public SequenceSerializationResult Read(string filePath) {
		//    SequenceSerializationResult serializationResult;

		//    _results.Clear();

		//    try {
		//        if(File.Exists(filePath)) {
		//            Sequence sequence = _CreateSequenceFor(filePath);
		//            XElement content = _LoadFile(filePath);
		//            XmlSequenceFilePolicy filePolicy = new XmlSequenceFilePolicy(sequence, content);
		//            filePolicy.Read();

		//            serializationResult = new SequenceSerializationResult(true, "File read successful.", sequence, _results);
		//        } else {
		//            serializationResult = new SequenceSerializationResult(false, "File does not exist.", null, _results);
		//        }
		//    } catch(Exception ex) {
		//        VixenSystem.Logging.Debug(ex);
		//        serializationResult = new SequenceSerializationResult(false, ex.Message, null, _results);
		//    }

		//    return serializationResult;
		//}

		//public SequenceSerializationResult Write(Sequence value, string filePath) {
		//    SequenceSerializationResult serializationResult;

		//    _results.Clear();

		//    try {
		//        XElement content = new XElement("Sequence");
		//        XmlSequenceFilePolicy filePolicy = new XmlSequenceFilePolicy(value, content);
		//        filePolicy.Write();

		//        content.Save(filePath);

		//        serializationResult = new SequenceSerializationResult(true, "File write successful.", value, _results);
		//    } catch(Exception ex) {
		//        VixenSystem.Logging.Debug(ex);
		//        serializationResult = new SequenceSerializationResult(false, ex.Message, null, _results);
		//    }

		//    return serializationResult;
		//}

		private XElement _LoadFile(string filePath) {
			XmlFileLoader fileLoader = new XmlFileLoader();
			XElement content = Helper.Load(filePath, fileLoader);
			content = _EnsureContentIsUpToDate(content, filePath);
			return content;
		}

		private XElement _EnsureContentIsUpToDate(XElement content, string originalFilePath) {
			int fileVersion = _GetVersion(content);
			XmlSequenceFilePolicy filePolicy = new XmlSequenceFilePolicy();
			IMigrator sequenceMigrator = new XmlSequenceMigrator(content);
			GeneralMigrationPolicy<XElement> migrationPolicy = new GeneralMigrationPolicy<XElement>(filePolicy, sequenceMigrator);
			content = migrationPolicy.MatureContent(fileVersion, content, originalFilePath);
			_AddResults(migrationPolicy.MigrationResults);

			//XmlSequenceFilePolicy filePolicy = new XmlSequenceFilePolicy();
			//int policyVersion = filePolicy.GetVersion();
			//IMigrator sequenceMigrator = new XmlSequenceMigrator(content);
			//MigrationDriver migrationDriver = new MigrationDriver(fileVersion, policyVersion, sequenceMigrator);
			//if(migrationDriver.MigrationNeeded) {
			//    if(migrationDriver.MigrationPathAvailable) {
			//        _BackupFile(originalFilePath, fileVersion);
			//        _AddResults(migrationDriver.Migrate());
			//    } else {
			//        throw new SerializationException("The file requires a migration, but a proper migration path is not available.");
			//    }
			//}

			return content;
		}

		//private void _AddResults(IEnumerable<IFileOperationResult> operationResults) {
		//    _results.AddRange(operationResults);
		//}

		//private void _BackupFile(string originalFilePath, int fileVersion) {
		//    File.Copy(originalFilePath, originalFilePath + "." + fileVersion);
		//}

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
