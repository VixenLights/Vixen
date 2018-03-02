using System;
using System.Drawing;
using System.Net.Configuration;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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
            //brightness matching conditions.  Checks first because it's easy math.
		    if ( !(HSV.VFromRgb(lowLayerColor) >= Convert.ToDouble(_data.LowerLimit) / 100
                  && HSV.VFromRgb(lowLayerColor) <= Convert.ToDouble(_data.UpperLimit) / 100) )
		    { return lowLayerColor; }  //brightness check failed - abort

		    //Saturation Matching
		    var keySaturation = _data.KeyColor.GetSaturation(); //this sat shit aint workin
		    if (!(lowLayerColor.GetSaturation() < keySaturation + _data.SaturationTolerance
		          && lowLayerColor.GetSaturation() > keySaturation - _data.SaturationTolerance))
		    { return lowLayerColor; } //saturation check failed - abort

            //Hue matching
		    var keyHue = _data.KeyColor.GetHue();
		    var lowLayerHue = lowLayerColor.GetHue();

            if (lowLayerHue - _data.HueTolerance > 0 //no low overflow
		        && lowLayerHue + _data.HueTolerance < 360 //no high overflow
		        && lowLayerHue >= keyHue - _data.HueTolerance
		        && lowLayerHue <= keyHue + _data.HueTolerance)
		    { return highLayerColor; }

            else if (   keyHue - _data.HueTolerance <= 0 //low end key overflow
                     && (lowLayerHue >= keyHue - _data.HueTolerance + 360
                        || lowLayerHue <= keyHue + _data.HueTolerance) )
		    { return highLayerColor; }

            else if (   keyHue + _data.HueTolerance >= 360 //high end key overflow
                     && lowLayerHue >= keyHue - _data.HueTolerance
                     && lowLayerHue <= keyHue + _data.HueTolerance - 360) 
		    { return highLayerColor; }
		    // need to add validation so that hueTolerance is always between 0 and 180
		    else return lowLayerColor;  //hue check failed - return low layer color
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = (ChromaKeyData)value; }
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