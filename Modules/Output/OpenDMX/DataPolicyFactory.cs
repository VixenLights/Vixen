using Vixen.Sys;

namespace VixenModules.Controller.OpenDMX
{
	class DataPolicyFactory : IDataPolicyFactory {
		public IDataPolicy CreateDataPolicy() {
			return new DataPolicy();
		}
	}
}
