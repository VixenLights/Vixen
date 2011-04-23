using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Vixen.Common;

namespace Vixen.Sys {
	public class OutputChannel : Channel, IEnumerable<CommandData>, IEqualityComparer<OutputChannel> {
		private ConcurrentQueue<CommandData> _data = new ConcurrentQueue<CommandData>();

		public OutputChannel(string name)
			: base(name) {
		}

		// Need a public parameterless constructor because this type is used as a
		// generic parameter and needs to support new().
		public OutputChannel() {
		}

		public IEnumerator<CommandData> GetEnumerator() {
			// We need an enumerator that is live and does not operate upon a snapshot
			// of the data.
			return new ConcurrentQueueLiveEnumerator<CommandData>(_data);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public void AddData(IEnumerable<CommandData> data) {
			foreach(CommandData dataElement in data) {
				_data.Enqueue(dataElement);
			}
		}

		public void AddData(CommandData data) {
			_data.Enqueue(data);
		}

		public override void Clear() {
			_data = new ConcurrentQueue<CommandData>();
		}

		public bool Equals(OutputChannel x, OutputChannel y) {
			return x.Id == y.Id;
		}

		public int GetHashCode(OutputChannel obj) {
			return obj.Id.GetHashCode();
		}
	}
}
