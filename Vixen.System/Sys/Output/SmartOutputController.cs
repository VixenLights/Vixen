using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Module.SmartController;

namespace Vixen.Sys.Output {
	public class SmartOutputController : ModuleBasedController<ISmartControllerModuleInstance, IntentOutput> {
		public SmartOutputController(string name, int outputCount, Guid moduleId)
			: this(Guid.NewGuid(), name, outputCount, moduleId) {
		}

		public SmartOutputController(Guid id, string name, int outputCount, Guid moduleId)
			: base(id, name, outputCount, moduleId) {
		}

		protected override ISmartControllerModuleInstance GetControllerModule(Guid moduleId) {
			return Modules.ModuleManagement.GetSmartController(moduleId);
		}

		protected override void _UpdateState() {
			if(Module != null) {
				BeginOutputChange();
				try {
					Outputs.AsParallel().ForAll(x => {
						x.UpdateState();

					//UpdateOutputStates(x => {
						IntentChangeCollection intentChanges = null;
						IIntent[] currentState = x.LastSetState;
						IIntent[] newState = x.State.Select(y => y.Intent).ToArray();
						if(!currentState.SequenceEqual(newState)) { //*** test the effectiveness of this
							IEnumerable<IIntent> addedIntents = newState.Except(currentState);
							IEnumerable<IIntent> removedIntents = currentState.Except(newState);
							intentChanges = new IntentChangeCollection(addedIntents, removedIntents);
						}
						x.IntentChangeCollection = intentChanges;
						x.LastSetState = newState.ToArray();
					
						x.LogicalFiltering();
					});
					Module.UpdateState(ExtractFromOutputs(x => x.IntentChangeCollection).ToArray());
				} finally {
					EndOutputChange();
				}
			}
		}
	}
}
