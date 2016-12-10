using System.Drawing;
using System.Windows.Forms;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;
using Vixen.Module;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.MaskFill
{
	public class MaskFillModule : LayerMixingFilterModuleInstanceBase
	{
		private MaskAndFillData _data;

		public override DiscreteValue CombineDiscreteIntensity(DiscreteValue highLayerValue, DiscreteValue lowLayerValue)
		{
			if (highLayerValue.Intensity > 0 || !_data.ExcludeZeroValues)
			{
				return highLayerValue;
			}
			
			return lowLayerValue;
		}

		public override Color CombineFullColor(Color highLayerColor, Color lowLayerColor)
		{
			if (HSV.VFromRgb(highLayerColor) > 0 || !_data.ExcludeZeroValues)
			{
				return highLayerColor;
			}
			
			return lowLayerColor;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = (MaskAndFillData)value;
			}
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			using (MaskAndFillSetup setup = new MaskAndFillSetup(_data.ExcludeZeroValues))
			{
				if (setup.ShowDialog() == DialogResult.OK)
				{
					_data.ExcludeZeroValues = setup.ExcludeZeroValuesValues;
					return true;
				}
			}
			return false;
		}
	}

	
}