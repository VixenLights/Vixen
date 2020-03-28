using Catel.Data;
using Catel.MVVM;
using System.Windows;
using System.Windows.Media;
using VixenModules.App.Polygon;

namespace PolygonEditor
{
	/// <summary>
	/// Maintains the a polygon point view model.
	/// </summary>
	public class PolygonPointViewModel : ViewModelBase
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>		
		public PolygonPointViewModel(PolygonPoint point, PolygonViewModel parent)
		{
			// Store off the model
			PolygonPoint = point;

			// Default the points to black
			Color = Colors.Black;

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
		public static readonly PropertyData PolygonPointProperty = RegisterProperty("PolygonPoint", typeof(PolygonPoint));

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
				if (Parent != null)
				{
					Parent.NotifyPointCollectionChanged();
				}				
			}
		}
		
		/// <summary>
		/// X property data.
		/// </summary>
		public static readonly PropertyData XProperty = RegisterProperty("X", typeof(double), null);
		
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
				if (Parent != null)
				{
					Parent.NotifyPointCollectionChanged();
				}				
			}
		}

		/// <summary>
		/// X property data.
		/// </summary>
		public static readonly PropertyData YProperty = RegisterProperty("Y", typeof(double), null);
		
		/// <summary>
		/// True when the point has been selected by the user.
		/// </summary>
		public bool Selected
		{
			get { return GetValue<bool>(SelectedProperty); }
			set 
			{ 
				SetValue(SelectedProperty, value); 
				if (Selected)
				{
					Color = Colors.HotPink;
				}
				else
				{
					Color = Colors.Black;
				}
			}
		}

		/// <summary>
		/// Selected property data.
		/// </summary>
		public static readonly PropertyData SelectedProperty = RegisterProperty("Selected", typeof(bool), null);

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
		public static readonly PropertyData ColorProperty = RegisterProperty("Color", typeof(Color), null);

		#endregion

		#region Public Properties
		
		/// <summary>
		/// Gets or sets the parent view model.
		/// </summary>
		public PolygonViewModel Parent { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Converts the polygon point into a .NET Point structure.
		/// </summary>
		/// <returns>Point data structure</returns>
		public Point GetPoint()
		{
			Point point = new Point();
			point.X = X;
			point.Y = Y;

			return point;
		}

		#endregion
	}
}
