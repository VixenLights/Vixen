using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Execution.DataSource;
using Vixen.Module.Timing;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Execution
{
	public class ProgramExecutor : IProgramExecutor
	{
		private QueuingSequenceEnumerator _sequenceEnumerator;
		private ISequenceExecutor _sequenceExecutor;
		private RunState _state;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private enum RunState
		{
			Stopped,
			Playing,
			Stopping
		};

		public event EventHandler<ProgramEventArgs> ProgramStarted;
		public event EventHandler<ProgramEventArgs> ProgramEnded;
		public event EventHandler<SequenceStartedEventArgs> SequenceStarted;
		public event EventHandler<SequenceStartedEventArgs> SequenceReStarted;
		public event EventHandler<SequenceEventArgs> SequenceEnded;
		public event EventHandler<ExecutorMessageEventArgs> Message;
		public event EventHandler<ExecutorMessageEventArgs> Error;

		public ProgramExecutor()
		{
			_state = RunState.Stopped;
		}

		public ProgramExecutor(IProgram program)
			: this()
		{
			Program = program;
		}

		public string Name
		{
			get { return Program.Name; }
		}

		public void Start()
		{
			if (Program == null) throw new Exception("No program has been specified");
			if (_state == RunState.Stopped) {
				_sequenceEnumerator = new QueuingSequenceEnumerator(Program);

				OnProgramStarted();

				// There are no sequences or there was a problem.
				// The end of a sequence normally triggers the ProgramEnded event.
				_NextSequence(_ProgramEnded);
			}
		}

		public void Pause()
		{
			if (_state == RunState.Playing && !IsPaused) {
				_sequenceExecutor.Pause();
				IsPaused = true;
			}
		}

		public void Resume()
		{
			if (_state == RunState.Playing && IsPaused) {
				_sequenceExecutor.Resume();
				IsPaused = false;
			}
		}

		public void Stop()
		{
			IsPaused = false;
			if (_sequenceExecutor != null) {
				_state = RunState.Stopping; // So that the iterator will break, but it's not yet stopped.
				_sequenceExecutor.Stop();
			}
		}

		public IProgram Program { get; set; }

		public bool IsPaused { get; private set; } // Can be paused and in the Playing state.

		public bool IsRunning
		{
			get { return _sequenceExecutor != null && _sequenceExecutor.IsRunning && DataSource != null; }
		}

		/// <summary>
		/// The data scheme the program executor should enforce in its sequence executors.
		/// This does not affect the behavior of the program executor directly.
		/// </summary>
		public IMutableDataSource DataSource { private get; set; }

		public ITiming TimingSource
		{
			get
			{
				if (_sequenceExecutor != null) {
					return _sequenceExecutor.TimingSource;
				}
				return null;
			}
		}

		public IEnumerable<ISequenceFilterNode> SequenceFilters
		{
			get
			{
				if (_sequenceExecutor != null) {
					return _sequenceExecutor.SequenceFilters;
				}
				return Enumerable.Empty<ISequenceFilterNode>();
			}
		}

		public SequenceLayers SequenceLayers
		{
			get { return _sequenceExecutor.Sequence.GetSequenceLayerManager(); }
		}

		/// <summary>
		/// Dynamically queues a sequence to be executed by the program.
		/// </summary>
		/// <param name="sequence"></param>
		/// <returns>The resulting length of the queue.  0 if it cannot be added.</returns>
		public int Queue(ISequence sequence)
		{
			if (_sequenceEnumerator != null) {
				return _sequenceEnumerator.Queue(sequence);
			}
			return 0;
		}

		#region Events

		private void _SequenceExecutorSequenceStarted(object sender, SequenceStartedEventArgs e)
		{
			DataSource.SetSequence(_sequenceExecutor.Sequence);
			DataSource.Start();
			OnSequenceStarted(e);
		}

		protected virtual void OnSequenceStarted(SequenceStartedEventArgs e)
		{
			if (SequenceStarted != null) {
				SequenceStarted(this, e);
			}
		}

		private void _SequenceExecutorSequenceEnded(object sender, SequenceEventArgs e)
		{
			DataSource.Stop();
			_NextSequence(() =>
			              	{
			              		_state = RunState.Stopped;
			              		_ProgramEnded();
			              	});
			OnSequenceEnded(e);
		}

		protected virtual void OnSequenceEnded(SequenceEventArgs e)
		{
			if (SequenceEnded != null) {
				SequenceEnded(this, e);
			}
		}

		private void _SequenceExecutorMessage(object sender, ExecutorMessageEventArgs e)
		{
			OnMessage(e);
		}

		protected virtual void OnMessage(ExecutorMessageEventArgs e)
		{
			if (Message != null) {
				Message(this, e);
			}
		}

		private void _SequenceExecutorError(object sender, ExecutorMessageEventArgs e)
		{
			OnError(e);
		}

		protected virtual void OnError(ExecutorMessageEventArgs e)
		{
			if (Error != null) {
				Error(this, e);
			}
		}

		protected virtual void OnProgramStarted()
		{
			if (ProgramStarted != null) {
				ProgramStarted(null, new ProgramEventArgs(Program));
			}
		}

		protected virtual void OnProgramEnded()
		{
			if (ProgramEnded != null) {
				ProgramEnded(null, new ProgramEventArgs(Program));
			}
		}

		#endregion

		private void _ProgramEnded()
		{
			OnProgramEnded();
		}

		private void _NextSequence(Action actionOnFail)
		{
			_DisposeSequenceExecutor();

			try {
				if (_state != RunState.Stopping && _sequenceEnumerator.MoveNext()) {
					_sequenceExecutor = _sequenceEnumerator.Current;

					_AssignEventHandlers();

					_state = RunState.Playing;
					_sequenceExecutor.Start();
				}
			}
			catch (Exception ex) {
				Logging.Error(ex, ex.Message);
				_DisposeSequenceExecutor();
			}
			finally {
				// Allow an exception to propogate after the caller's failure action
				// has had a chance to execute.
				if (_sequenceExecutor == null) {
					actionOnFail();
				}
			}
		}

		private void _DisposeSequenceExecutor()
		{
			if (_sequenceExecutor != null) {
				_RemoveEventHandlers();
				_sequenceExecutor.Dispose();
				_sequenceExecutor = null;
			}
		}

		private void _AssignEventHandlers()
		{
			if (_sequenceExecutor == null) return;
			_sequenceExecutor.SequenceStarted += _SequenceExecutorSequenceStarted;
			_sequenceExecutor.SequenceEnded += _SequenceExecutorSequenceEnded;
			_sequenceExecutor.Message += _SequenceExecutorMessage;
			_sequenceExecutor.Error += _SequenceExecutorError;
		}

		private void _RemoveEventHandlers()
		{
			if (_sequenceExecutor == null) return;
			_sequenceExecutor.SequenceStarted -= _SequenceExecutorSequenceStarted;
			_sequenceExecutor.SequenceEnded -= _SequenceExecutorSequenceEnded;
			_sequenceExecutor.Message -= _SequenceExecutorMessage;
			_sequenceExecutor.Error -= _SequenceExecutorError;
		}

		~ProgramExecutor()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		public virtual void Dispose(bool disposing)
		{
			Stop();
			if (_sequenceEnumerator != null) {
				_sequenceEnumerator.Dispose();
				_sequenceEnumerator = null;
			}
			_DisposeSequenceExecutor();
			GC.SuppressFinalize(this);
		}
	}
}