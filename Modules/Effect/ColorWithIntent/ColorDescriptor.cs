using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace ColorWithIntent {
	public class ColorDescriptor : EffectModuleDescriptorBase {
		//static internal Guid _colorIntentId = new Guid("{C66DEB83-1252-4d28-8C50-535ECCE183EC}");
		private Guid _typeId = new Guid("{2C8B4C3A-14E9-42f6-BC26-44571F672224}");
		private ParameterSignature _signature;

		public ColorDescriptor() {
			_signature = new ParameterSignature(
				new ParameterSpecification("Curve", typeof(Curve)),
				new ParameterSpecification("Color", typeof(ColorGradient)));
		}

		public override string TypeName {
			get { return "Color (with intent)"; }
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

		public override string EffectName {
			get { return TypeName; }
		}

		public override ParameterSignature Parameters {
			get { return _signature; }
		}

		//public override Guid[] Dependencies {
		//    get { return new[] { _colorIntentId }; }
		//}
	}
}
