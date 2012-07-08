using System;
using System.Collections;

namespace ControlsEx.SideBar
{
	/// <summary>
	/// Zusammenfassung für SideBarItemCollection.
	/// </summary>
	public class SideBarItemCollection:IList
	{

		#region variables
		private SideBar _owner;
		#endregion
		public SideBarItemCollection(SideBar owner)
		{
			if(owner==null)
				throw new ArgumentNullException("owner");
			this._owner=owner;
		}
		#region public members
		public void Add(SideBarItem value)
		{
			if (value == null) return;
			this._owner.Controls.Add(value);
		}
		public void AddRange(SideBarItem[] pages)
		{
			if (pages==null) return;
			this._owner.Controls.AddRange(pages);
		}
		public void Remove(SideBarItem value)
		{
			if (value==null) return;
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
		public SideBarItem this[int index]
		{
			get{return (SideBarItem)this._owner.Controls[index];}
		}
		#endregion
		#region IList Members
		public bool IsReadOnly{get{return false;}}
		object System.Collections.IList.this[int index]{get{return this[index];}set{throw new NotSupportedException();}}
		public void Insert(int index, object value)
		{
			throw new NotSupportedException();
		}
		void System.Collections.IList.Remove(object value)
		{
			if (value is SideBarItem)
				this.Remove((SideBarItem)value);
		}
		public bool Contains(object value)
		{
			if (value is SideBarItem)
				return this._owner.Controls.Contains((SideBarItem)value);
			return false;
		}
		public int IndexOf(object value)
		{
			if (value is SideBarItem)
				return this._owner.Controls.IndexOf((SideBarItem)value);
			return -1;
		}
		int System.Collections.IList.Add(object value)
		{
			if (value is SideBarItem)
			{
				this.Add((SideBarItem)value);
				return this.IndexOf((SideBarItem)value);
			}
			throw new ArgumentException("value");
		}
		public bool IsFixedSize{get{return false;}}
		#endregion
		#region ICollection Members
		public bool IsSynchronized{get{return false;}}
		public int Count
		{
			get{return this._owner.Controls.Count;}
		}
		public void CopyTo(Array array, int index)
		{
			if (index<0) return;
			this._owner.Controls.CopyTo(array,index);
		}
		public object SyncRoot{get{return this;}}
		#endregion
		#region IEnumerable Members
		public IEnumerator GetEnumerator()
		{
			return this._owner.Controls.GetEnumerator();
		}
		#endregion
	}
}
