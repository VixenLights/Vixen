using Vixen.Sys;

namespace VixenModules.Controller.DummyLighting {
	class OneChannelColorDataPolicyFactory : IDataPolicyFactory {
		public IDataPolicy CreateDataPolicy() {
			return new OneChannelColorDataPolicy();
		}
	}
}
