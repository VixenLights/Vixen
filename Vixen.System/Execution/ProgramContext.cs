using System;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution {
	/// <summary>
	/// Execution context for a Program of sequences.
	/// </summary>
	public class ProgramContext : Context {
		private ProgramExecutor _programExecutor;
		private IDataSource _dataSource;
		private ITiming _timingSource;

		public event EventHandler<SequenceStartedEventArgs> SequenceStarted;
		public event EventHandler<SequenceEventArgs> SequenceEnded;
		public event EventHandler<ProgramEventArgs> ProgramStarted;
		public event EventHandler<ProgramEventArgs> ProgramEnded;
		public event EventHandler<ExecutorMessageEventArgs> Message;
		public event EventHandler<ExecutorMessageEventArgs> Error;

		public ProgramContext(Program program)
			: base(program.Name) {
			Program = program;
			_programExecutor = new ProgramExecutor(Program);

			_programExecutor.SequenceStarted += _programExecutor_SequenceStarted;
			_programExecutor.SequenceEnded += _programExecutor_SequenceEnded;
			_programExecutor.ProgramStarted += _programExecutor_ProgramStarted;
			_programExecutor.ProgramEnded += _programExecutor_ProgramEnded;
			_programExecutor.Message += _programExecutor_Message;
			_programExecutor.Error += _programExecutor_Error;
		}

		public Program Program { get; private set; }

		public override bool IsPlaying {
			get { return _ProgramExecutorIsPlaying() && _DataIsReady(); }
		}

		protected override bool _OnPlay(TimeSpan startTime, TimeSpan endTime) {
			_programExecutor.Play(startTime, endTime);
			return true;
		}

		protected override void _OnPause() {
			_programExecutor.Pause();
		}

		protected override void _OnResume() {
			_programExecutor.Resume();
		}

		protected override void _OnStop() {
			_programExecutor.Stop();
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

		~ProgramContext() {
			Dispose();
		}

		override protected void Dispose(bool disposing) {
			if(disposing) {
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
				VixenSystem.Contexts.ReleaseContext(this);
			}
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
			OnContextEnded(EventArgs.Empty);
		}

		void _programExecutor_ProgramStarted(object sender, ProgramEventArgs e) {
			OnContextStarted(EventArgs.Empty);
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
			_dataSource = _programExecutor.GetCurrentSequenceData();
			_timingSource = _programExecutor.GetCurrentSequenceTiming();
			
			//Once a sequence is executed with this present, it can be saved and this
			//line commented out.  The sequence file will have it at that point.
			//****
			//_AddPreFilterToSequence();
			//****

			if(SequenceStarted != null) {
				SequenceStarted(this, e);
			}
		}

		//private void _AddPreFilterToSequence() {
		//    //Don't want it applied every time the sequence is started.  The result is cumulative.
		//    if(_programExecutor.Current.Sequence.GetAllSequenceFilters().Any()) return;

		//    IPreFilterModuleInstance preFilter = Modules.ModuleManagement.GetPreFilter(new Guid("{E0E26570-6A01-4368-B996-E34576FF4910}"));
		//    //Fade out over the first 5 seconds.
		//    preFilter.TimeSpan = TimeSpan.FromSeconds(5);
		//    //Every channel.
		//    preFilter.TargetNodes = VixenSystem.Nodes.ToArray();
		//    //The first three channels of my 1024-channel group.
		//    //preFilter.TargetNodes = VixenSystem.Nodes.Skip(64).Take(3).ToArray();
		//    //Starting right away.
		//    _programExecutor.Current.Sequence.AddSequenceFilter(new sequenceFilterNode(preFilter, TimeSpan.Zero));
		//}

		protected override IDataSource _DataSource {
			get { return _dataSource; }
		}

		protected override ITiming _TimingSource {
			get { return _timingSource; }
		}

		#endregion

		private bool _ProgramExecutorIsPlaying() {
			return _programExecutor != null && _programExecutor.IsPlaying;
		}

		private bool _DataIsReady() {
			return _dataSource != null && _timingSource != null;
		}
	}
}
