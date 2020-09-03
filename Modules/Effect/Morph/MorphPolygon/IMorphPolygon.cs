using System;
using System.Collections.Generic;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Polygon;

namespace VixenModules.Effect.Morph
{
	/// <summary>
	/// Maintains the properties of a polygon/line for the morph effect.
	/// </summary>
	public interface IMorphPolygon : ICloneable
	{
		/// <summary>
		/// Determines how the polygon/line is filled(Wipe, Solid or Outline).
		/// </summary>
		PolygonFillType FillType { get; set; }

		/// <summary>
		/// Determines the length of the wipe head.
		/// </summary>
		int HeadLength { get; set; }

		/// <summary>
		/// Determines the percentage of the effect duration it takes the wipe head to travel across the polygon/line. 
		/// </summary>
		int HeadDuration { get; set; }

		/// <summary>
		/// Determines the acceleration of the polygon/line wipe.
		/// </summary>
		int Acceleration { get; set; }

		/// <summary>
		/// Determines the wipe head color over the duration of the effect.
		/// </summary>
		ColorGradient HeadColor { get; set; }

		/// <summary>
		/// Determines the wipe tail color over the duration of the effect.
		/// </summary>
		ColorGradient TailColor { get; set; }

		/// <summary>
		/// Determines the color of the polygon/line for the duration of the effect.
		/// </summary>
		ColorGradient FillColor { get; set; }

		/// <summary>
		/// Determines the position in the effect for the polygon/line (0.0 - 1.0).
		/// </summary>
		double Time { get; set; }

		/// <summary>
		/// Polygon associated with the morph polygon.
		/// </summary>
		/// <remarks>Optional property may be null</remarks>
		Polygon Polygon { get; set; }

		/// <summary>
		/// Line associated with the morph polygon.  
		/// </summary>
		Line Line { get; set; }

		/// <summary>
		/// Ellipse associated with the morph polygon.
		/// </summary>
		Ellipse Ellipse { get; set; }

		/// <summary>
		/// Returns true if the morph polygon is to be deleted.
		/// </summary>
		/// <remarks>This property helps resolve shapes coming from the polygon editor back to the morph effect</remarks>
		bool Removed { get; set; }
		
		/// <summary>
		/// Limits the polygon/line points to the specified width and height.
		/// </summary>		
		void LimitPoints(int width, int height);

		/// <summary>
		/// Gets the points of the polygon or line.
		/// </summary>
		/// <returns></returns>
		IList<PolygonPoint> GetPolygonPoints();

		/// <summary>
		/// Gets a point based shape for either the polygon or line.
		/// </summary>		
		/// <remarks>This method allows for common shape processing code for polygon or line</remarks>
		PointBasedShape GetPointBasedShape();	
		
		/// <summary>
		/// Label of the shape.
		/// </summary>
		string Label { get; set; }

		/// <summary>
		/// Determines the percentage into the effect when the wipe should start.
		/// This property is similar to the stagger property but is used for Free Form mode. 
		/// </summary>
		int StartOffset { get; set; }

		/// <summary>
		/// Scales the associated shape by the X and Y scale factors.
		/// </summary>
		/// <param name="xScaleFactor">X Axis scale factor</param>
		/// <param name="yScaleFactor">Y Axis scale factor</param>
		/// <param name="width">Width of the display element</param>
		/// <param name="height">Height of the display element</param>
		void Scale(double xScaleFactor, double yScaleFactor, int width, int height);

		/// <summary>
		/// Determines the brightness of the wipe tail.
		/// </summary>
		Curve TailBrightness { get; set; }

		/// <summary>
		/// Determines the brightness of the wipe head.
		/// </summary>
		Curve HeadBrightness { get; set; }

		/// <summary>
		/// Determines the brightness of the associated solid or outlined shape.
		/// </summary>
		Curve FillBrightness { get; set; }
	}
}
