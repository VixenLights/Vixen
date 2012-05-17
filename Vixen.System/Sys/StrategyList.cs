using System.Collections.Generic;

namespace Vixen.Sys {
	public delegate void AddStrategyDelegate<T>(T itemAdding, IList<T> list);
	public delegate bool RemoveStrategyDelegate<T>(T itemRemoving, IList<T> list);
	public delegate void InsertStrategyDelegate<T>(int index, T itemInserting, IList<T> list);
	public delegate void ItemSetStrategyDelegate<T>(int index, T newItem, IList<T> list);
	public delegate void ClearStrategyDelegate<T>(IList<T> list);

	public class StrategyList<T> : IList<T> {
		private List<T> _list;
		private AddStrategyDelegate<T> _addStrategy;
		private RemoveStrategyDelegate<T> _removeStrategy;
		private InsertStrategyDelegate<T> _insertStrategy;
		private ItemSetStrategyDelegate<T> _setStrategy;
		private ClearStrategyDelegate<T> _clearStrategy;
 
		public StrategyList(AddStrategyDelegate<T> addStrategy = null, RemoveStrategyDelegate<T> removeStrategy = null, InsertStrategyDelegate<T> insertStrategy = null, ItemSetStrategyDelegate<T> setStrategy = null, ClearStrategyDelegate<T> clearStrategy = null) {
			_list = new List<T>();
			_addStrategy = addStrategy ?? _DefaultAddStrategy;
			_removeStrategy = removeStrategy ?? _DefaultRemoveStrategy;
			_insertStrategy = insertStrategy ?? _DefaultInsertStrategy;
			_setStrategy = setStrategy ?? _DefaultSetStrategy;
			_clearStrategy = clearStrategy ?? _DefaultClearStrategy;
		}

		public void AddRange(IEnumerable<T> items) {
			foreach(T item in items) {
				Add(item);
			}
		}

		public int IndexOf(T item) {
			return _list.IndexOf(item);
		}

		public void Insert(int index, T item) {
			_insertStrategy(index, item, _list);
		}

		protected void StrategyBypassInsert(int index, T item) {
			_list.Insert(index, item);
		}

		public void RemoveAt(int index) {
			_removeStrategy(_list[index], _list);
		}

		protected void StrategyBypassRemoveAt(int index) {
			_list.RemoveAt(index);
		}

		public T this[int index] {
			get { return _list[index]; }
			set { _setStrategy(index, value, _list); }
		}

		public void Add(T item) {
			_addStrategy(item, _list);
		}

		protected void StrategyBypassAdd(T item) {
			_list.Add(item);
		}

		public void Clear() {
			_clearStrategy(_list);
		}

		protected void StrategyBypassClear() {
			_list.Clear();
		}

		public bool Contains(T item) {
			return _list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex) {
			_list.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return _list.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove(T item) {
			return _removeStrategy(item, _list);
		}

		protected bool StrategyBypassRemove(T item) {
			return _list.Remove(item);
		}

		public IEnumerator<T> GetEnumerator() {
			return _list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		#region Default strategies
		private void _DefaultAddStrategy(T itemAdding, IList<T> list) {
			list.Add(itemAdding);
		}

		private bool _DefaultRemoveStrategy(T itemRemoving, IList<T> list) {
			return list.Remove(itemRemoving);
		}

		private void _DefaultInsertStrategy(int index, T itemInserting, IList<T> list) {
			list.Insert(index, itemInserting);
		}

		private void _DefaultSetStrategy(int index, T newItem, IList<T> list) {
			list[index] = newItem;
		}

		private void _DefaultClearStrategy(IList<T> list) {
			list.Clear();
		}
		#endregion
	}
}
