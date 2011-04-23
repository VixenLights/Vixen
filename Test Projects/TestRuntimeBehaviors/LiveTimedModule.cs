using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;

namespace TestRuntimeBehaviors {
	public class LiveTimedModule : IRuntimeBehaviorModuleDescriptor {
		static internal Guid _typeId = new Guid("{39B8C3A8-3B21-4fd7-810C-19BC9D705E5A}");

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(LiveTimed); }
		}

		public Type ModuleDataClass {
			get { return typeof(LiveTimedData); }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Live - Timed"; }
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
