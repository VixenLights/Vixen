using System.Windows.Shapes;

namespace VixenModules.Editor.PolygonEditor.ViewModels
{
	/// <summary>
	/// Maintains a polygon line segment.
	/// </summary>
	/// <remarks>This data structure helps with hit tests to determine if the mouse is over a polygon line segment.</remarks>
	public class PolygonLineSegment
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		public PolygonLineSegment()
		{
			// Create a WPF line to help with hit tests
			Line = new Line();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the WPF Line to help with hit testing.
		/// </summary>
		public Line Line { get; private set; }

		/// <summary>
		/// Beginning point for the line segment.
		/// </summary>
		public PolygonPointViewModel StartPoint { get; set; }
		
		/// <summary>
		/// Ending point for the line segment.
		/// </summary>
		public PolygonPointViewModel EndPoint { get; set; }

		#endregion
	}
}
