using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Property;

namespace SampleProperty {
	public class SamplePropertyDescriptor : PropertyModuleDescriptorBase {
		private Guid _typeId = new Guid("{F7A1D96D-DC90-427f-927C-7A79DEABDCA7}");

		public override string TypeName {
			get { return "Sample Property"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(SamplePropertyModule); }
		}

		public override string Author {
			get { return "Vixen Development Team"; }
		}

		public override string Description {
			get { return "Sample property module"; }
		}

		public override string Version {
			get { return "1.0"; }
		}
	}
}
