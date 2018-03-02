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
		    var lowerLimit = Convert.ToDouble(_data.LowerLimit) / 100;
		    var upperLimit = Convert.ToDouble(_data.UpperLimit) / 100;
            //for debugging
            var keyHue = _data.KeyColor.GetHue();
		    var lowLayerHue = lowLayerColor.GetHue();
            var hueMatch = false; //can i refactor this out by properly placing returns
		    var saturationMatch = true;  //change to false later when matching code is ready.

            //brightness matching conditions.  Checks first because it's easy math.
		    if (!(HSV.VFromRgb(lowLayerColor) >= lowerLimit
		          && HSV.VFromRgb(lowLayerColor) <= upperLimit))
		    { return lowLayerColor; }

            //hue matching conditions
		    if (lowLayerHue - _data.HueTolerance > 0 //no low overflow
		        && lowLayerHue + _data.HueTolerance < 360 //no high overflow
		        && lowLayerHue >= keyHue - _data.HueTolerance
		        && lowLayerHue <= keyHue + _data.HueTolerance)
		    { hueMatch = true; }

            else if (   keyHue - _data.HueTolerance <= 0 //low end key overflow
                     && (lowLayerHue >= keyHue - _data.HueTolerance + 360
                        || lowLayerHue <= keyHue + _data.HueTolerance) )
		    { hueMatch = true; }

            else if (   keyHue + _data.HueTolerance >= 360 //high end key overflow
                     && lowLayerHue >= keyHue - _data.HueTolerance
                     && lowLayerHue <= keyHue + _data.HueTolerance - 360) 
		    { hueMatch = true; }
		    // need to add validation so that hueTolerance is always between 0 and 180
		    else return lowLayerColor;

            //add saturation matching here...


            //If there's a match in hue, saturation and brightness ranges.
            if (hueMatch && saturationMatch)
			{ return highLayerColor; }			
			return lowLayerColor;
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