using System;
using Vixen.Module.PostFilter;

namespace Grayscale {
	public class GrayscaleDescriptor : PostFilterModuleDescriptorBase {
		private Guid _typeId = new Guid("{DAC271B0-0743-45ef-B4E0-D5957AF7F019}");

		public override string TypeName {
			get { return "Color to grayscale filter"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(GrayscaleModule); }
		}

		public override Type ModuleDataClass {
			get { return typeof(GrayscaleData); }
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

		public override string PostFilterName {
			get { return TypeName; }
		}
	}
}
