using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Vixen.Common {
	public class ConcurrentBagLiveEnumerator<T> : LiveEnumerator<T, ConcurrentBag<T>> {
		public ConcurrentBagLiveEnumerator(ConcurrentBag<T> collection)
			: base(collection) {
		}

		protected override bool _GetNext(out T value) {
			return Collection.TryTake(out value);
		}
	}
}
