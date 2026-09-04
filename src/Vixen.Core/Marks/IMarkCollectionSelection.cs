namespace Vixen.Marks
{
	/// <summary>
	/// Represents an effect-local mark collection selection that can be normalized against a sequence's collections.
	/// </summary>
	/// <remarks>
	/// Implementations own only effect or child-model state. They must not modify the shared <see cref="IMarkCollection" /> instances supplied to the selection service.
	/// </remarks>
	public interface IMarkCollectionSelection
	{
		/// <summary>
		/// Gets a value that indicates whether this selection participates in collection normalization.
		/// </summary>
		/// <value><see langword="true" /> if the effect is currently using this selection; otherwise, <see langword="false" />.</value>
		bool IsActive { get; }

		/// <summary>
		/// Gets or sets the identifier of the selected mark collection.
		/// </summary>
		/// <value>The identifier stored by the owning effect or child model, or <see cref="Guid.Empty" /> when no collection is selected.</value>
		Guid MarkCollectionId { get; set; }

		/// <summary>
		/// Gets the collection type preferred when the current selection is missing.
		/// </summary>
		/// <value>The preferred collection type, or <see langword="null" /> when no type is preferred.</value>
		MarkCollectionType? PreferredCollectionType { get; }

		/// <summary>
		/// Gets a value that indicates whether the first collection may be used when no preferred collection is available.
		/// </summary>
		/// <value><see langword="true" /> to select the first collection when needed; otherwise, <see langword="false" />.</value>
		bool AllowsFirstCollectionFallback { get; }
	}
}
