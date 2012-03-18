using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Rule.Patch {
	public class ToNOutputsOverControllers : IPatchingRule {
		public ToNOutputsOverControllers() {
		}

		public ToNOutputsOverControllers(ControllerReference startingPoint, int outputCountToPatch, IEnumerable<Guid> subsequentControllerIds) {
			StartingPoint = startingPoint;
			OutputCountToPatch = outputCountToPatch;
			SubsequentControllers = subsequentControllerIds.ToArray();
		}

		public ControllerReference StartingPoint { get; set; }
		public int OutputCountToPatch { get; set; }
		public Guid[] SubsequentControllers { get; set; }

		public string Description {
			get { return "To multiple outputs over a series of controllers"; }
		}

		public ControllerReference[] GenerateControllerReferences(int consecutiveApplicationCount) {
			// If the starting reference is bad, abandon?
			OutputController startingController = (OutputController)VixenSystem.Controllers.Get(StartingPoint.ControllerId);
			if(startingController == null || startingController.OutputCount <= StartingPoint.OutputIndex) return new ControllerReference[0];

			// Build the list of controllers we're going to go over.
			List<OutputController> subsequentControllers = new List<OutputController>(VixenSystem.Controllers.GetValidControllers(SubsequentControllers));

			// Create a list of references from the controllers involved.
			List<ControllerReference> allOutputs = new List<ControllerReference>();
			allOutputs.AddRange(Enumerable.Range(StartingPoint.OutputIndex, startingController.OutputCount - StartingPoint.OutputIndex - 1).Select(x => new ControllerReference(startingController.Id, x)));
			foreach(OutputController controller in subsequentControllers) {
				allOutputs.AddRange(Enumerable.Range(0, controller.OutputCount).Select(x => new ControllerReference(startingController.Id, x)));
			}

			// Take what we need from the list of references.
			int referenceCount = Math.Min(consecutiveApplicationCount * OutputCountToPatch, allOutputs.Count);
			return allOutputs.Take(referenceCount).ToArray();
		}
	}
}
