using System;
using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Execution {
	class ChannelStateListStateAggregator : CommandStateAggregator, IStateSourceCollection<Guid, Command> {
		private Dictionary<Guid, List<CommandStateSource>> _channelStateLists;
		private Dictionary<Guid, CommandStateAggregator> _channelStates;

		public ChannelStateListStateAggregator() {
			_channelStateLists = new Dictionary<Guid, List<CommandStateSource>>();
			_channelStates = new Dictionary<Guid, CommandStateAggregator>();
		}

		public void ClearState(Guid channelId) {
			List<CommandStateSource> stateList = _GetChannelStateList(channelId);
			stateList.Clear();
		}

		public void ClearState() {
			foreach(Guid channelId in ChannelsWithState) {
				ClearState(channelId);
			}
		}

		public void AddState(Guid channelId, Command state) {
			List<CommandStateSource> stateList = _GetChannelStateList(channelId);
			CommandStateSource commandStateSource = new CommandStateSource(state);
			stateList.Add(commandStateSource);
		}

		public void AggregateState(Guid channelId) {
			List<CommandStateSource> stateList = _GetChannelStateList(channelId);
			CommandStateAggregator aggregator = _GetChannelAggregator(channelId);
			aggregator.Aggregate(stateList);
		}

		public IEnumerable<Guid> ChannelsWithState {
			get { return _channelStates.Keys; }
		}

		public IStateSource<Command> GetValue(Guid key) {
			CommandStateAggregator aggregator;
			_channelStates.TryGetValue(key, out aggregator);
			return aggregator;
		}

		private List<CommandStateSource> _GetChannelStateList(Guid channelId) {
			List<CommandStateSource> stateList;

			if(!_channelStateLists.TryGetValue(channelId, out stateList)) {
				stateList = new List<CommandStateSource>();
				_channelStateLists[channelId] = stateList;
			}

			return stateList;
		}

		private CommandStateAggregator _GetChannelAggregator(Guid channelId) {
			CommandStateAggregator aggregator;

			if(!_channelStates.TryGetValue(channelId, out aggregator)) {
				aggregator = new CommandStateAggregator();
				_channelStates[channelId] = aggregator;
			}

			return aggregator;
		}
	}
}
