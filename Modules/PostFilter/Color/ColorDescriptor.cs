using System;
using Vixen.Module.OutputFilter;

namespace VixenModules.OutputFilter.Color {
	public class ColorDescriptor : OutputFilterModuleDescriptorBase {
		private Guid _typeId = new Guid("{B3C06A83-CE75-4e78-853D-B95B4E69CEAC}");

		public override string TypeName {
			get { return "Color filter"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(ColorModule); }
		}

		public override Type ModuleDataClass {
			get { return typeof(ColorData); }
		}

		public override string Author {
			get { return "Vixen Team"; }
		}

		public override string Description {
			get { return "Output filters for color components."; }
		}

		public override string Version {
			get { return "1.0"; }
		}
	}
}
