using System.Collections.Generic;

namespace VixenModules.App.Polygon
{
	/// <summary>
	/// Maintains polygons and lines and associated meta-data.
	/// Also contains information about the the associated display element.
	/// </summary>
	/// <remarks>This container is used to pass shapes between effects and the polygon editor.</remarks>
	public class PolygonContainer
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		public PolygonContainer()
		{
			Polygons = new List<Polygon>();
			Lines = new List<Line>();
			PolygonTimes = new List<double>();
			LineTimes = new List<double>();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the collection of polygons.
		/// </summary>
		public List<Polygon> Polygons { get; set; }

		/// <summary>
		/// Gets or sets the collection of times associated with the polygons.
		/// </summary>
		public List<double> PolygonTimes { get; set; }

		/// <summary>
		/// Gets or sets the collection of lines.
		/// </summary>
		public List<Line> Lines { get; set; }

		/// <summary>
		/// Gets or sets the collection of times associated with the lines.
		/// </summary>
		public List<double> LineTimes { get; set; }

		/// <summary>
		/// Gets or sets Width of the associated display element.
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// Gets or sets Height of the associated display element.
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// Gets or sets the editor capabilities desired.
		/// This property allows the client to configure the capabilities of the Polygon Editor.
		/// </summary>
		public PolygonEditorCapabilities EditorCapabilities { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Clones the polygon container and most of the asssociated data.
		/// </summary>		
		public PolygonContainer Clone()
		{
			// Create a new polygon container
			PolygonContainer clonedContainer = new PolygonContainer();

			// Clone the polygons
			foreach(Polygon polygon in Polygons)
			{
				// Clone the polygon
				Polygon clonePolygon = polygon.Clone();
				
				// Restore the original ID
				clonePolygon.ID = polygon.ID;

				// Add the polygon to the container
				clonedContainer.Polygons.Add(clonePolygon);
			}

			// Clone the lines
			foreach(Line line in Lines)
			{
				// Clone the line
				Line cloneLine = line.Clone();

				// Restore the original ID
				cloneLine.ID = line.ID;

				// Add the line to the container
				clonedContainer.Lines.Add(cloneLine);
			}

			// Clone the polygon times
			foreach(double time in PolygonTimes)
			{
				clonedContainer.PolygonTimes.Add(time);
			}
						
			// Clone the line times
			foreach (double time in LineTimes)
			{
				clonedContainer.LineTimes.Add(time);
			}

			// Copy the width and height
			clonedContainer.Width = Width;
			clonedContainer.Height = Height;

			// Making a copy of the Editor capability reference
			clonedContainer.EditorCapabilities = EditorCapabilities;

			return clonedContainer;
		}

		#endregion
	}
}
