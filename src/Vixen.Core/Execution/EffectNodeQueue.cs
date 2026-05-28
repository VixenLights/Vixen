using Vixen.Sys;

namespace Vixen.Execution
{
	internal class EffectNodeQueue 
	{
		private readonly Queue<IEffectNode> _queue;
		
		public EffectNodeQueue()
		{
			_queue = new Queue<IEffectNode>();
			}

		public EffectNodeQueue(int size)
		{
			_queue = new Queue<IEffectNode>(size);
			}

		public EffectNodeQueue(IEnumerable<IEffectNode> items)
		{
			_queue = new Queue<IEffectNode>(items);
			}

		public void Add(IEffectNode item)
		{
			_queue.Enqueue(item);
		}

		public void Clear()
		{
			_queue.Clear();
		}

		public IEnumerable<IEffectNode> Get(TimeSpan time)
		{
			IEffectNode effectNode;
			do {
				effectNode = null;
				if (_queue.Count <= 0) continue;

				effectNode = _queue.Peek();
				effectNode = ((effectNode != null) && (time >= effectNode.StartTime)) ? _queue.Dequeue() : null;

				if (effectNode != null)
					yield return effectNode;
			} while (effectNode != null);
		}

		public void GetInto(TimeSpan time, List<IEffectNode> destination)
		{
			while (_queue.Count > 0)
			{
				var peeked = _queue.Peek();
				if (peeked == null || time < peeked.StartTime) break;
				destination.Add(_queue.Dequeue());
			}
		}

	}
}