using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.App;

namespace SampleApp {
	public class SampleAppDescriptor : AppModuleDescriptorBase {
		private Guid _typeId = new Guid("{EF34B7C2-7504-4b8e-8EEB-295ACDA48B34}");

		public override string TypeName {
			get { return "Sample application module"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override string Author {
			get { return "Vixen Development Team"; }
		}

		public override string Description {
			get { return "Sample application module"; }
		}

		public override string Version {
			get { return "1.0"; }
		}

		public override Type ModuleClass {
			get { return typeof(SampleAppModule); }
		}
	}
}
