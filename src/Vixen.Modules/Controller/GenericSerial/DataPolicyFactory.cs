using Vixen.Sys;

namespace VixenModules.Output.GenericSerial
{
	internal class DataPolicyFactory : IDataPolicyFactory
	{
		public IDataPolicy CreateDataPolicy()
		{
			return new DataPolicy();
		}
	}
}