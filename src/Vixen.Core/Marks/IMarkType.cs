namespace Vixen.Marks
{
	/// <summary>
	/// Defines the semantic purpose of a mark collection.
	/// </summary>
	public enum MarkCollectionType
	{
		/// <summary>
		/// Represents a general-purpose mark collection.
		/// </summary>
		Generic = 0,

		/// <summary>
		/// Represents a phrase-level mark collection.
		/// </summary>
		Phrase = 1,

		/// <summary>
		/// Represents a word-level mark collection.
		/// </summary>
		Word = 2,

		/// <summary>
		/// Represents a phoneme-level mark collection.
		/// </summary>
		Phoneme = 3,

		/// <summary>
		/// Represents a state-label mark collection.
		/// </summary>
		State = 4,
	}
}
