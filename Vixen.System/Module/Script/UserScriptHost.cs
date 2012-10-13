using System;
using System.Threading;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Module.Script {
	abstract public class UserScriptHost : IUserScriptHost {
		private Thread _thread;

		public event EventHandler<ExecutorMessageEventArgs> Error;
		public event EventHandler Ended;

		virtual public ISequence Sequence { get; set; }
		virtual public ITiming TimingSource { get; set; }

		public void Start() {
			if(_thread == null) {
				// User Startup procedure.
				// Call here, outside the thread, so that the start of the thread is the true
				// start of the execution.
				Startup();
	
				_thread = new Thread(_PlayThread) { Name = Sequence.Name + " script host" };
				
				// Do NOT.
				//_thread.IsBackground = true;

				_thread.Start();
			}
		}

		public void Stop() {
			if(_thread != null) {
				_thread.Abort();
				_thread = null;
			}
		}

		protected virtual void OnError(ExecutorMessageEventArgs e) {
			if(Error != null) {
				Error(this, e);
			}
		}

		protected virtual void OnEnded(EventArgs e) {
			if(Ended != null) {
				Ended(this, e);
			}
		}

		protected void Wait(TimeSpan timeSpan) {
			TimeSpan startTime = TimingSource.Position;
			TimeSpan targetTime = startTime + timeSpan;
			while(TimingSource.Position < targetTime) {
				Thread.Sleep(1);
			}
		}

		protected abstract void Startup();
		protected abstract void Play();
		protected abstract void Shutdown();

		private void _PlayThread() {
			try {
				// User Play procedure.
				Play();
				// User script has finished executing, but that means nothing about the
				// execution time of the data.
				// Wait until the executor ends the execution due to sequence length
				// (or a user action) and aborts this thread.
				Thread.Sleep(Timeout.Infinite);
			} catch(ThreadAbortException) {
				// Thank you for letting us know that the user wants to stop.
				// You are dismissed.
				Thread.ResetAbort();
			} catch(Exception ex) {
				// Any other exception needs to result in the end of the sequence.
				OnError(new ExecutorMessageEventArgs(Sequence,  ex.Message));
			} finally {
				// User Shutdown procedure.
				Shutdown();
				OnEnded(EventArgs.Empty);
			}
		}

	}
}
