using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.LayerMixingFilter.LumaKey
{
	public class LumaKeyData: ModuleDataModelBase
	{
		public LumaKeyData()
		{
			ExcludeZeroValues = true;
		    LowerLimit = 0;  //is this where I initialize the default values for a new instance?
		    UpperLimit = 100;
		}

		public override IModuleDataModel Clone()
		{
            //**** What does thos do?  how to include other vars? {ExcludeZeroValues = ExcludeZeroValues}
            LumaKeyData newInstance = new LumaKeyData {ExcludeZeroValues = ExcludeZeroValues};
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
