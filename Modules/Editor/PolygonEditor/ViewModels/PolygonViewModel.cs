using Catel.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using VixenModules.App.Polygon;
using Point = System.Windows.Point;

namespace VixenModules.Editor.PolygonEditor.ViewModels
{
	/// <summary>
	/// Maintains a polygon view model.
	/// </summary>
	public class PolygonViewModel : PointBasedViewModel
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="polygon">Model polygon</param>
		/// <param name="labelVisible">Whether the label is visible</param>
		public PolygonViewModel(Polygon polygon, bool labelVisible) :
			base(labelVisible)
		{
			// Store off the model
			Polygon = polygon;

			// The WPF polygon is not shown until the polygon is closed
			Visibility = false;
						
			// By default the polygon starts with no points, so we are in point mode			
			PolygonClosed = false;
			
			// Create the center point but set the parent to null so that it doesn't
			// fire property change events on both the X and Y coordinates.
			// One event is done for the pair when the point is updated.
			CenterPoint = new PolygonPointViewModel(new PolygonPoint(), null);
			
			// Loop over all the points in the polygon model
			foreach(PolygonPoint pt in Polygon.Points)
			{
				// Add a view model point for each model point
				PointCollection.Add(new PolygonPointViewModel(pt, this));
			}
			
			// If this is complete polygon then...
			if (Polygon.Points.Count >= 3)
			{
				// Calculate the center of the polygon
				UpdateCenterPoint();

				// Add the line segment to the collection
				Segments.Add(new LineSegmentViewModel(PointCollection[0], PointCollection[1]));

				// Make the WPF polygon visible
				ClosePolygon();

				// Display the green line if the fill type is a wipe
				SegmentsVisible = (Polygon.FillType == PolygonFillType.Wipe);
			}

			// Register for the collection changed event
			PointCollection.CollectionChanged += PointCollection_CollectionChanged;

			// Create a collection of line segments
			LineSegments = new List<PolygonLineSegment>();
			
			// Mark the polygon as dirty, this causes the line segments to get refreshed
			Dirty = true;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Point Collection changed event handler.
		/// </summary>		
		private void PointCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// Raise the property changed event so the converter runs in the view
			RaisePropertyChanged(nameof(PointCollection));
		}

		#endregion

		#region Public Model Properties

		/// <summary>
		/// Model representation of the polygon.
		/// </summary>
		public Polygon Polygon
		{
			get
			{
				return (Polygon)Shape;
			}
			set
			{
				Shape = value;
			}
		}

		#endregion
		
		#region Public Properties
		
		/// <summary>
		/// True when the polygon has been completed.
		/// </summary>
		public bool PolygonClosed
		{
			get;
			set;
		}

		/// <summary>
		/// Flag indicating the polygon points have changed.
		/// </summary>
		public bool Dirty
		{
			get;
			set;
		}

		/// <summary>
		/// Gets collection of polygon line segments.
		/// </summary>
		/// <remarks>This property helps with polygon line segment hit testing.</remarks>
		public List<PolygonLineSegment> LineSegments { get; private set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Raises the property change event from the PointCollection and Segments properties.		
		/// </summary>
		/// <remarks>This method is needed to trigger a converter in the view.</remarks>
		public override void NotifyPointCollectionChanged()
		{
			// Call the base class implementation
			base.NotifyPointCollectionChanged();

			// Mark the polygon as dirty so that the line segments are refreshed
			Dirty = true;
		}

		/// <summary>
		/// Returns true when a polygon point is selected.
		/// </summary>
		/// <returns>True when a polygon point is selected.</returns>
		public bool IsPointSelected()
		{
			return SelectedVertex != null;
		}
		
		/// <summary>
		/// Returns true if all points are selected.
		/// </summary>
		/// <returns>True if all points are selected.</returns>
		public bool AreAllPointsSelected()
		{
			// Return true if all points are selected
			return PointCollection.All(point => point.Selected);			
		}
				
		/// <summary>
		/// Get the next point on the polygon following the selected point.
		/// </summary>
		/// <remarks>This method is used to help draw a ghost point</remarks>
		/// <returns>Next polygon point</returns>
		public PolygonPointViewModel GetNextPoint()
		{
			// Get the index of the selected point
			int selectedPointIndex = PointCollection.IndexOf(SelectedVertex);

			// Attempt to advance to the next point
			int nextPoint = selectedPointIndex + 1;

			// If we have passed the last point then...
			if (nextPoint > PointCollection.Count - 1)
			{
				// Wrap around to the first point
				nextPoint = 0;
			}

			// Return the next point
			return PointCollection[nextPoint];
		}

		/// <summary>
		/// Get the previous point to the selected point on the polygon.
		/// </summary>
		/// <remarks>This method is used to help draw a ghost point</remarks>
		/// <returns>Next polygon point</returns>
		public PolygonPointViewModel GetPreviousPoint()
		{
			// Get the index of the selected point
			int selectedPointIndex = PointCollection.IndexOf(SelectedVertex);

			// Attempt to move to the previous point
			int prevPoint = selectedPointIndex - 1;

			// If we passed the first point then...
			if (prevPoint < 0)
			{
				// Wrap around to the last point
				prevPoint = PointCollection.Count - 1;
			}

			// Return the previous point
			return PointCollection[prevPoint];
		}

		/// <summary>
		/// Inserts a polygon point at the specified position and at the specified index in the point collection.
		/// </summary>
		/// <param name="position">Position of the point</param>
		/// <param name="insertPosition">Index into the point collection</param>
		public void InsertPoint(Point position, int insertPosition)
		{
			// Create a new polygon point model object
			PolygonPoint modelPoint = new PolygonPoint();

			// Create a new view model polygon point object
			PolygonPointViewModel viewModelPoint = new PolygonPointViewModel(modelPoint, this);

			// Insert the model point into the model point collection
			Polygon.Points.Insert(insertPosition, modelPoint);

			// Insert the view model point into the point collection
			PointCollection.Insert(insertPosition, viewModelPoint);

			// Initialize the position of the point
			viewModelPoint.X = position.X;
			viewModelPoint.Y = position.Y;
		}

		/// <summary>
		/// Adds the specified point to the point collection.
		/// </summary>
		/// <param name="position">Position of the new point</param>
		public override void AddPoint(Point position)
		{						
			// Create the new model point
			PolygonPoint modelPoint = new PolygonPoint();

			// Add the model point to the model's point collection
			Polygon.Points.Add(modelPoint);

			// Create the new view model point
			PolygonPointViewModel viewModelPoint = new PolygonPointViewModel(modelPoint, this);

			// Initialize the position of the point
			viewModelPoint.X = position.X;
			viewModelPoint.Y = position.Y;

			// Add the point to the view model point collection
			PointCollection.Add(viewModelPoint);

			// If there are at least two points on the polygon then...
			if (PointCollection.Count > 1)
			{
				// Create a segment between the points
				LineSegmentViewModel segment =				
					new LineSegmentViewModel(
						viewModelPoint,
						PointCollection[PointCollection.Count - 2]);

				// Add the segment
				Segments.Add(segment);
			}

			// If there are two or more points then...
			if (PointCollection.Count >= 2)
			{
				// If the mouse is over the first polygon point then...
				if (IsMouseOverFirstPolygonPoint(position))
				{
					// Remove the last point since we are going to connect up to the first point
					PointCollection.Remove(PointCollection[PointCollection.Count - 1]);
					Polygon.Points.Remove(Polygon.Points[Polygon.Points.Count - 1]);

					// Make the WPF polygon visible
					ClosePolygon();
				}
			}			
		}

		/// <summary>
		/// Returns true if the mouse is over the first polygon point.
		/// </summary>
		/// <param name="mousePosition">Position of the mouse</param>
		/// <returns>True if the mouse is over the first polygon point</returns>
		public bool IsMouseOverFirstPolygonPoint(Point mousePosition)
		{
			// Calculate the distance from this point to the first point
			double deltaX = Math.Abs(PointCollection[0].X - mousePosition.X);
			double deltaY = Math.Abs(PointCollection[0].Y - mousePosition.Y);

			// The user has to get within 10 pixels of the first point
			const int Tolerance = 10;

			// Return whether the mouse is over the first polygon point
			return deltaX <= Tolerance &&
			       deltaY <= Tolerance;
		}

		/// <summary>
		/// Toggles the start side of a polygon.
		/// </summary>
		public void ToggleStartSide()
		{
			// Get first view model point
			PolygonPointViewModel point1 = PointCollection[0];

			// Remove the first view model point
			PointCollection.Remove(point1);

			// Add the point to the end of the point collection
			PointCollection.Add(point1);

			// Get the first model point
			PolygonPoint pt1 = Polygon.Points[0];

			// Remove the first model point
			Polygon.Points.Remove(pt1);

			// Add the point to the end of the point collection
			Polygon.Points.Add(pt1);

			// Update the green line segment
			Segments[0] = new LineSegmentViewModel(PointCollection[0], PointCollection[1]);
			Segments[0].Color = Colors.Lime;

			// Tell the view to refresh
			NotifyPointCollectionChanged();
		}

		/// <summary>
		/// Deletes the specified polygon point.
		/// </summary>
		public void DeletePoint(PolygonPointViewModel pt)
		{
			// Remove the point from the polygon
			PointCollection.Remove(pt);
			Polygon.Points.Remove(pt.PolygonPoint);

			// Update the green line segment
			Segments[0] = new LineSegmentViewModel(PointCollection[0], PointCollection[1]);
			Segments[0].Color = Colors.Lime;

			// Raise the collection Property changed event so that the converters in the view run
			NotifyPointCollectionChanged();
		}

		#endregion

		#region Public Static Properties

		/// <summary>
		/// Configure the polygons to draw the start side in a special color.
		/// </summary>
		/// <remarks>This property is static due to this value is not available at the time of polygon construction.</remarks>
		public static bool ColorStartSide { get; set; }

		#endregion

		#region Private Methods

		/// <summary>
		/// Completes the polygon by connecting the last point to the first point.
		/// </summary>
		private void ClosePolygon()
		{
			// Remember that we closed the polygon
			PolygonClosed = true;
			
			// If the start side is being colored a special color then...
			if (ColorStartSide)
			{
				// Remove all the segments except the last one
				while (Segments.Count > 1)
				{
					Segments.Remove(Segments[Segments.Count - 1]);
				}

				// Wipe fill type is only valid for polygons with four or three sides
				if (PointCollection.Count == 4 ||
				    PointCollection.Count == 3)
				{
					// Color the start side green
					Segments[0].Color = Colors.Lime;
				}
				// Otherwise if the fill type is set to wipe then...
				else if (Polygon.FillType == PolygonFillType.Wipe)
				{
					// Set the fill type to solid since the polygon does meet the requirements of a wipe
					Polygon.FillType = PolygonFillType.Solid;
					
					// Hide the green line
					SegmentsVisible = false;
				}
			}
			// Otherwise hide all segments
			else
			{
				SegmentsVisible = false;
			}

			// Fire the property changed event so the converters run
			NotifyPointCollectionChanged();

			// Make the WPF polygon visible
			Visibility = true;
		}
		
		/// <summary>
		/// Updates the center point of the polygon.
		/// </summary>
		protected override void UpdateCenterPoint()
		{
			// If the polygon contains at least three points then...
			if (PointCollection.Count > 2)
			{
				// Get the minimum X coordinate 
				double xMin = PointCollection.Min(point => point.X);

				// Get the maximum X coordinate 
				double xMax = PointCollection.Max(point => point.X);

				// Get the minimum Y coordinate 
				double yMin = PointCollection.Min(point => point.Y);

				// Get the maximum Y coordinate 
				double yMax = PointCollection.Max(point => point.Y);

				// Calculate the center of the polygon
				CenterPoint.X = (xMax - xMin) / 2.0 + xMin;
				CenterPoint.Y = (yMax - yMin) / 2.0 + yMin;

				// Notify the view that the center has changed
				RaisePropertyChanged(nameof(CenterPoint));
			}

			// Update the point labels
			UpdatePointLabels();
		}

		#endregion

	}
}
