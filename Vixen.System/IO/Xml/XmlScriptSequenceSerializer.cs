using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Vixen.Module.Sequence;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	//class XmlScriptSequenceSerializer : IFileSerializer<ScriptSequence, ScriptSequenceSerializationResult> {
	class XmlScriptSequenceSerializer : FileSerializer<ScriptSequence> {
		private const string ATTR_VERSION = "version";
		//private List<IFileOperationResult> _results;

		protected override ScriptSequence _Read(string filePath) {
			ScriptSequence sequence = _CreateSequenceFor(filePath);
			XElement content = _LoadFile(filePath);
			XmlScriptSequenceFilePolicy filePolicy = new XmlScriptSequenceFilePolicy(sequence, content);
			filePolicy.Read();

			return sequence;
		}

		protected override void _Write(ScriptSequence value, string filePath) {
			XElement content = new XElement("Script");
			XmlScriptSequenceFilePolicy filePolicy = new XmlScriptSequenceFilePolicy(value, content);
			filePolicy.Write();
			content.Save(filePath);
		}

		//public XmlScriptSequenceSerializer() {
		//    _results = new List<IFileOperationResult>();
		//}

		//public ScriptSequenceSerializationResult Read(string filePath) {
		//    ScriptSequenceSerializationResult serializationResult;

		//    _results.Clear();

		//    try {
		//        if(File.Exists(filePath)) {
		//            ScriptSequence sequence = _CreateSequenceFor(filePath);
		//            XElement content = _LoadFile(filePath);
		//            XmlScriptSequenceFilePolicy filePolicy = new XmlScriptSequenceFilePolicy(sequence, content);
		//            filePolicy.Read();

		//            serializationResult = new ScriptSequenceSerializationResult(true, "File read successful.", sequence, _results);
		//        } else {
		//            serializationResult = new ScriptSequenceSerializationResult(false, "File does not exist.", null, _results);
		//        }
		//    } catch(Exception ex) {
		//        VixenSystem.Logging.Debug(ex);
		//        serializationResult = new ScriptSequenceSerializationResult(false, ex.Message, null, _results);
		//    }

		//    return serializationResult;
		//}

		//public ScriptSequenceSerializationResult Write(ScriptSequence value, string filePath) {
		//    ScriptSequenceSerializationResult serializationResult;

		//    _results.Clear();

		//    try {
		//        XElement content = new XElement("Script");
		//        XmlScriptSequenceFilePolicy filePolicy = new XmlScriptSequenceFilePolicy(value, content);
		//        filePolicy.Write();

		//        content.Save(filePath);

		//        serializationResult = new ScriptSequenceSerializationResult(true, "File write successful.", value, _results);
		//    } catch(Exception ex) {
		//        VixenSystem.Logging.Debug(ex);
		//        serializationResult = new ScriptSequenceSerializationResult(false, ex.Message, null, _results);
		//    }

		//    return serializationResult;
		//}

		private ScriptSequence _CreateSequenceFor(string filePath) {
			// Get the specific sequence module manager.
			SequenceModuleManagement manager = Modules.GetManager<ISequenceModuleInstance, SequenceModuleManagement>();

			// Get an instance of the appropriate sequence module.
			ScriptSequence sequence = (ScriptSequence)manager.Get(filePath);
			if(sequence == null) throw new InvalidOperationException("No sequence type defined for file " + filePath);

			return sequence;
		}

		private XElement _LoadFile(string filePath) {
			XmlFileLoader fileLoader = new XmlFileLoader();
			XElement content = Helper.Load(filePath, fileLoader);
			content = _EnsureContentIsUpToDate(content, filePath);
			return content;
		}

		private XElement _EnsureContentIsUpToDate(XElement content, string originalFilePath) {
			int fileVersion = _GetVersion(content);
			XmlScriptSequenceFilePolicy filePolicy = new XmlScriptSequenceFilePolicy();
			IMigrator sequenceMigrator = new XmlScriptSequenceMigrator(content);
			GeneralMigrationPolicy<XElement> migrationPolicy = new GeneralMigrationPolicy<XElement>(filePolicy, sequenceMigrator);
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

		//private void _AddResults(IEnumerable<IFileOperationResult> operationResults) {
		//    _results.AddRange(operationResults);
		//}
	}
}
