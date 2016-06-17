using System;
using Vixen.Execution.DataSource;
using Vixen.Module.Timing;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Execution.Context
{
	public abstract class ProgramContext : ContextBase, IProgramContext
	{
		private IProgramExecutor _programExecutor;
		private IProgram _program;

		public event EventHandler<SequenceStartedEventArgs> SequenceStarted;
		public event EventHandler<SequenceEventArgs> SequenceEnded;
		public event EventHandler<ProgramEventArgs> ProgramStarted;
		public event EventHandler<ProgramEventArgs> ProgramEnded;
		public event EventHandler<ExecutorMessageEventArgs> Message;
		public event EventHandler<ExecutorMessageEventArgs> Error;

		public virtual IProgram Program
		{
			get { return _program; }
			set
			{
				_program = value;
				_AssignProgramToExecutor();
			}
		}

		protected override ILayer GetLayerForNode(IEffectNode node)
		{
			return _programExecutor.SequenceLayers.GetLayer(node);
		}

		public override IExecutor Executor
		{
			set
			{
				if (_programExecutor != null) {
					_DisposeProgramExecutor();
				}

				if (!(value is IProgramExecutor)) {
					throw new InvalidOperationException("Attempt to use a non-program executor with a program context.");
				}

				_programExecutor = (IProgramExecutor) value;

				if (_programExecutor != null) {
					_AssignProgramToExecutor();
					_AssignEventHandlers();
					_programExecutor.DataSource = ContextDataSource;
				}
			}
		}

		public override string Name
		{
			get { return (_programExecutor != null) ? _programExecutor.Name : null; }
		}

		public override bool IsRunning
		{
			get { return _ProgramExecutorIsPlaying() && _DataIsReady(); }
		}

		public override bool IsPaused
		{
			get { return _ProgramExecutorIsPaused(); }
		}

		public abstract IMutableDataSource ContextDataSource { get; }

		/// <returns>The resulting length of the queue.  0 if it cannot be added.</returns>
		public int Queue(ISequence sequence)
		{
			if (_programExecutor != null) {
				return _programExecutor.Queue(sequence);
			}
			return 0;
		}

		protected override void _OnStart()
		{
			_programExecutor.Start();
		}

		protected override void _OnPause()
		{
			_programExecutor.Pause();
		}

		protected override void _OnResume()
		{
			_programExecutor.Resume();
		}

		protected override void _OnStop()
		{
			_programExecutor.Stop();
		}

		protected override IDataSource _DataSource
		{
			get { return ContextDataSource; }
		}

		protected override ITiming _SequenceTiming
		{
			get { return _programExecutor != null ? _programExecutor.TimingSource : null; }
		}

		private void _AssignProgramToExecutor()
		{
			if (_programExecutor != null) {
				_programExecutor.Program = Program;
			}
		}

		private void _AssignEventHandlers()
		{
			_programExecutor.SequenceStarted += _ProgramExecutorSequenceStarted;
			_programExecutor.SequenceEnded += _ProgramExecutorSequenceEnded;
			_programExecutor.ProgramStarted += _ProgramExecutorProgramStarted;
			_programExecutor.ProgramEnded += _ProgramExecutorProgramEnded;
			_programExecutor.Message += _ProgramExecutorMessage;
			_programExecutor.Error += _ProgramExecutorError;
		}

		private void _RemoveEventHandlers()
		{
			_programExecutor.SequenceStarted -= _ProgramExecutorSequenceStarted;
			_programExecutor.SequenceEnded -= _ProgramExecutorSequenceEnded;
			_programExecutor.ProgramStarted -= _ProgramExecutorProgramStarted;
			_programExecutor.ProgramEnded -= _ProgramExecutorProgramEnded;
			_programExecutor.Message -= _ProgramExecutorMessage;
			_programExecutor.Error -= _ProgramExecutorError;
		}

		private void _DisposeProgramExecutor()
		{
			_RemoveEventHandlers();
			_programExecutor.Dispose();
			_programExecutor = null;
		}

		private bool _ProgramExecutorIsPlaying()
		{
			return _programExecutor != null && _programExecutor.IsRunning;
		}

		private bool _ProgramExecutorIsPaused()
		{
			return _programExecutor != null && _programExecutor.IsPaused;
		}

		private bool _DataIsReady()
		{
			return _DataSource != null && _SequenceTiming != null;
		}

		#region Events

		private void _ProgramExecutorProgramStarted(object sender, ProgramEventArgs e)
		{
			OnContextStarted(EventArgs.Empty);
			OnProgramStarted(e);
		}

		protected virtual void OnProgramStarted(ProgramEventArgs e)
		{
			if (ProgramStarted != null) {
				ProgramStarted(this, e);
			}
		}

		private void _ProgramExecutorProgramEnded(object sender, ProgramEventArgs e)
		{
			OnProgramEnded(e);
			OnContextEnded(EventArgs.Empty);
		}

		protected virtual void OnProgramEnded(ProgramEventArgs e)
		{
			if (ProgramEnded != null) {
				ProgramEnded(this, e);
			}
		}

		private void _ProgramExecutorMessage(object sender, ExecutorMessageEventArgs e)
		{
			OnMessage(e);
		}

		protected virtual void OnMessage(ExecutorMessageEventArgs e)
		{
			if (Message != null) {
				Message(this, e);
			}
		}

		private void _ProgramExecutorError(object sender, ExecutorMessageEventArgs e)
		{
			OnError(e);
		}

		protected virtual void OnError(ExecutorMessageEventArgs e)
		{
			if (Error != null) {
				Error(this, e);
			}
		}

		private void _ProgramExecutorSequenceStarted(object sender, SequenceStartedEventArgs e)
		{
			OnSequenceStarted(e);
		}

		protected virtual void OnSequenceStarted(SequenceStartedEventArgs e)
		{
			if (SequenceStarted != null) {
				SequenceStarted(this, e);
			}
		}

		private void _ProgramExecutorSequenceEnded(object sender, SequenceEventArgs e)
		{
			OnSequenceEnded(e);
		}

		protected virtual void OnSequenceEnded(SequenceEventArgs e)
		{
			if (SequenceEnded != null) {
				SequenceEnded(this, e);
			}
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_DisposeProgramExecutor();
			}
			base.Dispose(disposing);
		}
	}
}