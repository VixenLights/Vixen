using System;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ControlsEx.DockingFrames
{
	/// <summary>
	/// TabPage Collection for wrapping the controlcollection of tabcontrol
	/// </summary>
	public class TabPageCollection : IList<ITabPage>, IList
	{
		/// <summary>
		/// wraps a non generic enumerator into a generic one
		/// </summary>
		private class WrapperEnumerator<T> : IEnumerator<T>
		{
			private IEnumerator inner;
			public WrapperEnumerator(IEnumerator inner)
			{
				if (inner == null)
					throw new ArgumentNullException("inner");
				this.inner = inner;
			}
			public T Current { get { return (T)inner.Current; } }
			public void Dispose() { }
			object IEnumerator.Current { get { return inner.Current; } }
			public bool MoveNext() { return inner.MoveNext(); }
			public void Reset() { inner.Reset(); }
		}
		#region variables
		private TabControl _owner;
		#endregion
		public TabPageCollection(TabControl owner)
		{
			if (owner == null)
				throw new ArgumentNullException("owner");
			this._owner = owner;
		}
		#region public members
		public void Add(TabPage value)
		{
			this._owner.Controls.Add(value);
		}
		public void AddRange(TabPage[] pages)
		{
			this._owner.Controls.AddRange(pages);
		}
		public void Remove(TabPage value)
		{
			this._owner.Controls.Remove(value);
		}
		public void RemoveAt(int index)
		{
			this._owner.Controls.RemoveAt(index);
		}
		public void Clear()
		{
			this._owner.Controls.Clear();
		}
		public TabPage this[int index]
		{
			get { return (TabPage)this._owner.Controls[index]; }
		}
		#endregion
		#region IList Members
		public bool IsReadOnly
		{
			get { return this._owner.Controls.IsReadOnly; }
		}
		object System.Collections.IList.this[int index]
		{
			get { return this._owner.Controls[index]; }
			set { throw new NotSupportedException(); }
		}
		public void Insert(int index, object value)
		{
			throw new NotSupportedException();
		}
		void System.Collections.IList.Remove(object value)
		{
			this._owner.Controls.Remove(value as TabPage);
		}
		public bool Contains(object value)
		{
			return this._owner.Controls.Contains(value as TabPage);
		}
		public int IndexOf(object value)
		{
			return this._owner.Controls.IndexOf(value as TabPage);
		}
		int System.Collections.IList.Add(object value)
		{
			TabPage page = value as TabPage;
			if (page != null)
			{
				this._owner.Controls.Add(page);
				this._owner.Controls.IndexOf(page);
			}
			throw new ArgumentException("value");
		}
		public bool IsFixedSize { get { return false; } }
		#endregion
		#region ICollection Members
		public bool IsSynchronized { get { return false; } }
		public int Count
		{
			get { return this._owner.Controls.Count; }
		}
		public void CopyTo(Array array, int index)
		{
			this._owner.Controls.CopyTo(array, index);
		}
		public object SyncRoot { get { return this; } }
		#endregion
		#region IEnumerable Members
		public IEnumerator GetEnumerator()
		{
			return this._owner.Controls.GetEnumerator();
		}
		#endregion

		#region IList<ITabPage> Member

		public int IndexOf(ITabPage item)
		{
			return this._owner.Controls.IndexOf(item as TabPage);
		}

		public void Insert(int index, ITabPage item)
		{
			throw new NotSupportedException();
		}

		ITabPage IList<ITabPage>.this[int index]
		{
			get { return (ITabPage)this._owner.Controls[index]; }
			set { throw new NotSupportedException(); }
		}

		#endregion

		#region ICollection<ITabPage> Member

		public void Add(ITabPage item)
		{
			this._owner.Controls.Add(item as TabPage);
		}

		public bool Contains(ITabPage item)
		{
			return this._owner.Controls.Contains(item as TabPage);
		}

		public void CopyTo(ITabPage[] array, int arrayIndex)
		{
			this._owner.Controls.CopyTo(array, arrayIndex);
		}

		public bool Remove(ITabPage item)
		{
			this._owner.Controls.Remove(item as TabPage);
			return true;
		}

		#endregion

		#region IEnumerable<ITabPage> Member

		IEnumerator<ITabPage> IEnumerable<ITabPage>.GetEnumerator()
		{
			return new WrapperEnumerator<ITabPage>(this._owner.Controls.GetEnumerator());
		}

		#endregion
	}
}
