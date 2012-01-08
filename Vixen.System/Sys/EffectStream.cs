using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	/// <summary>
	/// A channel of high-level effect data that seeds a sequence.  Generally, there is only one in use, but more are allowed.
	/// </summary>
	public class EffectStream : IEnumerable<EffectNode> {
		private List<EffectNode> _data = new List<EffectNode>();

		public EffectStream(string name) {
			Name = name;
			Id = Guid.NewGuid();
		}

		private EffectStream() { }

		public string Name { get; set; }

		public Guid Id { get; private set; }

		public IEnumerator<EffectNode> GetEnumerator() {
			// We need an enumerator that is live and does not operate upon a snapshot
			// of the data.
			return new LiveListEnumerator<EffectNode>(_data);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public void AddData(EffectNode data) {
			_data.Add(data);
		}

		public bool RemoveData(EffectNode data)
		{
			return _data.Remove(data);
		}

		public void AddData(IEnumerable<EffectNode> data) {
			_data.AddRange(data);
		}

		public void Clear() {
			_data.Clear();
		}

		public override string ToString() {
			return Name;
		}
	}
}
