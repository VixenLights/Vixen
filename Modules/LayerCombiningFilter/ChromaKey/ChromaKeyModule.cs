using System;
using System.Drawing;
using System.Windows.Forms;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;
using Vixen.Module;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.ChromaKey
{
	public class ChromaKeyModule : LayerMixingFilterModuleInstanceBase
	{
		private ChromaKeyData _data;

		public override DiscreteValue CombineDiscreteIntensity(DiscreteValue highLayerValue, DiscreteValue lowLayerValue)
		{
		    var lowerLimit = Convert.ToDouble(_data.LowerLimit) / 100;
		    var upperLimit = Convert.ToDouble(_data.UpperLimit) / 100;
            if (lowLayerValue.Intensity >= lowerLimit && lowLayerValue.Intensity <= upperLimit /*|| !_data.ExcludeZeroValues*/)
			{
				return highLayerValue;
			}	
			return lowLayerValue;
		}

		public override Color CombineFullColor(Color highLayerColor, Color lowLayerColor)
		{
		    var lowerLimit = Convert.ToDouble(_data.LowerLimit) / 100;
		    var upperLimit = Convert.ToDouble(_data.UpperLimit) / 100;
            if (HSV.VFromRgb(lowLayerColor) >= lowerLimit 
                && HSV.VFromRgb(lowLayerColor) <= upperLimit
                && lowLayerColor.GetHue() == _data.KeyColor.GetHue()/360
                && lowLayerColor.GetSaturation() == _data.KeyColor.GetSaturation() )
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
				_data = (ChromaKeyData)value;
			}
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
            using (var setup = new ChromaKeySetup(_data.ExcludeZeroValues, _data.LowerLimit, _data.UpperLimit, _data.KeyColor))
            {
                if (setup.ShowDialog() != DialogResult.OK) return false;
                _data.ExcludeZeroValues = setup.ExcludeZeroValuesValues;
                _data.LowerLimit = setup.LowerLimit;
                _data.UpperLimit = setup.UpperLimit;
                _data.KeyColor = setup.KeyColor;
                return true;
            }			
		}
	}	
}
