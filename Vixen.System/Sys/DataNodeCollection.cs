using System.Collections.Concurrent;
using System.Collections.Generic;
using Vixen.Sys.Enumerator;

namespace Vixen.Sys {
	class DataNodeCollection : IEnumerable<IDataNode> {
		private ConcurrentQueue<IDataNode> _data;

		public DataNodeCollection() {
			_data = new ConcurrentQueue<IDataNode>();
		}

		public void Add(IDataNode value) {
			_data.Enqueue(value);
		}

		public void AddRange(IEnumerable<IDataNode> values) {
			foreach(IDataNode value in values) {
				_data.Enqueue(value);
			}
		}

		public IEnumerator<IDataNode> GetEnumerator() {
			// We need an enumerator that is live and does not operate upon a snapshot
			// of the data.
			return new ConcurrentQueueLiveEnumerator<IDataNode>(_data);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
