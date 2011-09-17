using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.Sys {
	// Need to implement IEquatable<T> so that IEnumerable<T>.Except() will not use
	// the default Object Equals() and GetHashCode().
	/// <summary>
	/// A channel of high-level effect data that seeds a sequence.  Generally, there is only one in use, but more are allowed.
	/// </summary>
	public class EffectStream : IEnumerable<EffectNode>, IEquatable<EffectStream> {
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

		public void AddData(IEnumerable<EffectNode> data) {
			_data.AddRange(data);
		}

		public void Clear() {
			_data.Clear();
		}

		// Both of these are required for Except(), Distinct(), Union() and Intersect().
		// Equals(<type>) for IEquatable and GetHashCode() because that's needed anytime
		// Equals(object) is overridden (which it really isn't, but this is what is said and
		// it doesn't work otherwise).
		public bool Equals(EffectStream other) {
			return this.Id == other.Id;
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public override string ToString() {
			return Name;
		}
	}
}
