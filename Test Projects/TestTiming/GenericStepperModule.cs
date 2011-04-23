using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Timing;

namespace TestTiming {
	public class GenericStepperModule : ITimingModuleDescriptor {
		static internal Guid _typeId = new Guid("{811ADF09-49F3-47e5-B602-F3A68CF4715E}");
		//static internal Guid _typeId = Guid.Empty;

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(GenericStepper); }
		}

		public Type ModuleDataClass {
			get { return null; }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Generic Stepper"; }
		}

		public string Description {
			get { throw new NotImplementedException(); }
		}

		public string Version {
			get { throw new NotImplementedException(); }
		}

		public string FileName { get; set; }

		public string ModuleTypeName { get; set; }
	}
}
