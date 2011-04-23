using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;

namespace TestRuntimeBehaviors {
	public class RecordingModule : IRuntimeBehaviorModuleDescriptor {
		static internal Guid _typeId = new Guid("{BEF934C3-B7C1-418f-8B25-CCD0566161FA}");

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(Recording); }
		}

		public Type ModuleDataClass {
			get { return typeof(RecordingData); }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Recording"; }
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
