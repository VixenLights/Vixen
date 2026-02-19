using System.ComponentModel;

using Common.Controls.ColorManagement.ColorModels;

using Vixen.Attributes;
using Vixen.Sys.Attribute;

using VixenModules.App.ColorGradients;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

using Color = System.Drawing.Color;

namespace VixenModules.Effect.Whirlpool
{
	/// <summary>
	/// Maintains a whirl.
	/// </summary>
	[ExpandableObject]
	public partial class Whirl : ExpandoObjectBase, IWhirl
	{
		#region Constructor
		
		/// <summary>
		/// Constructor
		/// </summary>
		public Whirl()
		{
			Enabled = true;
			//X - Set by parent
			//Y - Set by parent
			//Width - Set by parent
			//Height - Set by parent
			WhirlMode = WhirlpoolMode.RecurrentWhirls;
			TailLength = 10;
			StartLocation = WhirlpoolStartLocation.TopLeft;
			WhirlDirection = WhirlpoolDirection.In;
			Rotation = WhirlpoolRotation.Clockwise;
			ReverseDraw = false;
			Spacing = 1;
			Thickness = 1;
			Show3D = false;
			//XOffset - Set by parent 
			//YOffset - Set by parent
			ColorMode = WhirlpoolColorMode.GradientOverTime;
			BandLength = 10;
			LeftColor = new ColorGradient(Color.Blue);
			RightColor = new ColorGradient(Color.Yellow);
			TopColor = new ColorGradient(Color.Red);
			BottomColor = new ColorGradient(Color.Green);
			SingleColor = new ColorGradient(Color.Red);
			Colors = new List<ColorGradient> { new ColorGradient(Color.Red), new ColorGradient(Color.White) };
			Intensity = 1.0;
			//PauseAtEnd - Set by parent

			// Initialize the whirl vortex meta-data
			_vortexMetadata = new WhirlVortexMetadata();
		}

		#endregion

		#region Fields

		/// <summary>
		/// Number of frames the whirl we drawn during.
		/// </summary>
		private int _numberOfFrames;

		/// <summary>
		/// Total number of pixels that make up the whirl.
		/// </summary>
		private double _totalNumberOfPixels;

		/// <summary>
		/// Flag that indicates the end of whirl is going to drawn this frame.
		/// </summary>
		private bool _drawingTail;
		
		/// <summary>
		/// The number of additional pixels to draw each frame.
		/// </summary>
		private double _pixelsToDrawPerFrame;
		
		/// <summary>
		/// Number of whirl pixels to draw in the current frame.
		/// </summary>
		private double _numberOfPixelsToDraw;

		/// <summary>
		/// Number of frames between drawing symmetric rectangles.
		/// </summary>
		private int _framesPerRectangle;
		
		/// <summary>
		/// The number of symmetrical rectangles that can be drawn on the display element.
		/// </summary>
		private int _numberOfSymmetricRectangles;

		/// <summary>
		/// Index of the current color for drawing whirls.
		/// </summary>
		private int _colorIndex;

		/// <summary>
		/// Index of number of pixels drawn in a color band.
		/// </summary>
		private int _bandPixelCount;

		/// <summary>
		/// Internal copy of the ReverseDraw flag.
		/// This field is used support drawing with a direction of In & Out.
		/// </summary>
		private bool _internalReverseDraw;

		/// <summary>
		/// Maintains attributes about where the whirlpool vortex ends.
		/// </summary>
		private WhirlVortexMetadata _vortexMetadata;

		#endregion

		#region ICloneable

		///<inheritdoc\>
		public object Clone()
		{
			// Clone the whirl
			IWhirl result = new Whirl
			{
				Enabled = Enabled,
				X = X,
				Y = Y,
				Width = Width,
				Height = Height,
				WhirlMode = WhirlMode,
				TailLength = TailLength,
				StartLocation = StartLocation,
				WhirlDirection = WhirlDirection,
				Rotation = Rotation,
				ReverseDraw = ReverseDraw,
				Spacing = Spacing,
				Thickness = Thickness,
				Show3D = Show3D,
				XOffset = XOffset,
				YOffset = YOffset,
				ColorMode = ColorMode,
				BandLength = BandLength,
				LeftColor = LeftColor,
				RightColor = RightColor,
				TopColor = TopColor,
				BottomColor = BottomColor,
				SingleColor = SingleColor,
				Intensity = Intensity,
				PauseAtEnd = PauseAtEnd,
			};

			// Clear the color collection
			result.Colors.Clear();

			// Loop over the color gradients in the whirl
			foreach (ColorGradient cg in Colors)
			{
				// Copy the color gradient to the cloned whirl
				result.Colors.Add(cg);
			}

			return result;
		}

		#endregion

		#region IWhirl

		/// <inheritdoc/>
		[Browsable(false)]
		public int X { get; set; }

		/// <inheritdoc/>
		[Browsable(false)]
		public int Y { get; set; }

		/// <inheritdoc/>
		[Browsable(false)] 
		public int XOffset { get; set; }

		/// <inheritdoc/>
		[Browsable(false)]
		public int YOffset { get; set; }

		/// <inheritdoc/>
		[Browsable(false)]
		public int Width { get; set; }

		/// <inheritdoc/>
		[Browsable(false)]
		public int Height { get; set; }

		/// <inheritdoc/>
		[Browsable(false)]
		public double Intensity { get; set; }

		/// <inheritdoc/>
		[Browsable(false)]
		public int PauseAtEnd { get; set; }

		#endregion

		#region IWhirl Visual Properties 

		private bool _enabled;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"WhirlpoolEnabled")]
		[ProviderDescription(@"WhirpoolEnabled")]
		[PropertyOrder(0)]
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				_enabled = value;
				OnPropertyChanged();
			}
		}

		private WhirlpoolMode _whirlpoolMode;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"WhirlpoolConfiguration", 1)]
		[ProviderDisplayName(@"WhirlpoolWhirlMode")]
		[ProviderDescription(@"WhirlpoolWhirlMode")]
		[PropertyOrder(0)]
		public WhirlpoolMode WhirlMode
		{
			get { return _whirlpoolMode; }
			set
			{
				_whirlpoolMode = value;
				UpdateWhirlModeConfigurationAttributes(true);

				// If the whirlpool mode is meteor then...
				if (_whirlpoolMode == WhirlpoolMode.Meteor)
				{
					// Set the color mode to a single gradient
					ColorMode = WhirlpoolColorMode.GradientOverTime;
				}

				OnPropertyChanged();
			}
		}

		private int _tailLength;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"WhirlpoolConfiguration", 1)]
		[ProviderDisplayName(@"WhirlpoolTailLength")]
		[ProviderDescription(@"WhirlpoolTailLength")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(1)]
		public int TailLength
		{
			get { return _tailLength; }
			set
			{
				_tailLength = value;
				OnPropertyChanged();
			}
		}

		private WhirlpoolRotation _rotation;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"WhirlpoolRotation")]
		[ProviderDescription(@"WhirlpoolRotation")]
		[PropertyOrder(2)]
		public WhirlpoolRotation Rotation
		{
			get { return _rotation; }
			set
			{
				_rotation = value;
				OnPropertyChanged();
			}
		}

		private WhirlpoolStartLocation _startLocation;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"WhirlpoolStartLocation")]
		[ProviderDescription(@"WhirlpoolStartLocation")]
		[PropertyOrder(3)]
		public WhirlpoolStartLocation StartLocation
		{
			get { return _startLocation; }
			set
			{
				_startLocation = value;
				OnPropertyChanged();
			}
		}

		private WhirlpoolDirection _whirlDirection;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"WhirlpoolDirection")]
		[ProviderDescription(@"WhirlpoolDirection")]
		[PropertyOrder(4)]
		public WhirlpoolDirection WhirlDirection
		{
			get { return _whirlDirection; }
			set
			{
				_whirlDirection = value;
				OnPropertyChanged();
			}
		}

		private int _spacing;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"WhirlpoolSpacing")]
		[ProviderDescription(@"WhirlpoolSpacing")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(5)]
		public int Spacing
		{
			get { return _spacing; }
			set
			{
				_spacing = value;
				OnPropertyChanged();
			}
		}

		private int _thickness;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"WhirlpoolThickness")]
		[ProviderDescription(@"WhirlpoolThickness")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(6)]
		public int Thickness
		{
			get { return _thickness; }
			set
			{
				_thickness = value;
				UpdateThicknessAttributes(true);
				OnPropertyChanged();
			}
		}

		private bool _inverted;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Whirl Configuration", 1)]
		[ProviderDisplayName(@"WhirlpoolInverted")]
		[ProviderDescription(@"WhirlpoolInverted")]
		[PropertyOrder(7)]
		public bool ReverseDraw
		{
			get { return _inverted; }
			set
			{
				_inverted = value;
				OnPropertyChanged();
			}
		}

		private bool _show3D;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"WhirlpoolConfiguration", 1)]
		[ProviderDisplayName(@"WhirlpoolShow3D")]
		[ProviderDescription(@"WhirlpoolShow3D")]
		[PropertyOrder(8)]
		public bool Show3D
		{
			get { return _show3D; }
			set
			{
				_show3D = value;
				OnPropertyChanged();
			}
		}

		private WhirlpoolColorMode _colorMode;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Configuration", 4)]
		[ProviderDisplayName(@"WhirlpoolColorMode")]
		[ProviderDescription(@"WhirlpoolColorMode")]
		[PropertyOrder(8)]
		public WhirlpoolColorMode ColorMode
		{
			get { return _colorMode; }
			set
			{
				_colorMode = value;
				UpdateColorModeAttribute(true);
				OnPropertyChanged();
			}
		}

		private int _bandLength;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Configuration", 4)]
		[ProviderDisplayName(@"WhirlpoolBandLength")]
		[ProviderDescription(@"WhirlpoolBandLength")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(9)]
		public int BandLength
		{
			get { return _bandLength; }
			set
			{
				_bandLength = value;
				OnPropertyChanged();
			}
		}

		private ColorGradient _singleColor;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Configuration", 4)]
		[ProviderDisplayName(@"WhirlpoolColor")]
		[ProviderDescription(@"WhirlpoolColor")]
		[PropertyOrder(10)]
		public ColorGradient SingleColor
		{
			get { return _singleColor; }
			set
			{
				_singleColor = value;
				OnPropertyChanged();
			}
		}

		private List<ColorGradient> _colors;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Configuration", 4)]
		[ProviderDisplayName(@"WhirlpoolColors")]
		[ProviderDescription(@"WhirlpoolColors")]
		[PropertyOrder(11)]
		public List<ColorGradient> Colors
		{
			get { return _colors; }
			set
			{
				_colors = value;
				OnPropertyChanged();
			}
		}

		private ColorGradient _leftColor;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Configuration", 4)]
		[ProviderDisplayName(@"WhirlpoolLeftColor")]
		[ProviderDescription(@"WhirlpoolLeftColor")]
		[PropertyOrder(12)]
		public ColorGradient LeftColor
		{
			get
			{
				return _leftColor;
			}
			set
			{
				_leftColor = value;
				OnPropertyChanged();
			}
		}

		private ColorGradient _rightColor;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Configuration", 4)]
		[ProviderDisplayName(@"WhirlpoolRightColor")]
		[ProviderDescription(@"WhirlpoolRightColor")]
		[PropertyOrder(13)]
		public ColorGradient RightColor
		{
			get
			{
				return _rightColor;
			}
			set
			{
				_rightColor = value;
				OnPropertyChanged();
			}
		}

		private ColorGradient _topColor;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Configuration", 4)]
		[ProviderDisplayName(@"WhirlpoolTopColor")]
		[ProviderDescription(@"WhirlpoolTopColor")]
		[PropertyOrder(14)]
		public ColorGradient TopColor
		{
			get
			{
				return _topColor;
			}
			set
			{
				_topColor = value;
				OnPropertyChanged();
			}
		}

		private ColorGradient _bottomColor;

		/// <inheritdoc/>
		[Value]
		[ProviderCategory(@"Configuration", 4)]
		[ProviderDisplayName(@"WhirlpoolBottomColor")]
		[ProviderDescription(@"WhirlpoolBottomColor")]
		[PropertyOrder(15)]
		public ColorGradient BottomColor
		{
			get
			{
				return _bottomColor;
			}
			set
			{
				_bottomColor = value;
				OnPropertyChanged();
			}
		}

		#endregion

		public override string ToString()
		{
			return "Whirl";
		}

		#region Public Methods

		/// <summary>
		/// Resets the next pixel to draw for the whirl.
		/// </summary>
		public void ResetPixelNumber()
		{
			// If drawing the whirl in reverse then...
			if (_internalReverseDraw)
			{
				// Set the pixel to the last pixel
				_numberOfPixelsToDraw = _totalNumberOfPixels - 1;
			}
			else
			{
				// Otherwise set the pixel to the first pixel
				_numberOfPixelsToDraw = 0;
			}
		}

		/// <summary>
		/// Renders the whirl.
		/// </summary>
		/// <param name="numberOfFrames">Number of frames to render the whirl</param>
		public void SetupRender(int numberOfFrames)
		{
			// Capture the Reverse Draw setting
			_internalReverseDraw = ReverseDraw;

			// Default to NOT drawing the tail of the whirl
			_drawingTail = false;

			// Save off the number of frames
			_numberOfFrames = numberOfFrames;

			// Determine the number of frames to pause at then end
			double framesAtEnd = numberOfFrames * PauseAtEnd / 100.0;

			// If the whirl is NOT symmetrical then...
			if (WhirlMode != WhirlpoolMode.SymmetricalWhirls)
			{
				// Setup the whirl based on the start location
				switch (StartLocation)
				{
					case WhirlpoolStartLocation.TopLeft:
						SetupRenderTopLeft();
						break;
					case WhirlpoolStartLocation.BottomLeft:
						SetupRenderBottomLeft();
						break;
					case WhirlpoolStartLocation.TopRight:
						SetupRenderTopRight();
						break;
					case WhirlpoolStartLocation.BottomRight:
						SetupRenderBottomRight();
						break;
				}

				// Initialize the number of pixels to render
				double numberOfPixels = _totalNumberOfPixels;

				// If drawing both in and out we need to double the number of pixels
				if (WhirlDirection == WhirlpoolDirection.InAndOut)
				{
					numberOfPixels = numberOfPixels * 2;
				}

				// Calculate the number of pixels to draw each frame
				_pixelsToDrawPerFrame = numberOfPixels / (numberOfFrames - framesAtEnd);

				// Reset the current pixel being drawn
				ResetPixelNumber();
			}
			// Drawing symmetrical whirls
			else
			{
				// Calculate the number of symmetrical rectangle to draw
				_numberOfSymmetricRectangles = CalculateNumberOfRectangles(0, 0, Width, Height, CalculateThickness(Thickness), CalculateSpacing(Spacing));

				// Calculate the number of frames between drawing rectangles
				_framesPerRectangle = (int)(numberOfFrames - framesAtEnd) / _numberOfSymmetricRectangles;

				// If drawing both in and out then...
				if (WhirlDirection == WhirlpoolDirection.InAndOut)
				{
					// Need to reduce the number of frames per rectangle
					_framesPerRectangle = _framesPerRectangle / 2;
				}
			}
		}
		
		/// <summary>
		/// Calculates the spacing for the whirl.
		/// </summary>
		/// <param name="spacing">Spacing data setting</param>
		/// <returns>Spacing in pixels</returns>
		private int CalculateSpacing(int spacing)
		{
			return (int)(spacing / 100.0 * Math.Max(Width, Height));
		}
		
		/// <summary>
		/// Calculates the thickness for the whirl.
		/// </summary>
		/// <param name="thickness">Thickness data setting</param>
		/// <returns>Thickness in pixels</returns>
		private int CalculateThickness(int thickness)
		{
			int whirlThickness = (int)(thickness / 100.0 * Math.Max(Width, Height));

			// The thickness needs to be at least one pixel
			if (whirlThickness < 1)
			{
				whirlThickness = 1;
			}

			return whirlThickness;
		}

		/// <summary>
		/// Render's the whirl.
		/// </summary>
		/// <param name="frame">Frame number that is being rendered</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		/// <param name="intervalPos">Position within the effect's duration</param>
		public void RenderEffect(
			int frame,
			IPixelFrameBuffer frameBuffer,
			double intervalPos)
		{
			// If the WhirlDirection is In & Out and we are at the halfway point then...
			// Note by using the exact halfway point will divide the pause in half.
			// Part of the pause on the in and the other half on the out.
			if (WhirlDirection == WhirlpoolDirection.InAndOut && frame == _numberOfFrames / 2)
			{
				_internalReverseDraw = !ReverseDraw;

				// Since we are starting a new whirl reset the pixel (counter) number
				ResetPixelNumber();
			}

			// Drawing the whirl in reverse then...
			if (_internalReverseDraw)
			{
				// Subtract off the number of pixels for this frame
				_numberOfPixelsToDraw -= _pixelsToDrawPerFrame;
			}
			else
			{
				_numberOfPixelsToDraw += _pixelsToDrawPerFrame;

				// If we are already at the maximum number of pixels for the whirl then...
				if (_numberOfPixelsToDraw > _totalNumberOfPixels)
				{
					// Limit the number of pixels to the maximum
					_numberOfPixelsToDraw = _totalNumberOfPixels;
				}
			}

			// If we are on the last leg of the whirl then...
			if (_numberOfPixelsToDraw >= _totalNumberOfPixels - CalculateThickness(Thickness))
			{
				// Set a flag that we are drawing the tail
				_drawingTail = true;
			}

			// Reset the color index to zero
			_colorIndex = 0;

			// Reset the band pixel count to zero
			_bandPixelCount = 0;

			// If the whirl mode is symmetrical whirls
			if (WhirlMode == WhirlpoolMode.SymmetricalWhirls)
			{
				// Determine how many rectangle levels that need to be drawn for the frame
				int rectangleLevels = frame / _framesPerRectangle;

				// If drawing whirl rectangles that move in then...
				if (WhirlDirection == WhirlpoolDirection.In)
				{
					// Draw the symmetrical whirl rectangles moving in
					DrawWhirlSymmetricalIn(frameBuffer, 0, 0, Width, Height, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos, rectangleLevels);
				}
				// Otherwise if drawing whirl rectangles that move out then...
				else if (WhirlDirection == WhirlpoolDirection.Out)
				{
					// Draw the symmetrical whirl rectangles moving out
					DrawWhirlSymmetricalOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos, rectangleLevels);
				}
				// Whirl Direction is In and Out
				else
				{
					// If in the first half of the effect then...
					if (frame < _numberOfFrames / 2)
					{
						// Draw the symmetrical whirl rectangles moving in
						DrawWhirlSymmetricalIn(frameBuffer, 0, 0, Width, Height, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos, rectangleLevels);
					}
					else
					{
						// Determine how many rectangle levels that need to be drawn for the frame
						rectangleLevels = _numberOfSymmetricRectangles - (frame - _numberOfFrames / 2) / _framesPerRectangle;

						// If there is at least one rectangle left to draw then...
						if (rectangleLevels > 0)
						{
							// Draw the symmetrical whirl rectangles moving out
							DrawWhirlSymmetricalIn(frameBuffer, 0, 0, Width, Height, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos, rectangleLevels);
						}
					}
				}
			}
			else
			{
				// Render the whirl based on the start location
				switch (StartLocation)
				{
					case WhirlpoolStartLocation.TopLeft:
						RenderEffectTopLeft(frame, frameBuffer, intervalPos);
						break;
					case WhirlpoolStartLocation.BottomLeft:
						RenderEffectBottomLeft(frame, frameBuffer, intervalPos);
						break;
					case WhirlpoolStartLocation.TopRight:
						RenderEffectTopRight(frame, frameBuffer, intervalPos);
						break;
					case WhirlpoolStartLocation.BottomRight:
						RenderEffectBottomRight(frame, frameBuffer, intervalPos);
						break;
				}
			}
		}

		/// <summary>
		/// Gets the meteor tail length as a percentage of the display element size.
		/// </summary>
		/// <returns>Tail length</returns>
		private int GetTailLength()
		{
			// Get the tail length as a percentage of the max dimension of the display element
			int tailLength = (int)(TailLength / 100.0 * Math.Max(Width, Height));

			// If the tail length is not at least on pixel then...
			if (tailLength < 1)
			{
				// Ensure the tail length is at least on pixel
				tailLength = 1;
			}

			return tailLength;
		}

		/// <summary>
		/// Returns the color for the specified whirlpool side, position within the effect's duration and the row within the whirl thickness.
		/// </summary>
		/// <param name="side">Side being drawn</param>
		/// <param name="intervalPos">Position of the effect within the effect's duration</param>
		/// <param name="whirlIndex">Which row of the whirl's thickness is being drawn</param>
		/// <returns>Color for the whirl side</returns>
		private Color GetColor(WhirlpoolSideType side, double intervalPos, int whirlIndex, int totalNumberOfPixelsRemaining)
		{
			// Default the return value to transparent
			Color color = Color.Transparent;
			
			// Update the color based on the Color Mode of the whirl
			switch (ColorMode)
			{
				case WhirlpoolColorMode.GradientOverTime:

					if (WhirlMode == WhirlpoolMode.Meteor)
					{
						if (totalNumberOfPixelsRemaining <= GetTailLength())
						{
							double tailLength = GetTailLength();
							double tailInterval = (tailLength - totalNumberOfPixelsRemaining) / tailLength;
							color = SingleColor.GetColorAt(tailInterval);
						}
					}
					else
					{
						color = SingleColor.GetColorAt(intervalPos);
					}
					break;
				case WhirlpoolColorMode.LegColors:
					
					// Select the color based on the side being drawn
					switch (side)
					{
						case WhirlpoolSideType.LeftSide:
							color = LeftColor.GetColorAt(intervalPos);
							break;
						case WhirlpoolSideType.RightSide:
							color = RightColor.GetColorAt(intervalPos);
							break;
						case WhirlpoolSideType.TopSide:
							color = TopColor.GetColorAt(intervalPos);
							break;
						case WhirlpoolSideType.BottomSide:
							color = BottomColor.GetColorAt(intervalPos);
							break;
						default:
							break;
					}
					break;
				case WhirlpoolColorMode.Bands:

					// Get the color from the current color band
					color = Colors[_colorIndex].GetColorAt(intervalPos);

					// Update the color band indices
					UpdateSelectedColorBand();

					break;
				case WhirlpoolColorMode.RectangularRings:

					// Retrieve the color for the rectangle ring from the color collection
					color = Colors[_colorIndex].GetColorAt(intervalPos);
					break;
				default:
					break;
			}

			// If the whirl should be drawn in 3D then...
			if (Show3D)
			{
				// Update the intensity for a 3D look
				color = Get3DColor(whirlIndex, CalculateThickness(Thickness), color);
			}

			// Update the color intensity based on the effect's brightness setting
			HSV hsv = HSV.FromRGB(color);
			hsv.V = hsv.V * Intensity;

			// Return the color
			return hsv.ToRGB();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates the whirl drawing color when the color mode is Rectangular Rings.
		/// </summary>
		private void UpdateColor()
		{
			// If the color mode is Rectangular Rings then...
			if (ColorMode == WhirlpoolColorMode.RectangularRings)
			{
				// Increment the color index
				_colorIndex++;

				// If we have exceeded the number of colors in the color collection then...
				if (_colorIndex > Colors.Count - 1)
				{
					// Go back to the first color in the collection
					_colorIndex = 0;
				}
			}
		}

		/// <summary>
		/// Gets the 3-D color for the specified bar pixel.
		/// </summary>
		/// <param name="colorIndex">Index into the color array</param>
		/// <param name="currentThicknessCounter">Current row of the bar being drawn</param>
		/// <param name="barThickness">Thickness of the bar</param>
		/// <returns>3-D color for the specified bar pixel</returns>
		private Color Get3DColor(int currentThicknessCounter, int whirlThickness, Color color)
		{
			// Convert from RGB to HSV color format 
			HSV hsv = HSV.FromRGB(color);

			// Set the brightness based on the percentage of the bar thickness
			hsv.V *= (float)(whirlThickness - currentThicknessCounter) / whirlThickness;

			// Convert the color back to RGB format
			return hsv.ToRGB();
		}

		/// <summary>
		/// Gets the color band length as a percentage of the display element size.
		/// </summary>
		/// <returns>Tail length</returns>
		private int GetBandLength()
		{
			// Get the band length as a percentage of the max dimension of the display element
			int bandLength = (int)(BandLength / 100.0 * Math.Max(Width, Height));

			// If the band length is not at least on pixel then...
			if (bandLength < 1)
			{
				// Ensure the band length is at least on pixel
				bandLength = 1;
			}

			return bandLength;
		}

		/// <summary>
		/// Updates the selected color band when the color mode is Bands.
		/// </summary>
		private void UpdateSelectedColorBand()
		{
			// If the color mode is bands then...
			if (ColorMode == WhirlpoolColorMode.Bands)
			{
				// Keep track of the number pixels drawn in the band
				_bandPixelCount++;

				// If a complete band has been drawn then...
				if (_bandPixelCount >= GetBandLength())
				{
					// Increment to the next pixel
					_colorIndex++;

					// If we have used all the colors in the collection then...
					if (_colorIndex > Colors.Count - 1)
					{
						// Go back to the first color in the collection
						_colorIndex = 0;
					}

					// Reset the pixel per band counter
					_bandPixelCount = 0;
				}
			}
		}

		/// <summary>
		/// Shows and hides attributes based on the whirl color mode.
		/// </summary>
		/// <param name="refresh">Refresh the UI</param>
		private void UpdateColorModeAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(7);

			// Single color is only applicable with the color mode is gradient
			propertyStates.Add(nameof(SingleColor), ColorMode == WhirlpoolColorMode.GradientOverTime);

			// Color band length is only applicable with the color mode is Bands
			propertyStates.Add(nameof(BandLength), ColorMode == WhirlpoolColorMode.Bands);

			// The color collection is only visible when the color mode is rings or bands
			propertyStates.Add(nameof(Colors), ColorMode == WhirlpoolColorMode.RectangularRings ||
			                                   ColorMode == WhirlpoolColorMode.Bands);

			// Leg Color gradients are only visible when the color mode is leg colors
			propertyStates.Add(nameof(LeftColor), ColorMode == WhirlpoolColorMode.LegColors);
			propertyStates.Add(nameof(RightColor), ColorMode == WhirlpoolColorMode.LegColors);
			propertyStates.Add(nameof(TopColor), ColorMode == WhirlpoolColorMode.LegColors);
			propertyStates.Add(nameof(BottomColor), ColorMode == WhirlpoolColorMode.LegColors);

			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Update whirl thickness attributes.
		/// </summary>
		/// <param name="refresh">Whether to refresh the editor</param>
		private void UpdateThicknessAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);

			// 3-D Whirls are only applicable if the whirl is more than one pixel thick
			propertyStates.Add(nameof(Show3D), CalculateThickness(Thickness) > 1);

			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Updates whirl configuration attributes.
		/// </summary>
		/// <param name="refresh">Whether to refresh the editor</param>
		private void UpdateWhirlModeConfigurationAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(5);

			propertyStates.Add(nameof(TailLength), WhirlMode == WhirlpoolMode.Meteor);

			// Rotation, Start Location, Reverse Draw are NOT applicable to Symmetrical Whirls
			propertyStates.Add(nameof(Rotation), WhirlMode != WhirlpoolMode.SymmetricalWhirls);
			propertyStates.Add(nameof(StartLocation), WhirlMode != WhirlpoolMode.SymmetricalWhirls);
			propertyStates.Add(nameof(ReverseDraw), WhirlMode != WhirlpoolMode.SymmetricalWhirls);

			// Color mode is not applicable Meteor mode
			propertyStates.Add(nameof(ColorMode), WhirlMode != WhirlpoolMode.Meteor);

			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}
		
		/// <summary>
		/// Prepares the top left whirl for rendering.
		/// </summary>
		private void SetupRenderTopLeft()
		{
			// If the whirl is rotating clockwise then...
			if (Rotation == WhirlpoolRotation.Clockwise)
			{
				_totalNumberOfPixels = CalculateNumberOfPixelsTopLeftClockwise(0, 0, Width, Height, CalculateThickness(Thickness), CalculateSpacing(Spacing), true);
			}
			else if (Rotation == WhirlpoolRotation.CounterClockwise)
			{
				_totalNumberOfPixels = CalculateNumberOfPixelsTopLeftCounterClockwise(0, 0, Width, Height, CalculateThickness(Thickness), CalculateSpacing(Spacing), true);
			}
		}

		/// <summary>
		/// Prepares the bottom left whirl for rendering.
		/// </summary>
		private void SetupRenderBottomLeft()
		{
			// If the whirl is rotating clockwise then...
			if (Rotation == WhirlpoolRotation.Clockwise)
			{
				_totalNumberOfPixels = CalculateNumberOfPixelsBottomLeftClockwise(0, 0, Width, Height, CalculateThickness(Thickness), CalculateSpacing(Spacing), true);
			}
			// Otherwise if the whirl is rotating counter-clockwise then...
			else if (Rotation == WhirlpoolRotation.CounterClockwise)
			{
				_totalNumberOfPixels = CalculateNumberOfPixelsBottomLeftCounterClockwise(0, 0, Width, Height, CalculateThickness(Thickness), CalculateSpacing(Spacing), true);
			}
		}

		/// <summary>
		/// Prepares the top right whirl for rendering.
		/// </summary>
		private void SetupRenderTopRight()
		{
			// If the whirl is rotating clockwise then...
			if (Rotation == WhirlpoolRotation.Clockwise)
			{
				_totalNumberOfPixels = CalculateNumberOfPixelsTopRightClockwise(0, 0, Width, Height, CalculateThickness(Thickness), CalculateSpacing(Spacing), true);
			}
			// Otherwise if the whirl is rotating counter-clockwise then...
			else if (Rotation == WhirlpoolRotation.CounterClockwise)
			{
				_totalNumberOfPixels = CalculateNumberOfPixelsTopRightCounterClockwise(0, 0, Width, Height, CalculateThickness(Thickness), CalculateSpacing(Spacing), true);
			}
		}

		/// <summary>
		/// Prepares the bottom right whirl for rendering.
		/// </summary>
		private void SetupRenderBottomRight()
		{
			// If the whirl is rotating clockwise then...
			if (Rotation == WhirlpoolRotation.Clockwise)
			{
				_totalNumberOfPixels = CalculateNumberOfPixelsBottomRightClockwise(0, 0, Width, Height, CalculateThickness(Thickness), CalculateSpacing(Spacing), true);
			}
			// Otherwise if the whirl is rotating counter-clockwise then...
			else if (Rotation == WhirlpoolRotation.CounterClockwise)
			{
				_totalNumberOfPixels = CalculateNumberOfPixelsBottomRightCounterClockwise(0, 0, Width, Height, CalculateThickness(Thickness), CalculateSpacing(Spacing), true);
			}
		}

		/// <summary>
		/// Calculates the number of symmetric rectangles that can be drawn on the display element.
		/// </summary>
		/// <param name="x">X position of the draw area</param>
		/// <param name="y">Y position of the draw area</param>
		/// <param name="width">Width of the draw area</param>
		/// <param name="height">Height of the draw area</param>
		/// <param name="thickness">Thickness of the rectangle whirl</param>
		/// <param name="spacing">Spacing between rectangle whirls</param>
		/// <returns>Number of rectangles that can be drawn on the display element</returns>
		private int CalculateNumberOfRectangles(int x, int y, int width, int height, int thickness, int spacing)
		{
			// Default to no whirls fitting
			int numberOfRectangles = 0;

			// If the width and height can accomodate a rectangular whirl then...
			if (width >= thickness && height >= thickness)
			{
				// Indicate a whirl can be drawn
				numberOfRectangles = 1;

				// Reduce the drawing area by the thickness of the whirl and the spacing between whirls
				width -= 2 * (thickness + spacing);
				height -= 2 * (thickness + spacing);

				// If the reduced drawing area can draw more rectangular whirls then...
				if (width >= thickness && height >= thickness)
				{
					// Increment the X and Y position (reducing drawing area)
					x += thickness + spacing;
					y += thickness + spacing;

					// Recursively calculate the additional number of rectangles that fit on the display element
					numberOfRectangles += CalculateNumberOfRectangles(x, y, width, height, thickness, spacing);
				}
				else
				{
					// Otherwise save off the X,Y, Width and Height of the last rectangle
					_vortexMetadata.LastWidth = width + 2 * (thickness + spacing);
					_vortexMetadata.LastHeight = height + 2 * (thickness + spacing);
					_vortexMetadata.LastX = x;
					_vortexMetadata.LastY = y;
				}
			}

			// Return the number of rectangles that fit on the display element
			return numberOfRectangles;
		}

		/// <summary>
		/// Render the whirl starting in the top left corner.
		/// </summary>
		/// <param name="frame">Frame number being rendered</param>
		/// <param name="frameBuffer">Frame buffer to render into</param>
		/// <param name="intervalPos">Position within the effect's duration</param>
		private void RenderEffectTopLeft(int frame, IPixelFrameBuffer frameBuffer, double intervalPos)
		{
			// Render based on the rotation of the whirl
			switch (Rotation)
			{
				case WhirlpoolRotation.Clockwise:
					switch (WhirlDirection)
					{
						case WhirlpoolDirection.In:
							if (ReverseDraw)
							{
								DrawTopLeftClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							else
							{
								DrawTopLeftClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							break;
						case WhirlpoolDirection.Out:
							if (ReverseDraw)
							{
								DrawTopLeftClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							else
							{
								DrawTopLeftClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							break;
						case WhirlpoolDirection.InAndOut:
							DrawTopLeftClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos); 
							break;
					}
					break;
				case WhirlpoolRotation.CounterClockwise:
					switch (WhirlDirection)
					{
						case WhirlpoolDirection.In:
							if (ReverseDraw)
							{
								DrawTopLeftCounterClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							else
							{
								DrawTopLeftCounterClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							break;
						case WhirlpoolDirection.Out:
							if (ReverseDraw)
							{
								DrawTopLeftCounterClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							else
							{
								DrawTopLeftCounterClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							break;
						case WhirlpoolDirection.InAndOut:
							DrawTopLeftCounterClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							break;
					}
					break;
			}
		}

		/// <summary>
		/// Render the whirl starting in the bottom left corner.
		/// </summary>
		/// <param name="frame">Frame number being rendered</param>
		/// <param name="frameBuffer">Frame buffer to render into</param>
		/// <param name="intervalPos">Position within the effect's duration</param>
		private void RenderEffectBottomLeft(int frame, IPixelFrameBuffer frameBuffer, double intervalPos)
		{
			// Render based on the rotation of the whirl
			switch (Rotation)
			{
				case WhirlpoolRotation.CounterClockwise:
					switch (WhirlDirection)
					{
						case WhirlpoolDirection.In:
							if (ReverseDraw)
							{
								DrawBottomLeftCounterClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							else
							{ 
								DrawBottomLeftCounterClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							break;
						case WhirlpoolDirection.Out:
							if (ReverseDraw)
							{
								DrawBottomLeftCounterClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							else
							{
								DrawBottomLeftCounterClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							break;
						case WhirlpoolDirection.InAndOut:
							DrawBottomLeftCounterClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
								break;
					}
					break;
				case WhirlpoolRotation.Clockwise:
					switch (WhirlDirection)
					{
						case WhirlpoolDirection.In:
							if (ReverseDraw)
							{
								DrawBottomLeftClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							else
							{
								DrawBottomLeftClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							break;
						case WhirlpoolDirection.Out:
							if (ReverseDraw)
							{
								DrawBottomLeftClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							else
							{
								DrawBottomLeftClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							break;
						case WhirlpoolDirection.InAndOut:
							DrawBottomLeftClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos); break;
					}
					break;
			}
		}

		/// <summary>
		/// Render the whirl starting in the top right corner.
		/// </summary>
		/// <param name="frame">Frame number being rendered</param>
		/// <param name="frameBuffer">Frame buffer to render into</param>
		/// <param name="intervalPos">Position within the effect's duration</param>
		private void RenderEffectTopRight(int frame, IPixelFrameBuffer frameBuffer, double intervalPos)
		{
			// Render based on the rotation of the whirl
			switch (Rotation)
			{
				case WhirlpoolRotation.Clockwise:
					switch (WhirlDirection)
					{
						case WhirlpoolDirection.In:
							if (ReverseDraw)
							{
								DrawTopRightClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							else
							{
								DrawTopRightClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							break;
						case WhirlpoolDirection.Out:
							if (ReverseDraw)
							{
								DrawTopRightClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							else
							{
								DrawTopRightClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							break;
						case WhirlpoolDirection.InAndOut:
							DrawTopRightClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos); 
							break;
					}
					break;
				case WhirlpoolRotation.CounterClockwise:
					switch (WhirlDirection)
					{
						case WhirlpoolDirection.In:
							if (ReverseDraw)
							{
								DrawTopRightCounterClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							else
							{
								DrawTopRightCounterClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							break;
						case WhirlpoolDirection.Out:
							if (ReverseDraw)
							{
								DrawTopRightCounterClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							else
							{
								DrawTopRightCounterClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							break;
						case WhirlpoolDirection.InAndOut:
							DrawTopRightCounterClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							break;
					}
					break;
			}
		}

		/// <summary>
		/// Render the whirl starting in the bottom right corner.
		/// </summary>
		/// <param name="frame">Frame number being rendered</param>
		/// <param name="frameBuffer">Frame buffer to render into</param>
		/// <param name="intervalPos">Position within the effect's duration</param>
		private void RenderEffectBottomRight(int frame, IPixelFrameBuffer frameBuffer, double intervalPos)
		{
			// Render based on the rotation of the whirl
			switch (Rotation)
			{
				case WhirlpoolRotation.Clockwise:
					switch (WhirlDirection)
					{
						case WhirlpoolDirection.In:
							if (ReverseDraw)
							{
								DrawBottomRightClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							else
							{
								DrawBottomRightClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							break;
						case WhirlpoolDirection.Out:
							if (ReverseDraw)
							{
								DrawBottomRightClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							else
							{
								DrawBottomRightClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							break;
						case WhirlpoolDirection.InAndOut:
							DrawBottomRightClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							break;
					}
					break;
				case WhirlpoolRotation.CounterClockwise:
					switch (WhirlDirection)
					{
						case WhirlpoolDirection.In:
							if (ReverseDraw)
							{
								DrawBottomRightCounterClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							}
							else
							{
								DrawBottomRightCounterClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							break;
						case WhirlpoolDirection.Out:
							if (ReverseDraw)
							{
								DrawBottomRightCounterClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos);
							}
							else
							{
								DrawBottomRightCounterClockwiseOut(frameBuffer, _vortexMetadata.LastX, _vortexMetadata.LastY, _vortexMetadata.LastWidth, _vortexMetadata.LastHeight, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), _vortexMetadata.DrawTop, _vortexMetadata.DrawRight, _vortexMetadata.DrawBottom, _vortexMetadata.DrawLeft, intervalPos);
							} 
							break;
						case WhirlpoolDirection.InAndOut:
							DrawBottomRightCounterClockwiseIn(frameBuffer, 0, 0, Width, Height, (int)_numberOfPixelsToDraw, CalculateThickness(Thickness), CalculateSpacing(Spacing), intervalPos); 
							break;
					}
					break;
			}
		}

		/// <summary>
		/// Returns true if the specified numberOfPixels is part of the leading edge of the whirl.
		/// </summary>
		/// <param name="length">Total number of pixels being drawn</param>
		/// <param name="numberOfPixels">Current whirl pixel being drawn</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="index">Thickness index of the whirl; what band of the thickness is being drawn</param>
		/// <returns></returns>
		private bool LeadingEdge(int length, int numberOfPixels, int thickness, int index)
		{
			// Until we get to a corner we want to draw the whirl with a rectangular edge.
			// On a corner the whirl should be drawn as a triangle unless
			// we are drawing the tail which should also be drawn with a rectangular edge.
			return numberOfPixels <= length - (thickness - index) ||
				   numberOfPixels <= length && _drawingTail;
		}

		/// <summary>
		/// Sets a pixel in the frame buffer.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to update</param>
		/// <param name="x">X position of the pixel</param>
		/// <param name="y">Y position of the pixel</param>
		/// <param name="color">Color of the pixel</param>
		/// <param name="totalNumberOfPixelsRemaining"></param>
		private void SetPixel(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			Color color,
			int totalNumberOfPixelsRemaining)
		{
			// Add any offsets to the position
			int xOffset = X + XOffset; 
			int yOffset = Y + YOffset; 

			// If the whirl mode is Meteor then...
			if (WhirlMode == WhirlpoolMode.Meteor)
			{
				// If the pixel being drawn is within the Meteor then...
				if (totalNumberOfPixelsRemaining <= GetTailLength())
				{
					// Draw the meteor pixel
					frameBuffer.SetPixel(x + xOffset, y + yOffset, color);
				}
			}
			else
			{
				// Otherwise draw the whirl pixel
				frameBuffer.SetPixel(x + xOffset, y + yOffset, color);
			}
		}

		/// <summary>
		/// Draws a whirl bottom side in the clockwise direction.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to update</param>
		/// <param name="x">X position of the drawing area</param>
		/// <param name="y">Y position of the drawing area</param>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="numberOfPixels">Number of pixel to draw</param>
		/// <param name="maxNumberOfPixels">Max number of pixels for the whirl</param>
		/// <param name="intervalPos">Position of the whirl within the effect's duration</param>
		/// <param name="offset">Drawing offset to account for concentric whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <param name="totalNumberOfPixelsRemaining">Counter of remaining pixels to draw</param>
		private void DrawBottomSideClockwise(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int thickness,
			int numberOfPixels,
			int maxNumberOfPixels,
			double intervalPos,
			int offset,
			bool firstPass,
			int totalNumberOfPixelsRemaining)
		{
			DrawSide(
				WhirlpoolSideType.BottomSide,
				frameBuffer,
				x,
				y,
				width,
				height,
				thickness,
				numberOfPixels,
				maxNumberOfPixels,
				intervalPos,
				offset,
				firstPass,
				totalNumberOfPixelsRemaining);
		}

		/// <summary>
		/// Draws a whirl left side in the clockwise direction.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to update</param>
		/// <param name="x">X position of the drawing area</param>
		/// <param name="y">Y position of the drawing area</param>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="numberOfPixels">Number of pixel to draw</param>
		/// <param name="maxNumberOfPixels">Max number of pixels for the whirl</param>
		/// <param name="intervalPos">Position of the whirl within the effect's duration</param>
		/// <param name="offset">Drawing offset to account for concentric whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <param name="totalNumberOfPixelsRemaining">Counter of remaining pixels to draw</param>
		private void DrawLeftSideClockwise(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int thickness,
			int numberOfPixels,
			int maxNumberOfPixels,
			double intervalPos,
			int offset,
			bool firstPass,
			int totalNumberOfPixelsRemaining)
		{
			DrawSide(
				WhirlpoolSideType.LeftSide,
				frameBuffer,
				x,
				y,
				width,
				height,
				thickness,
				numberOfPixels,
				maxNumberOfPixels,
				intervalPos,
				offset,
				firstPass,
				totalNumberOfPixelsRemaining);
		}

		/// <summary>
		/// Draws a whirl right side in the clockwise direction.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to update</param>
		/// <param name="x">X position of the drawing area</param>
		/// <param name="y">Y position of the drawing area</param>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="numberOfPixels">Number of pixel to draw</param>
		/// <param name="maxNumberOfPixels">Max number of pixels for the whirl</param>
		/// <param name="intervalPos">Position of the whirl within the effect's duration</param>
		/// <param name="offset">Drawing offset to account for concentric whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <param name="totalNumberOfPixelsRemaining">Counter of remaining pixels to draw</param>
		private void DrawRightSideClockwise(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int thickness,
			int numberOfPixels,
			int maxNumberOfPixels,
			double intervalPos,
			int offset,
			bool firstPass,
			int totalNumberOfPixelsRemaining)
		{
			DrawSide(
				WhirlpoolSideType.RightSide,
				frameBuffer,
				x,
				y,
				width,
				height,
				thickness,
				numberOfPixels,
				maxNumberOfPixels,
				intervalPos,
				offset,
				firstPass,
				totalNumberOfPixelsRemaining);
		}

		/// <summary>
		/// Draws a whirl top side in the clockwise direction.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to update</param>
		/// <param name="x">X position of the drawing area</param>
		/// <param name="y">Y position of the drawing area</param>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="numberOfPixels">Number of pixel to draw</param>
		/// <param name="maxNumberOfPixels">Max number of pixels for the whirl</param>
		/// <param name="intervalPos">Position of the whirl within the effect's duration</param>
		/// <param name="offset">Drawing offset to account for concentric whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <param name="totalNumberOfPixelsRemaining">Counter of remaining pixels to draw</param>
		private void DrawTopSideClockwise(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int thickness,
			int numberOfPixels,
			int maxNumberOfPixels,
			double intervalPos,
			int offset,
			bool firstPass,
			int totalNumberOfPixelsRemaining)
		{
			DrawSide(
				WhirlpoolSideType.TopSide,
				frameBuffer,
				x,
				y,
				width,
				height,
				thickness,
				numberOfPixels,
				maxNumberOfPixels,
				intervalPos,
				offset,
				firstPass,
				totalNumberOfPixelsRemaining);
		}

		/// <summary>
		/// Draws a whirl right side in the counter-clockwise direction.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to update</param>
		/// <param name="x">X position of the drawing area</param>
		/// <param name="y">Y position of the drawing area</param>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="numberOfPixels">Number of pixel to draw</param>
		/// <param name="maxNumberOfPixels">Max number of pixels for the whirl</param>
		/// <param name="intervalPos">Position of the whirl within the effect's duration</param>
		/// <param name="offset">Drawing offset to account for concentric whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <param name="totalNumberOfPixelsRemaining">Counter of remaining pixels to draw</param>
		private void DrawRightSideCounterClockwise(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int thickness,
			int numberOfPixels,
			int maxNumberOfPixels,
			double intervalPos,
			int offset,
			bool firstPass,
			int totalNumberOfPixelsRemaining)
		{
			DrawSide(
				WhirlpoolSideType.RightSide,
				frameBuffer,
				x,
				y,
				width,
				height,
				thickness,
				numberOfPixels,
				maxNumberOfPixels,
				intervalPos,
				offset,
				firstPass,
				totalNumberOfPixelsRemaining);
		}

		/// <summary>
		/// Draws a whirl left side in the counter-clockwise direction.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to update</param>
		/// <param name="x">X position of the drawing area</param>
		/// <param name="y">Y position of the drawing area</param>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="numberOfPixels">Number of pixel to draw</param>
		/// <param name="maxNumberOfPixels">Max number of pixels for the whirl</param>
		/// <param name="intervalPos">Position of the whirl within the effect's duration</param>
		/// <param name="offset">Drawing offset to account for concentric whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <param name="totalNumberOfPixelsRemaining">Counter of remaining pixels to draw</param>
		private void DrawLeftSideCounterClockwise(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int thickness,
			int numberOfPixels,
			int maxNumberOfPixels,
			double intervalPos,
			int offset,
			bool firstPass,
			int totalNumberOfPixelsRemaining)
		{
			DrawSide(
				WhirlpoolSideType.LeftSide,
				frameBuffer,
				x,
				y,
				width,
				height,
				thickness,
				numberOfPixels,
				maxNumberOfPixels,
				intervalPos,
				offset,
				firstPass,
				totalNumberOfPixelsRemaining);
		}

		/// <summary>
		/// Draws a whirl top side in the counter-clockwise direction.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to update</param>
		/// <param name="x">X position of the drawing area</param>
		/// <param name="y">Y position of the drawing area</param>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="numberOfPixels">Number of pixel to draw</param>
		/// <param name="maxNumberOfPixels">Max number of pixels for the whirl</param>
		/// <param name="intervalPos">Position of the whirl within the effect's duration</param>
		/// <param name="offset">Drawing offset to account for concentric whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <param name="totalNumberOfPixelsRemaining">Counter of remaining pixels to draw</param>
		private void DrawTopSideCounterClockwise(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int thickness,
			int numberOfPixels,
			int maxNumberOfPixels,
			double intervalPos,
			int offset,
			bool firstPass,
			int totalNumberOfPixelsRemaining)
		{
			DrawSide(
				WhirlpoolSideType.TopSide,
				frameBuffer,
				x,
				y,
				width,
				height,
				thickness,
				numberOfPixels,
				maxNumberOfPixels,
				intervalPos,
				offset,
				firstPass,
				totalNumberOfPixelsRemaining);
		}

		/// <summary>
		/// Draws a whirl left side in the counter-clockwise direction.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to update</param>
		/// <param name="x">X position of the drawing area</param>
		/// <param name="y">Y position of the drawing area</param>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="numberOfPixels">Number of pixel to draw</param>
		/// <param name="maxNumberOfPixels">Max number of pixels for the whirl</param>
		/// <param name="intervalPos">Position of the whirl within the effect's duration</param>
		/// <param name="offset">Drawing offset to account for concentric whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <param name="totalNumberOfPixelsRemaining">Counter of remaining pixels to draw</param>
		private void DrawBottomSideCounterClockwise(
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int thickness,
			int numberOfPixels,
			int maxNumberOfPixels,
			double intervalPos,
			int offset,
			bool firstPass,
			int totalNumberOfPixelsRemaining)
		{
			DrawSide(
				WhirlpoolSideType.BottomSide,
				frameBuffer,
				x,
				y,
				width,
				height,
				thickness,
				numberOfPixels,
				maxNumberOfPixels,
				intervalPos,
				offset,
				firstPass,
				totalNumberOfPixelsRemaining);
		}

		/// <summary>
		/// Draws a whirl side.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to update</param>
		/// <param name="x">X position of the drawing area</param>
		/// <param name="y">Y position of the drawing area</param>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		/// <param name="thickness">Thickness of the whirl</param>
		/// <param name="numberOfPixels">Number of pixel to draw</param>
		/// <param name="maxNumberOfPixels">Max number of pixels for the whirl</param>
		/// <param name="intervalPos">Position of the whirl within the effect's duration</param>
		/// <param name="offset">Drawing offset to account for concentric whirls</param>
		/// <param name="firstPass">Whether this is the first whirl being drawn</param>
		/// <param name="totalNumberOfPixelsRemaining">Counter of remaining pixels to draw</param>
		private void DrawSide(
			WhirlpoolSideType side,
			IPixelFrameBuffer frameBuffer,
			int x,
			int y,
			int width,
			int height,
			int thickness,
			int numberOfPixels,
			int maxNumberOfPixels,
			double intervalPos,
			int offset,
			bool firstPass,
			int totalNumberOfPixelsRemaining)
		{
			// Save off the number of pixels remaining
			// Each row of the side needs to start out with this same number of pixels
			int savedTotalNumberOfPixelsRemaining = totalNumberOfPixelsRemaining;

			// Variables to save off color information
			int nextColorIndex = 0;
			int nextBandPixelCount = 0;

			// Save off the colors at the start of the whirl
			int colorIndex = _colorIndex;
			int bandPixelCount = _bandPixelCount;

			// Loop over the thickness of the whirls
			for (int index = 0; index < thickness; index++)
			{
				// Restore the colors at the start of each whirl row
				_colorIndex = colorIndex;
				_bandPixelCount = bandPixelCount;

				// Determine if this leg is the leading edge of the whirl
				bool leadingEdge = LeadingEdge(CalcNumPixels(maxNumberOfPixels, numberOfPixels), numberOfPixels, thickness, index);

				// If this is NOT the first leg of the whirl then...
				if (!firstPass)
				{
					for (int cIndex = 0; cIndex < index; cIndex++)
					{
						// Update the selected band color
						UpdateSelectedColorBand();
					}
				}

				// With each row of the whirl side we need to reset the number
				// pixels remaining to draw
				totalNumberOfPixelsRemaining = savedTotalNumberOfPixelsRemaining;

				// If the whirl mode is meteor or NOT lead edge then...
				if (WhirlMode == WhirlpoolMode.Meteor && !leadingEdge)
				{
					// Draw the pixels that make up one row of the side	
					totalNumberOfPixelsRemaining = DrawSidePixels(
						frameBuffer,
						side,
						x,
						y,
						offset,
						index,
						width,
						height,
						intervalPos,
						totalNumberOfPixelsRemaining,
						firstPass,
						numberOfPixels);
				}
				else
				{
					int numOfPixels = 0;

					// If drawing the tail then...
					if (_drawingTail)
					{
						// Don't adjust the number of pixels based on the index
						numOfPixels = CalcNumPixels(maxNumberOfPixels, numberOfPixels);
					}
					else
					{
						// Adjust the number of pixels based on the index to there is a tapered end
						numOfPixels = CalcNumPixels(maxNumberOfPixels, numberOfPixels) - index;
					}

					
					numOfPixels = Math.Min(numberOfPixels, numOfPixels);

					// Draw the pixels that make up one row of the side	
					totalNumberOfPixelsRemaining = DrawSidePixels(
						frameBuffer,
						side,
						x,
						y,
						offset,
						index,
						width,
						height,
						intervalPos,
						totalNumberOfPixelsRemaining,
						firstPass,
						numOfPixels);
				}

				// If this is the end of the first row then...
				if (index == 0)
				{
					// Save off the color information so we know how to color the next side
					nextColorIndex = _colorIndex;
					nextBandPixelCount = _bandPixelCount;
				}
			}

			// Restore the color information for the next side
			_colorIndex = nextColorIndex;
			_bandPixelCount = nextBandPixelCount;
		}

		/// <summary>
		/// Gets the position of a whirl pixel.
		/// </summary>
		/// <param name="rotation">Rotation of the whirl</param>
		/// <param name="side">Whirl side being drawn</param>
		/// <param name="x">X position of the whirl drawing area</param>
		/// <param name="y">Y position of the whirl drawing area</param>
		/// <param name="pixel">Index of the pixel being drawn</param>
		/// <param name="offset">Offset to account for increasing or decreasing whirls</param>
		/// <param name="index">Index within the thickness of the whirl</param>
		/// <param name="width">Width of the whirl drawing area</param>
		/// <param name="height">Height of the whirl drawing area</param>
		/// <returns>Position of the pixel to draw</returns>
		private (int XPos, int YPos) GetPixelPosition(WhirlpoolRotation rotation, WhirlpoolSideType side, int x, int y, int pixel, int offset, int index, int width, int height)
		{
			// Initialize the return values
			int xPos = 0;
			int yPos = 0;

			// If the whirl direction is Out then...
			if (WhirlDirection == WhirlpoolDirection.Out && !_internalReverseDraw ||
				WhirlDirection == WhirlpoolDirection.In && ReverseDraw)
			{
				// Need to invert the whirl rotation
				if (rotation == WhirlpoolRotation.Clockwise)
				{
					rotation = WhirlpoolRotation.CounterClockwise;
				}
				else
				{
					rotation = WhirlpoolRotation.Clockwise;
				}
			}
			
			// Determine the X and Y position based whirl rotation
			if (rotation == WhirlpoolRotation.CounterClockwise)
			{
				switch (side)
				{
					case WhirlpoolSideType.BottomSide:
						xPos = (x + pixel) + offset;
						yPos = y + index;
						break;
					case WhirlpoolSideType.LeftSide:
						xPos = x + index;
						yPos = Height - 1 - (y + pixel + offset);
						break;
					case WhirlpoolSideType.RightSide:
						xPos = x + width - 1 - index;
						yPos = pixel + y + offset;
						break;
					case WhirlpoolSideType.TopSide:
						xPos = Width - 1 - x - pixel + offset;
						yPos = Height - 1 - y - index;
						break;
				}
			}
			else
			{
				switch (side)
				{
					case WhirlpoolSideType.BottomSide:
						xPos = Width - 1 - (x + pixel) + offset;
						yPos = y + index;
						break;
					case WhirlpoolSideType.LeftSide:
						xPos = x + index;
						yPos = (y + pixel) + offset;
						break;
					case WhirlpoolSideType.RightSide:
						xPos = x + width - 1 - index;
						yPos = Height - 1 - (y + pixel) + offset;
						break;
					case WhirlpoolSideType.TopSide:
						xPos = x + pixel + offset;
						yPos = Height - 1 - y - index;
						break;
				}
			}

			return (xPos, yPos);
		}

		/// <summary>
		/// Draws a single row of pixels within the side of a whirl.
		/// </summary>
		/// <param name="frameBuffer">Frame buffer to update</param>
		/// <param name="side">Side being drawn</param>
		/// <param name="x">X position of the drawing area</param>
		/// <param name="y">Y position of the drawing area</param>
		/// <param name="offset">Offset to account for increasing or decreasing whirls</param>
		/// <param name="index">Index within the thickness of the whirl</param>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		/// <param name="intervalPos">Position of the whirl within the effect's duration</param>
		/// <param name="totalNumberOfPixelsRemaining">Number of pixel remaining to draw</param>
		/// <param name="firstPass">Whether this is the first whirl</param>
		/// <param name="numOfPixels">Number of pixels to draw for the side</param>
		/// <returns>Number of pixels remaining to draw</returns>
		private int DrawSidePixels(
			IPixelFrameBuffer frameBuffer,
			WhirlpoolSideType side, 
			int x, 
			int y,
			int offset, 
			int index, 
			int width, 
			int height,
			double intervalPos,
			int totalNumberOfPixelsRemaining,
			bool firstPass,
			int numOfPixels)
		{
			// Start position for the side is the index so that the starting point is tapered
			// unless this is the start of the first whirl or the we are drawing a meteor
			for (int pixel = firstPass || WhirlMode == WhirlpoolMode.Meteor ? 0 : index; pixel < numOfPixels; pixel++)
			{
				// Get the position of the next pixel
				(int XPos, int YPos) position = GetPixelPosition(Rotation, side, x, y, pixel, offset, index, width, height);

				// Get the color of the pixel
				Color color = GetColor(side, intervalPos, index, totalNumberOfPixelsRemaining);

				SetPixel(frameBuffer, position.XPos, position.YPos, color, totalNumberOfPixelsRemaining);

				// Reduce the number of pixels remaining
				totalNumberOfPixelsRemaining--;
			}

			// Return the total number of pixels remaining
			return totalNumberOfPixelsRemaining;
		}

		/// <summary>
		/// Calculates the number of pixels.
		/// This method adjusts the number of pixels based on if this is the last side to draw.
		/// All other sides stop one pixel short.  The last side therefore should be one pixel longer.
		/// </summary>
		/// <param name="pixels"></param>
		/// <param name="numberOfPixels"></param>
		/// <returns>Number of pixels to draw for a side</returns>
		private int CalcNumPixels(int pixels, int numberOfPixels)
		{
			// If drawing the whirl tail then...
			if (_drawingTail && pixels + 1 == numberOfPixels)
			{
				// Add an additional pixel since there are no more sides to draw
				return pixels + 1;
			}
			else
			{
				return pixels;
			}
		}
	}

	#endregion
}
