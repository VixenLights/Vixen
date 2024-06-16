using Catel.Data;
using Catel.MVVM;
using System.Windows;
using System.Windows.Media;
using VixenModules.App.Polygon;

namespace VixenModules.Editor.PolygonEditor.ViewModels
{
	/// <summary>
	/// Maintains a polygon point view model.
	/// </summary>
	public class PolygonPointViewModel : ViewModelBase
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>		
		public PolygonPointViewModel(PolygonPoint point, ShapeViewModel parent)
		{
			// Store off the model
			PolygonPoint = point;

			// Default the points to black
			Color = Colors.DodgerBlue;
			DeselectedColor = Colors.DodgerBlue;

			// Store off a reference to the parent view model
			Parent = parent;
		}

		#endregion

		#region Model Property

		/// <summary>
		/// Gets or sets the PolygonPoint value.
		/// </summary>
		[Model]
		public PolygonPoint PolygonPoint
		{
			get { return GetValue<PolygonPoint>(PolygonPointProperty); }
			private set { SetValue(PolygonPointProperty, value); }
		}

		/// <summary>
		/// Polygon Point model property data.
		/// </summary>
		public static readonly IPropertyData PolygonPointProperty = RegisterProperty<PolygonPoint>(nameof(PolygonPoint));

		#endregion

		#region Public Catel Properties

		/// <summary>
		/// Gets or sets the X position of the point.
		/// </summary>
		[ViewModelToModel("PolygonPoint")]
		public double X 
		{
			get { return GetValue<double>(XProperty); }
			set 
			{ 
				SetValue(XProperty, value);

				// If the point has a parent and change events are not being suppressed then...
				if (Parent != null && !SuppressChangeEvents)
				{
					// Notify the parent that a point changed
					Parent.NotifyPointCollectionChanged();
				}				
			}
		}
		
		/// <summary>
		/// X property data.
		/// </summary>
		public static readonly IPropertyData XProperty = RegisterProperty<double>(nameof(X));
		
		/// <summary>
		/// Gets or sets the Y position of the point.
		/// </summary>
		[ViewModelToModel("PolygonPoint")]
		public double Y
		{
			get { return GetValue<double>(YProperty); }
			set 
			{ 
				SetValue(YProperty, value);

				// If the point has a parent and change events are not being suppressed then...
				if (Parent != null && !SuppressChangeEvents)
				{
					// Notify the parent that a point changed
					Parent.NotifyPointCollectionChanged();
				}				
			}
		}

		/// <summary>
		/// Y property data.
		/// </summary>
		public static readonly IPropertyData YProperty = RegisterProperty<double>(nameof(Y));
		
		/// <summary>
		/// Gets or sets the label of the point.
		/// </summary>		
		public string Label
		{
			get { return GetValue<string>(LabelProperty); }
			set
			{
				SetValue(LabelProperty, value);				
			}
		}

		/// <summary>
		/// Label property data.
		/// </summary>
		public static readonly IPropertyData LabelProperty = RegisterProperty<string>(nameof(Label));

		/// <summary>
		/// True when the point has been selected.
		/// </summary>
		public bool Selected
		{
			get { return GetValue<bool>(SelectedProperty); }
			set 
			{ 
				SetValue(SelectedProperty, value); 

				// If the point is selected then...
				if (Selected)
				{
					// Change the point's color to the selection color
					Color = Colors.HotPink;
				}
				// Otherwise
				else
				{
					// Change the color the deselected color
					Color = DeselectedColor;
				}
			}
		}

		/// <summary>
		/// Selected property data.
		/// </summary>
		public static readonly IPropertyData SelectedProperty = RegisterProperty<bool>(nameof(Selected));

		/// <summary>
		/// Color of the center point hash.
		/// </summary>
		public Color Color
		{
			get { return GetValue<Color>(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}

		/// <summary>
		/// CenterPointColor property data.
		/// </summary>
		public static readonly IPropertyData ColorProperty = RegisterProperty<Color>(nameof(Color));

		#endregion

		#region Public Properties

		/// <summary>
		/// Color of the center point hash.
		/// </summary>
		public Color DeselectedColor { get; set; }
		
		/// <summary>
		/// Gets or sets the parent view model.
		/// </summary>
		public ShapeViewModel Parent { get; set; }

		/// <summary>
		/// .Net Point representation of the vertex.
		/// </summary>
		public Point Point
		{
			get 
			{ 
				return GetPoint();
			}
			set
			{
				// Store off the new point
				X = value.X;
				Y = value.Y;
			}
		}

		/// <summary>
		/// This property suppresses INotifyProperty change events
		/// to improve the performance of the editor.
		/// </summary>
		public bool SuppressChangeEvents { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Converts the polygon point into a .NET Point structure.
		/// </summary>
		/// <returns>Point data structure</returns>
		public Point GetPoint()
		{
			// Create a new .NET point
			Point point = new Point();

			// Initialize the point's coordinates
			point.X = X;
			point.Y = Y;

			// Return the .NET point
			return point;
		}

		#endregion
	}
}
