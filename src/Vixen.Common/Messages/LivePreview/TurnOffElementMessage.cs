namespace Common.Messages.LivePreview
{
	/// <summary>Broadcast message that requests a single element be turned off in a live preview context.</summary>
	public sealed record TurnOffElementMessage
	{
		/// <summary>Gets the unique identifier of the element node to turn off.</summary>
		public Guid ElementId { get; init; }

		/// <summary>Gets the name of the target live context, or <see langword="null"/> to use the default context.</summary>
		public string? ContextName { get; init; }
	}
}