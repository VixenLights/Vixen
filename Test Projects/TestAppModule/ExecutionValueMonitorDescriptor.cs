using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.App;

namespace TestAppModule {
	public class ExecutionValueMonitorDescriptor : AppModuleDescriptorBase {
		private Guid _typeId = new Guid("{F1BC1D1D-9BE6-49a1-BD24-FA4081ED49D2}");

		public override string TypeName {
			get { return "Execution value monitor"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override string Author {
			get { throw new NotImplementedException(); }
		}

		public override string Description {
			get { throw new NotImplementedException(); }
		}

		public override string Version {
			get { throw new NotImplementedException(); }
		}

		public override Type ModuleClass {
			get { return typeof(ExecutionValueMonitorModule); }
		}
	}
}
