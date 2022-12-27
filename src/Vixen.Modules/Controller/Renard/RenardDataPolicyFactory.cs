using Vixen.Sys;

namespace VixenModules.Output.Renard
{
	internal class RenardDataPolicyFactory : IDataPolicyFactory
	{
		public IDataPolicy CreateDataPolicy()
		{
			return new RenardDataPolicy();
		}
	}
}