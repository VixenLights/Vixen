using System;

namespace VixenModules.Effect.Effect
{
	/// <summary>
	/// Maintains a collection of ExpandoObjects.
	/// </summary>
	/// <typeparam name="TInterface">Collection Item Type</typeparam>
	public abstract class ExpandoObjectObservableCollection<TInterface, TImpl> : NotifyPropertyObservableCollection<TInterface>, IExpandoObjectCollection		
		where TImpl : class, new()
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="propertyName">Name of the collection for child property change events</param>
		public ExpandoObjectObservableCollection(string propertyName) : base(propertyName)
		{
		}

		#endregion
		
		#region IExpandoObjectCollection

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public Type GetItemType()
		{
			// Return the type of the item being maintained by the collection
			return typeof(TImpl);
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public virtual int GetMinimumItemCount()
		{
			// The default is to always keep one item in the collection
			return 1;
		}

		#endregion
	}
}
