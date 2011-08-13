using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module.Sequence;

namespace Vixen.IO.Xml {
	class XmlAnySequenceReader : ReaderBase {
		public override object Read(string filePath) {
			Sequence sequence = _CreateObject(filePath);
			if(sequence == null) throw new InvalidOperationException("No sequence type defined for file " + filePath);

			SequenceReaderAttribute readerClass = (SequenceReaderAttribute)sequence.GetType().GetCustomAttributes(typeof(SequenceReaderAttribute), true).FirstOrDefault();
			if(readerClass == null) throw new InvalidOperationException("Cannot load sequence " + Path.GetFileName(filePath) + " because it lacks a reader.");

			IReader reader = Activator.CreateInstance(readerClass.SequenceReaderClass) as IReader;
			if(reader == null) throw new InvalidOperationException("The reader class for sequence " + Path.GetFileName(filePath) + " is not of a valid type.");

			return reader.Read(filePath);
		}

		private Sequence _CreateObject(string filePath) {
			// Get the specific sequence module manager.
			SequenceModuleManagement manager = Modules.GetModuleManager<ISequenceModuleInstance, SequenceModuleManagement>();
			
			// Get an instance of the appropriate sequence module.
			Sequence sequence = manager.Get(filePath) as Sequence;
			
			return sequence;
		}
	}
}
