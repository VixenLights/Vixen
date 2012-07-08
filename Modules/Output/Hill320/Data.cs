using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Output.Hill320
{
   [DataContract]
    class Data : ModuleDataModelBase
    {
        [DataMember]
        public ushort PortAddress { get; set; }
        [DataMember]
        public ushort ControlPort { get; set; }
        [DataMember]
        public ushort StatusPort { get; set; }
       

        public override IModuleDataModel Clone()
        {
            Data newInstance = new Data() { PortAddress = PortAddress };
            return newInstance;
        }
    }
}
