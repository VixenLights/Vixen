using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	/// <summary>
	/// Stand-in stub
	/// </summary>
	class IntervalTree<T> {
		private List<T> _list;
		public IntervalTree() {
			_list = new List<T>();
		}

		public IntervalTree(IEnumerable<T> items) {
			_list = new List<T>(items);
		}

		public void Add(T item) {
			_list.Add(item);
		}

		public IEnumerable<T> Get(TimeSpan time) {
			//return Enumerable.Empty<T>();
			return _list.ToArray();
		}

		public void Remove(IEnumerable<T> items) {
			_list.Clear();
		}
	}
}
