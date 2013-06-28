using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.App.ColorGradients;

namespace VixenModules.Effect.Alternating
{
    public class AlternatingDescriptor : EffectModuleDescriptorBase
    {
        private static Guid _typeId = new Guid("{7B791008-56A2-4BFF-8CE3-A7FB89EA4637}");
        private static Guid _ColorGradientId = new Guid("{64f4ab26-3ed4-49a3-a004-23656ed0424a}");
		

        public override string EffectName { get { return "Alternating"; } }

        public override Guid TypeId { get { return _typeId; } }

        public override Type ModuleClass { get { return typeof(Alternating); } }

        public override Type ModuleDataClass { get { return typeof(AlternatingData); } }

        public override string Author { get { return "Darren McDaniel"; } }

        public override string TypeName { get { return EffectName; } }

        public override string Description { get { return "Sets the target elements to an alternating output level and/or color."; } }

        public override string Version { get { return "1.0"; } }
        public override Guid[] Dependencies
        {
            get { return new Guid[] { _ColorGradientId}; }
        }

        public override ParameterSignature Parameters
        {
            get
            {
                return new ParameterSignature(
                    new ParameterSpecification("Level1", typeof(double)),
                    new ParameterSpecification("Color1", typeof(Color)),
                    new ParameterSpecification("Level2", typeof(double)),
                    new ParameterSpecification("Color2", typeof(Color)),
                    new ParameterSpecification("Interval", typeof(double)),
                    new ParameterSpecification("Enable", typeof(bool)),
                    new ParameterSpecification("DepthOfEffect", typeof(int)),
                    new ParameterSpecification("GroupEffect", typeof(int)),
                    new ParameterSpecification("StaticColor1", typeof(bool)),
                    new ParameterSpecification("StaticColor2", typeof(bool)),
                    new ParameterSpecification("ColorGradient1", typeof(ColorGradient)),
                    new ParameterSpecification("ColorGradient2", typeof(ColorGradient))

                    );
            }
        }
    }
}
