using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;

namespace TestSequences {
	public class CSharpScriptSequenceModule : SequenceModuleDescriptorBase {
		private Guid _typeId = new Guid("{AC5AB571-797A-4814-ADF9-99E6FB227FBA}");

		override public string FileExtension {
			get { return ".csp"; }
		}

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(CSharpScript); }
		}

		override public string Author {
			get { throw new NotImplementedException(); }
		}

		override public string TypeName {
			get { return "C# script sequence"; }
		}

		override public string Description {
			get { throw new NotImplementedException(); }
		}

		override public string Version {
			get { throw new NotImplementedException(); }
		}
	}
}
