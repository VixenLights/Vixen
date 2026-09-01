namespace Vixen.Marks
{
	/// <summary>
	/// Selects an appropriate mark collection identifier for an effect-local selection.
	/// </summary>
	/// <remarks>
	/// This service is stateless. It reads the supplied collection sequence without subscribing to or modifying it, and returns only a collection identifier for the owning effect to commit.
	/// </remarks>
	public sealed class MarkCollectionSelectionService
	{
		/// <summary>
		/// Normalizes a selection against the supplied collections in their existing order.
		/// </summary>
		/// <param name="markCollections">The ordered shared mark collections to inspect.</param>
		/// <param name="selection">The effect-local selection to evaluate.</param>
		/// <returns>The existing identifier when it is active and valid; otherwise, a preferred or fallback collection identifier, or <see cref="Guid.Empty" /> when no collection may be selected.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="markCollections" /> or <paramref name="selection" /> is <see langword="null" />.</exception>
		public Guid Normalize(IEnumerable<IMarkCollection> markCollections, IMarkCollectionSelection selection)
		{
			ArgumentNullException.ThrowIfNull(markCollections);
			ArgumentNullException.ThrowIfNull(selection);

			if (!selection.IsActive)
			{
				return selection.MarkCollectionId;
			}

			IMarkCollection firstCollection = null;
			IMarkCollection preferredCollection = null;
			foreach (var markCollection in markCollections)
			{
				firstCollection ??= markCollection;
				if (markCollection.Id == selection.MarkCollectionId && selection.MarkCollectionId != Guid.Empty)
				{
					return selection.MarkCollectionId;
				}

				if (preferredCollection == null && markCollection.CollectionType == selection.PreferredCollectionType)
				{
					preferredCollection = markCollection;
				}
			}

			if (preferredCollection != null)
			{
				return preferredCollection.Id;
			}

			return selection.AllowsFirstCollectionFallback ? firstCollection?.Id ?? Guid.Empty : Guid.Empty;
		}
	}
}
