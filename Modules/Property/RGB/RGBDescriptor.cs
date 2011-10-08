using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Property;

namespace VixenModules.Property.RGB
{
	public class RGBDescriptor : PropertyModuleDescriptorBase {
		private static Guid _id = new Guid("{5c31be79-a6a7-4864-a660-4e0215ad4778}");

		override public string TypeName {
			get { return "RGB"; }
		}

		override public Guid TypeId {
			get { return _id; }
		}

		override public Type ModuleClass {
			get { return typeof(RGBModule); }
		}

		override public Type ModuleDataClass {
			get { return typeof(RGBData); }
		}

		public override Type ModuleStaticDataClass {
			get { return typeof(RGBStaticData); }
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

		// TODO: how can I do this nicely? I need to be able to get the type ID in a static method, but can't make TypeId static.
		// So I've had to do the same thing, but in a static property? booooo!
		public static Guid ModuleID {
			get { return _id; }
		}
	}
}
