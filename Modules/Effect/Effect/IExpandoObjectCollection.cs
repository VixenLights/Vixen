using System;

namespace VixenModules.Effect.Effect
{
	/// <summary>
	/// Provides type information about the items contained in a collection.
	/// </summary>
	public interface IExpandoObjectCollection
	{
		/// <summary>
		/// Returns the concrete type of the items in the collection.
		/// </summary>
		/// <returns>Type of the items contained in a collection</returns>
		Type GetItemType();

		/// <summary>
		/// Returns the minimum number items allowed in the collection.
		/// </summary>
		/// <returns>Minimum number of items allowed in the collection.</returns>
		int GetMinimumItemCount();
	}
}
