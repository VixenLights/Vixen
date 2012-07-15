namespace VixenModules.Output.E131
{
    using System.Runtime.Serialization;

    using Vixen.Module;

    [DataContract]
    public class E131ModuleDataModel : ModuleDataModelBase
    {
        [DataMember]
        public int OutputCount { get; set; }

        public override IModuleDataModel Clone()
        {
            return new E131ModuleDataModel { OutputCount = OutputCount };
        }
    }
}
