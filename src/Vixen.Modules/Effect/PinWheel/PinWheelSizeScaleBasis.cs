namespace VixenModules.Effect.PinWheel
{
	/// <summary>
	/// Specifies the virtual-buffer dimension used to scale a PinWheel's size.
	/// </summary>
	public enum PinWheelSizeScaleBasis
	{
		/// <summary>
		/// Scales size by the virtual-buffer height. This is the compatibility value for effects serialized before VIX-3989.
		/// </summary>
		Height = 0,

		/// <summary>
		/// Scales size by the virtual-buffer width.
		/// </summary>
		Width = 1,

		/// <summary>
		/// Scales size by the larger virtual-buffer dimension.
		/// </summary>
		LargestDimension = 2
	}
}
