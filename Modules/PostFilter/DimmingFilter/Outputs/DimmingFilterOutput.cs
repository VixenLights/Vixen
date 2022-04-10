using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;
using VixenModules.OutputFilter.TaggedFilter.Outputs;


namespace VixenModules.OutputFilter.DimmingFilter.Outputs
{
    /// <summary>
    /// Dimming filter output.  This output contains special conversion logic for lamp fixtures.
    /// This output extract the intensity from color intents and creates dimming intents.
    /// </summary>
    public class DimmingFilterOutput : TaggedFilterOutputBase<Filters.DimmingFilter>
	{
		#region Fields

		/// <summary>
		/// Flag determines if the output converts color intensity into dim intents
		/// otherwise it is going to set the dim intent to full intensity.
		/// </summary>
		private bool _convertColorIntensityIntoDimIntents;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tag">Tag (function name) of the dimmer function</param>
		/// <param name="convertColorIntensityIntoDimIntents">True when color intents should be converted to dimming intents</param>
		public DimmingFilterOutput(string tag, bool convertColorIntensityIntoDimIntents) : base(tag)			
		{
			// Store off whether to convert color intensity into dimmer intents
			_convertColorIntensityIntoDimIntents = convertColorIntensityIntoDimIntents;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Converts discrete intent into a dimming intent.
		/// </summary>
		/// <param name="discreteIntent">Discrete intent to convert</param>
		private void ConvertDiscreteIntentToDimIntent(IIntentState<DiscreteValue> discreteIntent)
		{
			// Get the discrete value from the intent
			DiscreteValue discreteValue = discreteIntent.GetValue();

			// Get the intensity from the discrete value
			double intensity = discreteValue.Intensity;

			// Convert the intensity into a dimmer intent
			ConvertIntensityToDimIntent(intensity, discreteIntent.Layer);
		}

		/// <summary>
		/// Converts the specified intensity (0-1.0) to a dimmer intent.
		/// </summary>
		/// <param name="intensity">Color intensity (0-1.0)</param>
		/// <param name="layer">Layer of the original color intent</param>
		private void ConvertIntensityToDimIntent(double intensity, ILayer layer)
		{
			// Convert the intensity from 0-1 to 0-255
			int integerIntensity = (int)(intensity * 255.0);

			// Declare a new range value
			RangeValue<FunctionIdentity> dimRangeValue;

			// If converting the color intensity into dimmer commands then...
			if (_convertColorIntensityIntoDimIntents)
			{
				// Convert the intensity from 0-1 to 0-255
				dimRangeValue = new RangeValue<FunctionIdentity>(FunctionIdentity.Dim, Filter.Tag, intensity);
			}
			else
			{
				// Otherwise we want to activate the dimmer channel at 100%
				dimRangeValue = new RangeValue<FunctionIdentity>(FunctionIdentity.Dim, Filter.Tag, 1.0);
			}

			// Wrap the dim range value in an intent
			IIntentState<RangeValue<FunctionIdentity>> commandIntentState = new StaticIntentState<RangeValue<FunctionIdentity>>(dimRangeValue, layer);

			// Add the intent to the outputs
			IntentData.Value.Add(commandIntentState);
		}

		/// <summary>
		/// Converts RGB intent into a dimming intent.
		/// </summary>
		/// <param name="rgbIntent">RGB intent to convert</param>
		private void ConvertRGBToDimIntent(IIntentState<RGBValue> rgbIntent)
		{
			// Get the RGBValue from the intent
			RGBValue theRGBValue = rgbIntent.GetValue();

			// Get the intensity from the RGB value
			double intensity = theRGBValue.Intensity;

			// Convert the intensity into a dimming intent
			ConvertIntensityToDimIntent(intensity, rgbIntent.Layer);
		}

		/// <summary>
		/// Converts lighting value intent into a dimming intent.
		/// </summary>
		/// <param name="rgbIntent">Lighting intent to convert</param>
		private void ConvertLightingValueToDimIntent(IIntentState<LightingValue> lightingIntent)
		{
			// Get the lighting value from the intent
			LightingValue lightingValue = lightingIntent.GetValue();

			// Get the intensity from the lighting value
			double intensity = lightingValue.Intensity;

			// Convert the intensity into a dimming intent
			ConvertIntensityToDimIntent(intensity, lightingIntent.Layer);
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Process the input intent.
		/// </summary>
		/// <param name="intent">Intent to process</param>
		protected override void ProcessInputDataInternal(IIntentState intent)
		{
			// If the intent is a discrete color then...
			if (intent is IIntentState<DiscreteValue>)
			{
				// Convert the discrete color into a dimming intent
				ConvertDiscreteIntentToDimIntent((IIntentState<DiscreteValue>)intent);
			}
			// Otherwise if the intent is an RGB value then...
			else if (intent is IIntentState<RGBValue>)
			{
				// Convert the RGB color into a dimming intent
				ConvertRGBToDimIntent((IIntentState<RGBValue>)intent);
			}
			else if (intent is IIntentState<LightingValue>)
			{
				// Convert the  color into a dimming intent
				ConvertLightingValueToDimIntent((IIntentState<LightingValue>)intent);
			}
			else
			{
				// Otherwise just add the intent to the output states
				IntentData.Value.Add(intent);
			}
		}
		
		#endregion
	}
}
