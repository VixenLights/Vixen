using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.OutputFilter;

namespace Color {
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
			get { throw new NotImplementedException(); }
		}

		public override string Description {
			get { throw new NotImplementedException(); }
		}

		public override string Version {
			get { throw new NotImplementedException(); }
		}
	}
}
