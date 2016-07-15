using System;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;
using Vixen.Module.MixingFilter;
using Vixen.Sys;

namespace VixenModules.LayerMixingFilter.ProportionalMix
{
	public class ProportionalMixModule : LayerMixingFilterModuleInstanceBase
	{
		public override DiscreteValue CombineDiscreteIntensity(DiscreteValue highLayerValue, DiscreteValue lowLayerValue)
		{
			double intensity = lowLayerValue.Intensity * (1 - highLayerValue.Intensity);
			highLayerValue.Intensity = Math.Max(intensity, highLayerValue.Intensity);
			return highLayerValue;
		}

		public override Color CombineFullColor(Color highLayerColor, Color lowLayerColor)
		{
			var hsv = HSV.FromRGB(lowLayerColor);
			hsv.V = hsv.V * (1 - HSV.VFromRgb(highLayerColor));
			return highLayerColor.Combine(hsv.ToRGB());
		}
	}

	
}