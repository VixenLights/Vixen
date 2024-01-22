using System.Collections.Concurrent;
using System.Diagnostics;

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
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="redraw">Delegate to redraw the preview</param>
		public MovingHeadIntentHandler(Action redrawPreviewPreview)
		{
			// Default the beam to Off
			DefaultBeamColor = Color.Transparent;

			// Store off the preview redraw delegate
			_redrawPreview = redrawPreviewPreview;
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Clears all strobe timers.
		/// <remarks>This method should be called after editing the preview</remarks>
		/// </summary>
		static public void ResetStrobeTimers()
		{
			// Create a new timer dictionary
			_timerDictionary = new ConcurrentDictionary<int, ConcurrentDictionary<int, MovingHeadTimer>>();
		}

		#endregion

		#region Private Static Fields

		/// <summary>
		/// Dictionary of moving head strobe timers.
		/// The first key is the strobe duration in ms.
		/// The second or inner dictionary key is the timer interval in ms.
		/// </summary>
		private static ConcurrentDictionary<int, ConcurrentDictionary<int, MovingHeadTimer>> _timerDictionary;

		#endregion

		#region Fields

		/// <summary>
		/// Delegate to redraw the preview.
		/// </summary>
		private Action _redrawPreview;

		/// <summary>
		/// Dictionary of command legend name value pairs.
		/// </summary>
		private Dictionary<string, string> _legendValues = new Dictionary<string, string>();

		/// <summary>
		/// Flag indicates that a strobe shutter index option was selected.
		/// This flag prevents automation from opening the shutter.
		/// </summary>
		private bool _strobeModeEnabled;

		/// <summary>
		/// This flag indicates that strobe shutter intents have been detected this frame.
		/// </summary>
		private bool _strobeIntentDetected;

		/// <summary>
		/// Flag indicates if color intents have been processed this frame.
		/// </summary>
		private bool _colorPresent;

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

		/// <summary>
		/// The interval in ms between strobe pulses.
		/// </summary>
		private int _strobeInterval;

		/// <summary>
		/// Reference to the active moving head strobe timer.
		/// </summary>
		private MovingHeadTimer _movingHeadStrobeTimer;

		#endregion

		#region Private Constants

		/// <summary>
		/// Max time in ms that the strobe should be illuminated.
		/// </summary>
		private const int MaxStrobeDuration = 50;

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
		/// Strobe Rate Minimum in Hz.
		/// </summary>
		public int StrobeRateMinimum
		{
			get;
			set;
		}

		/// <summary>
		/// Strobe Rate Maximum in Hz.
		/// </summary>
		public int StrobeRateMaximum
		{
			get;
			set;
		}

		/// <summary>
		/// Maximum strobe duration in ms.
		/// </summary>
		public int MaximumStrobeDuration
		{
			get;
			set;
		}

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

			// Clear out the fixture intensity
			MovingHead.Intensity = 0;

			// Reset whether color was detected
			_colorPresent = false;

			// Reset the flag indicating a strobe intent was detected
			_strobeIntentDetected = false;

			// If we are not in strobe mode then...
			if (!_strobeModeEnabled)
			{
				// Turn off the moving head beam
				MovingHead.OnOff = false;
			}

			// Set the beam color of the moving head back to the default
			MovingHead.BeamColorLeft = DefaultBeamColor;
			MovingHead.BeamColorRight = DefaultBeamColor;

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
		/// Determines the appropriate strobe timer for the moving head based on strobe rate.
		/// </summary>
		public void DetermineStrobeTimer()
		{
			// If there is not a timer associated with the moving head then...
			if (_movingHeadStrobeTimer == null)
			{
				// Create or register with an existing timer
				CreateOrRegisterWithTimer();
			}
			else
			{
				// Otherwise if existing timer's interval does NOT match the fixture's strobe rate then...
				if (_movingHeadStrobeTimer.Interval != _strobeInterval)
				{
					// Remove the moving head from the previous timer
					_movingHeadStrobeTimer.RemoveMovingHead(new Tuple<IMovingHead, Action>(MovingHead, DetermineStrobeTimer));

					// Create or register with an existing timer based on the strobe interval
					CreateOrRegisterWithTimer();
				}
			}
		}

		/// <summary>
		/// Allows the moving head intent handler to examine all of the intents
		/// received this frame to determine if the fixture should strobe.
		/// </summary>
		public void FinalizeStrobeState()
		{
			// If strobe intents and color has been detected then...
			if (_strobeIntentDetected && _colorPresent)
			{
				// Enable strobe mode
				_strobeModeEnabled = true;

				// Configure the strobe rate in ms
				_strobeInterval = MovingHead.StrobeRate;

				// If this moving head does NOT have a strobe timer then...
				if (_movingHeadStrobeTimer == null)
				{
					// Create or register with a moving head strobe timer 
					DetermineStrobeTimer();
				}
			}
			// If a strobe intent or color intent is NOT present then...
			else if (!_strobeIntentDetected || !_colorPresent)
			{
				// Disable strobe mode
				_strobeModeEnabled = false;

				// If the fixture was previously in strobe mode then...
				if (_movingHeadStrobeTimer != null)
				{
					// Disassociate the moving head with the timer 
					_movingHeadStrobeTimer.RemoveMovingHead(new Tuple<IMovingHead, Action>(MovingHead, DetermineStrobeTimer));

					// Clear out the reference to the timer
					_movingHeadStrobeTimer = null;
				}
			}
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

				// Increment the frame counter
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

			// Update the legend if applicable
			UpdateLegend(rangeIntent);
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

			// Extract the color from the discrete value
			Color color = discreteValue.Color;

			// Set the beam color of the moving head
			MovingHead.BeamColorLeft = color;
			MovingHead.BeamColorRight = color;

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
			MovingHead.BeamColorLeft = lightingIntent.GetValue().Color;
			MovingHead.BeamColorRight = lightingIntent.GetValue().Color;

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
			MovingHead.BeamColorLeft = lightingIntent.GetValue().FullColor;
			MovingHead.BeamColorRight = lightingIntent.GetValue().FullColor;

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

			// If the intent is a named commmand then...
			if (taggedCommand != null)
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
				// If not in strobe mode then...
				if (!_strobeModeEnabled)
				{
					// Turn on the moving head beam
					MovingHead.OnOff = true;
				}
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
				// Store off a flag that the fixture is in strobe mode so that the shutter
				// doesn't get automatically opened
				_strobeModeEnabled = true;

				// Store off the a strobe intent was received
				_strobeIntentDetected = true;

				// Retrieve the min and max strobe range constraints
				int min = taggedCommand.RangeMinimum;
				int max = taggedCommand.RangeMaximum;

				// Convert fixture strobe constraints from Hz to ms
				double fixtureMinimumInMs = 1.0 / StrobeRateMinimum * 1000;
				double fixtureMaximumInMs = 1.0 / StrobeRateMaximum * 1000;

				// Retrieve the strobe rate from the intent command
				double sRate = (double)taggedCommand.CommandValue;
			
				// Determine how far away from the minimum the rate is
				double distanceFromMinimum = sRate - min;

				// Determine penetration into the range as percent (0-1)
				double penetrationRatio =  distanceFromMinimum / (max - min);

				// Apply the ratio to strobe rate range
				double strobeRateMs = penetrationRatio * (fixtureMinimumInMs - fixtureMaximumInMs) + fixtureMaximumInMs;

				// Flip things around since the maximum strobe rate is really the smaller number
				MovingHead.StrobeRate = (int)(fixtureMinimumInMs + fixtureMaximumInMs) - (int)strobeRateMs;
			}
			
			//TODO: Better place to put this code?
			MovingHead.BeamLength = (int)BeamLength;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Creates or re-uses an existing moving head strobe timer.
		/// </summary>
		/// <param name="strobeRate">Interval of the strobe rate in ms</param>
		/// <param name="maxStrobeDuration">Maximum strobe duration in ms</param>
		private void CreateMovingHeadTimer(int strobeRate, int maxStrobeDuration)
		{
			// Create a new moving head timer
			MovingHeadTimer timer = new MovingHeadTimer(_strobeInterval, maxStrobeDuration, _redrawPreview);

			// If successful adding the timer to the dictionary then...
			if (_timerDictionary[MaximumStrobeDuration].TryAdd(_strobeInterval, timer))
			{
				// Associate the moving head with the timer
				timer.AddMovingHead(new Tuple<IMovingHead, Action>(MovingHead, DetermineStrobeTimer));
			}
			// Otherwise retrieve the existing timer based on the strobe interval
			else
			{
				// Associate the moving head with the timer
				_timerDictionary[MaximumStrobeDuration][_strobeInterval].AddMovingHead(new Tuple<IMovingHead, Action>(MovingHead, DetermineStrobeTimer));
			}

			// Store off the reference to the timer
			_movingHeadStrobeTimer = _timerDictionary[MaximumStrobeDuration][_strobeInterval];
		}

		/// <summary>
		/// Registers a moving head with a shared strobe timer.
		/// </summary>
		private void RegisterWithExistingTimer()
		{
			// Associate the moving head with the strobe timer
			_timerDictionary[MaximumStrobeDuration][_strobeInterval].AddMovingHead(new Tuple<IMovingHead, Action>(MovingHead, DetermineStrobeTimer));

			// Store off the reference to the timer
			_movingHeadStrobeTimer = _timerDictionary[MaximumStrobeDuration][_strobeInterval];
		}

		/// <summary>
		/// Creates or registers a moving head with an existing strobe timer.
		/// </summary>
		private void CreateOrRegisterWithTimer()
		{
			// If a dictionary does not exist for the specified strobe duration then...
			if (!_timerDictionary.ContainsKey(MaximumStrobeDuration))
			{
				// Create the internal dictionary for the specified strobe duration
				_timerDictionary.TryAdd(MaximumStrobeDuration, new ConcurrentDictionary<int, MovingHeadTimer>());
			}

			// If an existing strobe rate timer exists then...
			if (_timerDictionary.ContainsKey(_strobeInterval))
			{
				// Register the moving head with an existing strobe rate timer
				RegisterWithExistingTimer();
			}
			// Otherwise a strobe rate timer does NOT exist for the specified strobe interval
			else
			{
				// Create a new strobe timer for the specified interval
				CreateMovingHeadTimer(_strobeInterval, MaximumStrobeDuration);
			}
		}

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
																							 !clr.UseCurve);

			// If NOT null display the color wheel color
			if (wheelEntry != null)
			{
				// Set the beam color to the color wheel color
				MovingHead.BeamColorLeft = wheelEntry.Color1;
				MovingHead.BeamColorRight = wheelEntry.Color2;

				// Remember that we have color intents
				_colorPresent = true;
			}
			else
			{
				// Spin the color wheel
				SpinColorWheel(colorWheelFunction);
			}

			// If color is present then...
			if (_colorPresent)
			{
				// Open the fixture's shutter
				OpenShutter();
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

					// If the entry does not use a curve then...
					if (!entry.UseCurve)
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
			MovingHead.BeamColorLeft = _colorWheelEntry.Color1;
			MovingHead.BeamColorRight = _colorWheelEntry.Color2;

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

			// If the fixture was configured to convert color intents into shutter intents then...
			if (ConvertColorIntentsIntoShutter && !_strobeModeEnabled)
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
		/// Updates the legend for the specified range intent.
		/// </summary>
		/// <param name="rangeIntent">Range intent to extract the label from</param>
		private void UpdateLegend(IIntentState<RangeValue<FunctionIdentity>> rangeIntent)
		{
			// Call the common method to update the legend
			UpdateLegend(rangeIntent.GetValue().Label, ((int)(rangeIntent.GetValue().Value * 255)).ToString());
		}

		/// <summary>
		/// Updates the legend with the specified index command.
		/// </summary>
		/// <param name="taggedCommand">Named fixture command</param>
		private void UpdateLegend(Named8BitCommand<FixtureIndexType> taggedCommand)
		{
			// Call the common method to update the legend
			UpdateLegend(taggedCommand.Label, taggedCommand.CommandValue.ToString());
		}

		/// <summary>
		/// Updates the legend for the specified label and associated value.
		/// </summary>
		/// <param name="label">Label or function prefix</param>
		/// <param name="value">DMX value of the function</param>
		private void UpdateLegend(string label, string value)
		{
			// If the label is populated then...
			if (!string.IsNullOrEmpty(label))
			{
				// If the legend dictionary does NOT contain the function label 
				if (!_legendValues.ContainsKey(label))
				{
					// Add the function label and value to the dictionary
					_legendValues.Add(label, value);
				}
				else
				{
					// Otherwise update the function value in the dictionary
					_legendValues[label] = value;
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
		}

		#endregion		
	}
}
