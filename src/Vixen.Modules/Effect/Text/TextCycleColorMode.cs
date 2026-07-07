namespace VixenModules.Effect.Text
{
	/// <summary>
	/// Specifies how the Text effect advances gradients when cycle color is enabled.
	/// </summary>
	public enum TextCycleColorMode
	{
		/// <summary>
		/// Advances gradients for each rendered character.
		/// </summary>
		Character,

		/// <summary>
		/// Advances gradients for each rendered word or marked word.
		/// </summary>
		Word
	}
}
