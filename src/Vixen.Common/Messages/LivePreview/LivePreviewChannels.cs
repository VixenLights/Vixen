namespace Common.Messages.LivePreview
{
	/// <summary>Defines the typed broadcast channels used by the Live Preview messaging system.</summary>
	/// <remarks>
	/// Each channel is a <see cref="BroadcastChannel{TMessage}"/> that binds a channel name to its
	/// message type at the declaration site, preventing mismatched publish or subscribe calls.
	/// </remarks>
	public static class LivePreviewChannels
	{
		/// <summary>Channel for <see cref="TurnOnElementMessage"/> messages.</summary>
		public static readonly BroadcastChannel<TurnOnElementMessage> TurnOnElement = new("LivePreview.TurnOnElement");

		/// <summary>Channel for <see cref="TurnOnElementsMessage"/> messages.</summary>
		public static readonly BroadcastChannel<TurnOnElementsMessage> TurnOnElements = new("LivePreview.TurnOnElements");

		/// <summary>Channel for <see cref="TurnOffElementMessage"/> messages.</summary>
		public static readonly BroadcastChannel<TurnOffElementMessage> TurnOffElement = new("LivePreview.TurnOffElement");

		/// <summary>Channel for <see cref="TurnOffElementsMessage"/> messages.</summary>
		public static readonly BroadcastChannel<TurnOffElementsMessage> TurnOffElements = new("LivePreview.TurnOffElements");

		/// <summary>Channel for <see cref="ClearActiveEffectsMessage"/> messages.</summary>
		public static readonly BroadcastChannel<ClearActiveEffectsMessage> ClearActiveEffects = new("LivePreview.ClearActiveEffects");

		/// <summary>Channel for <see cref="ReleaseContextMessage"/> messages.</summary>
		public static readonly BroadcastChannel<ReleaseContextMessage> ReleaseContext = new("LivePreview.ReleaseContext");
	}
}
