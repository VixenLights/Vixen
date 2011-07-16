using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Output;

namespace TestOutput {
	public class LogModule : OutputModuleDescriptorBase {
		private Guid _typeId = new Guid("{36AF9D3E-E965-49fc-A426-0AC496D15EF3}");

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(Log); }
		}

		override public Type ModuleDataClass {
			get { return null; }
		}

		override public string Author {
			get { throw new NotImplementedException(); }
		}

		override public string TypeName {
			get { return "Logger"; }
		}

		override public string Description {
			get { throw new NotImplementedException(); }
		}

		override public string Version {
			get { throw new NotImplementedException(); }
		}
	}
}
