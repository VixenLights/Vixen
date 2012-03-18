using System;
using System.IO;
using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.IO.Xml.Migrator;
using Vixen.IO.Xml.Policy;
using Vixen.Module.Sequence;
using Vixen.Sys;

namespace Vixen.IO.Xml.Template {
	class XmlScriptSequenceSerializerTemplate : IXmlStandardFileWriteTemplate<ScriptSequence>, IXmlStandardFileReadTemplate<ScriptSequence> {
		public XmlVersionedContent GetContentNode() {
			return new XmlVersionedContent("Script");
		}

		public IFilePolicy GetEmptyFilePolicy() {
			return new XmlScriptSequenceFilePolicy();
		}

		public IFilePolicy GetFilePolicy(ScriptSequence obj, XElement content) {
			return new XmlScriptSequenceFilePolicy(obj, content);
		}

		public string GetAbsoluteFilePath(string filePath) {
			if(!Path.IsPathRooted(filePath)) filePath = Path.Combine(ScriptSequence.Directory, filePath);
			return filePath;
		}


		public ScriptSequence CreateNewObjectFor(string filePath) {
			return _CreateSequenceFor(filePath);
		}

		public IMigrator GetMigrator(XElement content) {
			return new XmlScriptSequenceMigrator(content);
		}

		private ScriptSequence _CreateSequenceFor(string filePath) {
			// Get the specific sequence module manager.
			SequenceModuleManagement manager = Modules.GetManager<ISequenceModuleInstance, SequenceModuleManagement>();

			// Get an instance of the appropriate sequence module.
			ScriptSequence sequence = (ScriptSequence)manager.Get(filePath);
			if(sequence == null) throw new InvalidOperationException("No sequence type defined for file " + filePath);

			return sequence;
		}
	}
}
