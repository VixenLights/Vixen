using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
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
            int newR , newG, newB;
            if(highLayerColor.R >0)
                newR = (highLayerColor.R * lowLayerColor.R)/256;
            else
                newR=lowLayerColor.R;
            if(highLayerColor.G >0)
                newG = (highLayerColor.G * lowLayerColor.G)/256;
            else
                newG=lowLayerColor.G;
            if(highLayerColor.B>0)
                newB = (highLayerColor.B * lowLayerColor.B)/256;
            else
                newB=lowLayerColor.B;
            return Color.FromArgb(newR, newG, newB);
        }
	}

	
}