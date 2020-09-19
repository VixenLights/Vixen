using Catel.Data;
using Catel.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using VixenModules.Editor.PolygonEditor.Converters;

namespace VixenModules.Editor.PolygonEditor.ViewModels
{
	/// <summary>
	/// Maintains a shape view model.
	/// </summary>
	public abstract class ShapeViewModel : ViewModelBase
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="labelVisible">Whether the label is visible</param>
		public ShapeViewModel(bool labelVisible)
		{
			// Initialize the point collection
			PointCollection = new ObservableCollection<PolygonPointViewModel>();

			// Show or hide the label
			LabelVisible = labelVisible;

			// Initialize the center hash mark to black						
			CenterPointColor = Colors.Black;
		}

		#endregion

		#region Protected Constants

		/// <summary>
		/// Tolerance for how close the user needs to click near a visual.
		/// </summary>
		protected const int ClickTolerance = 5;

		#endregion

		#region Catel Public Properties

		/// <summary>
		/// Shape model property.
		/// </summary>
		[Model]
		public App.Polygon.Shape Shape
		{
			get { return GetValue<App.Polygon.Shape>(ShapeProperty); }
			set { SetValue(ShapeProperty, value); }
		}

		/// <summary>
		/// Shape model property data.
		/// </summary>
		public static readonly PropertyData ShapeProperty = RegisterProperty(nameof(Shape), typeof(App.Polygon.Shape));

		/// <summary>
		/// Label of the shape.
		/// </summary>
		[ViewModelToModel("Shape")]
		public string Label
		{
			get { return GetValue<string>(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}

		/// <summary>
		/// Label property data.
		/// </summary>
		public static readonly PropertyData LabelProperty = RegisterProperty(nameof(Label), typeof(string), null);

		/// <summary>
		/// Controls whether the shape's label is visible.
		/// </summary>
		public bool LabelVisible
		{
			get { return GetValue<bool>(LabelVisibleProperty); }
			set { SetValue(LabelVisibleProperty, value); }
		}

		/// <summary>
		/// LabelVisible property data.
		/// </summary>
		public static readonly PropertyData LabelVisibleProperty = RegisterProperty(nameof(LabelVisible), typeof(bool), null);

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
		public static readonly PropertyData CenterPointColorProperty = RegisterProperty(nameof(CenterPointColor), typeof(Color), null);

		/// <summary>
		/// Position of the center point hash mark of the shape.
		/// </summary>
		public PolygonPointViewModel CenterPoint
		{
			get { return GetValue<PolygonPointViewModel>(CenterPointProperty); }
			set { SetValue(CenterPointProperty, value); }
		}

		/// <summary>
		/// CenterPoint property data.
		/// </summary>
		public static readonly PropertyData CenterPointProperty = RegisterProperty(nameof(CenterPoint), typeof(PolygonPointViewModel), null);

		/// <summary>
		/// True when the underlying shape is visible.		
		/// </summary>
		public bool Visibility
		{
			get { return GetValue<bool>(VisibilityProperty); }
			set { SetValue(VisibilityProperty, value); }
		}

		/// <summary>
		/// Visibility property data.
		/// </summary>
		public static readonly PropertyData VisibilityProperty = RegisterProperty(nameof(Visibility), typeof(bool), null);

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
		public static readonly PropertyData SelectedVertextProperty = RegisterProperty(nameof(SelectedVertex), typeof(PolygonPointViewModel), null);

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
		public static readonly PropertyData PointCollectionProperty = RegisterProperty(nameof(PointCollection), typeof(ObservableCollection<PolygonPointViewModel>));

		#endregion

		#region Public Properties

		/// <summary>
		/// True when all points that make up the shape are selected.
		/// </summary>
		public bool AllPointsSelected
		{
			get;
			set;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Selects all the points on the shape and the center hash mark.
		/// </summary>
		public void SelectShape()
		{
			// Loop over all points
			foreach (PolygonPointViewModel point in PointCollection)
			{
				// Select the point
				point.Selected = true;
			}

			// Color the center hash red
			CenterPointColor = Colors.HotPink;

			// Set a flag indicating the entire shape is selected
			AllPointsSelected = true;
		}

		/// <summary>
		/// Adds the specified point to the point collection.
		/// </summary>
		/// <param name="position">Position of the new point</param>
		public abstract void AddPoint(Point position);

		/// <summary>
		/// Deselects all points on the shape.
		/// </summary>		
		public void DeselectAllPoints()
		{
			// Clear the flag that indicates that all points are selected
			AllPointsSelected = false;

			// Reset the center hash color
			CenterPointColor = Colors.Black;

			// Clear out the selected point
			SelectedVertex = null;

			// Loop over all the points on the shape
			foreach (PolygonPointViewModel point in PointCollection)
			{
				// Deselect the point
				point.Selected = false;
			}
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

		/// <summary>
		/// Moves the selected point to the specified position.
		/// </summary>
		/// <param name="clickPosition">New position of point.</param>
		public void MoveSelectedPoint(Point clickPosition)
		{
			// Move the selected point to the click poisition
			SelectedVertex.X = clickPosition.X;
			SelectedVertex.Y = clickPosition.Y;

			NotifyPointCollectionChanged();
		}

		/// <summary>
		/// Raises the property change event for the PointCollection property.		
		/// </summary>
		/// <remarks>This method is needed to trigger a converter in the view.</remarks>
		public virtual void NotifyPointCollectionChanged()
		{
			// Notify the view that the points have changed
			RaisePropertyChanged(nameof(PointCollection));
			
			// Update the center point of the polygon 
			UpdateCenterPoint();			
		}

		/// <summary>
		/// Returns true if the specified position is over the center of the shape.
		/// </summary>
		/// <param name="position">Position to test</param>
		/// <returns>True if the position is over the center of the shape</returns>
		public bool IsOverCenterCrossHash(Point position)
		{
			bool overCenterHash = false;
			
			// If the center point has been defined then...
			if (CenterPoint != null)
			{
				// Determine how far the click position is from the center
				double deltaX = Math.Abs(CenterPoint.X - position.X);
				double deltaY = Math.Abs(CenterPoint.Y - position.Y);

				// If the differnce is less than the click tolerance then...
				if (deltaX <= ClickTolerance &&
					deltaY <= ClickTolerance)
				{
					// Indicate that the user if over the center cross hash
					overCenterHash = true;
				}
			}

			return overCenterHash;
		}

		/// <summary>
		/// Snaps the shapes points to the nearest display element pixel.
		/// </summary>
		/// <param name="actualWidth">Width of the canvas</param>
		/// <param name="actualHeight">Height of the canvas</param>
		public void SnapToGrid(double actualWidth, double actualHeight)
		{
			// Loop over the points on the shape
			foreach (PolygonPointViewModel point in PointCollection)
			{
				// Round to the nearest display element pixel
				int x = (int)Math.Round(point.X / PolygonPointXConverter.XScaleFactor);
				int y = (int)Math.Round(point.Y / PolygonPointYConverter.YScaleFactor);

				// Convert back to computer display pixels
				point.X = x * PolygonPointXConverter.XScaleFactor;
				point.Y = y * PolygonPointYConverter.YScaleFactor;

				// Make sure the X coordinate did not get rounded outside the canvas
				if (point.X > actualWidth - 1)
				{
					point.X = actualWidth - 1;
				}

				// Make sure the Y coordinate did not get rounded outside the canvas
				if (point.Y > actualHeight - 1)
				{
					point.Y = actualHeight - 1;
				}
			}
		}

		/// <summary>
		/// Moves the points on the shape the specified x and y offset.
		/// </summary>
		/// <param name="moveX">X coordinate offset</param>
		/// <param name="moveY">Y coordinate offset</param>
		/// <param name="limitPointToCanvas">Function to limit the point to the editor canvas</param>
		public void MovePoints(			 
			double moveX, 
			double moveY,
			Func<Point, Point> limitPointToCanvas)			
		{
			// Loop over the points on the shape
			foreach (PolygonPointViewModel pt in PointCollection)
			{
				// Move the points the specified X and Y offset
				pt.X += moveX;
				pt.Y += moveY;
				
				// Make sure the point is on the canvas
				Point position = limitPointToCanvas(pt.GetPoint());

				// Update the point to the limited point
				pt.X = position.X;
				pt.Y = position.Y;
			}
		}

		/// <summary>
		/// Selects the shape's points that are inside the specified lasso rectangle.
		/// </summary>
		/// <param name="lasso"></param>
		/// <returns></returns>
		public IList<PolygonPointViewModel> SelectShapePoints(Rect lasso)
		{
			// Create the collection of selected points
			List<PolygonPointViewModel> selectedPoints = new List<PolygonPointViewModel>();

			// Loop over the points on the shape
			foreach (PolygonPointViewModel point in PointCollection)
			{
				// If the point is inside the rectangle then...
				if (lasso.Contains(point.GetPoint()))
				{
					// Select the point
					point.Selected = true;
					selectedPoints.Add(point);
				}
			}

			// Return the collection of selected points
			return selectedPoints;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Updates the center point of the shape.
		/// </summary>
		protected abstract void UpdateCenterPoint();

		/// <summary>
		/// Updates the labels on the shape's points.
		/// </summary>
		protected void UpdatePointLabels()
		{
			//  Loop over the shape points			
			for (int index = 0; index < PointCollection.Count; index++)
			{
				// Convert the point index into a string label
				PointCollection[index].Label = (index + 1).ToString();
			}
		}

		#endregion		
	}
}
