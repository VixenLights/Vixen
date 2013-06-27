using Vixen.Sys;

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