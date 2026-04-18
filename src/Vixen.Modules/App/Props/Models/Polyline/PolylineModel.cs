using System.Collections.ObjectModel;
using Vixen.Sys.Props.Model;

namespace VixenModules.App.Props.Models.Polyline
{
	/// <summary>
	/// Maintains a polyline prop visual model.
	/// </summary>
	public class PolylineModel : BaseLightModel, ISegmentable
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public PolylineModel() 
		{
			// Register for changes to the Segments collection
			Segments.CollectionChanged += SegmentsCollectionChanged;
		}
		
		#endregion

		#region ISegmentable

		private ObservableCollection<ISegment> _segments = new();
		
		/// <inheritdoc/>		
		public ObservableCollection<ISegment> Segments
		{
			get => _segments;
			set => SetProperty(ref _segments, value);
		}		

		#endregion

		#region Protected Methods
		
		/// <inheritdoc/>		
		protected override IEnumerable<NodePoint> Get3DNodePoints()
		{
			// Create the collection of nodes return value
			List<NodePoint> nodes = new();

			// TODO: Jeff would like the number of points to be proportional to the segment length
			const int NumberOfPoints = 10;

			// Loop over the segments that make up the prop
			foreach(PolylineSegment segment in Segments)
			{
				// Save off the start of the line
				float x = segment.StartX;
				float y = segment.StartY;

				// Calculate the space between the points
				float deltaX = (segment.EndX - segment.StartX) / NumberOfPoints;
				float deltaY = (segment.EndY - segment.StartY) / NumberOfPoints;

				// If this is the last segment of the polyline then...
				if (segment == Segments[Segments.Count - 1])
				{
					// Loop to create the points
					for (int i = 0; i <= NumberOfPoints; i++)
					{
						// Create the point in the X-Y plane
						NodePoint node = new(x, y, 0.0);

						// Add the node to the return collection
						nodes.Add(node);

						// Increment the x and y to the next point
						x += deltaX;
						y += deltaY;
					}
				}
				else
				{
					// Loop to create the points
					for (int i = 0; i < NumberOfPoints; i++)
					{
						// Create the point in the X-Y plane
						NodePoint node = new(x, y, 0.0);

						// Add the node to the return collection
						nodes.Add(node);

						// Increment the x and y to the next point
						x += deltaX;
						y += deltaY;
					}
				}
			}

			return nodes;
		}

		#endregion

		#region Private Methods
		
		/// <summary>
		/// Segment collection changed event handler.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void SegmentsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			// Update the nodes when segments are added or removed
			UpdateNodes();
		}

		#endregion		
	}
}
