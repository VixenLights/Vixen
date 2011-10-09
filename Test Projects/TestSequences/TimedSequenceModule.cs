using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;

namespace TestSequences {
	public class TimedSequenceModule : SequenceModuleDescriptorBase {
		private Guid _typeId = new Guid("{4C258A3B-E725-4ae7-B50B-103F6AB8121E}");

		override public string FileExtension {
			get { return ".timtest"; }
		}

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(Timed); }
		}

		override public string Author {
			get { throw new NotImplementedException(); }
		}

		override public string TypeName {
			get { return "Timed sequence (test)"; }
		}

		override public string Description {
			get { throw new NotImplementedException(); }
		}

		override public string Version {
			get { throw new NotImplementedException(); }
		}
	}
}
