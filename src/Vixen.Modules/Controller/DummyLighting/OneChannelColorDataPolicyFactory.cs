using Vixen.Sys;

namespace VixenModules.Controller.DummyLighting
{
	internal class OneChannelColorDataPolicyFactory : IDataPolicyFactory
	{
		public IDataPolicy CreateDataPolicy()
		{
			return new OneChannelColorDataPolicy();
		}
	}
}