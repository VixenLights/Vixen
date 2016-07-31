using System;
using Vixen.Sys;
using Vixen.Module.Effect;
using VixenModules.App.ColorGradients;
using VixenModules.Effect.AudioHelp;

namespace VixenModules.Effect.VUMeter
{
    public class VUMeterDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("{8a360703-bf3f-456a-a798-13cd67287dde}");
        private static readonly Guid _ColorGradientId = new Guid("{64f4ab26-3ed4-49a3-a004-23656ed0424a}");

		public override bool SupportsMedia
		{
			get { return true; }
		}

		public override string EffectName
		{
            get { return "VU Meter"; }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Pixel; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
            get { return typeof(VUMeter); }
		}

		public override Type ModuleDataClass
		{
            get { return typeof(VUMeterData); }
		}

		public override string Author
		{
			get { return "David Corrigan";}
		}

		public override string TypeName
		{
			get { return EffectName; }
		}

		public override string Description
		{
			get { return "Modulates light intensity with sound volume."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Guid[] Dependencies
		{
            get { return new Guid[] { _ColorGradientId }; }
		}

		public override ParameterSignature Parameters
		{
			get
			{
                return new ParameterSignature(
                    new ParameterSpecification("Invert", typeof(bool)),
                    new ParameterSpecification("Decay Time", typeof(int)),
                    new ParameterSpecification("Attack Time", typeof(int)),
                    new ParameterSpecification("Normalize Audio", typeof(bool)),
                    new ParameterSpecification("Gain",typeof(int)),
                    new ParameterSpecification("Range",typeof(int)),
                    new ParameterSpecification("Green Color Position",typeof(int)),
                    new ParameterSpecification("Red Color Position",typeof(int)),
                    new ParameterSpecification("Meter Color Gradient",typeof(ColorGradient)),
                    new ParameterSpecification("Meter Gradient Style",typeof(MeterColorTypes))
                    );
			}
		}
	}
}