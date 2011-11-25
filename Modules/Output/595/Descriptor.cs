using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Output;

namespace VixenModules.Output.Olsen595
{
	public class Descriptor : OutputModuleDescriptorBase {
		private Guid _typeId = new Guid("{9B8E5BC9-474F-4a6c-BE6C-455E506E54BF}");

		public override string Author {
			get { return "Vixen Team"; }
		}

		public override string Description {
			get { return "595 hardware module"; }
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
			get { return "595"; }
		}

		public override string Version {
			get { return "1.0"; }
		}
	}
}
