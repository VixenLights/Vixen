using System;
using Vixen.Module.RuntimeBehavior;

namespace Recording {
	public class RecordingDescriptor : RuntimeBehaviorModuleDescriptorBase {
		private Guid _typeId = new Guid("{BEF934C3-B7C1-418f-8B25-CCD0566161FA}");

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(RecordingModule); }
		}

		override public Type ModuleDataClass {
			get { return typeof(RecordingData); }
		}

		override public string Author {
			get { return "Vixen Team"; }
		}

		override public string TypeName {
			get { return "Recording"; }
		}

		override public string Description {
			get { return "Provides recording functionality for sequences"; }
		}

		override public string Version {
			get { return "1.0"; }
		}
	}
}
