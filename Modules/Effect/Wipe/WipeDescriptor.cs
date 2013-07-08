using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace VixenModules.Effect.Wipe {
	public class WipeDescriptor : EffectModuleDescriptorBase {
		private static Guid _typeId = new Guid("{61746B54-A96C-4723-8BD6-39C7EA985F80}");
		private static Guid _CurvesId = new Guid("{4e258de2-7a75-4f0f-aa43-c8182e7f3400}");
		private static Guid _ColorGradientId = new Guid("{64f4ab26-3ed4-49a3-a004-23656ed0424a}");
		private static Guid _PulseId = new Guid("{cbd76d3b-c924-40ff-bad6-d1437b3dbdc0}");

		public override string EffectName {
			get { return "Wipe"; }
		}

		public override Guid TypeId {
			get { return _typeId; }
		}

		public override Type ModuleClass {
			get { return typeof(WipeModule); }
		}

		public override Type ModuleDataClass {
			get { return typeof(WipeData); }
		}

		public override string Author {
			get { return "Darren McDaniel"; }
		}

		public override string TypeName {
			get { return EffectName; }
		}

		public override string Description {
			get { return "Will Wipe a Color Gradient in a given direction across the elements"; }
		}

		public override string Version {
			get { return "1.0"; }
		}

		public override Guid[] Dependencies {
			get { return new Guid[] { _CurvesId, _ColorGradientId, _PulseId }; }
		}


		public override ParameterSignature Parameters {
			get {
				return new ParameterSignature(
					new ParameterSpecification("Color Gradient", typeof(ColorGradient)),
					new ParameterSpecification("Direction", typeof(WipeDirection)),
					new ParameterSpecification("Curve", typeof(Curve)),
					new ParameterSpecification("Pulse Time", typeof(int))
					);
			}
		}
	}
}

