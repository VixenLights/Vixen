using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;

using Common.Controls.ColorManagement.ColorModels;

using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;

using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Effect.Weave;

using Color = System.Drawing.Color;

namespace VixenModules.Effect.Pattern
{
	/// <summary>
	/// Creates an effect that draws a pattern and animates it.
	/// </summary>
	public partial class Pattern : PixelEffectBase
	{
		#region Private Fields

		/// <summary>
		/// This frame buffer contains a tile of the weave.
		/// This tile can be repeated to fill the display element.
		/// </summary>
		private IPixelFrameBuffer _tileFrameBuffer;

		/// <summary>
		/// Scale factor used on all the tile pattern settings.
		/// </summary>
		private int _scaleValue;

		/// <summary>
		/// Height of the frame buffer.  When in string mode with a rotation this height has been increased to support
		/// rotating the original frame buffer.
		/// </summary>
		private int _bufferHt;

		/// <summary>
		/// Width of the frame buffer.  When in string mode with a rotation this width has been increased to support
		/// rotating the original frame buffer.
		/// </summary>
		private int _bufferWi;

		/// <summary>
		/// This field represents both the width and height of the frame buffer when a rotation is being applied.
		/// This field is only used in string mode.
		/// </summary>
		private int _length;

		/// <summary>
		/// Height of the repeating tile.
		/// </summary>
		private int _heightOfTile;

		/// <summary>
		/// Width of the repeating tile.
		/// </summary>
		private int _widthOfTile;

		/// <summary>
		/// Effect data (settings) associated with the effect.
		/// </summary>
		private WeaveData _data;
		
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Pattern()
		{
			// Create the data (settings) associated with the effect
			_data = new WeaveData();
			
			// Enable Target Position Controls (Positioning and Orientation)
			EnableTargetPositioning(true, true);
			
			// Initialize the enabled status of the controls
			InitAllAttributes();
		}

		#endregion

		#region Public (Override) Methods

		///<inheritdoc/>
		public override bool IsDirty
		{
			get
			{
				// If any linked color gradients have changed then...
				if (HorizontalColors.Any(x => !x.CheckLibraryReference()) ||
					VerticalColors.Any(x => !x.CheckLibraryReference()))
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set
			{
				base.IsDirty = value;
			}
		}

		#endregion

		#region Public Properties

		///<inheritdoc/>
		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as WeaveData;
				
				// Re-initialize all the controls after getting new module data
				InitAllAttributes();
				
				IsDirty = true;
			}
		}

		#endregion

		#region Public Setup Properties

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

		#endregion

		#region Public Config Properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"PatternPatternType")]
		[ProviderDescription(@"PatternPatternType")]
		[PropertyOrder(1)]
		public PatternTypes PatternType
		{
			get { return _data.PatternType; }
			set
			{
				_data.PatternType = value;
				UpdatePatternTypeAttributes();				
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"PatternDirection")]
		[PropertyOrder(2)]
		public WeaveDirection Direction
		{
			get { return _data.Direction; }
			set
			{
				_data.Direction = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Rotation")]
		[ProviderDescription(@"PatternRotation")]
		[PropertyOrder(3)]
		public Curve RotationAngle
		{
			get { return _data.RotationAngle; }
			set
			{
				_data.RotationAngle = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"PatternSpeed")]
		[ProviderDescription(@"PatternSpeed")]
		[PropertyOrder(4)]
		public Curve SpeedCurve
		{
			get { return _data.SpeedCurve; }
			set
			{
				_data.SpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Highlight")]
		[ProviderDescription(@"PatternHighlight")]
		[PropertyOrder(5)]
		public bool Highlight
		{
			get { return _data.Highlight; }
			set
			{
				_data.Highlight = value;
				IsDirty = true;
				OnPropertyChanged();
				UpdateHighlightAttributes();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"HighlightPercentage")]
		[ProviderDescription(@"HighlightPercentage")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(6)]
		public int HighlightPercentage
		{
			get { return _data.HighlightPercentage; }
			set
			{
				_data.HighlightPercentage = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Show3D")]
		[ProviderDescription(@"Show3D")]
		[PropertyOrder(7)]
		public bool Show3D
		{
			get { return _data.Show3D; }
			set
			{
				_data.Show3D = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"PatternWeaveAdvancedSizing")]
		[ProviderDescription(@"PatternWeaveAdvancedSizing")]
		[PropertyOrder(8)]
		public bool AdvancedSizing
		{
			get { return _data.AdvancedSizing; }
			set
			{
				_data.AdvancedSizing = value;

				// If Advanced sizing is being enabled then...
				if (value)
				{
					// Transfer the current spacing and thickness values to the individual sliders
					WeaveVerticalSpacing = WeaveSpacing;
					WeaveVerticalThickness = WeaveThickness;
					WeaveHorizontalSpacing = WeaveSpacing;
					WeaveHorizontalThickness = WeaveThickness;
				}

				IsDirty = true;
				OnPropertyChanged();

				UpdateSizingAttributes();
			}
		}

		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[ProviderCategory(@"Config", 4)]
		[ProviderDisplayName(@"PatternWeaveThickness")]
		[ProviderDescription(@"PatternWeaveThickness")]
		[PropertyOrder(9)]
		public int WeaveThickness
		{
			get { return _data.WeaveThickness; }
			set
			{
				_data.WeaveThickness = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[ProviderCategory(@"Config", 4)]
		[ProviderDisplayName(@"PatternWeaveSpacing")]
		[ProviderDescription(@"PatternWeaveSpacing")]
		[PropertyOrder(10)]
		public int WeaveSpacing
		{
			get { return _data.WeaveSpacing; }
			set
			{
				_data.WeaveSpacing = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[ProviderCategory(@"Config", 4)]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[ProviderDisplayName(@"PatternWeaveHorizontalThickness")]
		[ProviderDescription(@"PatternWeaveHorizontalThickness")]
		[PropertyOrder(11)]
		public int WeaveHorizontalThickness
		{
			get { return _data.WeaveHorizontalThickness; }
			set
			{
				_data.WeaveHorizontalThickness = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		
		[ProviderCategory(@"Config", 4)]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[ProviderDisplayName(@"PatternWeaveHorizontalSpacing")]
		[ProviderDescription(@"PatternWeaveHorizontalSpacing")]
		[PropertyOrder(12)]
		public int WeaveHorizontalSpacing
		{
			get { return _data.WeaveHorizontalSpacing; }
			set
			{
				_data.WeaveHorizontalSpacing = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		
		[ProviderCategory(@"Config", 4)]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[ProviderDisplayName(@"PatternWeaveVerticalThickness")]
		[ProviderDescription(@"PatternWeaveVerticalThickness")]
		[PropertyOrder(13)]
		public int WeaveVerticalThickness
		{
			get { return _data.WeaveVerticalThickness; }
			set
			{
				_data.WeaveVerticalThickness = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[ProviderCategory(@"Config", 4)]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[ProviderDisplayName(@"PatternWeaveVerticalSpacing")]
		[ProviderDescription(@"PatternWeaveVerticalSpacing")]
		[PropertyOrder(14)]
		public int WeaveVerticalSpacing
		{
			get { return _data.WeaveVerticalSpacing; }
			set
			{
				_data.WeaveVerticalSpacing = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[ProviderCategory(@"Config", 4)]
		[ProviderDisplayName(@"PatternBrickHeight")]
		[ProviderDescription(@"PatternBrickHeight")]
		[PropertyOrder(15)]
		public int BrickHeight
		{
			get { return _data.WeaveThickness; }
			set
			{
				_data.WeaveThickness = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[ProviderCategory(@"Config", 4)]
		[ProviderDisplayName(@"PatternBrickWidth")]
		[ProviderDescription(@"PatternBrickWidth")]
		[PropertyOrder(16)]
		public int BrickWidth
		{
			get { return _data.BrickWidth; }
			set
			{
				_data.BrickWidth = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[ProviderCategory(@"Config", 4)]
		[ProviderDisplayName(@"PatternBrickMortarHeight")]
		[ProviderDescription(@"PatternBrickMortarHeight")]
		[PropertyOrder(17)]
		public int MortarHeight
		{
			get { return _data.BrickHeight; }
			set
			{
				_data.BrickHeight = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"PatternBrickRotateBrick")]
		[ProviderDescription(@"PatternBrickRotateBrick")]
		[PropertyOrder(18)]
		public bool RotateBrick
		{
			get 
			{ 
				return _data.TransposeTile; 
			}
			set
			{
				_data.TransposeTile = value;
				
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color Properties

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"PatternWeaveHorizontalColors")]
		[ProviderDescription(@"PatternWeaveHorizontalColors")]
		[PropertyOrder(1)]
		public List<ColorGradient> HorizontalColors
		{
			get { return _data.HorizontalColors; }
			set
			{
				_data.HorizontalColors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"PatternWeaveVerticalColors")]
		[ProviderDescription(@"PatternWeaveVerticalColors")]
		[PropertyOrder(2)]
		public List<ColorGradient> VerticalColors
		{
			get { return _data.VerticalColors; }
			set
			{
				_data.VerticalColors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"PatternBrickColors")]
		[ProviderDescription(@"PatternBrickColors")]
		[PropertyOrder(3)]
		public List<ColorGradient> BrickColors
		{
			get { return _data.HorizontalColors; }
			set
			{
				_data.HorizontalColors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"PatternBrickMortarColor")]
		[ProviderDescription(@"PatternBrickMortarColor")]
		[PropertyOrder(4)]
		public ColorGradient MortarColor
		{
			get { return _data.MortarColor; }
			set
			{
				_data.MortarColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Level Properties

		[Value]
		[ProviderCategory(@"Brightness", 3)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set
			{
				_data.LevelCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Information

		/// <inheritdoc/>
		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		/// <inheritdoc/>
		public override string InformationLink
		{
			get { return "https://www.vixenlights.com/docs/usage/sequencer/effects/pixel/pattern/"; }
		}

		#endregion

		#region Protected Properties

		/// <inheritdoc/>
		protected override EffectTypeModuleData EffectModuleData => _data;

		#endregion

		#region Protected Methods

		/// <summary>
		/// Renders the effect in string mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			// Get the rotation angle
			double angle = GetRotationAngle(frame);

			// If the rotation angle is zero then...
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (angle == 0.0)
			{
				// Render the effect in the (original) frame buffer
				RenderEffectStringsInternal(frame, frameBuffer);
			}
			// Otherwise we are rotating the frame buffer
			else
			{
				// Create a virtual frame buffer that is large enough to rotate the original frame buffer within it
				IPixelFrameBuffer virtualFrameBuffer = new PixelFrameBuffer(_bufferWi, _bufferHt);

				// Render the effect in the virtual frame buffer
				RenderEffectStringsInternal(frame, virtualFrameBuffer);

				// Loop over the pixels in the original frame buffer
				for (int x = 0; x < BufferWi; x++)
				{
					for (int y = 0; y < BufferHt; y++)
					{
						// Get the rotated coordinate
						int rotatedX = x;
						int rotatedY = y;
						GetRotatedPosition(ref rotatedX, ref rotatedY, angle, false);

						// Set the original frame buffer pixel with the color at the rotated coordinate
						frameBuffer.SetPixel(x, y, virtualFrameBuffer.GetColorAt(rotatedX, rotatedY));
					}
				}
			}
		}

		/// <summary>
		/// Handler for when the target positioning changes.
		/// </summary>
		protected override void TargetPositioningChanged()
		{
			// Update the visibility of the highlight percentage
			UpdateHighlightAttributes();
		}

		/// <inheritdoc/>
		protected override void CleanUpRender()
		{
			// Nothing to clean up
		}

		/// <summary>
		/// Renders the effect using the pixel location frame buffer.
		/// </summary>
		/// <param name="numFrames">Number of frames to render</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			RenderEffectByLocationWeave(numFrames, frameBuffer);
		}

		/// <summary>
		/// Perform calculations that only need to be performed once per rendering.
		/// </summary>
		protected override void SetupRender()
		{
			// Calculate the diagonal length of the display element
            // Plus 2 is an extra margin to prevent a pixel location from going negative during a rotation due to rounding.
			_length = (int)Math.Round(Math.Sqrt((BufferHt * BufferHt) + (BufferWi * BufferWi)),
				MidpointRounding.AwayFromZero) + 2;

			// If the effect is in string mode then...
			if (TargetPositioning == TargetPositioningType.Strings)
			{
				// Create the rotation transform
				bool angleIsZero =
					RotationAngle.Points.Count == 2 &&
					// ReSharper disable once CompareOfFloatsByEqualityOperator
					RotationAngle.Points[0].X == 0.0 &&
					// ReSharper disable once CompareOfFloatsByEqualityOperator
					RotationAngle.Points[0].Y == 50.0 &&
					// ReSharper disable once CompareOfFloatsByEqualityOperator
					RotationAngle.Points[1].X == 100.0 &&
					// ReSharper disable once CompareOfFloatsByEqualityOperator
					RotationAngle.Points[1].Y == 50.0;

				// If there is no rotation then...
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (angleIsZero)
				{
					// Use the original frame buffer dimensions
					_bufferWi = BufferWi;
					_bufferHt = BufferHt;
				}
				else
				{
					// Use the larger virtual frame buffer
					_bufferWi = _length;
					_bufferHt = _length;
				}
			}
			// Otherwise use the original frame buffer
			else
			{
				_bufferWi = BufferWi;
				_bufferHt = BufferHt;
			}

			// Determine the minimum between the display element height and width
			_scaleValue = GetScaleValue(BufferHt, BufferWi);	
			
			// If the pattern type is Weave then...
			if (PatternType == PatternTypes.Weave)
			{
				InitializeTile(0);
			}	
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Renders the effect in string mode using the specified frame buffer.
		/// </summary>
		/// <param name="frame">Frame to render</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectStringsInternal(int frame, IPixelFrameBuffer frameBuffer)
		{
			// If the pattern type is brick then...
			// The Brick pattern mortar is gradient sampled each frame
			if (PatternType == PatternTypes.Brick)
			{
				// Draw the repeating tile
				InitializeTile(frame);
			}

			// Render the pattern effect in string mode
			RenderEffectStringsWeave(frame, frameBuffer);
		}

		/// <summary>
		/// Initializes the pattern tile.
		/// </summary>	
		/// <param name="frame">Current frame within the effect duration</param>		
		private void InitializeTile(int frame)
		{
			// If the selected pattern is the Weave then...
			if (PatternType == PatternTypes.Weave)
			{
				// Initialize the Weave Tile 
				InitializeWeaveTile();
			}	
			else
			{
				// Initialize the Brick Tile
				InitializeBrickTile(frame);
			}
		}
				
		/// <summary>
		/// Initializes the visibility of the attributes.
		/// </summary>
		private void InitAllAttributes()
		{
			UpdateStringOrientationAttributes(true);
			UpdateHighlightAttributes(false);
			UpdateSizingAttributes(false);
			UpdatePatternTypeAttributes(false);

			TypeDescriptor.Refresh(this);
		}

		/// <summary>
		/// Updates the visibility of controls based on selected pattern type.
		/// </summary>
		/// <param name="refresh">Whether to refresh the view</param>
		private void UpdatePatternTypeAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>()
			{						
				{ nameof(AdvancedSizing), PatternType == PatternTypes.Weave },
				{ nameof(WeaveThickness),  PatternType == PatternTypes.Weave },
				{ nameof(WeaveSpacing),  PatternType == PatternTypes.Weave },
				{ nameof(HorizontalColors), PatternType == PatternTypes.Weave },
				{ nameof(VerticalColors), PatternType == PatternTypes.Weave },

				{ nameof(BrickHeight), PatternType == PatternTypes.Brick },
				{ nameof(BrickWidth), PatternType == PatternTypes.Brick },
				{ nameof(MortarHeight), PatternType == PatternTypes.Brick },
				{ nameof(MortarColor), PatternType == PatternTypes.Brick },
				{ nameof(RotateBrick), PatternType == PatternTypes.Brick },
				{ nameof(BrickColors), PatternType == PatternTypes.Brick },
			};

			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Updates the visibility of highlight attributes.
		/// </summary>
		private void UpdateHighlightAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				// Highlight percentage is only visible when Highlight is selected and we are in Location mode
				{ nameof(HighlightPercentage), Highlight && TargetPositioning == TargetPositioningType.Locations },
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Updates the visibility of Advanced Sizing attributes.
		/// </summary>
		/// <param name="refresh">Whether to refresh the view</param>
		private void UpdateSizingAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(6)
			{
				// Highlight percentage is only visible when Highlight is selected and we are in Location mode
				{ nameof(WeaveHorizontalSpacing), AdvancedSizing },
				{ nameof(WeaveVerticalSpacing), AdvancedSizing },
				{ nameof(WeaveHorizontalThickness), AdvancedSizing },
				{ nameof(WeaveVerticalThickness), AdvancedSizing },
				{ nameof(WeaveSpacing), !AdvancedSizing },
				{ nameof(WeaveThickness), !AdvancedSizing },
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}
				
		/// <summary>
		/// Renders the weave bars in strings mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectStringsWeave(int frame, IPixelFrameBuffer frameBuffer)
		{
			// Render the weave based on the direction
			switch (Direction)
			{
				case WeaveDirection.Up:
				case WeaveDirection.Down:
                    RenderEffectWeaveVertical(frame, frameBuffer);
                    break;
				case WeaveDirection.Left:
				case WeaveDirection.Right:
					RenderEffectWeaveHorizontal(frame, frameBuffer);
					break;
				case WeaveDirection.Expand:
					RenderEffectWeaveVerticalExpand(frame, frameBuffer);
					break;
				case WeaveDirection.Compress:
					RenderEffectWeaveVerticalCompress(frame, frameBuffer);
					break;
				case WeaveDirection.HorizontalExpand:
					RenderEffectWeaveHorizontalExpand(frame, frameBuffer);
					break;
				case WeaveDirection.HorizontalCompress:
					RenderEffectWeaveHorizontalCompress(frame, frameBuffer);
					break;
				case WeaveDirection.CenterCompress:
					RenderEffectWeaveVerticalCompressCenter(frame, frameBuffer);
					break;
				case WeaveDirection.CenterExpand:
					RenderEffectWeaveVerticalExpandCenter(frame, frameBuffer);
					break;
				default:
					Debug.Assert(false, "Unsupported Direction!");
					break;
			}
		}

		/// <summary>
		/// Renders the weave bars in location mode.
		/// </summary>
		/// <param name="numFrames">Number of frames to render</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectByLocationWeave(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			// Loop over all the frames
			for (int frame = 0; frame < numFrames; frame++)
			{
				// If the pattern type is Brick then...
				if (PatternType == PatternTypes.Brick)
				{
					// Draw the repeating tile
					InitializeTile(frame);
				}

				// Set the current frame number
				frameBuffer.CurrentFrame = frame;

				switch (Direction)
				{
					case WeaveDirection.Up:
					case WeaveDirection.Down:
						RenderEffectLocationWeaveVertical(frame, frameBuffer);
						break;
					case WeaveDirection.Left:
					case WeaveDirection.Right:
						RenderEffectLocationWeaveHorizontal(frame, frameBuffer);
						break;
					case WeaveDirection.Expand:
						RenderEffectLocationWeaveVerticalExpand(frame, frameBuffer);
						break;
					case WeaveDirection.Compress:
						RenderEffectLocationWeaveVerticalCompress(frame, frameBuffer);
						break;
					case WeaveDirection.HorizontalExpand:
						RenderEffectLocationWeaveHorizontalExpand(frame, frameBuffer);
						break;
					case WeaveDirection.HorizontalCompress:
						RenderEffectLocationWeaveHorizontalCompress(frame, frameBuffer);
						break;
					case WeaveDirection.CenterCompress:
						RenderEffectLocationWeaveVerticalCompressCenter(frame, frameBuffer);
						break;
					case WeaveDirection.CenterExpand:
						RenderEffectLocationWeaveVerticalExpandCenter(frame, frameBuffer);
						break;
					default:
						Debug.Assert(false, "Unsupported Direction!");
						break;
				}
			}
		}

		/// <summary>
		/// Defines a delegate so the algorithm used to initialize the tile Y position.
		/// </summary>
		/// <param name="heightOfTile">Height of the repeating tile</param>		
		/// <param name="frame">Current frame number</param>
		/// <returns>Initial Y position</returns>
		private delegate int InitializeTileYPosition(
			int heightOfTile,
			int frame);

		/// <summary>
		/// Initializes the Y position within the weave repeating tile.
		/// </summary>
		/// <param name="heightOfTile">Height of the tile</param>
		/// <param name="frame">Current frame number</param>
		/// <returns>Initial Y position within the repeating tile</returns>
		private int InitializeWeaveTileYPosition(
			int heightOfTile,
			int frame)
		{
			// Calculate the position within the repeating tile
			return GetWeaveYValue(frame) % heightOfTile;
		}

		/// <summary>
		/// Updates the Y position within the weave repeating tile.
		/// </summary>
		/// <param name="increment">Whether the weave is incrementing through the tile</param>
		/// <param name="tileY">Current Y position within the tile</param>
		/// <param name="heightOfTile">Height of the repeating tile</param>
		/// <returns></returns>
		private int UpdateWeaveTileYPosition(bool increment, int tileY, int heightOfTile)
		{
			// If the weave is incrementing then...
			if (increment)
			{
				// Increment the tile Y position
				tileY++;

				// If we have reached the end of the tile then...
				if (tileY == heightOfTile)
				{
					// Go back to the beginning of the tile
					tileY = 0;
				}
			}
			// Otherwise the weave is decrementing
			else
			{
				// Decrement the tile Y position
				tileY--;

				// If we have reached the beginning of the tile then...
				if (tileY < 0)
				{
					// Go back to the end of the tile
					tileY = heightOfTile - 1;
				}
			}

			return tileY;
		}

		/// <summary>
		/// Sets the specified horizontal pixel on the frame buffer from the specified pixel from the repeating tile.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		/// <param name="x">X position of the target frame buffer</param>
		/// <param name="y">Y position of the target frame buffer</param>
		/// <param name="tileX">X position within the repeating tile</param>
		/// <param name="tileY">Y position within the repeating tile</param>
		/// <param name="yTileStartPosition">Start offset into the repeating tile</param>
		private void SetPixelHorizontal(int frame, IPixelFrameBuffer frameBuffer, int x, int y, int tileX, int tileY,
			int yTileStartPosition)
		{
			// Transfer a pixel from the tile to the frame buffer
			frameBuffer.SetPixel(y, x,
				AdjustIntensity(_tileFrameBuffer.GetColorAt(tileX, tileY + yTileStartPosition), frame));
		}

		/// <summary>
		/// Sets the specified vertical pixel on the frame buffer from the specified pixel from the repeating tile.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		/// <param name="x">X position of the target frame buffer</param>
		/// <param name="y">Y position of the target frame buffer</param>
		/// <param name="tileX">X position within the repeating tile</param>
		/// <param name="tileY">Y position within the repeating tile</param>
		/// <param name="yTileStartPosition">Start offset into the repeating tile</param>
		private void SetPixelVertical(int frame, IPixelFrameBuffer frameBuffer, int x, int y, int tileX, int tileY,
			int yTileStartPosition)
		{
			// Transfer a pixel from the tile to the frame buffer
			frameBuffer.SetPixel(x, y,
				AdjustIntensity(_tileFrameBuffer.GetColorAt(tileX, tileY + yTileStartPosition), frame));
		}

		/// <summary>
		/// Defines a delegate for converting from location coordinates to string coordinates.
		/// </summary>
		/// <param name="x">X location coordinate</param>
		/// <param name="y">Y location coordinate</param>
		/// <param name="xOut">X string coordinate</param>
		/// <param name="yOut">Y string coordinate</param>
		private delegate void ConvertFromLocationToStringCoordinates(int x, int y, out int xOut, out int yOut);

		/// <summary>
		/// Calculates the pixel color for the specified coordinate.  
		/// </summary>
		/// <param name="tileFrameBuffer">Applicable tile frame buffer</param>
		/// <param name="x">X Coordinate</param>
		/// <param name="y">Y Coordinate</param>
		/// <param name="frame">Current frame number</param>
		/// <param name="heightOfTile"></param>
		/// <param name="widthOfTile"></param>
		/// <param name="movesRight"></param>
		/// <param name="yTileStartPosition"></param>
		/// <param name="movementY"></param>
		/// <returns>Color of the specified pixel</returns>
		private delegate Color CalculatePixelColor(
			IPixelFrameBuffer tileFrameBuffer,
			int x,
			int y,
			int frame,
			int heightOfTile,
			int widthOfTile,
			bool movesRight,
			int yTileStartPosition,
			int movementY);

		/// <summary>
		/// Calculates the color for a pixel that is part of a moving bar.		
		/// </summary>
		/// <param name="tileFrameBuffer">Applicable tile buffer</param>
		/// <param name="x">X Coordinate</param>
		/// <param name="y">Y Coordinate</param>
		/// <param name="frame">Current frame number</param>
		/// <param name="heightOfTile">Height of the repeating tile</param>
		/// <param name="widthOfTile">Width of the repeating tile</param>
		/// <param name="movesRight">Whether the movement is to the right</param>
		/// <param name="yTileStartPosition">Y Offset into the repeating tile</param>
		/// <param name="movementY">Y Movement speed factor</param>
		/// <returns>Color of the pixel</returns>
		private Color CalculateMovingColor(
			IPixelFrameBuffer tileFrameBuffer,
			int x,
			int y,
			int frame,
			int heightOfTile,
			int widthOfTile,
			bool movesRight,
			int yTileStartPosition,
			int movementY)
		{
			// Calculate the position in the tile for the specified Y coordinate
			int yTile = CalculateWeaveYTilePosition(
				movesRight,
				y,
				movementY,
				heightOfTile);

			// Calculate the position in the tile for the specified X coordinate
			int xTile = x % widthOfTile;

			Color movingColor;

			// If the bar is moving right then...
			if (movesRight)
			{
				// Retrieve the pixel from the tile taking into account the orientation of the tile
				// and any applicable offsets to skip
				movingColor = tileFrameBuffer.GetColorAt(xTile, yTile + yTileStartPosition);
			}
			// Otherwise the bar is moving left
			else
			{
				// Retrieve the pixel from the tile taking into account the orientation of the tile
				// and any applicable offsets to skip
				movingColor = tileFrameBuffer.GetColorAt(xTile, (heightOfTile - 1) - yTile + yTileStartPosition);
			}

			return movingColor;
		}

		#endregion

		#region Private Common Render Location Methods

		/// <summary>
		/// Renders an expanding or compressing bars.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		/// <param name="downIncreases">True when the weave expands on the lower/right side</param>
		/// <param name="heightOfTile">Height of the repeating tile</param>
		/// <param name="widthOfTile">Width of the repeating tile</param>
		/// <param name="yTileStartPosition">Y start position within the repeating tile frame buffer</param>
		/// <param name="angle">Angle of the rotation</param>
		/// <param name="swapXY">True when the effect is using a Horizontal Direction</param>
		/// <param name="convertFromLocationToStringCoordinates">Delegate to convert from location coordinates to string coordinates</param>
		/// <param name="tileFrameBuffer">Tile frame buffer to retrieve colors from</param>
		/// <param name="initializeTileYPosition">Method to determine the initial Y position</param>
		/// <param name="height">Height of the render frame buffer</param>
		private void RenderEffectLocationExpandCompress(
			int frame,
			PixelLocationFrameBuffer frameBuffer,
			bool downIncreases,
			int heightOfTile,
			int widthOfTile,
			int yTileStartPosition,
			double angle,
			// ReSharper disable once InconsistentNaming
			bool swapXY,
			ConvertFromLocationToStringCoordinates convertFromLocationToStringCoordinates,
			IPixelFrameBuffer tileFrameBuffer,
			InitializeTileYPosition initializeTileYPosition,
            int height)
		{
			// Calculate the position within the tile based on frame movement
			int movementY = initializeTileYPosition(heightOfTile, frame);

			// Loop over the location nodes
			foreach (ElementLocation node in frameBuffer.ElementLocations)
			{
				// Convert from location based coordinate to string coordinate
				int x;
				int y;
				convertFromLocationToStringCoordinates(node.X, node.Y, out x, out y);

				// If there is a rotation then...
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (angle != 0)
				{
					// Rotate the point
					GetRotatedPosition(ref x, ref y, angle, swapXY);
					
					// When a rotation is being performed the (logical) frame buffer is enlarged 
					// to be a square frame buffer large enough to allow the original string frame buffer to
                    // be rotated within the enlarged frame buffer.	
					height = _length;					
				}
			
				// Determine if we are in the lower half of the display element
                bool down = (y < height / 2);

				// Calculate the position in the tile for the specified Y coordinate
				int yTile = CalculateWeaveYTilePosition(
					downIncreases ? down : !down,
					y,
					movementY,
					heightOfTile,
                    height / 2);

				// Update the x position within the tile
				int repeatingX = x % widthOfTile;

				// If the node is in the lower (right) half of the display element then...
				if (down)
				{
					// Offset the coordinate and ensure we are still within the tile
                    yTile = (height / 2 - yTile) % heightOfTile;

					// Ensure the yTile position is not negative
					if (yTile < 0)
					{
						yTile += heightOfTile;
					}
				}

				// If the direction is compressing then...
				if (Direction == WeaveDirection.Compress ||
				    Direction == WeaveDirection.HorizontalCompress)
				{
					// Transfer a pixel from the tile to the frame buffer
					frameBuffer.SetPixel(node.X, node.Y,
						AdjustIntensity(
							tileFrameBuffer.GetColorAt(repeatingX, yTile + yTileStartPosition),
							frame));
				}
				else
				{
					// Transfer a pixel from the tile to the frame buffer
					frameBuffer.SetPixel(node.X, node.Y,
						AdjustIntensity(
							tileFrameBuffer.GetColorAt(repeatingX, (heightOfTile - 1) - yTile + yTileStartPosition),
							frame));
				}
			}
		}

		/// <summary>
		/// Renders an expanding or compressing bars.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		/// <param name="downIncreases">True when the weave expands on the lower/right side</param>
		/// <param name="heightOfTile">Height of the repeating tile</param>
		/// <param name="widthOfTile">Width of the repeating tile</param>
		/// <param name="yTileStartPosition">Y start position within the repeating tile frame buffer</param>
		/// <param name="angle">Angle of the rotation</param>
		/// <param name="swapXY">True when the effect is using a Horizontal Direction</param>
		/// <param name="convertFromLocationToStringCoordinates">Delegate to convert from location coordinates to string coordinates</param>
		/// <param name="tileFrameBuffer">Tile frame buffer to retrieve colors from</param>
		/// <param name="initializeTileYPosition">Method to determine the initial Y position</param>
		/// <param name="height">Height of the render frame buffer</param>
		private void RenderEffectLocationExpandCompressCenter(
			int frame,
			PixelLocationFrameBuffer frameBuffer,
			bool downIncreases,
			int heightOfTile,
			int widthOfTile,
			int yTileStartPosition,
			double angle,
			// ReSharper disable once InconsistentNaming
			bool swapXY,
			ConvertFromLocationToStringCoordinates convertFromLocationToStringCoordinates,
			IPixelFrameBuffer tileFrameBuffer,
			InitializeTileYPosition initializeTileYPosition,
			int height,
			int width)
		{
			// Calculate the position within the tile based on frame movement
			int movementY = initializeTileYPosition(heightOfTile, frame);
			int movementX = initializeTileYPosition(widthOfTile, frame);

			// Loop over the location nodes
			foreach (ElementLocation node in frameBuffer.ElementLocations)
			{
				// Convert from location based coordinate to string coordinate
				int x;
				int y;
				convertFromLocationToStringCoordinates(node.X, node.Y, out x, out y);

				// If there is a rotation then...
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (angle != 0)
				{
					// Rotate the point
					GetRotatedPosition(ref x, ref y, angle, swapXY);

					// When a rotation is being performed the (logical) frame buffer is enlarged 
					// to be a square frame buffer large enough to allow the original string frame buffer to
                    // be rotated within the enlarged frame buffer.
					height = _length;
					width = _length;
				}

				// Determine if we are in the lower half of the display element
				bool down = (y < height / 2);
				bool xDown = (x < width / 2);

				// Calculate the position in the tile for the specified Y coordinate
				int yTile = CalculateWeaveYTilePosition(
					downIncreases ? down : !down,
					y,
					movementY,
					heightOfTile,
					height / 2);

				// Calculate the position in the tile for the specified x coordinate
				int xTile = CalculateWeaveYTilePosition(
					downIncreases ? xDown : !xDown,
					x,
					movementX,
					widthOfTile,
					width / 2);

				// Update the x position within the tile
				int repeatingX = xTile % widthOfTile;
				
				// If the node is in the lower (right) half of the display element then...
				if (xDown)
				{
					// Offset the coordinate and ensure we are still within the tile
					repeatingX = (width / 2 - repeatingX) % widthOfTile;

					// Ensure the yTile position is not negative
					if (repeatingX < 0)
					{
						repeatingX += widthOfTile;
					}
				}

				// If the node is in the lower (right) half of the display element then...
				if (down)
				{
					// Offset the coordinate and ensure we are still within the tile
					yTile = (height / 2 - yTile) % heightOfTile;

					// Ensure the yTile position is not negative
					if (yTile < 0)
					{
						yTile += heightOfTile;
					}
				}

				// If the direction is compressing then...
				if (Direction == WeaveDirection.Compress ||
					Direction == WeaveDirection.HorizontalCompress)
				{
					// Transfer a pixel from the tile to the frame buffer
					frameBuffer.SetPixel(node.X, node.Y,
						AdjustIntensity(
							tileFrameBuffer.GetColorAt(repeatingX, yTile + yTileStartPosition),
							frame));
				}
				else
				{
					// Transfer a pixel from the tile to the frame buffer
					frameBuffer.SetPixel(node.X, node.Y,
						AdjustIntensity(
							tileFrameBuffer.GetColorAt(repeatingX, (heightOfTile - 1) - yTile + yTileStartPosition),
							frame));
				}
			}
		}

		/// <summary>
		/// Renders bars for location mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>       
		/// <param name="heightOfTile">Height of the repeating tile</param>
		/// <param name="widthOfTile">Width of the repeating tile</param>
		/// <param name="yTileStartPosition">Y start position within the repeating tile frame buffer</param>
		/// <param name="leftRight">Whether the weave is moving left or right</param>
		/// <param name="movesRight">Whether the weave is moving right</param>
		/// <param name="swapXY">True when the effect is using a Horizontal Direction</param>
		/// <param name="convertFromLocationToStringCoordinates">Delegate to convert location coordinates to string coordinates</param>
		/// <param name="tileFrameBuffer"></param>
		/// <param name="initializeTileYPosition"></param>
		/// <param name="calculatePixelColor"></param>
		private void RenderEffectByLocation(
			int frame,
			PixelLocationFrameBuffer frameBuffer,
			int heightOfTile,
			int widthOfTile,
			int yTileStartPosition,
			bool leftRight,
			bool movesRight,
			// ReSharper disable once InconsistentNaming
			bool swapXY,
			ConvertFromLocationToStringCoordinates convertFromLocationToStringCoordinates,
			IPixelFrameBuffer tileFrameBuffer,
			InitializeTileYPosition initializeTileYPosition,
			CalculatePixelColor calculatePixelColor)
		{
			// Get the rotation angle
			double angle = GetRotationAngle(frame);

			// If the effect is in horizontal mode then...
			if (swapXY)
			{
				// Negate the rotation angle
				angle = -angle;
			}

			// Initialize the Y position within the repeating tile 
			int movementY = initializeTileYPosition(
				heightOfTile,
				frame);

			// Loop over the location nodes
			foreach (ElementLocation node in frameBuffer.ElementLocations)
			{
				// Convert from location based coordinate to string coordinate
				int x;
				int y;
				convertFromLocationToStringCoordinates(node.X, node.Y, out x, out y);

				// If there is a rotation then...
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (angle != 0.0)
				{
					// Rotate the point by the specified angle
					GetRotatedPosition(ref x, ref y, angle, swapXY);
				}

				// Look up the color for the point in the repeating tile
				Color color = calculatePixelColor(
					tileFrameBuffer,
					x,
					y,
					frame,
					heightOfTile,
					widthOfTile,
					movesRight,
					yTileStartPosition,
					movementY);

				// Transfer a pixel from the tile to the frame buffer          
				frameBuffer.SetPixel(node.X, node.Y, AdjustIntensity(color, frame));
			}
		}

		#endregion

		#region Private Weave Render String Methods

		/// <summary>
		/// Renders a vertical moving weave for string mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectWeaveVertical(int frame, IPixelFrameBuffer frameBuffer)
		{
			// Render the vertical moving weave
			RenderEffectWeave(
				frame,
				frameBuffer,
				_heightOfTile,
				_widthOfTile,
				0,
				(Direction == WeaveDirection.Down || Direction == WeaveDirection.Up),
				(Direction == WeaveDirection.Down),
				_bufferHt,
				_bufferWi,
				SetPixelVertical);
		}

		/// <summary>
		/// Renders a horizontal moving weave for string mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectWeaveHorizontal(int frame, IPixelFrameBuffer frameBuffer)
		{
			// Render the horizontal moving weave
			RenderEffectWeave(
				frame,
				frameBuffer,
				_heightOfTile,
				_widthOfTile,
				0,
				(Direction == WeaveDirection.Left || Direction == WeaveDirection.Right),
				(Direction == WeaveDirection.Left),
				_bufferWi,
				_bufferHt,
				SetPixelHorizontal);
		}

		/// <summary>
		/// Renders a weave for string mode.
		/// </summary>             
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		/// <param name="heightOfTile">Height of the repeating tile</param>
		/// <param name="widthOfTile">Width of the repeating tile</param>
		/// <param name="yTileStartPosition">Y start position within the repeating tile</param>
		/// <param name="leftRight">Whether the weave is moving left or right</param>
		/// <param name="moveRight">Whether the weave is moving right</param>
		/// <param name="yLength">Length of the logical display element</param>
		/// <param name="xLength">Width of the logical display element</param>
		/// <param name="setPixel">Delegate that sets a pixel on the frame buffer</param>
		private void RenderEffectWeave(
			int frame,
			IPixelFrameBuffer frameBuffer,
			int heightOfTile,
			int widthOfTile,
			int yTileStartPosition,
			bool leftRight,
			bool moveRight,
			int yLength,
			int xLength,
			Action<int, IPixelFrameBuffer, int, int, int, int, int> setPixel)
		{
			// Initialize the Y position within the weave repeating tile 
			int tileY = InitializeWeaveTileYPosition(
				heightOfTile,
				frame);

			// Loop over the Y-axis of the buffer
			for (int y = 0; y < yLength; y++)
			{
				// Loop over the X-axis of the buffer
				for (int x = 0; x < xLength; x++)
				{
					// Calculate the X position within the repeating tile
					int tileX = x % widthOfTile;

					// Transfer a pixel from the tile to the frame buffer                   
					setPixel(frame, frameBuffer, x, y, tileX, tileY, yTileStartPosition);
				}

				// Update the repeating tile Y position
				tileY = UpdateWeaveTileYPosition(
					moveRight,
					tileY,
					heightOfTile);
			}
		}

		private int GetXPositionIncrementing(int x, int xLength)
		{
			return x;
		}

		private int GetXPositionDecrementing(int x, int xLength)
		{
			return xLength - x - 1;
		}

		/// <summary>
		/// Draws an x-axis row on a quadrant of the display element.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		/// <param name="expand">True when the weave is expanding</param>        
		/// <param name="heightOfTile">Height of the repeating tile</param>
		/// <param name="widthOfTile">Width of the repeating tile</param>
		/// <param name="y">Current y coordinate of the row being drawn</param>
		/// <param name="xLength">Width of the display element</param>
		/// <param name="startTileX">Where in the repeating tile to start sampling values from</param>
		/// <param name="tileY">Y-axis index into the repeating tile</param>
		/// <param name="yTileStartPosition">(Optional) y axis offset into repeating tile</param>
		/// <param name="setPixel">Delegate that sets a pixel on the frame buffer</param>
		/// <param name="getXPosition">Delegate the determines x coordinate into the frame buffer</param>
		private void DrawQuadrantRow(
			int frame,
			IPixelFrameBuffer frameBuffer,
			bool expand,
			int heightOfTile,
			int widthOfTile,
			int y,
			int xLength,
			int startTileX,
			int tileY,
			int yTileStartPosition,
			Action<int, IPixelFrameBuffer, int, int, int, int, int> setPixel,
			Func<int, int, int> getXPosition)
		{
			// Loop over the top left quadrant of the display element
			for (int x = 0; x < xLength / 2; x++)
			{
				int tileX;

				if (expand)
				{
					// Update the x position within the tile
					tileX = (x + startTileX) % widthOfTile;
				}
				else
				{
					// Update the x position within the tile
					tileX = (x + xLength - startTileX) % widthOfTile;
				}

				// Transfer a pixel from the tile to the frame buffer
				setPixel(frame, frameBuffer, getXPosition(x, xLength), y, tileX, tileY, yTileStartPosition);
			}
		}

		/// <summary>
		/// Renders an expanding or compressing weave.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		/// <param name="expand">True when the weave is expanding</param>        
		/// <param name="heightOfTile">Height of the repeating tile</param>
		/// <param name="widthOfTile">Width of the repeating tile</param>
		/// <param name="yTileStartPosition">Y start position within the repeating tile</param>
		/// <param name="yLength">Height of the logical display element</param>
		/// <param name="xLength">Width of the logical display element</param>
		/// <param name="setPixel">Delegate that sets a pixel on the frame buffer</param>
		private void RenderEffectWeaveExpandCompressCenter(
			int frame,
			IPixelFrameBuffer frameBuffer,
			bool expand,
			int heightOfTile,
			int widthOfTile,
			int yTileStartPosition,
			int yLength,
			int xLength,
			Action<int, IPixelFrameBuffer, int, int, int, int, int> setPixel)
		{
			// Initialize the Y position within the repeating tile
			int tileY = GetWeaveYValue(frame) % heightOfTile;

			// Initialize the position in the tile to start sampling X values from
			int startTileX = GetWeaveYValue(frame) % widthOfTile; 
			
			// Loop over the bottom half of the display element
			for (int y = yLength / 2 - 1; y >= 0; y--)
			{
				// Draw the bottom left quadrant row
				DrawQuadrantRow(
					frame,
					frameBuffer,
					expand,
					heightOfTile,
					widthOfTile,
					y,
					xLength,
					startTileX,
					tileY,
					yTileStartPosition,
					setPixel,
					GetXPositionIncrementing);

				// Draw the bottom right quadrant row
				DrawQuadrantRow(
					frame,
					frameBuffer,
					expand,
					heightOfTile,
					widthOfTile,
					y,
					xLength,
					startTileX,
					tileY,
					yTileStartPosition,
					setPixel,
					GetXPositionDecrementing);

				// Update the Y position within the tile
				tileY = UpdateWeaveTileYPosition(!expand, tileY, heightOfTile);
			}
			
			// Initialize the Y position within the repeating tile
			tileY = GetWeaveYValue(frame) % heightOfTile;

			// Initialize the position in the tile to start sampling X values from
			startTileX = GetWeaveYValue(frame) % widthOfTile;

			// Loop over the top half of the display element
			for (int y = yLength / 2; y < yLength; y++)
			{
				// Draw the top left quadrant row
				DrawQuadrantRow(
					frame,
					frameBuffer,
					expand,
					heightOfTile,
					widthOfTile,
					y,
					xLength,
					startTileX,
					tileY,
					yTileStartPosition,
					setPixel,
					GetXPositionIncrementing);

				// Draw the top right quadrant row
				DrawQuadrantRow(
					frame,
					frameBuffer,
					expand,
					heightOfTile,
					widthOfTile,
					y,
					xLength,
					startTileX,
					tileY,
					yTileStartPosition,
					setPixel,
					GetXPositionDecrementing);

				// Update the Y position within the tile
				tileY = UpdateWeaveTileYPosition(!expand, tileY, heightOfTile);
			}
		}

		/// <summary>
		/// Renders an expanding or compressing weave.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		/// <param name="expand">True when the weave is expanding</param>        
		/// <param name="heightOfTile">Height of the repeating tile</param>
		/// <param name="widthOfTile">Width of the repeating tile</param>
		/// <param name="yTileStartPosition">Y start position within the repeating tile</param>
		/// <param name="yLength">Height of the logical display element</param>
		/// <param name="xLength">Width of the logical display element</param>
		/// <param name="setPixel">Delegate that sets a pixel on the frame buffer</param>
		private void RenderEffectWeaveExpandCompress(
			int frame,
			IPixelFrameBuffer frameBuffer,
			bool expand,
			int heightOfTile,
			int widthOfTile,
			int yTileStartPosition,
			int yLength,
			int xLength,
			Action<int, IPixelFrameBuffer, int, int, int, int, int> setPixel)
		{
			// Initialize the Y position within the repeating tile
			int tileY = GetWeaveYValue(frame) % heightOfTile;

			// Loop over the left half of the display element
			for (int y = yLength / 2 - 1; y >= 0; y--)
			{
				for (int x = 0; x < xLength; x++)
				{
					// Update the x position within the tile
					int tileX = x % widthOfTile;

					// Transfer a pixel from the tile to the frame buffer
					setPixel(frame, frameBuffer, x, y, tileX, tileY, yTileStartPosition);
				}

				// Update the Y position within the tile
				tileY = UpdateWeaveTileYPosition(!expand, tileY, heightOfTile);
			}

			// Initialize the Y position within the repeating tile
			tileY = GetWeaveYValue(frame) % heightOfTile;

			// Loop over the right half of the display element
			for (int y = yLength / 2; y < yLength; y++)
			{
				for (int x = 0; x < xLength; x++)
				{
					// Update the x position within the tile
					int tileX = x % widthOfTile;

					// Transfer a pixel from the tile to the frame buffer
					setPixel(frame, frameBuffer, x, y, tileX, tileY, yTileStartPosition);
				}

				// Update the Y position within the tile
				tileY = UpdateWeaveTileYPosition(!expand, tileY, heightOfTile);
			}
		}

		/// <summary>
		/// Renders an expanding horizontal moving weave.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectWeaveHorizontalExpand(int frame, IPixelFrameBuffer frameBuffer)
		{
			// Render the expanding horizontal weave bars
			RenderEffectWeaveExpandCompress(
				frame,
				frameBuffer,
				true,
				_heightOfTile,
				_widthOfTile,
				0,
				_bufferWi,
				_bufferHt,
				SetPixelHorizontal);
		}

		/// <summary>
		/// Renders a compressing horizontal weave.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectWeaveHorizontalCompress(int frame, IPixelFrameBuffer frameBuffer)
		{
			// Render the compressing weave bars
			RenderEffectWeaveExpandCompress(
				frame,
				frameBuffer,
				false,
				_heightOfTile,
				_widthOfTile,
				0,
				_bufferWi,
				_bufferHt,
				SetPixelHorizontal);
		}

		/// <summary>
		/// Renders an expanding vertical weave.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectWeaveVerticalExpand(int frame, IPixelFrameBuffer frameBuffer)
		{
			// Render the expanding vertical weave bars
			RenderEffectWeaveExpandCompress(
				frame,
				frameBuffer,
				true,
				_heightOfTile,
				_widthOfTile,
				0,
				_bufferHt,
				_bufferWi,
				SetPixelVertical);
		}

		/// <summary>
		/// Renders an expanding vertical moving weave.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectWeaveVerticalExpandCenter(int frame, IPixelFrameBuffer frameBuffer)
		{
			// Render the expanding vertical moving weave bars
			RenderEffectWeaveExpandCompressCenter(
				frame,
				frameBuffer,
				true,
				_heightOfTile,
				_widthOfTile,
				0,
				_bufferHt,
				_bufferWi,
				SetPixelVertical);
		}

		/// <summary>
		/// Renders a compressing vertical moving weave.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectWeaveVerticalCompress(int frame, IPixelFrameBuffer frameBuffer)
		{
			// Render the compressing vertical weave bars
			RenderEffectWeaveExpandCompress(
				frame,
				frameBuffer,
				false,
				_heightOfTile,
				_widthOfTile,
				0,
				_bufferHt,
				_bufferWi,
				SetPixelVertical);
		}

		/// <summary>
		/// Renders a compressing vertical moving weave.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectWeaveVerticalCompressCenter(int frame, IPixelFrameBuffer frameBuffer)
		{
			// Render the compressing vertical weave bars
			RenderEffectWeaveExpandCompressCenter(
				frame,
				frameBuffer,
				false,
				_heightOfTile,
				_widthOfTile,
				0,
				_bufferHt,
				_bufferWi,
				SetPixelVertical);
		}

		#endregion

		#region Private Weave Render Location Methods

		/// <summary>
		/// Renders a horizontal moving weave for location mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectLocationWeaveHorizontal(int frame, PixelLocationFrameBuffer frameBuffer)
		{
			// Render the weave bars
			RenderEffectByLocationWeave(
				frame,
				frameBuffer,
				_heightOfTile,
				_widthOfTile,
				0,
				(Direction == WeaveDirection.Left || Direction == WeaveDirection.Right),
				(Direction == WeaveDirection.Right),
				true,
				ConvertFromHorizontalLocationToStringCoordinatesFlip);
		}

		/// <summary>
		/// Renders a vertical moving weave for location mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectLocationWeaveVertical(int frame, PixelLocationFrameBuffer frameBuffer)
		{
			// Render the weave bars
			RenderEffectByLocationWeave(
				frame,
				frameBuffer,
				_heightOfTile,
				_widthOfTile,
				0,
				(Direction == WeaveDirection.Down || Direction == WeaveDirection.Up),
				(Direction == WeaveDirection.Down),
				false,
				ConvertFromVerticalLocationToStringCoordinates);
		}

		/// <summary>
		/// Renders a vertical weave for location mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>       
		/// <param name="heightOfTile">Height of the repeating tile</param>
		/// <param name="widthOfTile">Width of the repeating tile</param>
		/// <param name="yTileStartPosition">Y start position within the repeating tile frame buffer</param>
		/// <param name="leftRight">Whether the weave is moving left or right</param>
		/// <param name="movesRight">Whether the weave is moving right</param>
		/// <param name="swapXY"></param>
		/// <param name="convertFromLocationToStringCoordinates">Delegate to convert location coordinates to string coordinates</param>
		private void RenderEffectByLocationWeave(
			int frame,
			PixelLocationFrameBuffer frameBuffer,
			int heightOfTile,
			int widthOfTile,
			int yTileStartPosition,
			bool leftRight,
			bool movesRight,
			// ReSharper disable once InconsistentNaming
			bool swapXY,
			ConvertFromLocationToStringCoordinates convertFromLocationToStringCoordinates)
		{
			// Render the weave in location mode
			RenderEffectByLocation(
				frame,
				frameBuffer,
				heightOfTile,
				widthOfTile,
				yTileStartPosition,
				leftRight,
				movesRight,
				swapXY,
				convertFromLocationToStringCoordinates,
				_tileFrameBuffer,
				InitializeWeaveTileYPosition,
				CalculateMovingColor);
		}

		/// <summary>
		/// Calculates the Y position within the tile for the specified Y coordinate.
		/// </summary>
		/// <param name="incrementing">True when the movement is increasing</param>
		/// <param name="y">String based Y coordinate</param>
		/// <param name="movementY">Movement in the Y axis</param>
		/// <param name="heightOfTile">Height of the repeating weave tile</param>
		/// <param name="yOffset"></param>
		/// <returns>The Y coordinate within the repeating tile</returns>
		private int CalculateWeaveYTilePosition(bool incrementing, int y, int movementY, int heightOfTile,
			int yOffset = 0)
		{
			int yTile;

			// If the movement is increasing then...
			if (incrementing)
			{
				// Calculate the Y position within the tile
				yTile = (y + movementY) % heightOfTile;
			}
			// Otherwise the movement is decreasing then...
			else
			{
				yTile = y - yOffset - movementY;
			}

			// If the edge of the tile has been reached then...
			if (yTile < 0)
			{
				yTile += heightOfTile;
			}

			// Ensure the coordinate is within the tile
			yTile = yTile % heightOfTile;

			return yTile;
		}

		/// <summary>
		/// Converts from horizontal location coordinate to a string coordinate.
		/// </summary>
		/// <param name="x">X location coordinate</param>
		/// <param name="y">Y location coordinate</param>
		/// <param name="xOut">Resulting X string coordinate</param>
		/// <param name="yOut">Resulting Y string coordinate</param>
		private void ConvertFromHorizontalLocationToStringCoordinates(int x, int y, out int xOut, out int yOut)
		{
			// Flip me over so and offset my coordinates I can act like the string version
			y = Math.Abs((BufferWiOffset - y) + (_bufferWi - 1 + BufferWiOffset));
			yOut = y - BufferWiOffset;
			xOut = x - BufferHtOffset;
		}

		/// <summary>
		/// Converts from horizontal location coordinate to a string coordinate.
		/// </summary>
		/// <param name="x">X location coordinate</param>
		/// <param name="y">Y location coordinate</param>
		/// <param name="xOut">Resulting X string coordinate</param>
		/// <param name="yOut">Resulting Y string coordinate</param>
		private void ConvertFromHorizontalLocationToStringCoordinatesFlip(int x, int y, out int xOut, out int yOut)
		{
			// Swap the input y and x coordinates 
			ConvertFromHorizontalLocationToStringCoordinates(y, x, out xOut, out yOut);
		}

		/// <summary>
		/// Converts from vertical location coordinate to a string coordinate.
		/// </summary>
		/// <param name="x">X location coordinate</param>
		/// <param name="y">Y location coordinate</param>
		/// <param name="xOut">Resulting X string coordinate</param>
		/// <param name="yOut">Resulting Y string coordinate</param>
		private void ConvertFromVerticalLocationToStringCoordinates(int x, int y, out int xOut, out int yOut)
		{
			// Flip me over so and offset my coordinates I can act like the string version
			y = Math.Abs((BufferHtOffset - y) + (_bufferHt - 1 + BufferHtOffset));
			yOut = y - BufferHtOffset;
			xOut = x - BufferWiOffset;
		}

		/// <summary>
		/// Renders an expanding or compressing weave.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		/// <param name="downIncreases">True when the weave expands on the lower/right side</param>
		/// <param name="heightOfTile">Height of the repeating tile</param>
		/// <param name="widthOfTile">Width of the repeating tile</param>
		/// <param name="yTileStartPosition">Y start position within the repeating tile frame buffer</param>
		/// <param name="angle">Angle of the rotation</param>
		/// <param name="swapXY">True when the effect is using a Horizontal Direction</param>
		/// <param name="convertFromLocationToStringCoordinates">Delegate to convert from location coordinates to string coordinates</param>
		/// <param name="height">Height of the render frame buffer</param>
		private void RenderEffectLocationWeaveExpandCompress(
			int frame,
			PixelLocationFrameBuffer frameBuffer,
			bool downIncreases,
			int heightOfTile,
			int widthOfTile,
			int yTileStartPosition,
			double angle,
			// ReSharper disable once InconsistentNaming
			bool swapXY,
			ConvertFromLocationToStringCoordinates convertFromLocationToStringCoordinates,
            int height,
			int width)
		{
			// Render the expanding or compressing weave
			RenderEffectLocationExpandCompress(
				frame,
				frameBuffer,
				downIncreases,
				heightOfTile,
				widthOfTile,
				yTileStartPosition,
				angle,
				swapXY,
				convertFromLocationToStringCoordinates,
				_tileFrameBuffer,
				InitializeWeaveTileYPosition,
				height);
		}

		/// <summary>
		/// Renders an expanding or compressing weave
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		/// <param name="downIncreases">True when the weave expands on the lower/right side</param>
		/// <param name="heightOfTile">Height of the repeating tile</param>
		/// <param name="widthOfTile">Width of the repeating tile</param>
		/// <param name="yTileStartPosition">Y start position within the repeating tile frame buffer</param>
		/// <param name="angle">Angle of the rotation</param>
		/// <param name="swapXY">True when the effect is using a Horizontal Direction</param>
		/// <param name="convertFromLocationToStringCoordinates">Delegate to convert from location coordinates to string coordinates</param>
		/// <param name="height">Height of the render frame buffer</param>
		/// <param name="width">Width of the render frame buffer</param>
		private void RenderEffectLocationWeaveExpandCompressCenter(
			int frame,
			PixelLocationFrameBuffer frameBuffer,
			bool downIncreases,
			int heightOfTile,
			int widthOfTile,
			int yTileStartPosition,
			double angle,
			// ReSharper disable once InconsistentNaming
			bool swapXY,
			ConvertFromLocationToStringCoordinates convertFromLocationToStringCoordinates,
			int height,
			int width)
		{
			// Render the expanding or compressing weave
			RenderEffectLocationExpandCompressCenter(
				frame,
				frameBuffer,
				downIncreases,
				heightOfTile,
				widthOfTile,
				yTileStartPosition,
				angle,
				swapXY,
				convertFromLocationToStringCoordinates,
				_tileFrameBuffer,
				InitializeWeaveTileYPosition,
				height,
				width);
		}

		/// <summary>
		/// Renders a horizontal expanding weave for location mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectLocationWeaveHorizontalExpand(int frame, PixelLocationFrameBuffer frameBuffer)
		{
			// Render the expanding weave bars
			RenderEffectLocationWeaveExpandCompress(
				frame,
				frameBuffer,
				true,
				_heightOfTile,
				_widthOfTile,
				0,
				-GetRotationAngle(frame),
				true,
                ConvertFromHorizontalLocationToStringCoordinatesFlip,
                _bufferWi,
				_bufferHt);
		}

		/// <summary>
		/// Renders a horizontal compressing weave for location mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectLocationWeaveHorizontalCompress(int frame, PixelLocationFrameBuffer frameBuffer)
		{
			// Render the compressing weave bars
			RenderEffectLocationWeaveExpandCompress(
				frame,
				frameBuffer,
				false,
				_heightOfTile,
				_widthOfTile,
				0,
				-GetRotationAngle(frame),
				true,
		        ConvertFromHorizontalLocationToStringCoordinatesFlip,
		        _bufferWi,
				_bufferHt);
		}

		/// <summary>
		/// Renders an expanding weave for location mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectLocationWeaveVerticalExpand(int frame, PixelLocationFrameBuffer frameBuffer)
		{
			// Render the expanding weave bars
			RenderEffectLocationWeaveExpandCompress(
				frame,
				frameBuffer,
				true,
				_heightOfTile,
				_widthOfTile,
				0,
				GetRotationAngle(frame),
				false,
	            ConvertFromVerticalLocationToStringCoordinates,
	            _bufferHt,
				_bufferWi);
		}

		/// <summary>
		/// Renders an expanding weave for location mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectLocationWeaveVerticalExpandCenter(int frame, PixelLocationFrameBuffer frameBuffer)
		{
			// Render the expanding weave bars
			RenderEffectLocationWeaveExpandCompressCenter(
				frame,
				frameBuffer,
				true,
				_heightOfTile,
				_widthOfTile,
				0,
				GetRotationAngle(frame),
				false,
				ConvertFromVerticalLocationToStringCoordinates,
				_bufferHt,
				_bufferWi);
		}

		/// <summary>
		/// Gets the rotation angle of the bars.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <returns>angle of rotation</returns>
		private double GetRotationAngle(int frame)
		{
			// Calculate the interval position factor
			double intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;

			// Create the rotation transform
			double angle = CalculateRotationAngle(intervalPosFactor);

			// Return the rotation angle
			return angle;
		}

		/// <summary>
		/// Renders a compressing weave for location mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectLocationWeaveVerticalCompress(int frame, PixelLocationFrameBuffer frameBuffer)
		{
			// Render the compressing weave bars
			RenderEffectLocationWeaveExpandCompress(
				frame,
				frameBuffer,
				false,
				_heightOfTile,
				_widthOfTile,
				0,
				GetRotationAngle(frame),
				false,
		        ConvertFromVerticalLocationToStringCoordinates,
		        _bufferHt,
				_bufferWi);
		}

		/// <summary>
		/// Renders a compressing weave for location mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectLocationWeaveVerticalCompressCenter(int frame, PixelLocationFrameBuffer frameBuffer)
		{
			// Render the compressing weave bars
			RenderEffectLocationWeaveExpandCompressCenter(
				frame,
				frameBuffer,
				false,
				_heightOfTile,
				_widthOfTile,
				0,
				GetRotationAngle(frame),
				false,
				ConvertFromVerticalLocationToStringCoordinates,
				_bufferHt,
				_bufferWi);
		}

		/// <summary>
		/// Applies the intensity setting to the specified color.
		/// </summary>
		/// <param name="color">Color to update</param>
		/// <param name="frame">Current frame number</param>
		/// <returns>Color updated for the intensity setting</returns>
		private Color AdjustIntensity(Color color, int frame)
		{
			// Calculate the interval position factor
			double intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;

			// Get the intensity level as a percentage
			double level = LevelCurve.GetValue(intervalPosFactor) / 100;

			// If the level is less than full brightness and
			// the color is NOT transparent then...
			if (level < 1 && color != Color.Transparent)
			{
				// Convert the RGB color to HSV format
				HSV hsv = HSV.FromRGB(color);

				// Set the brightness value
				hsv.V *= level;

				// Convert the HSV color back to RGB
				color = hsv.ToRGB();
			}

			return color;
		}

		/// <summary>
		/// Calculates the Y value for the weave bar based on the frame number.
		/// </summary>
		/// <param name="frame">Current frame number</param>              
		/// <returns>Y position of the weave bar</returns>
		private int GetWeaveYValue(int frame)
		{
			// The Y value for the weave is a function of the speed and how far the weave travels in a 50 ms frame
			int yValue = (int)(frame * (FrameTime / 50.0) * CalculateWeaveSpeed(GetEffectTimeIntervalPosition(frame) * 100));

			// Return the Y value for the weave
			return yValue;
		}

		/// <summary>
		/// Calculates the speed of the weave. 
		/// </summary>
		/// <param name="intervalPos">Interval position within the effect duration</param>            
		/// <returns>Speed of the weave</returns>
		private double CalculateWeaveSpeed(double intervalPos)
		{
			// Get the scale value based on the smaller of the width and height of the display element
			int newScaleValue = (int)(_scaleValue * 0.20);

			// Scale the speed based on the calculated scale factor
			return ScaleCurveToValue(SpeedCurve.GetValue(intervalPos), newScaleValue, 0);
		}

		/// <summary>
		/// Gets appropriate scale value for the effect settings.
		/// </summary>
		/// <param name="bufferHt">Height of the display element</param>
		/// <param name="bufferWi">Width of the display element</param>
		/// <returns>Scale value used to scale effect settings</returns>
		private int GetScaleValue(int bufferHt, int bufferWi)
		{
			// Use the minimum dimension of the display element as the scale factor
			int scaleValue = Math.Min(bufferHt, bufferWi);

			return scaleValue;
		}
		
		/// <summary>
		/// Gets the rotation angle for the specified position within the effect.
		/// </summary>
		/// <param name="intervalPos">Position within the effect timespan</param>
		/// <returns>Angle of the rotation</returns>
		private double CalculateRotationAngle(double intervalPos)
		{
			return ScaleCurveToValue(RotationAngle.GetValue(intervalPos), 180, -180);
		}

		/// <summary>
		/// Rotates the x and y coordinates for the specified angle.
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <param name="angle">Angle to rotate</param>
		/// <param name="swapXY">True when the X and Y coordinates are swapped due to the Direction being horizontal</param>
		// ReSharper disable once InconsistentNaming
		private void GetRotatedPosition(ref int x, ref int y, double angle, bool swapXY)
		{
			int xOffset;
			int yOffset;

			// If the X and Y are swapped due to the Direction being horizontal
			if (swapXY)
			{
				// Calculate the offset into the virtual frame buffer for the original frame buffer
				yOffset = (_length - BufferWi) / 2;
				xOffset = (_length - BufferHt) / 2;
			}
			else
			{
				// Calculate the offset into the virtual frame buffer for the original frame buffer
				yOffset = (_length - BufferHt) / 2;
				xOffset = (_length - BufferWi) / 2;
			}

			// Calculate the center of the square virtual frame buffer
			double center = (_length - 1) / 2.0;

			// Create a rotate transform with the specified angle and center of rotation
			RotateTransform rt = new RotateTransform(-angle, center, center);

			// Create a temporary point
			System.Windows.Point tempPoint = new System.Windows.Point();

			// Initialize the temporary point with the point in actual display element (frame buffer) that
			// we are setting
			tempPoint.X = x + xOffset;
			tempPoint.Y = y + yOffset;

			// Transform (rotate) the point
			System.Windows.Point transformedPoint = rt.Transform(tempPoint);

			// Updated the x and y coordinates for the rotation
			x = (int)Math.Round(transformedPoint.X);
			y = (int)Math.Round(transformedPoint.Y);
		}

		#endregion		
	}
}
