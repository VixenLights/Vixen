using Catel.Data;
using Catel.MVVM;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using VixenModules.App.Polygon;
using VixenModules.Editor.PolygonEditor.Adorners;

namespace VixenModules.Editor.PolygonEditor.ViewModels
{
	/// <summary>
	/// Maintains an ellipse view model.
	/// </summary>
	public class EllipseViewModel : PointBasedViewModel
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ellipseModel">Ellipse model</param>
		/// <param name="labelVisible">Determines if the label is visible</param>
		public EllipseViewModel(Ellipse ellipseModel, bool labelVisible) :
			base(labelVisible)
		{
			// Store off the model
			Ellipse = ellipseModel;

			// Default the ellipse to visible
			Visibility = true;

			// Create the center point of the ellipse
			CenterPoint = new PolygonPointViewModel(ellipseModel.Center, null);

			// Loop over all the points in the ellipse model
			foreach (PolygonPoint pt in ellipseModel.Points)
			{
				// Add a view model point for each model point
				PointCollection.Add(new PolygonPointViewModel(pt, null));
			}

			// If the ellipse has points then...
			if (ellipseModel.Points.Count > 0)
			{
				// Initialize the start side for when the ellipse is in wipe mode
				InitializeGreenLine();
			}

			// Update the position of the center point
			UpdateCenterPoint();

			// Fire the property changed event so the converters run
			NotifyPointCollectionChanged();
		}

		#endregion

		#region Protected Methods

		public override void AddPoint(Point position)
		{
			// Create the new model point
			PolygonPoint modelPoint = new PolygonPoint();

			// Add the model point to the model's point collection
			Ellipse.Points.Add(modelPoint);

			// Create the new view model point
			PolygonPointViewModel viewModelPoint = new PolygonPointViewModel(modelPoint, null);

			// Initialize the position of the point
			viewModelPoint.X = position.X;
			viewModelPoint.Y = position.Y;

			// Add the point to the view model point collection
			PointCollection.Add(viewModelPoint);

			// If this is complete polygon then...
			if (Ellipse.Points.Count >= 3)
			{
				// Calculate the center of the polygon
				UpdateCenterPoint();
			}
		}

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

		#region Public Model Properties

		/// <summary>
		/// Model representation of the ellipse.
		/// </summary>
		[Model]
		public App.Polygon.Ellipse Ellipse
		{
			get
			{
				return (App.Polygon.Ellipse)Shape;
				//return GetValue<App.Polygon.Ellipse>(EllipseModelProperty);
			}
			set
			{
				SetValue(EllipseModelProperty, value);
				Shape = value;
			}
		}

		public static readonly PropertyData EllipseModelProperty = RegisterProperty(nameof(Ellipse), typeof(App.Polygon.Ellipse), null);

		#endregion

		#region Catel Public Properties

		/// <summary>
		/// Gets or sets the width of the ellipse.
		/// </summary>
		[ViewModelToModel("Ellipse")]
		public double Width
		{
			get { return GetValue<double>(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}

		/// <summary>
		/// Width property data.
		/// </summary>
		public static readonly PropertyData WidthProperty = RegisterProperty(nameof(Width), typeof(double), null);

		/// <summary>
		/// Gets or sets the height of the ellipse.
		/// </summary>
		[ViewModelToModel("Ellipse")]
		public double Height
		{
			get { return GetValue<double>(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}

		/// <summary>
		/// Height property data.
		/// </summary>
		public static readonly PropertyData HeightProperty = RegisterProperty(nameof(Height), typeof(double), null);

		/// <summary>
		/// Gets or sets the left point of the ellipse rectangle.
		/// Ellipse graphics are drawn by specifying the left and top position of a rectangle.
		/// </summary>
		public double Left
		{
			get { return GetValue<double>(LeftProperty); }
			set { SetValue(LeftProperty, value); }
		}

		/// <summary>
		/// Left property data.
		/// </summary>
		public static readonly PropertyData LeftProperty = RegisterProperty(nameof(Left), typeof(double), null);

		/// <summary>
		/// Gets or sets the top point of the ellipse rectangle.
		/// Ellipse graphics are drawn by specifying the left and top position of a rectangle.
		/// </summary>
		public double Top
		{
			get { return GetValue<double>(TopProperty); }
			set { SetValue(TopProperty, value); }
		}

		/// <summary>
		/// Top property data.
		/// </summary>
		public static readonly PropertyData TopProperty = RegisterProperty(nameof(Top), typeof(double), null);

		/// <summary>
		/// Gets or sets the angle of the ellipse.
		/// </summary>
		[ViewModelToModel("Ellipse")]
		public double Angle
		{
			get { return GetValue<double>(AngleProperty); }
			set { SetValue(AngleProperty, value); }
		}

		/// <summary>
		/// Top property data.
		/// </summary>
		public static readonly PropertyData AngleProperty = RegisterProperty(nameof(Angle), typeof(double), null);

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the resize adorner.
		/// With the ellipse there were problems creating a new resize adorner based on the position of the points if a rotation has been applied.
		/// The easiest workaround for this was to just save off the adorner.
		/// </summary>
		public ResizeAdorner ResizeAdorner { get; set; }

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

			// If the ellipse rectangle is complete then...
			if (PointCollection.Count > 3)
			{
				// The rectangle bounding the ellipse segments are ordered such
				// that the first segment is colored green and is the start of the wipe.
				int ptZero = 0;
				int ptOne = 1;
				int ptTwo = 2;

				// To properly calculate the width and height we need to determine which line segments are which.
				if (Ellipse.StartSideRotation > 0)
				{
					// Adjust the ptZero and ptOne based on where the start side is located.
					ptZero -= Ellipse.StartSideRotation;
					if (ptZero < 0)
					{
						ptZero += 4;
					}

					ptOne -= Ellipse.StartSideRotation;
					if (ptOne < 0)
					{
						ptOne += 4;
					}

					ptTwo -= Ellipse.StartSideRotation;
					if (ptTwo < 0)
					{
						ptTwo += 4;
					}
				}

				// Calculate the width of the ellipse (rectangle)
				Width = Math.Sqrt((PointCollection[ptOne].X - PointCollection[ptZero].X) *
				                  (PointCollection[ptOne].X - PointCollection[ptZero].X) +
				                  (PointCollection[ptOne].Y - PointCollection[ptZero].Y) *
				                  (PointCollection[ptOne].Y - PointCollection[ptZero].Y));

				// Calculate the height of the ellipse (rectangle)
				Height = Math.Sqrt((PointCollection[ptTwo].X - PointCollection[ptOne].X) *
				                   (PointCollection[ptTwo].X - PointCollection[ptOne].X) +
				                   (PointCollection[ptTwo].Y - PointCollection[ptOne].Y) *
				                   (PointCollection[ptTwo].Y - PointCollection[ptOne].Y));

				// Update the Left and Top properties that are used to draw the ellipse
				Left = CenterPoint.X - Width / 2.0;
				Top = CenterPoint.Y - Height / 2.0;
			}
		}

		/// <summary>
		/// Initializes the green line segment that indicates where the wipe will start.
		/// </summary>
		public void InitializeGreenLine()
		{
			// Update the green line segment
			Segments.Add(new LineSegmentViewModel(PointCollection[0], PointCollection[1]));

			// Green line is only necessary when the ellipse is a wipe ellipse
			SegmentsVisible = (Ellipse.FillType == PolygonFillType.Wipe);

			// Color the segment green to indicate where the wipe starts
			Segments[0].Color = Colors.Lime;
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
			PolygonPoint pt1 = Ellipse.Points[0];

			// Remove the first model point
			Ellipse.Points.Remove(pt1);

			// Add the point to the end of the point collection
			Ellipse.Points.Add(pt1);

			// Increment the start side
			Ellipse.StartSideRotation++;

			// If we are past the last side then...
			if (Ellipse.StartSideRotation > 3)
			{
				// Wrap around to the first side 
				Ellipse.StartSideRotation = 0;
			}

			// Update the green line segment
			Segments[0] = new LineSegmentViewModel(PointCollection[0], PointCollection[1]);
			Segments[0].Color = Colors.Lime;

			// Force the view converters to run
			NotifyPointCollectionChanged();
		}

		#endregion
	}
}
