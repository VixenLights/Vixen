namespace VixenModules.Effect.State
{
	/// <summary>
	/// Specifies how the State effect schedules multiple active State item names.
	/// </summary>
	public enum PlaybackMode
	{
		/// <summary>
		/// Renders active State item names together for the selected duration.
		/// </summary>
		Default,

		/// <summary>
		/// Renders active State item names sequentially across the selected duration.
		/// </summary>
		Iterate
		// Future implementation
		//[Description("Count Down")]
		//CountDown,
		//[Description("Time Count Down")]
		//TimeCountDown,
		//Number
	}
}
