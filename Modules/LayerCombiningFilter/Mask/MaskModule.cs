using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.Mask
{
	public class MaskModule : LayerMixingFilterModuleInstanceBase
	{
		public override DiscreteValue CombineDiscreteIntensity(DiscreteValue highLayerValue, DiscreteValue lowLayerValue)
		{
			if (highLayerValue.Intensity > 0)
			{
				lowLayerValue.Intensity = 0;
				return lowLayerValue;
			}
			return lowLayerValue;
		}

		public override Color CombineFullColor(Color highLayerColor, Color lowLayerColor)
		{
			if(HSV.VFromRgb(highLayerColor) > 0)
			{
				return Color.Empty;
			}
			
			return lowLayerColor;
		}
	}

	
}