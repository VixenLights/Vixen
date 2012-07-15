using Vixen.Module.Effect;
using Vixen.Sys;

namespace Vixen.Services {
	public class EffectService {
		static private EffectService _instance;

		private EffectService() {
		}

		public static EffectService Instance {
			get { return _instance ?? (_instance = new EffectService()); }
		}

		public IEffectModuleInstance Get(string effectName) {
			EffectModuleManagement manager = Modules.GetManager<IEffectModuleInstance, EffectModuleManagement>();
			return manager.Get(effectName);
		}
	}
}
