namespace VixenModules.Preview.VixenPreview.Fixtures.Geometry
{
	/// <summary>
	/// Maintains geometry constants for drawing moving head fixtures.
	/// </summary>
	class MovingHeadGeometryConstants : IMovingHeadGeometryConstants
	{
		#region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="length">Wdith and height of the drawing area</param>
		public MovingHeadGeometryConstants(double length)
        {
            // Store off the length of the drawing area
            _length = length;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Width and height of the drawing area.
        /// </summary>
        private double _length;

        #endregion

        #region Private Constants

        /// <summary>
        /// Defines the width and depth of the base as a percentage of the drawing area.
        /// </summary>
        private double BaseWidthAndDepthPercentage = 0.45;

        /// <summary>
        /// Defines the height of the base as a percentage of the drawing area.
        /// </summary>
        private double BaseHeightPercentage = 0.07;

        /// <summary>
        /// Defines the bottom of the drawing area.
        /// </summary>
        private double BottomOfViewPort = 0.5;

        /// <summary>
        /// Defines the width of the side supports as a percentage of the drawing area.
        /// </summary>
        private double SupportWidth = 0.05;

        /// <summary>
        /// Defines the height of the side supports as a percentage of the drawing area.
        /// </summary>
        private double SupportHeight = 0.3;

        /// <summary>
        /// Defines the bottom depth of the vertical support as a percentage of the drawing area.
        /// </summary>
        private double SupportBottomDepth = 0.2;

        /// <summary>
        /// Defines the top depth of the vertical support as a percentage of the drawing area.
        /// </summary>
        private double SupportTopDepth = 0.1;

        /// <summary>
        /// Defines the radius of the light housing as a percentage of the drawing area.
        /// </summary>
        private double LightHousingRadius = 0.18;

        #endregion

        #region IMovingHeadGeometryConstants

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetBottomOfViewport()
        {
            return _length * BottomOfViewPort;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetBaseYPosition()
        {
            // Bottom of drawing area, backing off to leave space for the legend
	        return GetBottomOfViewport() - GetBaseHeight();
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetBaseDepth()
        {
	        return _length * BaseWidthAndDepthPercentage;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetBaseWidth()
        {
            return _length * BaseWidthAndDepthPercentage;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetBaseHeight()
        {
            return _length * BaseHeightPercentage;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetSupportWidth()
        {
	        return _length * SupportWidth;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetSupportHeight()
        {
            return _length * SupportHeight;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetSupportBaseDepth()
        {
            return _length * SupportBottomDepth;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetSupportTopDepth()
        {
            return _length * SupportTopDepth;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetSupportYPosition(double orientationSign)
        {
	        double supportYOffset = GetBottomOfViewport();
            supportYOffset -= 2 * GetBaseHeight();
            supportYOffset -= GetSupportHeight();

            return -orientationSign * supportYOffset;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetLightHousingRadius()
        {
            return _length * LightHousingRadius;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetSupportXOffset()
        {
            return GetLightHousingRadius() * 1.4;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetLightHousingLength()
        {
	        return 2.0 * _length * 0.3;	    
        }

        
        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetLightHousingYPosition(double orientationSign)
        {
	        return GetHorizontalCylinderYPosition(orientationSign) -
	               orientationSign * GetLightHousingLength() * GetLightHousingPercentageBelowHorizontalSupport();
        }
        
        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetLightHousingPercentageBelowHorizontalSupport()
        {
            return 0.33;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetLightHousingPercentageAboveHorizontalSupport()
        {
            return 0.67;            
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetHorizontalCylinderRadius()
        {
            return _length * 0.07;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetLightBeamBottomRadius()
        {
            return _length * 0.1;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetLightBeamTopRadius()
        {
            return 8.0 * GetLightBeamBottomRadius();
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetLightBeamLength()
        {
            return _length * 0.3;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetHorizontalCylinderYPosition(double orientationSign)
        {
	        double supportHorizontalCylinderYOffset =  GetBottomOfViewport();
	        supportHorizontalCylinderYOffset -=  2.0 * GetBaseHeight();
	        supportHorizontalCylinderYOffset -= GetLightHousingLength() * 0.75;

	        return -orientationSign * supportHorizontalCylinderYOffset;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetBaseLegendYPosition()
        {
            return -_length + GetBaseHeight();
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetBaseLegendXPosition()
        {
            double baseWidth = GetBaseWidth();

            return -baseWidth * 1.25;
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>        
        public double GetLegendCharacterHeight()
        {
            return 3 * GetBaseHeight();
        }

        #endregion
    }
}
