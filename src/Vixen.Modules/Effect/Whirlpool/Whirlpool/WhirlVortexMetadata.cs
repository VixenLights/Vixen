namespace VixenModules.Effect.Whirlpool
{
	/// <summary>
	/// Maintains data about the end of the whirlpool.
	/// This meta-data supports drawing a whirlpool starting in the center and drawing out.
	/// </summary>
	public class WhirlVortexMetadata
	{
		/// <summary>
		/// Keeps track of the X position of the last whirl.
		/// This field is used to draw a whirl that expands out.
		/// </summary>
		public int LastX { get; set; }

		/// <summary>
		/// Keeps track of the Y position of the last whirl.
		/// This field is used to draw a whirl that expands out.
		/// </summary>
		public int LastY { get; set; }

		/// <summary>
		/// Keeps track of the width of the last whirl.
		/// This field is used to draw a whirl that expands out.
		/// </summary>
		public int LastWidth { get; set; }

		/// <summary>
		/// Keeps track of the height of the last whirl.
		/// This field is used to draw a whirl that expands out.
		/// </summary>
		public int LastHeight { get; set; }

		/// <summary>
		/// Flag that indicates if the top side of the last whirl was drawn.
		/// This field is used to draw a whirl that expands out.
		/// </summary>
		public bool DrawTop { get; set; }

		/// <summary>
		/// Flag that indicates if the right side of the last whirl was drawn.
		/// This field is used to draw a whirl that expands out.
		/// </summary>
		public bool DrawRight { get; set; }

		/// <summary>
		/// Flag that indicates if the left side of the last whirl was drawn.
		/// This field is used to draw a whirl that expands out.
		/// </summary>
		public bool DrawLeft { get; set; }

		/// <summary>
		/// Flag that indicates if the bottom side of the last whirl was drawn.
		/// This field is used to draw a whirl that expands out.
		/// </summary>
		public bool DrawBottom { get; set; }
	}
}
