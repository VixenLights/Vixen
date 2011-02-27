using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Vixen.Module.Input {
	/// <summary>
	/// Base class for trigger module implementations.
	/// </summary>
	abstract public class InputBase : IInputModuleInstance, IEquatable<InputBase>, IEquatable<IInputModuleInstance> {
		private Thread _stateUpdateThread;
		private ManualResetEvent _pause = new ManualResetEvent(true);

		public event EventHandler<InputValueChangedEventArgs> InputValueChanged;

		abstract public IInputInput[] InputInputs { get; }

		abstract public void UpdateState();

		public bool Enabled { get; set; }

		public void Startup() {
			if(!IsRunning) {
				// Call the subclass first in case its startup creates the triggers.
				DoStartup();
				// Subscribe to triggers.
				foreach(IInputInput input in InputInputs) {
					input.ValueChanged += _InputValueChanged;
				}
				// Start monitoring the hardware.
				_stateUpdateThread = new Thread(_StateUpdate);
				_stateUpdateThread.IsBackground = true;
				_stateUpdateThread.Start();
			}
		}

		protected virtual void DoStartup() { }

		public void Shutdown() {
			// In opposite order of Startup...
			if(IsRunning) {
				// Stop monitoring the hardware.
				Resume();
				IsRunning = false;
				// Unsubscribe to triggers.
				foreach(IInputInput input in InputInputs) {
					input.ValueChanged -= _InputValueChanged;
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
			Shutdown();
			_pause.Dispose();
			_pause = null;
		}

		~InputBase() {
			Dispose(false);
		}

		private void _InputValueChanged(object sender, EventArgs e) {
			if(InputValueChanged != null) {
				InputValueChanged(this, new InputValueChangedEventArgs(sender as IInputInput));
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

		public bool Equals(InputBase other) {
			return this.InstanceId.Equals(other.InstanceId);
		}

		public bool Equals(IInputModuleInstance other) {
			return this.InstanceId.Equals(other.InstanceId);
		}
	}
}
