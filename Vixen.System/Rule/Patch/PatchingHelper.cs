using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Rule.Patch {
	class PatchingHelper {
		static public IEnumerable<ControllerReference> GenerateControllerReferences(OutputController controller, int outputIndex, int countToPatch) {
			List<ControllerReference> controllerReferences = new List<ControllerReference>();

			int outputsToPatch = Math.Min(countToPatch, controller.OutputCount - outputIndex);

			while(outputsToPatch-- > 0 && IsValidOutputIndex(outputIndex, controller)) {
				controllerReferences.Add(new ControllerReference(controller.Id, outputIndex++));
			}

			return controllerReferences.ToArray();
		}

		static public bool IsValidOutputIndex(int outputIndex, OutputController controller) {
			return outputIndex < controller.OutputCount;
		}
	}
}
