using System.Collections.Concurrent;

namespace Vixen.Sys.Enumerator
{
	internal class ConcurrentQueueLiveEnumerator<T> : LiveEnumerator<T, ConcurrentQueue<T>>
	{
		public ConcurrentQueueLiveEnumerator(ConcurrentQueue<T> collection)
			: base(collection)
		{
		}

		protected override bool _GetNext(out T value)
		{
			return Collection.TryDequeue(out value);
		}
	}
}