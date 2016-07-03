using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.IntensityOverlay
{
	public class IntensityOverlay : LayerMixingFilterModuleInstanceBase
	{
		
		public override DiscreteValue CombineDiscreteIntensity(DiscreteValue highLayerValue, DiscreteValue lowLayerValue)
		{
			lowLayerValue.Intensity = lowLayerValue.Intensity * highLayerValue.Intensity;
			return lowLayerValue;
		}

		public override Color CombineFullColor(Color highLayerColor, Color lowLayerColor)
		{
			var newV = HSV.VFromRgb(highLayerColor);
			HSV color = HSV.FromRGB(lowLayerColor);
			color.V = color.V * newV;
			return color.ToRGB();
		}
	}

	
}