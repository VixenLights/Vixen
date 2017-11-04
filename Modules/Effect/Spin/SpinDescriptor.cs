using System;
using Vixen.Sys;
using Vixen.Module.Effect;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using System.Drawing;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Spin
{
	public class SpinDescriptor : EffectModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{821a8540-ea34-401f-a8aa-416d7d9a196a}");
		private static Guid _CurvesId = new Guid("{4e258de2-7a75-4f0f-aa43-c8182e7f3400}");
		private static Guid _ColorGradientId = new Guid("{64f4ab26-3ed4-49a3-a004-23656ed0424a}");
		private static Guid _PulseId = new Guid("{cbd76d3b-c924-40ff-bad6-d1437b3dbdc0}");

		public override string EffectName
		{
			get { return "Spin"; }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Basic; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (Spin); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (SpinData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string TypeName
		{
			get { return EffectName; }
		}

		public override string Description
		{
			get { return "Generates pulses on consecutive elements in the given group to give the appearance of spinning."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Guid[] Dependencies
		{
			get { return new Guid[] {_CurvesId, _ColorGradientId, _PulseId}; }
		}

		public override ParameterSignature Parameters
		{
			get
			{
				return new ParameterSignature(
					new ParameterSpecification("Spin Speed Format", typeof (SpinSpeedFormat)),
					new ParameterSpecification("Pulse Length Format", typeof (SpinPulseLengthFormat)),
					new ParameterSpecification("Color Handling", typeof (SpinColorHandling)),
					new ParameterSpecification("Revolution Count", typeof (double)),
					new ParameterSpecification("Revolution Speed (Hz)", typeof (double)),
					new ParameterSpecification("Revolution Time (ms)", typeof (int)),
					new ParameterSpecification("Pulse Time", typeof (int)),
					new ParameterSpecification("Pulse Percentage (of revolution)", typeof (int)),
					new ParameterSpecification("Default element level", typeof (double)),
					new ParameterSpecification("Static Color", typeof (Color)),
					new ParameterSpecification("Color Gradient", typeof (ColorGradient)),
					new ParameterSpecification("Individual Pulse Curve", typeof (Curve)),
					new ParameterSpecification("Reverse Spin", typeof (bool)),
					new ParameterSpecification("Depth of Effect", typeof (int))
					);
			}
		}
	}
}