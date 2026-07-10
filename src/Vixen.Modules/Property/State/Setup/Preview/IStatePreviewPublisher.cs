namespace VixenModules.Property.State.Setup.Preview
{
	/// <summary>
	/// Defines the State preview operations that can be published to Live Preview.
	/// </summary>
	internal interface IStatePreviewPublisher
	{
		/// <summary>
		/// Turns on the specified element and color pairs.
		/// </summary>
		/// <param name="pairs">The element and color pairs to activate.</param>
		void TurnOn(IReadOnlyCollection<StatePreviewPair> pairs);

		/// <summary>
		/// Turns off every active effect for the specified elements.
		/// </summary>
		/// <param name="elementIds">The identifiers of the elements to deactivate.</param>
		void TurnOff(IReadOnlyCollection<Guid> elementIds);

		/// <summary>
		/// Clears active effects while retaining the State preview context.
		/// </summary>
		void Clear();

		/// <summary>
		/// Releases the State preview context.
		/// </summary>
		void Release();
	}
}
