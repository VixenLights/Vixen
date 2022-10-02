using System;
using System.Drawing;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	/// <summary>
	/// Abstract base class for mixing filters.
	/// These filters gets the intensity percent for a given state in simple color mixing.
	/// </summary>
	internal abstract class ColorBreakdownMixingFilterBase : IntentStateDispatch, IBreakdownFilter
	{
		#region Fields

		/// <summary>
		/// Intensity of the component color.
		/// </summary>
		private double _intensityValue;

		#endregion

		#region Protected Properties

		/// <summary>
		/// Gets or sets the delegate that returns the component color for the filter.
		/// </summary>
		protected Func<Color, float> GetMaxProportionFunc { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Process the intent and return a value that represents the percent of intensity for the state.
		/// </summary>
		/// <param name="intentValue">Intent to process</param>
		/// <returns>Intensity for the color component</returns>
		public double GetIntensityForState(IIntentState intentValue)
		{
			_intensityValue = 0;
			intentValue.Dispatch(this);
			return _intensityValue;
		}

		public override void Handle(IIntentState<RGBValue> obj)
		{
			RGBValue value = obj.GetValue();
			_intensityValue = GetMaxProportionFunc(value.FullColor);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			LightingValue lightingValue = obj.GetValue();
			_intensityValue = lightingValue.Intensity * GetMaxProportionFunc(lightingValue.Color);
		}

		#endregion
	}
}
