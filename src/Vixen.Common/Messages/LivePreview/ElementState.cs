namespace Common.Messages.LivePreview
{
	/// <summary>Represents the desired state for a single lighting element in a live preview operation.</summary>
	public sealed record ElementState
	{
		/// <summary>Gets or sets the unique identifier of the element node.</summary>
		public Guid Id { get; init; }

		/// <summary>Gets or sets the duration in seconds the element should remain on.</summary>
		public int Duration { get; init; }

		/// <summary>Gets or sets the HTML color string (e.g. <c>"#FF0000"</c>) for the element.</summary>
		public string Color { get; init; } = string.Empty;

		/// <summary>Gets or sets the intensity level from 0.0 (off) to 1.0 (full).</summary>
		public double Intensity { get; init; }
	}
}