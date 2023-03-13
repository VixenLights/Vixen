using Vixen.Sys;

namespace VixenModules.Output.LauncherController
{
	internal class DataPolicyFactory : IDataPolicyFactory {
		public IDataPolicy CreateDataPolicy() {
			return new DataPolicy();
		}
	}
}
