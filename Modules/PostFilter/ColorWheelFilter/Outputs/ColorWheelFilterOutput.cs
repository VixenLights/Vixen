using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vixen.Data.Evaluator;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;
using VixenModules.App.Fixture;
using VixenModules.OutputFilter.TaggedFilter.Outputs;
using ICommand = Vixen.Commands.ICommand;

namespace VixenModules.OutputFilter.ColorWheelFilter.Outputs
{
	/// <summary>
	/// Color wheel filter output.  This output contains special conversion logic for lamp fixtures.
	/// </summary>
	public class ColorWheelFilterOutput : TaggedFilterOutputBase<Filters.ColorWheelFilter>
	{
		#region Fields

		/// <summary>
		/// Color wheel data.  This property allows the output to know what colors are supported by the color wheel.
		/// </summary>
		private List<FixtureColorWheel> _colorWheelData;

		/// <summary>
		/// Flag indicating if the output should convert RGB colors into color wheel commands.
		/// </summary>
		private bool _convertRBGIntoColorWheel;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tag">Tag applicable to the color wheel</param>
		/// <param name="colorWheelData">Color wheel data</param>
		/// <param name="convertRBGIntoColorWheel">True when the output should convert RGB into color wheel index command</param>
		public ColorWheelFilterOutput(
			string tag, 
			List<FixtureColorWheel> colorWheelData,
			bool convertRBGIntoColorWheel) : base(tag)
		{
			// Save off the color wheel data
			_colorWheelData = colorWheelData;

			// Save off whether to convert RGB into color index commands
			_convertRBGIntoColorWheel = convertRBGIntoColorWheel;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Converts a discrete color intent into an index command intent.
		/// </summary>
		/// <param name="discreteIntent">Discrete color to convert to an index command</param>
		private void ConvertDiscreteColorToIndexCommand(IIntentState<DiscreteValue> discreteIntent)
		{
			// Get the discrete value from the intent
			DiscreteValue discreteValue = discreteIntent.GetValue();

			// Get the Full color from the discrete value
			Color color = discreteValue.FullColor;

			// Convert the color into a color wheel index command intent
			ConvertColorToIndexCommand(color, discreteIntent.Layer);
		}

		/// <summary>
		/// Converts the specified color into a color wheel index command
		/// </summary>
		/// <param name="color">Color to convert</param>
		/// <param name="layer">Layer associated with the intent</param>
		private void ConvertColorToIndexCommand(Color color, ILayer layer)
		{
			// Attempt to find the Full color in the color wheel
			FixtureColorWheel colorWheelItem = _colorWheelData.FirstOrDefault(
				colorWheelEntry => colorWheelEntry.Color1 == color &&
			           !colorWheelEntry.HalfStep);

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
				IIntentState<CommandValue> commandIntentState =
					new StaticIntentState<CommandValue>(commandValue, layer);

				// Add the command intent to the States associated with the output 
				IntentData.Value.Add(commandIntentState);
			}
		}

		/// <summary>
		/// Converts the RGB intent into a color wheel index command intent.
		/// </summary>
		/// <param name="rgbIntent">RGB intent to convert</param>
		private void ConvertRGBToIndexCommand(IIntentState<RGBValue> rgbIntent)
		{ 
			// Get the RGBValue from the intent
			RGBValue theRGBValue = rgbIntent.GetValue();
			
			// Get the full color from the RGBValue
			Color color = theRGBValue.FullColor;

			// Convert the color into a color wheel index command intent
			ConvertColorToIndexCommand(color, rgbIntent.Layer);
		}

		/// <summary>
		/// Converts Lighting intent into a color wheel index command intent.
		/// </summary>
		/// <param name="intent">Lighting intent to convert</param>
		private void ConvertLightingValueToIndexCommand(IIntentState<LightingValue> intent)
		{		
			// Get the lighting value from the intent
			LightingValue lightingValue = intent.GetValue();

			// Get the full color from the lighting value
			Color color = lightingValue.FullColor;

			// Convert the color into a color wheel index command intent
			ConvertColorToIndexCommand(color, intent.Layer);
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Process the input intent.
		/// </summary>
		/// <param name="intent">Intent to process</param>
		protected override void ProcessInputDataInternal(IIntentState intent)
		{
			// If the translating the RGB color into color wheel commands then...
			if (_convertRBGIntoColorWheel)
			{
				// If the intent is a discrete value then...
				if (intent is IIntentState<DiscreteValue>)
				{
					// Convert the discrete color into a color wheel index command
					ConvertDiscreteColorToIndexCommand((IIntentState<DiscreteValue>) intent);
				}
				// Otherwise if the intent is an RGB value then...
				else if (intent is IIntentState<RGBValue>)
				{
					// Convert the RGB color into a color wheel index command
					ConvertRGBToIndexCommand((IIntentState<RGBValue>) intent);
				}
				// Otherwise if the intent is a lighting value then...
				else if (intent is IIntentState<LightingValue>)
				{
					// Convert the lighting value color into a color wheel index command
					ConvertLightingValueToIndexCommand((IIntentState<LightingValue>) intent);
				}
				else
				{
					// Call the base class processing
					base.ProcessInputDataInternal(intent);
				}
			}
			else
			{
				// Call the base class processing				
				base.ProcessInputDataInternal(intent);
			}
		}

		#endregion
	}
}
