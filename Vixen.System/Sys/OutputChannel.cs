using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.Sys {
	/// <summary>
	/// A logical channel of low-level CommandData that is intended to be executed by a controller.
	/// </summary>
	public class OutputChannel : Channel, IEnumerable<Command>, IEqualityComparer<OutputChannel> {
		private Patch _patch;
		private ConcurrentQueue<Command> _data = new ConcurrentQueue<Command>();

		public OutputChannel(string name)
			: base(name) {
			this.Patch = new Patch();
		}

		public OutputChannel(Guid id, string name, Patch patch) {
			Id = id;
			Name = name;
			Patch = patch;
		}

		public Patch Patch {
			get { return _patch; }
			set {
				// Want any controller references to be properly removed.
				if(_patch != null) {
					_patch.Clear();
				}
				_patch = value;
			}
		}

		public bool Masked {
			get { return !this.Patch.Enabled; }
			set { this.Patch.Enabled = !value; }
		}
		
		public IEnumerator<Command> GetEnumerator() {
			// We need an enumerator that is live and does not operate upon a snapshot
			// of the data.
			return new ConcurrentQueueLiveEnumerator<Command>(_data);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public void AddData(IEnumerable<Command> data) {
			foreach(Command dataElement in data) {
				_data.Enqueue(dataElement);
			}
		}

		public void AddData(Command data) {
			_data.Enqueue(data);
		}

		public override void Clear() {
			_data = new ConcurrentQueue<Command>();
		}

		public bool Equals(OutputChannel x, OutputChannel y) {
			return x.Id == y.Id;
		}

		public int GetHashCode(OutputChannel obj) {
			return obj.Id.GetHashCode();
		}
	}
}
