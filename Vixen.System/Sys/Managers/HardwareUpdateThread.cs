using System;
using System.Diagnostics;
using System.Threading;
using Vixen.Sys.Instrumentation;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers
{
	internal class HardwareUpdateThread : IDisposable
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();		private Thread _thread;
		private ExecutionState _threadState = ExecutionState.Stopped;
		private EventWaitHandle _finished;
		private AutoResetEvent _updateSignalerSync;
		private ManualResetEvent _pauseSignal;
		private Stopwatch _localTime;

		private OutputDeviceSleepTimeActualValue _sleepTimeActualValue;
		private OutputDeviceRefreshRateValue _refreshRateValue;
		private OutputDeviceUpdateTimeValue _updateTimeValue;

		private const int STOP_TIMEOUT = 4000; // Four seconds should be plenty of time for a thread to stop.

		public event EventHandler Error;

		public HardwareUpdateThread(IOutputDevice outputDevice)
		{
			OutputDevice = outputDevice;
			_thread = new Thread(_ThreadFunc) {Name = outputDevice.Name + " update", IsBackground = true};
			_finished = new EventWaitHandle(false, EventResetMode.ManualReset);
			_updateSignalerSync = new AutoResetEvent(false);
			_pauseSignal = new ManualResetEvent(true);
			_localTime = new Stopwatch();
		}

		public IOutputDevice OutputDevice { get; private set; }

		public void Start()
		{
			if (_threadState == ExecutionState.Stopped) {
				_threadState = ExecutionState.Started;
				OutputDevice.Start();
				_finished.Reset();
				_localTime.Start();
				_CreatePerformanceValues();
				_thread.Start();
			}
		}

		public void Stop()
		{
			if (_threadState == ExecutionState.Started) {
				_threadState = ExecutionState.Stopping;
				_updateSignalerSync.Set();
				Resume();
				_localTime.Stop();
				_RemovePerformanceValues();
				OutputDevice.Stop();
			}
		}

		public void Pause()
		{
			OutputDevice.Pause();
			_pauseSignal.Reset();
		}

		public void Resume()
		{
			OutputDevice.Resume();
			_pauseSignal.Set();
		}

		public void WaitForFinish()
		{
			if (!_finished.WaitOne(STOP_TIMEOUT)) {
				// Timed out waiting for a stop.
				//(This will prevent hangs in stopping, due to controller code failing to stop).
				throw new TimeoutException("Controller " + OutputDevice.Name + " failed to stop in the required amount of time.");
			}
		}

		static long _lastMs = 0;

		private void _ThreadFunc()
		{
			// Thread main loop
			try {
				IOutputDeviceUpdateSignaler signaler = _CreateOutputDeviceUpdateSignaler();

				while (_threadState != ExecutionState.Stopping) {
					long nowMs = _localTime.ElapsedMilliseconds;
					long dtMs = nowMs - _lastMs;
					if( Math.Abs( OutputDevice.UpdateInterval - dtMs) > 10)
						Logging.Info("nowMs:{0}, dtMs:{1}", nowMs, dtMs);
					_lastMs = nowMs;
					Execution.UpdateState();
					_UpdateOutputDevice();

					_WaitOnSignal(signaler);
				//	_WaitOnPause();
				}

				_threadState = ExecutionState.Stopped;
				_finished.Set();
			}
			catch (Exception ex) {
				// Do this before calling OnError so that we can be in the stopped state.
				// Otherwise, we'll have a deadlock as the event causes a shutdown which
				// waits for the thread to end.
				_threadState = ExecutionState.Stopped;
				_finished.Set();

				Logging.ErrorException(string.Format("Controller {0} error", OutputDevice.Name), ex);
				OnError();
			}
		}

		private IOutputDeviceUpdateSignaler _CreateOutputDeviceUpdateSignaler()
		{
			IOutputDeviceUpdateSignaler signaler = OutputDevice.UpdateSignaler ?? new IntervalUpdateSignaler();
			signaler.OutputDevice = OutputDevice;
			signaler.UpdateSignal = _updateSignalerSync;

			return signaler;
		}

		private void _UpdateOutputDevice()
		{
			_refreshRateValue.Increment();

			long timeBeforeUpdate = _localTime.ElapsedMilliseconds;

			OutputDevice.Update();

			_updateTimeValue.Set(_localTime.ElapsedMilliseconds - timeBeforeUpdate);
		}

		private void _WaitOnSignal(IOutputDeviceUpdateSignaler signaler)
		{
			long timeBeforeSignal = _localTime.ElapsedMilliseconds;

			signaler.RaiseSignal();
			//_updateSignalerSync.WaitOne();

			long timeAfterSignal = _localTime.ElapsedMilliseconds;
			_sleepTimeActualValue.Set(timeAfterSignal - timeBeforeSignal);
		}

		private void _WaitOnPause()
		{
			_pauseSignal.WaitOne();
		}

		private void _CreatePerformanceValues()
		{
			_sleepTimeActualValue = new OutputDeviceSleepTimeActualValue(OutputDevice);
			VixenSystem.Instrumentation.AddValue(_sleepTimeActualValue);
			_refreshRateValue = new OutputDeviceRefreshRateValue(OutputDevice);
			VixenSystem.Instrumentation.AddValue(_refreshRateValue);
			_updateTimeValue = new OutputDeviceUpdateTimeValue(OutputDevice);
			VixenSystem.Instrumentation.AddValue(_updateTimeValue);
		}

		private void _RemovePerformanceValues()
		{
			if (_refreshRateValue != null) {
				VixenSystem.Instrumentation.RemoveValue(_refreshRateValue);
			}
			if (_updateTimeValue != null) {
				VixenSystem.Instrumentation.RemoveValue(_updateTimeValue);
			}
			if (_sleepTimeActualValue != null) {
				VixenSystem.Instrumentation.RemoveValue(_sleepTimeActualValue);
			}
		}

		protected virtual void OnError()
		{
			if (Error != null) {
				Error.Raise(this, EventArgs.Empty);
			}
		}

		public void Dispose()
		{
			_Dispose();
		}

		~HardwareUpdateThread()
		{
			_Dispose();
			GC.SuppressFinalize(this);
		}

		private void _Dispose()
		{
			_finished.Dispose();
			_updateSignalerSync.Dispose();
			_pauseSignal.Dispose();
		}
	}
}