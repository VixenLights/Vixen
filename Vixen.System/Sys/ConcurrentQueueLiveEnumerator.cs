using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Vixen.Sys {
	class ConcurrentQueueLiveEnumerator<T> : LiveEnumerator<T, ConcurrentQueue<T>> {
		public ConcurrentQueueLiveEnumerator(ConcurrentQueue<T> collection)
			: base(collection) {
		}

		protected override bool _GetNext(out T value) {
			return Collection.TryDequeue(out value);
		}
	}
}
