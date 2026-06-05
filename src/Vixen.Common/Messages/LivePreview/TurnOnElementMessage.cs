namespace Common.Messages.LivePreview
{
	/// <summary>Broadcast message that requests a single element be turned on in a live preview context.</summary>
	public sealed record TurnOnElementMessage
	{
		/// <summary>Gets the desired state for the element.</summary>
		public ElementState State { get; init; } = new();

		/// <summary>Gets the name of the target live context, or <see langword="null"/> to use the default context.</summary>
		public string? ContextName { get; init; }
	}
}