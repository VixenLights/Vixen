using Vixen.Sys;

namespace VixenModules.Output.CommandController {
	internal class DataPolicyFactory : IDataPolicyFactory {
		public IDataPolicy CreateDataPolicy() {
			return new DataPolicy();
		}
	}
}
