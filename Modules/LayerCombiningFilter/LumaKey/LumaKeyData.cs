using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.LayerMixingFilter.LumaKey
{
	public class LumaKeyData: ModuleDataModelBase
	{
		public LumaKeyData()
		{
			ExcludeZeroValues = true;
		    LowerLimit = 0;
		    UpperLimit = 100;
		}

		public override IModuleDataModel Clone()
		{
		    var newInstance = new LumaKeyData
		    {
		        ExcludeZeroValues = ExcludeZeroValues,
		        LowerLimit = LowerLimit,
		        UpperLimit = UpperLimit
		    };
		    return newInstance;
		}

		[DataMember]
		public bool ExcludeZeroValues { get; set; }

        [DataMember]
		public int LowerLimit { get; set; }

        [DataMember]
        public int UpperLimit { get; set; }
    }
}
