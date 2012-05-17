using System;
using System.Collections.Generic;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution {
	public class Context : IStateSourceCollection<Guid, IIntentStateList>, IDisposable {
		private ChannelStateSourceCollection _channelStates;
		private IContextCurrentEffects _currentEffects;
		private IntentStateBuilder _channelStateBuilder;
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
			_currentEffects = new ContextCurrentEffectsIncremental();
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

		// A context may or may not wrap a sequence, so the state of this property cannot be
		// dependent upon a sequence starting.
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

		private void _DiscoverIntentsFromEffects(TimeSpan currentTime, IEnumerable<IEffectNode> effects, IntentDiscoveryAction intentDiscoveryAction) {
			// For each effect in the in-effect list for the context...
			foreach(IEffectNode effectNode in effects) {
				// Get a time value relative to the start of the effect.
				TimeSpan effectRelativeTime = Helper.GetEffectRelativeTime(currentTime, effectNode);
				// Get the channels the effect affects at this time and the ways it will do so.
				ChannelIntents channelIntents = effectNode.Effect.GetChannelIntents(effectRelativeTime);
				// For each channel...
				foreach(Guid channelId in channelIntents.ChannelIds) {
					// Get the root intent node.
					IIntentNode intentNode = channelIntents[channelId];
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
