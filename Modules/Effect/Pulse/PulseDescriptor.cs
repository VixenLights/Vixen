using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Effect;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace VixenModules.Effect.Pulse
{
	public class PulseDescriptor : EffectModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{cbd76d3b-c924-40ff-bad6-d1437b3dbdc0}");
		private static Guid _CurvesId = new Guid("{4e258de2-7a75-4f0f-aa43-c8182e7f3400}");
		private static Guid _ColorGradientId = new Guid("{64f4ab26-3ed4-49a3-a004-23656ed0424a}");

		public override string EffectName { get { return "Pulse"; } }

		public override Guid TypeId { get { return _typeId; } }

		public override Type ModuleClass { get { return typeof(Pulse); } }

		public override Type ModuleDataClass { get { return typeof(PulseData); } }

		public override string Author { get { return "Vixen Team"; } }

		public override string TypeName { get { return EffectName; } }

		public override string Description { get { return "Applies a pulse with a variable level and/or color to the target channels."; } }

		public override string Version { get { return "1.0"; } }

		public override Guid[] Dependencies { get { return new Guid[] { _CurvesId, _ColorGradientId }; } }

		public override ParameterSignature Parameters
		{
			get
			{
				return new ParameterSignature(
					new ParameterSpecification("Intensity Curve", typeof(Curve)),
					new ParameterSpecification("Color Gradient", typeof(ColorGradient))
					);
			}
		}
	}
}
