using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	class EffectNodeQueue {
		private Queue<EffectNode> _queue;

		public EffectNodeQueue() {
			_queue = new Queue<EffectNode>();
		}

		public EffectNodeQueue(IEnumerable<EffectNode> items) {
			_queue = new Queue<EffectNode>(items);
		}

		public void Add(EffectNode item) {
			_queue.Enqueue(item);
		}

		public IEnumerable<EffectNode> Get(TimeSpan time) {
			EffectNode effectNode;
			do {
				effectNode = null;
				if(_queue.Count <= 0) continue;

				effectNode = _queue.Peek();
				effectNode = (time >= effectNode.StartTime) ? _queue.Dequeue() : null;
				
				if(effectNode != null) yield return effectNode;
			} while(effectNode != null); 
		}
	}
}
