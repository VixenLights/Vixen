using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Rule.Patch {
	public class ToNOutputsAtOutputs : IPatchingRule {
		public ToNOutputsAtOutputs() {
		}

		public ToNOutputsAtOutputs(IEnumerable<ControllerReference> controllerReferences, int outputCountToPatch) {
			ControllerReferences = controllerReferences.ToArray();
			OutputCountToPatch = outputCountToPatch;
		}

		public ControllerReference[] ControllerReferences { get; set; }
		public int OutputCountToPatch { get; set; }

		public string Description {
			get { return "To a single series of outputs mirrored across multiple controllers"; }
		}

		public IEnumerable<ControllerReferenceCollection> GenerateControllerReferenceCollections(int channelCount) {
			List<ControllerReferenceCollection> controllerReferences = new List<ControllerReferenceCollection>();

			// Get the mappings for only the valid controllers.
			OutputController[] controllers = VixenSystem.Controllers.GetValidControllers(ControllerReferences.Select(x => x.ControllerId));
			IEnumerable<Guid> validControllerIds = controllers.Select(x => x.Id);
			ControllerReference[] validControllerReferences = ControllerReferences.Where(x => validControllerIds.Contains(x.ControllerId)).ToArray();

			foreach(ControllerReference controllerReference in validControllerReferences) {
				controllerReferences.Add(new ControllerReferenceCollection(PatchingHelper.GenerateControllerReferences(controllers.FirstOrDefault(x => x.Id == controllerReference.ControllerId), controllerReference.OutputIndex, OutputCountToPatch)));
			}

			return controllerReferences.ToArray();
		}
	}
}
