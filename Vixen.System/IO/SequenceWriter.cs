using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module.Effect;
using Vixen.Execution;

namespace Vixen.IO {
	class SequenceWriter {
		public void Write(string filePath, Vixen.Sys.Sequence sequence) {
			// A sequence's name is coupled to its file name.
			sequence.Name = Path.GetFileName(filePath);
			Vixen.Sys.Sequence.WriteXml(sequence).Save(filePath);
		}
	}
}
