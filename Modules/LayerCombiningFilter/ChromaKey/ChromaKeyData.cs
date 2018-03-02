using System.Drawing;
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
            KeyColor = Color.FromArgb(0,255,0);
		    HueTolerance = 5; //need to add setup form controls for this.
		    SaturationTolerance = HueTolerance/100; //change to something else if I split the H/S tolerances.
		}

		public override IModuleDataModel Clone()
		{
		    var newInstance = new ChromaKeyData
		    {
		        ExcludeZeroValues = ExcludeZeroValues, KeyColor = KeyColor,
                LowerLimit = LowerLimit, UpperLimit = UpperLimit, 
		        HueTolerance = HueTolerance, SaturationTolerance = SaturationTolerance
		    };
            return newInstance;
		}

		[DataMember]
		public bool ExcludeZeroValues { get; set; }

        [DataMember]
		public int LowerLimit { get; set; }

        [DataMember]
        public int UpperLimit { get; set; }

        [DataMember]
        public Color KeyColor { get; set; }

	    [DataMember]
	    public float HueTolerance { get; set; }

	    [DataMember]
	    public float SaturationTolerance { get; set; }
    }
}
