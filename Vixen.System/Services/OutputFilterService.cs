using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Module.OutputFilter;
using Vixen.Rule;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Services {
	public class OutputFilterService {
		static private OutputFilterService _instance;

		private OutputFilterService() {
		}

		public static OutputFilterService Instance {
			get { return _instance ?? (_instance = new OutputFilterService()); }
		}

		public void ApplyTemplateOnce(string templateFileName, IPatchingRule patchingRule, bool clearExisting = false) {
			OutputFilterTemplate outputFilterTemplate = OutputFilterTemplate.Load(templateFileName);
			if(outputFilterTemplate == null) return;

			ControllerReference[] controllerReferences = patchingRule.GenerateControllerReferenceCollections(1).First().ToArray();
			IEnumerable<OutputController> referencedControllers = VixenSystem.Controllers.GetControllers(controllerReferences).ToArray();

			VixenSystem.Controllers.Pause(referencedControllers);
	
			_ApplyTemplateToOutputs(outputFilterTemplate, controllerReferences, clearExisting);
			
			VixenSystem.Controllers.Resume(referencedControllers);
		}

		public void ApplyTemplateMany(string templateFileName, IPatchingRule patchingRule, int channelCount, bool clearExisting = false) {
			OutputFilterTemplate outputFilterTemplate = OutputFilterTemplate.Load(templateFileName);
			if(outputFilterTemplate == null) return;

			IEnumerable<ControllerReferenceCollection> controllerReferenceCollections = patchingRule.GenerateControllerReferenceCollections(channelCount).ToArray();

			IEnumerable<OutputController> referencedControllers = VixenSystem.Controllers.GetControllers(controllerReferenceCollections.SelectMany(x => x)).ToArray();
			VixenSystem.Controllers.Pause(referencedControllers);

			foreach(ControllerReferenceCollection controllerReferences in controllerReferenceCollections) {
				_ApplyTemplateToOutputs(outputFilterTemplate, controllerReferences.ToArray(), clearExisting);
			}
			
			VixenSystem.Controllers.Resume(referencedControllers);
		}

		private void _ApplyTemplateToOutputs(OutputFilterTemplate template, ControllerReference[] controllerReferences, bool clearExisting = false) {
			int countToAffect = _GetCountToAffect(controllerReferences, template.OutputFilters);

			for(int i = 0; i < countToAffect; i++) {
				// Apply the appropriate list of filters to the appropriate output.
				_ApplyFilterCollectionToOutput(_CloneFilters(template[i]), controllerReferences[i], clearExisting);
			}
		}

		private IEnumerable<IOutputFilterModuleInstance> _CloneFilters(IEnumerable<IOutputFilterModuleInstance> filterModules) {
			return filterModules.Select(x => (IOutputFilterModuleInstance)x.Clone());
		}

		private int _GetCountToAffect(IEnumerable<ControllerReference> controllerReferences, IEnumerable<OutputFilterCollection> filterCollections) {
			return Math.Min(controllerReferences.Count(), filterCollections.Count());
		}

		private void _ApplyFilterCollectionToOutput(IEnumerable<IOutputFilterModuleInstance> filters, ControllerReference controllerReference, bool clearExisting = false) {
			OutputController controller = (OutputController)VixenSystem.Controllers.Get(controllerReference.ControllerId);
			if(controller != null) {
				if(clearExisting) {
					controller.ClearOutputFilters(controllerReference.OutputIndex);
				}
				foreach(IOutputFilterModuleInstance filter in filters) {
					controller.AddOutputFilter(controllerReference.OutputIndex, filter);
				}
			}
		}
	}
}
