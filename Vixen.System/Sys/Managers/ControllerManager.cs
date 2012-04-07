using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers {
	public class ControllerManager : OutputDeviceManagerBase {
		private HashSet<IOutputSourceCollection> _controllerSources;

		public ControllerManager() {
			_controllerSources = new HashSet<IOutputSourceCollection>();
		}

		public OutputController GetController(Guid id) {
			return (OutputController)Get(id);
		}

		public void AddSources(IOutputSourceCollection sources) {
			if(sources == null) throw new ArgumentNullException("sources");

			if(_controllerSources.Add(sources)) {
				foreach(Guid controllerId in sources.Controllers) {
					OutputController controller = GetController(controllerId);
					controller.AddSources(sources);
					//foreach(OutputSources outputSources in sources.GetControllerSources(controllerId)) {
					//    foreach(IOutputStateSource source in outputSources) {
					//        AddSource(source, new ControllerReference(controllerId, outputSources.OutputIndex));
					//    }
					//}
				}
			}
		}

		public void RemoveSources(IOutputSourceCollection sources) {
			if(sources == null) throw new ArgumentNullException("sources");

			_controllerSources.Remove(sources);

			foreach(Guid controllerId in sources.Controllers) {
				OutputController controller = GetController(controllerId);
				controller.RemoveSources(sources);
				//foreach(OutputSources outputSources in sources.GetControllerSources(controllerId)) {
				//    foreach(IOutputStateSource source in outputSources) {
				//        RemoveSource(source, new ControllerReference(controllerId, outputSources.OutputIndex));
				//    }
				//}
			}
		}

		//public void AddSource(IOutputStateSource source, ControllerReference controllerReference) {
		//    OutputController controller = GetController(controllerReference.ControllerId);
		//    if(controller != null) {
		//        controller.AddSource(source, controllerReference.OutputIndex);
		//    }
		//}

		//public void RemoveSource(IOutputStateSource source, ControllerReference controllerReference) {
		//    OutputController controller = GetController(controllerReference.ControllerId);
		//    if(controller != null) {
		//        controller.RemoveSource(source, controllerReference.OutputIndex);
		//    }
		//}

		public bool IsValidReference(ControllerReference controllerReference) {
			OutputController controller = GetController(controllerReference.ControllerId);
			if(controller != null) {
				return controllerReference.OutputIndex < controller.OutputCount;
			}
			return false;
		}

		public OutputController[] GetValidControllers(IEnumerable<Guid> controllerIds) {
			Guid[] validControllerIds = controllerIds.Distinct().ToArray();
			return validControllerIds.Select(VixenSystem.Controllers.Get).NotNull().Cast<OutputController>().ToArray();
		}

		public OutputController GetNext(OutputController controller) {
			Guid? nextControllerId = VixenSystem.ControllerLinking.GetNext(controller.Id);
			return (nextControllerId.HasValue) ? GetController(nextControllerId.Value) : null;
		}

		public OutputController GetPrior(OutputController controller) {
			Guid? priorControllerId = VixenSystem.ControllerLinking.GetPrior(controller.Id);
			return (priorControllerId.HasValue) ? GetController(priorControllerId.Value) : null;
		}

		protected override void _AddingDevice(IOutputDevice outputDevice) {
			// Apply any source collections that have been added to all controllers
			// (added through this manager).
			OutputController controller = (OutputController)outputDevice;
			foreach(IOutputSourceCollection sources in _controllerSources) {
				controller.AddSources(sources);
			}
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
