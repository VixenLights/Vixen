using System.ComponentModel;
using System.Drawing;

using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;

using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Whirlpool
{
	/// <summary>
	/// Renders the Whirlpool effect.
	/// </summary>
	public class Whirlpool : PixelEffectBase
	{
		#region Fields

		/// <summary>
		/// Data associated with the effect.
		/// </summary>
		private WhirlpoolData _data;

		/// <summary>
		/// Scaled height of the buffer.
		/// </summary>
		private int _scaledBufferHt;
		
		/// <summary>
		/// Scaled width of the buffer.
		/// </summary>
		private int _scaledBufferWi;

		/// <summary>
		/// Number of frames per iteration of the effect.
		/// </summary>
		private int _numberOfFramesPerIteration;

		/// <summary>
		/// Keeps track of the number of iteration frames.
		/// </summary>
		int _iterationFrame;

		/// <summary>
		/// Height of the display element.
		/// </summary>
		private int _bufferHt;

		/// <summary>
		/// Width of the display element.
		/// </summary>
		private int _bufferWi;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Whirlpool()
		{
			// Initialize the whirl collection
			Whirls = new WhirlCollection();

			// Create the data associated with the effect
			_data = new WhirlpoolData();

			// Enable Strings and Locations mode
			EnableTargetPositioning(true, true);
			
			// Default to a single whirlpool
			Columns = 1;
			Rows = 1;

			// Default to showing the completed whirl for 5% of the frames
			PauseAtEnd = 5;

			// Update the visibility of all the effect attributes
			UpdateAllAttributes();
		}

		#endregion

		#region String Setup Properties

		[Value]
		public override StringOrientation StringOrientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the fidelity of the rendering the whirlpool when in location mode.
		/// </summary>
		[Value]
		[ProviderCategory(@"Setup", 1)]
		[ProviderDisplayName(@"BorderRenderScaleFactor")]
		[ProviderDescription(@"BorderRenderScaleFactor")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10, 1)]
		[PropertyOrder(4)]
		public int RenderScaleFactor
		{
			get
			{
				return _data.RenderScaleFactor;
			}
			set
			{
				_data.RenderScaleFactor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Grid Panels Properties

		[Value]
		[ProviderCategory(@"WhirlpoolGridPanels", 1)]
		[ProviderDisplayName(@"WhirlpoolPanelColumns")]
		[ProviderDescription(@"WhirlpoolPanelColumns")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10, 1)]
		[PropertyOrder(1)]
		public int Columns
		{
			get { return _data.Columns; }
			set
			{
				_data.Columns = value;
				IsDirty = true;
				UpdateGridPanels();
				UpdateWhirlpoolPanelAttributes(true);
				OnPropertyChanged();
				OnPropertyChanged(nameof(Whirls));
			}
		}

		[Value]
		[ProviderCategory(@"WhirlpoolGridPanels", 1)]
		[ProviderDisplayName(@"WhirlpoolPanelRows")]
		[ProviderDescription(@"WhirlpoolPanelRows")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10, 1)]
		[PropertyOrder(2)]
		public int Rows
		{
			get { return _data.Rows; }
			set
			{
				_data.Rows = value;
				IsDirty = true;
				UpdateGridPanels();
				UpdateWhirlpoolPanelAttributes(true);
				OnPropertyChanged();
				OnPropertyChanged(nameof(Whirls));
			}
		}

		[Value]
		[ProviderCategory(@"WhirlpoolGridPanels", 1)]
		[ProviderDisplayName(@"WhirlpoolPanelSpacing")]
		[ProviderDescription(@"WhirlpoolPanelSpacing")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(3)]
		public int PanelSpacing
		{
			get { return _data.PanelSpacing; }
			set
			{
				_data.PanelSpacing = value;
				IsDirty = true;
				UpdateGridPanels();
				OnPropertyChanged();
				OnPropertyChanged(nameof(Whirls));
			}
		}

		[Value]
		[ProviderCategory(@"WhirlpoolGridPanels", 1)]
		[ProviderDisplayName(@"WhirlpoolPanelIndividualConfiguration")]
		[ProviderDescription(@"WhirlpoolPanelIndividualConfiguration")]
		[PropertyOrder(4)]
		public bool IndividualConfiguration
		{
			get { return _data.IndividualConfiguration; }
			set
			{
				_data.IndividualConfiguration = value;
				IsDirty = true;
				UpdateWhirlAttributes(true);
				OnPropertyChanged();
			}
		}

		#endregion

		#region Whirl Configuration Properties

		[Value]
		[ProviderCategory(@"WhirlpoolConfiguration", 1)]
		[ProviderDisplayName(@"WhirlpoolWhirlMode")]
		[ProviderDescription(@"WhirlpoolWhirlMode")]
		[PropertyOrder(0)]
		public WhirlpoolMode WhirlMode
		{
			get { return _data.WhirlMode; }
			set
			{
				_data.WhirlMode = value;
				IsDirty = true;
				UpdateWhirlMode(value);
				UpdateWhirlModeConfigurationAttributes(true);

				// If the whirlpool mode is meteor then...
				if (_data.WhirlMode == WhirlpoolMode.Meteor)
				{
					// Set the color mode to a single gradient
					ColorMode = WhirlpoolColorMode.GradientOverTime;
				}
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"WhirlpoolConfiguration", 1)]
		[ProviderDisplayName(@"WhirlpoolTailLength")]
		[ProviderDescription(@"WhirlpoolTailLength")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(1)]
		public int TailLength
		{
			get { return _data.TailLength; }
			set
			{
				_data.TailLength = value;
				IsDirty = true;
				UpdateTailLength(value);
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"WhirlpoolConfiguration", 1)]
		[ProviderDisplayName(@"WhirlpoolRotation")]
		[ProviderDescription(@"WhirlpoolRotation")]
		[PropertyOrder(2)]
		public WhirlpoolRotation Rotation
		{
			get { return _data.Rotation; }
			set
			{
				_data.Rotation = value;
				IsDirty = true;
				UpdateWhirlRotation(value);
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"WhirlpoolConfiguration", 1)]
		[ProviderDisplayName(@"WhirlpoolStartLocation")]
		[ProviderDescription(@"WhirlpoolStartLocation")]
		[PropertyOrder(3)]
		public WhirlpoolStartLocation StartLocation
		{
			get { return _data.StartLocation; }
			set
			{
				_data.StartLocation = value;
				IsDirty = true;
				UpdateStartLocation(value);
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"WhirlpoolConfiguration", 1)]
		[ProviderDisplayName(@"WhirlpoolDirection")]
		[ProviderDescription(@"WhirlpoolDirection")]
		[PropertyOrder(4)]
		public WhirlpoolDirection WhirlDirection
		{
			get { return _data.WhirlDirection; }
			set
			{
				_data.WhirlDirection = value;
				IsDirty = true;
				UpdateWhirlDirection(value);
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"WhirlpoolConfiguration", 1)]
		[ProviderDisplayName(@"WhirlpoolInverted")]
		[ProviderDescription(@"WhirlpoolInverted")]
		[PropertyOrder(5)]
		public bool Inverted
		{
			get { return _data.ReverseDraw; }
			set
			{
				_data.ReverseDraw = value;
				UpdateReverseDraw(value);
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"WhirlpoolConfiguration", 1)]
		[ProviderDisplayName(@"WhirlpoolSpacing")]
		[ProviderDescription(@"WhirlpoolSpacing")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(6)]
		public int Spacing
		{
			get { return _data.Spacing; }
			set
			{
				_data.Spacing = value;
				IsDirty = true;
				UpdateSpacing(value);
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"WhirlpoolConfiguration", 1)]
		[ProviderDescription(@"WhirlpoolThickness")]
		[ProviderDisplayName(@"WhirlpoolThickness")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(7)]
		public int Thickness
		{
			get { return _data.Thickness; }
			set
			{
				_data.Thickness = value;
				UpdateThickness(value);
				UpdateThicknessAttributes(true);
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"WhirlpoolConfiguration", 1)]
		[ProviderDisplayName(@"WhirlpoolShow3D")]
		[ProviderDescription(@"WhirlpoolShow3D")]
		[PropertyOrder(9)]
		public bool Show3D
		{
			get { return _data.Show3D; }
			set
			{
				_data.Show3D = value;
				UpdateShow3D(value);
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Configuration Properties

		[Value]
		[ProviderCategory(@"Config", 3)]
		[ProviderDisplayName(@"WhirlpoolIterations")]
		[ProviderDescription(@"WhirlpoolIterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10, 1)]
		[PropertyOrder(0)]
		public int Iterations
		{
			get { return _data.Iterations; }
			set
			{
				_data.Iterations = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 3)]
		[ProviderDisplayName(@"WhirlpoolPauseAtEnd")]
		[ProviderDescription(@"WhirlpoolPauseAtEnd")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(1)]
		public int PauseAtEnd
		{
			get { return _data.PauseAtEnd; }
			set
			{
				_data.PauseAtEnd = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Movement Properties

		[Value]
		[ProviderCategory(@"Movement", 3)]
		[ProviderDisplayName(@"WhirlpoolXScale")]
		[ProviderDescription(@"WhirlpoolXScale")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(5)]
		public int XScale
		{
			get { return _data.XScale; }
			set
			{
				_data.XScale = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 3)]
		[ProviderDisplayName(@"WhirlpoolYScale")]
		[ProviderDescription(@"WhirlpoolYScale")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(6)]
		public int YScale
		{
			get { return _data.YScale; }
			set
			{
				_data.YScale = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 3)]
		[ProviderDisplayName(@"WhirlpoolVerticalOffset")]
		[ProviderDescription(@"WhirlpoolVerticalOffset")]
		[PropertyOrder(7)]
		public Curve YOffsetCurve
		{
			get { return _data.YOffsetCurve; }
			set
			{
				_data.YOffsetCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 3)]
		[ProviderDisplayName(@"WhirlpoolHorizontalOffset")]
		[ProviderDescription(@"WhirlpoolHorizontalOffset")]
		[PropertyOrder(8)]
		public Curve XOffsetCurve
		{
			get { return _data.XOffsetCurve; }
			set
			{
				_data.XOffsetCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color Properties

		[Value]
		[ProviderCategory(@"Color", 4)]
		[ProviderDisplayName(@"WhirlpoolColorMode")]
		[ProviderDescription(@"WhirlpoolColorMode")]
		[PropertyOrder(0)]
		public WhirlpoolColorMode ColorMode
		{
			get { return _data.ColorMode; }
			set
			{
				_data.ColorMode = value;
				IsDirty = true;
				UpdateColorMode(value);
				UpdateColorModeAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 4)]
		[ProviderDisplayName(@"WhirlpoolColor")]
		[ProviderDescription(@"WhirlpoolColor")]
		[PropertyOrder(1)]
		public ColorGradient SingleColor
		{
			get { return _data.SingleColor; }
			set
			{
				_data.SingleColor = value;
				UpdateSingleColor(value);
				IsDirty = true;
				UpdateColorModeAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 4)]
		[ProviderDisplayName(@"WhirlpoolBandLength")]
		[ProviderDescription(@"WhirlpoolBandLength")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(1)]
		public int BandLength
		{
			get { return _data.BandLength; }
			set
			{
				_data.BandLength = value;
				UpdateBandLength(value);
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 4)]
		[ProviderDisplayName(@"WhirlpoolColors")]
		[ProviderDescription(@"WhirlpoolColors")]
		[PropertyOrder(2)]
		public List<ColorGradient> Colors
		{
			get { return _data.Colors; }
			set
			{
				_data.Colors = value;
				IsDirty = true;
				UpdateColors(value);
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 4)]
		[ProviderDisplayName(@"WhirlpoolLeftColor")]
		[ProviderDescription(@"WhirlpoolLeftColor")]
		[PropertyOrder(3)]
		public ColorGradient LeftColor
		{
			get
			{
				return _data.LeftColor;	
			}
			set
			{
				_data.LeftColor = value;
				UpdateLeftColor(value);
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 4)]
		[ProviderDisplayName(@"WhirlpoolRightColor")]
		[ProviderDescription(@"WhirlpoolRightColor")]
		[PropertyOrder(3)]
		public ColorGradient RightColor
		{
			get
			{
				return _data.RightColor;
			}
			set
			{
				_data.RightColor = value;
				UpdateRightColor(value);
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 4)]
		[ProviderDisplayName(@"WhirlpoolTopColor")]
		[ProviderDescription(@"WhirlpoolTopColor")]
		[PropertyOrder(3)]
		public ColorGradient TopColor
		{
			get
			{
				return _data.TopColor;
			}
			set
			{
				_data.TopColor = value;
				UpdateTopColor(value);
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 4)]
		[ProviderDisplayName(@"WhirlpoolBottomColor")]
		[ProviderDescription(@"WhirlpoolBottomColor")]
		[PropertyOrder(3)]
		public ColorGradient BottomColor
		{
			get
			{
				return _data.BottomColor;
			}
			set
			{
				_data.BottomColor = value;
				UpdateBottomColor(value);
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Whirls Collection

		private WhirlCollection _whirls;

		[ProviderCategory(@"Whirls", 2)]
		[ProviderDisplayName(@"Whirls")]
		[ProviderDescription(@"Whirls")]
		[PropertyOrder(2)]
		public WhirlCollection Whirls
		{
			get
			{
				return _whirls;
			}
			set
			{
				_whirls = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}

		#endregion

		#region Brightness Properties

		[Value]
		[ProviderCategory(@"Brightness", 5)]
		[ProviderDisplayName(@"WhirlpoolBrightness")]
		[ProviderDescription(@"WhirlpoolBrightness")]
		[PropertyOrder(0)]
		public Curve LevelCurve
		{
			get { return _data.Intensity; }
			set
			{
				_data.Intensity = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Public Properties

		/// </inheritdoc>
		public override IModuleDataModel ModuleData
		{
			get
			{
				// Update the serialized whirl data from the whirl collection
				UpdateWhirlData();

				return _data;
			}
			set
			{
				_data = value as WhirlpoolData;

				// Update the whirl model data
				UpdateWhirlModel(_data);

				// Update the visibility of the attributes
				UpdateAllAttributes();

				IsDirty = true;
			}
		}

		#endregion

		#region Information

		/// </inheritdoc>
		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		/// </inheritdoc>
		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/whirlpool/"; }
		}

		#endregion

		#region Protected Properties

		/// </inheritdoc>
		protected override EffectTypeModuleData EffectModuleData
		{
			get
			{
				// Update whirl serialized data from the whirl collection
				UpdateWhirlData();

				return _data;
			}
		}

		#endregion

		#region Protected Methods

		/// </inheritdoc>
		protected override void TargetPositioningChanged()
		{
			// Update the target position attributes 
			UpdateTargetPositionAttributes(true);
		}

		/// </inheritdoc>
		protected override void CleanUpRender()
		{
			// Nothing to clean up
		}

		/// <summary>
		/// Renders the Marquee border effect in location mode.
		/// </summary>
		/// <param name="numFrames">Total number of frames to render</param>
		/// <param name="frameBuffer">Frame buffer to render into</param>
		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			// Loop over the frames
			for (int frameNum = 0; frameNum < numFrames; frameNum++)
			{
				//Assign the current frame
				frameBuffer.CurrentFrame = frameNum;

				// Create a virtual matrix based on the rendering scale factor
				PixelFrameBuffer virtualFrameBuffer = new PixelFrameBuffer(_bufferWi, _bufferHt);

				// Render the effect in string mode into the virtual frame buffer
				RenderEffect(frameNum, virtualFrameBuffer);

				// Loop over all the location based pixel
				foreach (ElementLocation elementLocation in frameBuffer.ElementLocations)
				{
					// Transfer color information from the virtual frame buffer to the actual frame buffer
					UpdateFrameBufferForLocationPixel(
						elementLocation.X,
						elementLocation.Y,
						BufferHt,
						virtualFrameBuffer,
						frameBuffer);
				}
			}
		}

		/// </inheritdoc>
		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			// Increase the iteration frame
			_iterationFrame++;

			// If the iteration frame number is greater than the number of frames per iteration then...
			if (_iterationFrame > _numberOfFramesPerIteration)
			{
				// Reset the iteration frame counter
				_iterationFrame = 0;

				// Loop over the whirls
				foreach (Whirl whirl in Whirls)
				{
					// Reset the pixel number of the whirl
					whirl.ResetPixelNumber();
				}
			}

			// Get the position within the iteration
			double intervalPos = GetEffectTimeIntervalPosition(frame);
			double intervalPosFactor = intervalPos * 100.0;

			// Calculate the brightness of the effect
			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;

			// Loop over the whirls in the effect
			foreach (Whirl whirl in Whirls)
			{
				// If the whirl is enabled then...
				if (whirl.Enabled)
				{
					// Update the position of the whirl
					whirl.XOffset = CalculateXOffset(intervalPosFactor);
					whirl.YOffset = CalculateYOffset(intervalPosFactor);
					
					// Update the brightness of the whirl
					whirl.Intensity = level;

					// Render the whirl
					whirl.RenderEffect(_iterationFrame, frameBuffer, intervalPos);
				}
			}
		}

		/// </inheritdoc>
		protected override void SetupRender()
		{
			// Determine the logical render area based on the target positioning
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				// Adjust the virtual frame buffer based on the render scale factor
				_bufferWi = BufferWi / RenderScaleFactor;
				_bufferHt = BufferHt / RenderScaleFactor;

				// Need to increase the render height if the scale factor did not divide evenly
				// TODO: I THINK THIS LOGIC IS WRONG
				if (BufferHt % RenderScaleFactor != 0)
				{
					_bufferHt++;
				}

				// Need to increase the render width if the scale factor did not divide evenly
				// TODO: I THINK THIS LOGIC IS WRONG
				if (BufferWi % RenderScaleFactor != 0)
				{
					_bufferWi++;
				}
			}
			else
			{
				// In string mode just use the dimensions of the display element
				_bufferWi = BufferWi;
				_bufferHt = BufferHt;
			}

			// Calculate the scaled buffer size
			_scaledBufferWi = _bufferWi * XScale / 100;
			_scaledBufferHt = _bufferHt * YScale / 100;

			// Add and Remove Grid Panels to match effect settings
			UpdateGridPanels();

			// Reset the iteration frame counter
			_iterationFrame = 0;

			// Get the total number of frames for the effect duration
			int numberOfFrames = GetNumberFrames();

			// Determine the number of frames per iteration
			_numberOfFramesPerIteration = numberOfFrames / Iterations;

			// Loop over the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Give the whirl the number of frames to complete the whirl
				whirl.SetupRender(_numberOfFramesPerIteration);
				
				// Give the whirl the percentage of effect time to show the completed whirl
				whirl.PauseAtEnd = PauseAtEnd;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates the visibility of all the attributes.
		/// </summary>
		private void UpdateAllAttributes()
		{
			UpdateStringOrientationAttributes(false);
			UpdateColorModeAttribute(false);
			UpdateWhirlAttributes(false);
			UpdateWhirlModeConfigurationAttributes(false);
			UpdateTargetPositionAttributes(false);
			UpdateThicknessAttributes(false);
			UpdateWhirlpoolPanelAttributes(false);
			TypeDescriptor.Refresh(this);
		}

		/// <summary>
		/// Updates the target position attributes.
		/// </summary>
		/// <param name="refresh">Whether to refresh the editor</param>
		private void UpdateTargetPositionAttributes(bool refresh)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				// Render Scale Factor is only applicable to the Locations mode
				{ nameof(RenderScaleFactor), TargetPositioning == TargetPositioningType.Locations},
			};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Update whirl collection attributes.
		/// </summary>
		/// <param name="refresh">Whether to refresh the editor</param>
		private void UpdateWhirlAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			
			// Whirl collection is only visible when IndividualConfiguration flag is selected
			propertyStates.Add(nameof(Whirls), IndividualConfiguration);
		
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
			propertyStates.Add(nameof(Show3D), Thickness > 1);

			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Update whirlpool panel attributes.
		/// </summary>
		/// <param name="refresh">Whether to refresh the editor</param>
		private void UpdateWhirlpoolPanelAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2);

			// Individual configuration is only applicable if there is more than one whirl
			propertyStates.Add(nameof(IndividualConfiguration), Rows > 1 || Columns > 1);

			// Panel spacing is only applicable if ther eis more than one whirl
			propertyStates.Add(nameof(PanelSpacing), Rows > 1 || Columns > 1);
			
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
			propertyStates.Add(nameof(Inverted), WhirlMode != WhirlpoolMode.SymmetricalWhirls);

			// Color mode is not applicable Meteor mode
			propertyStates.Add(nameof(ColorMode), WhirlMode != WhirlpoolMode.Meteor);

			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
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
		/// Calculates the panel spacing as a percentage of the display element.
		/// </summary>
		/// <returns>Panel spacing as a percentage of the display element</returns>
		private int GetPanelSpacing()
		{
			return (int)(PanelSpacing / 100.0 * Math.Max(BufferWi, BufferHt));
		}

		/// <summary>
		/// Updates the whirls collection based on the row and column settings.
		/// </summary>
		private void UpdateGridPanels()
		{
			// Calculate the number of panels
			int panels = Rows * Columns;

			// If the whirl collection needs to shrink then...
			while (Whirls.Count > panels)
			{
				// Remove the last whirl
				Whirls.RemoveAt(Whirls.Count - 1);
			}

			// If there are too few whirls then...
			while (Whirls.Count < panels)
			{
				// If there is already a whirl in the collection then...
				if (Whirls.Count > 0)
				{
					// Clone the last whirl
					Whirls.Add((Whirl)Whirls[Whirls.Count - 1].Clone());
				}
				else
				{
					// Otherwise create a new whirl
					Whirls.Add(new Whirl());
				}
			}

			// Calculate the width of each whirl panel
			int panelWidth = (_scaledBufferWi - Columns * GetPanelSpacing()) / Columns;
			
			// Calculate the height of each whirl panel
			int panelHeight = (_scaledBufferHt - Rows * GetPanelSpacing()) / Rows;

			// Initialize a cell index
			int cellIndex = 0;

			// Loop over the rows in the grid 
			for (int r = 0; r < Rows; r++)
			{
				// Loop over the columns in the grid
				for (int c = 0; c < Columns; c++)
				{
					// Update the dimensions on the whirl
					Whirls[cellIndex].Width = panelWidth;
					Whirls[cellIndex].Height = panelHeight;

					// Update the space between the panels
					Whirls[cellIndex].X = c * panelWidth + GetPanelSpacing() * c;
					Whirls[cellIndex].Y = r * panelHeight + GetPanelSpacing() * r;
					
					// Advance to the next cell
					cellIndex++;
				}
			}
		}
		
		/// <summary>
		/// Calculate the X Offset of the effect.
		/// </summary>
		/// <param name="intervalPos">Position within the effect duration</param>
		/// <returns>Offset of the effect in the X axis</returns>
		private int CalculateXOffset(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), (int)(_bufferWi / 2), (int)(-_bufferWi / 2)));
		}

		/// <summary>
		/// Calculate the Y Offset of the effect.
		/// </summary>
		/// <param name="intervalPos">Position within the effect duration</param>
		/// <returns>Offset of the effect in the Y axis</returns>
		private int CalculateYOffset(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), (int)(_bufferHt / 2), (int)(-_bufferHt / 2)));
		}

		/// <summary>
		/// Converts from the serialized wave data to the model wave data.
		/// </summary>		
		private void UpdateWhirlModel(WhirlpoolData whirlpoolData)
		{
			// Clear the view model whirl collection
			Whirls.Clear();

			// Loop over the whirls in the serialized effect data
			foreach (WhirlData serializedWhirl in whirlpoolData.WhirlData)
			{
				// Create a new whirl in the model
				IWhirl whirl = new Whirl();

				// Transfer the properties from the serialized effect data to the whirl model
				whirl.Enabled = serializedWhirl.Enabled;
				whirl.X = serializedWhirl.X;
				whirl.Y = serializedWhirl.Y;
				whirl.Width = serializedWhirl.Width;
				whirl.Height = serializedWhirl.Height;
				whirl.WhirlMode = serializedWhirl.WhirlMode;
				whirl.TailLength = serializedWhirl.TailLength;
				whirl.StartLocation = serializedWhirl.StartLocation;
				whirl.WhirlDirection = serializedWhirl.WhirlDirection;
				whirl.Rotation = serializedWhirl.Rotation;
				whirl.ReverseDraw = serializedWhirl.ReverseDraw;
				whirl.Spacing = serializedWhirl.Spacing;
				whirl.Thickness = serializedWhirl.Thickness;
				whirl.Show3D = serializedWhirl.Show3D;
				//whirl.XOffset = serializedWhirl.XOffset;
				//whirl.YOffset = serializedWhirl.YOffset;
				whirl.ColorMode = serializedWhirl.ColorMode;
				whirl.BandLength = serializedWhirl.BandLength;
				whirl.LeftColor = serializedWhirl.LeftColor;
				whirl.RightColor = serializedWhirl.RightColor;
				whirl.TopColor = serializedWhirl.TopColor;
				whirl.BottomColor = serializedWhirl.BottomColor;
				whirl.SingleColor = serializedWhirl.SingleColor;

				whirl.Colors = serializedWhirl.Colors;

				//whirl.Intensity = serializedWhirl.
				//whirl.PauseAtEnd = serializedWhirl.

				// Add the whirl to the effect's collection
				Whirls.Add(whirl);
			}
		}

		/// <summary>
		/// Converts from the model whirl data to the serialized whirl data.
		/// </summary>
		private void UpdateWhirlData()
		{
			// Clear the collection of whirl serialized data
			_data.WhirlData.Clear();

			// Loop over the whirls in the model whirl collection
			foreach (IWhirl whirl in Whirls.ToList())
			{
				// Create a new serialized whirl data
				WhirlData serializedWhirl= new WhirlData();

				// Transfer the properties from the whirl model to the serialized whirl data
				serializedWhirl.Enabled = whirl.Enabled;
				serializedWhirl.X = whirl.X;
				serializedWhirl.Y = whirl.Y;
				serializedWhirl.Width = whirl.Width;
				serializedWhirl.Height = whirl.Height;
				serializedWhirl.WhirlMode = whirl.WhirlMode;
				serializedWhirl.TailLength = whirl.TailLength;
				serializedWhirl.StartLocation = whirl.StartLocation;
				serializedWhirl.WhirlDirection = whirl.WhirlDirection;
				serializedWhirl.Rotation = whirl.Rotation;
				serializedWhirl.ReverseDraw = whirl.ReverseDraw;
				serializedWhirl.Spacing = whirl.Spacing;
				serializedWhirl.Thickness = whirl.Thickness;
				serializedWhirl.Show3D = whirl.Show3D;
				//whirl.XOffset = serializedWhirl.XOffset;
				//whirl.YOffset = serializedWhirl.YOffset;
				serializedWhirl.ColorMode = whirl.ColorMode;
				serializedWhirl.BandLength = whirl.BandLength;
				serializedWhirl.LeftColor = whirl.LeftColor;
				serializedWhirl.RightColor = whirl.RightColor;
				serializedWhirl.TopColor = whirl.TopColor;
				serializedWhirl.BottomColor = whirl.BottomColor;
				serializedWhirl.SingleColor = whirl.SingleColor;

				serializedWhirl.Colors = whirl.Colors;
				//whirl.Intensity = serializedWhirl.
				//whirl.PauseAtEnd = serializedWhirl.



				// Add the serialized whirl to the collection
				_data.WhirlData.Add(serializedWhirl);
			}
		}

		/// <summary>
		/// Updates the frame buffer for a location based pixel.
		/// </summary>
		/// <param name="x">X coordinate of the pixel</param>
		/// <param name="y">Y coordinate of the pixel</param>
		/// <param name="bufferHt">Height of buffer</param>
		/// <param name="virtualFrameBuffer">Virtual frame buffer to retrieve color information from</param>
		/// <param name="frameBuffer">Frame buffer to update</param>
		private void UpdateFrameBufferForLocationPixel(int x, int y, int bufferHt, IPixelFrameBuffer virtualFrameBuffer, IPixelFrameBuffer frameBuffer)
		{
			// Save off the original location node
			int yCoord = y;
			int xCoord = x;

			// Flip me over so and offset my coordinates I can act like the string version
			y = Math.Abs((BufferHtOffset - y) + (bufferHt - 1 + BufferHtOffset));
			y = y - BufferHtOffset;
			x = x - BufferWiOffset;

			// Scale the location down to the render bitmap so that we can retrieve the color of the pixel
			int scaledX = x / RenderScaleFactor;
			int scaledY = y / RenderScaleFactor;

			// Retrieve the color from the bitmap
			Color color = virtualFrameBuffer.GetColorAt(scaledX, scaledY);

			// Set the pixel on the frame buffer
			frameBuffer.SetPixel(xCoord, yCoord, color);
		}

		/// <summary>
		/// Updates the start location on all the whirls.
		/// </summary>
		/// <param name="startLocation">Start location to assign to each whirl</param>
		private void UpdateStartLocation(WhirlpoolStartLocation startLocation)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the start location on the whirl
				whirl.StartLocation = startLocation;
			}
			
			// Refresh the collection of whirls
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the meteor tail length on all the whirls.
		/// </summary>
		/// <param name="tailLength">Meteor tail length to assign to each whirl</param>
		private void UpdateTailLength(int tailLength)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the meteor tail length
				whirl.TailLength = tailLength;
			}

			// Refresh the collection of whirls
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the whirl rotation (clockwise vs counter-clockwise) on all the whirls.
		/// </summary>
		/// <param name="rotation">Whirl rotation to assign to each whirl</param>
		private void UpdateWhirlRotation(WhirlpoolRotation rotation)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the whirl rotation
				whirl.Rotation = rotation;
			}

			// Refresh the collection of whirls
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the whirl direction (in vs out) on all the whirls.
		/// </summary>
		/// <param name="whirlDirection">Whirl direction to assign to each whirl</param>
		private void UpdateWhirlDirection(WhirlpoolDirection direction)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the whirl direction
				whirl.WhirlDirection = direction;
			}

			// Refresh the collection of whirls
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the whirl thickness on all the whirls.
		/// </summary>
		/// <param name="spacing">Whirl spacing to assign to each whirl</param>
		private void UpdateSpacing(int spacing)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the whirl spacing
				whirl.Spacing = spacing;
			}

			// Refresh the collection of whirls
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the whirl thickness on all the whirls.
		/// </summary>
		/// <param name="thickness">Whirl thickness to assign to each whirl</param>
		private void UpdateThickness(int thickness)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the whirl thickness
				whirl.Thickness = thickness;
			}

			// Refresh the collection of whirls
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the reverse draw setting on all the whirls.
		/// </summary>
		/// <param name="reverseDraw">Reverse draw setting to assign to each whirl</param>
		private void UpdateReverseDraw(bool reverseDraw)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update whether the whirl is drawn in reverse
				whirl.ReverseDraw = reverseDraw;
			}
			
			// Refresh the collection of whirls
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the show 3D setting on all the whirls.
		/// </summary>
		/// <param name="show3D">Show 3D setting to assign to each whirl</param>
		private void UpdateShow3D(bool show3D)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update whether the whirl is drawn in 3D
				whirl.Show3D = show3D;
			}

			// Refresh the collection of whirls
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the whirl mode on all the whirls.
		/// </summary>
		/// <param name="whirlMode">Whirl mode to assign to each whirl</param>
		private void UpdateWhirlMode(WhirlpoolMode whirlMode)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the whirl mode 
				whirl.WhirlMode = whirlMode;
			}

			// Refresh the collection of whirls
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the color mode on all the whirls.
		/// </summary>
		/// <param name="colorMode">Color mode to assign to each whirl</param>
		private void UpdateColorMode(WhirlpoolColorMode colorMode)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the color mode of the whirl
				whirl.ColorMode = colorMode;
			}
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the single color on all the whirls.
		/// </summary>
		/// <param name="color">Single color to assign to each whirl</param>
		private void UpdateSingleColor(ColorGradient color)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the single color of the whirl
				whirl.SingleColor = color;
			}
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the band length color on all the whirls.
		/// </summary>
		/// <param name="bandLength">Band length to assign to each whirl</param>
		private void UpdateBandLength(int bandLength)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the band length of the whirl
				whirl.BandLength = bandLength;
			}
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the left leg color on all the whirls.
		/// </summary>
		/// <param name="leftColor">Left leg color to assign to each whirl</param>
		private void UpdateLeftColor(ColorGradient leftColor)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the left color of the whirl
				whirl.LeftColor = leftColor;
			}
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the right leg color on all the whirls.
		/// </summary>
		/// <param name="rightColor">Right leg color to assign to each whirl</param>
		private void UpdateRightColor(ColorGradient rightColor)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the right color of the whirl
				whirl.RightColor = rightColor;
			}
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the top leg color on all the whirls.
		/// </summary>
		/// <param name="topColor">Top leg color to assign to each whirl</param>
		private void UpdateTopColor(ColorGradient topColor)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the top color of the whirl
				whirl.TopColor = topColor;
			}
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the bottom leg color on all the whirls.
		/// </summary>
		/// <param name="bottomColor">Bottom leg color to assign to each whirl</param>
		private void UpdateBottomColor(ColorGradient bottomColor)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the bottom color of the whirl
				whirl.BottomColor = bottomColor;
			}
			OnPropertyChanged(nameof(Whirls));
		}

		/// <summary>
		/// Updates the color collection on all the whirls.
		/// </summary>
		/// <param name="colors">Colors to assign to each whirl</param>
		private void UpdateColors(List<ColorGradient> colors)
		{
			// Loop over all the whirls
			foreach (Whirl whirl in Whirls)
			{
				// Update the color collection of the whirl
				whirl.Colors = colors;
			}
			OnPropertyChanged(nameof(Whirls));
		}

		#endregion
	}
}
