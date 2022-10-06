using Common.Controls.ColorManagement.ColorModels;
using System;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	/// <summary>
	/// This filter gets the intensity percent for a given state for non mixing colors
	/// </summary>
	internal class ColorBreakdownFilter : IntentStateDispatch, IBreakdownFilter
	{
		private double _intensityValue;
		private readonly ColorBreakdownItem _breakdownItem;
		private const double Tolerance = .0001; //For how close the Hue and Saturation should match for Discrete.

		public ColorBreakdownFilter(ColorBreakdownItem breakdownItem)
		{
			_breakdownItem = breakdownItem;
		}

		/// <summary>
		/// Process the intent and return a value that represents the percent of intensity for the state
		/// </summary>
		/// <param name="intentValue"></param>
		/// <returns></returns>
		public double GetIntensityForState(IIntentState intentValue)
		{
			_intensityValue = 0;
			intentValue.Dispatch(this);
			return _intensityValue;
		}

		public override void Handle(IIntentState<RGBValue> obj)
		{
			RGBValue value = obj.GetValue();
			
			// if we're not mixing colors, we need to compare the input color against the filter color -- but only the
			// hue and saturation components; ignore the intensity.
			if (Math.Abs(value.FullColor.R - _breakdownItem.Color.R) < Tolerance &&
			        Math.Abs(value.FullColor.G - _breakdownItem.Color.G) < Tolerance &&
			        Math.Abs(value.FullColor.B - _breakdownItem.Color.B) < Tolerance)
			{
				var i = HSV.VFromRgb(value.FullColor);
				//the value types are structs, so modify our copy and then set it back
				_intensityValue = i;
			}
			
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			LightingValue lightingValue = obj.GetValue();
			
			// if we're not mixing colors, we need to compare the input color against the filter color -- but only the
			// hue and saturation components; ignore the intensity.
			if (Math.Abs(lightingValue.Color.R - _breakdownItem.Color.R) < Tolerance &&
				Math.Abs(lightingValue.Color.G - _breakdownItem.Color.G) < Tolerance &&
				Math.Abs(lightingValue.Color.B - _breakdownItem.Color.B) < Tolerance) {
				_intensityValue = lightingValue.Intensity;
			}
		}

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			DiscreteValue discreteValue = obj.GetValue();

			// if we're not mixing colors, we need to compare the input color against the filter color -- but only the
			// hue and saturation components; ignore the intensity.
			if (discreteValue.Color.ToArgb() == _breakdownItem.Color.ToArgb())
			{
				_intensityValue = discreteValue.Intensity;
			}
		}
	}
}
