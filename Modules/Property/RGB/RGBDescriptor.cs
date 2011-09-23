using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Property;

namespace VixenModules.Property.RGB
{
	public class RGBDescriptor : PropertyModuleDescriptorBase {
		private Guid _id = new Guid("{55960E71-2151-454C-885E-00B9713A93EF}");

		override public string TypeName {
			get { return "RGB"; }
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
			get { return "Vixen Team"; }
		}

		override public string Description {
			get { return "Allows a channel (or group of channels) to be configured as a single RGB element, so it can be operated on in the context of a color."; }
		}

		override public string Version {
			get { return "0.1"; }
		}
	}
}
