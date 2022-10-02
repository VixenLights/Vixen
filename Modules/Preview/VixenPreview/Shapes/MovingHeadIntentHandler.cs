using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;
using VixenModules.App.Fixture;
using VixenModules.Editor.FixtureGraphics;
using VixenModules.Property.IntelligentFixture;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	/// <summary>
	/// Dispatches and handles intents for a moving head preview shape.
	/// </summary>
	public class MovingHeadIntentHandler : IntentStateDispatch,
		IHandler<IIntentState<RangeValue<FunctionIdentity>>>
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public MovingHeadIntentHandler()
		{
			// Default the beam to Off
			DefaultBeamColor = Color.Transparent;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Dictionary of command legend name value pairs.
		/// </summary>
		private Dictionary<string, string> _legendValues = new Dictionary<string, string>();

		/// <summary>
		/// Flag indicates that a strobbing shutter index option was selected.
		/// This flag prevents automation from opening the shutter.
		/// </summary>
		private bool _strobbing;

		/// <summary>
		/// Flag indicates if color intents have been processed this frame.
		/// </summary>
		private bool _colorPresent;

		/// <summary>
		/// Flag that indicates the beam should be turned on because of strobing as long as color intents are present.
		/// </summary>
		private bool _strobingOn;

		/// <summary>
		/// Keeps track of the current color wheel slot when spinning the color wheel.
		/// </summary>
		private int _colorWheelSlot = -1;
		
		/// <summary>
		/// Frame counter to use for spinning the color wheel or strobing.
		/// </summary>
		private int _frameCounter = 0;

		/// <summary>
		/// Current color wheel entry being displayed when spinning the color wheel.
		/// </summary>
		private FixtureColorWheel _colorWheelEntry = null;

		#endregion

		#region Public Properties

		/// <summary>
		/// Default beam color of the light beam when color intents are not being applied.
		/// </summary>
		public Color DefaultBeamColor { get; set; }

		/// <summary>
		/// Fixture node associated with the moving head preview shape. 
		/// </summary>
		public IElementNode Node { get; set; }

		/// <summary>
		/// Pan start angle in degrees.
		/// </summary>
		public int PanStartPosition
		{
			get;
			set;
		}

		/// <summary>
		/// Pan stop angle in degrees.
		/// </summary>
		public int PanStopPosition
		{
			get;
			set;
		}

		/// <summary>
		/// Tilt start position in degrees.
		/// </summary>
		public int TiltStartPosition
		{
			get;
			set;
		}

		/// <summary>
		/// Tilt stop position in degrees.
		/// </summary>
		public int TiltStopPosition
		{
			get;
			set;
		}

		/// <summary>
		/// Beam length factor in percent (0-100%).
		/// </summary>
		public int BeamLength
		{
			get;			
			set;			
		}

		/// <summary>
		/// Indicates if the fixture zooms from narrow to wide.
		/// </summary>
		public bool ZoomNarrowToWide
		{
			get;
			set;
		}

		/// <summary>
		/// Reverses the pan movement direction.
		/// </summary>
		public bool InvertPanDirection { get; set; }

		/// <summary>
		/// Reverses the tilt movement direction.
		/// </summary>
		public bool InvertTiltDirection { get; set; }
		
		/// <summary>
		/// Moving head settings associated with the preview shape.
		/// </summary>
		public IMovingHead MovingHead { get; set; }
		
		/// <summary>
		/// Controls whether color intents are converted into shutter index commands.
		/// </summary>
		public bool ConvertColorIntentsIntoShutter { get; set; }

		/// <summary>
		/// Controls whether color intents are converted into dimmer intents.
		/// </summary>
		public bool ConvertColorIntentsIntoDimmer { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Resets state between frames.
		/// </summary>
		public void Reset()
		{
			// Clear out the legend values
			_legendValues.Clear();
			MovingHead.Legend = String.Empty;

			// Turn off strobing
			_strobbing = false;

			// Reset whether color was detected
			_colorPresent = false;
			_strobingOn = false;

			// Turn off the moving head beam
			MovingHead.OnOff = false;

			// Set the beam color of the moving head back to the default
			MovingHead.BeamColor = DefaultBeamColor;

			// If the tilt movement direction is inverted then...
			if (InvertTiltDirection)
			{
				// Reset the head to the stop position
				MovingHead.TiltAngle = TiltStopPosition;
			}
			else
			{
				// Reset the head to the start position
				MovingHead.TiltAngle = TiltStartPosition;
			}

			// Reset the head to the start position
			MovingHead.PanAngle = PanStartPosition;
		}

		/// <summary>
		/// Dispatches intents to a specific handler method.
		/// </summary>
		/// <param name="states">Collection of IIntentState to dispatch</param>
		public void Dispatch(IIntentStates states)
		{
			// If there are valid intents then...
			if (states != null)
			{				
				// Loop over the intents
				foreach (IIntentState state in states)
				{
					// Dispatch the intents
					state.Dispatch(this);				
				}

				// Increemnt the frame counter
				IncrementFrameCounter();
			}
		}
				
		/// <summary>
		/// Handles range intents.
		/// </summary>
		/// <param name="positionIntent">Range function intent to handle</param>
		public override void Handle(IIntentState<RangeValue<FunctionIdentity>> rangeIntent)
		{
			// Determine which type of function is in the intent
			switch ((FunctionIdentity)rangeIntent.GetValue().TagType)
			{
				case FunctionIdentity.Pan:

					// If the pan movement direction is inverted then...
					if (InvertPanDirection)
					{
						// Calculate the pan angle
						double pan = PanStartPosition - rangeIntent.GetValue().Value * (PanStopPosition - PanStartPosition);

						// Limit the angle to +/- 360 degrees
						pan = LimitAngle(pan);

						// Set the moving head pan angle
						MovingHead.PanAngle = pan;
					}
					else
					{
						// Calculate the pan angle
						double pan = rangeIntent.GetValue().Value * (PanStopPosition - PanStartPosition) + PanStartPosition;

						// Limit the angle to +/- 360 degrees
						pan = LimitAngle(pan);

						// Set the moving head pan angle
						MovingHead.PanAngle = pan;
					}
					break;
				case FunctionIdentity.Tilt:

					// If the tilt movement direction is inverted then...
					if (InvertTiltDirection)
					{
						// Calculate the tilt angle
						double tilt = TiltStopPosition - rangeIntent.GetValue().Value * (TiltStopPosition - TiltStartPosition);

						// Limit the angle to +/- 360 degrees
						tilt = LimitAngle(tilt);

						// Set the moving head tilt angle converting to degrees
						MovingHead.TiltAngle = tilt;
					}
					else
					{
						// Calculate the tilt angle
						double tilt = rangeIntent.GetValue().Value * (TiltStopPosition - TiltStartPosition) + TiltStartPosition;

						// Limit the angle to +/- 360 degrees
						tilt = LimitAngle(tilt);

						// Set the moving head tilt angle converting to degrees
						MovingHead.TiltAngle = tilt;
					}
					break;
				case FunctionIdentity.Zoom:

					// If the fixture zooms from narrow to wide then...
					if (ZoomNarrowToWide)
					{
						MovingHead.Focus = (int)(rangeIntent.GetValue().Value * 100.0);
					}
					else
					{
						MovingHead.Focus = (int)((1.0 - rangeIntent.GetValue().Value) * 100.0);
					}
					break;
				case FunctionIdentity.Dim:
					// Set the intensity of the beam color
					// The value coming in is between 0.0 and 1.0					
					MovingHead.Intensity = (int)(rangeIntent.GetValue().Value * 100.0);
					break;
				case FunctionIdentity.Custom:
					// None of the custom functions are supported by the preview
					break;

				case FunctionIdentity.Gobo:
				case FunctionIdentity.Frost:
				case FunctionIdentity.Prism:
				case FunctionIdentity.OpenClosePrism:

				//
				// TODO: I DONT THINK THIS BELONGS HERE!
				//
				case FunctionIdentity.SpinColorWheel:
					// There is no support for these functions in the preview at this time.
					break;												
				default:
					Debug.Assert(false, "Unsupported Function Type");
					break;
			}
		}

		/// <summary>
		/// Handles discrete color intents.
		/// </summary>
		/// <param name="discreteIntent">Discrete intent to handle</param>
		public override void Handle(IIntentState<DiscreteValue> discreteIntent)
		{
			// Optionally open the shutter
			OpenShutter();
		
			// Get the discrete value from the intent
			DiscreteValue discreteValue = discreteIntent.GetValue();

			// Extract the full color from the discrete value
			Color color = discreteValue.FullColor;

			// Set the beam color of the moving head
			MovingHead.BeamColor = color;

			// If converting color into dimmer intents then...
			if (ConvertColorIntentsIntoDimmer)
			{
				// Set the intensity of the beam color
				// This code is necessary because the preview is prior to all the filters
				MovingHead.Intensity = (int)(discreteValue.Intensity * 100);
			}

			// Set the beam length taking into account a factor specified on the preview shape
			//TODO: Better place to put this code?
			MovingHead.BeamLength = (int)BeamLength;			
		}
		
		/// <summary>
		/// Handles lighting intents.
		/// </summary>
		/// <param name="lightingIntent">Lighting intent to handle</param>
		public override void Handle(IIntentState<LightingValue> lightingIntent)
		{
			// Optionally open the shutter
			OpenShutter();

			// Set the beam color of the moving head
			MovingHead.BeamColor = lightingIntent.GetValue().Color;

			// Set the intensity of the beam color
			MovingHead.Intensity = (int)(lightingIntent.GetValue().Intensity * 100);

			// Set the beam length taking into account a factor specified on the preview shape
			//TODO: Better place to put this code?
			MovingHead.BeamLength = (int)BeamLength;
		}

		/// <summary>
		/// Handles RGB lighting intents.
		/// </summary>
		/// <param name="lightingIntent">RGB lighting intents</param>
		public override void Handle(IIntentState<RGBValue> lightingIntent)
		{
			// Optionally open the shutter
			OpenShutter();

			// Set the beam color of the moving head
			MovingHead.BeamColor = lightingIntent.GetValue().FullColor;
				
			// Set the intensity of the beam color
			MovingHead.Intensity = (int)(lightingIntent.GetValue().Intensity * 100);

			// Set the beam length taking into account a factor specified on the preview shape
			//TODO: Better place to put this code?
			MovingHead.BeamLength = (int)BeamLength;
		}

		/// <summary>
		/// Handles tagged command intents.
		/// </summary>33
		/// <param name="commandIntent">Tagged command intent</param>
		public override void Handle(IIntentState<CommandValue> commandIntent)
		{
			// Attempt to cast the command intent to a named fixture index command
			Named8BitCommand<FixtureIndexType> taggedCommand = commandIntent.GetValue().Command as Named8BitCommand<FixtureIndexType>;

			// If the intent is a named commmand and
			// it has a populated label then...
			if (taggedCommand != null &&
				!string.IsNullOrEmpty(taggedCommand.Label))
			{
				// Update the legend on the moving head shape
				UpdateLegend(taggedCommand);				
			}
			
			// If the named command is the LampOff command or
			// shutter closed command then...
			if (((FixtureIndexType)taggedCommand.IndexType) == FixtureIndexType.LampOff ||
				((FixtureIndexType)taggedCommand.IndexType) == FixtureIndexType.ShutterClosed)
			{
				// Turn off the moving head beam
				MovingHead.OnOff = false;
			}
			// If the named command is the LampOn command or
			// shutter Open command then...
			else if (((FixtureIndexType)taggedCommand.IndexType) == FixtureIndexType.LampOn ||
					 ((FixtureIndexType)taggedCommand.IndexType) == FixtureIndexType.ShutterOpen)
			{
				// Turn on the moving head beam
				MovingHead.OnOff = true;
			}
			// If the command is a color wheel index command then...
			else if (((FixtureIndexType)taggedCommand.IndexType) == FixtureIndexType.ColorWheel)
			{
				// Handle the color wheel index command
				HandleColorWheelCommand(taggedCommand);
			}
			// Otherwise if the command is to strobe then...
			else if (((FixtureIndexType)taggedCommand.IndexType) == FixtureIndexType.Strobe)
			{				
				// Store off a flag that the fixture is strobing so that the shutter
				// doesn't get automatically opened
				_strobbing = true;

				// If this is an even frame then...
				if (_frameCounter % 2 == 0)
				{
					// If color intents were detected then...
					if (_colorPresent)
					{
						// Turn on the beam
						MovingHead.OnOff = true;
					}
					// Otherwise a color intent has not been detected so
					else
					{
						// Set a flag so that if a color intent is detected the beam is turned on
						_strobingOn = true;
					}
				}
				// Otherwise if this is an odd frame then...
				else
				{
					// Turn off the beam
					MovingHead.OnOff = false;
				}
			}

			//TODO: Better place to put this code?
			MovingHead.BeamLength = (int)BeamLength;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Increments the internal frame counter.
		/// </summary>
		private void IncrementFrameCounter()
		{
			// Increment the frame counter
			_frameCounter++;			
		}

		/// <summary>
		/// Handles color wheel index command.
		/// </summary>
		/// <param name="taggedCommand">Color wheel index command</param>
		private void HandleColorWheelCommand(Named8BitCommand<FixtureIndexType> taggedCommand)
		{
			// Retrieve the fixture property from the node
			IntelligentFixtureModule property = (IntelligentFixtureModule)Node.Properties.SingleOrDefault(prop => prop is IntelligentFixtureModule);

			// Find the color wheel function on the fixture
			FixtureFunction colorWheelFunction = property.FixtureSpecification.FunctionDefinitions.Single(func =>
				 (func.FunctionType == FixtureFunctionType.ColorWheel &&
				  func.Name == taggedCommand.Tag));

			// Find the color wheel entry for the command value
			// Ignore color wheel entries that are half step or curves
			FixtureColorWheel wheelEntry = colorWheelFunction.ColorWheelData.SingleOrDefault(clr => clr.StartValue == taggedCommand.CommandValue &&
																							 !clr.UseCurve && !clr.HalfStep);

			// If NOT null display the color wheel color
			if (wheelEntry != null)
			{
				// Set the beam color to the color wheel color
				MovingHead.BeamColor = wheelEntry.Color1;
			}
			else
			{
				// Check to see if the color wheel entry is a half step
				wheelEntry = colorWheelFunction.ColorWheelData.SingleOrDefault(clr => clr.StartValue == taggedCommand.CommandValue && !clr.UseCurve);

				// If the entry is not found then it must be a spin color wheel index
				if (wheelEntry == null)
				{
					// Spin the color wheel
					SpinColorWheel(colorWheelFunction);
				}
				else
				{
					// Preview does not support half steps currently so the workaround is just to show white
					MovingHead.BeamColor = Color.White;
				}
			}
		}

		/// <summary>
		/// Spin the color wheel using the specified color wheel function.
		/// </summary>
		/// <param name="colorWheelFunction">Color wheel function to spin</param>
		private void SpinColorWheel(FixtureFunction colorWheelFunction)
		{
			// Figure out how many frames to display each color
			// Attempting to display each color for a half second
			int framesPerColor = 500 / Vixen.Sys.VixenSystem.DefaultUpdateInterval;

			// If this is the first color wheel entry to spin OR
			// it is time to switch colors then...
			if (_colorWheelEntry == null || _frameCounter > framesPerColor)
			{
				// Reset the frame counter
				_frameCounter = 0;

				// Increment to the next color wheel entry
				_colorWheelSlot++;

				// If there are no more color wheel entries then...
				if (_colorWheelSlot > colorWheelFunction.ColorWheelData.Count - 1)
				{
					// Wrap around back to the first color wheel entry
					_colorWheelSlot = 0;
				}

				// Flag to indicate if a valid color wheel entry was found
				bool foundValidColor = false;

				// Loop until a valid color wheel entry is found
				do
				{
					// Get the current color wheel entry
					FixtureColorWheel entry = colorWheelFunction.ColorWheelData[_colorWheelSlot];

					// If the entry is NOT a half step AND
					// does not use a curve then...
					if (!entry.HalfStep && !entry.UseCurve)
					{
						// Indicate a valid color wheel entry was found
						foundValidColor = true;

						// Store off the found color wheel entry
						_colorWheelEntry = entry;
					}
					// Otherwise continue searching for valid color wheel entries
					else
					{
						// Increment the color wheel index
						_colorWheelSlot++;

						// If there are no more color wheel entries then...
						if (_colorWheelSlot > colorWheelFunction.ColorWheelData.Count - 1)
						{
							// Wrap around back to the first color wheel entry
							_colorWheelSlot = 0;
						}
					}
				}
				while (!foundValidColor);
			}

			// Set the beam color to match the color wheel entry
			MovingHead.BeamColor = _colorWheelEntry.Color1;

			// Turn on the beam
			MovingHead.OnOff = true;
		}
			
		/// <summary>
		/// Opens the fixture shutter if this automation was selected.
		/// </summary>
		private void OpenShutter()
		{
			// Remember that color was detected
			_colorPresent = true;

			// If the fixture was configured to conver color intents into shutter intents then...
			if (ConvertColorIntentsIntoShutter && !_strobbing)
			{
				// Turn on the beam
				MovingHead.OnOff = true;
			}

			// Since the strobe intent could come before the color intent;
			// If we are strobing and this frame is the On frame then...
			if (_strobbing && _strobingOn)
			{
				// Turn on the beam
				MovingHead.OnOff = true;
			}
		}

		/// <summary>
		/// Limits the specified angle to between -360 and +360.
		/// </summary>
		/// <param name="angle">Input angle in degrees</param>
		/// <returns>Limited angle between -360 and +360</returns>
		private double LimitAngle(double angle)
		{
			const double FullRotation = 360.0;

			// While the angle is greater than 360 then...
			while (angle > FullRotation)
			{
				// Subtract off 360 degrees
				angle -= FullRotation;
			}

			// While the angle is less than -360 then...
			while (angle < -FullRotation)
			{
				// Add 360 degrees
				angle += FullRotation;
			}

			// Return the limited angle
			return angle;
		}

		/// <summary>
		/// Updates the legend with the specified function.
		/// </summary>
		/// <param name="taggedCommand">Named fixture command</param>
		private void UpdateLegend(Named8BitCommand<FixtureIndexType> taggedCommand)
		{
			// If the legend dictionary does NOT contain the function label 
			if (!_legendValues.ContainsKey(taggedCommand.Label))
			{
				// Add the function label and value to the dictionary
				_legendValues.Add(taggedCommand.Label, taggedCommand.CommandValue.ToString());
			}
			else
			{
				// Otherwise update the function value in the dictionary
				_legendValues[taggedCommand.Label] = taggedCommand.CommandValue.ToString();
			}

			List<string> legendItems = new List<string>();

			// Loop over legend values in dictionary
			foreach (KeyValuePair<string, string> legendPair in _legendValues)
			{
				// Convert the legend name value pairs into a string
				legendItems.Add(legendPair.Key + legendPair.Value);
			}
			
			// Sort the legend items
			legendItems.Sort();

			// Assign the legend to the moving head shape
			MovingHead.Legend = string.Join(", ", legendItems);
		}

		#endregion		
	}
}
