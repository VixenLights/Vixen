namespace Common.Messages.LivePreview
{
	/// <summary>Broadcast message that requests a named live preview context be released.</summary>
	/// <remarks>
	/// Only contexts that were created through the Live Preview module will be released.
	/// The default context is managed by the module lifecycle and is not affected by this message.
	/// </remarks>
	public sealed record ReleaseContextMessage
	{
		/// <summary>Gets the name of the live context to release.</summary>
		public required string ContextName { get; init; }
	}
}
