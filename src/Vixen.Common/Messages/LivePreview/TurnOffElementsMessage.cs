namespace Common.Messages.LivePreview
{
	/// <summary>Broadcast message that requests multiple elements be turned off in a live preview context.</summary>
	public sealed record TurnOffElementsMessage
	{
		/// <summary>Gets the desired states identifying the elements to turn off.</summary>
		public IReadOnlyList<ElementState> States { get; init; } = [];

		/// <summary>Gets the name of the target live context, or <see langword="null"/> to use the default context.</summary>
		public string? ContextName { get; init; }
	}
}