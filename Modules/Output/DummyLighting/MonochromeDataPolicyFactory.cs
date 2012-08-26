using Vixen.Sys;

namespace VixenModules.Controller.DummyLighting {
	class MonochromeDataPolicyFactory : IDataPolicyFactory {
		public IDataPolicy CreateDataPolicy() {
			return new MonochromeDataPolicy();
		}
	}
}
