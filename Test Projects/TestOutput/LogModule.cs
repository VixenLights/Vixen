using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Output;

namespace TestOutput {
	public class LogModule : IOutputModuleDescriptor {
		static internal Guid _typeId = new Guid("{36AF9D3E-E965-49fc-A426-0AC496D15EF3}");

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(Log); }
		}

		public Type ModuleDataClass {
			get { return null; }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Logger"; }
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
