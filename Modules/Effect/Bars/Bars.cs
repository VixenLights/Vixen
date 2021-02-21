using Common.Controls.ColorManagement.ColorModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using ZedGraph;
using Color = System.Drawing.Color;

namespace VixenModules.Effect.Bars
{
    public class Bars : PixelEffectBase
    {
        #region Private Fields

        private BarsData _data;
        private double _position;
        private bool _negPosition;

        /// <summary>
        /// This frame buffer contains a tile of the flat bars.
        /// This tile can be repeated to fill the display element.
        /// </summary>
        private IPixelFrameBuffer _staticFlatFrameBuffer;

        /// <summary>
        /// Height of a single flat bar.
        /// </summary>
        private int _flatBarsBarHeight;

        /// <summary>
        /// This frame buffer contains a tile of the zig zags.
        /// This tile can be repeated to fill the display element.
        /// </summary>
        private IPixelFrameBuffer _staticZigZagTileFrameBuffer;

        /// <summary>
        /// The height of the zig zag tile frame buffer.       
        /// </summary>
        private int _staticZigZagTileFrameBufferHeight;

        /// <summary>
        /// Height of the zig zag repeating tile.
        /// Note thie frame buffer associated with this repeating tile is actually larger because the first part
        /// of the buffer contains a subset of the zig zag colors (ie it is incomplete). 
        /// </summary>
        private int _heightOfTile;

        /// <summary>
        /// Width of the zig zag repeating tile.
        /// </summary>
        private int _widthOfTile;

        /// <summary>
        /// Start position or offset into the tile frame buffer.  This offset accounts for the first part of the
        /// frame buffer being incomplete.
        /// </summary>
        private int _yTileStartPosition;

        /// <summary>
        /// Zig zag thickness in pixels.
        /// </summary>
        private int _zigZagThickness;

        /// <summary>
        /// Spacing between the zig zags in pixels.
        /// </summary>
        private int _zigZagSpacing;

        /// <summary>
        /// Amplitude of the zig zag in pixels.
        /// </summary>
        private int _zigZagAmplitude;

        /// <summary>
        /// Period of the zig zag in pixels.
        /// </summary>
        private int _zigZagPeriod;

        /// <summary>
        /// Scale factor used on all the zig zag settings.
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

        #endregion

        #region Constructor

        public Bars()
        {
            _data = new BarsData();
            EnableTargetPositioning(true, true);
            InitAllAttributes();
        }

        #endregion

        #region Public (Override) Methods

        public override bool IsDirty
        {
            get
            {
                if (Colors.Any(x => !x.CheckLibraryReference()))
                {
                    base.IsDirty = true;
                }

                return base.IsDirty;
            }
            protected set { base.IsDirty = value; }
        }

        #endregion

        #region Public Properties

        public override IModuleDataModel ModuleData
        {
            get { return _data; }
            set
            {
                _data = value as BarsData;
                InitAllAttributes();
                IsDirty = true;
            }
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

        #endregion

        #region Config properties

        [Value]
        [ProviderCategory(@"Config", 1)]
        [ProviderDisplayName(@"BarType")]
        [ProviderDescription(@"BarType")]
        [PropertyOrder(0)]
        public BarType BarType
        {
            get { return _data.BarType; }
            set
            {
                _data.BarType = value;

                // If the bar type is being changed to Zig Zag then...
                if (value == BarType.ZigZag)
                {
                    // Change the movement type to speed; we don't support iterations with the zig zag bar type
                    // because the height and spacing is user configurable
                    MovementType = MovementType.Speed;

                    // The default for the speed when using the straight bar type is 50 which was too fast
                    // given the scaling used for the zig zag bar type.
                    SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
                }
                else
                {
                    // Reset the movement type to iterations
	                MovementType = MovementType.Iterations;

                    // When switching back to flat bar type reset the speed curve back to the default
	                SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
                }

                IsDirty = true;
                UpdateBarTypeAttributes();
                OnPropertyChanged();
            }
        }


        [Value]
        [ProviderCategory(@"Config", 1)]
        [ProviderDisplayName(@"Direction")]
        [ProviderDescription(@"Direction")]
        [PropertyOrder(1)]
        public BarDirection Direction
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
        [ProviderDescription(@"BarsRotation")]
        [PropertyOrder(2)]
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
        [ProviderDisplayName(@"MovementType")]
        [ProviderDescription(@"MovementType")]
        [PropertyOrder(3)]
        public MovementType MovementType
        {
            get { return _data.MovementType; }
            set
            {
                _data.MovementType = value;
                IsDirty = true;
                UpdateMovementTypeAttribute();
                OnPropertyChanged();
            }
        }

        [Value]
        [ProviderCategory(@"Config", 1)]
        [ProviderDisplayName(@"Iterations")]
        [ProviderDescription(@"Iterations")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(0, 20, 1)]
        [PropertyOrder(4)]
        public int Speed
        {
            get { return _data.Speed; }
            set
            {
                _data.Speed = value;
                IsDirty = true;
                OnPropertyChanged();
            }
        }

        [Value]
        [ProviderCategory(@"Config", 1)]
        [ProviderDisplayName(@"Speed")]
        [ProviderDescription(@"Speed")]
        [PropertyOrder(5)]
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
        [ProviderDisplayName(@"Repeat")]
        [ProviderDescription(@"Repeat")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(1, 10, 1)]
        [PropertyOrder(6)]
        public int Repeat
        {
            get { return _data.Repeat; }
            set
            {
                _data.Repeat = value;
                IsDirty = true;
                OnPropertyChanged();
            }
        }

        [Value]
        [ProviderCategory(@"Config", 1)]
        [ProviderDisplayName(@"Highlight")]
        [ProviderDescription(@"Highlight")]
        [PropertyOrder(7)]
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
        [PropertyOrder(8)]
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
        [PropertyOrder(9)]
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

        #endregion

        #region Color properties

        [Value]
        [ProviderCategory(@"Color", 2)]
        [ProviderDisplayName(@"ColorGradients")]
        [ProviderDescription(@"Color")]
        [PropertyOrder(1)]
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

        #region Level properties

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

        #region Zig Zag Properties

        [Value]
        [PropertyEditor("SliderEditor")]
        [NumberRange(0, 100, 1)]
        [ProviderCategory(@"ZigZagConfig", 4)]
        [ProviderDisplayName(@"ZigZagAmplitude")]
        [ProviderDescription(@"ZigZagAmplitude")]
        [PropertyOrder(0)]
        public int ZigZagAmplitude
        {
            get
            {
                return _data.ZigZagAmplitude;
            }
            set
            {
                _data.ZigZagAmplitude = value;
                IsDirty = true;
                OnPropertyChanged();
            }
        }

        [Value]
        [PropertyEditor("SliderEditor")]
        [NumberRange(1, 100, 1)]
        [ProviderCategory(@"ZigZagConfig", 4)]
        [ProviderDisplayName(@"ZigZagPeriod")]
        [ProviderDescription(@"ZigZagPeriod")]
        [PropertyOrder(1)]
        public int ZigZagPeriod
        {
            get
            {
                return _data.ZigZagPeriod;
            }
            set
            {
                _data.ZigZagPeriod = value;
                IsDirty = true;
                OnPropertyChanged();
            }
        }

        [Value]
        [PropertyEditor("SliderEditor")]
        [NumberRange(1, 100, 1)]
        [ProviderCategory(@"ZigZagConfig", 4)]
        [ProviderDisplayName(@"ZigZagThickness")]
        [ProviderDescription(@"ZigZagThickness")]
        [PropertyOrder(2)]
        public int ZigZagBarThickness
        {
            get
            {
                return _data.ZigZagBarThickness;
            }
            set
            {
                _data.ZigZagBarThickness = value;
                IsDirty = true;
                OnPropertyChanged();
            }
        }

        [Value]
        [PropertyEditor("SliderEditor")]
        [NumberRange(0, 100, 1)]
        [ProviderCategory(@"ZigZagConfig", 4)]
        [ProviderDisplayName(@"ZigZagSpacing")]
        [ProviderDescription(@"ZigZagSpacing")]
        [PropertyOrder(3)]
        public int ZigZagSpacing
        {
            get
            {
                return _data.ZigZagSpacing;
            }
            set
            {
                _data.ZigZagSpacing = value;
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
            get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/bars/"; }
        }

        #endregion

        #region Protected Properties

        protected override EffectTypeModuleData EffectModuleData => _data;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handler for when the target positioning changes.
        /// </summary>
        protected override void TargetPositioningChanged()
        {
            // Update the visibility of the highlight percentage
            UpdateHighlightAttributes();
        }

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
	        // If the bar type is flat then...
            if (BarType == BarType.Flat)
            {
                RenderEffectByLocationFlat(numFrames, frameBuffer);
            }
            // Otherwise render the zig zag bars
            else
            {
                RenderEffectByLocationZigZag(numFrames, frameBuffer);
            }
        }

        /// <summary>
        /// Perform calculations that only need to be performed once per rendering.
        /// </summary>
        protected override void SetupRender()
        {
	        // Calculate the diagonal length of the display element
            _length = (int)Math.Round(Math.Sqrt((BufferHt * BufferHt) + (BufferWi * BufferWi)), MidpointRounding.AwayFromZero);

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

            // If the bar type is zig zag then...
            if (BarType == BarType.ZigZag)
            {
                // Determine the minimum between the display element height and width
                _scaleValue = GetScaleValue(BufferHt, BufferWi);

                // Calculate the zig zag bar thickness
                _zigZagThickness = GetZigZagThickness(_scaleValue);

                // Calculate the zig zag bar spacing
                _zigZagSpacing = GetZigZagSpacing(_scaleValue);

                // Calculate the height of the zig zag repeating tile
                _heightOfTile = GetZigZagRepeatingHeight(_zigZagSpacing, _zigZagThickness);

                // Calculate the period of the zig zag
                _zigZagPeriod = GetZigZagPeriod(_scaleValue);

                // Calculate the width of the zig zag repeating tile
                _widthOfTile = GetZigZagRepeatingWidth(_scaleValue);

                // Calculate the amplitude of the zig zag
                _zigZagAmplitude = GetZigZagAmplitude(_scaleValue);

                // Calculate the Y start position of the repeating tile
                _yTileStartPosition = GetZigZagTileYStartPosition();

                // Calculate the height of the repeating tile frame buffer
                _staticZigZagTileFrameBufferHeight = _heightOfTile + _yTileStartPosition;

                // Initialize the repeating tile frame buffer
                _staticZigZagTileFrameBuffer = new PixelFrameBuffer(_widthOfTile, _staticZigZagTileFrameBufferHeight);

                // Draw the repeating zig zag tile
                SetupRenderZigZag();
            }
            else
            {
                // Setup the flat bar settings
                SetupRenderFlat();
            }
        }

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

        #endregion

        #region Private Methods

        /// <summary>
        /// Renders the effect in string mode using the specified frame buffer.
        /// </summary>
        /// <param name="frame">Frame to render</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectStringsInternal(int frame, IPixelFrameBuffer frameBuffer)
        {
	        // If the bar type is flat then...
	        if (BarType == BarType.Flat)
	        {
		        RenderEffectFlat(frame, frameBuffer);
	        }
            // Otherwise render the zig-zag bars
	        else
	        {
		        RenderEffectStringsZigZag(frame, frameBuffer);
	        }
        }

        /// <summary>
        /// Gets the amplitude of the zig zag bar.
        /// </summary>        
        /// <param name="scaleValue">Value used to scale the amplitude</param>
        /// <returns>Amplitude of the zig zag</returns>
        private int GetZigZagAmplitude(int scaleValue)
        {
            // Gets the amplitude setting and scales based on the display element size
            int amplitude = (int)((ZigZagAmplitude) / 100.0 * scaleValue);
            
            return amplitude;
        }

        /// <summary>
        /// Gets the period of the zig zag bar.
        /// </summary>       
        /// <param name="scaleValue">Value used to scale the zig zag period</param>
        /// <returns>Period of the zig zag</returns>
        private int GetZigZagPeriod(int scaleValue)
        {           
            // Get the scaled zig zag period
            int period = (int)((ZigZagPeriod) / 100.0 * scaleValue);

            // Ensure the period is always at least 2 pixels
            if (period < 2)
            {
                period = 2;
            }

            return period;
        }

        /// <summary>
        /// Gets the spacing in between the zig zag bars.
        /// </summary>    
        /// <param name="scaleValue">Value used to scale the spacing</param>
        /// <returns>Returns the spacing between the zig zag bars</returns>
        private int GetZigZagSpacing(int scaleValue)
        {            
            // Calculate the spacing between the zig zag bars
            int spacing = (int)(ZigZagSpacing / 100.0 * scaleValue);
           
            return spacing;
        }

        /// <summary>
        /// Initializes the visibility of the attributes.
        /// </summary>
        private void InitAllAttributes()
        {
            UpdateStringOrientationAttributes(true);
            UpdateMovementTypeAttribute(false);
            UpdateBarTypeAttributes(false);
            UpdateHighlightAttributes(false);
            TypeDescriptor.Refresh(this);
        }

        /// <summary>
        /// Updates the visibility of movement type attributes.
        /// </summary>  
        private void UpdateMovementTypeAttribute(bool refresh = true)
        {
            Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
            {
                { "SpeedCurve", MovementType == MovementType.Speed},
                { "Speed", MovementType != MovementType.Speed}
            };
            SetBrowsable(propertyStates);
            if (refresh)
            {
                TypeDescriptor.Refresh(this);
            }
        }

        /// <summary>
        /// Updates the visibility of bar type attributes.
        /// </summary>
        private void UpdateBarTypeAttributes(bool refresh = true)
        {
            Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(5)
            {
                { nameof(Repeat), BarType == BarType.Flat },
                { nameof(MovementType), BarType == BarType.Flat },
                { nameof(ZigZagAmplitude), BarType == BarType.ZigZag },
                { nameof(ZigZagPeriod), BarType == BarType.ZigZag },
                { nameof(ZigZagBarThickness), BarType == BarType.ZigZag },
                { nameof(ZigZagSpacing), BarType == BarType.ZigZag },
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
		        { nameof(HighlightPercentage), Highlight && TargetPositioning == TargetPositioningType.Locations},
	        };
	        SetBrowsable(propertyStates);
	        if (refresh)
	        {
		        TypeDescriptor.Refresh(this);
	        }
        }

        /// <summary>
        /// Gets the height of the zig zag repeating tile.
        /// </summary>        
        /// <param name="zigZagThickness">Thickness of the zig zag</param>
        /// <param name="zigZagSpacing">Spacing in between the zig zag bars</param>
        /// <returns>Zig zag repeating tile height</returns>
        private int GetZigZagRepeatingHeight(int zigZagThickness, int zigZagSpacing)
        {
            // The repeating height is the thickness of the bar plus the spacing between the bars times the number of bars (colors)
            return Colors.Count * (zigZagThickness + zigZagSpacing);
        }

        /// <summary>
        /// Gets the width of the zig zag repeating tile.
        /// </summary>
        /// <param name="scaleValue">Scale value based on the width and height of the display element</param>
        /// <returns>Width of the zig zag repeating tile</returns>
        private int GetZigZagRepeatingWidth(int scaleValue)
        {
            // Multiplying by two to cover the part of the zig zag going up and
            // the part going down.  Subtracting 1 since there is only one peak and 
            // we only need to count it once.  Subtracting the second so that the next
            // pixel is the starting point.
            return 2 * GetZigZagPeriod(scaleValue) - 2;
        }

        /// <summary>
        /// Gets the zig zag thickness as a percentage of the display element.
        /// </summary>        
        /// <param name="scaleValue">Value used to scale the thickness</param>
        /// <returns>Thickness of the zig zag</returns>
        private int GetZigZagThickness(int scaleValue)
        {
            // Get the thickness of the zig zag bar as a percentage of the width or height
            int thickness = (int)(ZigZagBarThickness / 100.0 * scaleValue);

            // Ensure the thickness is always at least one pixel
            if (thickness == 0)
            {
                thickness = 1;
            }

            return thickness;
        }

        /// <summary>
        /// Gets the highlight color for the flat bar / zig zag color index.
        /// </summary>
        /// <returns>Highlight color for the specified color index</returns>
        private Color GetHighlightColor(int colorIndex)
        {
            // The color gradients run perpendicular to the bars so the highlight always
            // impacts the beginning of the gradient
            Color highlightColor = Colors[colorIndex].GetColorAt(0);

            // Convert from RGB to HSV color format 
            HSV hsv = HSV.FromRGB(highlightColor);

            // Highlight the zig zag
            hsv.S = 0.0f;

            // Convert the HSB color back to RGB
            highlightColor = hsv.ToRGB();

            return highlightColor;
        }

        /// <summary>
        /// The design of the zig-zag portion of this effect draws a small repeating tile of the zig zags.
        /// This tile is then repeated across the display element.  
        /// </summary>       
        private void SetupRenderZigZag()
        {
            // Default to the first color in the color array
            int colorIndex = 0;

            // Initialize the zig zag thickness counter
            int currentThicknessCounter = 0;

            // Default to the first color in the color array
            Color color = GetBarColor(colorIndex, currentThicknessCounter, _zigZagThickness);

            // Calculate the increment of the zig zag (this value affects the slope of the zig zag)
            double dZigZagIncrement = (_zigZagAmplitude - 1) / (double)(_zigZagPeriod - 1);

            // When the zig zag amplitude is set to zero the increment will go negative
            if (dZigZagIncrement < 0)
            {
                // Set the increment to zero so that we draw a flat bar
                dZigZagIncrement = 0;
            }

            // Loop over the height of the zig zag tile
            for (int y = 0; y < _staticZigZagTileFrameBufferHeight; y++)
            {
                // Reset the vertical position of the zig zag
                double y1 = 0;

                // Initialize the zig zag increment to going up
                double increment = dZigZagIncrement;

                // Loop over the width of the zig zag tile
                for (int x = 0; x < _widthOfTile; x++)
                {
                    // If the Y position is NOT off the zig zag tile then...
                    if ((y + y1) < _staticZigZagTileFrameBufferHeight)
                    {
                        // Set the color on the tile
                        _staticZigZagTileFrameBuffer.SetPixel(x, (int)(y + y1), color);
                    }

                    // Increment the Y coordinate of the zig zag
                    y1 += increment;

                    // Check to see if the zig zag is done going up and the increment needs to switch to negative
                    // so that the zig zag starts going down
                    if (y1 >= (_zigZagAmplitude - 1))
                    {
                        // Flip the increment to negative
                        increment = -dZigZagIncrement;
                    }
                    else if (y1 <= 0)
                    {
                        // Flip the increment to positive
                        increment = dZigZagIncrement;
                    }
                }

                // Increment the thickness pixel counter
                currentThicknessCounter++;

                // If we have completed either a zig zag bar or
                // a blank space zig zag bar then...
                if (color != Color.Transparent && currentThicknessCounter == _zigZagThickness ||
                    color == Color.Transparent && currentThicknessCounter == _zigZagSpacing)
                {
                    // Reset the bar thickness counter
                    currentThicknessCounter = 0;

                    // If the color was not transparent then we just completed a zig zag bar
                    if (color != Color.Transparent && _zigZagSpacing != 0)
                    {
                        // Switch the color to transparent
                        color = Color.Transparent;
                    }
                    // Otherwise we just completed a blank zig zag bar
                    else
                    {
                        // Increment to the next color in the array
                        colorIndex++;

                        // If we are at the end of the color array then...
                        if (colorIndex == Colors.Count)
                        {
                            // Go back to the beginning of the color array
                            colorIndex = 0;
                        }

                        // Get the color for the zig zag bar
                        color = GetBarColor(colorIndex, currentThicknessCounter, _zigZagThickness);
                    }
                }
                // Otherwise if we are not processing a blank zig zag bar then...
                else if (color != Color.Transparent)
                {
                    // Get the color for the zig zag bar
                    color = GetBarColor(colorIndex, currentThicknessCounter, _zigZagThickness);
                }
            }
        }

        /// <summary>
        /// Gets the color for the bar taking into account the specified pixel position within the bar.
        /// </summary>
        /// <param name="colorIndex">Index of the current color</param>       
        /// <param name="currentThicknessCounter">Pixel position within the bar</param>
        /// <param name="barThickness">Thickness of the bars</param>
        /// <returns>Gets the color for the specified bar pixel</returns>
        private Color GetBarColor(int colorIndex, int currentThicknessCounter, int barThickness)
        {
            Color color;

            // Default the highlight to only the first row of pixels
            int highLightRow = 0;

            // If the effect is in locations mode then...
            if (TargetPositioning == TargetPositioningType.Locations)
            {
	            // Make the first % pixels white
	            highLightRow = (int) (barThickness * HighlightPercentage / 100.0);
            }

            // If we are at the beginning of the bar and
            // highlight is selected then...
            if (Highlight && 
				currentThicknessCounter <= highLightRow)
            {
                // Set the color to the highlight color
                color = GetHighlightColor(colorIndex);
            }
            // Otherwise if the bar is 3-D then...
            else if (Show3D)
            {
                // Set the color to the 3-D color
                color = Get3DColor(colorIndex, currentThicknessCounter, barThickness);
            }
            else
            {
                // Default to the gradient position based on the position in the bar
                color = Colors[colorIndex].GetColorAt(currentThicknessCounter / (double)barThickness);
            }

            // Return the color of the bar
            return color;
        }

        /// <summary>
        /// Gets the 3-D color for the specified bar pixel.
        /// </summary>
        /// <param name="colorIndex">Index into the color array</param>
        /// <param name="currentThicknessCounter">Current row of the bar being drawn</param>
        /// <param name="barThickness">Thickness of the bar</param>
        /// <returns>3-D color for the specified bar pixel</returns>
        private Color Get3DColor(int colorIndex, int currentThicknessCounter, int barThickness)
        {
            // Get the specified color from the color array
            Color color = Colors[colorIndex].GetColorAt(currentThicknessCounter / (double)barThickness);

            // Convert from RGB to HSV color format 
            HSV hsv = HSV.FromRGB(color);

            // Set the brightness based on the percentage of the bar thickness
            hsv.V *= (float)(barThickness - currentThicknessCounter) / barThickness;

            // Convert the color back to RGB format
            return hsv.ToRGB();
        }

        /// <summary>
        /// Renders the zig zag bars in strings mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectStringsZigZag(int frame, IPixelFrameBuffer frameBuffer)
        {
            // If the zig zag bars are moving in one of the alternate directions then...
            if (Direction == BarDirection.AlternateUp ||
                Direction == BarDirection.AlternateDown ||
                Direction == BarDirection.AlternateLeft ||
                Direction == BarDirection.AlternateRight)
            {
                // update the zig zag position 
                UpdateZigZagPosition(frame);
            }

            // Render the zig zag based on the direction
            switch (Direction)
            {
                case BarDirection.Up:
                case BarDirection.Down:
                case BarDirection.AlternateUp:
                case BarDirection.AlternateDown:
                    RenderEffectZigZagVertical(frame, frameBuffer);
                    break;
                case BarDirection.Left:
                case BarDirection.Right:
                case BarDirection.AlternateLeft:
                case BarDirection.AlternateRight:
                    RenderEffectZigZagHorizontal(frame, frameBuffer);
                    break;
                case BarDirection.Expand:
                    RenderEffectZigZagVerticalExpand(frame, frameBuffer);
                    break;
                case BarDirection.Compress:
                    RenderEffectZigZagVerticalCompress(frame, frameBuffer);
                    break;
                case BarDirection.HExpand:
                    RenderEffectZigZagHorizontalExpand(frame, frameBuffer);
                    break;
                case BarDirection.HCompress:
                    RenderEffectZigZagHorizontalCompress(frame, frameBuffer);
                    break;
                default:
                    Debug.Assert(false, "Unsupported Direction!");
                    break;
            }            
        }

        /// <summary>
        /// Updates the zig zag position.  This position is only used when the Direction is 
        /// one of the Alternating values.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        private void UpdateZigZagPosition(int frame)
        {
            // Initialize the interval position factor
            double intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;
           
            // If this is the first frame then...
            if (frame == 0)
            {
                // Initialize the position
                _position = CalculateZigZagSpeed(intervalPosFactor);
            }

            // Increment the position
            _position += CalculateZigZagSpeed(intervalPosFactor) / 100; 
        }

        /// <summary>
        /// Updates the flat bar position.  
        /// </summary>
        /// <param name="frame">Current frame number</param>
        private void UpdateFlatPosition(int frame)
        {
	        double intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;

	        if (MovementType == MovementType.Iterations)
	        {
		        _position = (GetEffectTimeIntervalPosition(frame) * Speed) % 1;
	        }
	        else
	        {
		        if (frame == 0)
		        {
			        _position = CalculateFlatBarSpeed(intervalPosFactor);
		        }

		        _position += CalculateFlatBarSpeed(intervalPosFactor) / 100;
	        }
        }

        /// <summary>
        /// Gets the alternating mode pixel color for the specified Y coordinate.
        /// </summary>
        /// <param name="y">Y Coordinate to evaluate</param>
        /// <param name="frame">Current frame number</param>
        /// <param name="blockHt">Height of the repeating block (Bar Height * Color Count)</param>
        /// <param name="barCount">Number of bars</param>
        /// <param name="barHt">Bar height</param>
        /// <returns>Color of the alternating bar for the specified Y coordinate</returns>
        private Color GetFlatBarsAlternatingColor(int y, int frame, int blockHt, int barCount, int barHt)
        {
	        // If the direction is Up or Left then...
	        if (Direction == BarDirection.AlternateUp ||
	            Direction == BarDirection.AlternateLeft)
	        {
                // Need to flip the coordinates around
                // Subtract the coordinate from the height of the buffer
		        y = GetFlatBarsBufferHeight() - y;
		        
                // If the Y is outside the buffer
		        while (y < 0)
		        {
                    // Add the buffer height to the coordinate to make it positive
			        y += GetFlatBarsBufferHeight();
		        }
	        }

            // Update the flat bars position
            UpdateFlatPosition(frame);
            
            // The following math was copied from the original effect algorithm prior to adding rotation
            int fOffset = (int) (Math.Floor(_position * barCount) * barHt);
            int indexAdjust = 1;
            int n = y + fOffset;

	        int colorIdx = ((n + indexAdjust) % blockHt) / barHt;

            return GetBarColor(colorIdx, (n + indexAdjust) % barHt, barHt);
        }

        /// <summary>
        /// Renders the zig zag bars in location mode.
        /// </summary>
        /// <param name="numFrames">Number of frames to render</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectByLocationZigZag(int numFrames, PixelLocationFrameBuffer frameBuffer)
        {
            // Loop over all the frames
            for (int frame = 0; frame < numFrames; frame++)
            {
                // Set the current frame number
                frameBuffer.CurrentFrame = frame;

                // If the zig zag bars are moving in one of the alternate directions then...
                if (Direction == BarDirection.AlternateUp ||
                    Direction == BarDirection.AlternateDown ||
                    Direction == BarDirection.AlternateLeft ||
                    Direction == BarDirection.AlternateRight)
                {
                    // Update the position 
                    UpdateZigZagPosition(frame);
                }

                switch (Direction)
                {
                    case BarDirection.Up:
                    case BarDirection.Down:
                    case BarDirection.AlternateUp:
                    case BarDirection.AlternateDown:
                        RenderEffectLocationZigZagVertical(frame, frameBuffer);
                        break;
                    case BarDirection.Left:
                    case BarDirection.Right:
                    case BarDirection.AlternateLeft:
                    case BarDirection.AlternateRight:
                        RenderEffectLocationZigZagHorizontal(frame, frameBuffer);
                        break;
                    case BarDirection.Expand:
                        RenderEffectLocationZigZagVerticalExpand(frame, frameBuffer);
                        break;
                    case BarDirection.Compress:
                        RenderEffectLocationZigZagVerticalCompress(frame, frameBuffer);
                        break;
                    case BarDirection.HExpand:
                        RenderEffectLocationZigZagHorizontalExpand(frame, frameBuffer);
                        break;
                    case BarDirection.HCompress:
                        RenderEffectLocationZigZagHorizontalCompress(frame, frameBuffer);
                        break;
                    default:
                        Debug.Assert(false, "Unsupported Direction!");
                        break;
                }
            }
        }

        /// <summary>
        /// Renders the flat bars in location mode.
        /// </summary>
        /// <param name="numFrames">Number of frames to render</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectByLocationFlat(int numFrames, PixelLocationFrameBuffer frameBuffer)
        {
            // Loop over all the frames
            for (int frame = 0; frame < numFrames; frame++)
            {
                // Set the current frame number
                frameBuffer.CurrentFrame = frame;

                // If the Direction is not alternating then we need update the position
                // so that the bars move
                if (Direction != BarDirection.AlternateUp &&
                    Direction != BarDirection.AlternateDown &&
                    Direction != BarDirection.AlternateLeft &&
                    Direction != BarDirection.AlternateRight)
                {
	                // Update the flat bar position 
	                UpdateFlatPosition(frame);
                }

                // Update the bars based on the selected Direction
                switch (Direction)
                {
                    case BarDirection.Up:
                    case BarDirection.Down:
                    case BarDirection.AlternateUp:
                    case BarDirection.AlternateDown:
                        RenderEffectLocationFlatVertical(frame, frameBuffer);
                        break;
                    case BarDirection.Left:
                    case BarDirection.Right:
                    case BarDirection.AlternateLeft:
                    case BarDirection.AlternateRight:
                        RenderEffectLocationFlatHorizontal(frame, frameBuffer);
                        break;
                    case BarDirection.Expand:
                        RenderEffectLocationFlatVerticalExpand(frame, frameBuffer);
                        break;
                    case BarDirection.Compress:
                        RenderEffectLocationFlatVerticalCompress(frame, frameBuffer);
                        break;
                    case BarDirection.HExpand:
                        RenderEffectLocationFlatHorizontalExpand(frame, frameBuffer);
                        break;
                    case BarDirection.HCompress:
                        RenderEffectLocationFlatHorizontalCompress(frame, frameBuffer);
                        break;
                    default:
                        Debug.Assert(false, "Unsupported Direction!");
                        break;
                }
            }
        }

        /// <summary>
        /// Returns the start index or position within the static zig zag tile.  The
        /// first part of the tile is incomplete because we start with one color
        /// and draw up.
        /// </summary>       
        /// <returns>Y start position within the repeating tile</returns>
        private int GetZigZagTileYStartPosition()
        {
            // Skip the first 
            return (_zigZagAmplitude + _zigZagThickness - 1 + _zigZagSpacing) + (Colors.Count - 1) * (_zigZagThickness + _zigZagSpacing);
        }

        /// <summary>
        /// Determines the buffer height of the display element taking the selected direction into account.
        /// </summary>
        /// <returns>Logical height of the display element</returns>
        private int GetFlatBarsBufferHeight()
        {
            int bufferHeight = 0;

            switch (Direction)
            {
                case BarDirection.Up:
                case BarDirection.Down:
                case BarDirection.Expand:
                case BarDirection.Compress:
                case BarDirection.AlternateUp:
                case BarDirection.AlternateDown:
                    bufferHeight = BufferHt;
                    break;
                case BarDirection.Left:
                case BarDirection.Right:
                case BarDirection.HExpand:
                case BarDirection.HCompress:
                case BarDirection.AlternateLeft:
                case BarDirection.AlternateRight:
                    bufferHeight = BufferWi;
                    break;
                default:
                    Debug.Assert(false, "Unsupported Direction");
                    break;
            }

            return bufferHeight;
        }

        /// <summary>
        /// Initializes the settings for the flat bars.
        /// </summary>
        private void SetupRenderFlat()
        {
            // Retrieve the number of bar colors
            int colorcnt = Colors.Count();

            // Determine the number of bars to display on the display element
            int barCount = Repeat * colorcnt;

            // Ensure that the bar count is always positive
            if (barCount < 1) barCount = 1;

            // Calculate the height of each of the bars
            int barHt = GetFlatBarsBufferHeight() / barCount + 1;

            // Ensure the bar height is at least one pixel
            if (barHt < 1)
            {
                barHt = 1;
            }

            // Save off the bar height
            _flatBarsBarHeight = barHt;

            // Calculate the height of the repeating tile
            // This represents the height before the pattern repeats
            _heightOfTile = colorcnt * barHt;

            // Ensure that the bar height is at least one pixel
            if (_heightOfTile < 1)
            {
                _heightOfTile = 1;
            }

            // Bars don't vary across the bar so we only need one pixel
            _widthOfTile = 1;

            // Create the static frame buffer for the tile
            _staticFlatFrameBuffer = new PixelFrameBuffer(_widthOfTile, _heightOfTile);

            int y = 0;

            // Loop over the colors
            for (int colorIndex = 0; colorIndex < colorcnt; colorIndex++)
            {
                // Loop over the bar thickness
                for (int thicknessCounter = 0; thicknessCounter < barHt; thicknessCounter++)
                {
                    // Get the color for the specified pixel of the bar
                    Color color = GetBarColor(colorIndex, thicknessCounter, barHt);

                    // Store off the pixel in the tile
                    _staticFlatFrameBuffer.SetPixel(0, y, color);

                    // Advance to the next row of the bar
                    y++;
                }
            }
        }

        /// <summary>
        /// Calculates the speed for the flat bars.
        /// </summary>
        /// <param name="intervalPos">Position within the effect timespan</param>
        /// <returns>Speed of the flat bars</returns>
        private double CalculateFlatBarSpeed(double intervalPos)
        {
	        return ScaleCurveToValue(SpeedCurve.GetValue(intervalPos), 15, -15);
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
        /// Return the initial Y position within the flat bars repeating tile.
        /// </summary>
        /// <param name="increment">True when incrementing within the tile</param>
        /// <param name="heightOfTile">Height of the tile</param>
        /// <param name="barThickness">Thickness of the bar</param>
        /// <param name="frame">Current frame number</param>
        /// <returns>Initial Y position within the repeating tile</returns>
        private int InitializeFlatBarTileYPosition(
            bool increment,
            int heightOfTile,
            int barThickness,
            int frame)
        {
            // Calculate the Y position within the repeating tile
            int tileY = (int)(_position * _heightOfTile * Repeat);

            // Use the mod operator to figure out how many times to repeat the tile
            // and then what portion of the tile is left over
            tileY = tileY % heightOfTile;

            return tileY;
        }

        /// <summary>
        /// Defines a delegate so the algorithm used to initialize the tile Y position can be passed into common methods
        /// that support both the flat bars and the zig zag bars.
        /// </summary>
        /// <param name="increment">Determines if the Y position increments slowly or it jumps (one of the alternate directions selected)</param>
        /// <param name="heightOfTile">Height of the repeating tile</param>
        /// <param name="thicknessOfBar">Thickness of the bar</param>
        /// <param name="frame">Current frame number</param>
        /// <returns></returns>
        private delegate int InitializeTileYPosition(
            bool increment,
            int heightOfTile,
            int thicknessOfBar,
            int frame);

        /// <summary>
        /// Initializes the Y position within the zig zag repeating tile.
        /// </summary>
        /// <param name="increment">True when incrementing within the tile</param>
        /// <param name="heightOfTile">Height of the tile</param>
        /// <param name="barThickness">Thickness of the zig zag bar</param>
        /// <param name="frame">Current frame number</param>
        /// <returns>Initial Y position within the repeating tile</returns>
        private int InitializeZigZagTileYPosition(
            bool increment,
            int heightOfTile,
            int barThickness,
            int frame)
        {
            int tileY;

            // If the position within the tile is incrementing by a small amount then...
            if (increment)
            {
                // Calculate the position within the repeating tile
                tileY = GetZigZagYValue(frame) % heightOfTile;
            }
            // Otherwise if the position is alternating then...
            else
            {
                // Calculate the position within the repeating tile
                tileY = (int)(Math.Floor(_position * Colors.Count) * barThickness);
                tileY = tileY % heightOfTile;
            }

            return tileY;
        }

        /// <summary>
        /// Updates the Y position within the zig zag repeating tile.
        /// </summary>
        /// <param name="increment">Whether the zig zag is incrementing through the tile</param>
        /// <param name="tileY">Current Y position within the tile</param>
        /// <param name="heightOfTile">Height of the repeating tile</param>
        /// <returns></returns>
        private int UpdateZigZagTileYPosition(bool increment, int tileY, int heightOfTile)
        {
            // If the zig zag is incrementing then...
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
            // Otherwise the zig zag is decrementing
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
        private void SetPixelHorizontal(int frame, IPixelFrameBuffer frameBuffer, int x, int y, int tileX, int tileY, int yTileStartPosition)
        {
            // Transfer a pixel from the tile to the frame buffer
            frameBuffer.SetPixel(y, x, AdjustIntensity(_staticZigZagTileFrameBuffer.GetColorAt(tileX, tileY + yTileStartPosition), frame));
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
        private void SetPixelVertical(int frame, IPixelFrameBuffer frameBuffer, int x, int y, int tileX, int tileY, int yTileStartPosition)
        {
            // Transfer a pixel from the tile to the frame buffer
            frameBuffer.SetPixel(x, y, AdjustIntensity(_staticZigZagTileFrameBuffer.GetColorAt(tileX, tileY + yTileStartPosition), frame));
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
        /// Calculates the pixel color for the specified coordinate.  This delegate exists so that common routines can be used between
        /// the flat bars and the zig zag bars.
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
        /// Alternating bars are not moving bars.
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
            int yTile = CalculateZigZagYTilePosition(
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

        /// <summary>
        /// Calculates the color of pixel that is part of an alternating bar.
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
        private Color CalculateAlternatingColor(
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
	        // Determine the number of bars to display on the display element
            int barCount = Repeat * Colors.Count();

            // Calculate the bar height
            int barHt = GetFlatBarsBufferHeight() / barCount + 1;

            // Calculate the alternating color for the specified y position
            return GetFlatBarsAlternatingColor(y, frame, heightOfTile, barCount, barHt);
        }

        /// <summary>
        /// Returns a method for calculating the color of a flat bars pixel.
        /// </summary>
        /// <returns>Method to calculate the color of a pixel</returns>
        private CalculatePixelColor GetFlatBarsPixelColorCalculator()
        {
	        CalculatePixelColor calculatePixelColor;

	        // If the direction is one of the alternating ones then...
	        if (Direction == BarDirection.AlternateDown ||
	            Direction == BarDirection.AlternateLeft ||
	            Direction == BarDirection.AlternateRight ||
	            Direction == BarDirection.AlternateUp)
	        {
		        // Return the alternating bars method
		        calculatePixelColor = CalculateAlternatingColor;
	        }
	        else
	        {
		        // Return the moving bar method
		        calculatePixelColor = CalculateMovingColor;
	        }

	        return calculatePixelColor;
        }

        #endregion

        #region Private Common Render Location Methods

        /// <summary>
        /// Renders an expanding or compressing bars.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        /// <param name="downIncreases">True when the zig zag expands on the lower/right side</param>
        /// <param name="heightOfTile">Height of the repeating tile</param>
        /// <param name="widthOfTile">Width of the repeating tile</param>
        /// <param name="yTileStartPosition">Y start position within the repeating tile frame buffer</param>
        /// <param name="angle">Angle of the rotation</param>
        /// <param name="swapXY">True when the effect is using a Horizontal Direction</param>
        /// <param name="convertFromLocationToStringCoordinates">Delegate to convert from location coordinates to string coordinates</param>
        /// <param name="tileFrameBuffer">Tile frame buffer to retrieve colors from</param>
        /// <param name="initializeTileYPosition">Method to determine the initial Y position</param>
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
            InitializeTileYPosition initializeTileYPosition)
        {
            // Calculate the position within the tile based on frame movement
            int movementY = initializeTileYPosition(true, heightOfTile, 0, frame);  

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
                }

                // Determine if we are in the lower half of the display element
                bool down = (y < _length / 2);

                // Calculate the position in the tile for the specified Y coordinate
                int yTile = CalculateZigZagYTilePosition(
                    downIncreases ? down : !down,
                    y,
                    movementY,
                    heightOfTile,
                    _length / 2);

                // Update the x position within the tile
                int repeatingX = x % widthOfTile;

                // If the node is in the lower (right) half of the display element then...
                if (down)
                {
                    // Offset the coordinate and ensure we are still within the tile
                    yTile = (_length / 2 - yTile) % heightOfTile;

                    // Ensure the yTile position is not negative
                    if (yTile < 0)
                    {
	                    yTile += heightOfTile;
                    }
                }

                // If the direction is compressing then...
                if (Direction == BarDirection.Compress ||
                    Direction == BarDirection.HCompress)
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
        /// <param name="barThickness">Thickness of the bar</param>
        /// <param name="leftRight">Whether the zig zag is moving left or right</param>
        /// <param name="movesRight">Whether the zig zag is moving right</param>
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
            int barThickness,
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
                leftRight,
                heightOfTile,
                barThickness,
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

        #region Private Zig Zag Render String Methods

        /// <summary>
        /// Renders a vertical zig zag for string mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectZigZagVertical(int frame, IPixelFrameBuffer frameBuffer)
        {
           // Render the vertical zig zag 
            RenderEffectZigZag(
                frame,
                frameBuffer,
                _heightOfTile,
                _widthOfTile,
                _zigZagThickness + _zigZagSpacing,
                _yTileStartPosition,
                (Direction == BarDirection.Down || Direction == BarDirection.Up),
                (Direction == BarDirection.Down || Direction == BarDirection.AlternateDown),
                _bufferHt,
                _bufferWi,
                SetPixelVertical);
        }

        /// <summary>
        /// Renders a horizontal zig zag for string mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectZigZagHorizontal(int frame, IPixelFrameBuffer frameBuffer)
        {           
            // Render the zig zag
            RenderEffectZigZag(
                frame,
                frameBuffer,
                _heightOfTile,
                _widthOfTile,
                _zigZagThickness + _zigZagSpacing,
                _yTileStartPosition,
                (Direction == BarDirection.Left || Direction == BarDirection.Right),
                (Direction == BarDirection.Left || Direction == BarDirection.AlternateLeft),
                _bufferWi,
                _bufferHt,
                SetPixelHorizontal);
        }

        /// <summary>
        /// Renders a zig zag for string mode.
        /// </summary>             
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        /// <param name="heightOfTile">Height of the repeating tile</param>
        /// <param name="widthOfTile">Width of the repeating tile</param>
        /// <param name="barThickness">Thickness of the bar</param>
        /// <param name="yTileStartPosition">Y start position within the repeating tile</param>
        /// <param name="leftRight">Whether the zig zag is moving left or right</param>
        /// <param name="moveRight">Whether the zig zag is moving right</param>
        /// <param name="yLength">Length of the logical display element</param>
        /// <param name="xLength">Width of the logical display element</param>
        /// <param name="setPixel">Delegate that sets a pixel on the frame buffer</param>
        private void RenderEffectZigZag(
            int frame,
            IPixelFrameBuffer frameBuffer,
            int heightOfTile,
            int widthOfTile,
            int barThickness,
            int yTileStartPosition,
            bool leftRight,
            bool moveRight,
            int yLength,
            int xLength,
            Action<int, IPixelFrameBuffer, int, int, int, int, int> setPixel)
        {
            // Initialize the Y position within the zig zag repeating tile 
            int tileY = InitializeZigZagTileYPosition(
               leftRight,
               heightOfTile, 
               barThickness,
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
                tileY = UpdateZigZagTileYPosition(
                    moveRight, 
                    tileY,
                    heightOfTile);
            }
        }

        /// <summary>
        /// Renders an expanding or compressing zig zag.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        /// <param name="expand">True when the zig zag is expanding</param>        
        /// <param name="heightOfTile">Height of the repeating tile</param>
        /// <param name="widthOfTile">Width of the repeating tile</param>
        /// <param name="yTileStartPosition">Y start position within the repeating tile</param>
        /// <param name="yLength">Height of the logical display element</param>
        /// <param name="xLength">Width of the logical display element</param>
        /// <param name="setPixel">Delegate that sets a pixel on the frame buffer</param>
        private void RenderEffectZigZagExpandCompress(
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
            int tileY = GetZigZagYValue(frame) % heightOfTile;

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
                tileY = UpdateZigZagTileYPosition(!expand, tileY, heightOfTile);
            }

            // Initialize the Y position within the repeating tile
            tileY = GetZigZagYValue(frame) % heightOfTile;

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
                tileY = UpdateZigZagTileYPosition(!expand, tileY, heightOfTile);
            }
        }

        /// <summary>
        /// Renders an expanding horizontal zig zag.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectZigZagHorizontalExpand(int frame, IPixelFrameBuffer frameBuffer)
        {
            // Render the expanding horizontal zig zag bars
            RenderEffectZigZagExpandCompress(
                frame,
                frameBuffer,
                true,
                _heightOfTile,
                _widthOfTile,
                _yTileStartPosition,
                _bufferWi,
                _bufferHt,
                SetPixelHorizontal);
        }

        /// <summary>
        /// Renders a compressing horizontal zig zag.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectZigZagHorizontalCompress(int frame, IPixelFrameBuffer frameBuffer)
        {
            // Render the compressing zig zag bars
            RenderEffectZigZagExpandCompress(
                frame,
                frameBuffer,
                false,
                _heightOfTile,
                _widthOfTile,
                _yTileStartPosition,
                _bufferWi,
                _bufferHt,
                SetPixelHorizontal);
        }

        /// <summary>
        /// Renders an expanding vertical zig zag.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectZigZagVerticalExpand(int frame, IPixelFrameBuffer frameBuffer)
        {
            // Render the expanding verticdal zig zag bars
            RenderEffectZigZagExpandCompress(
                frame,
                frameBuffer,
                true,
                _heightOfTile,
                _widthOfTile,
                _yTileStartPosition,
                _bufferHt,
                _bufferWi,
                SetPixelVertical);
        }

        /// <summary>
        /// Renders a compressing vertical zig zag.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectZigZagVerticalCompress(int frame, IPixelFrameBuffer frameBuffer)
        {           
            // Render the compressing vertical zig zag bars
            RenderEffectZigZagExpandCompress(
                frame,
                frameBuffer,
                false,
                _heightOfTile,
                _widthOfTile,
                _yTileStartPosition,
                _bufferHt,
                _bufferWi,
                SetPixelVertical);
        }
       
        #endregion

        #region Private Zig Zag Render Location Methods

        /// <summary>
        /// Renders a horizontal zig zag for location mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectLocationZigZagHorizontal(int frame, PixelLocationFrameBuffer frameBuffer)
        {
	        // Render the zig zag bars
	        RenderEffectByLocationZigZag(
		        frame,
		        frameBuffer,
		        _heightOfTile,
		        _widthOfTile,
		        _yTileStartPosition,
		        _zigZagThickness,
		        _zigZagSpacing,
		        (Direction == BarDirection.Left || Direction == BarDirection.Right),
		        (Direction == BarDirection.Right || Direction == BarDirection.AlternateRight),
		        true,
		        ConvertFromHorizontalLocationToStringCoordinatesFlip);
        }

        /// <summary>
        /// Renders a vertical zig zag for location mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectLocationZigZagVertical(int frame, PixelLocationFrameBuffer frameBuffer)
        {
	        // Render the zig zag bars
	        RenderEffectByLocationZigZag(
		        frame,
		        frameBuffer,
		        _heightOfTile,
		        _widthOfTile,
		        _yTileStartPosition,
		        _zigZagThickness,
		        _zigZagSpacing,
		        (Direction == BarDirection.Down || Direction == BarDirection.Up),
		        (Direction == BarDirection.Down || Direction == BarDirection.AlternateDown),
		        false,
		        ConvertFromVerticalLocationToStringCoordinates);
        }

        /// <summary>
        /// Renders a vertical zig zag for location mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>       
        /// <param name="heightOfTile">Height of the repeating tile</param>
        /// <param name="widthOfTile">Width of the repeating tile</param>
        /// <param name="yTileStartPosition">Y start position within the repeating tile frame buffer</param>
        /// <param name="zigZagThickness">Thickness of the zig zag bar</param>
        /// <param name="zigZagSpacing">Space between zig zag bars</param>
        /// <param name="leftRight">Whether the zig zag is moving left or right</param>
        /// <param name="movesRight">Whether the zig zag is moving right</param>
        /// <param name="swapXY"></param>
        /// <param name="convertFromLocationToStringCoordinates">Delegate to convert location coordinates to string coordinates</param>
        private void RenderEffectByLocationZigZag(
            int frame,
            PixelLocationFrameBuffer frameBuffer,
            int heightOfTile,
            int widthOfTile,
            int yTileStartPosition,
            int zigZagThickness,
            int zigZagSpacing,
            bool leftRight,
            bool movesRight,
            // ReSharper disable once InconsistentNaming
            bool swapXY,
            ConvertFromLocationToStringCoordinates convertFromLocationToStringCoordinates)
        {
            // Render the zig zag in location mode
	        RenderEffectByLocation(
		        frame,
		        frameBuffer,
		        heightOfTile,
		        widthOfTile,
		        yTileStartPosition,
		        zigZagThickness + zigZagSpacing,
		        leftRight,
		        movesRight,
                swapXY,
		        convertFromLocationToStringCoordinates,
		        _staticZigZagTileFrameBuffer,
		        InitializeZigZagTileYPosition,
		        CalculateMovingColor);
        }

        /// <summary>
        /// Calculates the Y position within the tile for the specified Y coordinate.
        /// </summary>
        /// <param name="incrementing">True when the movement is increasing</param>
        /// <param name="y">String based Y coordinate</param>
        /// <param name="movementY">Movement in the Y axis</param>
        /// <param name="heightOfTile">Height of the repeating zig zag tile</param>
        /// <param name="yOffset"></param>
        /// <returns>The Y coordinate within the repeating tile</returns>
        private int CalculateZigZagYTilePosition(bool incrementing, int y, int movementY, int heightOfTile, int yOffset = 0)
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
                int yTest = y - yOffset - movementY;

                // If the edge of the tile has been reached then...
                if (yTest < 0)
                {
                    // Loop around to the end of the tile
                    yTest += heightOfTile;
                }

                // Ensure the coordinate is within the tile
                yTile = yTest % heightOfTile;
            }

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
        /// Renders an expanding or compressing zig zag.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        /// <param name="downIncreases">True when the zig zag expands on the lower/right side</param>
        /// <param name="heightOfTile">Height of the repeating tile</param>
        /// <param name="widthOfTile">Width of the repeating tile</param>
        /// <param name="yTileStartPosition">Y start position within the repeating tile frame buffer</param>
        /// <param name="angle">Angle of the rotation</param>
        /// <param name="swapXY">True when the effect is using a Horizontal Direction</param>
        /// <param name="convertFromLocationToStringCoordinates">Delegate to convert from location coordinates to string coordinates</param>
        private void RenderEffectLocationZigZagExpandCompress(
            int frame,
            PixelLocationFrameBuffer frameBuffer,
            bool downIncreases,
            int heightOfTile,
            int widthOfTile,
            int yTileStartPosition,
            double angle,
            // ReSharper disable once InconsistentNaming
            bool swapXY,
            ConvertFromLocationToStringCoordinates convertFromLocationToStringCoordinates)
        {
            // Render the expanding or compressing zig zag
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
		        _staticZigZagTileFrameBuffer,
		        InitializeZigZagTileYPosition);
        }

        /// <summary>
        /// Renders a horizontal expanding zig zag for location mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectLocationZigZagHorizontalExpand(int frame, PixelLocationFrameBuffer frameBuffer)
        {
	        // Render the expanding zig zag bars
            RenderEffectLocationZigZagExpandCompress(
                frame,
                frameBuffer,
                true,
                _heightOfTile,
                _widthOfTile,
                _yTileStartPosition,
                -GetRotationAngle(frame),
                true,
                ConvertFromHorizontalLocationToStringCoordinatesFlip);
        }

        /// <summary>
        /// Renders a horizontal compressing zig zag for location mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectLocationZigZagHorizontalCompress(int frame, PixelLocationFrameBuffer frameBuffer)
        {
	        // Render the compressing zig zag bars
	        RenderEffectLocationZigZagExpandCompress(
		        frame,
		        frameBuffer,
		        false,
		        _heightOfTile,
		        _widthOfTile,
		        _yTileStartPosition,
		        -GetRotationAngle(frame),
                true,
		        ConvertFromHorizontalLocationToStringCoordinatesFlip);
        }

        /// <summary>
        /// Renders an expanding zig zag for location mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectLocationZigZagVerticalExpand(int frame, PixelLocationFrameBuffer frameBuffer)
        {
	        // Render the expanding zig zag bars
            RenderEffectLocationZigZagExpandCompress(
	            frame,
	            frameBuffer,
	            true,
	            _heightOfTile,
	            _widthOfTile,
	            _yTileStartPosition,
	            GetRotationAngle(frame),
                false,
	            ConvertFromVerticalLocationToStringCoordinates);
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
        /// Renders a compressing zig zag for location mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectLocationZigZagVerticalCompress(int frame, PixelLocationFrameBuffer frameBuffer)
        {
	        // Render the compressing zig zag bars
	        RenderEffectLocationZigZagExpandCompress(
		        frame,
		        frameBuffer,
		        false,
		        _heightOfTile,
		        _widthOfTile,
		        _yTileStartPosition,
		        GetRotationAngle(frame),
                false,
		        ConvertFromVerticalLocationToStringCoordinates);
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
        /// Calculates the Y value for the zig zag bar based on the frame number.
        /// </summary>
        /// <param name="frame">Current frame number</param>              
        /// <returns>Y position of the zig zag bar</returns>
        private int GetZigZagYValue(int frame)
        {
            // The Y value for the zig zag is a function of the speed and how far the zig zag travels in a 50 ms frame
            int yValue = (int)(frame * (FrameTime / 50.0) * CalculateZigZagSpeed(GetEffectTimeIntervalPosition(frame)));

            // Return the Y value for the zig zag
            return yValue;
        }

        /// <summary>
        /// Get ths speed of the zig zag. 
        /// </summary>
        /// <param name="intervalPos">Interval position within the effect duration</param>            
        /// <returns>Speed of the zig zag</returns>
        private double CalculateZigZagSpeed(double intervalPos)
        {
            // Get the scale value based on the smaller of the width and height of the display element
            int newScaleValue = (int)(_scaleValue * 0.20);

            // Scale the speed based on the calculated scale factor
            return ScaleCurveToValue(SpeedCurve.GetValue(intervalPos), newScaleValue, 0);
        }

        /// <summary>
        /// Gets appropriate scale value for the effect settings.
        /// </summary>
        /// <param name="bufferHt">Height of the dsisplay element</param>
        /// <param name="bufferWi">Width of the display element</param>
        /// <returns>Scale value used to scale effect settings</returns>
        private int GetScaleValue(int bufferHt, int bufferWi)
        {
            // Use the minimum dimension of the display element as the scale factor
            int scaleValue = Math.Min(bufferHt, bufferWi);

            return scaleValue;
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

        #region Private Flat Bar Render Location Methods

        /// <summary>
        /// Renders a horizontal flat bars for location mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectLocationFlatHorizontal(int frame, PixelLocationFrameBuffer frameBuffer)
        {
	        // Render the flat bars
	        RenderEffectByLocationFlatBars(
		        frame,
		        frameBuffer,
		        _heightOfTile,
		        _widthOfTile,
		        _flatBarsBarHeight,
		        (Direction == BarDirection.Left || Direction == BarDirection.Right),
		        (Direction == BarDirection.Right || Direction == BarDirection.AlternateRight),
                true,
		        ConvertFromHorizontalLocationToStringCoordinatesFlip,
		        GetFlatBarsPixelColorCalculator());
        }
        
        /// <summary>
        /// Renders a vertical flat bars for location mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectLocationFlatVertical(int frame, PixelLocationFrameBuffer frameBuffer)
        {
	        // Render the flat bars
	        RenderEffectByLocationFlatBars(
		        frame,
		        frameBuffer,
		        _heightOfTile,
		        _widthOfTile,
		        _flatBarsBarHeight,
		        (Direction == BarDirection.Down || Direction == BarDirection.Up),
		        (Direction == BarDirection.Down || Direction == BarDirection.AlternateDown),
                false,
		        ConvertFromVerticalLocationToStringCoordinates,
		        GetFlatBarsPixelColorCalculator());
        }

        /// <summary>
        /// Renders an expanding or compressing flat bars.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        /// <param name="downIncreases">True when the bar expands on the lower/right side</param>
        /// <param name="heightOfTile">Height of the repeating tile</param>
        /// <param name="widthOfTile">Width of the repeating tile</param>
        /// <param name="swapXY">True when the X and Y coordinates are swapped due to the Direction being horizontal</param>
        /// <param name="convertFromLocationToStringCoordinates">Delegate to convert from location coordinates to string coordinates</param>
        /// <param name="angle">Angle of the bars rotation</param>
        private void RenderEffectLocationFlatExpandCompress(
            int frame,
            PixelLocationFrameBuffer frameBuffer,
            bool downIncreases,
            int heightOfTile,
            int widthOfTile,
            double angle,
            // ReSharper disable once InconsistentNaming
            bool swapXY,
            ConvertFromLocationToStringCoordinates convertFromLocationToStringCoordinates)
        {
            // Render the expanding or compressing flat bars 
	        RenderEffectLocationExpandCompress(
		        frame,
		        frameBuffer,
		        downIncreases,
		        heightOfTile,
		        widthOfTile,
		        0,
		        angle,
                swapXY,
		        convertFromLocationToStringCoordinates,
		        _staticFlatFrameBuffer,
		        InitializeFlatBarTileYPosition);
        }

        /// <summary>
        /// Renders a vertical flat bars for location mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>       
        /// <param name="heightOfTile">Height of the repeating tile</param>
        /// <param name="widthOfTile">Width of the repeating tile</param>
        /// <param name="barThickness">Thickness of the bars</param>
        /// <param name="leftRight">Whether the bars are moving left or right</param>
        /// <param name="movesRight">Whether the bars are moving right</param>
        /// <param name="swapXY">True when the Direction is in one of the horizontal modes</param>
        /// <param name="convertFromLocationToStringCoordinates">Delegate to convert location coordinates to string coordinates</param>
        /// <param name="calculatePixelColor">Delegate that calculates the pixel color for the point</param>
        private void RenderEffectByLocationFlatBars(
	        int frame,
	        PixelLocationFrameBuffer frameBuffer,
	        int heightOfTile,
	        int widthOfTile,
	        int barThickness,
	        bool leftRight,
	        bool movesRight,
	        // ReSharper disable once InconsistentNaming
	        bool swapXY,
	        ConvertFromLocationToStringCoordinates convertFromLocationToStringCoordinates,
	        CalculatePixelColor calculatePixelColor)
        {
	        // Render the bars in location mode
	        RenderEffectByLocation(
		        frame,
		        frameBuffer,
		        heightOfTile,
		        widthOfTile,
		        0,
                barThickness,
		        leftRight,
		        movesRight,
                swapXY,
		        convertFromLocationToStringCoordinates,
		        _staticFlatFrameBuffer,
		        InitializeFlatBarTileYPosition,
		        calculatePixelColor);
        }

        /// <summary>
        /// Renders a horizontal expanding flat bars for location mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectLocationFlatHorizontalExpand(int frame, PixelLocationFrameBuffer frameBuffer)
        {
	        // Render the expanding flat bars
	        RenderEffectLocationFlatExpandCompress(
		        frame,
		        frameBuffer,
		        true,
		        _heightOfTile,
		        _widthOfTile,
		        -GetRotationAngle(frame),
                true,
		        ConvertFromHorizontalLocationToStringCoordinatesFlip);
        }

        /// <summary>
        /// Renders an expanding or compressing flat bars for location mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectLocationFlatVerticalExpand(int frame, PixelLocationFrameBuffer frameBuffer)
        {
	        // Render the expanding or compressing flat bars
	        RenderEffectLocationFlatExpandCompress(
		        frame,
		        frameBuffer,
		        true,
		        _heightOfTile,
		        _widthOfTile,
		        GetRotationAngle(frame),
                false,
		        ConvertFromVerticalLocationToStringCoordinates);
        }

        /// <summary>
        /// Renders a compressing flat bars for location mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectLocationFlatVerticalCompress(int frame, PixelLocationFrameBuffer frameBuffer)
        {
	        // Render the compressing flat bars
	        RenderEffectLocationFlatExpandCompress(
		        frame,
		        frameBuffer,
		        false,
		        _heightOfTile,
		        _widthOfTile,
		        GetRotationAngle(frame),
                false,
		        ConvertFromVerticalLocationToStringCoordinates);
        }

        /// <summary>
        /// Renders a horizontal compressing flat bars for location mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        private void RenderEffectLocationFlatHorizontalCompress(int frame, PixelLocationFrameBuffer frameBuffer)
        {
	        // Render the compressing flat bars
	        RenderEffectLocationFlatExpandCompress(
		        frame,
		        frameBuffer,
		        false,
		        _heightOfTile,
		        _widthOfTile,
		        -GetRotationAngle(frame),
                true,
		        ConvertFromHorizontalLocationToStringCoordinatesFlip);
        }
        #endregion

        #region Private Flat Bar Render String Methods

        private void RenderEffectFlat(int frame, IPixelFrameBuffer frameBuffer)
        {
            int x, y, n, colorIdx;
            int colorcnt = Colors.Count();
            int barCount = Repeat * colorcnt;
            double intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;

            _negPosition = false;

            if (MovementType == MovementType.Iterations)
            {
                _position = (GetEffectTimeIntervalPosition(frame) * Speed) % 1;
            }
            else
            {
                if (frame == 0) _position = CalculateFlatBarSpeed(intervalPosFactor);
                _position += CalculateFlatBarSpeed(intervalPosFactor) / 100;
                if (_position < 0)
                {
                    _negPosition = true;
                    _position = -_position;
                }
            }

            if (barCount < 1) barCount = 1;
            double level = LevelCurve.GetValue(intervalPosFactor) / 100;

            if (Direction < BarDirection.Left || Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown)
            {
                int barHt = BufferHt / barCount + 1;
                if (barHt < 1) barHt = 1;
                int halfHt = _bufferHt / 2;
                int blockHt = colorcnt * barHt;
                if (blockHt < 1) blockHt = 1;
                int fOffset = (int)(_position * blockHt * Repeat);// : Speed * frame / 4 % blockHt);
                if (Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown)
                {
                    fOffset = (int)(Math.Floor(_position * barCount) * barHt);
                }

                var indexAdjust = 1;

                for (y = 0; y < _bufferHt; y++)
                {
                    n = y + fOffset;
                    colorIdx = ((n + indexAdjust) % blockHt) / barHt;
                    //we need the integer division here to make things work
                    double colorPosition = (n + indexAdjust) % barHt / (double)barHt;
                    Color c = Colors[colorIdx].GetColorAt(colorPosition);

                    if (Highlight || Show3D)
                    {
                        var hsv = HSV.FromRGB(c);
                        if (Highlight && (n + indexAdjust) % barHt == 0) hsv.S = 0.0f;
                        if (Show3D) hsv.V *= (float)(barHt - (n + indexAdjust) % barHt - 1) / barHt;
                        hsv.V *= level;
                        c = hsv.ToRGB();
                    }
                    else
                    {
                        if (level < 1)
                        {
                            HSV hsv = HSV.FromRGB(c);
                            hsv.V *= level;
                            c = hsv.ToRGB();
                        }
                    }

                    switch (Direction)
                    {
                        case BarDirection.Down:
                        case BarDirection.AlternateDown:
                            // dow
                            if (_negPosition)
                            {
                                for (x = 0; x < _bufferWi; x++)
                                {
                                    frameBuffer.SetPixel(x, _bufferHt - y - 1, c);
                                }
                            }
                            else
                            {
                                for (x = 0; x < _bufferWi; x++)
                                {
                                    frameBuffer.SetPixel(x, y, c);
                                }
                            }

                            break;
                        case BarDirection.Compress:
                            // expand
                            if (_negPosition)
                            {
                                if (y <= halfHt)
                                {
                                    for (x = 0; x < _bufferWi; x++)
                                    {
                                        frameBuffer.SetPixel(x, y, c);
                                        frameBuffer.SetPixel(x, _bufferHt - y - 1, c);
                                    }
                                }
                            }
                            else
                            {
                                if (y >= halfHt)
                                {
                                    for (x = 0; x < _bufferWi; x++)
                                    {
                                        frameBuffer.SetPixel(x, y, c);
                                        frameBuffer.SetPixel(x, _bufferHt - y - 1, c);
                                    }
                                }
                            }
                            break;
                        case BarDirection.Expand:
                            // compress
                            if (!_negPosition)
                            {
                                if (y <= halfHt)
                                {
                                    for (x = 0; x < _bufferWi; x++)
                                    {
                                        frameBuffer.SetPixel(x, y, c);
                                        frameBuffer.SetPixel(x, _bufferHt - y - 1, c);
                                    }
                                }
                            }
                            else
                            {
                                if (y >= halfHt)
                                {
                                    for (x = 0; x < _bufferWi; x++)
                                    {
                                        frameBuffer.SetPixel(x, y, c);
                                        frameBuffer.SetPixel(x, _bufferHt - y - 1, c);
                                    }
                                }
                            }
                            break;
                        default:
                            // up & AlternateUp
                            if (!_negPosition)
                            {
                                for (x = 0; x < _bufferWi; x++)
                                {
                                    frameBuffer.SetPixel(x, _bufferHt - y - 1, c);
                                }
                            }
                            else
                            {
                                for (x = 0; x < _bufferWi; x++)
                                {
                                    frameBuffer.SetPixel(x, y, c);
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                int barWi = BufferWi / barCount + 1;
                if (barWi < 1) barWi = 1;
                int halfWi = _bufferWi / 2;
                int blockWi = colorcnt * barWi;
                if (blockWi < 1) blockWi = 1;
                int fOffset = (int)(_position * blockWi * Repeat);
                if (Direction > BarDirection.AlternateDown)
                {
                    fOffset = (int)(Math.Floor(_position * barCount) * barWi);
                }

                var indexAdjust = 1;

                for (x = 0; x < _bufferWi; x++)
                {
                    n = x + fOffset;
                    //we need the integer division here to make things work
                    colorIdx = ((n + 1) % blockWi) / barWi;
                    var colorPosition = (n + 1) % barWi / (double)barWi;
                    Color c = Colors[colorIdx].GetColorAt(colorPosition);

                    if (Highlight || Show3D)
                    {
                        var hsv = HSV.FromRGB(c);
                        if (Highlight && (n + 1) % barWi == 0) hsv.S = 0.0f;
                        if (Show3D) hsv.V *= (float)(barWi - (n + indexAdjust) % barWi - 1) / barWi;
                        hsv.V *= level;
                        c = hsv.ToRGB();
                    }
                    else
                    {
                        if (level < 1)
                        {
                            HSV hsv = HSV.FromRGB(c);
                            hsv.V *= level;
                            c = hsv.ToRGB();
                        }
                    }

                    switch (Direction)
                    {
                        case BarDirection.Right:
                        case BarDirection.AlternateRight:
                            // right
                            for (y = 0; y < _bufferHt; y++)
                            {
                                frameBuffer.SetPixel(_bufferWi - x - 1, y, c);
                            }
                            break;
                        case BarDirection.HExpand:
                            // H-expand
                            if (x <= halfWi)
                            {
                                for (y = 0; y < _bufferHt; y++)
                                {
                                    frameBuffer.SetPixel(x, y, c);
                                    frameBuffer.SetPixel(_bufferWi - x - 1, y, c);
                                }
                            }
                            break;
                        case BarDirection.HCompress:
                            // H-compress
                            if (x >= halfWi)
                            {
                                for (y = 0; y < _bufferHt; y++)
                                {
                                    frameBuffer.SetPixel(x, y, c);
                                    frameBuffer.SetPixel(_bufferWi - x - 1, y, c);
                                }
                            }
                            break;
                        default:
                            // left & AlternateLeft
                            for (y = 0; y < _bufferHt; y++)
                            {
                                frameBuffer.SetPixel(x, y, c);
                            }
                            break;
                    }
                }
            }
        }

        #endregion       
    }
}
