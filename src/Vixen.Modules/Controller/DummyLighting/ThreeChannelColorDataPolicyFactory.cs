using Vixen.Sys;
using VixenModules.Output.DummyLighting;

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