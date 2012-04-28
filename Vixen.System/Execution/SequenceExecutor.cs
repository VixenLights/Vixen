using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
// Using System.Timers.Timer because it exposes a SynchronizingObject member that lets
// you specify the thread context for the Elapsed event.
using System.Timers;
using System.Threading;
using Vixen.Sys;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Timing;
using Vixen.Module.Media;
using Vixen.Sys.Attribute;

namespace Vixen.Execution {
	class SequenceExecutor : IExecutor {
		private System.Timers.Timer _updateTimer;
		private ISequence _sequence;
		private IRuntimeBehaviorModuleInstance[] _runtimeBehaviors;
		private SynchronizationContext _syncContext;

		public event EventHandler<SequenceStartedEventArgs> SequenceStarted;
		public event EventHandler<SequenceEventArgs> SequenceEnded;
		public event EventHandler<ExecutorMessageEventArgs> Message;
		public event EventHandler<ExecutorMessageEventArgs> Error;

		// Public for Activator.
		public SequenceExecutor() {
			_updateTimer = new System.Timers.Timer(10);
			_updateTimer.Elapsed += _UpdateTimerElapsed;
			_syncContext = AsyncOperationManager.SynchronizationContext;
		}

		public ISequence Sequence {
			get { return _sequence; }
			set {
				if(_sequence != value) {
					_sequence = value;

					// Get runtime behavior.
					_runtimeBehaviors = value.RuntimeBehaviors;
				}
			}
		}

		public bool IsPlaying {
			get { return IsRunning; }
		}

		private bool _DataListener(IEffectNode effectNode) {
			// Data has been inserted into the sequence.
			// Give every behavior a chance at the data.
			foreach(IRuntimeBehaviorModuleInstance behavior in _runtimeBehaviors) {
				if(behavior.Enabled) {
					//*** Can't remember why this is being set here in this fashion. 
					//    Shouldn't it be done by whatever creates the node?
					//effectNode.StartTime = _TimingSource.Position;
					behavior.Handle(effectNode);
				}
			}

			// Data written to a sequence will go through the behaviors and then on to the
			// effect enumerator of the executor by way of the CommandNodeIntervalSync
			// to be executed against the sequence's time.  This has the side effect of
			// allowing timed-live behavior without an explicit runtime behavior that has
			// to manage timing on its own.
			_sequence.Data.AddLive(effectNode);

			// We don't want any handlers beyond the executor to get live data.
			return true;
		}

		public void Play(TimeSpan startTime, TimeSpan endTime) {
			if(Sequence != null) {
				// Only hook the input stream during execution.
				// Hook before starting the behaviors.
				_sequence.InsertDataListener += _DataListener;

				// Bound the execution range.
				StartTime = startTime < Sequence.Length ? startTime : Sequence.Length;
				EndTime = endTime < Sequence.Length ? endTime : Sequence.Length;

				// Notify any subclass that we're ready to start and allow it to do
				// anything it needs to prepare.
				OnPlaying(StartTime, EndTime);
				
				_TimingSource = Sequence.GetTiming() ??
					Modules.GetManager<ITimingModuleInstance, TimingModuleManagement>().GetDefault();

				// Initialize behaviors BEFORE data is pulled from the sequence,
				// they may influence the data.
				foreach(IRuntimeBehaviorModuleInstance behavior in _runtimeBehaviors) {
					behavior.Startup(Sequence);
				}

				// Load the media.
				foreach(IMediaModuleInstance media in Sequence.GetAllMedia()) {
					media.LoadMedia(StartTime);
				}

				// Start the crazy train.
				// Need to set IsRunning to true before firing the SequenceStarted event, otherwise
				// we will be lying to our subscribers and they will be very confused.
				IsRunning = true;
				OnSequenceStarted(new SequenceStartedEventArgs(Sequence, _TimingSource));

				// Start the media.
				foreach(IMediaModuleInstance media in Sequence.GetAllMedia()) {
					media.Start();
				}
				_TimingSource.Position = StartTime;
				_TimingSource.Start();
				
				// Fire the first event manually because it takes a while for the timer
				// to elapse the first time.
				_UpdateOutputs();
				// If there is no length, we may have been stopped as a cascading result
				// of that update.
				if(IsRunning) {
					_updateTimer.Start();
				}
			}
		}

		public IEnumerable<IEffectNode> GetSequenceData() {
			if(_sequence != null) {
				return _sequence.GetData();
			}
			return null;
		}

		public ITiming GetSequenceTiming() {
			return _TimingSource;
		}

		public IEnumerable<IPreFilterNode> GetSequenceFilters() {
			if(_sequence != null) {
				return _sequence.GetAllPreFilters();
			}
			return Enumerable.Empty<PreFilterNode>();
		}

		public string Name {
			get {
				if(_sequence != null) {
					return _sequence.Name;
				}
				return null;
			}
		}

		virtual protected void OnPlaying(TimeSpan startTime, TimeSpan endTime) { }

		public void Pause() {
			if(_updateTimer.Enabled) {
				_TimingSource.Pause();
				foreach(IMediaModuleInstance media in Sequence.GetAllMedia()) {
					media.Pause();
				}

				OnPausing();
				_updateTimer.Enabled = false;
			}
		}

		virtual protected void OnPausing() { }

		public void Resume() {
			if(!_updateTimer.Enabled && Sequence != null) {
				_TimingSource.Resume();
				foreach(IMediaModuleInstance media in Sequence.GetAllMedia()) {
					media.Resume();
				}

				_updateTimer.Enabled = true;
				OnResumed();
			}
		}

		virtual protected void OnResumed() { }

		public void Stop() {
			if(IsRunning) {
				_Stop();
			}
		}

		private void _Stop() {
			// Stop whatever is driving this crazy train.
			lock(_updateTimer) {
				_updateTimer.Enabled = false;
			}

			OnStopping();

			// Release the hook before the behaviors are shut down so that
			// they can affect the sequence.
			_sequence.InsertDataListener -= _DataListener;

			// Shutdown the behaviors.
			foreach(IRuntimeBehaviorModuleInstance behavior in _runtimeBehaviors) {
				behavior.Shutdown();
			}

			IsRunning = false;

			OnSequenceEnded(new SequenceEventArgs(Sequence));

			_TimingSource.Stop();
			foreach(IMediaModuleInstance media in Sequence.GetAllMedia()) {
				media.Stop();
			}
		}

		virtual protected void OnStopping() { }

		public bool IsRunning { get; private set; }

		private void _UpdateTimerElapsed(object sender, ElapsedEventArgs e) {
			lock(_updateTimer) {
				// To catch events that may trail after the timer's been disabled
				// due to it being a threaded timer and Stop being called between the
				// timer message being posted and acted upon.
				if(_updateTimer == null || !_updateTimer.Enabled) return;

				_updateTimer.Enabled = false;

				_UpdateOutputs();

				if(IsRunning) {
					_updateTimer.Enabled = true;
				}
			}
		}

		private void _UpdateOutputs() {
			if(_IsEndOfSequence()) {
				_syncContext.Post(x => Stop(), null);
			}
		}

		private bool _IsEndOfSequence() {
			return EndTime >= StartTime && _TimingSource.Position >= EndTime;
		}

		protected virtual void OnSequenceStarted(SequenceStartedEventArgs e) {
			if(SequenceStarted != null) {
				SequenceStarted(null, e);
			}
		}

		protected virtual void OnSequenceEnded(SequenceEventArgs e) {
			if(SequenceEnded != null) {
				SequenceEnded(null, e);
			}
		}

		protected virtual void OnMessage(ExecutorMessageEventArgs e) {
			if(Message != null) {
				Message(null, e);
			}
		}

		protected virtual void OnError(ExecutorMessageEventArgs e) {
			if(Error != null) {
				Error(Sequence, e);
			}
		}

		// Because these are calculated values, changing the length of the sequence
		// during execution will not affect the end time.
		public TimeSpan StartTime { get; protected set; }
		public TimeSpan EndTime { get; protected set; }

		static public IExecutor GetExecutor(ISequence executable) {
			Type attributeType = typeof(ExecutorAttribute);
			IExecutor executor = null;
			// If the executable is decorated with [Executor], get that executor.
			// Since sequences are implemented as modules now, we need to look in the inheritance chain
			// for the attribute.
			ExecutorAttribute attribute = (ExecutorAttribute)executable.GetType().GetCustomAttributes(attributeType, true).FirstOrDefault();
			if(attribute != null) {
				// Create the executor.
				executor = Activator.CreateInstance(attribute.ExecutorType) as IExecutor;
				if(executor != null) {
					// Assign the sequence to the executor.
					executor.Sequence = executable;
				}
			}
			return executor;
		}

		~SequenceExecutor() {
			Dispose(false);
		}

		public void Dispose() {
			Dispose(true);
		}

		public virtual void Dispose(bool disposing) {
			if(_updateTimer != null) {
				lock(_updateTimer) {
					Stop();
					_updateTimer.Elapsed -= _UpdateTimerElapsed;
					_updateTimer.Dispose();
					_updateTimer = null;
				}
			}
			GC.SuppressFinalize(this);
		}

		private ITiming _TimingSource { get; set; }
	}

}
