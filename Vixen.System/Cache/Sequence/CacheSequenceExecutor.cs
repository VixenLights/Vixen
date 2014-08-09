using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Execution;
using Vixen.Module.Timing;
using Vixen.Services;
using Vixen.Sys;

namespace Vixen.Cache.Sequence
{
	public class CacheSequenceExecutor:ISequenceExecutor
	{
		private bool _isRunning;
		public event EventHandler<SequenceStartedEventArgs> SequenceStarted;
		public event EventHandler<SequenceEventArgs> SequenceEnded;
		public event EventHandler<ExecutorMessageEventArgs> Message;
		public event EventHandler<ExecutorMessageEventArgs> Error;

		public ISequence Sequence { get; set; }

		public void Play(TimeSpan startTime, TimeSpan endTime)
		{
			if (IsRunning) return;
			if (Sequence == null) return;

			// Only hook the input stream during execution.
			// Hook before starting the behaviors.
			//_HookDataListener();

			// Bound the execution range.
			StartTime = _CoerceStartTime(startTime);
			EndTime = _CoerceEndTime(endTime);
			TimingSource = _GetDefaultTimingSource();
			TimingSource.Position = StartTime;
			TimingSource.Start();
			IsRunning = true;
		}

		public void PlayLoop(TimeSpan startTime, TimeSpan endTime)
		{
			throw new NotImplementedException();
		}

		public void Start()
		{
			Play(TimeSpan.Zero, TimeSpan.MaxValue);
		}

		public void Stop()
		{
			if (!IsRunning) return;
			IsRunning = false;
			IsPaused = false;

			TimingSource.Stop();
		}

		public void Pause()
		{
			throw new NotImplementedException();
		}

		public void Resume()
		{
			throw new NotImplementedException();
		}

		public bool IsRunning
		{
			get { return _isRunning; }
			private set
			{
				_isRunning = value;
				if (_isRunning)
				{
					OnSequenceStarted(new SequenceStartedEventArgs(Sequence, TimingSource, StartTime, EndTime));
				} else
				{
					OnSequenceEnded(new SequenceEventArgs(Sequence));
				}
			}
		}
		public bool IsPaused { get; private set; }

		#region Events

		protected virtual void OnSequenceStarted(SequenceStartedEventArgs e)
		{
			if (SequenceStarted != null)
			{
				SequenceStarted(null, e);
			}
		}

		protected virtual void OnSequenceEnded(SequenceEventArgs e)
		{
			if (SequenceEnded != null)
			{
				SequenceEnded(null, e);
			}
		}

		protected virtual void OnMessage(ExecutorMessageEventArgs e)
		{
			if (Message != null)
			{
				Message(null, e);
			}
		}

		protected virtual void OnError(ExecutorMessageEventArgs e)
		{
			if (Error != null)
			{
				Error(Sequence, e);
			}
		}

		#endregion

		public string Name
		{
			get
			{
				if (Sequence != null)
				{
					return string.Format("Cache: {0}", Sequence.Name);
				}
				return null;
			}
		}
		public ITiming TimingSource { get; private set; }
		public IEnumerable<ISequenceFilterNode> SequenceFilters { get; private set; }

		protected virtual TimeSpan _CoerceStartTime(TimeSpan startTime)
		{
			return startTime < Sequence.Length ? startTime : Sequence.Length;
		}

		protected virtual TimeSpan _CoerceEndTime(TimeSpan endTime)
		{
			return endTime < Sequence.Length ? endTime : Sequence.Length;
		}

		// Because these are calculated values, changing the length of the sequence
		// during execution will not affect the end time.
		public TimeSpan StartTime { get; protected set; }

		public TimeSpan EndTime { get; protected set; }

		protected virtual ITiming _GetDefaultTimingSource()
		{
			return new SequenceCacheTiming();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
