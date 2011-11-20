using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Execution {
	/// <summary>
	/// Execution context for a Program of sequences.
	/// </summary>
	public class ProgramContext : IDisposable {
		private ProgramExecutor _programExecutor;

		public event EventHandler<SequenceStartedEventArgs> SequenceStarted;
		public event EventHandler<SequenceEventArgs> SequenceEnded;
		public event EventHandler<ProgramEventArgs> ProgramStarted;
		public event EventHandler<ProgramEventArgs> ProgramEnded;
		public event EventHandler<ExecutorMessageEventArgs> Message;
		public event EventHandler<ExecutorMessageEventArgs> Error;

		public ProgramContext(Program program) {
			this.Program = program;
			_programExecutor = new ProgramExecutor(this.Program);
			Id = Guid.NewGuid();

			_programExecutor.SequenceStarted += _programExecutor_SequenceStarted;
			_programExecutor.SequenceEnded += _programExecutor_SequenceEnded;
			_programExecutor.ProgramStarted += _programExecutor_ProgramStarted;
			_programExecutor.ProgramEnded += _programExecutor_ProgramEnded;
			_programExecutor.Message += _programExecutor_Message;
			_programExecutor.Error += _programExecutor_Error;
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

		public bool Play(TimeSpan startTime, TimeSpan endTime) {
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequence"></param>
		/// <returns>The resulting length of the queue.  0 if it cannot be added.</returns>
		public int Queue(ISequence sequence) {
			if(_programExecutor != null) {
				return _programExecutor.Queue(sequence);
			}
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns>True if execution entered the playing state.</returns>
		private bool _Play(TimeSpan startTime, TimeSpan endTime) {
			try {
				if(!IsPlaying) {
					_programExecutor.Play(startTime, endTime);
				} else if(IsPaused) {
					// Paused, must be resumed.
					_programExecutor.Resume();
				}
				return true;
			} catch(Exception ex) {
				VixenSystem.Logging.Error(ex);
				return false;
			}
		}

		~ProgramContext() {
			Dispose();
		}

		public void Dispose() {
			Stop();

			_programExecutor.SequenceStarted -= _programExecutor_SequenceStarted;
			_programExecutor.SequenceEnded -= _programExecutor_SequenceEnded;
			_programExecutor.ProgramStarted -= _programExecutor_ProgramStarted;
			_programExecutor.ProgramEnded -= _programExecutor_ProgramEnded;
			_programExecutor.Message -= _programExecutor_Message;
			_programExecutor.Error -= _programExecutor_Error;

			_programExecutor.Dispose();
			_programExecutor = null;

			// In case we're being disposed by something other than the
			// act of being released.
			Vixen.Sys.Execution.ReleaseContext(this);
			GC.SuppressFinalize(this);
		}

		#region Events
		void _programExecutor_Error(object sender, ExecutorMessageEventArgs e) {
			if(Error != null) {
				Error(this, e);
			}
		}

		void _programExecutor_Message(object sender, ExecutorMessageEventArgs e) {
			if(Message != null) {
				Message(this, e);
			}
		}

		void _programExecutor_ProgramEnded(object sender, ProgramEventArgs e) {
			if(ProgramEnded != null) {
				ProgramEnded(this, e);
			}
		}

		void _programExecutor_ProgramStarted(object sender, ProgramEventArgs e) {
			if(ProgramStarted != null) {
				ProgramStarted(this, e);
			}
		}

		void _programExecutor_SequenceEnded(object sender, SequenceEventArgs e) {
			if(SequenceEnded != null) {
				SequenceEnded(this, e);
			}
		}

		void _programExecutor_SequenceStarted(object sender, SequenceStartedEventArgs e) {
			if(SequenceStarted != null) {
				SequenceStarted(this, e);
			}
		}
		#endregion
	}
}
