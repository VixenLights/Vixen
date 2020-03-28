using Catel.Data;
using Catel.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Windows.Point;
using System.Linq;
using VixenModules.App.Polygon;

namespace PolygonEditor
{
	/// <summary>
	/// Maintains a polygon view model.
	/// </summary>
	public class PolygonViewModel : ViewModelBase
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public PolygonViewModel(Polygon polygon, PolygonEditorViewModel parent)
		{
			// Store off the model
			Polygon = polygon;

			// The WPF polygon is not shown until the polygon is closed
			Visibility = false;
						
			// By default the polygon starts with no points, so we are in point mode			
			PolygonClosed = false;
			
			// Create a colletion of line segments to draw the polygon until it is closed
			Segments = new ObservableCollection<Tuple<PolygonPointViewModel, PolygonPointViewModel>>();
			
			// Initialize the segments to visible
			SegmentsVisible = true;

			// Create the collection of polygon points
			PointCollection = new ObservableCollection<PolygonPointViewModel>();

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

				// Make the WPF polygon visible
				ClosePolygon();
			}

			// Register for the collection changed event
			PointCollection.CollectionChanged += PointCollection_CollectionChanged;

			// Initialize the center hash mark to black						
			CenterPointColor = Colors.Black;

			// Create commands
			DeletePointCommand = new Command(DeletePoint, CanExecuteDeletePoint);			
			
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
		private void PointCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			// Raise the property changed event so the converter runs in the view
			RaisePropertyChanged("PointCollection");
		}

		#endregion

		#region Public Model Properties
		
		/// <summary>
		/// Model representation of the polygon.
		/// </summary>
		public Polygon Polygon { get; set; }

		#endregion

		#region Public Properties

		/// <summary>
		/// Parent view model.
		/// </summary>
		public PolygonEditorViewModel Parent { get; set; }

		#endregion

		#region Public Catel Line Segment Properties

		/// <summary>
		/// Controls whether the segments are visible.  These segment lines help define the polygon until the polygon has been closed.
		/// </summary>
		public bool SegmentsVisible
		{
			get { return GetValue<bool>(SegmentsVisibleProperty); }
			set { SetValue(SegmentsVisibleProperty, value); }
		}

		/// <summary>
		/// SegmentsVisible property data.
		/// </summary>
		public static readonly PropertyData SegmentsVisibleProperty = RegisterProperty("SegmentsVisible", typeof(bool), null);

		/// <summary>
		/// Maintains a collection of line segments.  The line segments help define the polygon until the polygon has been closed.
		/// </summary>
		public ObservableCollection<Tuple<PolygonPointViewModel, PolygonPointViewModel>> Segments
		{
			get { return GetValue<ObservableCollection<Tuple<PolygonPointViewModel, PolygonPointViewModel>>>(SegmentsProperty); }
			private set { SetValue(SegmentsProperty, value); }
		}

		/// <summary>
		/// SegmentsVisible property data.
		/// </summary>
		public static readonly PropertyData SegmentsProperty =
			RegisterProperty("Segments", typeof(ObservableCollection<Tuple<PolygonPointViewModel, PolygonPointViewModel>>), null);

		#endregion
		
		#region Public Catel Properties
		
		/// <summary>
		/// Collection of polygon points.
		/// </summary>
		public ObservableCollection<PolygonPointViewModel> PointCollection
		{
			get { return GetValue<ObservableCollection<PolygonPointViewModel>>(PointCollectionProperty); }
			private set { SetValue(PointCollectionProperty, value); }			
		}

		/// <summary>		
		/// PointCollection property data.
		/// </summary>
		public static readonly PropertyData PointCollectionProperty = RegisterProperty("PointCollection", typeof(ObservableCollection<PolygonPointViewModel>));

		/// <summary>
		/// True when the underlying polygon is visible.
		/// Until three points are defined the polygon is not visible.
		/// </summary>
		public bool Visibility
		{
			get { return GetValue<bool>(VisibilityProperty); }
			set { SetValue(VisibilityProperty, value); }
		}

		/// <summary>
		/// Visibility property data.
		/// </summary>
		public static readonly PropertyData VisibilityProperty = RegisterProperty("Visibility", typeof(bool), null);

		/// <summary>
		/// Position of the center point hash mark.
		/// </summary>
		public PolygonPointViewModel CenterPoint
		{
			get { return GetValue<PolygonPointViewModel>(CenterPointProperty); }
			set { SetValue(CenterPointProperty, value); }
		}

		/// <summary>
		/// CenterPoint property data.
		/// </summary>
		public static readonly PropertyData CenterPointProperty = RegisterProperty("CenterPoint", typeof(PolygonPointViewModel), null);

		/// <summary>
		/// Selected vertex of the polygon.
		/// </summary>
		public PolygonPointViewModel SelectedVertex
		{
			get { return GetValue<PolygonPointViewModel>(SelectedVertextProperty); }
			set { SetValue(SelectedVertextProperty, value); }
		}

		/// <summary>
		/// SelectedVertex property data.
		/// </summary>
		public static readonly PropertyData SelectedVertextProperty = RegisterProperty("SelectedVertext", typeof(PolygonPointViewModel), null);
		
		/// <summary>
		/// Color of the center point hash.
		/// </summary>
		public Color CenterPointColor
		{
			get { return GetValue<Color>(CenterPointColorProperty); }
			set { SetValue(CenterPointColorProperty, value); }
		}

		/// <summary>
		/// CenterPointColor property data.
		/// </summary>
		public static readonly PropertyData CenterPointColorProperty = RegisterProperty("CenterPointColor", typeof(Color), null);

		#endregion

		#region Public Properties

		/// <summary>
		/// True when polygon has been selected.
		/// </summary>
		public bool AllPointsSelected
		{
			get;
			set;
		}

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
		public void NotifyPointCollectionChanged()
		{
			// Notify the view that the points have changed
			RaisePropertyChanged("PointCollection");
			RaisePropertyChanged("Segments");

			// Update the center point of the polygon 
			UpdateCenterPoint();

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
		/// Moves the selected point to the specified position.
		/// </summary>
		/// <param name="clickPosition">New position of point.</param>
		public void MoveSelectedPoint(Point clickPosition)
		{
			SelectedVertex.X = clickPosition.X;
			SelectedVertex.Y = clickPosition.Y;

			NotifyPointCollectionChanged();
		}

		/// <summary>
		/// Returns true if the specified position is over the center of the polygon.
		/// </summary>
		/// <param name="position">Position to test</param>
		/// <returns>True if the position is over the center of the polygon</returns>
		public bool IsOverCenterCrossHash(Point position)
		{
			bool overCenterHash = false;
			const int Tolerance = 5;

			if (CenterPoint != null)
			{
				double deltaX = Math.Abs(CenterPoint.X - position.X);
				double deltaY = Math.Abs(CenterPoint.Y - position.Y);

				if (deltaX <= Tolerance && 
					deltaY <= Tolerance)
				{
					overCenterHash = true;
				}
			}

			return overCenterHash;
		}

		/// <summary>
		/// Get the next point on the polygon following the selected point.
		/// </summary>
		/// <remarks>This method is used to help draw a ghost point</remarks>
		/// <returns>Next polygon point</returns>
		public PolygonPointViewModel GetNextPoint()
		{
			int selectedPointIndex = PointCollection.IndexOf(SelectedVertex);

			int nextPoint = selectedPointIndex + 1;

			if (nextPoint > PointCollection.Count - 1)
			{
				nextPoint = 0;
			}

			return PointCollection[nextPoint];
		}

		/// <summary>
		/// Get the previous point to the selected point on the polygon.
		/// </summary>
		/// <remarks>This method is used to help draw a ghost point</remarks>
		/// <returns>Next polygon point</returns>
		public PolygonPointViewModel GetPreviousPoint()
		{
			int selectedPointIndex = PointCollection.IndexOf(SelectedVertex);

			int prevPoint = selectedPointIndex - 1;

			if (prevPoint < 0)
			{
				prevPoint = PointCollection.Count - 1;
			}

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
		public void AddPoint(Point position)
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
				Tuple<PolygonPointViewModel, PolygonPointViewModel> segment =
					new Tuple<PolygonPointViewModel, PolygonPointViewModel>(
						viewModelPoint,
						PointCollection[PointCollection.Count - 2]);

				// Add the segment
				Segments.Add(segment);
			}

			// If there are three or more points then...
			if (PointCollection.Count >= 3)
			{
				// Calculate the distance from this point to the first point
				double deltaX = Math.Abs(PointCollection[0].X - PointCollection[PointCollection.Count - 1].X);
				double deltaY = Math.Abs(PointCollection[0].Y - PointCollection[PointCollection.Count - 1].Y);

				const int Tolerance = 5;

				// If this point is within the tolerance then close the polygon
				if (deltaX <= Tolerance && 
					deltaY <= Tolerance)
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
		/// Deselects all points on the specified polygon.
		/// </summary>
		/// <param name="polygon">Polygon to update</param>
		public void DeselectAllPoints()
		{
			// Clear the flag that indicates that all points are selected
			AllPointsSelected = false;

			// Reset the center hash color
			CenterPointColor = Colors.Black;

			// Clear out the selected point
			SelectedVertex = null;

			// Loop over all the points on the polygon
			foreach (PolygonPointViewModel point in PointCollection)
			{
				// Deselect the point
				point.Selected = false;
			}
		}

		/// <summary>
		/// Selects all the points on the polygon and the center hash mark.
		/// </summary>
		public void SelectPolygon()
		{
			// Loop over all points
			foreach (PolygonPointViewModel point in PointCollection)
			{
				// Select the point
				point.Selected = true;
			}

			// Color the center hash red
			CenterPointColor = Colors.HotPink;

			// Set a flag indicating the entire polygon is selected
			AllPointsSelected = true;
		}

		/// <summary>
		/// Selects the specified point.
		/// </summary>
		/// <param name="point">Point to select</param>
		public void SelectPoint(PolygonPointViewModel point)
		{			
			// Mark the point as selected
			point.Selected = true;

			// Store off the selected point
			SelectedVertex = point;
		}

		#endregion

		#region Public Commands

		/// <summary>
		/// Delete polygon point command.
		/// </summary>
		public ICommand DeletePointCommand { get; private set; }

		#endregion
		
		#region Private Methods

		/// <summary>
		/// Completes the polygon by connecting the last point to the first point.
		/// </summary>
		private void ClosePolygon()
		{
			// Remember that we closed the polygon
			PolygonClosed = true;

			// Once the polygon is closed there is no use for the line segments
			Segments.Clear();
			SegmentsVisible = false;

			// Fire the property changed event so the converters run
			NotifyPointCollectionChanged();

			// Make the WPF polygon visible
			Visibility = true;
		}
		
		/// <summary>
		/// Updates the center point of the polygon.
		/// </summary>
		private void UpdateCenterPoint()
		{
			// If the polygon contains at least three points then...
			if (PointCollection.Count > 2)
			{
				// Update the center point of the polygon
				CenterPoint = GetCenterOfPolygon();
			}
		}

		/// <summary>
		/// Calculates the center of the polygon.
		/// </summary>
		/// <returns>Center of the polygon.</returns>
		private PolygonPointViewModel GetCenterOfPolygon()
		{
			// Default the minimum X to the first point
			double xMin = PointCollection[0].X;

			// Default the maximum X to the first point
			double xMax = PointCollection[0].X;

			// Default the minimum Y to the first point
			double yMin = PointCollection[0].Y;

			// Default the maximum Y to the first point
			double yMax = PointCollection[0].Y;

			// Loop over the points in the polygon point collection
			foreach (PolygonPointViewModel point in PointCollection)
			{
				// If the point is less than the minimum X then...
				if (point.X < xMin)
				{
					// Update the minimum X
					xMin = point.X;
				}

				// If the point is less than the minimum Y then...
				if (point.Y < yMin)
				{
					// Update the minimum Y
					yMin = point.Y;
				}

				// If the point is greater than the maximum X then...
				if (point.X > xMax)
				{
					// Update the maximum X
					xMax = point.X;
				}

				// If the point is greater than the maximum Y then...
				if (point.Y > yMax)
				{
					// Update the maximum Y
					yMax = point.Y;
				}
			}

			// Create a new polygon point view model object; not giving it a model object 
			PolygonPointViewModel centerPoint = new PolygonPointViewModel(null, null);
			
			// Calculate the center of the polygon
			centerPoint.X = (xMax - xMin) / 2.0 + xMin;
			centerPoint.Y = (yMax - yMin) / 2.0 + yMin;

			// Return the center of the polygon
			return centerPoint;
		}

		#endregion

		#region Private Command Methods

		/// <summary>
		/// Deletes the currently selected polygon point.
		/// </summary>
		private void DeletePoint()
		{
			// Remove the point from the polygon
			PointCollection.Remove(SelectedVertex);
			Polygon.Points.Remove(SelectedVertex.PolygonPoint);

			// Clear out the selected point
			SelectedVertex = null;

			// Raise the collection Property changed event so that the converters in the view run
			NotifyPointCollectionChanged();
		}

		/// <summary>
		/// Returns true if the selected point can be deleted.
		/// </summary>
		/// <returns>Returns true if the selected point can be deleted.</returns>
		private bool CanExecuteDeletePoint()
		{
			// Can only delete a polygon point after the polygon is complete (closed) and
			// a point has been selected and
			// there are more than three points
			return PolygonClosed &&
				IsPointSelected() &&
				PointCollection.Count > 3;
		}

		#endregion

		
		/*
		// TODO: Should we add an AddPoint command ? 
		 
		public Command<object> AddPointCommand
		{
			get;
			private set;
		}

		//public Command RemovePoint { get; private set; }


		private void OnAddPointExecuted(object parameter)
		{
			Point clickPosition = (Point)parameter;

			AddPoint(clickPosition);
		}
		*/		
	}
}
