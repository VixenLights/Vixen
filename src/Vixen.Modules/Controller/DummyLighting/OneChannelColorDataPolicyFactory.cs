using Vixen.Sys;
using VixenModules.Output.DummyLighting;

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