using System;
using Vixen.Module.Controller;

namespace VixenModules.Controller.PSC {
	public class PscDescriptor : ControllerModuleDescriptorBase {
		private Guid _typeId = new Guid("{2D2EA530-3AC9-4f9c-8896-8FD8E5A4D7A3}");

		public override string TypeName {
			get { return "Parallax Servo Controller"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(PscModule); }
		}

		public override Type ModuleDataClass {
			get { return typeof(PscData); }
		}

		public override string Author {
			get { return "Vixen Team"; }
		}

		public override string Description {
			get { return "Drives PSC versions x to x"; }
		}

		public override string Version {
			get { return "1.0"; }
		}
	}
}
