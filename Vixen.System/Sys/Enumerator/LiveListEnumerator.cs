using System.Collections.Generic;

namespace Vixen.Sys.Enumerator {
	class LiveListEnumerator<T> : IEnumerator<T> {
		private List<T> _list;
		private T _current;
		private int _index;

		public LiveListEnumerator(List<T> list) {
			_list = list;
			Reset();
		}

		public T Current {
			get { return _current; }
		}

		public void Dispose() {
		}

		object System.Collections.IEnumerator.Current {
			get { return Current; }
		}

		public bool MoveNext() {
			if(_index+1 < _list.Count) {
				_current = _list[++_index];
				return true;
			}
			_current = default(T);
			return false;
		}

		public void Reset() {
			_current = default(T);
			_index = -1;
		}
	}
}
