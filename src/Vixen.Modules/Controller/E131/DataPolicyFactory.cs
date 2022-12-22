using Vixen.Sys;
using VixenModules.Output.E131;

namespace VixenModules.Controller.E131
{
	internal class DataPolicyFactory : IDataPolicyFactory
	{
		public IDataPolicy CreateDataPolicy()
		{
			return new DataPolicy();
		}
	}
}