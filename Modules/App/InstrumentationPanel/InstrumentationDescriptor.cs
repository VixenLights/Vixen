using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.App;
using Vixen.Sys;

namespace VixenModules.App.InstrumentationPanel
{
	public class InstrumentationDescriptor : AppModuleDescriptorBase {
		private Guid _typeId = new Guid("{CBC07BF2-7E1B-4a29-AEED-9EE235CB0DEE}");

		public override string TypeName {
			get { return "Instrumentation Panel"; }
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
			get { return typeof(InstrumentationModule); }
		}
	}
}
