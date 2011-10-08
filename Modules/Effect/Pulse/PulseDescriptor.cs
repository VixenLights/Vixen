using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using Vixen.Module.Effect;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace VixenModules.Effect.Pulse
{
	public class PulseDescriptor : EffectModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{cbd76d3b-c924-40ff-bad6-d1437b3dbdc0}");

		static internal Guid RGBProperty = new Guid("{5c31be79-a6a7-4864-a660-4e0215ad4778}");
		static internal Guid CurvesModule = new Guid("{4e258de2-7a75-4f0f-aa43-c8182e7f3400}");
		static internal Guid ColorGradientModule = new Guid("{64f4ab26-3ed4-49a3-a004-23656ed0424a}");

		public PulseDescriptor()
		{
			PropertyDependencies = new[] {
				RGBProperty,
				CurvesModule,
				ColorGradientModule,
			};
		}

		override public string EffectName
		{
			get { return "Pulse"; }
		}

		override public CommandParameterSignature Parameters
		{
			get
			{
				return new CommandParameterSignature(
					new CommandParameterSpecification("Curve", typeof(Curve)),
					new CommandParameterSpecification("ColorGradient", typeof(ColorGradient))
					);
			}
		}

		override public Guid TypeId
		{
			get { return _typeId; }
		}

		override public Type ModuleClass
		{
			get { return typeof(Pulse); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(PulseData); }
		}

		override public string Author
		{
			get { return "Vixen Team"; }
		}

		override public string TypeName
		{
			get { return EffectName; }
		}

		override public string Description
		{
			get { return "Applies a pulse with a variable level and/or color to the target channels."; }
		}

		override public string Version
		{
			get { return "0.1"; }
		}
	}
}
