using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Vixen.Sys;

namespace Vixen.Execution {
	public class ControllerManager : IEnumerable<OutputController> {
		// Controller id : Controller
		private Dictionary<Guid, OutputController> _controllers = new Dictionary<Guid, OutputController>();

		private enum ExecutionState { Stopped, Starting, Started, Stopping };
		private ExecutionState _stateAll = ExecutionState.Stopped;
		private Dictionary<Guid, HardwareUpdateThread> _updateThreads = new Dictionary<Guid, HardwareUpdateThread>();
		//// Controller id : List of (source, reference)
		//private Dictionary<Guid, List<Tuple<IOutputStateSource, ControllerReference>>> _unresolvedReferences = new Dictionary<Guid, List<Tuple<IOutputStateSource, ControllerReference>>>();

		public ControllerManager() {
		}

		public ControllerManager(IEnumerable<OutputController> controllers)
			: this() {
			AddControllers(controllers);
		}

		public void AddController(OutputController controller) {
			lock(_controllers) {
				_controllers[controller.Id] = controller;
				_StartController(controller);
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
					foreach(ChannelNode node in VixenSystem.Nodes) {
						if(node.Channel != null) {
							foreach(ControllerReference cr in node.Channel.Patch.ToArray()) {
								if(cr.ControllerId == controller.Id) {
									node.Channel.Patch.Remove(cr);
								}
							}
						}
					}
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
		
		public void OpenControllers() {
			if(_stateAll == ExecutionState.Stopped) {
				_stateAll = ExecutionState.Starting;

				// Start the hardware.
				// Running in parallel to prevent a bad actor from screwing up the other
				// controllers' ability to start.
				Parallel.ForEach(_controllers.Values, _StartController);

				_stateAll = ExecutionState.Started;
			}
		}

		public void CloseControllers() {
			if(_stateAll == ExecutionState.Started) {
				_stateAll = ExecutionState.Stopping;

				// Stop the hardware.
				// Running in parallel to prevent a bad actor from screwing up the other
				// controllers' ability to stop.
				Parallel.ForEach(_controllers.Values, _StopController);

				_stateAll = ExecutionState.Stopped;
			}
		}

		public void AddSource(IOutputStateSource source, ControllerReference controllerReference) {
			OutputController controller;
			if(_controllers.TryGetValue(controllerReference.ControllerId, out controller)) {
				controller.AddSource(source, controllerReference.OutputIndex);
			//} else {
			//    List<Tuple<IOutputStateSource, ControllerReference>> references;
			//    if(!_unresolvedReferences.TryGetValue(controllerReference.ControllerId, out references)) {
			//        _unresolvedReferences[controllerReference.ControllerId] = references = new List<Tuple<IOutputStateSource, ControllerReference>>();
			//    }
			//    references.Add(new Tuple<IOutputStateSource, ControllerReference>(source, controllerReference));
			}
		}

		public void RemoveSource(IOutputStateSource source, ControllerReference controllerReference) {
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

		private void _AddController(OutputController controller) {
			// Reference the controller.
			_controllers[controller.Id] = controller;

			//// Fix up any unresolved references.
			//List<Tuple<IOutputStateSource, ControllerReference>> references;
			//if(_unresolvedReferences.TryGetValue(controller.Id, out references)) {
			//    foreach(Tuple<IOutputStateSource, ControllerReference> reference in references) {
			//        AddSource(reference.Item1, reference.Item2);
			//    }
			//    _unresolvedReferences.Remove(controller.Id);
			//}

			// Make sure the controller is running/not running like all the others.
			if(_stateAll == ExecutionState.Started) {
				_StartController(controller);
			} else {
				_StopController(controller);
			}
		}

		private OutputController _GetController(Guid controllerId) {
			OutputController controller = null;
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

				controller.Start();

				// Create / Start the thread that updates the hardware.
				HardwareUpdateThread thread = new HardwareUpdateThread(controller);
				lock (_updateThreads) {
					_updateThreads[controller.Id] = thread;
				}
				thread.Start();
			}
		}

		private void _StopController(OutputController controller) {
			if(controller.IsRunning) {
				// Stop the thread that updates the hardware.
				HardwareUpdateThread thread;
				if(_updateThreads.TryGetValue(controller.Id, out thread)) {
					thread.Stop();
					thread.WaitForFinish();
				}

				controller.Stop();
			}
		}


		#region class HardwareUpdateThread
		class HardwareUpdateThread {
			private Thread _thread;
			private OutputController _controller;
			private ExecutionState _threadState = ExecutionState.Stopped;
			private EventWaitHandle _finished;

			private const int STOP_TIMEOUT = 4000;   // Four seconds should be plenty of time for a thread to stop.

			public HardwareUpdateThread(OutputController controller) {
				_controller = controller;
				_thread = new Thread(_ThreadFunc);
				_finished = new EventWaitHandle(false, EventResetMode.ManualReset);
			}

			public ExecutionState State { get { return _threadState; } }

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
					throw new TimeoutException("Controller " + _controller.Name + " failed to stop in the required amount of time.");
				}
			}

			private void _ThreadFunc() {
				long frameStart, frameEnd, timeLeft;
				Stopwatch currentTime = Stopwatch.StartNew();

				// Thread main loop
				while(_threadState != ExecutionState.Stopping) {
					frameStart = currentTime.ElapsedMilliseconds;
					frameEnd = frameStart + _controller.UpdateInterval;

					_controller.Update();

					timeLeft = frameEnd - currentTime.ElapsedMilliseconds;

					if(timeLeft > 1) {
						Thread.Sleep((int)timeLeft);
					}
				}

				_threadState = ExecutionState.Stopped;
				_finished.Set();
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
