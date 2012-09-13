using Vixen.Sys;

namespace VixenModules.Controller.DummyLighting {
	class ThreeChannelColorDataPolicyFactory : IDataPolicyFactory {
		public IDataPolicy CreateDataPolicy() {
			return new ThreeChannelColorDataPolicy();
		}
	}
}
