using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;
using Vixen.Module;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.Mask
{
	public class MaskModule : LayerMixingFilterModuleInstanceBase
	{
		private MaskFilterModuleData _data;

		public MaskModule()
		{
			_data = new MaskFilterModuleData();
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = (MaskFilterModuleData)value;
			}
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override DiscreteValue CombineDiscreteIntensity(DiscreteValue highLayerValue, DiscreteValue lowLayerValue)
		{
			if (highLayerValue.Intensity > 0)
			{
				return highLayerValue;
			}
			return lowLayerValue;
		}

		public override Color CombineFullColor(Color highLayerColor, Color lowLayerColor)
		{
			if(HSV.VFromRgb(highLayerColor) > 0)
			{
				return highLayerColor;
			}
			
			return lowLayerColor;
		}
	}

	
}