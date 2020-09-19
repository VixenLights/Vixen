using Catel.Data;
using Catel.MVVM;
using System.Windows.Media;

namespace VixenModules.Editor.PolygonEditor.ViewModels
{
	/// <summary>
	/// Maintains a line segment.
	/// The line segments are used to draw the polygon until the polygon is closed.
	/// It is also used to highlight the start side of a wipe polygon in green.
	/// </summary>
	public class LineSegmentViewModel : ViewModelBase
	{
		#region Constructor
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="point1">First point of the line segment</param>
		/// <param name="point2">Second point of the line segment</param>
		public LineSegmentViewModel(PolygonPointViewModel point1, PolygonPointViewModel point2)
		{
			// Store off the line segment points
			Point1 = point1; 
			Point2 = point2; 

			// Set the line segment color to blue
			Color = Colors.DodgerBlue;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets and sets point 1 of the line segment.
		/// </summary>
		public PolygonPointViewModel Point1 { get; set; }

		/// <summary>
		/// Gets and sets point 2 of the line segment.
		/// </summary>
		public PolygonPointViewModel Point2 { get; set; }

		#endregion

		#region Public Catel Properties

		/// <summary>
		/// Color of the line.
		/// </summary>
		public Color Color
		{
			get { return GetValue<Color>(CenterPointColorProperty); }
			set { SetValue(CenterPointColorProperty, value); }
		}

		/// <summary>
		/// CenterPointColor property data.
		/// </summary>
		public static readonly PropertyData CenterPointColorProperty = RegisterProperty(nameof(Color), typeof(Color), null);

		#endregion
	}
}
