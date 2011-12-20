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
			newInstance.Map.AddRange(Map);
			newInstance.ModuleData = ModuleData.Clone() as ModuleLocalDataSet;
			return newInstance;
		}

		[DataMember]
		public List<InputEffectMap> Map {
			get { return _map ?? (_map = new List<InputEffectMap>()); }
			set { _map = value; }
		}

		[DataMember]
		public ModuleLocalDataSet ModuleData {
			get { return _moduleData ?? (_moduleData = new ModuleLocalDataSet()); }
			set { _moduleData = value; }
		}

		// Need to do it this way so that there is a single instance created from the data
		// and so this member is kept up-to-date.
		public IEnumerable<IInputModuleInstance> InputModules {
			get {
				if(_inputModules == null) {
					_inputModules = ModuleData.GetInstances<IInputModuleInstance>().ToList();
				}
				return _inputModules;
			}
			set {
				_inputModules.Clear();
				_inputModules.AddRange(value);
				
				ModuleData.Clear();
				foreach(IInputModuleInstance inputModule in _inputModules) {
					ModuleData.AddModuleInstanceData(inputModule);
				}
			}
		}
	}
}
