using Common.Controls.ColorManagement.ColorModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using ZedGraph;

namespace VixenModules.Effect.Bars
{
    public class Bars : PixelEffectBase
    {
        #region Private Fields

        private BarsData _data;
        private double _position;
        private bool _negPosition;

        /// <summary>
        /// This frame buffer contains a tile of the zig zags.
        /// This tile can be repeated to fill the dispay element.
        /// </summary>
        IPixelFrameBuffer _staticZigZagTileFrameBuffer;

        /// <summary>
        /// The height of the zig zag tile frame buffer.       
        /// </summary>
        int _staticZigZagTileFrameBufferHeight;

        /// <summary>
        /// Height of the zig zag repeating tile.
        /// Note thie frame buffer associated with this repeating tile is actually larger because the first part
        /// of the buffer contains a subset of the zig zag colors (ie it is incomplete). 
        /// </summary>
        int _heightOfTile;

        /// <summary>
        /// Width of the zig zag repeating tile.
        /// </summary>
        int _widthOfTile;

        /// <summary>
        /// Start position or offset into the tile frame buffer.  This offset accounts for the first part of the
        /// frame buffer being incomplete.
        /// </summary>
        int _yTileStartPosition;

        /// <summary>
        /// Zig zag thickness in pixels.
        /// </summary>
        int _zigZagThickness;

        /// <summary>
        /// Spacing between the zig zags in pixels.
        /// </summary>
        int _zigZagSpacing;

        /// <summary>
        /// Amplitude of the zig zag in pixels.
        /// </summary>
        int _zigZagAmplitude;

        /// <summary>
        /// Period of the zig zag in pixels.
        /// </summary>
        int _zigZagPeriod;

        /// <summary>
        /// Scale factor used on all the zig zag settings.
        /// </summary>
        int _scaleValue;

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
        [ProviderDisplayName(@"MovementType")]
        [ProviderDescription(@"MovementType")]
        [PropertyOrder(2)]
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
        [PropertyOrder(3)]
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
        [ProviderDisplayName(@"Repeat")]
        [ProviderDescription(@"Repeat")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(1, 10, 1)]
        [PropertyOrder(5)]
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
        [PropertyOrder(6)]
        public bool Highlight
        {
            get { return _data.Highlight; }
            set
            {
                _data.Highlight = value;
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

        protected override EffectTypeModuleData EffectModuleData
        {
            get { return _data; }
        }

        #endregion

        #region Protected Methods

        protected override void CleanUpRender()
        {
            // Nothing to clean up
        }

        #endregion

        #region Private Methods

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
            // Get the thickness of the zig zag bar as a perecentage of the width or height
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
            // The color gradients run perpendicular to the bars or zig zags so the highlight always
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
            Color color = GetZigZagColor(colorIndex, currentThicknessCounter);

            // Calculate the increment of the zig zag (this value affects the slope of the zig zag)
            double dZigZagIncrement = (double)(_zigZagAmplitude - 1) / (double)(_zigZagPeriod - 1);

            // When the zig zag amplitidue is set to zero the increment will go negative
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
                        color = GetZigZagColor(colorIndex, currentThicknessCounter);
                    }
                }
                // Otherwise if we are not processing a blank zig zag bar then...
                else if (color != Color.Transparent)
                {
                    // Get the color for the zig zag bar
                    color = GetZigZagColor(colorIndex, currentThicknessCounter);
                }
            }
        }

        /// <summary>
        /// Gets the color for the zig zag bar taking into account the specified pixel position within the bar.
        /// </summary>
        /// <param name="colorIndex">Index of the current color</param>       
        /// <param name="currentThicknessCounter">Pixel position within the bar</param>
        /// <returns>Gets the color for the specified zig zag pixel</returns>
        private Color GetZigZagColor(int colorIndex, int currentThicknessCounter)
        {
            Color color;

            // If we are at the beginning of a zig zag and
            // highlight is selected then...
            if (currentThicknessCounter == 0 &&
                Highlight)
            {
                // Set the color to the highlight color
                color = GetHighlightColor(colorIndex);
            }
            // Otherwise if the zig zag bar is 3-D then...
            else if (Show3D)
            {
                // Set the color to the 3-D color
                color = Get3DColor(colorIndex, currentThicknessCounter);
            }
            else
            {
                // Default to the gradient position based on the position in the wave thickness
                color = Colors[colorIndex].GetColorAt(currentThicknessCounter / _zigZagThickness);
            }

            // Return the color of the zig zag
            return color;
        }

        /// <summary>
        /// Gets the 3-D color for the specified zig zag pixel.
        /// </summary>
        /// <param name="colorIndex">Index into the color array</param>
        /// <param name="currentWidthCounter">Zig zag pixel being drawn</param>
        /// <returns>3-D color for the specified zig zag pixel</returns>
        private Color Get3DColor(int colorIndex, int currentThicknessCounter)
        {
            // Get the specified color from the color array
            Color color = Colors[colorIndex].GetColorAt(0);

            // Convert from RGB to HSV color format 
            HSV hsv = HSV.FromRGB(color);

            // Set the brightness based on the percentage of the bar thickness
            hsv.V *= (float)(_zigZagThickness - currentThicknessCounter) / _zigZagThickness;

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
        /// Returns the start index or position within the static zig zag tile.  The
        /// first part of the tile is incomplete because we start with one color
        /// and draw up.
        /// </summary>       
        /// <returns>Y start position within the repeating tile</returns>
        private int GetTileYStartPosition()
        {
            // Skip the first 
            return (_zigZagAmplitude + _zigZagThickness - 1 + _zigZagSpacing) + (Colors.Count - 1) * (_zigZagThickness + _zigZagSpacing);
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
                _zigZagThickness,
                _zigZagSpacing,
                _yTileStartPosition,
                (Direction == BarDirection.Down || Direction == BarDirection.Up),
                (Direction == BarDirection.Down || Direction == BarDirection.AlternateDown),
                BufferHt,
                BufferWi,
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
                _zigZagThickness,
                _zigZagSpacing,
                _yTileStartPosition,
                (Direction == BarDirection.Left || Direction == BarDirection.Right),
                (Direction == BarDirection.Right || Direction == BarDirection.AlternateRight),
                BufferWi,
                BufferHt,
                SetPixelHorizontal);
        }

        /// <summary>
        /// Renders a zig zag for string mode.
        /// </summary>             
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        /// <param name="heightOfTile">Height of the repeating tile</param>
        /// <param name="widthOfTile">Width of the repeating tile</param>
        /// <param name="zigZagThickness">Thickness of the zig zag</param>
        /// <param name="zigZagSpacing">Space between the zig zags</param>
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
            int zigZagThickness,
            int zigZagSpacing,
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
               zigZagThickness,
               zigZagSpacing,
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
        /// Initializes the Y position within the zig zag repeating tile.
        /// </summary>
        /// <param name="increment">True when incrementing within the tile</param>
        /// <param name="heightOfTile">Height of the tile</param>
        /// <param name="zigZagThickness">Thickness of the zig zag</param>
        /// <param name="zigZagSpacing">Spacing between the zig zags</param>
        /// <param name="frame">Current frame number</param>
        /// <returns>Initial Y position within the repeating tile</returns>
        private int InitializeZigZagTileYPosition(
            bool increment, 
            int heightOfTile, 
            int zigZagThickness, 
            int zigZagSpacing, 
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
                tileY = (int)(Math.Floor(_position * Colors.Count) * (zigZagThickness + zigZagSpacing));
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
        /// Sets the specified horizonatl pixel on the frame buffer from the specified pixel from the repeating tile.
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
                BufferWi,
                BufferHt,
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
                BufferWi,
                BufferHt,
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
                BufferHt,
                BufferWi,
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
                BufferHt,
                BufferWi,
                SetPixelVertical);
        }
       
        #endregion

        #region Private Zig Zag Render Location Methods

        /// <summary>
        /// Renders a horizonal zig zag for location mode.
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
                ConvertFromHorizontalLocationToStringCoordinatesFlip,
                SetPixelHorizontal);
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
                ConvertFromVerticalLocationToStringCoordinates,
                SetPixelVertical);
        }

        /// <summary>
        /// Defines a delegate for converting from location coordinates to string coordinates.
        /// </summary>
        /// <param name="x">X location coordinate</param>
        /// <param name="y">Y location coordinate</param>
        /// <param name="xOut">X string coordinate</param>
        /// <param name="yOut">Y string coordinate</param>
        delegate void ConvertFromLocationToStringCoordinates(int x, int y, out int xOut, out int yOut);

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
        /// <param name="convertFromLocationToStringCoordinates">Delete to convert location coordinates to string coordinates</param>
        /// <param name="setPixel">Delegate that sets a pixel on the frame buffer</param>
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
            ConvertFromLocationToStringCoordinates convertFromLocationToStringCoordinates,
            Action<int, IPixelFrameBuffer, int, int, int, int, int> setPixel)
        {
            // Initialize the Y position within the zig zag repeating tile 
            int movementY = InitializeZigZagTileYPosition(
                leftRight,
                heightOfTile,
                zigZagThickness,
                zigZagSpacing,
                frame);
                             
            // Loop over the location nodes
            foreach (ElementLocation node in frameBuffer.ElementLocations)
            {
                // Convert from location based coordinate to string coordinate
                int x;
                int y;
                convertFromLocationToStringCoordinates(node.X, node.Y, out x, out y);

                // Calculate the position in the tile for the specified Y coordinate
                int yTile = CalculateZigZagYTilePosition(
                    movesRight,
                    y,
                    movementY,
                    heightOfTile);

                // Update the x position within the tile
                int xTile = x % widthOfTile;

                // Transfer a pixel from the tile to the frame buffer          
                frameBuffer.SetPixel(node.X, node.Y, AdjustIntensity(_staticZigZagTileFrameBuffer.GetColorAt(xTile, yTile + yTileStartPosition), frame));
            }
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
            y = Math.Abs((BufferWiOffset - y) + (BufferWi - 1 + BufferWiOffset));
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
            y = Math.Abs((BufferHtOffset - y) + (BufferHt - 1 + BufferHtOffset));
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
        /// <param name="yLength">Logical length of the display element</param>
        /// <param name="convertFromLocationToStringCoordinates">Delete to convert from location coordinates to string coordinates</param>
        private void RenderEffectLocationZigZagExpandCompress(
            int frame,
            PixelLocationFrameBuffer frameBuffer,
            bool downIncreases,
            int heightOfTile,
            int widthOfTile,
            int yTileStartPosition,
            int yLength,  
            ConvertFromLocationToStringCoordinates convertFromLocationToStringCoordinates)
        {
            // Calculate the position within the tile based on frame movement
            int movementY = GetZigZagYValue(frame) % heightOfTile;

            // Loop over the location nodes
            foreach (ElementLocation node in frameBuffer.ElementLocations)
            {
                // Convert from location based coordinate to string coordinate
                int x;
                int y;
                convertFromLocationToStringCoordinates(node.X, node.Y, out x, out y);

                // Determine if we are in the lower half of the display element
                bool down = (y < yLength / 2);

                // Calculate the position in the tile for the specified Y coordinate
                int yTile = CalculateZigZagYTilePosition(
                    downIncreases ? down : !down,
                    y,
                    movementY,
                    heightOfTile,
                    yLength / 2);

                // Update the x position within the tile
                int repeatingX = x % widthOfTile;

                // If the node is in the lower (right) half of the display element then...
                if (down)
                {
                    // Offset the coordinate and ensure we are still within the tile
                    yTile = (yLength / 2 - yTile) % heightOfTile;
                }

                // Transfer a pixel from the tile to the frame buffer
                frameBuffer.SetPixel(node.X, node.Y, AdjustIntensity(_staticZigZagTileFrameBuffer.GetColorAt(repeatingX, yTile + yTileStartPosition), frame));
            }
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
                BufferWi,
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
                BufferWi,
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
                BufferHt,
                ConvertFromVerticalLocationToStringCoordinates);
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
               BufferHt,       
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

            /*
            // Determine the scale value based on the Direction of the bars
            switch (Direction)
            {
                case BarDirection.Up:
                case BarDirection.Down:
                case BarDirection.AlternateUp:
                case BarDirection.AlternateDown:
                case BarDirection.Expand:
                case BarDirection.Compress:
                    scaleValue = bufferHt;
                    break;
                case BarDirection.Left:
                case BarDirection.Right:
                case BarDirection.AlternateLeft:
                case BarDirection.AlternateRight:
                case BarDirection.HExpand:
                case BarDirection.HCompress:
                    scaleValue = bufferWi;
                    break;
                default:
                    scaleValue = bufferHt;
                    Debug.Assert(false, "Unsupported Direction!");
                    break;
            }
            */
          
            return scaleValue;
        }

        #endregion

        #region Protected Methods

        protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
        {
            // If the bar type is flat then...
            if (BarType == BarType.Flat)
            {
                RenderEffectByLocationFlat(numFrames, frameBuffer);
            }
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
                _yTileStartPosition = GetTileYStartPosition();

                // Calculate the height of the repeating tile frame buffer
                _staticZigZagTileFrameBufferHeight = _heightOfTile + _yTileStartPosition;

                // Initialize the repeating tile frame buffer
                _staticZigZagTileFrameBuffer = new PixelFrameBuffer(_widthOfTile, _staticZigZagTileFrameBufferHeight);
                   
                // Draw the repeating zig zag tile
                SetupRenderZigZag();
            }
		}

        /// <summary>
        /// Renders the effect in string mode.
        /// </summary>
        /// <param name="frame">Current frame number</param>
        /// <param name="frameBuffer">Frame buffer to render in</param>
        protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
        {
            // If the bar type is flat then...
            if (BarType == BarType.Flat)
            {
                RenderEffectFlat(frame, frameBuffer);
            }
            else
            {
                RenderEffectStringsZigZag(frame, frameBuffer);
            }
        }

        #endregion
                                   
        #region Private Solid Bar Methods

        private void RenderEffectByLocationFlat(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			int colorcnt = Colors.Count();
			int barCount = Repeat * colorcnt;
			if (barCount < 1) barCount = 1;
			
			int barHt = BufferHt / barCount + 1;
			if (barHt < 1) barHt = 1;
			int blockHt = colorcnt * barHt;
			if (blockHt < 1) blockHt = 1;

			int barWi = BufferWi / barCount + 1;
			if (barWi < 1) barWi = 1;
			int blockWi = colorcnt * barWi;
			if (blockWi < 1) blockWi = 1;

			var bufferHt = BufferHt;
			var bufferWi = BufferWi;
			var bufferHtOffset = BufferHtOffset;
			var bufferWiOffset = BufferWiOffset;

			IEnumerable<IGrouping<int, ElementLocation>> nodes;
			List<IGrouping<int, ElementLocation>> reversedNodes = new List<IGrouping<int, ElementLocation>>();
			
			switch (Direction)
			{
				case BarDirection.AlternateUp:
				case BarDirection.Up:
					nodes = frameBuffer.ElementLocations.OrderBy(x => x.Y).ThenBy(x => x.X).GroupBy(x => x.Y);
					break;
				case BarDirection.Left:
				case BarDirection.AlternateLeft:
					nodes = frameBuffer.ElementLocations.OrderByDescending(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
					break;
				case BarDirection.Right:
				case BarDirection.AlternateRight:
					nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
					break;
				case BarDirection.Compress:
				case BarDirection.Expand:
					nodes = frameBuffer.ElementLocations.OrderByDescending(x => x.Y).ThenBy(x => x.X).GroupBy(x => x.Y);
					reversedNodes = nodes.Reverse().ToList();
					break;
				case BarDirection.HCompress:
				case BarDirection.HExpand:
					nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
					reversedNodes = nodes.Reverse().ToList();
					break;
				default:
					nodes = frameBuffer.ElementLocations.OrderByDescending(x => x.Y).ThenBy(x => x.X).GroupBy(x => x.Y);
					break;

			}
			var nodeCount = nodes.Count();
			var halfNodeCount = (nodeCount - 1) / 2;
			var evenHalfCount = nodeCount%2!=0;
			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;
				double intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;
				double level = LevelCurve.GetValue(intervalPosFactor) / 100;
				
				if (MovementType == MovementType.Iterations)
				{
					_position = (GetEffectTimeIntervalPosition(frame) * Speed) % 1;
				}
				else
				{
					if (frame == 0) _position = CalculateSpeed(intervalPosFactor);
					_position += CalculateSpeed(intervalPosFactor) / 100;
				}

				int n;
				int colorIdx;
				if (Direction < BarDirection.Left || Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown)
				{
					
					int fOffset = (int)(_position * blockHt * Repeat);// : Speed * frame / 4 % blockHt);
					if (Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown)
					{
						fOffset = (int)(Math.Floor(_position * barCount) * barHt);
					}
					
					int indexAdjust = 1;

					int i = 0;
					bool exitLoop = false;
					foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
					{
						int y = elementLocations.Key;

						switch (Direction)
						{
							case BarDirection.Down:
							case BarDirection.AlternateDown:
							case BarDirection.Expand:
								n = (bufferHt+bufferHtOffset) - y + fOffset;
								break;
							default:
								n =  y - bufferHtOffset + fOffset;
								break;
						}
						
						colorIdx = ((n + indexAdjust) % blockHt) / barHt;
						//we need the integer division here to make things work
						var colorPosition =(n + indexAdjust) % barHt / (double)barHt;
						Color c = Colors[colorIdx].GetColorAt(colorPosition); 
						
						if (Highlight || Show3D)
						{
							var hsv = HSV.FromRGB(c);
							if (Highlight && (n + indexAdjust) % barHt == 0 || colorPosition > .95) hsv.S = 0.0f;
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
							case BarDirection.Expand:
							case BarDirection.Compress:
								// expand / compress
								if (i <= halfNodeCount)
								{
									foreach (var elementLocation in elementLocations)
									{
										frameBuffer.SetPixel(elementLocation.X, y, c);
									}
									if (i == halfNodeCount & evenHalfCount)
									{
										i++;
										continue;
									}
									foreach (var elementLocation in reversedNodes[i])
									{
										frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, c);
									}

									i++;
								}
								else
								{
									exitLoop = true;
								}
								break;
							default:
								foreach (var elementLocation in elementLocations)
								{
									frameBuffer.SetPixel(elementLocation.X, y, c);
								}
								break;
						}
						if (exitLoop) break;
					}
				}
				else
				{
					
					int fOffset = (int)(_position * blockWi * Repeat);
					if (Direction > BarDirection.AlternateDown)
					{
						fOffset = (int)(Math.Floor(_position * barCount) * barWi);
					}
					
					int i = 0;
					
					foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
					{
						int x = elementLocations.Key;
						
						switch (Direction)
						{
							case BarDirection.Right:
							case BarDirection.AlternateRight:
								case BarDirection.HCompress:
								n = (bufferWi+bufferWiOffset) - x + fOffset;
								break;
							default:
								n = x - bufferWiOffset + fOffset;
								break;
						}
						
						//we need the integer division here to make things work
						colorIdx = (n + 1) % blockWi / barWi;
						double colorPosition = (n + 1) % barWi / (double)barWi;
						Color c = Colors[colorIdx].GetColorAt(colorPosition);
						
						if (Highlight || Show3D)
						{
							var hsv = HSV.FromRGB(c);
							if (Highlight && (n+1) % barWi == 0 || colorPosition > .95) hsv.S = 0.0f;
							if (Show3D) hsv.V *= (float)(barWi - n % barWi - 1) / barWi;
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
							case BarDirection.HExpand:
							case BarDirection.HCompress:
								if (i <= halfNodeCount)
								{
									foreach (var elementLocation in elementLocations)
									{
										frameBuffer.SetPixel(x, elementLocation.Y, c);
									}
									if (i == halfNodeCount & evenHalfCount)
									{
										i++;
										continue;
									}
									foreach (var elementLocation in reversedNodes[i])
									{
										frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, c);
									}

									i++;
								}
								break;
							default:
								foreach (var elementLocation in elementLocations)
								{
									frameBuffer.SetPixel(x, elementLocation.Y, c);
								}
								break;
						}
					}

				}

			}
		}

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
                if (frame == 0) _position = CalculateSpeed(intervalPosFactor);
                _position += CalculateSpeed(intervalPosFactor) / 100;
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
                int halfHt = BufferHt / 2;
                int blockHt = colorcnt * barHt;
                if (blockHt < 1) blockHt = 1;
                int fOffset = (int)(_position * blockHt * Repeat);// : Speed * frame / 4 % blockHt);
                if (Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown)
                {
                    fOffset = (int)(Math.Floor(_position * barCount) * barHt);
                }

                var indexAdjust = 1;

                for (y = 0; y < BufferHt; y++)
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
                                for (x = 0; x < BufferWi; x++)
                                {
                                    frameBuffer.SetPixel(x, BufferHt - y - 1, c);
                                }
                            }
                            else
                            {
                                for (x = 0; x < BufferWi; x++)
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
                                    for (x = 0; x < BufferWi; x++)
                                    {
                                        frameBuffer.SetPixel(x, y, c);
                                        frameBuffer.SetPixel(x, BufferHt - y - 1, c);
                                    }
                                }
                            }
                            else
                            {
                                if (y >= halfHt)
                                {
                                    for (x = 0; x < BufferWi; x++)
                                    {
                                        frameBuffer.SetPixel(x, y, c);
                                        frameBuffer.SetPixel(x, BufferHt - y - 1, c);
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
                                    for (x = 0; x < BufferWi; x++)
                                    {
                                        frameBuffer.SetPixel(x, y, c);
                                        frameBuffer.SetPixel(x, BufferHt - y - 1, c);
                                    }
                                }
                            }
                            else
                            {
                                if (y >= halfHt)
                                {
                                    for (x = 0; x < BufferWi; x++)
                                    {
                                        frameBuffer.SetPixel(x, y, c);
                                        frameBuffer.SetPixel(x, BufferHt - y - 1, c);
                                    }
                                }
                            }
                            break;
                        default:
                            // up & AlternateUp
                            if (!_negPosition)
                            {
                                for (x = 0; x < BufferWi; x++)
                                {
                                    frameBuffer.SetPixel(x, BufferHt - y - 1, c);
                                }
                            }
                            else
                            {
                                for (x = 0; x < BufferWi; x++)
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
                int halfWi = BufferWi / 2;
                int blockWi = colorcnt * barWi;
                if (blockWi < 1) blockWi = 1;
                int fOffset = (int)(_position * blockWi * Repeat);
                if (Direction > BarDirection.AlternateDown)
                {
                    fOffset = (int)(Math.Floor(_position * barCount) * barWi);
                }

                for (x = 0; x < BufferWi; x++)
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
                        if (Show3D) hsv.V *= (float)(barWi - n % barWi - 1) / barWi;
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
                            for (y = 0; y < BufferHt; y++)
                            {
                                frameBuffer.SetPixel(BufferWi - x - 1, y, c);
                            }
                            break;
                        case BarDirection.HExpand:
                            // H-expand
                            if (x <= halfWi)
                            {
                                for (y = 0; y < BufferHt; y++)
                                {
                                    frameBuffer.SetPixel(x, y, c);
                                    frameBuffer.SetPixel(BufferWi - x - 1, y, c);
                                }
                            }
                            break;
                        case BarDirection.HCompress:
                            // H-compress
                            if (x >= halfWi)
                            {
                                for (y = 0; y < BufferHt; y++)
                                {
                                    frameBuffer.SetPixel(x, y, c);
                                    frameBuffer.SetPixel(BufferWi - x - 1, y, c);
                                }
                            }
                            break;
                        default:
                            // left & AlternateLeft
                            for (y = 0; y < BufferHt; y++)
                            {
                                frameBuffer.SetPixel(x, y, c);
                            }
                            break;
                    }
                }
            }
        }

        private double CalculateSpeed(double intervalPos)
        {
            return ScaleCurveToValue(SpeedCurve.GetValue(intervalPos), 15, -15);
        }

        #endregion       
    }
}
