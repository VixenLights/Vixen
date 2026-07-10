namespace VixenModules.Property.State.Setup.Preview
{
	/// <summary>
	/// Represents one element and color pair that should be active in the State preview context.
	/// </summary>
	/// <param name="ElementId">The identifier of the previewed element.</param>
	/// <param name="Color">The HTML color string for the previewed element.</param>
	internal readonly record struct StatePreviewPair(Guid ElementId, string Color);
}
