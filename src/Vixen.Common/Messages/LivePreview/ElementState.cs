namespace Common.Messages.LivePreview
{
	/// <summary>Represents the desired state for a single lighting element in a live preview operation.</summary>
	public sealed record ElementState
	{
		/// <summary>Gets or sets the unique identifier of the element node.</summary>
		public Guid Id { get; init; }

		/// <summary>Gets or sets the duration in seconds the element should remain on. <c>0</c> means indefinite.</summary>
		public int Duration { get; init; }

		/// <summary>Gets or sets the HTML color string for the element.</summary>
		/// <remarks>Must be empty or a seven-character hex string in the format <c>#RRGGBB</c>.</remarks>
		/// <exception cref="ArgumentException">The value is not empty and is not a valid <c>#RRGGBB</c> hex string.</exception>
		public string Color
		{
			get;
			init
			{
				if (!string.IsNullOrEmpty(value) && (value.Length != 7 || !value.StartsWith('#')))
					throw new ArgumentException("Color must be empty or a hex string in the format #RRGGBB.",
						nameof(Color));
				field = value;
			}
		} = string.Empty;

		/// <summary>Gets or sets the intensity level from <c>0.0</c> (off) to <c>100.0</c> (full brightness).</summary>
		/// <exception cref="ArgumentOutOfRangeException">The value is less than 0.0 or greater than 100.0.</exception>
		public double Intensity
		{
			get;
			init
			{
				if (value < 0.0 || value > 100.0)
					throw new ArgumentOutOfRangeException(nameof(Intensity), value,
						"Intensity must be between 0.0 and 1.0.");
				field = value;
			}
		}
	}
}
