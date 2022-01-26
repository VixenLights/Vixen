using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	/// <summary>
	/// This filter gets the intensity percent for a given state in simple RGB mixing
	/// </summary>
	internal class ColorBreakdownMixingFilter : IntentStateDispatch, IBreakdownFilter
	{
		private double _intensityValue;
		
		public ColorBreakdownMixingFilter(ColorBreakdownItem breakdownItem)
		{
			if (breakdownItem.Color.Equals(Color.Red))
			{
				_getMaxProportionFunc = color => color.R / 255f;
			}
			else if(breakdownItem.Color.Equals(Color.Lime))
			{
				_getMaxProportionFunc = color => color.G / 255f;
			}
			else
			{
				_getMaxProportionFunc = color => color.B / 255f;
			}
		}

		/// <summary>
		/// Process the intent and return a value that represents the percent of intensity for the state
		/// </summary>
		/// <param name="intentValue"></param>
		/// <returns></returns>
		public double GetIntensityForState(IIntentState intentValue)
		{
			intentValue.Dispatch(this);
			return _intensityValue;
		}

		private readonly Func<Color, float> _getMaxProportionFunc;

		public override void Handle(IIntentState<RGBValue> obj)
		{
			RGBValue value = obj.GetValue();
			_intensityValue = _getMaxProportionFunc(value.FullColor);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			LightingValue lightingValue = obj.GetValue();
			_intensityValue = lightingValue.Intensity * _getMaxProportionFunc(lightingValue.Color);
		}
	}
}
