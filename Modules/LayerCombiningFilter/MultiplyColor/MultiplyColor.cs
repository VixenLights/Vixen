using System.Drawing;
using Vixen.Data.Value;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.MultiplyColor
{
    public class MultiplyColor : LayerMixingFilterModuleInstanceBase
	{

        public override DiscreteValue CombineDiscreteIntensity(DiscreteValue highLayerValue, DiscreteValue lowLayerValue)
        {
            lowLayerValue.Intensity = lowLayerValue.Intensity * highLayerValue.Intensity;
            return lowLayerValue;
        }

        public override Color CombineFullColor(Color highLayerColor, Color lowLayerColor)
        {
            int newR = Convert(highLayerColor.R, lowLayerColor.R);
            int newG = Convert(highLayerColor.G, lowLayerColor.G);
            int newB = Convert(highLayerColor.B, lowLayerColor.B);
            return Color.FromArgb((int)newR, newG, newB);
        }
        private int Convert(int highValue, int lowValue)
        {
            double newVal = highValue > 0 ? (lowValue * (1d - (highValue / 255d))) : lowValue;
            return (int)newVal;
        }
	}

	
}