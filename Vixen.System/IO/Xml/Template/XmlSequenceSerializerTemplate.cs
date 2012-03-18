using System;
using System.IO;
using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.IO.Xml.Migrator;
using Vixen.IO.Xml.Policy;
using Vixen.Module.Sequence;
using Vixen.Sys;

namespace Vixen.IO.Xml.Template {
	class XmlSequenceSerializerTemplate : IXmlStandardFileWriteTemplate<Sequence>, IXmlStandardFileReadTemplate<Sequence> {
		public XmlVersionedContent GetContentNode() {
			return new XmlVersionedContent("Sequence");
		}

		public IFilePolicy GetEmptyFilePolicy() {
			return new XmlSequenceFilePolicy();
		}

		public IFilePolicy GetFilePolicy(Sequence obj, XElement content) {
			return new XmlSequenceFilePolicy(obj, content);
		}

		public string GetAbsoluteFilePath(string filePath) {
			if(!Path.IsPathRooted(filePath)) {
				filePath = Path.Combine(Sequence.DefaultDirectory, filePath);
			}
			return filePath;
		}


		public Sequence CreateNewObjectFor(string filePath) {
			return _CreateSequenceFor(filePath);
		}

		public IMigrator GetMigrator(XElement content) {
			return new XmlSequenceMigrator(content);
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
