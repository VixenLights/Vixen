using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Vixen.Module.Trigger {
	/// <summary>
	/// Base class for trigger module implementations.
	/// </summary>
	abstract public class TriggerBase : ITriggerModuleInstance, IEquatable<TriggerBase>, IEquatable<ITriggerModuleInstance> {
		private Thread _stateUpdateThread;
		private ManualResetEvent _pause = new ManualResetEvent(true);

		public event EventHandler<TriggerSetEventArgs> TriggerSet;

		abstract public ITriggerInput[] TriggerInputs { get; }

		abstract public void UpdateState();

		public void Start() {
			if(!IsRunning) {
				// Call the subclass first in case its startup creates the triggers.
				DoStartup();
				// Subscribe to triggers.
				foreach(ITriggerInput trigger in TriggerInputs) {
					trigger.Set += _TriggerSet;
				}
				// Start monitoring the hardware.
				_stateUpdateThread = new Thread(_StateUpdate);
				_stateUpdateThread.IsBackground = true;
				_stateUpdateThread.Start();
			}
		}

		protected virtual void DoStartup() { }

		public void Stop() {
			// In opposite order of Startup...
			if(IsRunning) {
				// Stop monitoring the hardware.
				Resume();
				IsRunning = false;
				// Unsubscribe to triggers.
				foreach(ITriggerInput trigger in TriggerInputs) {
					trigger.Set -= _TriggerSet;
				}
				// Notify the subclass.
				DoShutdown();
			}
		}

		protected virtual void DoShutdown() { }

		public void Pause() {
			if(!IsPaused) {
				IsPaused = true;
				_pause.Reset();
				DoPause();
			}
		}

		protected virtual void DoPause() { }

		public void Resume() {
			if(IsPaused) {
				IsPaused = false;
				_pause.Set();
				DoResume();
			}
		}

		protected virtual void DoResume() { }

		public bool IsRunning { get; private set; }

		public bool IsPaused { get; private set; }

		abstract public bool Setup();

		abstract public Guid TypeId { get; }

		public Guid InstanceId { get; set; }

		virtual public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			Stop();
			_pause.Dispose();
			_pause = null;
		}

		~TriggerBase() {
			Dispose(false);
		}

		private void _TriggerSet(object sender, EventArgs e) {
			if(TriggerSet != null) {
				TriggerSet(this, new TriggerSetEventArgs(sender as ITriggerInput));
			}
		}

		private void _StateUpdate() {
			IsRunning = true;
			while(IsRunning) {
				UpdateState();
				Thread.Sleep(5);
				_pause.WaitOne();
			}
			_stateUpdateThread = null;
		}

		public bool Equals(TriggerBase other) {
			return this.InstanceId.Equals(other.InstanceId);
		}

		public bool Equals(ITriggerModuleInstance other) {
			return this.InstanceId.Equals(other.InstanceId);
		}
	}

}
