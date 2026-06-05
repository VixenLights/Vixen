namespace Common.Messages.LivePreview
{
	/// <summary>Broadcast message that requests a live preview context be released.</summary>
	/// <remarks>
	/// Only contexts that were created through the Live Preview module will be released.
	/// Set <see cref="ContextName"/> to an empty string to release the default context.
	/// </remarks>
	public sealed record ReleaseContextMessage
	{
		/// <summary>
		/// Gets the name of the live context to release, or an empty string to release the default context.
		/// </summary>
		public required string ContextName { get; init; }
	}
}
