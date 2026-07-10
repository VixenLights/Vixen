using Vixen.Sys;

namespace VixenModules.Controller.OpenDMX
{
	internal class DataPolicyFactory : IDataPolicyFactory
	{
		public IDataPolicy CreateDataPolicy()
		{
			return new DataPolicy();
		}
	}
}