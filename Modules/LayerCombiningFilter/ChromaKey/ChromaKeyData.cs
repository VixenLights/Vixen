using System.Drawing;
using System.Runtime.Serialization;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Module;

namespace VixenModules.LayerMixingFilter.ChromaKey
{
	public class ChromaKeyData: ModuleDataModelBase
	{
		public ChromaKeyData()
		{
		    LowerLimit = 0;
		    UpperLimit = 1;
            KeyColor = Color.FromArgb(0,255,0);
			KeySaturation = HSV.FromRGB(KeyColor).S;
			KeyHue = KeyColor.GetHue();
		    HueTolerance = 5; 
		    SaturationTolerance = HueTolerance/100; 
		}

		public override IModuleDataModel Clone()
		{
		    var newInstance = new ChromaKeyData
		    {
		        KeyColor = KeyColor, KeySaturation = KeySaturation, KeyHue = KeyHue,
			    LowerLimit = LowerLimit, UpperLimit = UpperLimit, 
		        HueTolerance = HueTolerance, SaturationTolerance = SaturationTolerance
		    };
            return newInstance;
		}

        [DataMember]
		public double LowerLimit { get; set; }

        [DataMember]
        public double UpperLimit { get; set; }

        [DataMember]
        public Color KeyColor { get; set; }

		[DataMember]
		public double KeySaturation { get; set; }

		[DataMember]
		public float KeyHue { get; set; }

	    [DataMember]
	    public float HueTolerance { get; set; }

	    [DataMember]
	    public float SaturationTolerance { get; set; }
    }
}
