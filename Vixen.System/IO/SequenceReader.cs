using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module.Effect;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Media;
using Vixen.Execution;

namespace Vixen.IO {
	class SequenceReader {
		public virtual bool Read(string filePath, Vixen.Sys.Sequence sequence) {
			// A sequence is a bit different because a specific instance type has
			// to be created from a module, so this method cannot create the instance.

			// A sequence's name is coupled to its file name.
			sequence.Name = Path.GetFileName(filePath);
			XElement element = Helper.LoadXml(filePath);
			Vixen.Sys.Sequence.ReadXml(element, sequence);
			return true;
		}

	}
}
