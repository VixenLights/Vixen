using Vixen.Sys;
using VixenModules.Output.DummyLighting;

namespace VixenModules.Controller.DummyLighting
{
	internal class MonochromeDataPolicyFactory : IDataPolicyFactory
	{
		public IDataPolicy CreateDataPolicy()
		{
			return new MonochromeDataPolicy();
		}
	}
}