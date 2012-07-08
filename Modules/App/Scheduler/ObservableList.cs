using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace VixenModules.App.Scheduler {
	class ObservableList<T> : IList<T>, INotifyCollectionChanged {
		private List<T> _list;

		public ObservableList() {
			_list = new List<T>();
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public int IndexOf(T item) {
			return _list.IndexOf(item);
		}

		public void Insert(int index, T item) {
			_list.Insert(index, item);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}

		public void RemoveAt(int index) {
			T obj = default(T);
			if(index < _list.Count) obj = _list[index];

			_list.RemoveAt(index);
			
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj, index));
		}

		public T this[int index] {
			get {
				return _list[index];
			}
			set {
				T obj = default(T);
				if(index < _list.Count) obj = _list[index];
				_list[index] = value;

				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, obj, index));
			}
		}

		public void Add(T item) {
			_list.Add(item);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _list.Count - 1));
		}

		public void AddRange(IEnumerable<T> items) {
			_list.AddRange(items);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList()));
		}

		public void Clear() {
			_list.Clear();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
			if(_list.Remove(item)) {
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
				return true;
			}
			return false;
		}

		public void Replace(T item1, T item2) {
			int index = IndexOf(item1);
			if(index != -1) {
				this[index] = item2;
			}
		}

		public IEnumerator<T> GetEnumerator() {
			return _list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if(CollectionChanged != null) {
				CollectionChanged(this, e);
			}
		}
	}
}
