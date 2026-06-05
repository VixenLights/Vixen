namespace Common.Messages.LivePreview
{
	/// <summary>Broadcast message that requests all active effects be cleared from a live preview context.</summary>
	public sealed record ClearActiveEffectsMessage
	{
		/// <summary>Gets the name of the target live context, or <see langword="null"/> to use the default context.</summary>
		public string? ContextName { get; init; }
	}
}