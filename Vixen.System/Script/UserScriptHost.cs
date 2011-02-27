using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Vixen.Common;
using Vixen.Sys;
using Vixen.Module.Sequence;

namespace Vixen.Script {
	abstract public class UserScriptHost {
		private Thread _thread = null;

		public event EventHandler<ExecutorMessageEventArgs> Error;
		public event EventHandler Ended;

		virtual public ScriptSequenceBase Sequence { get; set; }

		public void Start() {
			if(_thread == null) {
				_thread = new Thread(_PlayThread);
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
				Startup();
				Play();
			} catch(ThreadAbortException) {
				// Thank you for letting us know that the user wants to stop.
				// You are dismissed.
				Thread.ResetAbort();
			} catch(Exception ex) {
				// Any other exception needs to result in the end of the sequence.
				OnError(ex.Message);
			} finally {
				Shutdown();
				OnEnded();
			}
		}

	}
}
