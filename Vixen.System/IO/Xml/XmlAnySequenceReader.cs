using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Vixen.Sys;
using Vixen.Module.Sequence;

namespace Vixen.IO.Xml {
	class XmlAnySequenceReader : XmlReaderBase<Sequence> {
		public override Sequence Read(string filePath) {
			// Get an instance of a sequence appropriate for the file path.
			Sequence sequence = _CreateObject(filePath);
			if(sequence == null) throw new InvalidOperationException("No sequence type defined for file " + filePath);

			// Get the reader for the sequence type.
			SequenceReaderAttribute readerClass = (SequenceReaderAttribute)sequence.GetType().GetCustomAttributes(typeof(SequenceReaderAttribute), true).FirstOrDefault();
			if(readerClass == null) throw new InvalidOperationException("Cannot load sequence " + Path.GetFileName(filePath) + " because it lacks a reader.");

			// Create the reader.
			IReader reader = Activator.CreateInstance(readerClass.SequenceReaderClass) as IReader;
			if(reader == null) throw new InvalidOperationException("The reader class for sequence " + Path.GetFileName(filePath) + " is not of a valid type.");

			// Read the sequence file.
			return reader.Read(filePath) as Sequence;
		}

		private Sequence _CreateObject(string filePath) {
			// Get the specific sequence module manager.
			SequenceModuleManagement manager = Modules.GetModuleManager<ISequenceModuleInstance, SequenceModuleManagement>();
			
			// Get an instance of the appropriate sequence module.
			Sequence sequence = manager.Get(filePath) as Sequence;
			
			return sequence;
		}

		protected override Sequence _CreateObject(XElement element, string filePath) {
			throw new NotImplementedException();
		}

		protected override void _PopulateObject(Sequence obj, XElement element) {
			throw new NotImplementedException();
		}

		protected override IEnumerable<Func<XElement, XElement>> _ProvideMigrations(int versionAt, int targetVersion) {
			throw new NotImplementedException();
		}
	}
}
