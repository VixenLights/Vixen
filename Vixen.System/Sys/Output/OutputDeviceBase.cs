using System;
using System.Diagnostics;
using Vixen.Sys.Instrumentation;

namespace Vixen.Sys.Output {
	abstract public class OutputDeviceBase : IOutputDevice {
		private OutputDeviceRefreshRateValue _refreshRateValue;
		private OutputDeviceUpdateTimeValue _updateTimeValue;
		private Stopwatch _stopwatch;

		protected OutputDeviceBase(Guid id, string name) {
			Id = id;
			Name = name;
			_stopwatch = new Stopwatch();
		}

		public void Start() {
			if(!IsRunning) {
				IsRunning = true;
				IsPaused = false;
				_Start();
				_CreateInstrumentation();
			}
		}
		abstract protected void _Start();

		public void Pause() {
			if(IsRunning && !IsPaused) {
				IsPaused = true;
				_Pause();
			}
		}
		abstract protected void _Pause();

		public void Resume() {
			if(IsRunning && IsPaused) {
				IsPaused = false;
				_Resume();
			}
		}
		abstract protected void _Resume();

		public void Stop() {
			if(IsRunning) {
				_Stop();
				IsRunning = false;
				IsPaused = false;
				_RemoveInstrumentation();
			}
		}
		abstract protected void _Stop();

		virtual public bool HasSetup {
			get { return false; }
		}

		virtual public bool Setup() {
			return false;
		}

		public Guid Id { get; protected set; }

		virtual public string Name { get; set; }

		public int UpdateInterval { get; set; }

		public virtual bool IsRunning { get; private set; }

		public virtual bool IsPaused { get; private set; }

		public void Update() {
			if(IsRunning && !IsPaused) {
				_refreshRateValue.Increment();

				// First, get what we pull from to update...
				Execution.UpdateState();

				// Then we update ourselves from that.
				_stopwatch.Restart();
				_UpdateState();
				_updateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
			} else {
				_refreshRateValue.Clear();
			}
		}

		//public IDataPolicy DataPolicy { get; set; }

		abstract protected void _UpdateState();

		private void _CreateInstrumentation() {
			_refreshRateValue = new OutputDeviceRefreshRateValue(this);
			VixenSystem.Instrumentation.AddValue(_refreshRateValue);
			_updateTimeValue = new OutputDeviceUpdateTimeValue(this);
			VixenSystem.Instrumentation.AddValue(_updateTimeValue);
			_stopwatch.Reset();
			_stopwatch.Start();
		}

		private void _RemoveInstrumentation() {
			if(_refreshRateValue != null) {
				VixenSystem.Instrumentation.RemoveValue(_refreshRateValue);
			}
			if(_updateTimeValue != null) {
				VixenSystem.Instrumentation.RemoveValue(_updateTimeValue);
			}
			if(_stopwatch != null) {
				_stopwatch.Stop();
			}
		}
	}
}
