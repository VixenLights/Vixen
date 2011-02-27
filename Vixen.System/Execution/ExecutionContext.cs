using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sys;
using Vixen.Sequence;
using Vixen.Module.Sequence;

namespace Vixen.Execution {
	public class ExecutionContext : IDisposable {
		private ProgramExecutor _programExecutor;

		public ExecutionContext(Program program) {
			this.Program = program;
			_programExecutor = new ProgramExecutor(this.Program);
			Id = Guid.NewGuid();
		}

		public Program Program { get; private set; }

		public Guid Id { get; private set; }

		public string Name {
			get {
				if(this.Program != null) {
					return this.Program.Name;
				}
				return null;
			}
		}

		public bool Play() {
			return _Play(ProgramExecutor.START_ENTIRE_SEQUENCE, ProgramExecutor.END_ENTIRE_SEQUENCE);
		}

		public bool Play(int startTime, int endTime) {
			return _Play(startTime, endTime);
		}

		public void Pause() {
			_programExecutor.Pause();
		}

		public void Stop() {
			if(IsPlaying) { // Which will be true even if paused.
				_programExecutor.Stop();
			}
		}

		public bool IsPaused {
			get { return _programExecutor.IsPaused; }
		}

		public bool IsPlaying {
			get {
				return _programExecutor.State == ProgramExecutor.RunState.Playing;
			}
		}

		public void Queue(ISequenceModuleInstance sequence) {
			if(_programExecutor != null) {
				_programExecutor.Queue(sequence);
			}
		}

		private bool _Play(int startTime, int endTime) {
			try {
				if(!IsPlaying) {
					_programExecutor.Play(startTime, endTime);
				} else if(IsPaused) {
					// Paused, must be resumed.
					_programExecutor.Resume();
				}
				return true;
			} catch(Exception ex) {
				Server.Logging.Error(ex);
				return false;
			}
		}

		// Events are pass-through.
		public event EventHandler<SequenceStartedEventArgs> SequenceStarted {
			add { _programExecutor.SequenceStarted += value; }
			remove { _programExecutor.SequenceStarted -= value; }
		}

		public event EventHandler SequenceEnded {
			add { _programExecutor.SequenceEnded += value;	}
			remove { _programExecutor.SequenceEnded -= value; }
		}

		public event EventHandler ProgramStarted {
			add { _programExecutor.ProgramStarted += value; }
			remove { _programExecutor.ProgramStarted -= value; }
		}

		public event EventHandler ProgramEnded {
			add { _programExecutor.ProgramEnded += value; }
			remove { _programExecutor.ProgramEnded -= value; }
		}

		public event EventHandler<ExecutorMessageEventArgs> Message {
			add { _programExecutor.Message += value; }
			remove { _programExecutor.Message -= value; }
		}

		public event EventHandler<ExecutorMessageEventArgs> Error {
			add { _programExecutor.Error += value; }
			remove { _programExecutor.Error -= value; }
		}

		~ExecutionContext() {
			Dispose();
		}

		public void Dispose() {
			Stop();
			_programExecutor.Dispose();
			Vixen.Sys.Execution.ReleaseContext(this);
			GC.SuppressFinalize(this);
		}
	}
}
