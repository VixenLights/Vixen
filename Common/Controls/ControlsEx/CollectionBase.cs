using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Collections;

namespace CommonElements.ControlsEx
{
	/// <summary>
	/// generic-reimplementation of the System.Collections.CollectionBase
	/// </summary>
	/// <typeparam name="Type">type of item</typeparam>
	[Serializable, ComVisible(true)]
	public abstract class CollectionBase<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
	{
		private List<T> _list;

		public CollectionBase()
		{
			_list = new List<T>();
		}
		public CollectionBase(int capacity)
		{
			_list = new List<T>(capacity);
		}
		#region virtuals
		protected virtual void OnClear()
		{
		}

		protected virtual void OnClearComplete()
		{
		}

		protected virtual void OnInsert(int index, T value)
		{
		}

		protected virtual void OnInsertComplete(int index, T value)
		{
		}

		protected virtual void OnRemove(int index, T value)
		{
		}

		protected virtual void OnRemoveComplete(int index, T value)
		{
		}

		protected virtual void OnSet(int index, T oldValue, T newValue)
		{
		}

		protected virtual void OnSetComplete(int index, T oldValue, T newValue)
		{
		}

		protected virtual void OnValidate(T value)
		{

		}
		#endregion

		#region IList<Type> Member

		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			if (index < 0 || index > _list.Count)
				throw new ArgumentOutOfRangeException("index");
			OnValidate(item);
			OnInsert(index, item);
			_list.Insert(index, item);
			try { OnInsertComplete(index, item); }
			catch { _list.RemoveAt(index); throw; }
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= _list.Count)
				throw new ArgumentOutOfRangeException("index");
			T oldvalue = _list[index];
			OnValidate(oldvalue);
			OnRemove(index, oldvalue);
			_list.RemoveAt(index);
			try { OnRemoveComplete(index, oldvalue); }
			catch { _list.Insert(index, oldvalue); throw; }
		}

		public T this[int index]
		{
			get
			{
				return _list[index];
			}
			set
			{
				if (index < 0 || index >= _list.Count)
					throw new ArgumentOutOfRangeException("index");
				OnValidate(value);
				T olditem = _list[index];
				OnSet(index, olditem, value);
				_list[index] = value;
				//
				try { OnSetComplete(index, olditem, value); }
				catch { _list[index] = olditem; }
			}
		}

		#endregion

		#region ICollection<Type> Member

		public void Add(T item)
		{
			OnValidate(item);
			int index = _list.Count;
			OnInsert(index, item);
			_list.Insert(index, item);
			try
			{
				OnInsertComplete(index, item);
			}
			catch
			{
				_list.RemoveAt(index);
				throw;
			}
		}

		public void Clear()
		{
			OnClear();
			_list.Clear();
			OnClearComplete();
		}

		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _list.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			OnValidate(item);
			int index = this.InnerList.IndexOf(item);
			if (index < 0)
				return false;
			OnRemove(index, item);
			_list.RemoveAt(index);
			try
			{
				OnRemoveComplete(index, item);
			}
			catch
			{
				_list.Insert(index, item);
				throw;
			}
			return true;
		}

		#endregion

		#region IEnumerable<Type> Member

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		#endregion

		#region IEnumerable Member

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		#endregion
		private bool IsCompatible(object value)
		{
			return (value is T) || ((value == null) && !typeof(T).IsValueType);
		}
		private void Verify(object value)
		{
			if (!IsCompatible(value))
				throw new ArgumentException("wrong collection item type");
		}

		#region IList Member
		int IList.Add(object item)
		{
			Verify(item);
			this.Add((T)item);
			return (this.Count - 1);
		}
		bool IList.Contains(object item)
		{
			return (IsCompatible(item) && this.Contains((T)item));
		}
		int IList.IndexOf(object item)
		{
			if (!IsCompatible(item))
				return -1;
			return this.IndexOf((T)item);
		}
		void IList.Insert(int index, object item)
		{
			Verify(item);
			this.Insert(index, (T)item);
		}
		void IList.Remove(object item)
		{
			if (IsCompatible(item))
				this.Remove((T)item);
		}
		object IList.this[int index]
		{
			get { return this[index]; }
			set
			{
				Verify(value);
				this[index] = (T)value;
			}
		}
		bool IList.IsReadOnly
		{
			get { return false; }
		}
		bool IList.IsFixedSize
		{
			get { return false; }
		}
		#endregion
		#region ICollection Member
		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			((ICollection)_list).CopyTo(array, arrayIndex);
		}
		object ICollection.SyncRoot
		{
			get { return ((ICollection)_list).SyncRoot; }
		}
		bool ICollection.IsSynchronized
		{
			get { return false; }
		}
		#endregion

		#region properties
		protected List<T> InnerList
		{
			get
			{
				return _list;
			}
		}

		protected IList<T> List
		{
			get
			{
				return this;
			}
		}
		#endregion
	}
	/// <summary>
	/// generic collectionbase including an owner
	/// </summary>
	/// <typeparam name="TOwner">type of owner</typeparam>
	/// <typeparam name="TItem">type of item</typeparam>
	public class CollectionBase<TOwner, TItem> : CollectionBase<TItem>
	{
		#region variables
		private TOwner _owner;
		#endregion
		public CollectionBase(TOwner owner)
		{
			if (owner == null)
				throw new ArgumentNullException("owner");
			_owner = owner;
		}

		protected TOwner Owner
		{
			get { return _owner; }
		}
	}
}
