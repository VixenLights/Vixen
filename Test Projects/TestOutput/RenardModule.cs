using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Output;

namespace TestOutput {
	public class RenardModule : OutputModuleDescriptorBase {
		private Guid _typeId = new Guid("{5E867382-36E4-45a3-A4CA-A220081D1167}");

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(Renard); }
		}

		override public string Author {
			get { return ""; }
		}

		override public string TypeName {
			get { return "Test Renard"; }
		}

		override public string Description {
			get { return ""; }
		}

		override public string Version {
			get { return ""; }
		}
	}
}
