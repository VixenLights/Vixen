using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;

namespace TestRuntimeBehaviors {
	public class LiveModule : IRuntimeBehaviorModuleDescriptor {
		static internal Guid _typeId = new Guid("{54AEC5A8-CB6C-4256-A980-5E1D15C5427A}");

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(Live); }
		}

		public Type ModuleDataClass {
			get { return typeof(LiveData); }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Live"; }
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
