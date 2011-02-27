using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Vixen.Common;

namespace Vixen.Sys {
	public class OutputChannel : Channel, IEnumerable<CommandData> {
		private ConcurrentQueue<CommandData> _data = new ConcurrentQueue<CommandData>();

		public OutputChannel() {
		}

		public OutputChannel(bool createId)
			: base(createId) {
		}

		public IEnumerator<CommandData> GetEnumerator() {
			// We need an enumerator that is live and does not operate upon a snapshot
			// of the data.
			return new ConcurrentQueueLiveEnumerator<CommandData>(_data);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public void AddData(CommandData data) {
			_data.Enqueue(data);
		}

		public override void Clear() {
			_data = new ConcurrentQueue<CommandData>();
		}
	}
}
