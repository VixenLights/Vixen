namespace VixenModules.App.Props.Models.Polyline
{
	/// <summary>
	/// Maintains a polyline segment.
	/// </summary>
	public class PolylineSegment : ISegment
	{
		#region ISegment
				
		/// <inheritdoc/>		
		public float StartX { get; set; }

		/// <inheritdoc/>		
		public float StartY { get; set; }

		/// <inheritdoc/>		
		public float EndX { get; set; }

		/// <inheritdoc/>		
		public float EndY { get; set; }

		#endregion
	}
}
