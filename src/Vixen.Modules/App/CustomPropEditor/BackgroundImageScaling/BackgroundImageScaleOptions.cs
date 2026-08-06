namespace VixenModules.App.CustomPropEditor.BackgroundImageScaling
{
	/// <summary>
	/// Represents a validated logical canvas size and coordinate-scaling preference.
	/// </summary>
	/// <param name="targetWidth">The target logical canvas width in editor pixels.</param>
	/// <param name="targetHeight">The target logical canvas height in editor pixels.</param>
	/// <param name="scaleExistingLightPositions"><see langword="true" /> to scale existing light centers; otherwise, <see langword="false" />.</param>
	internal sealed record BackgroundImageScaleOptions(
		int TargetWidth,
		int TargetHeight,
		bool ScaleExistingLightPositions);
}
