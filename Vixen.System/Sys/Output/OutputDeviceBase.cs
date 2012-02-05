using System;
using System.Diagnostics;
using Vixen.Sys.Instrumentation;

namespace Vixen.Sys.Output {
	abstract public class OutputDeviceBase : IOutputDevice {
		private bool _isRunning;
		private OutputDeviceRefreshRateValue _refreshRateValue;
		private OutputDeviceUpdateTimeValue _updateTimeValue;
		private Stopwatch _stopwatch;

		protected OutputDeviceBase() {
			_stopwatch = new Stopwatch();
		}

		public void Start() {
			if(!IsRunning) {
				_isRunning = true;
				_Start();
				_refreshRateValue = new OutputDeviceRefreshRateValue(this);
				VixenSystem.Instrumentation.AddValue(_refreshRateValue);
				_updateTimeValue = new OutputDeviceUpdateTimeValue(this);
				VixenSystem.Instrumentation.AddValue(_updateTimeValue);
				_stopwatch.Reset();
				_stopwatch.Start();
			}
		}
		abstract protected void _Start();

		//public void Pause() {
		//    if(IsRunning) {
		//        _Pause();
		//    }
		//}
		//abstract protected void _Pause();

		//public void Resume() {
		//    if(IsRunning) {
		//        _Resume();
		//    }
		//}
		//abstract protected void _Resume();

		public void Stop() {
			if(IsRunning) {
				_Stop();
				_isRunning = false;
				VixenSystem.Instrumentation.RemoveValue(_refreshRateValue);
				VixenSystem.Instrumentation.RemoveValue(_updateTimeValue);
				_stopwatch.Stop();
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

		abstract public int UpdateInterval { get; }

		virtual public bool IsRunning {
			get { return _isRunning; }
		}

		public void Update() {
			_stopwatch.Restart();

			_refreshRateValue.Increment();
			// First, get what we pull from to update...
			Execution.UpdateState();
			// Then we update ourselves from that.
			_UpdateState();

			_updateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
		}
		abstract protected void _UpdateState();
	}
}
