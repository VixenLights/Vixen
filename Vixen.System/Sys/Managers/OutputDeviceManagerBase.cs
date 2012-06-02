using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Vixen.Sys.Instrumentation;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers {
	abstract public class OutputDeviceManagerBase : IOutputDeviceManager {
		private Dictionary<Guid, IOutputDevice> _instances;
		private enum ExecutionState { Stopped, Starting, Started, Paused, Stopping };
		private ExecutionState _stateAll = ExecutionState.Stopped;
		private Dictionary<Guid, HardwareUpdateThread> _updateThreads;

		protected OutputDeviceManagerBase() {
			_instances = new Dictionary<Guid, IOutputDevice>();
			_updateThreads = new Dictionary<Guid, HardwareUpdateThread>();
		}

		public void Start(IOutputDevice outputDevice) {
			_Start(outputDevice);
		}

		public void Stop(IOutputDevice outputDevice) {
			_Stop(outputDevice);
		}

		public void Pause(IOutputDevice outputDevice) {
			_Pause(outputDevice);
		}

		public void Resume(IOutputDevice outputDevice) {
			_Resume(outputDevice);
		}

		public void Pause(IEnumerable<IOutputDevice> outputDevices) {
			_PauseAll(outputDevices);
		}

		public void Resume(IEnumerable<IOutputDevice> outputDevices) {
			_ResumeAll(outputDevices);
		}

		public void StartAll() {
			_StartAll(_AllDevices());
		}

		public void StartAll(IEnumerable<IOutputDevice> outputDevices) {
			_StartAll(outputDevices);
		}

		public void StopAll() {
			_StopAll(_AllDevices());
		}

		public void StopAll(IEnumerable<IOutputDevice> outputDevices) {
			_StopAll(outputDevices);
		}

		public void PauseAll() {
			_PauseAll(_AllDevices());
		}

		public void PauseAll(IEnumerable<IOutputDevice> outputDevices) {
			_PauseAll(outputDevices);
		}

		public void ResumeAll() {
			_ResumeAll(_AllDevices());
		}

		public void ResumeAll(IEnumerable<IOutputDevice> outputDevices) {
			_ResumeAll(outputDevices);
		}

		public void Add(IOutputDevice outputDevice) {
			_Add(outputDevice);
		}

		public void AddRange(IEnumerable<IOutputDevice> outputDevices) {
			foreach(IOutputDevice outputDevice in outputDevices) {
				_Add(outputDevice);
			}
		}

		public void Remove(IOutputDevice outputDevice) {
			_Remove(outputDevice);
		}

		public IOutputDevice Get(Guid id) {
			IOutputDevice outputDevice;
			_instances.TryGetValue(id, out outputDevice);
			return outputDevice;
		}

		public IEnumerable<IOutputDevice> GetAll() {
			return _instances.Values.ToArray();
		}

		public IEnumerator<IOutputDevice> GetEnumerator() {
			// Enumerate against a copy of the collection.
			return _instances.Values.ToList().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		virtual protected void _Add(IOutputDevice outputDevice) {
			lock(_instances) {
				_AddingDevice(outputDevice);

				_instances[outputDevice.Id] = outputDevice;

				// Make sure the device is running/not running like all the others.
				switch(_stateAll) {
					case ExecutionState.Started:
						_Start(outputDevice);
						break;
					case ExecutionState.Stopped:
						_Stop(outputDevice);
						break;
					case ExecutionState.Paused:
						_Start(outputDevice);
						_Pause(outputDevice);
						break;
				}
				//if(_stateAll == ExecutionState.Started) {
				//    _Start(outputDevice);
				//} else {
				//    _Stop(outputDevice);
				//}

				_AddedDevice(outputDevice);
			}
		}

		virtual protected void _Remove(IOutputDevice outputDevice) {
			lock(_instances) {
				_RemovingDevice(outputDevice);

				if(_instances.Remove(outputDevice.Id)) {
					// Stop the controller.
					_Stop(outputDevice);
				}

				_RemovedDevice(outputDevice);
			}
		}

		virtual protected void _StartAll(IEnumerable<IOutputDevice> outputDevices) {
			// For now, doing them serially in the UI thread.  When there are multiple 
			// modules, there seems to be a problem with what's invoked and when
			// (including one controller being invoked from multiple threads while
			//  other controllers not being invoked...sounded like a closure issue
			//  initially, but I can't figure it out).
			if(_stateAll == ExecutionState.Stopped) {
				_stateAll = ExecutionState.Starting;
				_DeviceAction(outputDevices, _Start);
				_stateAll = ExecutionState.Started;
			}
		}

		virtual protected void _StopAll(IEnumerable<IOutputDevice> outputDevices) {
			if(_stateAll == ExecutionState.Started) {
				_stateAll = ExecutionState.Stopping;
				_DeviceAction(outputDevices, _Stop);
				_stateAll = ExecutionState.Stopped;
			}
		}

		virtual protected void _PauseAll(IEnumerable<IOutputDevice> outputDevices) {
			if(_stateAll == ExecutionState.Started) {
				_DeviceAction(outputDevices, _Pause);
				_stateAll = ExecutionState.Paused;
			}
		}

		virtual protected void _ResumeAll(IEnumerable<IOutputDevice> outputDevices) {
			if(_stateAll == ExecutionState.Paused) {
				_DeviceAction(outputDevices, _Resume);
				_stateAll = ExecutionState.Started;
			}
		}

		virtual protected void _Start(IOutputDevice outputDevice) {
			if(_CanStart(outputDevice)) {
				try {
					_StartingDevice(outputDevice);

					outputDevice.Start();

					if(outputDevice.IsRunning) {
						_StartNewUpdateThread(outputDevice);
						_StartedDevice(outputDevice);
					} else {
						VixenSystem.Logging.Error("Tried to start device " + outputDevice.Name + ", but it did not start.");
					}
				} catch(Exception ex) {
					VixenSystem.Logging.Error("Error starting device " + outputDevice.Name, ex);
				}
			}
		}

		virtual protected void _Stop(IOutputDevice outputDevice) {
			if(_CanStop(outputDevice)) {
				try {
					_StoppingDevice(outputDevice);

					_StopUpdateThread(outputDevice);

					outputDevice.Stop();

					_StoppedDevice(outputDevice);
				} catch(Exception ex) {
					VixenSystem.Logging.Error("Error trying to stop device " + outputDevice.Name, ex);
				}
			}
		}
		
		virtual protected void _Pause(IOutputDevice outputDevice) {
			if(_CanPause(outputDevice)) {
				try {
					outputDevice.Pause();
				} catch(Exception ex) {
					VixenSystem.Logging.Error("Error trying to pause device " + outputDevice.Name, ex);
				}
			}
		}
		
		virtual protected void _Resume(IOutputDevice outputDevice) {
			if(_CanResume(outputDevice)) {
				try {
					outputDevice.Resume();
				} catch(Exception ex) {
					VixenSystem.Logging.Error("Error trying to resume device " + outputDevice.Name, ex);
				}
			}
		}

		virtual protected void _AddingDevice(IOutputDevice outputDevice) { }
		virtual protected void _AddedDevice(IOutputDevice outputDevice) { }
		virtual protected void _RemovingDevice(IOutputDevice outputDevice) { }
		virtual protected void _RemovedDevice(IOutputDevice outputDevice) { }
		virtual protected void _StartingDevice(IOutputDevice outputDevice) { }
		virtual protected void _StartedDevice(IOutputDevice outputDevice) { }
		virtual protected void _StoppingDevice(IOutputDevice outputDevice) { }
		virtual protected void _StoppedDevice(IOutputDevice outputDevice) { }

		private bool _CanStart(IOutputDevice outputDevice) {
			return !outputDevice.IsRunning && _InRunningState;
		}

		private bool _CanStop(IOutputDevice outputDevice) {
			return outputDevice.IsRunning;
		}

		private bool _CanPause(IOutputDevice outputDevice) {
			return outputDevice.IsRunning;
		}

		private bool _CanResume(IOutputDevice outputDevice) {
			return outputDevice.IsRunning && outputDevice.IsPaused && _InRunningState;
		}

		private bool _InRunningState {
			get { return _stateAll == ExecutionState.Starting || _stateAll == ExecutionState.Started || _stateAll == ExecutionState.Paused; }
		}

		private void _StartNewUpdateThread(IOutputDevice outputDevice) {
			HardwareUpdateThread thread = new HardwareUpdateThread(outputDevice);
			thread.Error += _HardwareError;
			lock(_updateThreads) {
				_updateThreads[outputDevice.Id] = thread;
			}
			thread.Start();
		}

		private void _StopUpdateThread(IOutputDevice outputDevice) {
			HardwareUpdateThread thread;
			if(_updateThreads.TryGetValue(outputDevice.Id, out thread)) {
				lock(_updateThreads) {
					_updateThreads.Remove(outputDevice.Id);
				}
				thread.Stop();
				thread.WaitForFinish();
				thread.Error -= _HardwareError;
			}
		}

		private void _DeviceAction(IEnumerable<IOutputDevice> outputDevices, Action<IOutputDevice> action) {
			foreach(IOutputDevice outputDevice in outputDevices) {
				action(outputDevice);
			}
		}

		private IEnumerable<IOutputDevice> _AllDevices() {
			return _instances.Values;
		}

		private void _HardwareError(object sender, EventArgs e) {
			HardwareUpdateThread hardwareUpdateThread = (HardwareUpdateThread)sender;
			_Stop(hardwareUpdateThread.OutputDevice);
			VixenSystem.Logging.Error("Device " + hardwareUpdateThread.OutputDevice.Name + " experienced an error during execution and was shutdown.");
		}

		#region class HardwareUpdateThread
		class HardwareUpdateThread {
			private Thread _thread;
			private ExecutionState _threadState = ExecutionState.Stopped;
			private EventWaitHandle _finished;
			private OutputDeviceSleepTimeActualValue _outputDeviceSleepTimeActualValue;
			private OutputDeviceSleepTimeRequestedValue _outputDeviceSleepTimeRequestedValue;

			private const int STOP_TIMEOUT = 4000;   // Four seconds should be plenty of time for a thread to stop.

			public event EventHandler Error;

			public HardwareUpdateThread(IOutputDevice outputDevice) {
				OutputDevice = outputDevice;
				_thread = new Thread(_ThreadFunc) { Name = outputDevice.Name + " update", IsBackground = true };
				_finished = new EventWaitHandle(false, EventResetMode.ManualReset);
			}

			public IOutputDevice OutputDevice { get; private set; }

			public void Start() {
				if(_threadState == ExecutionState.Stopped) {
					_threadState = ExecutionState.Started;
					_finished.Reset();
					_thread.Start();
				}
			}

			public void Stop() {
				if(_threadState == ExecutionState.Started) {
					_threadState = ExecutionState.Stopping;
				}
			}

			public void WaitForFinish() {
				if(!_finished.WaitOne(STOP_TIMEOUT)) {
					// Timed out waiting for a stop.
					//(This will prevent hangs in stopping, due to controller code failing to stop).
					throw new TimeoutException("Controller " + OutputDevice.Name + " failed to stop in the required amount of time.");
				}
			}

			private void _ThreadFunc() {
				long frameStart, frameEnd, timeLeft;
				Stopwatch currentTime = Stopwatch.StartNew();

				_outputDeviceSleepTimeRequestedValue = new OutputDeviceSleepTimeRequestedValue(OutputDevice);
				VixenSystem.Instrumentation.AddValue(_outputDeviceSleepTimeRequestedValue);
				_outputDeviceSleepTimeActualValue = new OutputDeviceSleepTimeActualValue(OutputDevice);
				VixenSystem.Instrumentation.AddValue(_outputDeviceSleepTimeActualValue);

				// Thread main loop
				try {
					while(_threadState != ExecutionState.Stopping) {
						frameStart = currentTime.ElapsedMilliseconds;
						frameEnd = frameStart + OutputDevice.UpdateInterval;

						OutputDevice.Update();

						timeLeft = frameEnd - currentTime.ElapsedMilliseconds;

						_Sleep((int)timeLeft, currentTime);
					}
					_threadState = ExecutionState.Stopped;
					_finished.Set();
				} catch(Exception ex) {
					// Do this before calling OnError so that we can be in the stopped state.
					// Otherwise, we'll have a deadlock as the event causes a shutdown which
					// waits for the thread to end.
					_threadState = ExecutionState.Stopped;
					_finished.Set();

					VixenSystem.Logging.Error("Controller " + OutputDevice.Name + " error", ex);
					VixenSystem.Logging.Debug("Controller error:" + Environment.NewLine + ex.StackTrace);
					OnError();
				}
			}

			private void _Sleep(int timeLeft, Stopwatch threadTime) {
				_outputDeviceSleepTimeRequestedValue.Set(timeLeft);

				long timeBeforeSleep = threadTime.ElapsedMilliseconds;

				if(timeLeft > 1) {
					Thread.Sleep(timeLeft);
				}

				_outputDeviceSleepTimeActualValue.Set(threadTime.ElapsedMilliseconds - timeBeforeSleep);
			}

			protected virtual void OnError() {
				if(Error != null) {
					Error.Raise(this, EventArgs.Empty);
				}
			}
		}
		#endregion
	}
}
