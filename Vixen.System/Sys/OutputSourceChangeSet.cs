using System.Collections.Generic;
using System.Linq;
using Vixen.Sys.Output;

namespace Vixen.Sys {
	class OutputSourceChangeSet {
		private HashSet<ControllerReference> _affectedOutputs;

		public OutputSourceChangeSet() {
			_affectedOutputs = new HashSet<ControllerReference>();
		}

		public void AffectOutput(ControllerReference controllerReference, bool outputIsAffected = true) {
			if(outputIsAffected) {
				_Add(controllerReference);
			} else {
				_Remove(controllerReference);
			}
		}

		public void Commit() {
			var changesByController = _affectedOutputs.GroupBy(x => x.ControllerId);
			foreach(var controllerChanges in changesByController) {
				OutputController controller = (OutputController)VixenSystem.Controllers.Get(controllerChanges.Key);
				foreach(ControllerReference controllerReference in controllerChanges) {
					controller.ReloadOutputSources(controllerReference.OutputIndex);
				}
			}
		}

		private void _Add(ControllerReference controllerReference) {
			_affectedOutputs.Add(controllerReference);
		}

		private void _Remove(ControllerReference controllerReference) {
			_affectedOutputs.Remove(controllerReference);
		}
	}
}
