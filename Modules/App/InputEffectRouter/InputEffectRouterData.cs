using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using Vixen.Module.Input;

namespace VixenModules.App.InputEffectRouter {
	[DataContract]
	public class InputEffectRouterData : ModuleDataModelBase {
		private List<InputEffectMap> _map;
		private ModuleLocalDataSet _moduleData;
		private List<IInputModuleInstance> _inputModules;

		public override IModuleDataModel Clone() {
			InputEffectRouterData newInstance = new InputEffectRouterData();
			newInstance.Map = Map;
			newInstance.ModuleData = ModuleData.Clone() as ModuleLocalDataSet;
			return newInstance;
		}

		[DataMember]
		public IEnumerable<InputEffectMap> Map {
			get { return _map ?? (_map = new List<InputEffectMap>()); }
			set { _map = new List<InputEffectMap>(value); }
		}

		[DataMember]
		public ModuleLocalDataSet ModuleData {
			get { return _moduleData ?? (_moduleData = new ModuleLocalDataSet()); }
			set { _moduleData = value; }
		}

		// Need to do it this way so that there is a single instance created from the data
		// and so this member is kept up-to-date.
		//*** IModuleDataSet changed
		public IEnumerable<IInputModuleInstance> InputModules {
			get { return _inputModules ?? (_inputModules = ModuleData.GetInstances<IInputModuleInstance>().ToList()); }
			set {
				_inputModules = new List<IInputModuleInstance>(value);
				
				ModuleData.Clear();
				foreach(IInputModuleInstance inputModule in _inputModules) {
					ModuleData.AddModuleInstanceData(inputModule);
				}
			}
		}
	}
}
