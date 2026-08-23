using System.ComponentModel;
using Vixen.Execution;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module.Timing;
using Vixen.Module.Media;
using Vixen.Sys.LayerMixing;
using Vixen.Utility;

namespace BaseSequence
{
	public class SequenceExecutor : ISequenceExecutor
	{
		private readonly Lock _lifecycleLock = new();
		private HighResolutionTimer _endCheckTimer;
		private SynchronizationContext _syncContext;
		private bool _isRunning;
		private bool _isPaused;
		private bool _loop;
		private bool _isDisposed;
		private long _executionGeneration;

		public event EventHandler<SequenceStartedEventArgs> SequenceStarted;
		public event EventHandler<SequenceStartedEventArgs> SequenceReStarted;
		public event EventHandler<SequenceEventArgs> SequenceEnded;
		public event EventHandler<ExecutorMessageEventArgs> Message;
		public event EventHandler<ExecutorMessageEventArgs> Error;

		public SequenceExecutor()
		{
			_endCheckTimer = new HighResolutionTimer(10f);
			_endCheckTimer.UseHighPriorityThread = false;
			_endCheckTimer.Elapsed += _EndCheckTimerElapsed;
			_syncContext = AsyncOperationManager.SynchronizationContext;
		}

		#region Public

		// Because these are calculated values, changing the length of the sequence
		// during execution will not affect the end time.
		public TimeSpan StartTime { get; protected set; }

		public TimeSpan EndTime { get; protected set; }

		public ISequence Sequence { get; set; }

		public bool IsRunning
		{
			get { return _isRunning; }
			private set
			{
				_isRunning = value;
				if (_isRunning) {
					OnSequenceStarted(new SequenceStartedEventArgs(Sequence, TimingSource, StartTime, EndTime));
				}
				else {
					OnSequenceEnded(new SequenceEventArgs(Sequence));
				}
			}
		}

		public bool IsPaused
		{
			get { return _isPaused; }
			private set { _isPaused = value; }
		}

		public void Start()
		{
			Play(TimeSpan.Zero, TimeSpan.MaxValue);
		}

		public void Play(TimeSpan startTime, TimeSpan endTime)
		{
			lock (_lifecycleLock)
			{
				if (_isDisposed) return;
				_loop = false;
			}
			_Play(startTime, endTime);
		}

		public void PlayLoop(TimeSpan startTime, TimeSpan endTime)
		{
			lock (_lifecycleLock)
			{
				if (_isDisposed) return;
				_loop = true;
			}
			_Play(startTime, endTime);
		}

		public void Pause()
		{
			_Pause();
		}

		public void Resume()
		{
			_Resume();
		}

		public void Stop()
		{
			_Stop();
		}

		public IEnumerable<ISequenceFilterNode> SequenceFilters
		{
			get
			{
				if (Sequence != null) {
					return Sequence.GetAllSequenceFilters();
				}
				return Enumerable.Empty<ISequenceFilterNode>();
			}
		}

		public SequenceLayers SequenceLayers
		{
			get
			{
				//Would the sequence ever be null here? If it is that's bad anyway
				return Sequence.GetSequenceLayerManager();
			}
		}

		public string Name
		{
			get
			{
				if (Sequence != null) {
					return Sequence.Name;
				}
				return null;
			}
		}

		#endregion

		#region Events

		protected virtual void OnSequenceStarted(SequenceStartedEventArgs e)
		{
			if (SequenceStarted != null) {
				SequenceStarted(null, e);
			}
		}

		protected virtual void OnSequenceReStarted(SequenceStartedEventArgs e)
		{
			if (SequenceReStarted != null)
			{
				SequenceReStarted(null, e);
			}
		}

		protected virtual void OnSequenceEnded(SequenceEventArgs e)
		{
			if (SequenceEnded != null) {
				SequenceEnded(null, e);
			}
		}

		protected virtual void OnMessage(ExecutorMessageEventArgs e)
		{
			if (Message != null) {
				Message(null, e);
			}
		}

		protected virtual void OnError(ExecutorMessageEventArgs e)
		{
			if (Error != null) {
				Error(Sequence, e);
			}
		}

		#endregion

		#region Private

		private void _Play(TimeSpan startTime, TimeSpan endTime)
		{
			long executionGeneration;
			lock (_lifecycleLock)
			{
				if (_isDisposed || IsRunning || Sequence == null) return;
				executionGeneration = ++_executionGeneration;
			}

			// Only hook the input stream during execution.
			// Hook before starting the behaviors.
			//_HookDataListener();

			// Bound the execution range.
			StartTime = _CoerceStartTime(startTime);
			EndTime = _CoerceEndTime(endTime);
			_IsTimedSequence = EndTime >= StartTime; 

			if (StartTime == EndTime)
			{
				//We are done before we get started
				_PostNaturalEnd(executionGeneration, false);
			}

			TimingSource = Sequence.GetTiming() ?? _GetDefaultTimingSource();

			_LoadMedia();

			_StartMedia();

			TimingSource.Position = StartTime;
			TimingSource.Start();

			// Start the crazy train.
			IsRunning = true;

			while (TimingSource.Position == StartTime)
			{
				Thread.Sleep(1); //Give the train a chance to get out of the station.
			}

			_StartEndCheckTimer(executionGeneration);

		}

		private void _loopPlay(long executionGeneration)
		{
			ITiming timingSource;
			ISequence sequence;
			TimeSpan startTime;
			TimeSpan endTime;

			lock (_lifecycleLock)
			{
				if (!_IsCurrentLoopExecution(executionGeneration)) return;

				var endCheckTimer = _endCheckTimer;
				if (endCheckTimer == null) return;

				endCheckTimer.Stop(false);
				timingSource = TimingSource;
				sequence = Sequence;
				startTime = StartTime;
				endTime = EndTime;

				//Reset our position. No need to stop the source, we will just reset its position.
				timingSource.Position = startTime;
				timingSource.Start();
			}

			OnSequenceReStarted(new SequenceStartedEventArgs(sequence, timingSource, startTime, endTime));
			
			while (_IsCurrentExecution(executionGeneration) && timingSource.Position == startTime)
			{
				Thread.Sleep(1); //Give the train a chance to get out of the station.
			}

			_StartEndCheckTimer(executionGeneration);
		}

		protected virtual void _HookDataListener()
		{
			Sequence.InsertDataListener += _DataListener;
		}

		protected virtual void _UnhookDataListener()
		{
			Sequence.InsertDataListener -= _DataListener;
		}

		protected virtual TimeSpan _CoerceStartTime(TimeSpan startTime)
		{
			return startTime < Sequence.Length ? startTime : Sequence.Length;
		}

		protected virtual TimeSpan _CoerceEndTime(TimeSpan endTime)
		{
			return endTime < Sequence.Length ? endTime : Sequence.Length;
		}

		protected virtual ITiming _GetDefaultTimingSource()
		{
			return TimingService.Instance.GetDefaultSequenceTiming();
		}

		protected virtual void _LoadMedia()
		{
			var sequenceMedia = Sequence.GetAllMedia();
            if (sequenceMedia != null)
				foreach (IMediaModuleInstance media in sequenceMedia) {
					media.LoadMedia(StartTime);
				}
		}

		protected virtual void _StartMedia()
		{
			var sequenceMedia = Sequence.GetAllMedia();
			if (sequenceMedia != null)
				foreach (IMediaModuleInstance media in sequenceMedia) {
					media.Start();
				}
		}

		protected virtual void _PauseMedia()
		{
			var sequenceMedia = Sequence.GetAllMedia();
            if (sequenceMedia != null)
				foreach (IMediaModuleInstance media in sequenceMedia) {
					media.Pause();
				}
		}

		protected virtual void _ResumeMedia()
		{
			var sequenceMedia = Sequence.GetAllMedia();
            if (sequenceMedia != null)
				foreach (IMediaModuleInstance media in sequenceMedia) {
					media.Resume();
				}
		}

		protected virtual void _StopMedia()
		{
			var sequenceMedia = Sequence.GetAllMedia();
            if (sequenceMedia != null)
				foreach (IMediaModuleInstance media in sequenceMedia) {
					media.Stop();
				}
		}

		private void _Pause()
		{
			lock (_lifecycleLock)
			{
				if (_isDisposed || !IsRunning || IsPaused) return;

				var endCheckTimer = _endCheckTimer;
				if (endCheckTimer == null || !endCheckTimer.IsRunning) return;

				IsPaused = true;

				TimingSource.Pause();

				_PauseMedia();

				endCheckTimer.Stop(false);
			}
		}

		private void _Resume()
		{
			lock (_lifecycleLock)
			{
				if (_isDisposed || !IsPaused) return;

				var endCheckTimer = _endCheckTimer;
				if (endCheckTimer == null || endCheckTimer.IsRunning || Sequence == null) return;

				IsPaused = false;

				TimingSource.Resume();

				_ResumeMedia();

				endCheckTimer.Start();
			}
		}

		private void _Stop()
		{
			lock (_lifecycleLock)
			{
				if (!IsRunning) return;
				_executionGeneration++;

				var endCheckTimer = _endCheckTimer;
				endCheckTimer?.Stop(false);
			}

			// Release the hook before the behaviors are shut down so that
			// they can affect the sequence.
			//_UnhookDataListener();
			
			TimingSource.Stop();
			_StopMedia();

			IsRunning = false;
			IsPaused = false;

		}

		public ITiming TimingSource { get; private set; }

		protected virtual bool _DataListener(IEffectNode effectNode)
		{
			// We don't want any handlers beyond the executor to get live data.
			return true;
		}

		private void _EndCheckTimerElapsed(object sender, HighResolutionTimerElapsedEventArgs e)
		{
			// To catch events that may trail after the timer's been disabled
			// due to it being a threaded timer and Stop being called between the
			// timer message being posted and acted upon.
			lock (_lifecycleLock)
			{
				if (_isDisposed) return;

				var endCheckTimer = _endCheckTimer;
				if (endCheckTimer == null || !endCheckTimer.IsRunning) return;

				if (_CheckForNaturalEnd() || !IsRunning)
				{
					endCheckTimer.Stop(false);
				}
			}
		}

		private bool _CheckForNaturalEnd()
		{
			lock (_lifecycleLock)
			{
				if (_isDisposed || !IsRunning) return false;

				bool isEnd = _IsEndOfSequence();
				if (isEnd) {
					_PostNaturalEnd(_executionGeneration, _loop);
				}
				return isEnd;
			}
		}

		private void _PostNaturalEnd(long executionGeneration, bool loop)
		{
			var syncContext = _syncContext;
			if (_isDisposed || syncContext == null) return;

			if (loop)
			{
				syncContext.Post(x => _loopPlay((long)x), executionGeneration);
			}
			else
			{
				syncContext.Post(x => _stopAfterNaturalEnd((long)x), executionGeneration);
			}
		}

		private void _stopAfterNaturalEnd(long executionGeneration)
		{
			lock (_lifecycleLock)
			{
				if (!_IsCurrentExecution(executionGeneration)) return;
			}

			_Stop();
		}

		private void _StartEndCheckTimer(long executionGeneration)
		{
			lock (_lifecycleLock)
			{
				if (!_IsCurrentExecution(executionGeneration)) return;

				_endCheckTimer?.Start();
			}
		}

		private bool _IsCurrentLoopExecution(long executionGeneration)
		{
			return _loop && _IsCurrentExecution(executionGeneration);
		}

		private bool _IsCurrentExecution(long executionGeneration)
		{
			return !_isDisposed && IsRunning && executionGeneration == _executionGeneration;
		}

		private bool _IsEndOfSequence()
		{
			TimeSpan position = TimingSource.Position;
			return _IsTimedSequence && (position >= EndTime || position == TimeSpan.Zero);
		}

		protected bool _IsTimedSequence { get; set; }

		#endregion

		#region Dispose

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			HighResolutionTimer endCheckTimer;
			bool wasRunning;

			lock (_lifecycleLock)
			{
				if (_isDisposed) return;

				_isDisposed = true;
				_executionGeneration++;
				wasRunning = IsRunning;
				endCheckTimer = _endCheckTimer;
				_endCheckTimer = null;
				_syncContext = null;
			}

			if (disposing)
			{
				endCheckTimer?.Stop();
				endCheckTimer?.Elapsed -= _EndCheckTimerElapsed;

				if (wasRunning)
				{
					TimingSource?.Stop();
					_StopMedia();
					IsRunning = false;
					IsPaused = false;
				}
			}
		}

		#endregion
	}
}
