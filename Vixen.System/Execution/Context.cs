using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution {
	public class Context : IStateSourceCollection<Guid, IIntentStateList>, IDisposable {
		private ChannelStateSourceCollection _channelStates;
		private ContextCurrentEffects _currentEffects;
		private IntentStateBuilder _channelStateBuilder;
		private Dictionary<Guid, PreFilterNode[]> _preFilterCache;
		private bool _disposed;

		public event EventHandler ContextStarted;
		public event EventHandler ContextEnded;

		delegate void IntentDiscoveryAction(Guid channelId, IIntentNode intentNode, TimeSpan intentRelativeTime);

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

			_channelStates = new ChannelStateSourceCollection();
			_currentEffects = new ContextCurrentEffects();
			_preFilterCache = new Dictionary<Guid, PreFilterNode[]>();
			_channelStateBuilder = new IntentStateBuilder();
		}

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

		public IEnumerable<Guid> UpdateChannelStates(TimeSpan currentTime) {
			Guid[] affectedChannels = null;

			if(IsPlaying && !IsPaused) {
				affectedChannels = _UpdateCurrentEffectList(currentTime);
				_RepopulateChannelBuffer(currentTime, affectedChannels);
			}

			return affectedChannels;
		}

		public IStateSource<IIntentStateList> GetState(Guid key) {
			return _channelStates.GetState(key);
		}

		//public void FilterChannelStates(IEnumerable<Guid> channelIds, TimeSpan currentTime) {
		//    if(channelIds == null) return;

		//    // VixenSystem.AllowFilterEvaluation would actually be caught by AddFilters, but
		//    // we're going to save the time to do this loop.
		//    if(!VixenSystem.AllowFilterEvaluation) return;

		//    foreach(Guid channelId in channelIds) {
		//        // Get the pre-filters that would affect this channel.
		//        IEnumerable<PreFilterNode> preFilters = _GetPreFiltersForChannel(channelId, currentTime);
		//        // Get the list of stated intents that are going to affect this channel.
		//        IStateSource<IIntentStateList> stateSource = _channelStates.GetState(channelId);
		//        if(stateSource != null && stateSource.State != null) {
		//            // Add the channel's filters to each intent state.
		//            IEnumerable<IFilterState> filterStates = preFilters.Select(x => x.CreateFilterState(Helper.GetPreFilterRelativeTime(currentTime, x)));
		//            stateSource.State.AddFilters(filterStates);
		//        }
		//    }
		//}

		private Guid[] _UpdateCurrentEffectList(TimeSpan currentTime) {
			// We have an object that does this for us.
			return _currentEffects.UpdateCurrentEffects(_DataSource, currentTime);
		}

		private void _RepopulateChannelBuffer(TimeSpan currentTime, IEnumerable<Guid> affectedChannelIds) {
			_InitializeChannelStateBuilder();
			_DiscoverIntentsFromCurrentEffects(currentTime, _AddIntentToChannelStateBuilder);
			_LatchChannelStatesFromBuilder(affectedChannelIds);
		}

		private void _DiscoverIntentsFromCurrentEffects(TimeSpan currentTime, IntentDiscoveryAction intentDiscoveryAction) {
			_DiscoverIntentsFromEffects(currentTime, _currentEffects, intentDiscoveryAction);
		}

		private void _DiscoverIntentsFromEffects(TimeSpan currentTime, IEnumerable<EffectNode> effects, IntentDiscoveryAction intentDiscoveryAction) {
			// For each effect in the in-effect list for the context...
			foreach(EffectNode effectNode in effects) {
				// Get a time value relative to the start of the effect.
				TimeSpan effectRelativeTime = Helper.GetEffectRelativeTime(currentTime, effectNode);
				// Get the channels the effect affects and the ways it will do so.
				ChannelIntents channelIntents = effectNode.Effect.GetChannelIntents(effectRelativeTime);
				// For each channel...
				foreach(Guid channelId in channelIntents.ChannelIds) {
					// Get the root intent node.
					IntentNode intentNode = channelIntents[channelId];
					// Get a timing value relative to the intent.
					TimeSpan intentRelativeTime = Helper.GetIntentRelativeTime(effectRelativeTime, intentNode);
					// Do whatever is going to be done.
					intentDiscoveryAction(channelId, intentNode, intentRelativeTime);
				}
			}
		}

		private void _InitializeChannelStateBuilder() {
			_channelStateBuilder.Clear();
		}

		private void _AddIntentToChannelStateBuilder(Guid channelId, IIntentNode intentNode, TimeSpan intentRelativeTime) {
			IIntentState intentState = intentNode.CreateIntentState(intentRelativeTime);
			_channelStateBuilder.AddChannelState(channelId, intentState);
		}

		private void _LatchChannelStatesFromBuilder(IEnumerable<Guid> affectedChannelIds) {
			foreach(Guid channelId in affectedChannelIds) {
				_channelStates.SetValue(channelId, _channelStateBuilder.GetChannelState(channelId));
			}
		}

		private void _ResetChannelStates() {
			_InitializeChannelStateBuilder();
			_LatchChannelStatesFromBuilder(_channelStates.ChannelsInCollection);
		}

		private IEnumerable<PreFilterNode> _GetPreFiltersForChannel(Guid channelId, TimeSpan currentTime) {
			PreFilterNode[] preFilters;
			_preFilterCache.TryGetValue(channelId, out preFilters);
			if(preFilters != null) return preFilters.Where(x => _PreFilterQualifies(x, currentTime));
			return Enumerable.Empty<PreFilterNode>();
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

		//private TimeSpan _GetEffectRelativeTime(TimeSpan currentTime, EffectNode effectNode) {
		//    return currentTime - effectNode.StartTime;
		//}

		//private TimeSpan _GetIntentRelativeTime(TimeSpan effectRelativeTime, IntentNode intentNode) {
		//    return effectRelativeTime - intentNode.StartTime;
		//}

		//private TimeSpan _GetPreFilterRelativeTime(TimeSpan sequenceRelativeTime, PreFilterNode preFilterNode) {
		//    return sequenceRelativeTime - preFilterNode.StartTime;
		//}

		private IntentNode _GetCurrentEffectIntent(TimeSpan effectRelativeTime, IEnumerable<IntentNode> intentNodes) {
			return intentNodes.FirstOrDefault(x => effectRelativeTime >= x.StartTime && effectRelativeTime <= x.EndTime);
		}

		private bool _PreFilterQualifies(PreFilterNode preFilterNode, TimeSpan currentTime) {
			return currentTime >= preFilterNode.StartTime && currentTime < preFilterNode.EndTime;
		}

		protected void _BuildPreFilterLookup(IEnumerable<ChannelNode> logicalNodes, IEnumerable<PreFilterNode> preFilters) {
			_preFilterCache.Clear();
			Stack<IEnumerable<PreFilterNode>> preFilterStack = new Stack<IEnumerable<PreFilterNode>>();
			foreach(ChannelNode node in logicalNodes) {
				_SearchLogicalBranchForFilters(node, preFilters, preFilterStack);
			}
		}

		virtual protected IDataSource _DataSource { get; private set; }

		virtual protected ITiming _TimingSource { get; private set; }

		protected virtual bool _OnPlay(TimeSpan startTime, TimeSpan endTime) {
			return true;
		}

		protected virtual void _OnPause() { }

		protected virtual void _OnResume() { }

		protected virtual void _OnStop() { }

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
	}
}
