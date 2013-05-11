using Vixen.Sys;

namespace VixenModules.Output.GenericSerial
{
    class DataPolicyFactory : IDataPolicyFactory
    {
        public IDataPolicy CreateDataPolicy()
        {
            return new DataPolicy();
        }
    }
}
