using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;

namespace VixenModules.OutputFilter.DimmingFilter.Filters
{
	/// <summary>
	/// Fixture filter specific to dimmer functions.
	/// This filter handles color intents to help extract out the intensity.
	/// </summary>
	public class DimmingFilter : TaggedFilter.Filters.TaggedFilter
	{
		#region Public Properties

		/// <summary>
		/// Determines if colors are converted into dimming intents.
		/// </summary>
		public bool ConvertColorIntensityIntoDimIntents { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Handle(IIntentState<DiscreteValue> obj)
		{			
			// Handle the discrete color value
			IntentValue = ConvertDiscreteIntentToDimIntent(obj);			
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Handle(IIntentState<RGBValue> obj)
		{			
			// Handle the RGB color value
			IntentValue = ConvertRGBToDimIntent(obj);			
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Handle(IIntentState<LightingValue> obj)
		{		
			// Handle the lighting color value
			IntentValue = ConvertLightingValueToDimIntent(obj);		
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Converts discrete intent into a dimming intent.
		/// </summary>
		/// <param name="discreteIntent">Discrete intent to convert</param>
		/// <returns>Dimming intent</returns>
		private IIntentState ConvertDiscreteIntentToDimIntent(IIntentState<DiscreteValue> discreteIntent)
		{
			// Get the discrete value from the intent
			DiscreteValue discreteValue = discreteIntent.GetValue();

			// Get the intensity from the discrete value
			double intensity = discreteValue.Intensity;

			// Convert the intensity into a dimmer intent
			return ConvertIntensityToDimIntent(intensity);
		}

		/// <summary>
		/// Converts the specified intensity (0-1.0) to a dimmer intent.
		/// </summary>
		/// <param name="intensity">Color intensity (0-1.0)</param>		
		/// <returns>Dimming intent</returns>
		private IIntentState ConvertIntensityToDimIntent(double intensity)
		{
			// Convert the intensity from 0-1 to 0-255
			int integerIntensity = (int)(intensity * 255.0);

			// Declare a new range value
			RangeValue<FunctionIdentity> dimRangeValue;

			// If converting the color intensity into dimmer commands then...
			if (ConvertColorIntensityIntoDimIntents)
			{
				// Convert the intensity from 0-1 to 0-255
				dimRangeValue = new RangeValue<FunctionIdentity>(FunctionIdentity.Dim, Tag, intensity);
			}
			else
			{
				// Otherwise we want to activate the dimmer channel at 100%
				dimRangeValue = new RangeValue<FunctionIdentity>(FunctionIdentity.Dim, Tag, 1.0);
			}

			// Wrap the dim range value in an intent
			IIntentState<RangeValue<FunctionIdentity>> commandIntentState = new StaticIntentState<RangeValue<FunctionIdentity>>(dimRangeValue);

			// Return the dimmer command
			return commandIntentState;
		}

		/// <summary>
		/// Converts RGB intent into a dimming intent.
		/// </summary>
		/// <param name="rgbIntent">RGB intent to convert</param>
		/// <returns>Dimming intent</returns>
		private IIntentState ConvertRGBToDimIntent(IIntentState<RGBValue> rgbIntent)
		{
			// Get the RGBValue from the intent
			RGBValue theRGBValue = rgbIntent.GetValue();

			// Get the intensity from the RGB value
			double intensity = theRGBValue.Intensity;

			// Convert the intensity into a dimming intent
			return ConvertIntensityToDimIntent(intensity);
		}

		/// <summary>
		/// Converts lighting value intent into a dimming intent.
		/// </summary>
		/// <param name="rgbIntent">Lighting intent to convert</param>
		/// <returns>Dimming intent</returns>
		private IIntentState ConvertLightingValueToDimIntent(IIntentState<LightingValue> lightingIntent)
		{
			// Get the lighting value from the intent
			LightingValue lightingValue = lightingIntent.GetValue();

			// Get the intensity from the lighting value
			double intensity = lightingValue.Intensity;

			// Convert the intensity into a dimming intent
			return ConvertIntensityToDimIntent(intensity);
		}

		#endregion
	}
}