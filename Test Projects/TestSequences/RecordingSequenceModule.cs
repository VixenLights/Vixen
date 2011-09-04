using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;

namespace TestSequences {
	public class RecordingSequenceModule : SequenceModuleDescriptorBase {
		private Guid _typeId = new Guid("{8D61518B-22D9-4c44-8271-D08AC64D5B19}");

		override public string FileExtension {
			get { return ".rec"; }
		}

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(Recording); }
		}

		override public string Author {
			get { throw new NotImplementedException(); }
		}

		override public string TypeName {
			get { return "Recording sequence"; }
		}

		override public string Description {
			get { throw new NotImplementedException(); }
		}

		override public string Version {
			get { throw new NotImplementedException(); }
		}
	}
}
