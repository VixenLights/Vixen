using System.Text;
using Vixen.Sys;

namespace VixenModules.Output.FGDimmer
{
    class FGDimmerDataPolicyFactory : IDataPolicyFactory
    {
        public IDataPolicy CreateDataPolicy()
        {
            return new FGDimmerDataPolicy();
        }
    }
}
