using Catel.Data;
using System.Collections.ObjectModel;

namespace VixenModules.Editor.PolygonEditor.ViewModels
{
	/// <summary>
	/// Maintains a shape that based on points.
	/// </summary>
	public abstract class PointBasedViewModel : ShapeViewModel
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public PointBasedViewModel(bool labelVisible) :
			base(labelVisible)
		{
			// Initialize the segments to visible
			SegmentsVisible = true;

			// Create a collection of line segments 
			// Some shapes like the polygon use the line segments to draw the shape until the polygon is closed.
			Segments = new ObservableCollection<LineSegmentViewModel>();
		}

		#endregion

		#region Public Catel Properties

		/// <summary>
		/// Controls whether the segments are visible. 
		/// </summary>
		/// <remarks>For polygons the segment lines define the polygon until the polygon has been closed.</remarks>
		public bool SegmentsVisible
		{
			get { return GetValue<bool>(SegmentsVisibleProperty); }
			set { SetValue(SegmentsVisibleProperty, value); }
		}

		/// <summary>
		/// SegmentsVisible property data.
		/// </summary>
		public static readonly PropertyData SegmentsVisibleProperty = RegisterProperty(nameof(SegmentsVisible), typeof(bool), null);

		/// <summary>
		/// Maintains a collection of line segments.  The line segments help define the polygon until the polygon has been closed.
		/// </summary>
		public ObservableCollection<LineSegmentViewModel> Segments
		{
			get { return GetValue<ObservableCollection<LineSegmentViewModel>>(SegmentsProperty); }
			private set { SetValue(SegmentsProperty, value); }
		}

		/// <summary>
		/// SegmentsVisible property data.
		/// </summary>
		public static readonly PropertyData SegmentsProperty =
			RegisterProperty(nameof(Segments), typeof(ObservableCollection<LineSegmentViewModel>), null);

		#endregion

		#region Public Methods

		/// <summary>
		/// Raises the property change event for the PointCollection and Segments properties.		
		/// </summary>
		/// <remarks>This method is needed to trigger a converter in the view.</remarks>
		public override void NotifyPointCollectionChanged()
		{
			// Notify the view that the segments have changed
			RaisePropertyChanged(nameof(Segments));

			// Call the base class implementation
			base.NotifyPointCollectionChanged();
		}

		#endregion
	}
}
