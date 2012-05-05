using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Module.SmartController;

namespace Vixen.Sys.Output {
	public class SmartOutputController : ModuleBasedController<ISmartControllerModuleInstance, IntentOutput> {
		//private Guid _moduleId;
		//private ISmartControllerModuleInstance _module;
		//private ModuleLocalDataSet _dataSet;
		//private OutputCollection<IntentOutput> _outputs; 

		public SmartOutputController(string name, int outputCount, Guid moduleId)
			: this(Guid.NewGuid(), name, outputCount, moduleId) {
		}

		public SmartOutputController(Guid id, string name, int outputCount, Guid moduleId)
			//: base(id, name) {
			: base(id, name, outputCount, moduleId) {
			//_outputs = new OutputCollection<IntentOutput>(Id);
			//ModuleId = moduleId;
			//OutputCount = outputCount;
			//_dataSet = new ModuleLocalDataSet();
		}

		protected override ISmartControllerModuleInstance GetControllerModule(Guid moduleId) {
			return Modules.ModuleManagement.GetSmartController(moduleId);
		}

		//protected override void _Start() {
		//    if(Module != null) {
		//        Module.Start();
		//    }
		//}

		//protected override void _Stop() {
		//    if(Module != null) {
		//        Module.Stop();
		//    }
		//}

		//protected override void _Pause() {
		//    if(Module != null) {
		//        Module.Pause();
		//    }
		//}

		//protected override void _Resume() {
		//    if(Module != null) {
		//        Module.Resume();
		//    }
		//}

		protected override void _UpdateState() {
			if(Module != null) {
				//lock(_outputs) {
				BeginOutputChange();
				try {
					UpdateOutputStates(x => {
							IntentChangeCollection intentChanges = null;
							IEnumerable<IIntent> currentState = x.LastSetState;
							IEnumerable<IIntent> newState = x.State.Select(y => y.Intent);
							if(!currentState.SequenceEqual(newState)) { //*** test the effectiveness of this
								IEnumerable<IIntent> addedIntents = newState.Except(currentState);
								IEnumerable<IIntent> removedIntents = currentState.Except(newState);
								intentChanges = new IntentChangeCollection(addedIntents, removedIntents);
							}
							x.IntentChangeCollection = intentChanges;
							x.LastSetState = newState.ToArray();
						});
					Module.UpdateState(ExtractFromOutputs(x => x.IntentChangeCollection).ToArray());
				} finally {
					EndOutputChange();
				}
			}
		}

		//public IntentOutput[] Outputs {
		//    get { return _outputs.AsArray; }
		//}

		//public int OutputCount {
		//    get { return _outputs.Count; }
		//    set {
		//        _outputs.Count = value;
		//        _SetOutputModuleOutputCount();
		//    }
		//}

		//public Guid ModuleId {
		//    get { return _moduleId; }
		//    set {
		//        _moduleId = value;
		//        _module = null;
		//    }
		//}

		//public ISmartControllerModuleInstance Module {
		//    get {
		//        if(_module == null) {
		//            _module = Modules.ModuleManagement.GetSmartController(_moduleId);

		//            if(_module != null) {
		//                _SetModuleData();
		//                ResetUpdateInterval();
		//            }
		//        }
		//        return _module;
		//    }
		//}

		//public ModuleLocalDataSet ModuleDataSet {
		//    get { return _dataSet; }
		//    set {
		//        _dataSet = value;
		//        _SetModuleData();
		//    }
		//}

		//override public bool IsRunning {
		//    get { return _module != null && _module.IsRunning; }
		//}

		//public override bool IsPaused {
		//    get { return _module != null && _module.IsPaused; }
		//}

		//public void ResetUpdateInterval() {
		//    if(Module != null) {
		//        UpdateInterval = Module.UpdateInterval;
		//    }
		//}

		//override public bool HasSetup {
		//    get { return _module.HasSetup; }
		//}

		///// <summary>
		///// Runs the controller setup.
		///// </summary>
		///// <returns>True if the setup was successful and committed.  False if the user canceled.</returns>
		//override public bool Setup() {
		//    if(_module != null) {
		//        if(_module.Setup()) {
		//            return true;
		//        }
		//    }
		//    return false;
		//}

		//public override string ToString() {
		//    return Name;
		//}

		//private void _SetModuleData() {
		//    if(_module != null && ModuleDataSet != null) {
		//        ModuleDataSet.AssignModuleTypeData(_module);
		//    }
		//}

		//private void _SetOutputModuleOutputCount() {
		//    if(_module != null && OutputCount != 0) {
		//        _module.OutputCount = OutputCount;
		//    }
		//}
	}
}
