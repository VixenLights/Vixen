using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.LayerMixingFilter.ChromaKey
{
	public class ChromaKeyData: ModuleDataModelBase
	{
		public ChromaKeyData()
		{
		    LowerLimit = 0;
		    UpperLimit = 100;
            KeyColor = Color.FromArgb(0,255,0);
		    HueTolerance = 5; 
		    SaturationTolerance = HueTolerance/100; 
		}

		public override IModuleDataModel Clone()
		{
		    var newInstance = new ChromaKeyData
		    {
		        KeyColor = KeyColor, LowerLimit = LowerLimit, UpperLimit = UpperLimit, 
		        HueTolerance = HueTolerance, SaturationTolerance = SaturationTolerance
		    };
            return newInstance;
		}

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
