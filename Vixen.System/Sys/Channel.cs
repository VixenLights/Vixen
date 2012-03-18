using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys {
	// Need to implement IEquatable<T> so that IEnumerable<T>.Except() will not use
	// the default Object Equals() and GetHashCode().
	/// <summary>
	/// A logical channel of low-level CommandData that is intended to be executed by a controller.
	/// </summary>
	public class Channel : IOutputStateSource, IEqualityComparer<Channel>, IEquatable<Channel> {
		private ChannelContextSource _dataSource;
		private IIntentStateList _state;

		internal Channel(string name)
			: this(Guid.NewGuid(), name) {
		}

		internal Channel(Guid id, string name) {
			Id = id;
			Name = name;
			_dataSource = new ChannelContextSource(Id);
		}

		public string Name { get; set; }

		public Guid Id { get; protected set; }

		public bool Masked { get; set; }

		public void Update() {
			_state = _AggregateStateFromContexts();
		}

		public IIntentStateList State {
			get { return _state; }
		}

		public override string ToString() {
			return Name;
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
			return Id == other.Id;
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		private IIntentStateList _AggregateStateFromContexts() {
			//In reality, all this needs to do is call GetChannelState on each context
			//and put them all into a single collection.
			// NotNull() - filter out any null lists resulting from the context not yet having
			// a state for the channel.  This versus creating a new list in
			// ChannelStateSourceCollection.GetState (or maybe Context.GetState instead, may
			// make more sense there) on a dictionary miss.
			IEnumerable<IIntentState> intentStates = _dataSource.NotNull().SelectMany(x => x.State);
			return new IntentStateList(intentStates);
		}
	}
}