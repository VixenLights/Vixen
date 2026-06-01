namespace Common.Messages.LivePreview
{
	/// <summary>Defines the broadcast channel names used by the Live Preview messaging system.</summary>
	public static class LivePreviewChannels
	{
		/// <summary>Channel for <see cref="TurnOnElementMessage"/> messages.</summary>
		public const string TurnOnElement = "LivePreview.TurnOnElement";

		/// <summary>Channel for <see cref="TurnOnElementsMessage"/> messages.</summary>
		public const string TurnOnElements = "LivePreview.TurnOnElements";

		/// <summary>Channel for <see cref="TurnOffElementMessage"/> messages.</summary>
		public const string TurnOffElement = "LivePreview.TurnOffElement";

		/// <summary>Channel for <see cref="TurnOffElementsMessage"/> messages.</summary>
		public const string TurnOffElements = "LivePreview.TurnOffElements";

		/// <summary>Channel for <see cref="ClearActiveEffectsMessage"/> messages.</summary>
		public const string ClearActiveEffects = "LivePreview.ClearActiveEffects";
	}
}