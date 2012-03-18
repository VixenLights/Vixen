using System.Collections.Generic;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Rule.Patch {
	public class ToNOutputsAtOutput : IPatchingRule {
		public ToNOutputsAtOutput() {
		}

		public ToNOutputsAtOutput(ControllerReference startingPoint, int outputCountToPatch) {
			StartingPoint = startingPoint;
			OutputCountToPatch = outputCountToPatch;
		}

		public ControllerReference StartingPoint { get; set; }
		public int OutputCountToPatch { get; set; }

		public string Description {
			get { return "To multiple outputs on a single controller"; }
		}

		public ControllerReference[] GenerateControllerReferences(int consecutiveApplicationCount) {
			List<ControllerReference> controllerReferences = new List<ControllerReference>();

			// If the controller doesn't exist, abandon.
			OutputController controller = (OutputController)VixenSystem.Controllers.Get(StartingPoint.ControllerId);
			if(controller == null) return new ControllerReference[0];

			int outputIndex = StartingPoint.OutputIndex;
			while(consecutiveApplicationCount-- > 0 && PatchingHelper.IsValidOutputIndex(outputIndex, controller)) {
				IEnumerable<ControllerReference> references = PatchingHelper.PatchControllerAt(controller, outputIndex, OutputCountToPatch);
				controllerReferences.AddRange(references);
				outputIndex += OutputCountToPatch;
			}

			return controllerReferences.ToArray();
		}
	}
}
