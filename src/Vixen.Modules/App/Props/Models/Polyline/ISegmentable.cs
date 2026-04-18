using System.Collections.ObjectModel;

namespace VixenModules.App.Props.Models.Polyline
{
	/// <summary>
	/// Maintains a segmentable prop model.
	/// </summary>
	interface ISegmentable
	{
		/// <summary>
		/// Collection of segments.
		/// </summary>
		ObservableCollection<ISegment> Segments { get; set; }
	}
}
