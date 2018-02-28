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
		}

		public override IModuleDataModel Clone()
		{
		    var newInstance = new ChromaKeyData
		    {
		        ExcludeZeroValues = ExcludeZeroValues,
		        LowerLimit = LowerLimit,
		        UpperLimit = UpperLimit                
		    };
		    newInstance.KeyColor = new Color();  //this doesn't work like Jeff's Example shows
		    newInstance.KeyColor = KeyColor;
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
    }
}
