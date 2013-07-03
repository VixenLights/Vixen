using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using System.Runtime.Serialization;

namespace VixenModules.SequenceType.Vixen2x
{
    [DataContract]
    public class Vixen2xSequenceStaticData : ModuleDataModelBase
    {

       [DataMember]
        private List<ChannelMapping> _vixen2xMappings;
        
        public List<ChannelMapping> Vixen2xMappings
        {
            get
            {
                if(_vixen2xMappings == null)
                    _vixen2xMappings = new List<ChannelMapping>();
                return _vixen2xMappings;
            }
            set
            {
                _vixen2xMappings = value;
            }
        }

        public Vixen2xSequenceStaticData()
        {
            Vixen2xMappings = new List<ChannelMapping>();
        }

        public override IModuleDataModel Clone()
        {
            Vixen2xSequenceStaticData data = new Vixen2xSequenceStaticData();
            data.Vixen2xMappings = new List<ChannelMapping>(Vixen2xMappings);
            return data;
        }
    }
}
