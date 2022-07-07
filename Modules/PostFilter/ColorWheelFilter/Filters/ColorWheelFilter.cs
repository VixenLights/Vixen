using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vixen.Commands;
using Vixen.Data.Evaluator;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using VixenModules.App.Fixture;

namespace VixenModules.OutputFilter.ColorWheelFilter.Filters
{
    /// <summary>
    /// Maintains a color wheel filter.
    /// </summary>
    public class ColorWheelFilter : TaggedFilter.Filters.TaggedFilter
	{
        #region Public Properties

        /// <summary>
        /// Color wheel data.  This property allows the output to know what colors are supported by the color wheel.
        /// </summary>
        public List<FixtureColorWheel> ColorWheelData { get; set; }

		/// <summary>
		/// Flag indicating if the output should convert color intents into color wheel commands.
		/// </summary>
		public bool ConvertColorIntentsIntoColorWheel { get; set; }

		#endregion

		#region Private Methods

		/// <summary>
		/// Converts a discrete color intent into an index command intent.
		/// </summary>
		/// <param name="discreteIntent">Discrete color to convert to an index command</param>
		private IIntentState ConvertDiscreteColorToIndexCommand(IIntentState<DiscreteValue> discreteIntent)
		{
			// Get the discrete value from the intent
			DiscreteValue discreteValue = discreteIntent.GetValue();

			// Get the Full color from the discrete value
			Color color = discreteValue.FullColor;

			// Convert the color into a color wheel index command intent
			return ConvertColorToIndexCommand(color);
		}

		/// <summary>
		/// Converts the specified color into a color wheel index command
		/// </summary>
		/// <param name="color">Color to convert</param>
		/// <param name="layer">Layer associated with the intent</param>
		private IIntentState ConvertColorToIndexCommand(Color color)
		{
			// Attempt to find the Full color in the color wheel
			FixtureColorWheel colorWheelItem = ColorWheelData.FirstOrDefault(
				colorWheelEntry => colorWheelEntry.Color1 == color &&
					   !colorWheelEntry.HalfStep);

			IIntentState<CommandValue> commandIntentState = null;

			// If the color matches one of the color wheel colors then... 
			if (colorWheelItem != null)
			{
				// Get the index of the color on the color wheel
				byte index = (byte)colorWheelItem.StartValue;

				// Retrieve the command for the specified value
				ICommand command = CommandLookup8BitEvaluator.CommandLookup[index];

				// Create a command value, wrapping the 8 bit command
				CommandValue commandValue = new CommandValue(command);

				// Create a new intent wrapping the command
				commandIntentState = new StaticIntentState<CommandValue>(commandValue);				
			}

			// Return color wheel index command
			return commandIntentState;
		}

		/// <summary>
		/// Converts the RGB intent into a color wheel index command intent.
		/// </summary>
		/// <param name="rgbIntent">RGB intent to convert</param>
		private IIntentState ConvertRGBToIndexCommand(IIntentState<RGBValue> rgbIntent)
		{
			// Get the RGBValue from the intent
			RGBValue theRGBValue = rgbIntent.GetValue();

			// Get the full color from the RGBValue
			Color color = theRGBValue.FullColor;

			// Convert the color into a color wheel index command intent
			return ConvertColorToIndexCommand(color);
		}

		/// <summary>
		/// Converts Lighting intent into a color wheel index command intent.
		/// </summary>
		/// <param name="intent">Lighting intent to convert</param>
		private IIntentState ConvertLightingValueToIndexCommand(IIntentState<LightingValue> intent)
		{
			// Get the lighting value from the intent
			LightingValue lightingValue = intent.GetValue();

			// Get the full color from the lighting value
			Color color = lightingValue.FullColor;

			// Convert the color into a color wheel index command intent
			return ConvertColorToIndexCommand(color);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			// If converting color intents into color wheel commands then...
			if (ConvertColorIntentsIntoColorWheel)
			{
				// Handle the discrete color value
				IntentValue = ConvertDiscreteColorToIndexCommand(obj);
			}
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Handle(IIntentState<RGBValue> obj)
		{
			// If converting color intents into color wheel commands then...
			if (ConvertColorIntentsIntoColorWheel)
			{
				// Handle the RGB color value
				IntentValue = ConvertRGBToIndexCommand(obj);
			}
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void Handle(IIntentState<LightingValue> obj)
		{
			// If converting color intents into color wheel commands then...
			if (ConvertColorIntentsIntoColorWheel)
			{
				// Handle the lighting color value
				IntentValue = ConvertLightingValueToIndexCommand(obj);
			}
		}

		#endregion
	}
}
