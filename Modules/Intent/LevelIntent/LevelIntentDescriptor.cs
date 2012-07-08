using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Intent;

namespace LevelIntent {
	public class LevelIntentDescriptor : IntentModuleDescriptorBase {
		private Guid _typeId = new Guid("{0DFDF022-B1C4-49b9-9D65-2568A372FE28}");

		public override string TypeName {
			get { return "Lighting-level intent"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(LevelIntentModule); }
		}

		public override string Author {
			get { throw new NotImplementedException(); }
		}

		public override string Description {
			get { throw new NotImplementedException(); }
		}

		public override string Version {
			get { throw new NotImplementedException(); }
		}

		public override Type ModuleDataClass {
			get { return typeof(LevelIntentData); }
		}
	}
}
