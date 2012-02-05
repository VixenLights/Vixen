using System;
using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Module.PostFilter;

namespace Vixen.Sys {
	// Need to implement IEquatable<T> so that IEnumerable<T>.Except() will not use
	// the default Object Equals() and GetHashCode().
	/// <summary>
	/// A logical channel of low-level CommandData that is intended to be executed by a controller.
	/// </summary>
	//public class Channel : IEnumerable<CommandNode>, IEqualityComparer<Channel>, IEquatable<Channel> {
	public class Channel : IStateSource<Command>, IEqualityComparer<Channel>, IEquatable<Channel> {
		//The relationship between channels and outputs should be maintained separate
		//from the entities.  Like what would be done in a database.
		//private Patch _patch;
		//private IChannelDataStore _data = new ChannelSortedList();
		private ChannelContextSource _dataSource;
		private CommandStateAggregator _stateAggregator;
		private List<IPostFilterModuleInstance> _postFilters;

		public Channel(string name)
			: this(Guid.NewGuid(), name) {
		}

		//public Channel(string name)
		//    : this(Guid.NewGuid(), name, new Patch()) {
		//}

		public Channel(Guid id, string name) {
			Id = id;
			Name = name;
			_dataSource = new ChannelContextSource(Id);
			_stateAggregator = new CommandStateAggregator();
			_postFilters = new List<IPostFilterModuleInstance>();
		}

		//public Channel(Guid id, string name, Patch patch) {
		//    Id = id;
		//    Name = name;
		//    Patch = patch;
		//    ChannelContextSource dataSource = new ChannelContextSource(Id);
		//    _stateAggregator = new CommandStateAggregator(dataSource);
		//}

		public string Name { get; set; }

		public Guid Id { get; protected set; }

		//public Patch Patch {
		//    get { return _patch; }
		//    set {
		//        // Want any controller references to be properly removed.
		//        if(_patch != null) {
		//            _patch.Clear();
		//        }
		//        _patch = value;
		//    }
		//}

		public bool Masked { get; set; }
		//public bool Masked {
		//    get { return !Patch.Enabled; }
		//    set { Patch.Enabled = !value; }
		//}

		//public IEnumerator<CommandNode> GetEnumerator() {
		//    // We need an enumerator that is live and does not operate upon a snapshot
		//    // of the data.
		//    return _data.GetEnumerator();
		//}

		//System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
		//    return this.GetEnumerator();
		//}

		//public void AddData(IEnumerable<CommandNode> data) {
		//    foreach(CommandNode dataElement in data) {
		//        _data.Add(dataElement);
		//    }
		//}

		//public void AddData(CommandNode data) {
		//    _data.Add(data);
		//}

		//public void Clear() {
		//    _data.Clear();
		//}

		public void Update() {
			// Aggregate our state from the contexts.
			_stateAggregator.Aggregate(_dataSource);
			Value = _stateAggregator.Value;
		}

		public Command Value { get; private set; }

		public void AddPostFilter(IPostFilterModuleInstance filter) {
			_postFilters.Add(filter);
		}

		public void AddPostFilters(IEnumerable<IPostFilterModuleInstance> filters) {
			_postFilters.AddRange(filters);
		}

		public void InsertPostFilter(int index, IPostFilterModuleInstance filter) {
			_postFilters.Insert(index, filter);
		}

		public void RemovePostFilter(IPostFilterModuleInstance filter) {
			_postFilters.Remove(filter);
		}

		public void RemovePostFilterAt(int index) {
			_postFilters.RemoveAt(index);
		}

		public void ClearPostFilters() {
			_postFilters.Clear();
		}

		public IEnumerable<IPostFilterModuleInstance> PostFilters {
			get { return _postFilters; }
		}

		public void FilterState() {
			foreach(IPostFilterModuleInstance postFilter in _postFilters) {
				Value = postFilter.Affect(Value);
				if(Value == null) break;
			}
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
	}
}
