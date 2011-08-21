using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Vixen.Sys;
using Vixen.Module.Sequence;

namespace Vixen.Script {
	// Public for VB inheritance.
	abstract class UserScriptHost : IUserScriptHost {
		private Thread _thread = null;

		public event EventHandler<ExecutorMessageEventArgs> Error;
		public event EventHandler Ended;

		virtual public ScriptSequence Sequence { get; set; }

		public void Start() {
			if(_thread == null) {
				_thread = new Thread(_PlayThread);
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

		protected virtual void OnError(string value) {
			if(Error != null) {
				Error(this, new ExecutorMessageEventArgs(value));
			}
		}

		protected virtual void OnEnded() {
			if(Ended != null) {
				Ended(this, EventArgs.Empty);
			}
		}

		protected abstract void Startup();
		protected abstract void Play();
		protected abstract void Shutdown();

		private void _PlayThread() {
			try {
				// User Startup procedure.
				Startup();
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
				OnError(ex.Message);
			} finally {
				// User Shutdown procedure.
				Shutdown();
				OnEnded();
			}
		}

	}
}
