using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using Vixen.Commands;

namespace Vixen.Sys {
	public class ControllerManager : IEnumerable<OutputController> {
		// Controller id : Controller
		private Dictionary<Guid, OutputController> _controllers = new Dictionary<Guid, OutputController>();

		private enum ExecutionState { Stopped, Starting, Started, Stopping };
		private ExecutionState _stateAll = ExecutionState.Stopped;
		private Dictionary<Guid, HardwareUpdateThread> _updateThreads = new Dictionary<Guid, HardwareUpdateThread>();

		public void AddController(OutputController controller) {
			lock(_controllers) {
				_controllers[controller.Id] = controller;

				// Make sure the controller is running/not running like all the others.
				if(_stateAll == ExecutionState.Started) {
					_StartController(controller);
				} else {
					_StopController(controller);
				}
			}
		}

		public void AddControllers(IEnumerable<OutputController> controllers) {
			lock(_controllers) {
				foreach(OutputController controller in controllers) {
					AddController(controller);
				}
			}
		}

		public void RemoveController(OutputController controller) {
			lock(_controllers) {
				if(_controllers.Remove(controller.Id)) {
					// Stop the controller.
					_StopController(controller);
					// Remove it from any patching.
					foreach(ChannelOutputPatch patch in VixenSystem.ChannelPatching) {
						patch.Remove(controller.Id);
					}
					//foreach(ChannelNode node in VixenSystem.Nodes) {
					//    if(node.Channel != null) {
					//        foreach(ControllerReference cr in node.Channel.Patch.ToArray()) {
					//            if(cr.ControllerId == controller.Id) {
					//                node.Channel.Patch.Remove(cr);
					//            }
					//        }
					//    }
					//}
				}
			}
		}

		public OutputController Get(Guid id) {
			return _GetController(id);
		}

		public void PauseControllers() {
			foreach(OutputController controller in _GetRootControllers()) {
				controller.Pause();
			}
		}

		public void ResumeControllers() {
			foreach(OutputController controller in _GetRootControllers()) {
				controller.Resume();
			}
		}
		
		public void OpenControllers(IEnumerable<OutputController> controllers) {
			if(_stateAll == ExecutionState.Stopped) {
				_stateAll = ExecutionState.Starting;

				// Start the hardware.
				// Running in parallel to prevent a bad actor from screwing up the other
				// controllers' ability to start.
				//Parallel.ForEach(_controllers.Values, _StartController);
				// Scratch that.
				// For now, doing them serially in the UI thread.  When there are multiple 
				// modules, there seems to be a problem with what's invoked and when
				// (including one controller being invoked from multiple threads while
				//  other controllers not being invoked...sounded like a closure issue
				//  initially, but I can't figure it out).
				foreach(OutputController controller in controllers) {
					_StartController(controller);
				}

				_stateAll = ExecutionState.Started;
			}
		}

		public void CloseControllers() {
			if(_stateAll == ExecutionState.Started) {
				_stateAll = ExecutionState.Stopping;

				// Stop the hardware.
				// Running in parallel to prevent a bad actor from screwing up the other
				// controllers' ability to stop.
				//Parallel.ForEach(_controllers.Values, _StopController);
				// Not doing this in parallel anymore to keep all invocations on the UI
				// thread.
				foreach(OutputController controller in _controllers.Values) {
					_StopController(controller);
				}

				_stateAll = ExecutionState.Stopped;
			}
		}

		public void StartController(OutputController controller) {
			_StartController(controller);
		}

		public void StopController(OutputController controller) {
			_StopController(controller);
		}

		public void AddSources(IOutputSourceCollection sources) {
			if(sources == null) throw new ArgumentNullException("sources");

			foreach(Guid controllerId in sources.Controllers) {
				foreach(OutputSources outputSources in sources.GetControllerSources(controllerId)) {
					foreach(IStateSource<Command> source in outputSources) {
						AddSource(source, new ControllerReference(controllerId, outputSources.OutputIndex));
					}
				}
			}
		}

		public void RemoveSources(IOutputSourceCollection sources) {
			if(sources == null) throw new ArgumentNullException("sources");

			foreach(Guid controllerId in sources.Controllers) {
				foreach(OutputSources outputSources in sources.GetControllerSources(controllerId)) {
					foreach(IStateSource<Command> source in outputSources) {
						RemoveSource(source, new ControllerReference(controllerId, outputSources.OutputIndex));
					}
				}
			}
		}

		public void AddSource(IStateSource<Command> source, ControllerReference controllerReference) {
			OutputController controller;
			if(_controllers.TryGetValue(controllerReference.ControllerId, out controller)) {
				controller.AddSource(source, controllerReference.OutputIndex);
			}
		}

		public void RemoveSource(IStateSource<Command> source, ControllerReference controllerReference) {
			OutputController controller;
			if(_controllers.TryGetValue(controllerReference.ControllerId, out controller)) {
				controller.RemoveSource(source, controllerReference.OutputIndex);
			}
		}

		public bool IsValidReference(ControllerReference controllerReference) {
			OutputController controller;
			if(_controllers.TryGetValue(controllerReference.ControllerId, out controller)) {
				return controllerReference.OutputIndex < controller.OutputCount;
			}
			return false;
		}

		private IEnumerable<OutputController> _GetRootControllers() {
			return _controllers.Values.Where(x => x.IsRootController);
		}

		private OutputController _GetController(Guid controllerId) {
			OutputController controller;
			if(_controllers.TryGetValue(controllerId, out controller)) {
				return controller;
			}
			return null;
		}

		private void _StartController(OutputController controller) {
			if(!controller.IsRunning && (_stateAll == ExecutionState.Starting || _stateAll == ExecutionState.Started)) {
				// Fixup link to another controller.
				OutputController parentController = _controllers.Values.FirstOrDefault(x => x.Id == controller.LinkedTo);
				if(parentController == null || controller.CanLinkTo(parentController)) {
					controller.LinkTo(parentController);
				} else {
					VixenSystem.Logging.Error("Controller " + controller.Name + " is linked to controller " + parentController.Name + ", but it's an invalid link.");
				}

				// Start the controller.
				try {
					controller.Start();

					// Create / Start the thread that updates the hardware.
					HardwareUpdateThread thread = new HardwareUpdateThread(controller);
					thread.Error += _HardwareError;
					lock(_updateThreads) {
						_updateThreads[controller.Id] = thread;
					}
					thread.Start();
				} catch(Exception ex) {
					VixenSystem.Logging.Error("Error starting controller " + controller.Name, ex);
				}
			}
		}

		private void _StopController(OutputController controller) {
			if(controller.IsRunning) {
				try {
					// Stop the thread that updates the hardware.
					HardwareUpdateThread thread;
					if(_updateThreads.TryGetValue(controller.Id, out thread)) {
						thread.Stop();
						thread.WaitForFinish();
						thread.Error -= _HardwareError;
					}

					// Stop the controller.
					controller.Stop();
				} catch(Exception ex) {
					VixenSystem.Logging.Error("Error trying to stop controller " + controller.Name, ex);
				}
			}
		}

		private void _HardwareError(object sender, EventArgs e) {
			HardwareUpdateThread hardwareUpdateThread = (HardwareUpdateThread)sender;
			_StopController(hardwareUpdateThread.Controller);
			VixenSystem.Logging.Error("Controller " + hardwareUpdateThread.Controller.Name + " experienced an error during execution and was shutdown.");
		}


		#region class HardwareUpdateThread
		class HardwareUpdateThread {
			private Thread _thread;
			private ExecutionState _threadState = ExecutionState.Stopped;
			private EventWaitHandle _finished;

			private const int STOP_TIMEOUT = 4000;   // Four seconds should be plenty of time for a thread to stop.

			public event EventHandler Error;

			public HardwareUpdateThread(OutputController controller) {
				Controller = controller;
				_thread = new Thread(_ThreadFunc);
				_thread.IsBackground = true;
				_finished = new EventWaitHandle(false, EventResetMode.ManualReset);
			}

			//public ExecutionState State { get { return _threadState; } }

			public OutputController Controller { get; private set; }

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
					throw new TimeoutException("Controller " + Controller.Name + " failed to stop in the required amount of time.");
				}
			}

			private void _ThreadFunc() {
				long frameStart, frameEnd, timeLeft;
				Stopwatch currentTime = Stopwatch.StartNew();

				// Thread main loop
				try {
					while(_threadState != ExecutionState.Stopping) {
						frameStart = currentTime.ElapsedMilliseconds;
						frameEnd = frameStart + Controller.UpdateInterval;

						Controller.Update();

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

					VixenSystem.Logging.Error("Controller " + Controller.Name + " error", ex);
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


		public IEnumerator<OutputController> GetEnumerator() {
			// Enumerate against a copy of the collection.
			return _controllers.Values.ToList().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
