using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace VixenModules.Effect.Liquid
{
	/// <summary>
	/// Collection that fires an event when any property of the items in the collection changes.	
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class NotifyPropertyObservableCollection<T> : ObservableCollection<T>	
	{
		#region Protected Methods

		/// <summary>
		/// Refer to MSDN documentation.  Virtual method for when the observable collection changes.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (T addedItem in e.NewItems)
					{
						ProcessItemAdded(addedItem);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach(T removedItem in e.OldItems)
					{
						ProcessItemRemoved(removedItem);
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					NotifyChildPropertyChanged(this, "Emitters");
					break;
				case NotifyCollectionChangedAction.Reset:
					break;
				case NotifyCollectionChangedAction.Move:
					break;
				default:
					System.Diagnostics.Debug.Assert(false, "Unsupported NotifyCollectionChangeAction enumeration");
					break;					
			}
		}

		/// <summary>
		/// Method to handle when an item is added.
		/// </summary>		
		protected void ProcessItemAdded(T addedItem)
		{
			// Registers for the property changed of the new item
			((INotifyPropertyChanged)addedItem).PropertyChanged += ChildPropertyChangedEventHandler;
		}

		/// <summary>
		/// Method to handle when an item is removed.
		/// </summary>		
		protected void ProcessItemRemoved(T removedItem)
		{
			// Unregisters for the property changed of the removed item
			((INotifyPropertyChanged)removedItem).PropertyChanged -= ChildPropertyChangedEventHandler;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Property changed event handler.
		/// </summary>		
		private void ChildPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
		{
			// Fires the child property changed event
			NotifyChildPropertyChanged(sender, e.PropertyName);
		}

		/// <summary>
		/// Fires the child property changed event.
		/// </summary>		
		private void NotifyChildPropertyChanged(object sender, String propertyName)
		{
			// If there are any handlers registered then... 
			if (ChildPropertyChanged != null)
			{
				// Fire the child property changed event
				ChildPropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		#region Public Events

		public event PropertyChangedEventHandler ChildPropertyChanged;
				
		#endregion
	}
}
