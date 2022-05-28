using System;

namespace VixenModules.Effect.Effect
{
	/// <summary>
	/// Provides type information about the items contained in a collection.
	/// </summary>
	public interface IDiscoverCollectionItemType
	{
		/// <summary>
		/// Returns the concrete type of the items in the collection.
		/// </summary>
		/// <returns>Type of the items contained in a collection</returns>
		Type GetItemType();
	}
}
