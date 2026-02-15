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

		/// <summary>
		/// Gets the display name of the owner, providing a user-friendly name based on the owner's type when appropriate.
		/// </summary>
		/// <remarks>If the owner's string representation matches its type name, this property returns a descriptive
		/// display name using the PropertyDiscovery class. Otherwise, it returns the owner's string representation. This can
		/// be useful for displaying owner information in UI elements or logs.</remarks>
		public string OwnerDisplayName
		{
			get
			{
				var name = Owner.ToString();
				return name != null && name.Equals(Owner.GetType().ToString())?PropertyDiscovery.GetDisplayName(Owner.GetType()):Owner.ToString();
			}
		}
	}
}