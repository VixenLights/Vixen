using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Output;

namespace TestOutput {
	public class RenardModule : IOutputModuleDescriptor {
		static internal Guid _typeId = new Guid("{5E867382-36E4-45a3-A4CA-A220081D1167}");

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(Renard); }
		}

		public Type ModuleDataClass {
			get { return null; }
		}

		public string Author {
			get { return ""; }
		}

		public string TypeName {
			get { return "Renard"; }
		}

		public string Description {
			get { return ""; }
		}

		public string Version {
			get { return ""; }
		}

		public string FileName { get; set; }
		public string ModuleTypeName { get; set; }
	}
}
