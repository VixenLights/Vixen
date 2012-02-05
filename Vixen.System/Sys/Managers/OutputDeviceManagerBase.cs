using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers {
	abstract public class OutputDeviceManagerBase : IOutputDeviceManager {
		private Dictionary<Guid, IOutputDevice> _instances;
		private enum ExecutionState { Stopped, Starting, Started, Stopping };
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
				if(_stateAll == ExecutionState.Started) {
					_Start(outputDevice);
				} else {
					_Stop(outputDevice);
				}

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

		virtual protected void _Start(IOutputDevice outputDevice) {
			if(!outputDevice.IsRunning && (_stateAll == ExecutionState.Starting || _stateAll == ExecutionState.Started)) {
				try {
					_StartingDevice(outputDevice);

					outputDevice.Start();

					// Create / Start the thread that updates the hardware.
					HardwareUpdateThread thread = new HardwareUpdateThread(outputDevice);
					thread.Error += _HardwareError;
					lock(_updateThreads) {
						_updateThreads[outputDevice.Id] = thread;
					}
					thread.Start();

					_StartedDevice(outputDevice);
				} catch(Exception ex) {
					VixenSystem.Logging.Error("Error starting controller " + outputDevice.Name, ex);
				}
			}
		}

		virtual protected void _Stop(IOutputDevice outputDevice) {
			if(outputDevice.IsRunning) {
				try {
					_StoppingDevice(outputDevice);

					// Stop the thread that updates the hardware.
					HardwareUpdateThread thread;
					if(_updateThreads.TryGetValue(outputDevice.Id, out thread)) {
						thread.Stop();
						thread.WaitForFinish();
						thread.Error -= _HardwareError;
					}

					// Stop the controller.
					outputDevice.Stop();

					_StoppedDevice(outputDevice);
				} catch(Exception ex) {
					VixenSystem.Logging.Error("Error trying to stop device " + outputDevice.Name, ex);
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

			private const int STOP_TIMEOUT = 4000;   // Four seconds should be plenty of time for a thread to stop.

			public event EventHandler Error;

			public HardwareUpdateThread(IOutputDevice outputDevice) {
				OutputDevice = outputDevice;
				_thread = new Thread(_ThreadFunc);
				_thread.IsBackground = true;
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

				// Thread main loop
				try {
					while(_threadState != ExecutionState.Stopping) {
						frameStart = currentTime.ElapsedMilliseconds;
						frameEnd = frameStart + OutputDevice.UpdateInterval;

						OutputDevice.Update();

						timeLeft = frameEnd - currentTime.ElapsedMilliseconds;

						if(timeLeft > 1) {
							Thread.Sleep((int)timeLeft);
						}
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

			protected virtual void OnError() {
				if(Error != null) {
					Error.Raise(this, EventArgs.Empty);
				}
			}
		}
		#endregion
	}
}
