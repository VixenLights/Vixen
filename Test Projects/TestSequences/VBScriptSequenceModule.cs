using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;

namespace TestSequences {
	public class VBScriptSequenceModule : SequenceModuleDescriptorBase {
		private Guid _typeId = new Guid("{9A5F40EA-9150-471b-A76A-7BE43DA79972}");

		override public string FileExtension {
			get { return ".vbp"; }
		}

		override public string TypeName {
			get { return "VB script sequence"; }
		}

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(VBScript); }
		}

		override public string Author {
			get { throw new NotImplementedException(); }
		}

		override public string Description {
			get { throw new NotImplementedException(); }
		}

		override public string Version {
			get { throw new NotImplementedException(); }
		}
	}
}
