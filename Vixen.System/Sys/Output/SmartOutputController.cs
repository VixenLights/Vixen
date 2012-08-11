using System;
using System.Linq;
using Vixen.Data.Policy;
using Vixen.Module.SmartController;

namespace Vixen.Sys.Output {
	public class SmartOutputController : ModuleBasedController<ISmartControllerModuleInstance, IntentOutput> {
		private IntentOutputStates _outputCurrentStates;
		private SmartControllerDataPolicy _dataPolicy;
		private IntentOutputDataFlowAdapterFactory _adapterFactory;

		public SmartOutputController(string name, int outputCount, Guid moduleId)
			: this(Guid.NewGuid(), name, outputCount, moduleId) {
		}

		public SmartOutputController(Guid id, string name, int outputCount, Guid moduleId)
			: base(id, name, outputCount, moduleId) {
			_outputCurrentStates = new IntentOutputStates();
			_dataPolicy = new SmartControllerDataPolicy();
			_adapterFactory = new IntentOutputDataFlowAdapterFactory();
		}

		protected override ISmartControllerModuleInstance GetControllerModule(Guid moduleId) {
			return Modules.ModuleManagement.GetSmartController(moduleId);
		}

		protected override void UpdateState() {
			if(Module != null) {
				BeginOutputChange();
				try {
					Outputs.AsParallel().ForAll(x => {
						x.Update();
						x.IntentChangeCollection = _GenerateChangeCollection(x);

					//    x.UpdateState();

					////UpdateOutputStates(x => {
					//    IntentChangeCollection intentChanges = null;
					//    IIntent[] currentState = x.LastSetState;
					//    IIntent[] newState = x.State.Select(y => y.Intent).ToArray();
					//    if(!currentState.SequenceEqual(newState)) { //*** test the effectiveness of this
					//        IEnumerable<IIntent> addedIntents = newState.Except(currentState);
					//        IEnumerable<IIntent> removedIntents = currentState.Except(newState);
					//        intentChanges = new IntentChangeCollection(addedIntents, removedIntents);
					//    }
					//    x.IntentChangeCollection = intentChanges;
					//    x.LastSetState = newState.ToArray();
					
						//x.LogicalFiltering();
					});
					Module.UpdateState(ExtractFromOutputs(x => x.IntentChangeCollection).ToArray());
				} finally {
					EndOutputChange();
				}
			}
		}

		protected override void OutputAdded(object sender, OutputCollectionEventArgs<IntentOutput> e) {
			VixenSystem.DataFlow.AddComponent(_adapterFactory.GetAdapter(e.Output));
		}

		protected override void OutputRemoved(object sender, OutputCollectionEventArgs<IntentOutput> e) {
			VixenSystem.DataFlow.RemoveComponent(_adapterFactory.GetAdapter(e.Output));
		}

		private IntentChangeCollection _GenerateChangeCollection(IntentOutput output) {
			_dataPolicy.OutputCurrentState = _outputCurrentStates.GetOutputCurrentState(output);
			output.State.Dispatch(_dataPolicy);
			_outputCurrentStates.SetOutputCurrentState(output, _dataPolicy.OutputCurrentState);
			return _dataPolicy.Result;
		}
	}
}
