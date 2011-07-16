using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;

namespace TestRuntimeBehaviors {
	public class RecordingModule : RuntimeBehaviorModuleDescriptorBase {
		private Guid _typeId = new Guid("{BEF934C3-B7C1-418f-8B25-CCD0566161FA}");

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(Recording); }
		}

		override public Type ModuleDataClass {
			get { return typeof(RecordingData); }
		}

		override public string Author {
			get { throw new NotImplementedException(); }
		}

		override public string TypeName {
			get { return "Recording"; }
		}

		override public string Description {
			get { throw new NotImplementedException(); }
		}

		override public string Version {
			get { throw new NotImplementedException(); }
		}
	}
}
