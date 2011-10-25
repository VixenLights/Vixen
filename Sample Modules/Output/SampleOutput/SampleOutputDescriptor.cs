using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Output;

namespace SampleOutput {
	public class SampleOutputDescriptor : OutputModuleDescriptorBase {
		private Guid _typeId = new Guid("{BE95370C-1E68-422d-BD2E-EABFAED65A1B}");

		public override string Author {
			get { return "Vixen Development Team"; }
		}

		public override string Description {
			get { return "Sample output module"; }
		}

		public override Type ModuleClass {
			get { return typeof(SampleOutputModule); }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override string TypeName {
			get { return "Sample output"; }
		}

		public override string Version {
			get { return "1.0"; }
		}

		public override Type ModuleDataClass {
			get { return typeof(SampleOutputData); }
		}
	}
}
