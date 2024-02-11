using System.ComponentModel;

using Common.Controls.ColorManagement.ColorModels;

using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;

using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Border;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Borders
{
	public class Border : PixelEffectBase
	{
		#region Fields

		private BorderData _data;
		private double _minBufferSize;
		
		/// <summary>
		/// Maximum dimension of the display element (width or height).
		/// </summary>
		private int _maxBufferSize;

		/// <summary>
		/// Height of the display element.
		/// </summary>
		private int _bufferHt;

		/// <summary>
		/// Width of the display element.
		/// </summary>
		private int _bufferWi;

		/// <summary>
		/// Render scale factor.  This field is used to determine the size of the virtual frame buffer to
		/// use when rendering a Marquee border in locations mode.
		/// </summary>
		private int _renderScaleFactor = 4;

		/// <summary>
		/// Position of the Marquee border.  This field is used to animate the Marquee bands each frame.
		/// </summary>
		private double _positionOfMarquee;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Border()
		{
			_data = new BorderData();
			EnableTargetPositioning(true, true);
			InitAllAttributes();
		}

		#endregion

		#region Setup

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
		/// Determines the fidelity of the rendering the Marquee border
		/// when in location mode.
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

		#region Marquee Mode Config Properties

		/// <summary>
		/// Render level of the Marquee border.
		/// </summary>
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderRenderLevel")]
		[ProviderDescription(@"BorderRenderLevel")]
		[PropertyOrder(2)]
		public RenderLevel RenderLevel
		{
			get { return _data.RenderLevel; }
			set
			{
				_data.RenderLevel = value;
				IsDirty = true;
				OnPropertyChanged();
				UpdateMarqueeModeControlAttributes();
			}
		}

		/// <summary>
		/// Whether several of the Marquee border properties are measured in pixels
		/// or a percentage of the display element.
		/// </summary>
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderUsePercent")]
		[ProviderDescription(@"BorderUsePercent")]
		[PropertyOrder(3)]
		public bool UsePercent
		{
			get { return _data.UsePercent; }
			set
			{
				_data.UsePercent = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		/// <summary>
		/// Thickness of the Marquee border.
		/// </summary>
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderEffectThickness")]
		[ProviderDescription(@"BorderEffectThickness")]
		[PropertyOrder(4)]
		public int Thickness
		{
			get { return _data.Thickness; }
			set
			{
				_data.Thickness = value;
				IsDirty = true;
				OnPropertyChanged();
				UpdateMarqueeModeControlAttributes(true);
			}
		}

		/// <summary>
		/// Stagger offset of the Marquee border.
		/// </summary>
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderStagger")]
		[ProviderDescription(@"BorderStagger")]
		[PropertyOrder(5)]
		public int Stagger
		{
			get { return _data.Stagger; }
			set
			{
				_data.Stagger = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Length of each color band in the Marquee border.
		/// </summary>
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderBandLength")]
		[ProviderDescription(@"BorderBandLength")]
		[PropertyOrder(6)]
		public int BandLength
		{
			get { return _data.BandSize; }
			set
			{
				_data.BandSize = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Length of the dark area between the Marquee color bands.
		/// </summary>
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderSkipLength")]
		[ProviderDescription(@"BorderSkipLength")]
		[PropertyOrder(7)]
		public int SkipLength
		{
			get { return _data.SkipSize; }
			set
			{
				_data.SkipSize = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Speed of movement of the Marquee color bands.
		/// </summary>
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderSpeed")]
		[ProviderDescription(@"BorderSpeed")]
		[PropertyOrder(8)]
		public Curve Speed
		{
			get { return _data.Speed; }
			set
			{
				_data.Speed = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Reverses the direction of movement of the Marquee border.
		/// </summary>
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderReverse")]
		[ProviderDescription(@"BorderReverse")]
		[PropertyOrder(9)]
		public bool Reverse
		{
			get { return _data.Reverse; }
			set
			{
				_data.Reverse = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Size of the Marquee border in the X-axis.
		/// </summary>
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderXSize")]
		[ProviderDescription(@"BorderXSize")]
		[PropertyOrder(10)]
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

		/// <summary>
		/// Size of the Marquee border in the Y-axis.
		/// </summary>
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderYSize")]
		[ProviderDescription(@"BorderYSize")]
		[PropertyOrder(11)]
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

		#endregion

		#region Config Properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderMode")]
		[ProviderDescription(@"BorderMode")]
		[PropertyOrder(0)]
		public BorderMode BorderMode
		{
			get { return _data.BorderMode; }
			set
			{
				_data.BorderMode = value;
				UpdateBorderControlAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderType")]
		[ProviderDescription(@"BorderType")]
		[PropertyOrder(1)]
		public BorderType BorderType
		{
			get { return _data.BorderType; }
			set
			{
				_data.BorderType = value;
				UpdateBorderControlAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"BorderWidth")]
		[ProviderDescription(@"BorderWidth")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(0)]
		public int SimpleBorderWidth
		{
			get { return _data.SimpleBorderWidth; }
			set
			{
				_data.SimpleBorderWidth = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"BorderThickness")]
		[ProviderDescription(@"BorderThickness")]
		[PropertyOrder(1)]
		public Curve ThicknessCurve
		{
			get { return _data.ThicknessCurve; }
			set
			{
				_data.ThicknessCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"TopBorderWidth")]
		[ProviderDescription(@"TopBorderWidth")]
		[PropertyOrder(2)]
		public Curve TopThicknessCurve
		{
			get { return _data.TopThicknessCurve; }
			set
			{
				_data.TopThicknessCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"BottomBorderWidth")]
		[ProviderDescription(@"BottomBorderWidth")]
		[PropertyOrder(3)]
		public Curve BottomThicknessCurve
		{
			get { return _data.BottomThicknessCurve; }
			set
			{
				_data.BottomThicknessCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"LeftBorderWidth")]
		[ProviderDescription(@"LeftBorderWidth")]
		[PropertyOrder(4)]
		public Curve LeftThicknessCurve
		{
			get { return _data.LeftThicknessCurve; }
			set
			{
				_data.LeftThicknessCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"RightBorderWidth")]
		[ProviderDescription(@"RightBorderWidth")]
		[PropertyOrder(5)]
		public Curve RightThicknessCurve
		{
			get { return _data.RightThicknessCurve; }
			set
			{
				_data.RightThicknessCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"BorderWidth")]
		[ProviderDescription(@"BorderWidth")]
		[PropertyOrder(6)]
		public Curve BorderSizeCurve
		{
			get { return _data.BorderSizeCurve; }
			set
			{
				_data.BorderSizeCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"BorderHeight")]
		[ProviderDescription(@"BorderHeight")]
		[PropertyOrder(7)]
		public Curve BorderHeightCurve
		{
			get { return _data.BorderHeightCurve; }
			set
			{
				_data.BorderHeightCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Movement Properties

		[Value]
		[ProviderCategory(@"Movement", 3)]
		[ProviderDisplayName(@"XOffset")]
		[ProviderDescription(@"XOffset")]
		[PropertyOrder(0)]
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

		[Value]
		[ProviderCategory(@"Movement", 3)]
		[ProviderDisplayName(@"YOffset")]
		[ProviderDescription(@"YOffset")]
		[PropertyOrder(1)]
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

		/// <summary>
		/// Whether the Marquee border will wrap around the X-axis.
		/// </summary>
		[ProviderCategory(@"Movement", 4)]
		[ProviderDisplayName(@"BorderWrapX")]
		[ProviderDescription(@"BorderWrapX")]
		[PropertyOrder(2)]
		public bool WrapX
		{
			get { return _data.WrapX; }
			set
			{
				_data.WrapX = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Whether the Marquee border will wrap around the Y-axis.
		/// </summary>
		[ProviderCategory(@"Movement", 4)]
		[ProviderDisplayName(@"BorderWrapY")]
		[ProviderDescription(@"BorderWrapY")]
		[PropertyOrder(3)]
		public bool WrapY
		{
			get { return _data.WrapY; }
			set
			{
				_data.WrapY = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color Properties

		[Value]
		[ProviderCategory(@"Color", 4)]
		[ProviderDisplayName(@"GradientMode")]
		[ProviderDescription(@"GradientMode")]
		[PropertyOrder(1)]
		public GradientMode GradientMode
		{
			get { return _data.GradientMode; }
			set
			{
				_data.GradientMode = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 4)]
		[ProviderDisplayName(@"ColorGradient")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(2)]
		public ColorGradient Color
		{
			get { return _data.Gradient; }
			set
			{
				_data.Gradient = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"BorderColorGradients")]
		[ProviderDescription(@"BorderColorGradients")]
		[PropertyOrder(3)]
		public List<ColorGradient> Colors
		{
			get { return _data.Colors; }
			set
			{
				_data.Colors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Level Properties

		[Value]
		[ProviderCategory(@"Brightness", 5)]
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

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/border/"; }
		}

		#endregion

		#region Protected Methods

		/// <inheritdoc/>
		protected override void TargetPositioningChanged()
		{
			// Update whether the render scale factor is visible 
			UpdateMarqueeModeControlAttributes();
		}

		/// <inheritdoc/>
		protected override void SetupRender()
		{
			// Determine which dimension of the display element is the largest
			_maxBufferSize = Math.Max(BufferHt, BufferWi);

			// Determine which dimension of the display element is the smallest
			_minBufferSize = Math.Min(BufferHt, BufferWi);

			// Initialize the position of the marquee border
			_positionOfMarquee = 0;

			// Determine the logical render area based on the target positioning
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				// Save off the render scale factor
				_renderScaleFactor = RenderScaleFactor;

				// Adjust the virtual frame buffer based on the render scale factor
				_bufferWi = BufferWi / _renderScaleFactor;
				_bufferHt = BufferHt / _renderScaleFactor;

				// Need to increase the render height if the scale factor did not divide evenly
				if (BufferHt % _renderScaleFactor != 0)
				{
					_bufferHt++;
				}

				// Need to increase the render width if the scale factor did not divide evenly
				if (BufferWi % _renderScaleFactor != 0)
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
		}

		/// <inheritdoc/>
		protected override void CleanUpRender()
		{
			//Not required
		}

		/// <inheritdoc/>
		protected override void RenderEffect(int effectFrame, IPixelFrameBuffer frameBuffer)
		{
			// If the border mode is either Simple or Advanced then...
			if (BorderMode == BorderMode.Simple ||
			    BorderMode == BorderMode.Advanced)
			{
				RenderEffectSimpleOrAdvancedBorder(effectFrame, frameBuffer);
			}
			// Otherwise the border mode is Marquee 
			// If the render level is level zero then...
			else if (RenderLevel == RenderLevel.Level0)
			{
				// Render the typical marquee border effect
				RenderEffectMarquee(effectFrame, frameBuffer, _bufferWi, _bufferHt);
			}
			else
			{
				// Otherwise render the marquee border effect on a display element
				// that is one long string of pixels
				RenderEffectMarqueeSingleString(effectFrame, frameBuffer);
			}
		}

		/// <inheritdoc/>
		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			// If the border mode is either Simple or Advanced then...
			if (BorderMode == BorderMode.Simple ||
			    BorderMode == BorderMode.Advanced)
			{
				// Render the Simple or Advanced border 
				RenderEffectSimpleOrAdvancedByLocation(numFrames, frameBuffer);
			}
			// Otherwise the border mode is Marquee 
			else 
			{
				// Render the typical marquee border effect
				RenderEffectMarqueeByLocation(numFrames, frameBuffer);
			}
		}

		#endregion

		#region Protected Properties

		/// <inheritdoc/>
		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		#endregion

		#region Public Properties

		/// <inheritdoc/>
		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as BorderData;
				InitAllAttributes();
				IsDirty = true;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates control visibility for the effect.
		/// </summary>
		private void InitAllAttributes()
		{
			UpdateBorderControlAttribute(false);
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
		}

		/// <summary>
		/// Updates control visibility based on border mode.
		/// </summary>
		private void UpdateBorderControlAttribute(bool refresh = true)
		{
			// If the effect is in Simple border mode then...
			if (BorderMode == BorderMode.Simple)
			{
				Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(27)
				{
					{ nameof(SimpleBorderWidth), true },
					{ nameof(ThicknessCurve), false },
					{ nameof(TopThicknessCurve), false },
					{ nameof(BottomThicknessCurve), false },
					{ nameof(LeftThicknessCurve), false },
					{ nameof(RightThicknessCurve), false },
					{ nameof(BorderSizeCurve), false },
					{ nameof(BorderHeightCurve), false },
					{ nameof(BorderType), false },
					{ nameof(YOffsetCurve), false },
					{ nameof(XOffsetCurve), false },
					{ nameof(GradientMode), true },
					{ nameof(Color), true },

					{ nameof(Colors), false },
					{ nameof(RenderLevel), false },
					{ nameof(UsePercent), false },
					{ nameof(Thickness), false },
					{ nameof(Stagger), false },
					{ nameof(BandLength), false },
					{ nameof(SkipLength), false },
					{ nameof(Speed), false },
					{ nameof(Reverse), false },
					{ nameof(XScale), false },
					{ nameof(YScale), false },
					{ nameof(WrapX), false },
					{ nameof(WrapY), false },
					{ nameof(RenderScaleFactor), false },
				};
				
				SetBrowsable(propertyStates);
			}
			// Otherwise if effect is in Advanced Mode then...
			else if (BorderMode == BorderMode.Advanced)
			{
				Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(27)
				{
					{nameof(SimpleBorderWidth), false},
					{nameof(ThicknessCurve), BorderType == BorderType.Single},
					{nameof(TopThicknessCurve), BorderType != BorderType.Single},
					{nameof(BottomThicknessCurve), BorderType != BorderType.Single},
					{nameof(LeftThicknessCurve), BorderType != BorderType.Single},
					{nameof(RightThicknessCurve), BorderType != BorderType.Single},
					{nameof(BorderSizeCurve), true},
					{nameof(BorderHeightCurve), true},
					{nameof(BorderType), true},
					{nameof(YOffsetCurve), true},
					{nameof(XOffsetCurve), true},
					{nameof(GradientMode), true},
					{nameof(Color), true},
					
					{ nameof(Colors), false },
					{ nameof(RenderLevel), false },
					{ nameof(UsePercent), false },
					{ nameof(Thickness), false },
					{ nameof(Stagger), false },
					{ nameof(BandLength), false },
					{ nameof(SkipLength), false },
					{ nameof(Speed), false },
					{ nameof(Reverse), false },
					{ nameof(XScale), false },
					{ nameof(YScale), false },
					{ nameof(WrapX), false },
					{ nameof(WrapY), false },
					{ nameof(RenderScaleFactor), false },

				};
				SetBrowsable(propertyStates);
			}
			// Otherwise effect is in Marquee Mode
			else
			{
				Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(27)
				{
					{nameof(SimpleBorderWidth), false},
					{nameof(ThicknessCurve), false},
					{nameof(TopThicknessCurve), false},
					{nameof(BottomThicknessCurve), false},
					{nameof(LeftThicknessCurve), false},
					{nameof(RightThicknessCurve), false},
					{nameof(BorderSizeCurve), false},
					{nameof(BorderHeightCurve), false},
					{nameof(BorderType), false},
					{nameof(YOffsetCurve), true},
					{nameof(XOffsetCurve), true},
					{nameof(GradientMode), false},
					{nameof(Color), false},
					
					{ nameof(Colors), true },
					{ nameof(RenderLevel), true },
					{ nameof(UsePercent), true },
					{ nameof(Thickness), RenderLevel == RenderLevel.Level0 },
					{ nameof(Stagger), RenderLevel == RenderLevel.Level0 && Thickness > 1 },
					{ nameof(BandLength), true },
					{ nameof(SkipLength), true },
					{ nameof(Speed), true },
					{ nameof(Reverse), true },
					{ nameof(XScale),RenderLevel == RenderLevel.Level0 },
					{ nameof(YScale), RenderLevel == RenderLevel.Level0 },
					{ nameof(WrapX), RenderLevel == RenderLevel.Level0},
					{ nameof(WrapY), RenderLevel == RenderLevel.Level0 },
					{ nameof(RenderScaleFactor),  TargetPositioning == TargetPositioningType.Locations},
				};
				SetBrowsable(propertyStates);
			}

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Updates the visibility of Marquee properties.
		/// </summary>
		private void UpdateMarqueeModeControlAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3)
			{
				{ nameof(Thickness), BorderMode == BorderMode.Marquee && (RenderLevel == RenderLevel.Level0) },
				{ nameof(Stagger), BorderMode == BorderMode.Marquee && (Thickness > 1 && RenderLevel == RenderLevel.Level0) },
				{ nameof(RenderScaleFactor), BorderMode == BorderMode.Marquee && (TargetPositioning == TargetPositioningType.Locations) }

			};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}


		/// <summary>
		/// Renders the effect when the border mode is simple or advanced.
		/// </summary>
		/// <param name="effectFrame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to populate</param>
		private void RenderEffectSimpleOrAdvancedBorder(int effectFrame, IPixelFrameBuffer frameBuffer)
		{
			var intervalPos = GetEffectTimeIntervalPosition(effectFrame);
			var intervalPosFactor = intervalPos * 100;
			var bufferWi = BufferWi;
			var bufferHt = BufferHt;

			double level = LevelCurve.GetValue(intervalPosFactor) / 100;
			int thickness = (int)Math.Round(CalculateBorderThickness(intervalPosFactor) / 2);
			int topThickness = (int)Math.Round(TopThicknessCurve.GetValue(intervalPosFactor) * bufferHt / 100);
			int bottomThickness = (int)Math.Round(BottomThicknessCurve.GetValue(intervalPosFactor) * bufferHt / 100);
			int leftThickness = (int)Math.Round(LeftThicknessCurve.GetValue(intervalPosFactor) * bufferWi / 100);
			int rightThickness = (int)Math.Round(RightThicknessCurve.GetValue(intervalPosFactor) * bufferWi / 100);
			int borderHeight = (int)CalculateBorderHeight(intervalPosFactor) / 2;
			int borderWidth = (int)(CalculateBorderSize(intervalPosFactor) / 2);
			int xOffsetAdj = CalculateXOffset(intervalPosFactor) * (bufferWi - borderWidth) / 100;
			int yOffsetAdj = CalculateYOffset(intervalPosFactor) * (bufferHt - borderHeight) / 100;
			Color color = Color.GetColorAt(GetEffectTimeIntervalPosition(effectFrame));

			if (BorderMode == BorderMode.Simple)
			{
				thickness = SimpleBorderWidth;
				borderWidth = 0;
				borderHeight = 0;
			}
			else if (BorderType == BorderType.Single)
			{
				rightThickness = thickness;
				topThickness = thickness;
				leftThickness = thickness;
				bottomThickness = thickness;
			}

			for (int x = 0; x < bufferWi; x++)
			{
				for (int y = 0; y < bufferHt; y++)
				{
					CalculatePixel(x, y, frameBuffer, thickness, topThickness,
						bottomThickness, leftThickness, rightThickness, level, borderWidth, borderHeight, color, xOffsetAdj, yOffsetAdj, ref bufferHt, ref bufferWi);
				}
			}
		}

		/// <summary>
		/// Renders a Simple or Advanced border effect by location.
		/// </summary>
		/// <param name="numFrames">Total number of frame to render</param>
		/// <param name="frameBuffer">Frame buffer to render into</param>
		private void RenderEffectSimpleOrAdvancedByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var bufferWi = BufferWi;
			var bufferHt = BufferHt;

			for (int effectFrame = 0; effectFrame < numFrames; effectFrame++)
			{
				frameBuffer.CurrentFrame = effectFrame;

				var intervalPos = GetEffectTimeIntervalPosition(effectFrame);
				var intervalPosFactor = intervalPos * 100;

				double level = LevelCurve.GetValue(intervalPos);
				int thickness = (int)Math.Round(CalculateBorderThickness(intervalPosFactor) / 2);
				int topThickness = (int)(TopThicknessCurve.GetValue(intervalPosFactor) * bufferHt / 100);
				int bottomThickness = (int)(BottomThicknessCurve.GetValue(intervalPosFactor) * bufferHt / 100);
				int leftThickness = (int)(LeftThicknessCurve.GetValue(intervalPosFactor) * bufferWi / 100);
				int rightThickness = (int)(RightThicknessCurve.GetValue(intervalPosFactor) * bufferWi / 100);
				int borderHeight = (int)CalculateBorderHeight(intervalPosFactor) / 2;
				int borderWidth = (int)(CalculateBorderSize(intervalPosFactor) / 2);
				int xOffsetAdj = CalculateXOffset(intervalPosFactor) * (bufferWi - borderWidth) / 100;
				int yOffsetAdj = CalculateYOffset(intervalPosFactor) * (bufferHt - borderHeight) / 100;
				Color color = Color.GetColorAt(GetEffectTimeIntervalPosition(effectFrame));

				if (BorderMode == BorderMode.Simple)
				{
					thickness = SimpleBorderWidth;
					borderWidth = 0;
					borderHeight = 0;
					xOffsetAdj = 0;
					yOffsetAdj = 0;
				}
				else if (BorderType == BorderType.Single)
				{
					rightThickness = thickness;
					topThickness = thickness;
					leftThickness = thickness;
					bottomThickness = thickness;
				}

				foreach (var elementLocation in frameBuffer.ElementLocations)
				{
					CalculatePixel(elementLocation.X, elementLocation.Y, frameBuffer, thickness,
						topThickness, bottomThickness, leftThickness, rightThickness, level, borderWidth, borderHeight,
						color, xOffsetAdj, yOffsetAdj, ref bufferHt, ref bufferWi);
				}
			}
		}

		private void CalculatePixel(int x, int y, IPixelFrameBuffer frameBuffer, int thickness, int topThickness, int bottomThickness, int leftThickness, int rightThickness, double level, int borderWidth, int borderHeight, Color color, int xOffsetAdj, int yOffsetAdj, ref int bufferHt, ref int bufferWi)
		{
			int yCoord = y;
			int xCoord = x;
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				//Flip me over so and offset my coordinates I can act like the string version
				y = Math.Abs((BufferHtOffset - y) + (bufferHt - 1 + BufferHtOffset));
				y = y - BufferHtOffset;
				x = x - BufferWiOffset;
			}

			x -= xOffsetAdj;
			y -= yOffsetAdj;

			if (BorderType == BorderType.Single || BorderMode == BorderMode.Simple)//Single Border Control
			{
				//Displays borders 
				if ((y < borderHeight + thickness || y >= bufferHt - borderHeight - thickness || x < borderWidth + thickness || x >= bufferWi - borderWidth - thickness)
					&& x >= borderWidth && y < bufferHt - borderHeight && y >= borderHeight && x < bufferWi - borderWidth)
				{
					color = GetColor(x, y, color, level, bufferHt, bufferWi);
					frameBuffer.SetPixel(xCoord, yCoord, color);
				}
			}
			else
			{
				//Displays Independent Borders
				if ((y < borderHeight + bottomThickness || y >= bufferHt - borderHeight - topThickness || x < borderWidth + leftThickness || x >= bufferWi - borderWidth - rightThickness)
					&& x >= borderWidth && y < bufferHt - borderHeight && y >= borderHeight && x < bufferWi - borderWidth)
				{
					color = GetColor(x, y, color, level, bufferHt, bufferWi);
					frameBuffer.SetPixel(xCoord, yCoord, color);
				}
			}
		}
		
		/// <summary>
		/// Gets the color for the specified pixel.
		/// </summary>
		/// <remarks>Method is used for Simple and Advanced border mode</remarks>
		private Color GetColor(int x, int y, Color color, double level, int bufferHt, int bufferWi)
		{
			if (GradientMode != GradientMode.OverTime)
			{
				switch (GradientMode)
				{
					case GradientMode.AcrossElement:
						color = Color.GetColorAt(100 / (double)bufferWi * x / 100);
						break;
					case GradientMode.VerticalAcrossElement:
						color = Color.GetColorAt(100 / (double)bufferHt * (bufferHt - y) / 100);
						break;
					case GradientMode.DiagonalTopBottomElement:
						color = Color.GetColorAt(
							(100 / (double)bufferHt * (bufferHt - y) + 100 / (double)bufferWi * x) / 200);
						break;
					case GradientMode.DiagonalBottomTopElement:
						color = Color.GetColorAt((100 / (double)bufferHt * y + 100 / (double)bufferWi * x) / 200);
						break;
				}
			}
			if (level < 1)
			{
				HSV hsv = HSV.FromRGB(color);
				hsv.V *= level;
				color = hsv.ToRGB();
			}
			return color;
		}

		private double CalculateBorderSize(double intervalPosFactor)
		{
			return ScaleCurveToValue(BorderSizeCurve.GetValue(intervalPosFactor), 0, BufferWi);
		}

		private double CalculateBorderThickness(double intervalPosFactor)
		{
			return ScaleCurveToValue(ThicknessCurve.GetValue(intervalPosFactor), _minBufferSize, 2);
		}

		private double CalculateBorderHeight(double intervalPosFactor)
		{
			return ScaleCurveToValue(BorderHeightCurve.GetValue(intervalPosFactor), 0, BufferHt);
		}

		private int CalculateXOffset(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), 100, -100));
		}

		private int CalculateYOffset(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), 100, -100));
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
			int scaledX = x / _renderScaleFactor;
			int scaledY = y / _renderScaleFactor;

			// Retrieve the color from the bitmap
			Color color = virtualFrameBuffer.GetColorAt(scaledX, scaledY);

			// Set the pixel on the frame buffer
			frameBuffer.SetPixel(xCoord, yCoord, color);
		}

		/// <summary>
		/// Renders the Marquee border effect in location mode.
		/// </summary>
		/// <param name="numFrames">Total number of frames to render</param>
		/// <param name="frameBuffer">Frame buffer to render into</param>
		private void RenderEffectMarqueeByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
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

		/// <summary>
		/// Renders the Marquee effect on a virtual frame buffer that is a single string of pixels.
		/// </summary>
		/// <param name="effectFrame">Frame number to render</param>
		/// <param name="frameBuffer">Frame buffer to render into</param>
		private void RenderEffectMarqueeSingleString(int effectFrame, IPixelFrameBuffer frameBuffer)
		{
			// Calculate the length of the single string of pixels
			int length = _bufferWi * _bufferHt;

			// Create a virtual frame buffer that is really long by one pixel high
			IPixelFrameBuffer singleStrandFrameBuffer = new PixelFrameBuffer(length, 1);

			// Render the effect using the virtual string frame buffer
			RenderEffectMarquee(effectFrame, singleStrandFrameBuffer, length, 1);

			// If the render level is Level 1 (Zig Zag) then...
			if (RenderLevel == RenderLevel.Level1)
			{
				// Transfer the pixels from the virtual string frame buffer to the actual frame buffer
				// by going back and forth (Zig Zag)
				int index = 0;

				// Loop over the width of the display element
				for (int x = 0; x < _bufferWi; x++)
				{
					// Loop over the height of the display element
					for (int y = 0; y < _bufferHt; y++)
					{
						// If this is an even row then transfer pixel information left to right
						if (x % 2 == 0)
						{
							frameBuffer.SetPixel(x, y, singleStrandFrameBuffer.GetColorAt(index, 0));
						}
						// Otherwise if this an odd row transfer pixel information right to left
						else
						{
							frameBuffer.SetPixel(_bufferWi - x - 1, y, singleStrandFrameBuffer.GetColorAt(index, 0));
						}

						// Increment the position in the single strand frame buffer
						index++;
					}
				}
			}
			else
			{
				// Typewriter (returns to left side)
				// Otherwise transfer the pixels like a type writer when we get to the end of the row
				// jump back to the beginning of the next row
				int index = 0;

				// Loop over the width of the display element
				for (int x = 0; x < _bufferWi; x++)
				{
					// Loop over the height of the display element
					for (int y = 0; y < _bufferHt; y++)
					{
						frameBuffer.SetPixel(x, y, singleStrandFrameBuffer.GetColorAt(index, 0));

						// Increment the position in the single strand frame buffer
						index++;
					}
				}
			}
		}

		/// <summary>
		/// Retrieves the length of the marquee bands in pixels.
		/// </summary>
		/// <returns>Length of the marquee bands in pixels</returns>
		private int GetMarqueeBandLength()
		{
			int bandSize;

			if (UsePercent)
			{
				bandSize = (int)((_maxBufferSize * BandLength) / 100.0);
				if (bandSize == 0)
				{
					bandSize = 1;
				}
			}
			else
			{
				bandSize = BandLength;
			}

			return bandSize;
		}

		/// <summary>
		/// Retrieves the skip length of the marquee in pixels.
		/// </summary>
		/// <returns>Skip length of the marquee in pixels</returns>
		private int GetMarqueeSkipLength()
		{
			int skipSize;

			if (UsePercent)
			{
				skipSize = (int)(_maxBufferSize * SkipLength / 100.0);
			}
			else
			{
				skipSize = SkipLength;
			}

			return skipSize;
		}

		/// <summary>
		/// Gets the Marquee border thickness in pixels.
		/// </summary>
		/// <returns>Marquee border thickness in pixels</returns>
		private int GetMarqueeThickness()
		{
			int thickness;

			if (UsePercent)
			{
				thickness = (int)(_maxBufferSize * Thickness / 100.0);
				if (thickness == 0)
				{
					thickness = 1;
				}
			}
			else
			{
				thickness = Thickness;
			}

			return thickness;	
		}
		
		/// <summary>
		/// Gets the Marquee stagger offset in pixels.
		/// </summary>
		/// <returns>Marquee stagger offset in pixels</returns>
		private int GetMarqueeStagger()
		{
			int stagger;

			if (UsePercent)
			{
				stagger = (int)(_maxBufferSize * Stagger / 100.0);
			}
			else
			{
				stagger = Stagger;
			}

			return stagger;
		}
		
		/// <summary>
		/// Applies the specified intensity to the color.
		/// </summary>
		/// <param name="intensity">Intensity of the color desired</param>
		/// <param name="color">Color to update</param>
		/// <returns>Color adjusted for desired intensity</returns>
		private Color GetColorIntensity(double intensity, Color color)
		{
			if (intensity < 1)
			{
				HSV hsv = HSV.FromRGB(color);
				hsv.V *= intensity;
				color = hsv.ToRGB();
			}

			return color;
		}

		/// <summary>
		/// Renders the Marquee border effect in string mode.
		/// </summary>
		/// <param name="effectFrame">Frame number to render</param>
		/// <param name="frameBuffer">Frame buffer to render into</param>
		/// <param name="bufferWi">Width of the frame buffer</param>
		/// <param name="bufferHt">Height of the frame buffer</param>
		private void RenderEffectMarquee(int effectFrame, IPixelFrameBuffer frameBuffer, int bufferWi, int bufferHt)
		{
			// Get the position within the effect
			double intervalPos = GetEffectTimeIntervalPosition(effectFrame);
			double intervalPosFactor = intervalPos * 100;

			double intensity = LevelCurve.GetValue(intervalPos) / 100;

			int bandSize = GetMarqueeBandLength();
			int skipSize = GetMarqueeSkipLength();
			int thickness = GetMarqueeThickness();
			int stagger = GetMarqueeStagger();
			double mSpeed = ScaleCurveToValue(Speed.GetValue(intervalPosFactor), 20.0, 0);

			int mStart = 0; 

			int x_scale = XScale; 
			int y_scale = YScale; 

			int xc_adj = (int)Math.Round(ScaleCurveToValue(XOffsetCurve.GetValue(intervalPosFactor), 100, -100));
			int yc_adj = (int)Math.Round(ScaleCurveToValue(YOffsetCurve.GetValue(intervalPosFactor), 100, -100));

			bool reverse_dir = Reverse; 
			bool pixelOffsets = false; 
			bool wrap_x = WrapX; 
			bool wrap_y = WrapY; 

			int colorcnt = Colors.Count; 
			int color_size = bandSize + skipSize;
			int repeat_size = color_size * colorcnt;
			
			_positionOfMarquee += mSpeed;
			int x = (int)(_positionOfMarquee / 5);

			
			int corner_x1 = 0;
			int corner_y1 = 0;
			int corner_x2 = (int)Math.Round(((double)(bufferWi * x_scale) / 100.0) - 1.0);
			int corner_y2 = (int)Math.Round(((double)(bufferHt * y_scale) / 100.0) - 1.0);
			int sign = 1;
			if (reverse_dir)
			{
				sign = -1;
			}

			int xoffset_adj = xc_adj;
			int yoffset_adj = yc_adj;
			if (!pixelOffsets)
			{
				xoffset_adj = (int)((xoffset_adj * bufferWi) / 100.0); // xc_adj is from -100 to 100
				yoffset_adj = (int)((yoffset_adj * bufferHt) / 100.0); // yc_adj is from -100 to 100
			}

			for (int thick = 0; thick < thickness; thick++)
			{
				int current_color = ((x + mStart) % repeat_size) / color_size;
				int current_pos = (((x + mStart) % repeat_size) % color_size);
				if (sign < 0)
				{
					current_color = colorcnt - current_color - 1;
				}
				
				if (corner_y2 != corner_y1)
				{
					UpdateMarqueeColor(ref current_pos, ref current_color, colorcnt, color_size, thick * (stagger + 1) * sign);
					for (int x_pos = corner_x1; x_pos <= corner_x2; x_pos++)
					{
						Color color = System.Drawing.Color.Transparent;
						if (current_pos < bandSize)
						{
							color = GetColorIntensity(intensity, Colors[current_color].GetColorAt(intervalPos));
						}
						ProcessPixel(frameBuffer, x_pos + xoffset_adj, corner_y2 + yoffset_adj, color, wrap_x, wrap_y);

						UpdateMarqueeColor(ref current_pos, ref current_color, colorcnt, color_size, 1 * sign);
					}
					UpdateMarqueeColor(ref current_pos, ref current_color, colorcnt, color_size, thick * 2 * sign);
					for (int y_pos = corner_y2; y_pos >= corner_y1; y_pos--)
					{
						Color color = System.Drawing.Color.Transparent;
						if (current_pos < bandSize)
						{
							color = GetColorIntensity(intensity, Colors[current_color].GetColorAt(intervalPos));
						}
						ProcessPixel(frameBuffer, corner_x2 + xoffset_adj, y_pos + yoffset_adj, color, wrap_x, wrap_y);

						UpdateMarqueeColor(ref current_pos, ref current_color, colorcnt, color_size, 1 * sign);
					}
				}
				UpdateMarqueeColor(ref current_pos, ref current_color, colorcnt, color_size, thick * 2 * sign);
				for (int x_pos = corner_x2; x_pos >= corner_x1; x_pos--)
				{
					Color color = System.Drawing.Color.Transparent;
					if (current_pos < bandSize)
					{
						color = GetColorIntensity(intensity, Colors[current_color].GetColorAt(intervalPos));
					}
					ProcessPixel(frameBuffer, x_pos + xoffset_adj, corner_y1 + yoffset_adj, color, wrap_x, wrap_y);
					UpdateMarqueeColor(ref current_pos, ref current_color, colorcnt, color_size, 1 * sign);
				}

				if (corner_y2 != corner_y1)
				{
					UpdateMarqueeColor(ref current_pos, ref current_color, colorcnt, color_size, thick * 2 * sign);
					for (int y_pos = corner_y1; y_pos <= corner_y2 - 1; y_pos++)
					{
						Color color = System.Drawing.Color.Transparent;
						if (current_pos < bandSize)
						{
							color = GetColorIntensity(intensity, Colors[current_color].GetColorAt(intervalPos));
						}
						ProcessPixel(frameBuffer, corner_x1 + xoffset_adj, y_pos + yoffset_adj, color, wrap_x, wrap_y);
						UpdateMarqueeColor(ref current_pos, ref current_color, colorcnt, color_size, 1 * sign);
					}
				}

				corner_x1++;
				corner_y1++;
				corner_x2--;
				corner_y2--;
			}
		}

		/// <summary>
		/// Updates the next Marquee band color.
		/// </summary>
		/// <param name="position">Position to determine color for</param>
		/// <param name="band_color">Current band color</param>
		/// <param name="colorcnt">Number of repeating colors</param>
		/// <param name="color_size">Size of the color band</param>
		/// <param name="shift">Stagger shift being used</param>
		private void UpdateMarqueeColor(ref int position, ref int band_color, int colorcnt, int color_size, int shift)
		{
			if (shift == 0) return;
			if (shift > 0)
			{
				int index = 0;
				while (index < shift)
				{
					position++;
					if (position >= color_size)
					{
						band_color++;
						band_color %= colorcnt;
						position = 0;
					}
					index++;
				}
			}
			else
			{
				int index = 0;
				while (index > shift)
				{
					position--;
					if (position < 0)
					{
						band_color++;
						band_color %= colorcnt;
						position = color_size - 1;
					}
					index--;
				}
			}
		}

		/// <summary>
		/// Sets a marquee color on the frame buffer.
		/// </summary>
		/// <param name="buffer">Frame buffer to update</param>
		/// <param name="x_pos">X Position of the pixel</param>
		/// <param name="y_pos">Y Position of the pixel</param>
		/// <param name="color">Color of the pixel</param>
		/// <param name="wrap_x">Whether wrapping around the display element in the X axis is enabled</param>
		/// <param name="wrap_y">Whether wrapping around the display element in the Y axis is enabled</param>
		private void ProcessPixel(IPixelFrameBuffer buffer, int x_pos, int y_pos, Color color, bool wrap_x, bool wrap_y)
		{

			int x_value = x_pos;
			if (wrap_x)  // if set wrap image at boundary
			{
				x_value %= BufferWi;
				x_value = (x_value >= 0) ? (x_value) : (BufferWi + x_value);
			}
			int y_value = y_pos;
			if (wrap_y)
			{
				y_value %= BufferHt;
				y_value = (y_value >= 0) ? (y_value) : (BufferHt + y_value);
			}
			buffer.SetPixel(x_value, y_value, color);
		}

		#endregion
	}
}
