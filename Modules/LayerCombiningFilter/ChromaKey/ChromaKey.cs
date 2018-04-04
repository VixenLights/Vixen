using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.LayerMixingFilter.ChromaKey
{
	public class ChromaKeyData: ModuleDataModelBase
	{
		public ChromaKeyData()
		{
			ExcludeZeroValues = true;
		    LowerLimit = 0;
		    UpperLimit = 100;
		}

		public override IModuleDataModel Clone()
		{
		    var newInstance = new ChromaKeyData
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
