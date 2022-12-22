using Vixen.Sys;

namespace VixenModules.Controller.DummyLighting
{
	internal class ThreeChannelColorDataPolicyFactory : IDataPolicyFactory
	{
		public IDataPolicy CreateDataPolicy()
		{
			return new ThreeChannelColorDataPolicy();
		}
	}
}