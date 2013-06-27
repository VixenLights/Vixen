using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Common.Controls.ControlsEx
{
	/// <summary>
	/// wrapper list for lists, which elements super the type of another list.
	/// therefore it is not readable!!!
	/// = java List( ? super Type )
	/// = contravariant
	/// </summary>
	/// <typeparam name="Base">BaseType</typeparam>
	/// <typeparam name="Type">Type</typeparam>
	public class SuperList<B, T> : IList<T> where T : B
	{
		private IList<B> _inner;

		public SuperList(IList<B> inner)
		{
			if (inner == null)
				throw new ArgumentNullException("inner");
			_inner = inner;
		}

		#region IList<Type> Member

		public int IndexOf(T item)
		{
			return _inner.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_inner.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_inner.RemoveAt(index);
		}

		public T this[int index]
		{
			get { throw new NotImplementedException("contravariant generic class, not readable"); }
			set { _inner[index] = value; }
		}

		#endregion

		#region ICollection<Type> Member

		public void Add(T item)
		{
			_inner.Add(item);
		}

		public void Clear()
		{
			_inner.Clear();
		}

		public bool Contains(T item)
		{
			return _inner.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException("contravariant generic class, not readable");
		}

		public int Count
		{
			get { return _inner.Count; }
		}

		public bool IsReadOnly
		{
			get { return _inner.IsReadOnly; }
		}

		public bool Remove(T item)
		{
			return _inner.Remove(item);
		}

		#endregion

		#region IEnumerable<Type> Member

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException("contravariant generic class, not readable");
		}

		#endregion

		#region IEnumerable Member

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _inner.GetEnumerator();
		}

		#endregion
	}

	/// <summary>
	/// wrapper list for lists, which elements extend the type of another list.
	/// therefore it is not writable!!!
	/// = java List ( ? extends Base )
	/// = covariant
	/// </summary>
	/// <typeparam name="Base">BaseType</typeparam>
	/// <typeparam name="Type">Type</typeparam>
	public class ExtendsList<B, T> : IList<B> where T : B
	{
		#region types

		/// <summary>
		/// wraps a non generic enumerator into a generic one
		/// </summary>
		private class ExtendsEnumerator<Base, Type> : IEnumerator<Base> where Type : Base
		{
			private IEnumerator<Type> _inner;

			public ExtendsEnumerator(IEnumerator<Type> inner)
			{
				if (inner == null)
					throw new ArgumentNullException("inner");
				_inner = inner;
			}

			public Base Current
			{
				get { return _inner.Current; }
			}

			public void Dispose()
			{
			}

			object IEnumerator.Current
			{
				get { return _inner.Current; }
			}

			public bool MoveNext()
			{
				return _inner.MoveNext();
			}

			public void Reset()
			{
				_inner.Reset();
			}
		}

		#endregion

		private IList<T> _inner;

		public ExtendsList(IList<T> inner)
		{
			if (inner == null)
				throw new ArgumentNullException("inner");
			_inner = inner;
		}

		#region IList<Base> Member

		public int IndexOf(B item)
		{
			if (item is T)
				return _inner.IndexOf((T) item);
			return -1;
		}

		public void Insert(int index, B item)
		{
			throw new NotImplementedException("covariant class, cannot write");
		}

		public void RemoveAt(int index)
		{
			_inner.RemoveAt(index);
		}

		public B this[int index]
		{
			get { return _inner[index]; }
			set { throw new NotImplementedException("covariant class, cannot write"); }
		}

		#endregion

		#region ICollection<Base> Member

		public void Add(B item)
		{
			throw new NotImplementedException("covariant class, cannot write");
		}

		public void Clear()
		{
			_inner.Clear();
		}

		public bool Contains(B item)
		{
			if (item is T)
				return _inner.Contains((T) item);
			return false;
		}

		public void CopyTo(B[] array, int arrayIndex)
		{
			if (array != null)
				foreach (T item in _inner) {
					if (arrayIndex >= array.Length) return;
					array[arrayIndex++] = item;
				}
		}

		public int Count
		{
			get { return _inner.Count; }
		}

		public bool IsReadOnly
		{
			get { return _inner.IsReadOnly; }
		}

		public bool Remove(B item)
		{
			if (item is T)
				return _inner.Remove((T) item);
			return false;
		}

		#endregion

		#region IEnumerable<Base> Member

		public IEnumerator<B> GetEnumerator()
		{
			return new ExtendsEnumerator<B, T>(_inner.GetEnumerator());
		}

		#endregion

		#region IEnumerable Member

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _inner.GetEnumerator();
		}

		#endregion
	}
}