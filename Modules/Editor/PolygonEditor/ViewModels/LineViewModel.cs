using Catel.Data;
using System.Windows;
using System.Windows.Media;
using VixenModules.App.Polygon;

namespace VixenModules.Editor.PolygonEditor.ViewModels
{
	/// <summary>
	/// Maintains a line view model.
	/// </summary>
	public class LineViewModel : ShapeViewModel
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="line">Line model object</param>
		/// <param name="labelVisible">Determines if the label is visible</param>
		public LineViewModel(Line line, bool labelVisible):
			base(labelVisible)
		{
			// Store off the line model object
			Line = line;			

			// Loop over all the points in the line model
			foreach (PolygonPoint pt in Line.Points)
			{
				// Add a view model point for each model point
				PointCollection.Add(new PolygonPointViewModel(pt, this));
			}

			// If the line has at least one point then...
			if (PointCollection.Count > 1)
			{
				// Initialize the start point
				StartPoint = PointCollection[0];

				// If the fill type is a Wipe then...
				if (Line.FillType == PolygonFillType.Wipe)
				{
					// Point #1 is always green
					StartPoint.DeselectedColor = Colors.Green;
					StartPoint.Color = StartPoint.DeselectedColor;
				}
			}

			// If the line has two points then...
			if (PointCollection.Count == 2)
			{
				// Initialize the end point
				EndPoint = PointCollection[1];
				
				// Make the line visible
				Visibility = true;

				// Update the center point of the line
				UpdateCenterPoint();
			}
		}

		#endregion

		#region Public Catel Properties

		public PolygonPointViewModel StartPoint
		{
			get { return GetValue<PolygonPointViewModel>(StartPointProperty); }
			set { SetValue(StartPointProperty, value); }
		}

		/// <summary>		
		/// PointCollection property data.
		/// </summary>
		public static readonly PropertyData StartPointProperty = RegisterProperty(nameof(StartPoint), typeof(PolygonPointViewModel));

		public PolygonPointViewModel EndPoint
		{
			get { return GetValue<PolygonPointViewModel>(EndPointProperty); }
			set { SetValue(EndPointProperty, value); }
		}

		/// <summary>		
		/// PointCollection property data.
		/// </summary>
		public static readonly PropertyData EndPointProperty = RegisterProperty(nameof(EndPoint), typeof(PolygonPointViewModel));

		#endregion

		#region Public Model Properties

		/// <summary>
		/// Model representation of the line.
		/// </summary>
		public Line Line
		{
			get
			{
				return (Line)Shape;
			}
			set
			{
				Shape = value;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds the specified point to the point collection.
		/// </summary>
		/// <param name="position">Position of the new point</param>
		public override void AddPoint(Point position)
		{
			// Create the new model point
			PolygonPoint modelPoint = new PolygonPoint();

			// Add the model point to the model's point collection
			Line.Points.Add(modelPoint);

			// Create the new view model point
			PolygonPointViewModel viewModelPoint = new PolygonPointViewModel(modelPoint, this);

			// Initialize the position of the point
			viewModelPoint.X = position.X;
			viewModelPoint.Y = position.Y;

			// Add the point to the view model point collection
			PointCollection.Add(viewModelPoint);

			// If the line is complete then...
			if (PointCollection.Count == 2)
			{
				// Calculate the center of the line
				UpdateCenterPoint();

				// Color the first point green
				PointCollection[0].DeselectedColor = Colors.Lime;
				PointCollection[0].Color = PointCollection[0].DeselectedColor;

				// Make the line visible
				Visibility = true;
			}
		}

		/// <summary>
		/// Toggles the start point of a wipe line.
		/// </summary>
		public void ToggleStartPoint()
		{
			// Create temporary references to the view model points
			PolygonPointViewModel tempStart = StartPoint;
			PolygonPointViewModel tempEnd = EndPoint;

			// Create temporary references to the model points
			PolygonPoint startPoint = Line.Points[0];
			PolygonPoint endPoint = Line.Points[1];

			// Swap the colors on the points
			tempStart.DeselectedColor = Colors.DodgerBlue;
			tempEnd.DeselectedColor = Colors.Lime;

			// Swap the points on the view model
			PointCollection.Clear();
			StartPoint = tempEnd;
			EndPoint = tempStart;
			PointCollection.Add(StartPoint);
			PointCollection.Add(EndPoint);

			// Swap the points on the model
			Line.Points.Clear();
			Line.Points.Add(endPoint);
			Line.Points.Add(startPoint);

			// Fire the collection changed event			
			NotifyPointCollectionChanged();
		}

		#endregion

		#region Protected Methods

		/// <summary>
			/// Updates the center point of the line.
			/// </summary>
		protected override void UpdateCenterPoint()
		{
			// If the line contains two points then...
			if (PointCollection.Count == 2)
			{
				// Update the center point of the Line
				CenterPoint = GetCenterOfLine();
			}

			// Update the point labels
			UpdatePointLabels();			
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Calculates the center of the line.
		/// </summary>
		/// <returns>Center of the ;ome.</returns>
		private PolygonPointViewModel GetCenterOfLine()
		{			
			// Create a new polygon point view model object; not giving it a model object 
			PolygonPointViewModel centerPoint = new PolygonPointViewModel(null, null);

			// Calculate the center of the line
			centerPoint.X = (PointCollection[0].X + PointCollection[1].X) / 2.0;
			centerPoint.Y = (PointCollection[0].Y + PointCollection[1].Y) / 2.0;

			// Return the center of the line
			return centerPoint;
		}

		#endregion			
	}
}
