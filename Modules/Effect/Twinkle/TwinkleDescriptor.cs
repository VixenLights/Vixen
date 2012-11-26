using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vixen.Sys;
using Vixen.Module.Effect;
using VixenModules.App.ColorGradients;

namespace VixenModules.Effect.Twinkle
{
	public class TwinkleDescriptor : EffectModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{83bdd6f7-19c7-4598-b8e3-7ce28c44e7db}");
		internal static Guid _RGBPropertyId = new Guid("{5c31be79-a6a7-4864-a660-4e0215ad4778}");
		private static Guid _CurvesId = new Guid("{4e258de2-7a75-4f0f-aa43-c8182e7f3400}");
		private static Guid _ColorGradientId = new Guid("{64f4ab26-3ed4-49a3-a004-23656ed0424a}");
		private static Guid _PulseId = new Guid("{cbd76d3b-c924-40ff-bad6-d1437b3dbdc0}");

		public override string EffectName { get { return "Twinkle"; } }

		public override Guid TypeId { get { return _typeId; } }

		public override Type ModuleClass { get { return typeof(Twinkle); } }

		public override Type ModuleDataClass { get { return typeof(TwinkleData); } }

		public override string Author { get { return "Vixen Team"; } }

		public override string TypeName { get { return EffectName; } }

		public override string Description { get { return "Randomly generates flickering pulses of light on the target channels."; } }

		public override string Version { get { return "1.0"; } }

		public override Guid[] Dependencies { get { return new Guid[] { _CurvesId, _ColorGradientId, _PulseId }; } }

		public override ParameterSignature Parameters
		{
			get
			{
				return new ParameterSignature(
					new ParameterSpecification("Individual Channels", typeof(bool)),
					new ParameterSpecification("Minimum Level", typeof(double)),
					new ParameterSpecification("Maximum Level", typeof(double)),
					new ParameterSpecification("Level Variation (%)", typeof(int)),
					new ParameterSpecification("Average Pulse Time (ms)", typeof(int)),
					new ParameterSpecification("Pulse Time Variation (%)", typeof(int)),
					new ParameterSpecification("Average Coverage (%)", typeof(int)),
					new ParameterSpecification("Color Handling", typeof(TwinkleColorHandling)),
					new ParameterSpecification("Static Color", typeof(Color)),
					new ParameterSpecification("Color Gradient", typeof(ColorGradient))
					);
			}
		}
	}

	public enum TwinkleColorHandling
	{
		StaticColor,
		GradientThroughWholeEffect,
		GradientForEachPulse,
		ColorAcrossItems
	}
}
