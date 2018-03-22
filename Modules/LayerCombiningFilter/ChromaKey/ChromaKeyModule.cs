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
			//do nothing on discrete intents.	
			return lowLayerValue;
		}

		public override Color CombineFullColor(Color highLayerColor, Color lowLayerColor)
		{
			//brightness matching conditions.  Checks first because it's easy math.
			var lowLayerV = Math.Round(HSV.VFromRgb(lowLayerColor), 2);
			if ( !(lowLayerV >= _data.LowerLimit && lowLayerV <= _data.UpperLimit) )
			{ return lowLayerColor; }			
            
		    //Saturation Matching
		    var lowLayerSaturation = Math.Round(HSV.FromRGB(lowLayerColor).S , 2);
		    var keySaturation = Math.Round(HSV.FromRGB(_data.KeyColor).S , 2);
		    if (!(lowLayerSaturation <= keySaturation + _data.SaturationTolerance
		          && lowLayerSaturation >= keySaturation - _data.SaturationTolerance))
		    { return lowLayerColor; } //saturation check failed - abort

            //Hue Matching
		    var keyHue = _data.KeyColor.GetHue();
		    var lowLayerHue = lowLayerColor.GetHue();

            if (lowLayerHue - _data.HueTolerance > 0 //no low overflow
		        && lowLayerHue + _data.HueTolerance < 360 //no high overflow
		        && lowLayerHue >= keyHue - _data.HueTolerance
		        && lowLayerHue <= keyHue + _data.HueTolerance)
		    { return highLayerColor; }
            if (keyHue - _data.HueTolerance <= 0 //low end key overflow
                     && (lowLayerHue >= keyHue - _data.HueTolerance + 360
                        || lowLayerHue <= keyHue + _data.HueTolerance) )
		    { return highLayerColor; }
            if (keyHue + _data.HueTolerance >= 360 //high end key overflow
                     && lowLayerHue >= keyHue - _data.HueTolerance
                     && lowLayerHue <= keyHue + _data.HueTolerance - 360) 
		    { return highLayerColor; }
		    return lowLayerColor;  //hue check failed - return low layer color
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
		    using (var setup = new ChromaKeySetup(_data))
		    {
                if (setup.ShowDialog() != DialogResult.OK) return false;
                _data.LowerLimit = setup.LowerLimit;
                _data.UpperLimit = setup.UpperLimit;
                _data.KeyColor = setup.KeyColor;
                _data.HueTolerance = setup.HueTolerance;
                _data.SaturationTolerance = setup.SaturationTolerance;
                return true;
            }			
		}
	}	
}