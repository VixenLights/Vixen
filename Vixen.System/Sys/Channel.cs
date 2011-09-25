using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.Sys {
	// Need to implement IEquatable<T> so that IEnumerable<T>.Except() will not use
	// the default Object Equals() and GetHashCode().
	/// <summary>
	/// A logical channel of low-level CommandData that is intended to be executed by a controller.
	/// </summary>
	public class Channel : IEnumerable<Command>, IEqualityComparer<Channel>, IEquatable<Channel> {
		private Patch _patch;
		private IChannelDataStore _data = new ChannelSortedList();

		public Channel(string name)
			: this(Guid.NewGuid(), name, new Patch()) {
		}

		public Channel(Guid id, string name, Patch patch) {
			Id = id;
			Name = name;
			Patch = patch;
		}

		public string Name { get; set; }

		public Guid Id { get; protected set; }

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
			return _data.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public void AddData(IEnumerable<Command> data) {
			foreach(Command dataElement in data) {
				_data.Add(dataElement);
			}
		}

		public void AddData(Command data) {
			_data.Add(data);
		}

		public void Clear() {
			_data.Clear();
		}

		public bool Equals(Channel x, Channel y) {
			return x.Id == y.Id;
		}

		public int GetHashCode(Channel obj) {
			return obj.Id.GetHashCode();
		}

		// Both of these are required for Except(), Distinct(), Union() and Intersect().
		// Equals(<type>) for IEquatable and GetHashCode() because that's needed anytime
		// Equals(object) is overridden (which it really isn't, but this is what is said and
		// it doesn't work otherwise).
		public bool Equals(Channel other) {
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
