using System.Drawing;
using Vixen.Data.Value;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.HighestValueWins
{
	public class HighestValueModule : LayerMixingFilterModuleInstanceBase
	{
		public override DiscreteValue CombineDiscreteIntensity(DiscreteValue highLayerValue, DiscreteValue lowLayerValue)
		{
            return highLayerValue.Intensity > lowLayerValue.Intensity ? highLayerValue : lowLayerValue;
        }

		public override Color CombineFullColor(Color highLayerColor, Color lowLayerColor)
        {
            var r = highLayerColor.R > lowLayerColor.R ? highLayerColor.R : lowLayerColor.R;
            var g = highLayerColor.G > lowLayerColor.G ? highLayerColor.G : lowLayerColor.G;
            var b = highLayerColor.B > lowLayerColor.B ? highLayerColor.B : lowLayerColor.B;
            return Color.FromArgb(r,g,b);
		}
	}	
}