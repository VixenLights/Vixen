using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution {
    class ProgramExecutor : IEnumerator<IExecutor> {
        private IEnumerator<IExecutor> _sequences;
        private IExecutor _executor;
		private TimeSpan _startTime, _endTime;
		private TimeSpan _currentSequenceStartTime, _currentSequenceEndTime;
		private FilteringIntentCache _intentCache;
		private IntentBuffer _intentBuffer;

		static public readonly TimeSpan START_ENTIRE_SEQUENCE = TimeSpan.Zero;
		static public readonly TimeSpan END_ENTIRE_SEQUENCE = TimeSpan.MaxValue;
        public enum RunState { Stopped, Playing, Stopping };

        public event EventHandler<ProgramEventArgs> ProgramStarted;
		public event EventHandler<ProgramEventArgs> ProgramEnded;
		public event EventHandler<SequenceStartedEventArgs> SequenceStarted;
		public event EventHandler<SequenceEventArgs> SequenceEnded;
		public event EventHandler<ExecutorMessageEventArgs> Message;
		public event EventHandler<ExecutorMessageEventArgs> Error;

        public ProgramExecutor(Program program) {
            State = RunState.Stopped;
            Program = program;
			// Don't want to create this for every execution.
			_intentCache = new FilteringIntentCache();
		}

		private bool _PlayAllSequences {
			get { return _startTime == START_ENTIRE_SEQUENCE && _endTime == END_ENTIRE_SEQUENCE; }
		}

		public void Play(TimeSpan startTime, TimeSpan endTime) {
            if(Program == null) throw new Exception("No program has been specified");
            if(State == RunState.Stopped) {
                _startTime = startTime;
                _endTime = endTime;
				_sequences = this;
				// Since we are the iterator, there isn't a constructor that can call this
				// at the appropriate time (right now).
				Reset();

                OnProgramStarted();

				// There are no sequences or there was a problem.
				// The end of a sequence normally triggers the ProgramEnded event.
				_NextSequence(_ProgramEnded);
			}
        }

        private void _ProgramEnded() {
            OnProgramEnded();
        }

        public void Pause() {
            if(State == RunState.Playing && !IsPaused) {
                _executor.Pause();
                IsPaused = true;
            }
        }

        public void Resume() {
            if(State == RunState.Playing && IsPaused) {
                _executor.Resume();
                IsPaused = false;
            }
        }

        public void Stop() {
            IsPaused = false;
            if(_executor != null) {
                State = RunState.Stopping; // So that the iterator will break, but it's not yet stopped.
                _executor.Stop();
            }
        }

        public Program Program { get; private set; }

        public RunState State;
        
        public bool IsPaused { get; private set; } // Can be paused and in the Playing state.

    	public bool IsPlaying {
    		get { return _executor != null && _executor.IsPlaying && _intentBuffer != null; }
    	}

    	public IDataSource GetCurrentSequenceData() {
			if(_executor != null) {
				return _intentBuffer;
			}
			return null;
		}

		private IntentBuffer _CreateIntentBuffer() {
			IEnumerable<IEffectNode> sequenceData = _executor.GetSequenceData().OrderBy(x => x.StartTime);
			IEnumerable<IPreFilterNode> sequenceFilters = _executor.GetSequenceFilters();
			// The buffer needs to use the cache as its source of effect data so that it will
			// pull data through the cache and buffer pre-filtered data.
			_intentCache.Use(sequenceData, sequenceFilters, _executor.Name);
			return new IntentBuffer(_intentCache, _executor.Name);
		}

    	public ITiming GetCurrentSequenceTiming() {
			if(_executor != null) {
				return _executor.GetSequenceTiming();
			}
			return null;
		}

		public IEnumerable<IPreFilterNode> GetCurrentSequenceFilters() {
			if(_executor != null) {
				return _executor.GetSequenceFilters();
			}
			return Enumerable.Empty<IPreFilterNode>();
		}

    	#region Events
		protected virtual void OnSequenceStarted(object sender, SequenceStartedEventArgs e) {
			_intentBuffer = _CreateIntentBuffer();
			_intentBuffer.Start();
            if(SequenceStarted != null) {
                SequenceStarted(sender, e);
            }
        }

        protected virtual void OnSequenceEnded(object sender, SequenceEventArgs e) {
            if(SequenceEnded != null) {
                SequenceEnded(sender, e);
            }
			_intentBuffer.Stop();
			_intentBuffer = null;
			_NextSequence(() => {
				State = RunState.Stopped;
				_ProgramEnded();
			});
        }

		protected virtual void OnMessage(object sender, ExecutorMessageEventArgs e) {
			if(Message != null) {
				Message(sender, e);
			}
		}

		protected virtual void OnError(object sender, ExecutorMessageEventArgs e) {
			if(Error != null) {
				Error(sender, e);
			}
		}

		protected virtual void OnProgramStarted() {
            if(ProgramStarted != null) {
                ProgramStarted(null, new ProgramEventArgs(Program));
            }
        }

        protected virtual void OnProgramEnded() {
            if(ProgramEnded != null) {
                ProgramEnded(null, new ProgramEventArgs(Program));
            }
        }
		#endregion

		private void _NextSequence(Action actionOnFail) {
            _executor = null;
			try {
				if(State != RunState.Stopping && _sequences.MoveNext()) {
					_executor = _sequences.Current;
					State = RunState.Playing;
					_executor.Play(_currentSequenceStartTime, _currentSequenceEndTime);
				}
			} catch(Exception ex) {
				VixenSystem.Logging.Error(ex);
				_executor = null;
			} finally {
				// Allow an exception to propogate after the caller's failure action
				// has had a chance to execute.
				if(_executor == null) {
					actionOnFail();
				}
			}
        }

        ~ProgramExecutor() {
            Dispose();
        }

        public void Dispose() {
            Stop();
            if(_executor != null) {
                _executor.Dispose();
				_executor = null;
            }
			if(_intentCache != null) {
				_intentCache.Dispose();
				_intentCache = null;
			}
        	GC.SuppressFinalize(this);
        }


		#region Sequence enumerator enabling queuing

		// Keep an eye on both:
		// - This is an executor of a static set of sequences that will be played repeatedly.
		// - This is a queue of sequences that will be added to while it's executing.
		// Has to be a TimedSequence because otherwise there will be no end
		// time to watch for to move the program along.

		// This cannot be genericized because we need access to the event handlers
		// inside this class.

		private Queue<ISequence> _sequenceQueue;
		private IExecutor _cursor;

		public IExecutor Current {
			get { return _cursor; }
		}

		object System.Collections.IEnumerator.Current {
			get { return Current; }
		}

		public bool MoveNext() {
			if(_cursor != null) {
				// Cleanup after the prior sequence.
				_cursor.SequenceStarted -= OnSequenceStarted;
				_cursor.SequenceEnded -= OnSequenceEnded;
				_cursor.Message -= OnMessage;
				_cursor.Error -= OnError;
				_cursor.Dispose();
				_cursor = null;
				if(State != RunState.Playing) return false;
			}

			// Anything left to play?
			if(_sequenceQueue.Count > 0) {
				// Get the sequence.
				ISequence sequence = _sequenceQueue.Dequeue();
				// Get an executor for the sequence.
				IExecutor sequenceExecutor = SequenceExecutor.GetExecutor(sequence);
				if(sequenceExecutor == null) throw new InvalidOperationException("Sequence " + sequence.Name + " has no executor.");
				
				// Set the time span to be played.
				_currentSequenceStartTime = TimeSpan.Zero > _startTime ? TimeSpan.Zero : _startTime;
				_currentSequenceEndTime = (_endTime == END_ENTIRE_SEQUENCE) ? sequence.Length : _endTime;

				sequenceExecutor.SequenceStarted += OnSequenceStarted;
				sequenceExecutor.SequenceEnded += OnSequenceEnded;
				sequenceExecutor.Message += OnMessage;
				sequenceExecutor.Error += OnError;

				_cursor = sequenceExecutor;
				return true;
			}
			return false;
		}

		public void Reset() {
			// According to MSDN, this is only provided for COM interop and does not need to
			// be implemented.
			if(Program != null) {
				if(_PlayAllSequences) {
					_sequenceQueue = new Queue<ISequence>(Program.Sequences);
				} else {
					_sequenceQueue = new Queue<ISequence>();
					ISequence sequence = Program.Sequences.FirstOrDefault();
					if(sequence != null) {
						_sequenceQueue.Enqueue(sequence);
					}
				}

				if(_cursor != null) {
					_cursor.Dispose();
					_cursor = null;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequence"></param>
		/// <returns>The resulting length of the queue.  0 if it cannot be added.</returns>
		public int Queue(ISequence sequence) {
			// _sequenceQueue will only be around if the program is being enumerated
			// for execution.
			if(_sequenceQueue != null) {
				_sequenceQueue.Enqueue(sequence);
				return _sequenceQueue.Count;
			}
			return 0;
		}

		#endregion
    }
}
