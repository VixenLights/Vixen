namespace VixenModules.Editor.TimedSequenceEditor
{
	public sealed class PropertyOwnerMetaData(object owner, int collectionIndex = -1)
	{
		/// <summary>
		/// This is the associated owner of the property, this is used to group properties by owner and to display the owner name in the UI when needed.
		/// </summary>
		public object Owner { get; init; } = owner;

		/// <summary>
		/// Specifies if the Owner is part of a collection, if true the Owner is a child of a collection and the CollectionIndex is the index of the owner in the collection.
		/// </summary>
		public bool IsCollectionChild { get; init; } = collectionIndex > -1;

		/// <summary>
		/// If the Owner is part of a collection, <see cref="IsCollectionChild"/> this is the Owners index in the collection.
		/// </summary>
		public int CollectionIndex { get; init; } = collectionIndex;

		public string OwnerDisplayName => PropertyDiscovery.GetDisplayName(Owner.GetType());
	}
}