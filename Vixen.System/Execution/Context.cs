using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Commands;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution {
	public class Context : IStateSourceCollection<Guid, Command>, IDisposable {
		private CommandStateSourceCollection<Guid> _channelStates;
		private ContextCurrentEffects _currentEffects;
		private ChannelStateListStateAggregator _stateListStateAggregator;
		private Dictionary<Guid, PreFilterNode[]> _preFilterCache;
		private bool _disposed;

		public event EventHandler ContextStarted;
		public event EventHandler ContextEnded;

		internal Context(string name, IDataSource dataSource, ITiming timingSource)
			: this(name) {
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
			if(dataSource == null) throw new ArgumentNullException("timingSource");

			_DataSource = dataSource;
			_TimingSource = timingSource;
		}

		protected Context(string name) {
			Id = Guid.NewGuid();
			Name = name;

			_channelStates = new CommandStateSourceCollection<Guid>();
			_currentEffects = new ContextCurrentEffects();
			_stateListStateAggregator = new ChannelStateListStateAggregator();
			_preFilterCache = new Dictionary<Guid, PreFilterNode[]>();
		}

		virtual protected IDataSource _DataSource { get; private set; }
		virtual protected ITiming _TimingSource { get; private set; }

		public Guid Id { get; private set; }

		public string Name { get; private set; }

		public bool Play() {
			return Play(ProgramExecutor.START_ENTIRE_SEQUENCE, ProgramExecutor.END_ENTIRE_SEQUENCE);
		}

		public bool Play(TimeSpan startTime, TimeSpan endTime) {
			try {
				IsPlaying |= _OnPlay(startTime, endTime);
				return IsPlaying;
			} catch(Exception ex) {
				VixenSystem.Logging.Error(ex);
				return false;
			}
		}

		public void Pause() {
			if(IsPlaying && !IsPaused) {
				IsPaused = true;
				_OnPause();
			}
		}

		public void Resume() {
			if(IsPaused) {
				_OnResume();
				IsPaused = false;
			}
		}

		public void Stop() {
			if(IsPlaying) {
				_OnStop();
				_ResetChannelStates();
				IsPaused = false;
				IsPlaying = false;
			}
		}

		public virtual bool IsPaused { get; private set; }

		virtual public bool IsPlaying { get; private set; }

		public TimeSpan GetTimeSnapshot() {
			return (_TimingSource != null) ? _TimingSource.Position : TimeSpan.Zero;
		}

		/// <summary>
		/// If the context is playing, the context will aggregate new states based on the current time.
		/// </summary>
		/// <returns>The ids of the channels affected by the update.</returns>
		public IEnumerable<Guid> UpdateChannelStates(TimeSpan currentTime) {
			Guid[] affectedChannels = null;

			// Allowing this to update even when it's paused will have the same effect,
			// but may as well save the cycles.
			if(IsPlaying && !IsPaused) {
				affectedChannels = _currentEffects.UpdateCurrentEffects(_DataSource, currentTime);

				// Clear the local states of the affected channels.
				_ClearAffectedChannelStates(affectedChannels);

				_UpdateChannelStatesFromEffects(currentTime, _currentEffects);

				// Aggregate the current state of the affected channels from their
				// new states.
				_AggregateChannelStates(affectedChannels);
			}

			return affectedChannels;
		}

		private void _AggregateChannelStates(IEnumerable<Guid> affectedChannels) {
			foreach(Guid channelId in affectedChannels) {
				_stateListStateAggregator.AggregateState(channelId);
				// The aggregator could be the one to return a channel's state
				// for the context, but filtering will result in a changed value
				// that needs to be stored and that's not what an aggregator does.
				// So there is a separate object that has the official channel
				// values for the context.
				_channelStates.SetValue(channelId, _stateListStateAggregator.GetValue(channelId).Value);
			}
		}

		private void _UpdateChannelStatesFromEffects(TimeSpan currentTime, IEnumerable<EffectNode> effects) {
			// For each effect in the in-effect list for the context...
			foreach(EffectNode effectNode in effects) {
				// Render it to get a dictionary of intent collections by channel id.
				EffectIntents effectIntents = effectNode.Effect.Render();
				// For each channel id in the dictionary...
				foreach(Guid channelId in effectIntents.Keys) {
					// Get the current intent by effect-relative time.
					TimeSpan effectRelativeTime = _GetEffectRelativeTime(currentTime, effectNode);
					IntentNode intentNode = _GetCurrentEffectIntent(effectRelativeTime, effectIntents[channelId]);
					// If there is an intent,
					if(intentNode != null) {
						// Get the current state command by intent-relative time.
						TimeSpan intentRelativeTime = _GetIntentRelativeTime(effectRelativeTime, intentNode);
						Command currentIntentState = _GetCurrentIntentState(intentRelativeTime, intentNode);
						if(currentIntentState != null) {
							_stateListStateAggregator.AddState(channelId, currentIntentState);
						}
					}
				}
			}
		}

		private void _ClearAffectedChannelStates(IEnumerable<Guid> affectedChannels) {
			foreach(Guid channelId in affectedChannels) {
				_stateListStateAggregator.ClearState(channelId);
			}
		}

		private void _ResetChannelStates() {
			_stateListStateAggregator.ClearState();
			_AggregateChannelStates(_stateListStateAggregator.ChannelsWithState);
		}

		public void FilterChannelStates(IEnumerable<Guid> channelIds, TimeSpan currentTime) {
			if(channelIds == null) return;

			foreach(Guid channelId in channelIds) {
				IStateSource<Command> commandStateSource = _stateListStateAggregator.GetValue(channelId);
				if(commandStateSource != null && commandStateSource.Value != null) {
					Command commandState = _ApplyPreFilters(channelId, commandStateSource.Value, currentTime);
					_channelStates.SetValue(channelId, commandState);
				}
			}
		}

		protected virtual bool _OnPlay(TimeSpan startTime, TimeSpan endTime) {
			return true;
		}

		protected virtual void _OnPause() {
		}

		protected virtual void _OnResume() {
		}

		protected virtual void _OnStop() {
		}

		protected virtual void OnContextStarted(EventArgs e) {
			if(ContextStarted != null) {
				ContextStarted(this, e);
			}
		}

		protected virtual void OnContextEnded(EventArgs e) {
			if(ContextEnded != null) {
				ContextEnded(this, e);
			}
		}

		public IStateSource<Command> GetValue(Guid key) {
			return _channelStates.GetValue(key);
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if(!_disposed) {
				if(disposing) {
					Stop();
				}
				_disposed = true;
			}
		}

		~Context() {
			Dispose(false);
		}

		protected void _BuildPreFilterLookup(IEnumerable<ChannelNode> logicalNodes, IEnumerable<PreFilterNode> preFilters) {
			_preFilterCache.Clear();
			Stack<IEnumerable<PreFilterNode>> preFilterStack = new Stack<IEnumerable<PreFilterNode>>();
			foreach(ChannelNode node in logicalNodes) {
				_SearchLogicalBranchForFilters(node, preFilters, preFilterStack);
			}
		}

		private void _SearchLogicalBranchForFilters(ChannelNode node, IEnumerable<PreFilterNode> preFilters, Stack<IEnumerable<PreFilterNode>> preFiltersFound) {
			// Must push a single value for each level we enter.
			PreFilterNode[] preFilterNodes = _GetPreFiltersForNode(node, preFilters);
			if(preFilterNodes.Length > 0) {
				preFiltersFound.Push(preFilterNodes);
			} else {
				preFiltersFound.Push(null);
			}

			if(node.IsLeaf) {
				PreFilterNode[] channelFilters = preFiltersFound.Where(x => x != null).Reverse().SelectMany(x => x).ToArray();
				if(channelFilters.Length > 0) {
					_preFilterCache[node.Channel.Id] = channelFilters;
				}
			} else {
				foreach(ChannelNode childNode in node.Children) {
					_SearchLogicalBranchForFilters(childNode, preFilters, preFiltersFound);
				}
			}

			// Pop a single value for every level we exit.
			preFiltersFound.Pop();
		}

		private PreFilterNode[] _GetPreFiltersForNode(ChannelNode node, IEnumerable<PreFilterNode> preFilters) {
			return preFilters.Where(x => x.PreFilter.TargetNodes.Contains(node)).ToArray();
		}

		private TimeSpan _GetEffectRelativeTime(TimeSpan currentTime, EffectNode effectNode) {
			return currentTime - effectNode.StartTime;
		}

		private TimeSpan _GetIntentRelativeTime(TimeSpan effectRelativeTime, IntentNode intentNode) {
			return effectRelativeTime - intentNode.StartTime;
		}

		private IntentNode _GetCurrentEffectIntent(TimeSpan effectRelativeTime, IEnumerable<IntentNode> intentNodes) {
			return intentNodes.FirstOrDefault(x => effectRelativeTime >= x.StartTime && effectRelativeTime <= x.EndTime);
		}

		private Command _GetCurrentIntentState(TimeSpan intentRelativeTime, IntentNode intentNode) {
			return intentNode.Intent.GetCurrentState(intentRelativeTime);
		}

		private Command _ApplyPreFilters(Guid channelId, Command value, TimeSpan currentTime) {
			if(value != null) {
				PreFilterNode[] preFilters;
				if(_preFilterCache.TryGetValue(channelId, out preFilters)) {
					foreach(PreFilterNode preFilterNode in preFilters) {
						if(_PreFilterQualifies(preFilterNode, currentTime)) {
							TimeSpan filterRelativeTime = currentTime - preFilterNode.StartTime;
							value = preFilterNode.PreFilter.Affect(value, filterRelativeTime);
							if(value == null) break;
						}
					}
				}
			}
			return value;
		}

		private bool _PreFilterQualifies(PreFilterNode preFilterNode, TimeSpan currentTime) {
			return currentTime >= preFilterNode.StartTime && currentTime < preFilterNode.EndTime;
		}
	}
}
