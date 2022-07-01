namespace VixenModules.Preview.VixenPreview.Fixtures.Geometry
{
	/// <summary>
	/// Provides dimensions for drawing a moving head.
	/// </summary>
	/// <remarks>
	/// Most of the methods in the class take the length of the drawing area.
	/// </remarks>
	interface IMovingHeadGeometryConstants
	{
		/// <summary>
		/// Gets the bottom of the drawing area still leaving space for the legened.
		/// </summary>		
		/// <returns>Bottom of the drawing area where the base should start</returns>
		double GetBottomOfViewport();

		/// <summary>
		/// Gets the width of the fixture base.
		/// </summary>		
		/// <returns>Width of the fixture base</returns>
		double GetBaseWidth();

		/// <summary>
		/// Gets the height of the fixture base.
		/// </summary>		
		/// <returns>Height of the fixture base</returns>
		double GetBaseHeight();

		/// <summary>
		/// Gets the depth of the fixture base.
		/// </summary>		
		/// <returns>Depth of the fixture base</returns>
		double GetBaseDepth();

		/// <summary>
		/// Gets the Y position of the fixture base.
		/// </summary>		
		/// <returns>Y position of the fixture base</returns>
		double GetBaseYPosition();

		/// <summary>
		/// Gets the X offset from center for the light support.
		/// </summary>		
		/// <returns>X offset from center for the light support</returns>
		double GetSupportXOffset();

		/// <summary>
		/// Gets the width of the vertical supports.
		/// </summary>		
		/// <returns>Width of the vertical supports</returns>
		double GetSupportWidth();

		/// <summary>
		/// Gets the height of the vertical supports.
		/// </summary>		
		/// <returns>Height of the vertical supports</returns>
		double GetSupportHeight();

		/// <summary>
		/// Gets the depth at the base of the vertical supports.
		/// </summary>		
		/// <returns>Depth at the base of the vertical supports</returns>
		double GetSupportBaseDepth();

		/// <summary>
		/// Gets the depth at the top of the vertical supports.
		/// </summary>		
		/// <returns>Depth at the top of the vertical supports</returns>
		double GetSupportTopDepth();

		/// <summary>
		/// Gets the Y position of the vertical supports.
		/// </summary>		
		/// <returns>Y position of vertical supports</returns>
		double GetSupportYPosition();

		/// <summary>
		/// Gets the light housing Y position.
		/// </summary>		
		/// <returns>Y position of light housing</returns>
		double GetLightHousingYPosition();

		/// <summary>
		/// Gets the light housing radius.
		/// </summary>		
		/// <returns>Radius of the light housing</returns>
		double GetLightHousingRadius();

		/// <summary>
		/// Gets the light housing length.
		/// </summary>		
		/// <returns>Radius of the light housing</returns>
		double GetLightHousingLength();

		/// <summary>
		/// Gets the percentage of the light housing below the horizontal support.
		/// </summary>		
		/// <returns>Percentage of the light housing below the horizontal support</returns>
		double GetLightHousingPercentageBelowHorizontalSupport();

		/// <summary>
		/// Gets the percentage of the light housing above the horizontal support.
		/// </summary>		
		/// <returns>Percentage of the light housing above the horizontal support</returns>
		double GetLightHousingPercentageAboveHorizontalSupport();

		/// <summary>
		/// Gets the horizontal support cylinder radius.
		/// </summary>		
		/// <returns>Horizontal support cylinder radius</returns>
		double GetHorizontalCylinderRadius();

		/// <summary>
		/// Gets the horizontal support Y position.
		/// </summary>		
		/// <returns>Horizontal support Y position</returns>
		double GetHorizontalCylinderYPosition();

		/// <summary>
		/// Gets the light beam bottom radius.
		/// </summary>		
		/// <returns>Light beam bottom radius</returns>
		double GetLightBeamBottomRadius();

		/// <summary>
		/// Gets the light beam top radius.
		/// </summary>		
		/// <returns>Light beam top radius</returns>
		double GetLightBeamTopRadius();

		/// <summary>
		/// Gets the light beam length.
		/// </summary>		
		/// <returns>Light beam length</returns>
		double GetLightBeamLength();

		/// <summary>
		/// Gets legend Y position.
		/// </summary>		
		/// <returns>Legend Y position</returns>
		double GetBaseLegendYPosition();

		/// <summary>
		/// Gets legend Y position.
		/// </summary>		
		/// <returns>Legend Y position</returns>
		double GetBaseLegendXPosition();

		/// <summary>
		/// Gets the legend character height.
		/// </summary>		
		/// <returns>Legend Y position</returns>
		double GetLegendCharacterHeight();
	}
}
