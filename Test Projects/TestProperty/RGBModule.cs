using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Property;

namespace TestProperty {
	public class RGBModule : PropertyModuleDescriptorBase {
		private Guid _id = new Guid("{55960E71-2151-454c-885E-00B9713A93EF}");

		override public string TypeName {
			get { return "RGB (test)"; }
		}

		override public Guid TypeId {
			get { return _id; }
		}

		override public Type ModuleClass {
			get { return typeof(RGB); }
		}

		override public Type ModuleDataClass {
			get { return typeof(RGBData); }
		}

		override public string Author {
			get { throw new NotImplementedException(); }
		}

		override public string Description {
			get { throw new NotImplementedException(); }
		}

		override public string Version {
			get { throw new NotImplementedException(); }
		}
	}
}
