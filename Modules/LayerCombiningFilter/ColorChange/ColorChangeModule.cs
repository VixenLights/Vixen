using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.ColorChange
{
	public class ColorChangeModule : LayerMixingFilterModuleInstanceBase
	{
		public override DiscreteValue CombineDiscreteIntensity(DiscreteValue highLayerValue, DiscreteValue lowLayerValue)
		{
			highLayerValue.Intensity = lowLayerValue.Intensity;
			return highLayerValue;
		}

		public override Color CombineFullColor(Color highLayerColor, Color lowLayerColor)
		{

			var i = HSV.VFromRgb(lowLayerColor);

			var color = HSV.FromRGB(highLayerColor);

			color.V = i * color.V;

			return color.ToRGB();
		}

		#region Overrides of LayerMixingFilterModuleInstanceBase

		/// <inheritdoc />
		public override bool RequiresMixingPartner => true;

		#endregion
	}

	
}