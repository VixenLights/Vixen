using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Controller;

namespace VixenModules.Output.Renard
{
	public class Descriptor : ControllerModuleDescriptorBase {
		private Guid _typeId = new Guid("{F66130A6-FF8A-48a7-BF1E-FAC6CEC2FC2C}");

		public override string Author {
			get { return "Vixen Team"; }
		}

		public override string Description {
			get { return "Renard hardware module"; }
		}

		public override Type ModuleClass {
			get { return typeof(Module); }
		}

		public override Type ModuleDataClass {
			get { return typeof(Data); }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override string TypeName {
			get { return "Renard"; }
		}

		public override string Version {
			get { return "1.0"; }
		}
	}
}
