using Vixen.Sys;

namespace VixenModules.Output.DmxUsbPro
{
	internal class DataPolicyFactory : IDataPolicyFactory
	{
		public IDataPolicy CreateDataPolicy()
		{
			return new DataPolicy();
		}
	}
}