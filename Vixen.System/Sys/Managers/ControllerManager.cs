using System;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers {
	public class ControllerManager : OutputDeviceManagerBase {
		// Controller id : Controller
		//private Dictionary<Guid, OutputController> _controllers = new Dictionary<Guid, OutputController>();
		//private ControllerLinker _controllerLinks = new ControllerLinker();

		//private enum ExecutionState { Stopped, Starting, Started, Stopping };
		//private ExecutionState _stateAll = ExecutionState.Stopped;
		//private Dictionary<Guid, HardwareUpdateThread> _updateThreads = new Dictionary<Guid, HardwareUpdateThread>();

		//public void AddController(OutputController controller) {
		//    lock(_controllers) {
		//        _controllers[controller.Id] = controller;
		//        _controllerLinks.AddController(controller.Id);

		//        // Make sure the controller is running/not running like all the others.
		//        if(_stateAll == ExecutionState.Started) {
		//            _StartController(controller);
		//        } else {
		//            _StopController(controller);
		//        }
		//    }
		//}

		//public void AddControllers(IEnumerable<OutputController> controllers) {
		//    lock(_controllers) {
		//        foreach(OutputController controller in controllers) {
		//            AddController(controller);
		//        }
		//    }
		//}

		//public void RemoveController(OutputController controller) {
		//    lock(_controllers) {
		//        if(_controllers.Remove(controller.Id)) {
		//            // Stop the controller.
		//            _StopController(controller);
		//            // Remove it from any patching.
		//            foreach(ChannelOutputPatch patch in VixenSystem.ChannelPatching) {
		//                patch.Remove(controller.Id);
		//            }
		//            //foreach(ChannelNode node in VixenSystem.Nodes) {
		//            //    if(node.Channel != null) {
		//            //        foreach(ControllerReference cr in node.Channel.Patch.ToArray()) {
		//            //            if(cr.ControllerId == controller.Id) {
		//            //                node.Channel.Patch.Remove(cr);
		//            //            }
		//            //        }
		//            //    }
		//            //}
		//            _controllerLinks.RemoveController(controller.Id);
		//        }
		//    }
		//}

		//public IOutputDevice Get(Guid id) {
		//    return _GetController(id);
		//}

		//public void PauseControllers() {
		//    foreach(OutputController controller in _GetRootControllers()) {
		//        controller.Pause();
		//    }
		//}

		//public void ResumeControllers() {
		//    foreach(OutputController controller in _GetRootControllers()) {
		//        controller.Resume();
		//    }
		//}

		//public void StartAll(IEnumerable<IOutputDevice> controllers) {
		//    if(_stateAll == ExecutionState.Stopped) {
		//        _stateAll = ExecutionState.Starting;

		//        // Start the hardware.
		//        // Running in parallel to prevent a bad actor from screwing up the other
		//        // controllers' ability to start.
		//        //Parallel.ForEach(_controllers.Values, _StartController);
		//        // Scratch that.
		//        // For now, doing them serially in the UI thread.  When there are multiple 
		//        // modules, there seems to be a problem with what's invoked and when
		//        // (including one controller being invoked from multiple threads while
		//        //  other controllers not being invoked...sounded like a closure issue
		//        //  initially, but I can't figure it out).
		//        foreach(OutputController controller in controllers) {
		//            _StartController(controller);
		//        }

		//        _stateAll = ExecutionState.Started;
		//    }
		//}

		//public void CloseControllers() {
		//    if(_stateAll == ExecutionState.Started) {
		//        _stateAll = ExecutionState.Stopping;

		//        // Stop the hardware.
		//        // Running in parallel to prevent a bad actor from screwing up the other
		//        // controllers' ability to stop.
		//        //Parallel.ForEach(_controllers.Values, _StopController);
		//        // Not doing this in parallel anymore to keep all invocations on the UI
		//        // thread.
		//        foreach(OutputController controller in _controllers.Values) {
		//            _StopController(controller);
		//        }

		//        _stateAll = ExecutionState.Stopped;
		//    }
		//}

		//public void StartController(OutputController controller) {
		//    _StartController(controller);
		//}

		//public void StopController(OutputController controller) {
		//    _StopController(controller);
		//}

		public OutputController GetController(Guid id) {
			return (OutputController)Get(id);
		}

		public void AddSources(IOutputSourceCollection sources) {
			if(sources == null) throw new ArgumentNullException("sources");

			foreach(Guid controllerId in sources.Controllers) {
				foreach(OutputSources outputSources in sources.GetControllerSources(controllerId)) {
					foreach(IOutputStateSource source in outputSources) {
						AddSource(source, new ControllerReference(controllerId, outputSources.OutputIndex));
					}
				}
			}
		}

		public void RemoveSources(IOutputSourceCollection sources) {
			if(sources == null) throw new ArgumentNullException("sources");

			foreach(Guid controllerId in sources.Controllers) {
				foreach(OutputSources outputSources in sources.GetControllerSources(controllerId)) {
					foreach(IOutputStateSource source in outputSources) {
						RemoveSource(source, new ControllerReference(controllerId, outputSources.OutputIndex));
					}
				}
			}
		}

		public void AddSource(IOutputStateSource source, ControllerReference controllerReference) {
			OutputController controller = GetController(controllerReference.ControllerId);
			if(controller != null) {
				controller.AddSource(source, controllerReference.OutputIndex);
			}
		}

		public void RemoveSource(IOutputStateSource source, ControllerReference controllerReference) {
			OutputController controller = GetController(controllerReference.ControllerId);
			if(controller != null) {
				controller.RemoveSource(source, controllerReference.OutputIndex);
			}
		}

		public bool IsValidReference(ControllerReference controllerReference) {
			OutputController controller = GetController(controllerReference.ControllerId);
			if(controller != null) {
				return controllerReference.OutputIndex < controller.OutputCount;
			}
			return false;
		}

		//private IEnumerable<OutputController> _GetRootControllers() {
		//    return GetAll().Cast<OutputController>().Where(IsRootController);
		//}

		//private OutputController _GetController(Guid controllerId) {
		//    OutputController controller;
		//    if(_controllers.TryGetValue(controllerId, out controller)) {
		//        return controller;
		//    }
		//    return null;
		//}

		//private void _StartController(OutputController controller) {
		//    if(!controller.IsRunning && (_stateAll == ExecutionState.Starting || _stateAll == ExecutionState.Started)) {
		//        //// Fixup link to another controller.
		//        //OutputController parentController = _controllers.Values.FirstOrDefault(x => x.Id == controller.LinkedTo);
		//        //if(parentController == null || controller.CanLinkTo(parentController)) {
		//        //    controller.LinkTo(parentController);
		//        //} else {
		//        //    VixenSystem.Logging.Error("Controller " + controller.Name + " is linked to controller " + parentController.Name + ", but it's an invalid link.");
		//        //}

		//        // Start the controller.
		//        try {
		//            controller.Start();

		//            // Create / Start the thread that updates the hardware.
		//            HardwareUpdateThread thread = new HardwareUpdateThread(controller);
		//            thread.Error += _HardwareError;
		//            lock(_updateThreads) {
		//                _updateThreads[controller.Id] = thread;
		//            }
		//            thread.Start();
		//        } catch(Exception ex) {
		//            VixenSystem.Logging.Error("Error starting controller " + controller.Name, ex);
		//        }
		//    }
		//}

		//private void _StopController(OutputController controller) {
		//    if(controller.IsRunning) {
		//        try {
		//            // Stop the thread that updates the hardware.
		//            HardwareUpdateThread thread;
		//            if(_updateThreads.TryGetValue(controller.Id, out thread)) {
		//                thread.Stop();
		//                thread.WaitForFinish();
		//                thread.Error -= _HardwareError;
		//            }

		//            // Stop the controller.
		//            controller.Stop();
		//        } catch(Exception ex) {
		//            VixenSystem.Logging.Error("Error trying to stop controller " + controller.Name, ex);
		//        }
		//    }
		//}

		//private void _HardwareError(object sender, EventArgs e) {
		//    HardwareUpdateThread hardwareUpdateThread = (HardwareUpdateThread)sender;
		//    _StopController(hardwareUpdateThread.Controller);
		//    VixenSystem.Logging.Error("Controller " + hardwareUpdateThread.Controller.Name + " experienced an error during execution and was shutdown.");
		//}


		//#region class HardwareUpdateThread
		//class HardwareUpdateThread {
		//    private Thread _thread;
		//    private ExecutionState _threadState = ExecutionState.Stopped;
		//    private EventWaitHandle _finished;

		//    private const int STOP_TIMEOUT = 4000;   // Four seconds should be plenty of time for a thread to stop.

		//    public event EventHandler Error;

		//    public HardwareUpdateThread(OutputController controller) {
		//        Controller = controller;
		//        _thread = new Thread(_ThreadFunc);
		//        _thread.IsBackground = true;
		//        _finished = new EventWaitHandle(false, EventResetMode.ManualReset);
		//    }

		//    //public ExecutionState State { get { return _threadState; } }

		//    //*** would want any output device in here so that they all get started/stopped/managed
		//    //    together, but there is a lot in here that is controller-specific, like linking
		//    //    and sources
		//    //-> So maybe ControllerManager handles all output devices, but also has subclasses or just
		//    //   methods for controller-specific.
		//    //-> Or maybe there is another manager for output devices that ControllerManager defers
		//    //   to.
		//    public OutputController Controller { get; private set; }

		//    public void Start() {
		//        if(_threadState == ExecutionState.Stopped) {
		//            _threadState = ExecutionState.Started;
		//            _finished.Reset();
		//            _thread.Start();
		//        }
		//    }

		//    public void Stop() {
		//        if(_threadState == ExecutionState.Started) {
		//            _threadState = ExecutionState.Stopping;
		//        }
		//    }

		//    public void WaitForFinish() {
		//        if(!_finished.WaitOne(STOP_TIMEOUT)) {
		//            // Timed out waiting for a stop.
		//            //(This will prevent hangs in stopping, due to controller code failing to stop).
		//            throw new TimeoutException("Controller " + Controller.Name + " failed to stop in the required amount of time.");
		//        }
		//    }

		//    private void _ThreadFunc() {
		//        long frameStart, frameEnd, timeLeft;
		//        Stopwatch currentTime = Stopwatch.StartNew();

		//        // Thread main loop
		//        try {
		//            while(_threadState != ExecutionState.Stopping) {
		//                frameStart = currentTime.ElapsedMilliseconds;
		//                frameEnd = frameStart + Controller.UpdateInterval;

		//                Controller.Update();

		//                timeLeft = frameEnd - currentTime.ElapsedMilliseconds;

		//                if(timeLeft > 1) {
		//                    Thread.Sleep((int)timeLeft);
		//                }
		//            }
		//            _threadState = ExecutionState.Stopped;
		//            _finished.Set();
		//        } catch(Exception ex) {
		//            // Do this before calling OnError so that we can be in the stopped state.
		//            // Otherwise, we'll have a deadlock as the event causes a shutdown which
		//            // waits for the thread to end.
		//            _threadState = ExecutionState.Stopped;
		//            _finished.Set();

		//            VixenSystem.Logging.Error("Controller " + Controller.Name + " error", ex);
		//            VixenSystem.Logging.Debug("Controller error:" + Environment.NewLine + ex.StackTrace);
		//            OnError();
		//        }
		//    }

		//    protected virtual void OnError() {
		//        if(Error != null) {
		//            Error.Raise(this, EventArgs.Empty);
		//        }
		//    }
		//}
		//#endregion

		//public bool IsRootController(OutputController controller) {
		//    return _controllerLinks.IsRootController(controller);
		//}

		//public int GetChainIndex(OutputController controller) {
		//    return _controllerLinks.GetChainIndex(controller.Id);
		//}

		public OutputController GetNext(OutputController controller) {
			Guid? nextControllerId = VixenSystem.ControllerLinking.GetNext(controller.Id);
			//return (nextControllerId.HasValue) ? _controllers[nextControllerId.Value] : null;
			return (nextControllerId.HasValue) ? GetController(nextControllerId.Value) : null;
		}

		public OutputController GetPrior(OutputController controller) {
			Guid? priorControllerId = VixenSystem.ControllerLinking.GetPrior(controller.Id);
			//return (priorControllerId.HasValue) ? _controllers[priorControllerId.Value] : null;
			return (priorControllerId.HasValue) ? GetController(priorControllerId.Value) : null;
		}

		//public void LinkController(Guid childControllerId, Guid? parentControllerId) {
		//    _controllerLinks.LinkController(childControllerId, parentControllerId);
		//}

		//public IEnumerator<OutputController> GetEnumerator() {
		//    // Enumerate against a copy of the collection.
		//    return _controllers.Values.ToList().GetEnumerator();
		//}

		//System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
		//    return GetEnumerator();
		//}

		protected override void _AddingDevice(IOutputDevice outputDevice) {
			//_controllerLinks.AddController(outputDevice.Id);
		}

		protected override void _RemovedDevice(IOutputDevice outputDevice) {
			// Remove it from any patching.
			// The patching is added by ChannelOutputPatchManager when the system starts.
			foreach(ChannelOutputPatch patch in VixenSystem.ChannelPatching) {
				patch.Remove(outputDevice.Id);
			}

			VixenSystem.ControllerLinking.RemoveController(outputDevice.Id);
		}
	}
}
