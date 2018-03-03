using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.LayerMixingFilter.LumaKey
{
	public class LumaKeyData: ModuleDataModelBase
	{
		public LumaKeyData()
		{			
		    LowerLimit = 0;
		    UpperLimit = 100;
		}

		public override IModuleDataModel Clone()
		{
		    var newInstance = new LumaKeyData
		    {
		        LowerLimit = LowerLimit,
		        UpperLimit = UpperLimit
		    };
		    return newInstance;
		}

        [DataMember]
		public int LowerLimit { get; set; }

        [DataMember]
        public int UpperLimit { get; set; }
    }
}
