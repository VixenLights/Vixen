using Catel.Data;
using Catel.MVVM;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using VixenModules.App.Polygon;

namespace VixenModules.Editor.PolygonEditor.ViewModels
{
	/// <summary>
	/// Maintains a polygon snapshot view model.  
	/// This view model maintains a polygon/line/ellipse reference and a time position within the effect.
	/// </summary>
	public class PolygonSnapshotViewModel : ViewModelBase
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public PolygonSnapshotViewModel()
		{
			PointCollection = new ObservableCollection<PolygonPointViewModel>();

			// Initialize the color Charcoal
			_charcoal = Color.FromRgb(68, 68, 68);

			// Default the color to charcoal
			Color = _charcoal;
		}

		#endregion

		#region Private Fields

		/// <summary>
		/// Defines the color charcoal.
		/// </summary>
		private Color _charcoal;

		#endregion

		#region Public Constants

		/// <summary>
		/// Defines the half width of the timer bar pointer.
		/// </summary>
		public const int HalfWidth = 4;

		#endregion

		#region Public Catel Properties

		/// <summary>
		/// Collection of polygon points.
		/// </summary>
		public double Time
		{
			get { return GetValue<double>(TimeProperty); }
			set { SetValue(TimeProperty, value); }
		}

		/// <summary>		
		/// PointCollection property data.
		/// </summary>
		public static readonly PropertyData TimeProperty = RegisterProperty(nameof(Time), typeof(double));

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

		/// <summary>
		/// Color of the center point hash.
		/// </summary>
		public Color Color
		{
			get { return GetValue<Color>(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}

		/// <summary>
		/// Color property data.
		/// </summary>
		public static readonly PropertyData ColorProperty = RegisterProperty(nameof(Color), typeof(Color), null);

		/// <summary>
		/// Whether the polygon snapshot is selected.
		/// </summary>
		public bool Selected
		{
			get { return GetValue<bool>(SelectedProperty); }
			set
			{
				SetValue(SelectedProperty, value);
				
				// If the time bar arrow is selected then...
				if (Selected)
				{					
					// Set the selected color to hot pink
					Color = Colors.HotPink;											
				}
				// Otherwise...
				else
				{
					// Set the color back to charcoal
					Color = _charcoal;
				}
			}
		}

		/// <summary>
		/// Selected property data.
		/// </summary>
		public static readonly PropertyData SelectedProperty = RegisterProperty(nameof(Selected), typeof(bool), null);

		#endregion

		#region Public Properties

		/// <summary>
		/// Time of the snapshot polygon normalized to (0.0 - 1.0).
		/// </summary>
		public double NormalizedTime { get; set; }

		/// <summary>
		/// Associated polygon view model.
		/// </summary>
		public PolygonViewModel PolygonViewModel { get; set; }

		/// <summary>
		/// Associated line view model.
		/// </summary>
		public LineViewModel LineViewModel { get; set; }

		/// <summary>
		/// Associated ellipse view model.
		/// </summary>
		public EllipseViewModel EllipseViewModel { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns true if the mouse is over the time bar.
		/// </summary>
		/// <param name="mousePosition">Position of mouse</param>
		/// <returns>True if the mouse is over the time bar.</returns>
		public bool IsMouseOverTimeBar(Point mousePosition)
		{
			// Default to not over the time bar
			bool isMouseOverTimeBar = false;

			// Get the X position of the left side of the time bar
			double xLeft = PointCollection[0].X;

			// Get the X position of the right side of the time bar
			double xRight = PointCollection[2].X;

			// If the mouse X position between the two points then...
			if (mousePosition.X >= xLeft &&
				mousePosition.X <= xRight)
			{
				// Indicate the mouse is over the time bar
				isMouseOverTimeBar = true;
			}

			// Return whether the mouse is over the time bar
			return isMouseOverTimeBar;
		}
		
		/// <summary>
		/// Moves the time bar to the specified x position.
		/// </summary>
		/// <param name="x">Position to move the time bar too</param>
		public void Move(int x) 
		{
			// If the X position is off the scale to the left then...
			if (x < HalfWidth)
			{
				// Put the position at the zero position
				x = HalfWidth;
			}

			// Set the time to the new position					
			Time = x;
			
			// Update the points of the time bar pointer
			PointCollection[0].X = x - HalfWidth;
			PointCollection[1].X = x;
			PointCollection[2].X = x + HalfWidth;
			PointCollection[3].X = x + HalfWidth;
			PointCollection[4].X = x - HalfWidth;
			PointCollection[5].X = x - HalfWidth;

			// Refresh the snapshot pointers
			NotifyPointCollectionChanged();
		}

		/// <summary>
		/// Intialize the snapshot time bar pointer at the specified position.
		/// </summary>
		/// <param name="position">Initial position of the time bar pointer</param>
		public void Initialize(int position)
		{
			// Store off the position
			Time = position;

			const int HeightOfPointer = 25;
			const int HeightOfRectangle = 20;

			//
			// Initialize the time bar points
			//
			
			// Bottom Left Corner
			PolygonPointViewModel p1 = new PolygonPointViewModel(new PolygonPoint(), null);
			p1.X = Time - HalfWidth;
			p1.Y = HeightOfRectangle;

			// Center Pointer
			PolygonPointViewModel p2 = new PolygonPointViewModel(new PolygonPoint(), null);
			p2.X = Time;
			p2.Y = HeightOfPointer;

			// Bottom Right Corner
			PolygonPointViewModel p3 = new PolygonPointViewModel(new PolygonPoint(), null);
			p3.X = Time + HalfWidth;
			p3.Y = HeightOfRectangle;

			// Top Right Corner
			PolygonPointViewModel p4 = new PolygonPointViewModel(new PolygonPoint(), null);
			p4.X = Time + HalfWidth;
			p4.Y = 0; 

			// Top Left Corner
			PolygonPointViewModel p5 = new PolygonPointViewModel(new PolygonPoint(), null);
			p5.X = Time - HalfWidth;
			p5.Y = 0; 

			// Add the points to the point collection	
			PointCollection.Add(p1);
			PointCollection.Add(p2);
			PointCollection.Add(p3);
			PointCollection.Add(p4);
			PointCollection.Add(p5);
			PointCollection.Add(p1);
		}
		
		/// <summary>
		/// Raises the property change event for the PointCollection property.		
		/// </summary>
		/// <remarks>This method is needed to trigger a converter in the view.</remarks>
		public void NotifyPointCollectionChanged()
		{
			// Notify the view that the points have changed
			RaisePropertyChanged(nameof(PointCollection));			
		}
		
		#endregion
	}
}
